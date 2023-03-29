using _2DAE15_HovhannesHakobyan_Exam.Model;
using _2DAE15_HovhannesHakobyan_Exam.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private ISummonerRepository _summonerRepository;

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
            get { return _summonerRepository; }
        }

        public event EventHandler ShowDetailsRequest;

        private TopSummoner _selectedSummoner;
        public TopSummoner SelectedSummoner
        {
            get 
            {
                return _selectedSummoner;
            }
            set
            {
                _selectedSummoner = value;
                ShowDetail();
            }
        }

        public OverviewVM()
        {
            _summonerRepository = new SummonerAPIRepository();

            //Loading
            LoadingText = "Loading, please wait...";
            GetTopPlayersAsync();

        }

        private async void GetTopPlayersAsync()
        {
            //If we can't read the data from the api, switch to local
            try
            {
                TopSummoners = await _summonerRepository.GetTopSummonersAsync();
                Console.WriteLine("Using API");
            }
            catch (Exception)
            {
                _summonerRepository = new SummonerLocalRepository();
                TopSummoners = await _summonerRepository.GetTopSummonersAsync();
                Console.WriteLine("Using Local");
            }

            OnPropertyChanged(nameof(TopSummoners));
            LoadingText = string.Empty;
        }

        private void ShowDetail()
        {
            //Used to let the main vm know about this
            ShowDetailsRequest?.Invoke(this, EventArgs.Empty);

        }
    }
}
