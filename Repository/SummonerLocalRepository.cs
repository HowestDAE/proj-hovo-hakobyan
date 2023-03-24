using _2DAE15_HovhannesHakobyan_Exam.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.Repository
{
    public class SummonerLocalRepository : ISummonerRepository
    {
        private List<TopSummoner> _topSummoners;

        private async Task LoadTopSummoners()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "_2DAE15_HovhannesHakobyan_Exam.Resources.Data.topSummoners.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = await reader.ReadToEndAsync();

                    _topSummoners = JsonConvert.DeserializeObject<List<TopSummoner>>(json);
                }
            }
        }

        public async Task<List<TopSummoner>> GetTopSummonersAsync()
        {
            if(_topSummoners == null)
            {
                await LoadTopSummoners();
            }

            return _topSummoners;
        }
    }
}
