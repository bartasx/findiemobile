using System;
using System.Threading.Tasks;

namespace FindieMobile.Services.Interfaces
{
    public interface ISignalRService
    {
        Task ConnectToChat();
        void SendMessage(string firstUserNickname, string secondUserNickname, string message);
        Task SendCurrentLocationAsync(double latituide, double longitude, string username);
        Task<bool> IsClientSuccesfullyConnected(string username);
        event EventHandler OnMessageReceived;
    }
}