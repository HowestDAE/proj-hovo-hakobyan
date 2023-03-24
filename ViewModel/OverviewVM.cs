using _2DAE15_HovhannesHakobyan_Exam.Model;
using _2DAE15_HovhannesHakobyan_Exam.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class OverviewVM : ObservableObject
    {
        private ISummonerRepository _summonerAPIRepository;
       
        private string _loadingText;
        public string LoadingText
        {
            get { return _loadingText; }
            set
            {
                _loadingText = value;
                OnPropertyChanged(nameof(LoadingText));
            }
        }

        public List<TopSummoner> TopSummoners { get; set; }

        public ISummonerRepository SummonerAPIRepository
        {
            get { return _summonerAPIRepository; }
        }

        public OverviewVM()
        {
            //_summonerAPIRepository = new SummonerAPIRepository();
            _summonerAPIRepository = new SummonerLocalRepository();

            //Loading
            LoadingText = "Loading, please wait...";
            GetTopPlayersAsync();
         
        }

        private async void GetTopPlayersAsync()
        {
            TopSummoners = await _summonerAPIRepository.GetTopSummonersAsync();
            OnPropertyChanged(nameof(TopSummoners));
            LoadingText = string.Empty;
        }
    }
}
