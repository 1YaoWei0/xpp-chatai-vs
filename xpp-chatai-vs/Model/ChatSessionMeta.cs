using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace xpp_chatai_vs.Model
{
    public class ChatSessionMeta
    {
        public Guid SessionId { get; } = Guid.NewGuid();
        public string SessionName { get; set; } = "New Chat";
        public List<ChatMessage> Messages { get; set; }
        public DateTime LastActiveTime { get; set; }
    }

    [DataContract]
    public class PersistedSession
    {
        [DataMember] public Guid Id { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public List<ChatMessage> Messages { get; set; }
        [DataMember] public DateTime LastActive { get; set; }

        public static byte[] Serialize(ChatSessionMeta chatSessionMeta)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(PersistedSession));
                serializer.WriteObject(ms, new PersistedSession { 
                    Id = chatSessionMeta.SessionId,
                    Name = chatSessionMeta.SessionName,
                    Messages = chatSessionMeta.Messages,
                    LastActive = chatSessionMeta.LastActiveTime
                });
                return ms.ToArray();
            }
        }
    }    
}
