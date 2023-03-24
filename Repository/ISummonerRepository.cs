using _2DAE15_HovhannesHakobyan_Exam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.Repository
{
    public interface ISummonerRepository
    {
        Task<List<TopSummoner>> GetTopSummonersAsync();
    }
}
