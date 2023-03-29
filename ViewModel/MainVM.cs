using _2DAE15_HovhannesHakobyan_Exam.Model;
using _2DAE15_HovhannesHakobyan_Exam.Repository;
using _2DAE15_HovhannesHakobyan_Exam.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Controls;


namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class MainVM : ObservableObject
    {
        public Page CurrentPage { get; set; }
        ValidationPage ValidationPage { get; set; }
        OverviewPage OverviewPage { get; set; }
        DetailsPage DetailsPage { get; set; }
        MenuPage MenuPage { get; set; }
        SearchPage SearchPage { get; set; }
        public RelayCommand SwitchToMainPageCommand { get; private set; }

        public MainVM()
        {
            ValidationPage = new ValidationPage();
            DetailsPage = new DetailsPage();
            MenuPage = new MenuPage();
            SearchPage = new SearchPage();
            SwitchToMainPageCommand = new RelayCommand(SwitchToMainPage);
            CurrentPage = ValidationPage;

           SearchVM searchVM = SearchPage.DataContext as SearchVM;
            if (searchVM != null)
            {
                searchVM.ShowDetailsRequest += ShowDetailsRequest;
            }

            //Get the MenuVM to subscribe to NavigateToPage
            MenuVM menuVM = MenuPage.DataContext as MenuVM;
            if (menuVM != null)
            {
                menuVM.NavigateToPage += MenuVM_NavigateToPage;
            }

            ValidationVM validationVM = ValidationPage.DataContext as ValidationVM;
            if(validationVM != null)
            {
                validationVM.APIKeyApprovedEvent += ValidationVM_APIKeyApproved;
            }
        }
        private void SwitchToMainPage()
        {
            CurrentPage = MenuPage;

            OnPropertyChanged(nameof(CurrentPage));
        }

        private void ShowDetailsRequest(object sender, Summoner currentSummoner)
        {
            CurrentPage = DetailsPage;

            (DetailsPage.DataContext as DetailsVM).CurrentSummoner = currentSummoner;

            OnPropertyChanged(nameof(CurrentPage));
        }

        private void ValidationVM_APIKeyApproved(object sender, EventArgs eventArgs)
        {
            //Creating this page when API key is approves
            //Guarantees that we send all the requests to RIOT API with a valid API key
            OverviewPage = new OverviewPage();
            // Get the OverviewVM from the OverviewPage to subscribe to ShowDetails
            OverviewVM overviewVM = OverviewPage.DataContext as OverviewVM;
            if (overviewVM != null)
            {
                // Subscribe to the ShowDetailsRequested event
                overviewVM.ShowDetailsRequest += ShowDetailsRequest;
            }

            CurrentPage = MenuPage;

            OnPropertyChanged(nameof(CurrentPage));
        }

        private void MenuVM_NavigateToPage(object sender, string page)
        {
            if (page.Equals("leaderboard"))
            {
                CurrentPage = OverviewPage;
                OnPropertyChanged(nameof(CurrentPage));
            }
            else if (page.Equals("search"))
            {
                CurrentPage = SearchPage;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

       
    }
}
