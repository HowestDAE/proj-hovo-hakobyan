using _2DAE15_HovhannesHakobyan_Exam.Model;
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
        public List<Summoner> Summoners { get; set; }
    }
}
