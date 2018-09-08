using System;

namespace FindieMobile.Models
{
   public class MessagesModel
    {
        public int Id { get; set; }
        public int MessageFrom { get; set; }
        public int MessageTo { get; set; }
        public DateTime TimeStamp { get; set; }
        public string MessageText { get; set; }
    }
}