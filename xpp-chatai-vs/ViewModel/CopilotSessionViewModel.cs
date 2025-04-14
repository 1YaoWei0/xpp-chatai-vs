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
using System.Linq;
using xpp_chatai_vs.Utilities;
using System.Diagnostics;
using xpp_chatai_vs.View;
using System.Windows.Forms;
using System.Drawing;

namespace xpp_chatai_vs.ViewModel
{
    /// <summary>
    /// Copilot session view model class
    /// Willie Yao - 04/14/2025
    /// </summary>
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
                if (value == null) return;

                _currentSession = value;
                OnSessionChanged(value);
                OnPropertyChanged();
            }
        }

        public ICommand AddNewSessionCommand => new RelayCommand(AddNewSessionAsync);

        public ICommand RemoveSessionCommand => new RelayCommand(RemoveCurrentSessionAsync);

        public ICommand RenameSessionNameCommand => new RelayCommand(OpenRenameDialogAsync);

        /// <summary>
        /// Add new session on ai chat
        /// Willie Yao - 04.10/2025
        /// </summary>
        private async void AddNewSessionAsync()
        {
            int messageCount = CurrentSession.Messages.Count;
            var existBlankSession = ValidateExistBlankSession();

            if (messageCount <= 0 || existBlankSession)
            {
                return;
            }

            var newChatViewModel = CopilotSessionUtility.CreateChatViewModel(Sessions.Count + 1);

            Sessions.Add(newChatViewModel);

            CurrentSession = CopilotSessionUtility.GetLastCopilotChatViewModel(Sessions);
        }

        /// <summary>
        /// Remove current session on ai chat
        /// Willie Yao - 04/10/2025
        /// </summary>
        private async void RemoveCurrentSessionAsync()
        {
            int sessionsCount = Sessions.Count;            

            if (sessionsCount <= 1)
            {
                Sessions.Clear();

                Sessions.Add(CopilotSessionUtility.CreateChatViewModel());
            }
            else
            {
                Guid sessionId = CurrentSession.Metadata.SessionId;

                var currentSession = CopilotSessionUtility.GetCopilotChatViewModel(Sessions, sessionId);

                Sessions.Remove(currentSession);
            }

            CopilotChatViewModel lastSession = CopilotSessionUtility.GetLastCopilotChatViewModel(Sessions);

            CurrentSession = lastSession;
        }

        /// <summary>
        /// This is a dialog for accepting the new session name.
        /// Willie Yao - 04/10/2025
        /// </summary>
        private async void OpenRenameDialogAsync()
        {
            using (var inputDialog = new Form())
            {                
                inputDialog.StartPosition = FormStartPosition.CenterParent; 
                inputDialog.ShowInTaskbar = false; 

                inputDialog.Text = "New Name";
                inputDialog.ClientSize = new Size(300, 120);
                inputDialog.FormBorderStyle = FormBorderStyle.FixedDialog;

                var txtInput = new TextBox
                {
                    Location = new Point(20, 20),
                    Width = 260
                };

                var btnOK = new Button
                {
                    Text = "Ok",
                    DialogResult = DialogResult.OK,
                    Location = new Point(120, 60)
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(200, 60)
                };

                inputDialog.Controls.Add(txtInput);
                inputDialog.Controls.Add(btnOK);
                inputDialog.Controls.Add(btnCancel);
                inputDialog.AcceptButton = btnOK;
                inputDialog.CancelButton = btnCancel;

                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(txtInput.Text))
                    {
                        CurrentSession.Metadata.SessionName = txtInput.Text;
                    }
                }
            }
        }

        /// <summary>
        /// Extend logic when session changed.
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <param name="newSession">New session</param>
        private void OnSessionChanged(CopilotChatViewModel newSession)
        { 
            newSession.Metadata.LastActiveTime = DateTime.Now;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        

        public CopilotSessionViewModel()
        {
            
        }

        /// <summary>
        /// Validate exists blank session
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <returns>True if exists</returns>
        private bool ValidateExistBlankSession()
        {
            foreach (var chatViewModel in Sessions)
            {
                if (!chatViewModel.Messages.Any())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Load session when the visual studio opened.
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <returns>Task</returns>
        public async Task LoadSessionsAsync()
        {
            try
            {
                var path = CopilotSessionUtility.GetStoragePath();

                if (File.Exists(path))
                {
                    var sessions = JsonConvert.DeserializeObject<List<CopilotChatViewModel>>(File.ReadAllText(path));

                    Sessions.Clear();

                    if (sessions.Count <= 0)
                    {
                        Sessions.Add(CopilotSessionUtility.CreateChatViewModel());
                    }
                    else
                    {
                        foreach (var session in sessions)
                            Sessions.Add(session);
                    }
                }
                else
                {
                    Sessions.Add(CopilotSessionUtility.CreateChatViewModel());
                }

                await SwitchToLastSessionAsync();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON parsing failed: {ex.Message}");
                Sessions.Clear();
                Sessions.Add(CopilotSessionUtility.CreateChatViewModel());
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"File operation failure: {ex.Message}");
                Sessions.Clear();
                Sessions.Add(CopilotSessionUtility.CreateChatViewModel());
            }            
        }

        /// <summary>
        /// Switch to last session
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <returns>Task</returns>
        public async Task SwitchToLastSessionAsync()
        {
            CopilotChatViewModel lastSession = CopilotSessionUtility.GetLastCopilotChatViewModel(Sessions);

            if (lastSession == null) 
            {
                lastSession = CopilotSessionUtility.CreateChatViewModel();
            }

            CurrentSession = lastSession;
        }
    }
}
