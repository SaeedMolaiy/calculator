using System.Windows;
using System.Windows.Media;

namespace Calculator
{
    public partial class App : Application
    {
        public static SolidColorBrush OperationButtonBrush => GetBrushByName("OperationButtonBrush");

        public static SolidColorBrush OperationButtonClickBrush => GetBrushByName("OperationButtonClickBrush");

        public static SolidColorBrush NumberButtonBrush => GetBrushByName("NumberButtonBrush");

        public static SolidColorBrush NumberButtonClickBrush => GetBrushByName("NumberButtonClickBrush");

        public static SolidColorBrush EqualButtonBrush => GetBrushByName("EqualButtonBrush");

        public static SolidColorBrush EqualButtonClickBrush => GetBrushByName("EqualButtonClickBrush");

        private static SolidColorBrush GetBrushByName(string brushName) =>
            (SolidColorBrush)Current.Resources[brushName];
    }
}