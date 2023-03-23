﻿using Newtonsoft.Json;
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
        public string ImageUrl
        {
            get
            {
                return $"http://ddragon.leagueoflegends.com/cdn/13.6.1/img/profileicon/{SummonerInfo.ProfileIconId}.png";
            }
        }
       
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
}
