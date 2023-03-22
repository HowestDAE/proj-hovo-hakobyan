using _2DAE15_HovhannesHakobyan_Exam.Model;
using _2DAE15_HovhannesHakobyan_Exam.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class OverviewVM : ObservableObject
    {
        private SummonerAPIRepository _summonerAPIRepository;
        public List<Summoner> Summoners { get; set; }

        public SummonerAPIRepository SummonerAPIRepository
        {
            get { return _summonerAPIRepository; }
        }

        public OverviewVM()
        {
            _summonerAPIRepository = new SummonerAPIRepository();
            GetTopPlayersAsync();
        }

        private async void GetTopPlayersAsync()
        {
            Summoners = await _summonerAPIRepository.GetTopSummonersAsync(false);
            OnPropertyChanged(nameof(Summoners));
        }
    }
}
