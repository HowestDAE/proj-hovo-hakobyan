using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace _2DAE15_HovhannesHakobyan_Exam.View.Converters
{
    internal class RankToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rank = value.ToString().ToLower();
            return new BitmapImage(new Uri($"pack://application:,,,/Resources/RankedEmblem/emblem-{rank}.png", UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
