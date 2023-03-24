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
        }

        private void SwitchPage()
        {
            //If we are in Main page
            if (CurrentPage is OverviewPage)
            {
                //Go back to prev
            }
            else if(CurrentPage is DetailsPage)
            {
                CurrentPage = OverviewPage;

            }
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
}
