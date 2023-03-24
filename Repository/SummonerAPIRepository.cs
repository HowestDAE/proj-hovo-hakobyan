using _2DAE15_HovhannesHakobyan_Exam.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Reflection;
using System.Net;
using System.Xml.Linq;
using System.Windows.Markup;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace _2DAE15_HovhannesHakobyan_Exam.Repository
{
    public class SummonerAPIRepository : ISummonerRepository
    {
        private List<TopSummoner> _topSummoners;
        private string _apiKey = "RGAPI-99a17753-df9a-4076-8e43-d37956fe50f5";
        private int _nrTopPlayers = 15;

        //To avoid having Too Many Request exceptions
        private int _delayBetweenRequestsMs = 1000;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(15,15);


        private async Task LoadTopSummonersAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);
                string endpoint = "https://euw1.api.riotgames.com/lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5";

                try
                {                   
                    await _semaphore.WaitAsync();
                    var response = await client.GetAsync(endpoint);             
                    await Task.Delay(_delayBetweenRequestsMs);
                    _semaphore.Release();

                    if (!response.IsSuccessStatusCode)
                    { 
                        throw new HttpRequestException(response.ReasonPhrase); 
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(json);
                    _topSummoners = JsonConvert.DeserializeObject<List<TopSummoner>>(jsonObject["entries"].ToString());

                    //Order the summoners based on how much league points the have
                    _topSummoners = _topSummoners.OrderByDescending(s => s.LeaguePoints).ToList();

                    // Get only the best players
                    _topSummoners = _topSummoners.Take(_nrTopPlayers).ToList();

                    //Fill in the Rank property (cannot be retrieved from the api)
                    _topSummoners = _topSummoners.Select((summoner,index) => { summoner.LadderRank = index + 1; return summoner; }).ToList();

                    //Fill in the summoner info that are retrieved from different endpoints

                    // Get a list of tasks that load summoner info for each TopSummoner object
                    List<Task> parallelTasks = new List<Task>();

                    foreach (var summoner in _topSummoners)
                    {
                        parallelTasks.Add(LoadSummonerInfoAsync(summoner));
                    }
                    // Wait for all tasks to complete
                    await Task.WhenAll(parallelTasks);
                }
                catch (Exception ex)
                {               
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task LoadSummonerInfoAsync(Summoner outSummoner)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);

                await Task.WhenAll(
                     LoadLeagueV4EntriesAsync(outSummoner, client),
                     LoadSummonerV4Async(outSummoner, client),
                     LoadChampionMasteryV4Async(outSummoner, client)
                    );
               

                //Important
                //Can only be executre after the above endpoint calls are made
                await LoadChampionInfoAsync(outSummoner, client);
            }
        }

        //Api call to League-V4-Entries endpoint
        private async Task LoadLeagueV4EntriesAsync(Summoner outSummoner, HttpClient client)
        {
            try
            {
                await _semaphore.WaitAsync();
                //League-v4-entries endpoint call
                string endpoint = $"https://euw1.api.riotgames.com/lol/league/v4/entries/by-summoner/{outSummoner.Id}";
                HttpResponseMessage response = await client.GetAsync(endpoint);

                await Task.Delay(_delayBetweenRequestsMs);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }

                string json = await response.Content.ReadAsStringAsync();
                List<RankInfo> allRanksInfo = JsonConvert.DeserializeObject<List<RankInfo>>(json);
                var rankedSolo5v5Data = allRanksInfo.Where(d => d.QueueType == "RANKED_SOLO_5x5").FirstOrDefault();
                outSummoner.RankInfo = rankedSolo5v5Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown when pulling from League-V4-entries endpoint: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //API call to Summoner-V4 endpoint
        private async Task LoadSummonerV4Async(Summoner outSummoner, HttpClient client)
        {
            try
            {
                await _semaphore.WaitAsync();
                HttpResponseMessage response;
                string endpoint = string.Empty;
                //Summoner-v4 endpoit call
                //These 2 endpoints return the same thing, but sometimes we only have the name and sometimes we only have the id
                if (outSummoner.Id != null)
                {
                    endpoint = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/{outSummoner.Id}";
                    response = await client.GetAsync(endpoint);
                }
                else if (outSummoner.SummonerInfo.Name != string.Empty)
                {
                    endpoint = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{outSummoner.SummonerInfo.Name}";
                    response = await client.GetAsync(endpoint);
                }
                else
                {
                    throw new Exception("Id or Name should be know in order to extract summoner info");
                }

                await Task.Delay(_delayBetweenRequestsMs);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }

                string json = await response.Content.ReadAsStringAsync();
                outSummoner.SummonerInfo = JsonConvert.DeserializeObject<SummonerInfo>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown when pulling from Summoner-v4 endpoint: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
           
        }

        private async Task LoadChampionMasteryV4Async(Summoner outSummoner, HttpClient client)
        {
            try
            {
                await _semaphore.WaitAsync();
                const int champCount = 5;

                string endpoint = $"https://euw1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-summoner/{outSummoner.Id}/top?count={champCount}";
                var response = await client.GetAsync(endpoint);

                await Task.Delay(_delayBetweenRequestsMs);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }

                string json = await response.Content.ReadAsStringAsync();
                outSummoner.MasteryInfos = JsonConvert.DeserializeObject<List<MasteryInfo>>(json);
             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown when pulling from Champion-Mastery-V4 endpoint: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task LoadChampionInfoAsync(Summoner outSummoner, HttpClient client)
        {
            try
            {
                await _semaphore.WaitAsync();

                string endpoint = $"http://ddragon.leagueoflegends.com/cdn/13.6.1/data/en_US/champion.json";
                var response = await client.GetAsync(endpoint);

                await Task.Delay(_delayBetweenRequestsMs);


                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }

                string json = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON into a JObject
                JObject championList = JObject.Parse(json);


                for (int i = 0; i < outSummoner.MasteryInfos.Count; i++)
                {
                    int key = outSummoner.MasteryInfos[i].ChampoinId;

                    //Get the matching champion
                    JToken championToken = championList["data"].Children().FirstOrDefault(c => c.First["key"].ToString() == key.ToString())?.First;

                    if(championToken!=null)
                    {
                        outSummoner.MasteryInfos[i].ChampionInfo.Name = (string)championToken.SelectToken("name");
                        outSummoner.MasteryInfos[i].ChampionInfo.Title = (string)championToken.SelectToken("title");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //This method serialized the _topSummoners into a local JSON 
        private void SerializeTopSummoners()
        {
            if (_topSummoners == null)
                return;

            string json = JsonConvert.SerializeObject(_topSummoners, Formatting.Indented);

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","..", "Resources", "Data", "topSummoners.json");

            System.IO.File.WriteAllText(filePath, json);

        }

        public async Task<List<TopSummoner>> GetTopSummonersAsync()
        {
            if (_topSummoners ==null)
            {
                await LoadTopSummonersAsync();
                SerializeTopSummoners();
            }
           

            return _topSummoners;
        }
    }
}
