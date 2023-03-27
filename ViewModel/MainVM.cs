using _2DAE15_HovhannesHakobyan_Exam.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class MainVM :ObservableObject
    {
        public Page CurrentPage { get; set; }
        public RelayCommand SwitchPageCommand { get; private set; }
        OverviewPage OverviewPage { get; set; }
        DetailsPage DetailsPage { get; set; }
        

        public MainVM()
        {
            SwitchPageCommand = new RelayCommand(SwitchPage);
            OverviewPage = new OverviewPage();
            DetailsPage = new DetailsPage();
            CurrentPage = OverviewPage;

            // Get the OverviewVM from the OverviewPage to subscribe to ShowDetails
            OverviewVM overviewVM = OverviewPage.DataContext as OverviewVM;
            if (overviewVM != null)
            {
                // Subscribe to the ShowDetailsRequested event
                overviewVM.ShowDetailsRequest += OverviewVM_ShowDetailsRequest;
            }
        }

        private void SwitchPage()
        {
   
        }

        private void OverviewVM_ShowDetailsRequest(object sender, EventArgs e)
        {
            CurrentPage = DetailsPage;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
}
