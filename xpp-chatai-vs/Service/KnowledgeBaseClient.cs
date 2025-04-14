using System.Net.Http;
using System.Threading.Tasks;

namespace xpp_chatai_vs.Service
{
    internal class KnowledgeBaseClient : IAIHttpClient
    {
        private HttpClient _httpClient;

        public KnowledgeBaseClient()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Option.Instance.AiKey);
        }

        public Task<HttpResponseMessage> GETAsync(string requestUrl)
        {
            return _httpClient.GetAsync(requestUrl);
        }

        public Task<HttpResponseMessage> PostAsync(string baseUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}
