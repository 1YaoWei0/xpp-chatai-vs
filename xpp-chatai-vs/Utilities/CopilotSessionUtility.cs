using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using xpp_chatai_vs.Model;
using xpp_chatai_vs.ViewModel;
using static Microsoft.VisualStudio.Shell.ThreadedWaitDialogHelper;

namespace xpp_chatai_vs.Utilities
{
    public class CopilotSessionUtility
    {
        /// <summary>
        /// Get ai chat session storage path.
        /// Willie Yao - 04/09/2025
        /// </summary>
        /// <returns>Path</returns>
        public static string GetStoragePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XppChatAiVs", "sessions.json");
        }

        /// <summary>
        /// Create new chat view model element.
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <returns>
        /// Chat view model element
        /// </returns>
        public static CopilotChatViewModel CreateChatViewModel(int index = 1)
        {
            var defaultChatSessionMeta = new ChatSessionMeta()
            {
                SessionName = $"New chat {index}",
                LastActiveTime = DateTime.Now
            };

            var defaultChatViewModel = new CopilotChatViewModel()
            {
                Metadata = defaultChatSessionMeta
            };

            return defaultChatViewModel;
        }

        /// <summary>
        /// Save session collection
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <param name="copilotChatViewModels">
        /// CopilotChatViewModels
        /// </param>
        /// <returns>
        /// Task
        /// </returns>
        public static async Task SaveSessionsAsync(ObservableCollection<CopilotChatViewModel> copilotChatViewModels)
        {
            var path = GetStoragePath();
            var json = JsonConvert.SerializeObject(copilotChatViewModels);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Get last copilot chat view model
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <param name="copilotChatViewModels">
        /// CopilotChatViewModels
        /// </param>
        /// <returns>
        /// The last copilot chat view model
        /// </returns>
        public static CopilotChatViewModel GetLastCopilotChatViewModel(ObservableCollection<CopilotChatViewModel> copilotChatViewModels)
        {
            return copilotChatViewModels
                .Where(s => s.Metadata.LastActiveTime != null)
                .OrderBy(s => Math.Abs((DateTime.UtcNow - s.Metadata.LastActiveTime).Ticks))
                .FirstOrDefault();
        }

        /// <summary>
        /// Get the copilot chat view model by session ID.
        /// Willie Yao - 04/10/2025
        /// </summary>
        /// <param name="copilotChatViewModels">
        /// CopilotChatViewModels
        /// </param>
        /// <param name="sessionId">Session ID</param>
        /// <returns>
        /// The copilot chat view model
        /// </returns>
        public static CopilotChatViewModel GetCopilotChatViewModel(ObservableCollection<CopilotChatViewModel> copilotChatViewModels, Guid sessionId)
        {
            return copilotChatViewModels
                .Where(s => s.Metadata.SessionId == sessionId && s.Metadata.SessionId != null)
                .FirstOrDefault();
        }
    }
}
