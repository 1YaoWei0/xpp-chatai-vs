using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using xpp_chatai_vs.Command;
using xpp_chatai_vs.Model;

namespace xpp_chatai_vs.ViewModel
{
    public class CopilotChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ChatSessionMeta _metaData;
        public ChatSessionMeta Metadata
        {
            get => _metaData;
            set => SetField(ref _metaData, value);
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set => SetField(ref _inputText, value);
        }

        private ObservableCollection<ChatMessage> _messages = new ObservableCollection<ChatMessage>();
        public ObservableCollection<ChatMessage> Messages 
        {
            get => _messages;
            set => SetField(ref _messages, value);
        }

        public ICommand SendCommand => new RelayCommand(SendMessageAsync);

        private async void SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(InputText)) return;

            var userMessage = new ChatMessage() { Content = InputText, MessageType = MessageType.UserInput };

            Messages.Add(userMessage);

            InputText = string.Empty;

            var responseMessage = new ChatMessage() { Content = "Test response", MessageType = MessageType.AIResponse };

            Messages.Add(responseMessage);
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CopilotChatViewModel()
        {

        }
    }
}
