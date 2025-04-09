using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using xpp_chatai_vs.Command;
using xpp_chatai_vs.Model;
using System.Linq;

namespace xpp_chatai_vs.ViewModel
{
    public class CopilotSessionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const int MAX_SESSIONS = 20;

        private ObservableCollection<CopilotChatViewModel> _sessions = new ObservableCollection<CopilotChatViewModel>();
        public ObservableCollection<CopilotChatViewModel> Sessions 
        {
            get => _sessions;
            private set
            {
                _sessions = value;
                OnPropertyChanged();
            }
        }

        private CopilotChatViewModel _currentSession;
        public CopilotChatViewModel CurrentSession
        {
            get => _currentSession;
            set 
            {
                _currentSession = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddNewSessionCommand => new RelayCommand(AddNewSessionAsync);

        private async void AddNewSessionAsync()
        {
            Console.WriteLine("Trigger!");
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        

        public CopilotSessionViewModel()
        {
            LoadSessionsAsync().ConfigureAwait(false);
        }

        public async Task LoadSessionsAsync()
        {
            var path = GetStoragePath();
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var sessions = JsonConvert.DeserializeObject<List<CopilotChatViewModel>>(json);
                Sessions.Clear();
                if (sessions.Count == 0)
                {
                    Sessions.Add(new CopilotChatViewModel() { Metadata = new ChatSessionMeta() });
                }
                else
                {
                    foreach (var session in sessions)
                    {
                        Sessions.Add(session);
                    }
                }                    
            }
            else
            {
                await this.SaveAllSessionsAsync();
            }
            CurrentSession = Sessions.Last();
        }

        public async Task SaveAllSessionsAsync()
        {
            var path = GetStoragePath();
            var json = JsonConvert.SerializeObject(Sessions);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
        }

        private static string GetStoragePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "XppChatAiVs",
                "sessions.json");
        }
    }
}
