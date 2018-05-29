using System;
using System.Net.Http;
using FindieMobile.Models;
using FindieMobile.SQLite.Tables;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Forms;

namespace FindieMobile.Services
{
    public class SignalRService
    {
        public readonly HubConnection _hubConnection;

        public SignalRService()
        {
            try
            {
                this._hubConnection = new HubConnectionBuilder().WithUrl("http://findieweb.pl/apphub").WithTransport(Microsoft.AspNetCore.Sockets.
                    TransportType.LongPolling).Build();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public async void ConnectToChat()
        {
            try
            {
              await this._hubConnection.StartAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async void SendMessage(string firstUserNickname, string secondUserNickname, string message)
        {
            await this._hubConnection.InvokeAsync("Send", firstUserNickname, secondUserNickname, message);
        }
    }
}
