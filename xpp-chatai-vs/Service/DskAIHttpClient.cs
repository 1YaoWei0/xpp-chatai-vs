using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace xpp_chatai_vs.Service
{
    /// <summary>
    /// Deepseek ai http client model
    /// Willie Yao - 04/14/2025
    /// </summary>
    internal class DskAIHttpClient : IAIHttpClient
    {
        public List<dynamic> Messages { private set; get; }

        public string Model { private set; get; }

        public int Temperature { private set; get; }

        public Int64 MaxTokens { private set; get; }

        private HttpClient _httpClient;

        /// <summary>
        /// Constructor
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="messages">Context messages</param>
        /// <param name="model">Completion model</param>
        /// <param name="temperature">Temperature</param>
        /// <param name="maxTokens">Max tokens</param>
        public DskAIHttpClient(
            List<dynamic> messages,
            string model,
            int temperature,
            Int64 maxTokens) 
        {
            Messages = messages;
            Model = model;
            Temperature = temperature;
            MaxTokens = maxTokens;
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Option.Instance.AiKey);
        }

        /// <summary>
        /// Post method
        /// Willie Yao - 04/14/2025
        /// </summary>
        /// <param name="baseUrl">Base url</param>
        /// <returns>HttpResponseMessage</returns>
        /// <exception cref="NullReferenceException">
        /// NullReferenceException
        /// </exception>
        public Task<HttpResponseMessage> PostAsync(string baseUrl)
        {
            if (_httpClient == null)
            {
                throw new NullReferenceException("HttpClient is null, please construct it first.");
            }

            var requestBody = new
            {
                messages = Messages,
                model = Model,
                stream = true,
                temperature = Temperature,
                max_tokens = MaxTokens
            };

            var json = JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return _httpClient.PostAsync(baseUrl, content);
        }
    }
}
