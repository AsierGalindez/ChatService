using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatService;
using System.Linq;
using System;
using System.Threading;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        
        private readonly string _botUser;
        private readonly IDictionary<string, UserConnection> _connections;        
        private IDictionary<string, string[][]> _deck;
        private IDictionary<string, int[][]> _cleanDeck = new Dictionary<string, int[][]>();
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
        //ACCIÓN ASIGNAR PLAYERS
        public async Task SetPlayer (int player)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                userConnection.Player = player;

                await Clients.Group(userConnection.Room).SendAsync("ReceivePlayerNumber", userConnection.User, userConnection.Player);
            }
        }
        //ACCIÓN ASIGNAR AVATAR
        public async Task SetAvatar (int avatar)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                userConnection.Avatar = avatar;

                await Clients.Group(userConnection.Room).SendAsync("ReceivePlayerAvatar", userConnection.Player, userConnection.Avatar);
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
                _cleanDeck["barajaLimpia"]=_game.CleanCards(deck);
            }
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
        //SETEAR PLAYERS EN NUEVAMANO
        public async Task NuevaMano()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
              
                int nextPlayer= userConnection.Player;

                for (int i = 0; i < 4; i++)
                {
                    nextPlayer++;
                    if (nextPlayer == 4)
                    {
                        nextPlayer = 0;
                    }
                 var player = _connections.Values.Where(r => r.Room == userConnection.Room).Where(p => p.Player==nextPlayer).FirstOrDefault();
                 player.PosicionDeVuelta = i;
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
        //CABIAR ROUND
        public async Task ChangeRound(int round)
        {
             if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
             { 
                 round++;
                 await Clients.Group(userConnection.Room).SendAsync("NextRound" , round);
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
                // await Clients.Group(userConnection.Room).SendAsync("Descarte", pide);
                
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
        //ACCIÓN RECIBIR CARTAS
        public async Task RecibirCartas(string[][] deck)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                
                _cleanDeck["barajaClean"] = _game.CleanCards(deck);
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
                string team;
                if (userConnection.Player%2 == 0)
                {
                    team = "blue";
                    await Clients.Group(userConnection.Room).SendAsync("Bet", bet, team);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} envida {bet}");
                }
                else
                {
                    team = "red";
                    await Clients.Group(userConnection.Room).SendAsync("Bet", bet, team);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User}  envida {bet}.");
                }
            }
        }

        //ACCION NO QUIERO
        public async Task Fold(int contador, int round)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                string team;
                if (userConnection.Player % 2 == 0)
                {
                    team = "red";
                    await Clients.Group(userConnection.Room).SendAsync("Fold", contador, team);
                }
                else
                {
                    team = "blue";
                    await Clients.Group(userConnection.Room).SendAsync("Fold", contador, team);
                }
                round++;
                await Clients.Group(userConnection.Room).SendAsync("NextRound", round);

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
                string team;
                if (userConnection.Player % 2 == 0)
                {
                    team = "blue";
                    await Clients.Group(userConnection.Room).SendAsync("SecondBet", contador, secondBet, team);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User} sube {secondBet} piedras.");
                }
                else
                {
                    team = "red";
                    await Clients.Group(userConnection.Room).SendAsync("SecondBet", contador, secondBet, team);
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"{userConnection.User}  sube {secondBet} piedras.");
                }
            }

        }

        //ACCION cuenta de mayor
        public async Task AccountantMayor(string[][]deck, int contador, int contadorRed, int contadorBlue)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                int[][]cleanDeck = _game.CleanCards(deck);
                            
                int mayor = _game.Mayor(cleanDeck);
               

                if (userConnection.Player % 2 == 0)
                {


                    if (mayor == 0)
                    {
                        
                        contadorRed += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana la mayor.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 11);

                    }
                    else
                    {
                        contadorBlue += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana la mayor.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 11);
                    }
                }
                else
                {
                    if (mayor == 0)
                    {
                        contadorBlue += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana la mayor.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 11);


                    }
                    else
                    {
                        contadorRed += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana la mayor.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 11);
                    }
                }
            }
        }

        //Cuenta de PEQUEÑA
        public async Task AccountantPeque(string[][] deck, int contador, int contadorRed, int contadorBlue)
        {
            int[][] cleanDeck = _game.CleanCards(deck);
            int peque = _game.Pequenia(cleanDeck);
            
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {

                if (userConnection.Player % 2 == 0)
                {

                    if (peque == 1)
                    {
                        contadorRed += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana la pequeña.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 12);
                    }
                    else
                    {
                        contadorBlue += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana la pequeña.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 12);
                    }

                }
                else
                {
                    if (peque == 1)
                    {
                        contadorBlue += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana la pequeña.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 12);
                    }
                    else
                    {
                        contadorRed += contador;
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana la pequeña.");
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 12);
                    }
                }
            }
        }
        //HAY PARES
        public async Task HayPares(string[][] deck)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                bool[] hayPares = new bool[4];
                int[][] cleanDeck = _game.CleanCards(deck);
                hayPares = _game.HayPares(cleanDeck);
                await Clients.Group(userConnection.Room).SendAsync("BoolPares", hayPares);


                string[] hayParesString = new string[4];
                for (int i = 0; i < hayPares.Length; i++)
                {
                    if (hayPares[i])
                    {
                        hayParesString[i] = "Sí, tengo pares.";
                    }
                    else
                    {
                        hayParesString[i] = "no tengo pares.";
                    }
                }
                string[] userName = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    userName[i] = _connections.Values.Where(e => e.PosicionDeVuelta == i).Select(n => n.User).FirstOrDefault();

                }
                for (int i = 0; i < 4; i++)
                {
                    
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage",  userName[i] ,hayParesString[i]);
                    Thread.Sleep(800);
                }
                if((!hayPares[0] && !hayPares[2]) || (!hayPares[1] && !hayPares[3]))
                {
                    await Clients.Group(userConnection.Room).SendAsync("NextRound", 6);
                }        
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("NextRound", 5);
                }
            }
        }
        //HAY JUEGO
        public async Task HayJuego(string[][] deck)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                bool[] hayJuego = new bool[4];
               
                int[][] cleanDeck = _game.CleanCards(deck);            
                hayJuego = _game.HayJuego(cleanDeck);
                await Clients.Group(userConnection.Room).SendAsync("BoolJuego", hayJuego);

                string[] hayJuegoString = new string[4];
                for (int i = 0; i < hayJuego.Length; i++)
                {
                    if (hayJuego[i])
                    {
                        hayJuegoString[i] = "Sí, tengo juego.";
                    }
                    else
                    {
                        hayJuegoString[i] = "no tengo juego.";
                    }
                }
                string[] userName = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    userName[i] = _connections.Values.Where(e => e.PosicionDeVuelta == i).Select(n => n.User).FirstOrDefault();

                }
                for (int i = 0; i < 4; i++)
                {       
                    await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", userName[i], hayJuegoString[i]);
                    Thread.Sleep(800);
                } 

                if (!hayJuego[0] && !hayJuego[1] && !hayJuego[2] && !hayJuego[3])
                {
                    await Clients.Group(userConnection.Room).SendAsync("NextRound", 8);
                }
                else if ((hayJuego[0] || hayJuego[2]) && (hayJuego[1] || hayJuego[3]))
                {
                    await Clients.Group(userConnection.Room).SendAsync("NextRound", 7);
                }
                else
                {
                    await Clients.Group(userConnection.Room).SendAsync("NextRound", 9);
                }
           
            }
          
        }
        //PARES
        public async Task Pares(string[][] deck, int accountant, int contadorRed, int contadorBlue)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {

                int[][] cleanDeck = _game.CleanCards(deck);
                int[] pares = new int[2];
                pares = _game.Pares(cleanDeck, accountant);
                if (userConnection.Player % 2 == 0)
                {
                    if (pares[0] == 0)
                    {
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana a pares.");
                        contadorBlue += accountant + pares[1];
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 13);


                    }
                    else
                    {
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana a pares.");
                        contadorRed += accountant + pares[1];
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 13);

                    }
                }
                else
                {
                    if (pares[0] == 1)
                    {
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana a pares.");
                        contadorBlue += accountant + pares[1];
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 13);

                    }
                    else
                    {
                        await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana a pares.");
                        contadorRed += accountant + pares[1];
                        await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                        await Clients.Group(userConnection.Room).SendAsync("NextRound", 13);
                    }
                }

            }
        }
        //Cuenta de JUEGO
        public async Task Juego(string[][] deck, int accountant, int contadorRed, int contadorBlue)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                int[][] cleanDeck = _game.CleanCards(deck);
                int[] juego = new int[2];
                juego = _game.Juego(cleanDeck, accountant);
                if (juego[1] == 0)
                {
                    await Clients.Group(userConnection.Room).SendAsync("NextRound", 15);
                }
                else
                {

                    if (userConnection.Player % 2 == 0)
                    {
                        if (juego[0] == 0)
                        {
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana a juego.");
                            contadorBlue +=  juego[1];
                            await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                            
                            await Clients.Group(userConnection.Room).SendAsync("NextJuego");
                        }
                        else
                        {
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana a juego.");
                            
                            contadorRed += juego[1];
                            await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);

                            await Clients.Group(userConnection.Room).SendAsync("NextJuego");
                        }
                    }
                    
                    else
                    {
                        if (juego[0] == 1)
                        {
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana a juego.");
                            contadorBlue += juego[1];
                            await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);

                            await Clients.Group(userConnection.Room).SendAsync("NextJuego");
                        }
                        else
                        {
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana a juego.");

                            contadorRed += juego[1];
                            await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);

                            await Clients.Group(userConnection.Room).SendAsync("NextJuego");
                        }
                    }

                }
            }
        }
        //PUNTO
        public async Task Punto(string[][] deck, int accountant, int contadorRed, int contadorBlue)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                int[][] cleanDeck = _game.CleanCards(deck);
                int[] juego = new int[2];
                juego = _game.Punto(cleanDeck, accountant);
                if(juego[0] == 0)
                {
                    contadorBlue += juego[1];
                    await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);
                    await Clients.Group(userConnection.Room).SendAsync("NextJuego");
                }
                else
                {
                    contadorRed += juego[1];
                    await Clients.Group(userConnection.Room).SendAsync("TeamAccountants", contadorRed, contadorBlue);             
                    await Clients.Group(userConnection.Room).SendAsync("NextJuego");
                }
            }
        }
        //TERMINAR JUEGO 
        public async Task TerminarJuego(string team)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("NextJuego", team);
            }

        }
        //PARTIDA TERMINADA

        //ORDAGO
        public async Task Ordago(int contador, string team)
        {

        }
        //QUIERO DE ORDAGO
        public async Task CallOrdago(int round, string[][] deck)
        {
            string winner="";
            
            if (round == 2)
            {
                if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
                {

                    int[][] cleanDeck = _game.CleanCards(deck);

                    int mayor = _game.Mayor(cleanDeck);
                    if (userConnection.Player % 2 == 0)
                    {
                        if (mayor == 0)
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO");
                        }
                        else
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL");
                        }
                    }
                    else
                    {
                        if (mayor == 0)
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana la mayor.");
                        }
                        else
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana la mayor.");
                        }
                    }
                }
                else if (round == 3)
                {
                    int[][] cleanDeck = _game.CleanCards(deck);
                    int peque = _game.Pequenia(cleanDeck);
                    if (userConnection.Player % 2 == 0)
                    {
                        if (peque == 0)
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO");
                        }
                        else
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL");
                        }
                    }
                    else
                    {
                        if (peque == 0)
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Azul gana la mayor.");
                        }
                        else
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana la mayor.");
                        }
                    }

                }
                else if (round == 5)
                {
                    int[][] cleanDeck = _game.CleanCards(deck);
                    int[] pares = new int[2];
                    pares = _game.Pares(cleanDeck, 100);
                    if (userConnection.Player % 2 == 0)
                    {
                        if (pares[0] == 1)
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO");
                        }
                        else
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL");
                        }
                    }
                    else
                    {
                        if (pares[0] == 0)
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El Equipo azul gana el juego.");
                        }
                        else
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"El equipo Rojo gana el juego.");
                        }
                    }

                }
                else if (round == 7)
                {
                    
                    int[][] cleanDeck = _game.CleanCards(deck);
                    int[] juego = new int[2];
                    juego = _game.Juego(cleanDeck, 100);
                    if (userConnection.Player % 2 == 0)
                    {
                        if (juego[0] == 1)
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO GANA EL JUEGO");
                        }
                        else
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL GANA EL JUEGO");
                        }
                    }
                    else
                    {
                        if (juego[0] == 0)
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL GANA EL JUEGO");
                        }
                        else
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO GANA EL JUEGO");
                        }
                    }
                }            
                else
                {
                    int[][] cleanDeck = _game.CleanCards(deck);
                    int[] juego = new int[2];
                    juego = _game.Punto(cleanDeck, 100);
                    if (userConnection.Player % 2 == 0)
                    {
                        if (juego[0] == 1)
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO GANA EL JUEGO");
                        }
                        else
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL GANA EL JUEGO");
                        }
                    }
                    else
                    {
                        if (juego[0] == 0)
                        {
                            winner = "AZUL";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO AZUL GANA EL JUEGO");
                        }
                        else
                        {
                            winner = "ROJO";
                            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", _botUser, $"EL EQUIPO ROJO GANA EL JUEGO");
                        }
                    }
                }
                await Clients.Group(userConnection.Room).SendAsync("NextJuego", winner);
            }
        }
        //LEVANTAR CARTAS
        public async Task LevantarCartas(string[][] deck)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room).SendAsync("LevantarCartas", deck);
                await Clients.Group(userConnection.Room).SendAsync("NextRound", 10);

            }
        }


    }   
}
