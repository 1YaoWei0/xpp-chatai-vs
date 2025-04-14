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
using xpp_chatai_vs.Service;

namespace xpp_chatai_vs.ViewModel
{
    /// <summary>
    /// Copilot chat view model class for copilot chat form
    /// Willie Yao - 04/14/2025
    /// </summary>
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

        private ChatMessage _currentAssistantMessage;

        public ICommand SendCommand => new RelayCommand(SendMessageAsync);

        /// <summary>
        /// Send message commend
        /// Willie Yao - 04/14/2025
        /// </summary>
        private async void SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(InputText)) return;

            var userMessage = new ChatMessage() { Content = InputText, MessageType = MessageType.UserInput };

            Messages.Add(userMessage);

            InputText = string.Empty;

            await GetAIResponse();
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

        /// <summary>
        /// Get ai response
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <returns>Task</returns>
        private async Task GetAIResponse()
        {
            try
            {
                var model = Option.Instance.CompletionModel;

                HttpResponseMessage responseMessage = new HttpResponseMessage();

                switch (model)
                {
                    case Base.CompletionModel.Deepseek:
                        DskAIHttpClient dskAIHttpClient = new DskAIHttpClient(BuildMessageHistory(), "deepseek-chat", 1, 2048);
                        responseMessage = await dskAIHttpClient.PostAsync("https://api.deepseek.com/chat/completions");
                        break;
                    case Base.CompletionModel.XppAssistant:
                        XppAssistantHttpClient xppAssistant = new XppAssistantHttpClient(BuildMessageHistory(), "", 1, 2048);
                        responseMessage = await xppAssistant.PostAsync("https://ai.huameisoft.cn/api/v1/chat/completions");
                        break;
                }

                await ProcessStreamResponseAsync(responseMessage);
            }
            catch (NullReferenceException ex)
            {
                var nullErrorResponse = new ChatMessage() { Content = "Please check the model or api key!", MessageType = MessageType.SystemAlert };

                Messages.Add(nullErrorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ChatMessage() { Content = "Connection error!", MessageType = MessageType.SystemAlert };

                Messages.Add(errorResponse);
            }            
            finally
            {
                
            }
        }

        /// <summary>
        /// Build message history
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <returns>List<dynamic></returns>
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

        /// <summary>
        /// Process stream response
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="response">HttpResponseMessage</param>
        /// <returns>Task</returns>
        private async Task ProcessStreamResponseAsync(HttpResponseMessage response)
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

                        await ProcessSingleDataLine(dataLine);
                    }
                }

                // await _currentAssistantMessage.FlushContentAsync();
                _currentAssistantMessage = null;
            }
        }

        /// <summary>
        /// Process single data line
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="dataLine"></param>
        /// <returns></returns>
        private async Task ProcessSingleDataLine(string dataLine)
        {
            if (!dataLine.StartsWith("data: ")) return;

            var json = dataLine.Substring("data: ".Length);
            if (json == "[DONE]")
            {
                _currentAssistantMessage = null; // 重置状态
                return;
            }

            try
            {
                var chunk = JsonConvert.DeserializeObject<ResponseChunk>(json);
                var content = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;

                if (!string.IsNullOrEmpty(content))
                {
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        if (_currentAssistantMessage == null)
                        {
                            _currentAssistantMessage = new ChatMessage()
                            {
                                Content = content,
                                MessageType = MessageType.AIResponse
                            };
                            Messages.Add(_currentAssistantMessage);
                        }
                        else
                        {
                            await _currentAssistantMessage.AppendContentAsync(content);
                        }
                    });
                }
            }
            catch (JsonException) { }
        }

        /// <summary>
        /// Response chunk
        /// Willie Yao - 04/14/2025
        /// </summary>
        public class ResponseChunk
        {
            [JsonProperty("choices")]
            public List<StreamChoice> Choices { get; set; }
        }

        /// <summary>
        /// Stream choice
        /// Willie Yao - 04/14/2025
        /// </summary>
        public class StreamChoice
        {
            [JsonProperty("delta")]
            public StreamDelta Delta { get; set; }
        }

        /// <summary>
        /// Steam delta
        /// Willie Yao - 04/14/2025
        /// </summary>
        public class StreamDelta
        {
            [JsonProperty("content")]
            public string Content { get; set; }
        }
    }
}
