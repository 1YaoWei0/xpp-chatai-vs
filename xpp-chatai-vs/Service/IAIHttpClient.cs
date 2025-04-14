using System.Net.Http;
using System.Threading.Tasks;

namespace xpp_chatai_vs.Service
{
    public interface IAIHttpClient
    {
        Task<HttpResponseMessage> PostAsync(string baseUrl);
    }
}
