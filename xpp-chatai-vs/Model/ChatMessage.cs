using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
