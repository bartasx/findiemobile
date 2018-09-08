using System.Collections.Generic;
using System.Threading.Tasks;
using FindieMobile.Models;
using Plugin.Media.Abstractions;

namespace FindieMobile.Services.Interfaces
{
    public interface IFindieWebApiService
    {
        bool IsUserExists();
        bool IsUserExists(string username, string password);
        Task Logout();
        bool RegisterNewUser(RegisterModel registerModel);
        Task<List<string>> GetFriendsBrowserResult(string username);
        Task<bool> SendFriendRequest(string friendUsername);
        Task<bool> AcceptFriendRequest(string username);
        Task<UserModel> GetSpecificUserInfo(string username);
        Task<bool> RemoveFriend(string friendName);
        Task<List<string>> GetFriendsList(string username);
        Task<List<LocationModel>> GetFriendsLocationAsync();
        Task<string> GetAllSubscribedEvents();
        Task<bool> SendNewEvent(EventModel eventModel, MediaFile photo);
        void GetSpecificEventInfo();
        Task<List<MessagesModel>> GetMessages(FriendModel friendModel);
    }
}