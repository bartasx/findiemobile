using System;
using System.Threading.Tasks;
using FindieMobile.Services.Interfaces;

namespace FindieMobile.Services
{
    public class MessagesService : IMessageService
    {
        public string FriendUsername { get; set; }

        public void SetFriendsUsername(string username)
        {
            if (username != null)
            this.FriendUsername = username;
        }

        public string GetFriendsUsername()
        {
            return this.FriendUsername;
        }

        public Task SendMessage()
        {
            throw new NotImplementedException();
        }

        public Task GetMessages(string username)
        {
            throw new NotImplementedException();
        }
    }
}