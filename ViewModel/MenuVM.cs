using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DAE15_HovhannesHakobyan_Exam.ViewModel
{
    public class MenuVM :ObservableObject
    {
        public RelayCommand ShowLeaderboardCommand { get; private set; }
        public RelayCommand ShowSearchCommand { get; private set; }

        public event EventHandler<string> NavigateToPage;
        public MenuVM()
        {
            ShowLeaderboardCommand = new RelayCommand(ShowLeaderboard);
            ShowSearchCommand = new RelayCommand(ShowSearch);
        }

        void ShowLeaderboard()
        {
            NavigateToPage?.Invoke(this, "leaderboard");
        }

        void ShowSearch()
        {
            NavigateToPage?.Invoke(this, "search");
        }
    }
}
