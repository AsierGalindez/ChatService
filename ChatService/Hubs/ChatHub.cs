using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatService.Hubs
{
    public class ChatHub: Hub
    {
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        public static Dictionary<string, int> rooms = new Dictionary<string, int>();

        int countPeople;
        

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
            
        }
        public async Task SendMessage(string message)
        {
            if(_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                
                await Clients.Group(userConnection.Room)
                    .SendAsync("ReceiveMessage", userConnection.User, message);

            }
        }
        public async Task JoinRoom(UserConnection userConnection)
        {
            if (_connections.Count < 2)
            {

            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} se ha unido a {userConnection.Room}", _connections.Count);
                Console.WriteLine(userConnection.User);

            
            }
            else
            {
                await Clients.Caller.SendAsync("RedirectWithMessage", "The lobby is already full.");

            }
        }
        
        
    }
}
