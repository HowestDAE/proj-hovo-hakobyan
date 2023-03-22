using _2DAE15_HovhannesHakobyan_Exam.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.Repository
{
    public class SummonerAPIRepository
    {
        private List<Summoner> _topSummoners;
        private string _apiKey = "RGAPI-407a2e80-c7fe-474c-8c26-e2826e304006";
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
                    _topSummoners = JsonConvert.DeserializeObject<List<Summoner>>(jsonObject["entries"].ToString());
                    _topSummoners = _topSummoners.OrderByDescending(s => s.LeaguePoints).ToList();
                }
                catch (Exception ex)
                {
               
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task<List<Summoner>> GetTopSummonersAsync(bool reloadData)
        {
            if (_topSummoners ==null || reloadData)
            {
                await LoadTopSummonersAsync();
            }

            return _topSummoners;
        }
    }
}
