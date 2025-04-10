using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using xpp_chatai_vs.Command;
using xpp_chatai_vs.Model;
using System.IO;
using System.Linq;
using System.Windows;

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

            var assistantMessage = new ChatMessage() { Content = "", MessageType = MessageType.UserInput };

            Messages.Add(userMessage);

            InputText = string.Empty;

            await GetAIResponse(assistantMessage);
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

        private async Task GetAIResponse(ChatMessage assistantMessage)
        {
            try
            {
                var requestBody = new
                {
                    messages = BuildMessageHistory(),
                    model = "deepseek-chat",
                    stream = true,
                    temperature = 1,
                    max_tokens = 2048
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "sk-613975d20507407399cb67b54a313504");

                var response = await httpClient.PostAsync("https://api.deepseek.com/chat/completions", content);
                await ProcessStreamResponseAsync(response, assistantMessage);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                
            }
        }

        private List<dynamic> BuildMessageHistory()
        {
            var messages = new List<dynamic>();

            foreach (var msg in Messages)
            {
                messages.Add(new
                {
                    content = msg.Content,
                    role = msg.MessageType == MessageType.UserInput ? "user" : "assistant"
                });
            }

            return messages;
        }

        private async Task ProcessStreamResponseAsync(HttpResponseMessage response, ChatMessage assistantMessage)
        {
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
                StringBuilder buffer = new StringBuilder();
                char[] readBuffer = new char[4096];

                while (true)
                {
                    int bytesRead = await reader.ReadAsync(readBuffer, 0, readBuffer.Length);
                    if (bytesRead == 0) break;

                    string chunk = new string(readBuffer, 0, bytesRead);
                    buffer.Append(chunk);

                    while (true)
                    {
                        int dataStart = buffer.ToString().IndexOf("data: ");
                        if (dataStart == -1) break;

                        int dataEnd = buffer.ToString().IndexOf("\n\n", dataStart, StringComparison.Ordinal);
                        if (dataEnd == -1) break;

                        string dataLine = buffer.ToString().Substring(dataStart, dataEnd - dataStart);
                        buffer.Remove(0, dataEnd + 2);

                        ProcessSingleDataLine(dataLine, assistantMessage);
                    }
                }
            }
        }

        private void ProcessSingleDataLine(string dataLine, ChatMessage assistantMessage)
        {
            if (!dataLine.StartsWith("data: ")) return;

            var json = dataLine.Substring("data: ".Length);
            if (json == "[DONE]") return;

            try
            {
                var chunk = JsonConvert.DeserializeObject<ResponseChunk>(json);
                var content = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;

                if (!string.IsNullOrEmpty(content))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var responseMessage = new ChatMessage() { Content = content, MessageType = MessageType.AIResponse };
                        Messages.Add(responseMessage);
                    });
                }
            }
            catch (JsonException) { }
        }

        public class ResponseChunk
        {
            [JsonProperty("choices")]
            public List<StreamChoice> Choices { get; set; }
        }

        public class StreamChoice
        {
            [JsonProperty("delta")]
            public StreamDelta Delta { get; set; }
        }

        public class StreamDelta
        {
            [JsonProperty("content")]
            public string Content { get; set; }
        }
    }
}
