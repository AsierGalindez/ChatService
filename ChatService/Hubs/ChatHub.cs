using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatService;
using System.Linq;
using System;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        private static List<Game> _games = new List<Game>();
        private readonly Game _game;


        public ChatHub(IDictionary<string, UserConnection> connections, Game game)
        {
            _botUser = "MyChat Bot";
            _connections = connections;
            _game = game;
            
        }
       

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has left");
                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;
            userConnection.SignalrId = Context.ConnectionId;

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} has joined {userConnection.Room}");

            await SendUsersConnected(userConnection.Room);
            
        }

        public async Task SendMessage(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userConnection.User, message);
            }
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }

        public async Task SetPlayer (int player)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                userConnection.Player = player;

                await Clients.Group(userConnection.Room).SendAsync("ReceivePlayerNumber", userConnection.User, userConnection.Player);
            }
        }
        // ACCIÓN DE DAR CARTAS
        public async Task SendCards(string[][] deck)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                var clients = _connections.Values.Where(r => r.Room == userConnection.Room);
                var clientIds = clients
                .Select(c => c.ToString())
                .ToList();
                int position;
                position = userConnection.Player;

                for (int i = 0; i <4 ; i++)
                {
                    position++;
                    if(position == 4)
                    {
                        position = 0;
                    }
                     
                     string userActive = _connections.Values.Where(r => r.Room == userConnection.Room).Where(p => p.Player == position).Select(s=>s.SignalrId).FirstOrDefault();

                    await Clients.Client(userActive).SendAsync("ReceiveHandCards", deck[i]);
                }
            }


            var game = _games.FirstOrDefault();
            


        }
        //Botón Ready. Cuando todos le dan el juego comienza y determina quien es el postre
        public async Task IsReady(bool ready)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                if (ready)
                {
                userConnection.Ready = ready;
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} está listo");
                }
                else { await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} se lo está pensando..."); }
               
                var readys= _connections.Values.Where(r=>r.Room == userConnection.Room).Select(re => re.Ready);
                int contador = 0;
                foreach (bool ok in readys)
                {
                    if (ok)
                    {
                        contador++;

                    if(contador == 4)
                        {
                            Random rnd = new Random();
                            int player3 = rnd.Next(0, 4);
                            var player= _connections.Values.Where(r => r.Room == userConnection.Room).Where(p => p.Player == player3).FirstOrDefault();

                            await Clients.Group(userConnection.Room).SendAsync("StartGame", player3);
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{player.User} reparte cartas.");
                     
                        }
                    }
                } 
            
            }
        }
        // CAMBIAR TURNO
        public async Task ChangeTurn(int postre, int round)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {

                if (userConnection.Player == postre)//Si el que pasa es el postre pasamos de ronda.
                {
                    round++;
                    await Clients.Group(userConnection.Room).SendAsync("NextRound" , round);
                }
                else
                {
                    int turn = userConnection.Player;
                    turn++;
                    if (turn == 4)
                    {
                        turn = 0;
                    }                
                    await Clients.Group(userConnection.Room).SendAsync("NewTurn", turn);

                }
               

            }
        }
        //ACCIÓN DE MUS, DESCARTES
        public async Task DropCards(string[] dropped, int postre)
        {

            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {

                string userActive = _connections.Values.Where(r => r.Room == userConnection.Room).Where(p => p.Player == postre).Select(s => s.SignalrId).FirstOrDefault();
                int descartes = dropped.Count(c => c == "F000");
                int pide = 4 - descartes;

                await Clients.Client(userActive).SendAsync("DroppedCards", dropped);
                await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} pide {descartes} cartas.");
                await Clients.Group(userConnection.Room).SendAsync("Descarte", pide);
                
                if(userConnection.Player != postre)              
                {
                    int turn = userConnection.Player;
                    turn++;
                        if (turn == 4)
                        {
                            turn = 0;
                        }
                    await Clients.Group(userConnection.Room).SendAsync("NewTurn", turn);
                }


            }
        }



        //ACCIÓN NO HAY MUS

        public async Task NoMus()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                
                
                    await Clients.Group(userConnection.Room).SendAsync("NoMus");
                
            }
        }

        //ACCIÓN ENVIDO

        public async Task Bet(int bet)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                if (userConnection.Player%2 == 0)
                {
                    await Clients.Group(userConnection.Room).SendAsync("ParTeamBets", bet);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} envida {bet}");
                }
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("OddTeamBets", bet);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User}  envida {bet}.");
                }
            }
        }

        //ACCION NO QUIERO

        public async Task Fold(int contador)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                if (userConnection.Player % 2 == 0)
                {
                    await Clients.Group(userConnection.Room).SendAsync("ParTeamFold", contador);
                }
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("OddTeamFold", contador);
                }
            }
        }


        //QUIERO


        public async Task Call(int bet, int contador)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                if (contador == 1)
                {
                    contador = bet;
                    await Clients.Group(userConnection.Room).SendAsync("Call", contador);

                }
                else
                {

                    contador = contador + bet;
                
                   await Clients.Group(userConnection.Room).SendAsync("Call", contador);                             
                }
            }

        }

        //ACCION REENVIDO
        public async Task SecondBet(int contador, int bet, int secondBet)
        {
            contador = bet + contador;
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                if (userConnection.Player % 2 == 0)
                {
                    await Clients.Group(userConnection.Room).SendAsync("ParTeamSdecondBet", contador, secondBet);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} sube {secondBet} piedras.");
                }
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("OddTeamSecondBet", contador, secondBet);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User}  sube {secondBet} piedras.");
                }
            }

        }

        //ACCION CONTARPIEDRAS
        //MAYOR (s.Remove(0, 1)

        public async Task AccountantMayor(string[][] deck)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                int[] mayorPeque = new int[2];
                mayorPeque= _game.CleanCards(deck);
                if (mayorPeque[0]== 0)
                {
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo par gana la mayor.");

                }
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo impar gana la mayor.");
                }

                if (mayorPeque[1] == 1)
                {
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo par gana la pequeña.");
                }
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo impar gana la pequeña.");
                }

               

            }
        }
    }   
}
