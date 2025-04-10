using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace xpp_chatai_vs.Model
{
    public enum MessageType
    {
        UserInput,
        AIResponse,
        SystemAlert,
        CodeSnippet
    }

    public class ChatMessage : INotifyPropertyChanged
    {                      
        private string _content;

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged();
            }
        }

        private DateTime _timestamp = DateTime.Now;
        
        public MessageType MessageType { get; set; }

        private string _messageId = Guid.NewGuid().ToString("N");

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly StringBuilder _renderBuffer = new StringBuilder();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AppendContent(string text)
        {
            lock (_renderBuffer)
            {
                _renderBuffer.Append(text);

                if (DateTime.Now - _timestamp > TimeSpan.FromMilliseconds(30) ||
                    _renderBuffer.Length >= 5)
                {
                    string flushContent = _renderBuffer.ToString();
                    _renderBuffer.Clear();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Content += flushContent;
                    });

                    _timestamp = DateTime.Now;
                }
            }
        }
    }
}
