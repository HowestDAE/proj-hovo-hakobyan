using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.Model
{
    public class Summoner
    {
        [JsonProperty("summonerName")]
        public string Name { get; set; }

        [JsonProperty("leaguePoints")]
        public int LeaguePoints { get; set; }

        [JsonProperty("summonerId")]
        public string Id { get; set; }
    }
}
