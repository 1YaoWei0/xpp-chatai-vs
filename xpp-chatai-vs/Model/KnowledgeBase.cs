using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace xpp_chatai_vs.Model
{
    public class KnowledgeBase : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _id;
        private string _name;

        [JsonProperty("_id")]
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _intro;
        [JsonProperty("intro")]
        public string Intro
        {
            get => _intro;
            set
            {
                _intro = value;
                OnPropertyChanged();
            }
        }

        private string _type;
        [JsonProperty("type")]
        public string Type 
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        } 

        [JsonProperty("permission")]
        public string Permission { get; set; } 

        [JsonProperty("tmbId")]
        public string ThumbnailId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("canWrite")]
        public bool CanWrite { get; set; }

        [JsonProperty("isOwner")]
        public bool IsOwner { get; set; }
    }       
}
