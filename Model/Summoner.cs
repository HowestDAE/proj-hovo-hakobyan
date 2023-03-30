using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.Model
{
    public class Summoner
    {
        [JsonProperty("summonerId")]
        public string Id { get; set; }


        public SummonerInfo SummonerInfo { get; set; }

        public RankInfo RankInfo { get; set; }

        [JsonIgnore]
        public string ProfileIconUrl
        {
            get
            {
                return $"http://ddragon.leagueoflegends.com/cdn/13.6.1/img/profileicon/{SummonerInfo.ProfileIconId}.png";
            }
        }

        public List<MasteryInfo> MasteryInfos { get; set; }

        public Summoner()
        {
            Id = string.Empty;
            SummonerInfo = new SummonerInfo();
            RankInfo = new RankInfo();
            MasteryInfos = new List<MasteryInfo>();
        }
    }

    public class TopSummoner : Summoner
    {
        [JsonProperty("leaguePoints")]
        public int LeaguePoints { get; set; }

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

        private string _championPoints;
        [JsonProperty("championPoints")]
        public string ChampionPoints
        {
            get 
            {
                int championPoints = 0;
                bool canParse = int.TryParse(_championPoints, out championPoints);
                if (canParse)
                {
                    return championPoints.ToString("N0");
                }

                return _championPoints;

            }
            set
            {
                _championPoints = value;
            }
        }

        private ChampionInfo _champInfo = new ChampionInfo();


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
        public string ChampionIconUrl
        {
            get
            {
                //This will work for most champion Names
                //The way they built this endpoint is really inconsistent
                //You are supposed to remove all the symbols from the name, e.g Kai'sa becomes Kaisa, Rek'sai becomes Reksai
                //But some names, they use Pascal case, for example RekSai works, but Reksai doesn't work
                //For some names they don't use Pascal case, for example Kaisa works, but KaiSa doesn't work
                //And some champions are called by their nicknames, for example Wukong is MonkeyKing
                //Some contain numbers in them
                string correctName = Regex.Replace(Name, "[^a-zA-Z]+", "");
                return $"http://ddragon.leagueoflegends.com/cdn/13.6.1/img/champion/{correctName}.png";
            }
        }
    }
}
