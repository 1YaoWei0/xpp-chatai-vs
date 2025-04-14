using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using xpp_chatai_vs.Model;
using xpp_chatai_vs.ViewModel;

namespace xpp_chatai_vs.View
{
    /// <summary>
    /// Interaction logic for CopilotWindowControl.
    /// Willie Yao - 04/14/2025
    /// </summary>
    public partial class CopilotWindowControl : UserControl
    {
        private CopilotSessionViewModel _sessionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopilotWindowControl"/> class.
        /// Willie Yao - 04/14/2025
        /// </summary>
        public CopilotWindowControl()
        {
            var _ = new MdXaml.MarkdownScrollViewer();

            this.InitializeComponent();

            _sessionManager = CopilotWindowPackage.Instance.CopilotSessionViewModel;

            this.DataContext = _sessionManager;

            _sessionManager.CurrentSession.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        /// <summary>
        /// The event handler for messages collections' changed
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">NotifyCollectionChangedEventArgs</param>
        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessagesScrollViewer.ScrollToEnd();
                }), DispatcherPriority.ContextIdle);
            }
        }
    }

    /// <summary>
    /// Alignment converter
    /// Willie Yao - 04/14/2025
    /// </summary>
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

    /// <summary>
    /// Foreground converter
    /// Willie Yao - 04/14/2025
    /// </summary>
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

    /// <summary>
    /// Percentage converter
    /// Willie Yao - 04/14/2025
    /// </summary>
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

    /// <summary>
    /// Background converter
    /// Willie Yao - 04/14/2025
    /// </summary>
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