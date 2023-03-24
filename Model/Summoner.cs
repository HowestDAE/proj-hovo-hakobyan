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
        [JsonProperty("summonerId")]
        public string Id { get; set; }


        [JsonIgnore]
        public SummonerInfo SummonerInfo { get; set; }

        [JsonIgnore]
        public RankInfo RankInfo { get; set; }

        [JsonIgnore]
        public string ProfileIconUrl
        {
            get
            {
                return $"http://ddragon.leagueoflegends.com/cdn/13.6.1/img/profileicon/{SummonerInfo.ProfileIconId}.png";
            }
        }

        [JsonIgnore]
        public List<MasteryInfo> MasteryInfos { get; set; }
    }

    public class TopSummoner : Summoner
    {
        [JsonProperty("leaguePoints")]
        public int LeaguePoints { get; set; }

        [JsonIgnore]
        public int LadderRank { get; set; }
    }

    public class SummonerInfo
    {
        [JsonProperty("profileIconId")]
        public int ProfileIconId { get; set; }

        [JsonProperty("summonerLevel")]
        public int Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class RankInfo
    {
        [JsonProperty("queueType")]
        public string QueueType { get; set; }

        [JsonProperty("tier")]
        public string Tier { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }
    }

    public class MasteryInfo
    {
        [JsonProperty("championId")]
        public int ChampoinId { get; set; }

        [JsonProperty("championLevel")]
        public int ChampionLevel { get; set; }

        [JsonProperty("championPoints")]
        public string ChampionPoints { get; set; }

        private ChampionInfo _champInfo = new ChampionInfo();
        [JsonIgnore]
        public ChampionInfo ChampionInfo
        {
            get { return _champInfo; }
            set { _champInfo = value; }
        }
    }

    public class ChampionInfo
    {
        public string Name { get; set; }

        public string Title { get; set; }

        [JsonIgnore]
        public string ProfileIconUrl
        {
            get
            {
                return $"http://ddragon.leagueoflegends.com/cdn/13.6.1/img/champion/{Name}.png";
            }
        }
    }
}
