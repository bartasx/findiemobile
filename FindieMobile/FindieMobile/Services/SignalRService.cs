using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using FindieMobile.Services.Interfaces;
using Xamarin.Forms;

namespace FindieMobile.Services
{
    public class SignalRService : ISignalRService
    {
        public event EventHandler OnMessageReceived;
        public HubConnection HubConnection;

        public async Task ConnectToChat()
        {
            try
            {
                this.HubConnection = new HubConnectionBuilder().WithUrl("http://findieweb.azurewebsites.net/apphub").Build();
                await HubConnection.StartAsync();
                this.Config();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        // TO FIX
        public async void SendMessage(string firstUserNickname, string secondUserNickname, string message)
        {
            if (this.HubConnection != null)
            {
                   await this.HubConnection.InvokeAsync("Send", firstUserNickname, secondUserNickname, message);          
            }
        }

        public async Task SendCurrentLocationAsync(double latituide, double longitude, string username)
        {
            await this.HubConnection.InvokeAsync("Location", longitude, latituide, username);
        }

        public async Task<bool> IsClientSuccesfullyConnected(string username)
        {
            try
            {
                await this.HubConnection.InvokeAsync("Connect", username);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void Config()
        {
            this.HubConnection.On<string, string, string>("broadcast",
            (fromUsername, toUsername, message) =>
            {
                this.OnMessageReceived?.Invoke(this, new MessageReceiveEventArgs() { MessageContent = message, MessageFrom = fromUsername });
            });
        }
    }

    public class MessageReceiveEventArgs : EventArgs
    {
        public string MessageFrom { get; set; }
        public string MessageContent { get; set; }
    }
}