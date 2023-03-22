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

namespace _2DAE15_HovhannesHakobyan_Exam.Repository
{
    public class SummonerAPIRepository
    {
        private List<TopSummoner> _topSummoners;
        private string _apiKey = "RGAPI-407a2e80-c7fe-474c-8c26-e2826e304006";
        private int _nrTopPlayers = 15;

        private async Task LoadTopSummonersAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);
                string endpoint = "https://euw1.api.riotgames.com/lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5";

                try
                {
                   var response = await client.GetAsync(endpoint);

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
                        parallelTasks.Add(LoadSummonerInfoFromIdAsync(summoner.Id, summoner));
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

        private async Task LoadSummonerInfoFromIdAsync(string id,Summoner outSummoner )
        {
            string endpoint = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/{id}";
            //no return value because outSummoner gets it's property values from different endpoints
            await LoadSummonerInfoAsync(endpoint, outSummoner);       
        }

        private async Task LoadSummonerInfoFromNameAsync(string name, Summoner outSummoner)
        {
            string endpoint = $"https://euw1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{name}";
            await LoadSummonerInfoAsync(endpoint , outSummoner);
        }

        private async Task LoadSummonerInfoAsync(string endpoint, Summoner outSummoner)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);
                await Task.Delay(3000);
                try
                {
                    var response = await client.GetAsync(endpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }

                    string json = await response.Content.ReadAsStringAsync();

                    outSummoner.SummonerInfo = JsonConvert.DeserializeObject<SummonerInfo>(json);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }
        }

       


        public async Task<List<TopSummoner>> GetTopSummonersAsync(bool reloadData)
        {
            if (_topSummoners ==null || reloadData)
            {
                await LoadTopSummonersAsync();
            }

            return _topSummoners;
        }
    }
}
