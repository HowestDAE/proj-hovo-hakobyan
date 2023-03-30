using _2DAE15_HovhannesHakobyan_Exam.Model;
using _2DAE15_HovhannesHakobyan_Exam.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class SearchVM : ObservableObject
    {
        private const string _searchHistoryFile = "../../Resources/Data/searchHistory.txt";

        public string SearchedName { get; set; }
        private string _selectedHistoryName;
        public string SelectedName
        {
            get
            {
                return _selectedHistoryName;
            }
            set
            {
                _selectedHistoryName = value;
                SearchedName = value;
                LookForSummoner();
            }
        }
        public bool IsLoading { get;set; }
        public string StatusMessage { get; set; }
        public List<string> SearchHistory { get; set; }

        public RelayCommand LookForSummonerCommand { get; private set; }
        private SummonerAPIRepository _summonerRepository;
        public event EventHandler<Summoner> ShowDetailsRequest;
        public SearchVM()
        {
            _summonerRepository = new SummonerAPIRepository();
            LookForSummonerCommand = new RelayCommand(LookForSummoner);
            StatusMessage = "Look for an EUW Summoner";
            OnPropertyChanged(nameof(StatusMessage));

            try
            {
                SearchHistory = File.ReadAllLines(_searchHistoryFile).ToList();
                OnPropertyChanged(nameof(SearchHistory));
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
           
        }

        private async void LookForSummoner()
        {
            IsLoading = true;
            OnPropertyChanged(nameof(IsLoading));      
            try
            {
                StatusMessage = $"Looking for {SearchedName} ...";
                OnPropertyChanged(nameof(StatusMessage));
                Task<Summoner> summonerTask = _summonerRepository.GetSummonerAsync(SearchedName);
                ShowDetailsRequest?.Invoke(this, await summonerTask);
                SaveSearchHistory(SearchedName);
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                OnPropertyChanged(nameof(StatusMessage));
            }
            finally
            {
                SearchedName = string.Empty;
                OnPropertyChanged(nameof(SearchedName));
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));

                await Task.Delay(2000);
                StatusMessage = "Look for an EUW Summoner";
                OnPropertyChanged(nameof(StatusMessage));

            }
        }

        private void SaveSearchHistory(string searchedName)
        {
            List<string> updatedSearchHistory = File.ReadAllLines(_searchHistoryFile).ToList();
            updatedSearchHistory.Insert(0, searchedName);

            // Remove oldest search if search history exceeds 5
            if (updatedSearchHistory.Count > 5)
            {
                updatedSearchHistory.RemoveAt(5);
            }

            // Write updated search history to file
            File.WriteAllLines(_searchHistoryFile, updatedSearchHistory);

            SearchHistory= updatedSearchHistory;
            OnPropertyChanged(nameof(SearchHistory));

        }

    }
}
