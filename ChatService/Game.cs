using System;
using System.Collections.Generic;

namespace ChatService
{
    public class Game
    {

      
        public int[] CleanCards(string[][] deck)
            {
                
           
                string[] jugador0 = deck[0];
                int[] jugador0Int = new int[4];
                string[] jugador1 = deck[1];
                int[] jugador1Int = new int[4];
                string[] jugador2 = deck[2];
                int[] jugador2Int = new int[4];
                string[] jugador3 = deck[3];
                int[] jugador3Int = new int[4];



                for (int i = 0; i < 4; i++)
                {

                    int cardInt = Int32.Parse(jugador0[i].Remove(0, 1));
                    if (cardInt == 3)
                    {
                        cardInt = 12;
                    }
                    else if (cardInt == 2)
                    {
                        cardInt = 1;
                    }
                    jugador0Int[i] = cardInt;

                }

                Array.Sort(jugador0Int);
                Array.Reverse(jugador0Int);



                for (int i = 0; i < 4; i++)
                {

                    int cardInt = Int32.Parse(jugador1[i].Remove(0, 1));
                    if (cardInt == 3)
                    {
                        cardInt = 12;
                    }
                    else if (cardInt == 2)
                    {
                        cardInt = 1;
                    }
                    jugador1Int[i] = cardInt;

                }

                Array.Sort(jugador1Int);
                Array.Reverse(jugador1Int);



                for (int i = 0; i < 4; i++)
                {

                    int cardInt = Int32.Parse(jugador2[i].Remove(0, 1));
                    if (cardInt == 3)
                    {
                        cardInt = 12;
                    }
                    else if (cardInt == 2)
                    {
                        cardInt = 1;
                    }
                    jugador2Int[i] = cardInt;

                }

                Array.Sort(jugador2Int);
                Array.Reverse(jugador2Int);



                for (int i = 0; i < 4; i++)
                {

                    int cardInt = Int32.Parse(jugador3[i].Remove(0, 1));
                    if (cardInt == 3)
                    {
                        cardInt = 12;
                    }
                    else if (cardInt == 2)
                    {
                        cardInt = 1;
                    }
                    jugador3Int[i] = cardInt;
                }

                Array.Sort(jugador3Int);
                Array.Reverse(jugador3Int);
                int aMayor=Mayor(jugador0Int, jugador1Int, jugador2Int, jugador3Int);

                int aPequenia = Pequenia(jugador0Int, jugador1Int, jugador2Int, jugador3Int);

                int[] mayorPeque= new int[2] {aMayor, aPequenia};
                
                return  mayorPeque;


            }
        public int Mayor(int[] jugador0Int, int[] jugador1Int, int[] jugador2Int, int[] jugador3Int)
        {
            //comparar jugador 0 con jugador 2 para saber quien es el mejor

            int[] parTeamBest = new int[4];
            int[] oddTeamBest = new int[4];
            int posiPar = -1;
            int contador = 0;
            int contador1 = 0;
            int contador2 = 0;
            int posiOdd = -1;

            for (int i = 0; i < 4; i++)
            {
                if (jugador0Int[i] != jugador2Int[i])
                {
                    if (jugador0Int[i] > jugador2Int[i])
                    {
                        parTeamBest = jugador0Int;
                        posiPar = 0;
                        break;
                    }
                    else
                    {
                        parTeamBest = jugador2Int;
                        posiPar = 2;
                        break;
                    }
                }
                else
                {
                    contador++;
                }
            }
            if (contador == 4)
            {
                parTeamBest = jugador0Int;
                posiPar = 0;

            }





            for (int i = 0; i < 4; i++)
            {
                if (jugador1Int[i] != jugador3Int[i])
                {
                    if (jugador1Int[i] > jugador3Int[i])
                    {
                        oddTeamBest = jugador1Int;
                        posiOdd = 1;
                        break;
                    }
                    else
                    {
                        oddTeamBest = jugador3Int;
                        posiOdd = 3;
                        break;
                    }
                }
                else
                {
                    contador1++;
                }
            }
            if (contador1 == 4)
            {
                oddTeamBest = jugador1Int;
                posiPar = 0;


            }


            bool parTeamWins = false;
            bool oddTeamWins = false;


            for (int i = 0; i < 4; i++)
            {
                if (parTeamBest[i] != oddTeamBest[i])
                {
                    if (parTeamBest[i] > oddTeamBest[i])
                    {
                        parTeamWins = true;
                        break;
                    }
                    else
                    {
                        oddTeamWins = true;
                        break;
                    }
                }
                else
                {
                    contador2++;
                }
            }
            if (contador2 == 4)
            {
                if (posiPar < posiOdd)
                {
                    parTeamWins = true;
                }
                else
                {
                    oddTeamWins = true;
                }
            }

            if (parTeamWins)
            {
                return 0;
            }
            else
            {
                return 1;
            }
            
            
        }
        public int Pequenia(int[] jugador0Int, int[] jugador1Int, int[] jugador2Int, int[] jugador3Int)
        {
            //comparar jugador 0 con jugador 2 para saber quien es el mejor
            Array.Sort(jugador0Int);
            Array.Sort(jugador1Int);
            Array.Sort(jugador2Int);
            Array.Sort(jugador3Int);


            int[] parTeamBest = new int[4];
            int posiPar = -1;
            int contador = 0;
            int contador1 = 0;
            int contador2 = 0;
            int posiOdd = -1;

            for (int i = 0; i < 4; i++)
            {
                if (jugador0Int[i] != jugador2Int[i])
                {
                    if (jugador0Int[i] < jugador2Int[i])
                    {
                        parTeamBest = jugador0Int;
                        posiPar = 0;
                        break;
                    }
                    else
                    {
                        parTeamBest = jugador2Int;
                        posiPar = 2;
                        break;
                    }
                }
                else
                {
                    contador++;
                }
            }
            if (contador == 4)
            {
                posiPar = 0;
                parTeamBest = jugador0Int;
            }
            int[] oddTeamBest = new int[4];


            for (int i = 0; i < 4; i++)
            {
                if (jugador1Int[i] != jugador3Int[i])
                {
                    if (jugador1Int[i] < jugador3Int[i])
                    {
                        oddTeamBest = jugador1Int;
                        posiOdd = 1;
                        break;
                    }
                    else
                    {
                        oddTeamBest = jugador3Int;
                        posiOdd = 3;
                        break;
                    }
                }
                else
                {
                    contador1++;
                }
            }
            if (contador1 == 4)
            {
                oddTeamBest = jugador1Int;
                posiOdd = 1;

            }

            bool parTeamWins = false;
            bool oddTeamWins = false;


            for (int i = 0; i < 4; i++)
            {
                if (parTeamBest[i] != oddTeamBest[i])
                {
                    if (parTeamBest[i] < oddTeamBest[i])
                    {
                        parTeamWins = true;
                        break;
                    }
                    else
                    {
                        oddTeamWins = true;
                        break;
                    }
                }
                else
                {
                    contador2++;
                }
            }

            if (contador2 == 4)
            {
                if (posiPar < posiOdd)
                {
                    parTeamWins = true;
                }
                else
                {
                    oddTeamWins = true;
                }
            }

            if (parTeamWins)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }



    }
}


  

