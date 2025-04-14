using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xpp_chatai_vs.ViewModel;

namespace xpp_chatai_vs.View
{
    /// <summary>
    /// Interaction logic for KnowledgeBaseWindowControl.
    /// </summary>
    public partial class KnowledgeBaseWindowControl : UserControl
    {
        private KnowledgeBaseViewModel _knowledgeBaseViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnowledgeBaseWindowControl"/> class.
        /// </summary>
        public KnowledgeBaseWindowControl()
        {
            this.InitializeComponent();

            _knowledgeBaseViewModel = CopilotWindowPackage.Instance.KnowledgeBaseViewModel;

            this.DataContext = _knowledgeBaseViewModel;
        }        
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public static BoolToVisibilityConverter Instance { get; } = new BoolToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool boolValue = (bool)value;

            var invert = ParseInvertParameter(parameter);

            return boolValue ^ invert ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibilityValue = (Visibility) value;

            var invert = ParseInvertParameter(parameter);

            return (visibilityValue == Visibility.Visible) ^ invert;
        }

        private static bool ParseInvertParameter(object parameter)
        {
            if (parameter == null) return false;

            if (parameter is bool) return (bool) parameter;

            switch (parameter)
            {
                case "inverse":
                case "reverse":
                case "true": return true;
                case "false": return false;
            }

            switch (parameter)
            {
                case 1: return true;
                case 2: return false;
            }

            return bool.TryParse(parameter.ToString(), out var result) && result;
        }
    }
}