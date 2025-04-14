using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace xpp_chatai_vs.Model
{
    public class ApiResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("statusText")]
        public string StatusText { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<KnowledgeBase> Data { get; set; }
    }
}
