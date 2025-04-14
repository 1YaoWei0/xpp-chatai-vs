using Community.VisualStudio.Toolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using xpp_chatai_vs.Command;
using xpp_chatai_vs.Model;
using xpp_chatai_vs.Service;

namespace xpp_chatai_vs.ViewModel
{
    public class KnowledgeBaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public ObservableCollection<KnowledgeBase> _knowledgeBases = new ObservableCollection<KnowledgeBase>();
        public ObservableCollection<KnowledgeBase> KnowledgeBases
        {
            get => _knowledgeBases;
            set => SetField(ref _knowledgeBases, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public ICommand RefreshCommand => new RelayCommand(LoadDataAsync);

        // public ICommand ShowCreateDialogCommand { get; }

        public ICommand ConfirmCreateCommand => new RelayCommand(CreateKnowledgeBaseAsync);

        public KnowledgeBaseViewModel()
        {
            
        }

        public async void LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                KnowledgeBaseClient knowledgeBaseClient = new KnowledgeBaseClient();

                var items = await knowledgeBaseClient.GETAsync($"https://ai.huameisoft.cn/api/core/dataset/list?parentId=67fccec595a21b808c299a6f");

                var responseStr = await GetJsonResponseAdvanced(items);

                var response = JsonConvert.DeserializeObject<ApiResponse>(responseStr);

                KnowledgeBases.Clear();

                foreach (var item in response.Data)
                {
                    if (item.Name != "")
                        KnowledgeBases.Add(item);
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task<string> GetJsonResponseAdvanced(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                var statusCode = response.StatusCode;
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"请求失败 ({statusCode}): {errorContent}",
                    ex
                );
            }
        }

        private async void CreateKnowledgeBaseAsync()
        {
            //var newKb = new NewKnowledgeBaseRequest
            //{
            //    Name = NewKbName,
            //    Type = SelectedType.Id
            //};

            //var result = await _service.CreateAsync(newKb);
            //if (result.IsSuccess)
            //{
            //    await LoadDataAsync();
            //    ShowCreateDialog = false;
            //}
        }
    }
}
