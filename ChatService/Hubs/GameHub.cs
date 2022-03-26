using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatService.Hubs
{
    public class GameHub: Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;
        public GameHub(IDictionary<string, UserConnection> connections)
        {
            
            _connections = connections;
        }

        public async Task SendCards(string [] cards)
        {

            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Client(userConnection.User)
                    .SendAsync("ReceiveCards", userConnection.User, cards);

            }
        }
    }
}
