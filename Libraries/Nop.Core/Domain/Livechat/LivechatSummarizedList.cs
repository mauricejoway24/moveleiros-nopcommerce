using System;

namespace Nop.Core.Domain.Livechat
{
    public class LivechatSummarizedList
    {
        public string ChannelId { get; set; }
        public string AgentName { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessage { get; set; }
        public int MessagesCount { get; set; }
        public bool IsFinished { get; set; }

        public string CreatedAtFormated => CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
