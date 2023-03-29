using _2DAE15_HovhannesHakobyan_Exam.Model;
using _2DAE15_HovhannesHakobyan_Exam.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class SearchVM : ObservableObject
    {
        public string SearchedName { get; set; }
        public bool IsLoading { get;set; }

        public RelayCommand LookForSummonerCommand { get; private set; }
        private SummonerAPIRepository _summonerRepository;
        public event EventHandler<Summoner> ShowDetailsRequest;
        public SearchVM()
        {
            _summonerRepository = new SummonerAPIRepository();
            LookForSummonerCommand = new RelayCommand(LookForSummoner);
        }

        private async void LookForSummoner()
        {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));
            try
            {
                Task<Summoner> summonerTask = _summonerRepository.GetSummonerAsync(SearchedName);
                ShowDetailsRequest?.Invoke(this, await summonerTask);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't find summoner,");
                
            }
            finally
            {

                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));

            }
        }

    }
}
