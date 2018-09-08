using System.Threading.Tasks;

namespace FindieMobile.Services.Interfaces
{
    public interface IMessageService
    {
        void SetFriendsUsername(string username);
        string GetFriendsUsername();
        Task SendMessage();
        Task GetMessages(string username);
    }
}