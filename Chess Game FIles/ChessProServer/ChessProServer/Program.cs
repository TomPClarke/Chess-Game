using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace ChessProServer
{
    class Program
    {
        static int port = 11202;
        static TcpClient white = default(TcpClient);
        static TcpClient black = default(TcpClient);
        static NetworkStream white_stream = default(NetworkStream);
        static NetworkStream black_stream = default(NetworkStream);
        static int[,] gameboard = new int[,]
{
            {-2,-3,-4,-5,-6,-4,-3,-2},
            {-1,-1,-1,-1,-1,-1,-1,-1},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1},
            {2,3,4,5,6,4,3,2}};
        private static void update(string msg)
        {
            Remove_EnPassant();
            int piece = 0;
            msg = msg.Substring(1, msg.IndexOf("#") - 1);
            int x = Int32.Parse(msg[0].ToString());
            int y = Int32.Parse(msg[1].ToString());
            int oldx = Int32.Parse(msg[2].ToString());
            int oldy = Int32.Parse(msg[3].ToString());
            piece = Int32.Parse(msg.Substring(4));
            if(piece == -14 || piece == 14)
            {
                piece /= 2;
                gameboard[y + (piece / 7), x] = 0;
            }
            gameboard[y, x] = piece;
            gameboard[oldx, oldy] = 0;
        }
        private static void Remove_EnPassant()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (gameboard[i, k] == 10)
                    {
                        gameboard[i, k] = 7;
                    }
                    if (gameboard[i, k] == -10)
                    {
                        gameboard[i, k] = -7;
                    }
                }
            }
        }
        private static string boardasstring()
        {
            string boardstring = "";
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    boardstring += gameboard[i, k].ToString() + "#";
                }
            }
            return boardstring;
        }
        private static void InitServer()
        {

        }
        private static byte[] bites(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }
        private static void Main()
        {
            InitServer();
            IPAddress ip = System.Net.IPAddress.Any;
            TcpListener server = new TcpListener(ip, port);
            try
            {
                server.Start();
                Console.WriteLine("Attempting to find player...");
                white = server.AcceptTcpClient();
                white_stream = white.GetStream();
                white_stream.Write(bites("white"), 0, 5);
                white_stream.Flush();
                Console.WriteLine("Player one found!");
                Console.WriteLine("Attempting to find second player...");
                black = server.AcceptTcpClient();
                black_stream = black.GetStream();
                black_stream.Write(bites("black"), 0, 5);
                black_stream.Flush();
                white_stream.Write(bites("start"), 0, 5);
                white_stream.Flush();
                black_stream.Write(bites("start"), 0, 5);
                black_stream.Flush();
                Console.WriteLine("Player two found!");
                white.ReceiveTimeout = 5000;
                black.ReceiveTimeout = 5000;
             }
            catch (Exception ex)
            {
                return;
            }
            while(black.Connected && white.Connected)
            {
                byte[] buffer = new byte[10];
                byte[] board = new byte[500];
                if (white.Client.Poll(100, SelectMode.SelectRead))
                {
                    white_stream.Read(buffer, 0, buffer.Length);
                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                    update(Encoding.ASCII.GetString(buffer));
                    string boardstring = boardasstring();
                    board = Encoding.ASCII.GetBytes(boardstring);
                    black_stream.Write(board, 0, board.Length);
                        
                }
                if (black.Client.Poll(100, SelectMode.SelectRead)) 
                { 
                    black_stream.Read(buffer, 0, buffer.Length);
                    Console.WriteLine(Encoding.ASCII.GetString(buffer));
                    update(Encoding.ASCII.GetString(buffer));
                    string boardstring = boardasstring();
                    board = Encoding.ASCII.GetBytes(boardstring);
                    white_stream.Write(board, 0, board.Length);
                }
                 
            }

        }
    }
}