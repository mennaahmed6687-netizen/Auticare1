using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace auticare.core
{
    public class PushSubscriptionModel
    {
        public int Id { get; set; }

        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }

        public string ParentId { get; set; }

        // ❌ مهم جدًا: تجاهل الـ navigation property في الـ API
        [JsonIgnore]
        public virtual Parent Parent { get; set; }
    }
}
