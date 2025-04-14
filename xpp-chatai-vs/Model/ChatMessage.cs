using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace xpp_chatai_vs.Model
{
    /// <summary>
    /// Message type
    /// Willie Yao - 04/14/2025
    /// </summary>
    public enum MessageType
    {
        UserInput,
        AIResponse,
        SystemAlert,
        CodeSnippet
    }

    /// <summary>
    /// Chat message model
    /// Willie Yao - 04/14/2025
    /// </summary>
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

        private Stopwatch _renderStopwatch = new Stopwatch();

        private SemaphoreSlim _renderLock = new SemaphoreSlim(1, 1);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Appent stream content async
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="text">Content</param>
        /// <param name="ct">CancellationToken</param>
        /// <returns>Task</returns>
        public async Task AppendContentAsync(string text, CancellationToken ct = default)
        {
            await _renderLock.WaitAsync(ct);

            try
            {
                _renderBuffer.Append(text);

                var elapsedMs = _renderStopwatch.ElapsedMilliseconds;
                var requiredDelay = Math.Max(20 - (int)elapsedMs, 0);

                if (_renderStopwatch.IsRunning)
                    await Task.Delay(requiredDelay, ct);

                _renderStopwatch.Restart();

                while (_renderBuffer.Length > 0)
                {
                    var takeCount = Math.Min(3, _renderBuffer.Length);
                    var segment = _renderBuffer.ToString(0, takeCount);
                    _renderBuffer.Remove(0, takeCount);

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Content += segment;
                    }, DispatcherPriority.Background, ct);

                    if (takeCount < 3)
                        break;
                }
            }
            finally
            {
                _renderLock.Release();
            }
        }

        /// <summary>
        /// Flush content async
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <returns>Task</returns>
        public async Task FlushContentAsync()
        {
            await _renderLock.WaitAsync();
            try
            {
                //if (_renderBuffer.Length > 0)
                //{
                //    await Application.Current.Dispatcher.InvokeAsync(() =>
                //    {
                //        Content += _renderBuffer.ToString();
                //        _renderBuffer.Clear();
                //    }, DispatcherPriority.Background);
                //}
            }
            finally
            {
                _renderLock.Release();
            }
        }
    }
}
