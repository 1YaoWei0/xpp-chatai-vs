using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using xpp_chatai_vs.Model;
using xpp_chatai_vs.ViewModel;

namespace xpp_chatai_vs.View
{
    /// <summary>
    /// Interaction logic for CopilotWindowControl.
    /// </summary>
    public partial class CopilotWindowControl : UserControl
    {
        private CopilotSessionViewModel _sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopilotWindowControl"/> class.
        /// </summary>
        public CopilotWindowControl()
        {
            var _ = new MdXaml.MarkdownScrollViewer();

            this.InitializeComponent();

            _sessionManager = CopilotWindowPackage.Instance.CopilotSessionViewModel;

            this.DataContext = _sessionManager;
        }
    }

    public class AlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MessageType messageType = (MessageType)value;

            if (messageType == MessageType.UserInput) 
            {
                return HorizontalAlignment.Right;
            }

            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class ForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MessageType messageType = (MessageType)value;

            if (messageType == MessageType.UserInput)
            {
                return System.Windows.Media.Brushes.White;
            }

            return System.Windows.Media.Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string percentageStr &&
                double.TryParse(percentageStr, NumberStyles.Any, culture, out double percentage))
            {
                return width * percentage;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MessageType messageType = (MessageType)value;

            if (messageType == MessageType.UserInput)
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 120, 215));
            }

            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}