using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

            // this.RestoreLastSession();

            this.DataContext = _sessionManager;
        }

        private void RestoreLastSession()
        {
            if (_sessionManager.Sessions.Any())
            {
                _sessionManager.CurrentSession = _sessionManager.Sessions.Last();
            }
            else
            {
                // Set a default current session

                _sessionManager.CurrentSession = new CopilotChatViewModel() 
                {
                    Metadata = new ChatSessionMeta()
                };
            }
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
                return Brushes.White;
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}