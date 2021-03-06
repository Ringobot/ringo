﻿using System;

namespace Ringo.Common.Models
{

    public class BotMessage
    {
        public Activity[] activities { get; set; }
        public string watermark { get; set; }
    }

    public class Activity
    {
        public string type { get; set; }
        public string id { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime localTimestamp { get; set; }
        public string channelId { get; set; }
        public From from { get; set; }
        public Conversation conversation { get; set; }
        public string text { get; set; }
        public string inputHint { get; set; }
        public string replyToId { get; set; }
        public string serviceUrl { get; set; }
        public string code { get; set; }
    }

    public class From
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Conversation
    {
        public string id { get; set; }
    }

    
    public class Message
    {
        public string type { get; set; }
        public MessageFrom from { get; set; }
        public string text { get; set; }
    }

    public class MessageFrom
    {
        public string id { get; set; }
    }

}
