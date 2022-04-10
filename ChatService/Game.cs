using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatService
{
    public class Game
    {
         
      
        public int[][] CleanCards(string[][] deck)
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


            int[][] cleandDeck = { jugador0Int, jugador1Int, jugador2Int, jugador3Int};
                
                return  cleandDeck;


            }
        public int Mayor(int[][] cleanCards)
        {
            //comparar jugador 0 con jugador 2 para saber quien es el mejor
            int[] jugador0Int;
            int[] jugador1Int;
            int[] jugador2Int;
            int[] jugador3Int;
            jugador0Int = cleanCards[0];
            jugador1Int = cleanCards[1];
            jugador2Int = cleanCards[2];
            jugador3Int = cleanCards[3];
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
        public int Pequenia(int[][] cleanCards)
        {
            int[] jugador0Int;
            int[] jugador1Int;
            int[] jugador2Int;
            int[] jugador3Int;
            jugador0Int = cleanCards[0];
            jugador1Int = cleanCards[1];
            jugador2Int = cleanCards[2];
            jugador3Int = cleanCards[3];
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
        public bool[] HayPares(int[][] cleanCards)
        {
      
            bool[] hayPares = new bool[4] { false, false, false, false };
            for (int e = 0; e < 4; e++)
            {

                for (int i = 0; i < 3; i++)
                {
                    if (cleanCards[e][i] == cleanCards[e][i + 1])
                    {
                        hayPares[e] = true;
                    }
                }
            }
            return hayPares;
        }
        public int[] Pares(int[][] cleanCards, int accountant)
        {
            //quiero saber la besthand de cada equipo

            int[] winner = new int[2];
            int[] empate = new int[4] { 0, 0, 0, 0 };
            bool parTeamWin = false;
            bool oddTeamWin = false;
            int[] bestParesParTeam;
            int bestParesParTeamPosi;
            int[] bestParesOddTeam;
            int bestParesOddTeamPosi;
            bool[] hayPares = new bool[4] { false, false, false, false };
            bool alguienPar = false;
            for (int e = 0; e < 4; e++)
            {

                for (int i = 0; i < 3; i++)
                {
                    if (cleanCards[e][i] == cleanCards[e][i + 1])
                    {
                        hayPares[e] = true;
                    }
                }
            }
            foreach (var cards in hayPares)
            {
                if (cards)
                {
                    alguienPar = true;
                }
            }
            if (alguienPar)
            {

                if ((hayPares[0] || hayPares[2]) && (hayPares[1] || hayPares[3]))
                {
                    if (hayPares[0] && hayPares[2])
                    {
                        bestParesParTeam = peleaPares(cleanCards[0], cleanCards[2]);
                        if (bestParesParTeam == cleanCards[0])
                        {
                            bestParesParTeamPosi = 0;
                        }
                        else
                        {
                            bestParesParTeamPosi = 2;
                        }
                    }
                    else
                    {
                        if (hayPares[0])
                        {
                            bestParesParTeam = cleanCards[0];
                            bestParesParTeamPosi = 0;

                        }
                        else
                        {
                            bestParesParTeam = cleanCards[2];
                            bestParesParTeamPosi = 2;
                        }
                    }
                    if (hayPares[1] && hayPares[3])
                    {
                        bestParesOddTeam = peleaPares(cleanCards[1], cleanCards[3]);
                        if (bestParesOddTeam == cleanCards[1])
                        {
                            bestParesOddTeamPosi = 1;
                        }
                        else
                        {
                            bestParesOddTeamPosi = 3;
                        }
                    }
                    else
                    {
                        if (hayPares[1])
                        {
                            bestParesOddTeam = cleanCards[1];
                            bestParesOddTeamPosi = 1;

                        }
                        else
                        {
                            bestParesOddTeam = cleanCards[3];
                            bestParesOddTeamPosi = 3;
                        }
                    }
                    int[] paresGanadores = new int[4];
                    paresGanadores = peleaPares(bestParesParTeam, bestParesOddTeam);

                    if (paresGanadores.SequenceEqual(empate))
                    {
                        if (bestParesOddTeamPosi > bestParesParTeamPosi)
                        {
                            parTeamWin = true;
                            winner[0] = 0;
                            Console.WriteLine("gana par team con empate");

                        }
                        else
                        {
                            winner[0] = 1;
                            oddTeamWin = true;
                            Console.WriteLine("gana impar team con empate");
                        }
                    }
                    if (paresGanadores == bestParesOddTeam)
                    {
                        winner[0] = 1;
                        oddTeamWin = true;
                        Console.WriteLine("gana impar team a pares");
                    }
                    else
                    {
                        winner[0] = 0;
                        parTeamWin = true;
                        Console.WriteLine("gana par team a pares");
                    }
                }
                else if ((hayPares[0] || hayPares[2]))
                {
                    winner[0] = 0;
                    parTeamWin = true;
                    Console.WriteLine("par team gana pares");
                }
                else if ((hayPares[1] || hayPares[3]))
                {
                    winner[0] = 1;
                    oddTeamWin = true;
                    Console.WriteLine("impar team gana pares");
                }
                else
                {

                    return winner;
                }
                if (parTeamWin)
                {
                    int mano = checkPares(cleanCards[0]);
                    int tercero = checkPares(cleanCards[2]);
                    int piedrasPares = mano + tercero;
                    accountant += piedrasPares;
                    winner[1]=accountant;
                    return winner;
                }
                else
                {
                    int segundo = checkPares(cleanCards[1]);
                    int postre = checkPares(cleanCards[3]);
                    int piedrasPares = segundo + postre;
                    accountant += piedrasPares;
                    winner[1] = accountant;
                    return winner;
                }
                static int checkPares(int[] cards)
                {
                    bool duples = false;
                    bool medias = false;
                    bool pares = false;
                    if (cards[0] == (cards[1]) && cards[2] == (cards[3]))
                    {
                        duples = true;
                    }
                    else if ((cards[0] == cards[1] && cards[1] == cards[2]) || (cards[1] == cards[2] && cards[2] == cards[3]))
                    {
                        medias = true;
                    }
                    else if (cards[0] == cards[1] || cards[1] == cards[2] || cards[2] == cards[3])
                    {
                        pares = true;
                    }
                    if (duples)
                    {
                        return 3;
                    }
                    else if (medias)
                    {
                        return 2;
                    }
                    else if (pares)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return winner;
            }
        }
        public int[] peleaPares(int[] mano0, int[] mano1)
        {
            int[] bestHand = new int[4];
            bool duples0 = false;
            bool medias0 = false;
            bool duples1 = false;
            bool medias1 = false;
            int[] empate = new int[4] { 0, 0, 0, 0 };

            //DUPLES

            if (mano0[0] == (mano0[1]) && mano0[2] == (mano0[3]))
            {
                duples0 = true;
            }
            else if (mano0[0] == mano0[1] && mano0[1] == (mano0[2]) || mano0[1] == mano0[2] && mano0[2] == (mano0[3]))
            {
                medias0 = true;
            }
            if (mano1[0] == (mano1[1]) && mano1[2] == (mano1[3]))
            {
                duples1 = true;
            }
            else if (mano1[0] == mano1[1] && mano1[1] == (mano1[2]) || mano1[1] == mano1[2] && mano1[2] == (mano1[3]))
            {
                medias1 = true;
            }
            if (duples0)
            {
                if (!duples1)
                {
                    bestHand = mano0;
                }
                else
                {
                    bestHand = peleaDuples(mano0, mano1);

                }
            }
            else if (duples1 && !duples0)
            {
                bestHand = mano1;
            }
            //MEDIAS
            else if (medias0)
            {
                if (!medias1)
                {
                    bestHand = mano0;
                }
                else
                {
                    if (mano0[1] > mano1[1])
                    {
                        bestHand = mano0;
                    }
                    else if (mano1[1] > mano0[1])
                    {
                        bestHand = mano1;
                    }
                    else
                    {
                        bestHand = empate;
                    }
                }
            }
            //PARES
            else
            {
                int par0;
                int par1;

                if (mano0[0] == mano0[1] || mano0[1] == mano0[2])
                {
                    par0 = mano0[1];
                }
                else
                {
                    par0 = mano0[2];
                }
                if (mano1[0] == mano1[1] || mano1[1] == mano1[2])
                {
                    par1 = mano1[1];
                }
                else
                {
                    par1 = mano1[2];
                }
                if (par0 > par1)
                {
                    bestHand = mano0;
                }
                else if (par1 > par0)
                {
                    bestHand = mano1;
                }
                else
                {
                    bestHand = empate;
                }
            }
            return bestHand;
        }
        public int[] peleaDuples(int[] mano0, int[] mano1)
        {
            if (mano0[0] != (mano1[0]))
            {
                if (mano0[0] > mano1[0])
                {
                    return mano0;
                }
                else
                {
                    return mano1;
                }

            }
            else
            {
                if (mano0[2] != (mano1[2]))
                {
                    if (mano0[2] > mano1[2])
                    {
                        return mano0;
                    }
                    else
                    {
                        return mano1;
                    }
                }
                else
                {
                    int[] empate = new int[4] { 0, 0, 0, 0 };
                    return empate;
                }
            }
        }
        public bool[] HayJuego(int[][] cleanCards)
        {
            bool[] hayJuego = new bool[4] { false, false, false, false };
            for (int e = 0; e < 4; e++)
            {
                for (int i = 0; i < 4; i++)
                {

                    if (cleanCards[e][i] == 12)
                    {
                        cleanCards[e][i] = 10;
                    }
                    else if (cleanCards[e][i] == 11)
                    {
                        cleanCards[e][i] = 10;
                    }
                }
            }
            int[] juego = new int[4];


            for (int e = 0; e < 4; e++)
            {
                juego[e] = Enumerable.Sum(cleanCards[e]);
                if (juego[e] > 30)
                {
                    hayJuego[e] = true;
                }
            }
            return hayJuego;
        }
        public int[] Juego(int[][] cleanCards, int accountant)
        {
            int[] result = new int[2] { 0, 0 };
            int[] empate = new int[4] { 0, 0, 0, 0 };
            bool parTeamWin = false;
            bool oddTeamWin = false;
            bool[] hayJuego = new bool[4] { false, false, false, false };
            bool alguienJuego = false;
            int bestJuegoParTeam = 0;
            int bestJuegoOddTeam = 0;
            int bestJuegoParTeamPosi;
            int bestJuegoOddTeamPosi;
            for (int e = 0; e < 4; e++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (cleanCards[e][i] == 12)
                    {
                        cleanCards[e][i] = 10;
                    }
                    else if (cleanCards[e][i] == 11)
                    {
                        cleanCards[e][i] = 10;
                    }
                }
            }
            int[] juego = new int[4];


            for (int e = 0; e < 4; e++)
            {
                juego[e] = Enumerable.Sum(cleanCards[e]);
                if (juego[e] > 30)
                {
                    hayJuego[e] = true;
                }
            }
            foreach (var cards in hayJuego)
            {
                if (cards)
                {
                    alguienJuego = true;
                }
            }
            if (alguienJuego)
            {

                if ((hayJuego[0] || hayJuego[2]) && (hayJuego[1] || hayJuego[3]))
                {
                    if (hayJuego[0] && hayJuego[2])
                    {
                        if (juego[0] == juego[2])
                        {
                            bestJuegoParTeamPosi = 0;
                        }
                        else
                        {
                            bestJuegoParTeam = peleaJuego(juego[0], juego[2]);
                            if (bestJuegoParTeam == juego[0])
                            {
                                bestJuegoParTeamPosi = 0;
                            }
                            else
                            {
                                bestJuegoParTeamPosi = 2;
                            }
                        }
                    }
                    else
                    {
                        if (hayJuego[0])
                        {
                            bestJuegoParTeam = juego[0];
                            bestJuegoParTeamPosi = 0;
                        }
                        else
                        {
                            bestJuegoParTeam = juego[2];
                            bestJuegoParTeamPosi = 2;
                        }
                    }
                    if (hayJuego[1] && hayJuego[3])
                    {
                        if (juego[1] == juego[3])
                        {
                            bestJuegoOddTeamPosi = 1;
                        }
                        else
                        {
                            bestJuegoOddTeam = peleaJuego(juego[1], juego[3]);
                            if (bestJuegoOddTeam == juego[1])
                            {
                                bestJuegoOddTeamPosi = 1;
                            }
                            else
                            {
                                bestJuegoOddTeamPosi = 3;
                            }
                        }
                    }
                    else
                    {
                        if (hayJuego[1])
                        {
                            bestJuegoOddTeam = juego[1];
                            bestJuegoOddTeamPosi = 1;

                        }
                        else
                        {
                            bestJuegoOddTeam = juego[3];
                            bestJuegoOddTeamPosi = 3;
                        }
                    }
                    if (bestJuegoParTeam == bestJuegoOddTeam)
                    {
                        if (bestJuegoParTeamPosi < bestJuegoOddTeamPosi)
                        {
                            parTeamWin = true;
                            Console.WriteLine("gana par team a juego con empate");
                        }
                        else
                        {
                            oddTeamWin = true;
                            Console.WriteLine("gana impar team a juego con empate");
                        }
                    }
                    else
                    {
                        int juegoGanador;
                        juegoGanador = peleaJuego(bestJuegoParTeam, bestJuegoOddTeam);

                        if (juegoGanador == bestJuegoOddTeam)
                        {
                            oddTeamWin = true;
                            Console.WriteLine("gana impar team a juego");
                        }
                        else
                        {
                            parTeamWin = true;
                            Console.WriteLine("gana par team a juego");
                        }
                    }
                }
                else if ((hayJuego[0] || hayJuego[2]))
                {
                    parTeamWin = true;
                    Console.WriteLine("par team gana juego");
                }
                else if ((hayJuego[1] || hayJuego[3]))
                {

                    oddTeamWin = true;
                    Console.WriteLine("impar team gana juego");
                }
                else
                {
                    return result;
                }
                if (parTeamWin)
                {
                    int mano = checkJuego(juego[0]);
                    int tercero = checkJuego(juego[2]);
                    int piedrasPares = mano + tercero;
                    accountant += piedrasPares;
                    result[0] = 0;
                    result[1] = accountant;
                    return result;
                }
                else
                {
                    int segundo = checkJuego(juego[1]);
                    int postre = checkJuego(juego[3]);
                    int piedrasPares = segundo + postre;
                    accountant += piedrasPares;
                    result[0] = 1;
                    result[1] = accountant;

                    return result;
                }
                static int checkJuego(int cards)
                {
                    if (cards == 31)
                    {
                        return 3;
                    }
                    else if (cards > 31)
                    {
                        return 2;
                    }
                    else return 0;
                }
            }
            else
            {
                return result;
            }
        }
        public int peleaJuego(int juego0, int juego1)
        {
            if (juego0 == 31 && juego1 != 31)
            {
                return juego0;
            }
            else if (juego1 == 31 && juego0 != 31)
            {
                return juego1;
            }
            else if (juego0 == 32 && juego1 != 32)
            {
                return juego0;
            }
            else if (juego1 == 32 && juego0 != 32)
            {
                return juego1;
            }
            else if (juego0 > juego1)
            {
                return juego0;
            }
            else
            {
                return juego1;
            }
        }
        public int[] Punto(int[][] cleanCards, int accountant)
        {
            int[] result = new int[2];
            int bestPuntoPosi;
            for (int e = 0; e < 4; e++)
            {
                for (int i = 0; i < 4; i++)
                {

                    if (cleanCards[e][i] == 12)
                    {
                        cleanCards[e][i] = 10;
                    }
                    else if (cleanCards[e][i] == 11)
                    {
                        cleanCards[e][i] = 10;
                    }
                }
            }
            int[] juego = new int[4];


            for (int e = 0; e < 4; e++)
            {
                juego[e] = Enumerable.Sum(cleanCards[e]);
            }

            bestPuntoPosi = Array.IndexOf(juego, Enumerable.Max(juego));
            if (bestPuntoPosi == 0 || bestPuntoPosi == 2)
            {
                accountant += 1;
                Console.WriteLine("par team gana al punto   " + accountant);
                result[0] = 0;
                result[1] = accountant;
                return result;
            }
            else
            {
                accountant += 1;
                Console.WriteLine("impar team gana al punto   " + accountant);
                result[0] = 1;
                result[1] = accountant;
                return result;
            }

        }

    }




}



  

