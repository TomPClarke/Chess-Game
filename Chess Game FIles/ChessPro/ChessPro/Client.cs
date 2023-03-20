using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace ChessPro
{
    public class Client
    {
        public bool myturn;
        public string serverip = "192.168.0.87";
        public int port = 19001;
        public string name = "";
        TcpClient link = new TcpClient();
        NetworkStream stream = default(NetworkStream);
        public bool found = false;
        public string hostip;
        public Client()
        {
            link.ReceiveTimeout = 10000;
        }
        public bool connect(string ip = "127.0.0.1")
        {
            try
            {
                link.Connect(serverip, port);
                stream = link.GetStream();
                byte[] buffer = new byte[5];
                stream.Read(buffer, 0, 5);
                name = Encoding.ASCII.GetString(buffer);
                if(name == "black") { myturn = false; }
                else { myturn = true; }
                found = true;
                try {stream.Read(buffer, 0, 5); }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                found = false;
                return false;
            }
        }
        public void send(int x, int y, int oldx, int oldy, int piece)
        {
            if (found)
            {
                string message = 
                    name[0].ToString()
                    + x.ToString() 
                    + y.ToString() 
                    + oldx.ToString() 
                    + oldy.ToString() 
                    + piece.ToString() 
                    + "#";
                byte[] send = (Encoding.ASCII.GetBytes(message));
                stream.Write(send, 0, send.Length);
                stream.Flush();
            }
        }
        public int[,] Checkforboard() 
        {
            byte[] board_buffer = new byte[500];
            if (link.Client.Poll(2500, SelectMode.SelectRead))
            { 
                stream.Read(board_buffer, 0, board_buffer.Length); 
            }
            else { return null; }
            myturn = true;
            int[,] boardint = new int[8, 8];
            string stringboard = Encoding.ASCII.GetString(board_buffer, 0, board_buffer.Length);
            int x = 0, y = 0;
            string piece = "";
            for (int i = 0; i < stringboard.Length; i++) //expects 2#3#4#5#6#4#3#2#
            {
                if (stringboard[i].ToString() == "#" && y < 8)
                {
                    boardint[y, x] = Int32.Parse(piece);
                    x++;
                    if (x > 7)
                    {
                        y++;
                        x = 0;
                    }
                    piece = "";
                }
                else
                {
                    piece += stringboard[i].ToString();
                }
            }
            return boardint;
        }
        public int[,] decodeboard(string board)
        {
            if(board == null)
            {
                return null;
            }
            string stringboard;
            int[,] boardint = new int[8, 8];
            byte[] buffer = new byte[500];
            if (link.Client.Poll(2500, SelectMode.SelectRead)) { stream.Read(buffer, 0, buffer.Length); }
            else { return null; }
            stringboard = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            int x = 0, y = 0;
            string piece = "";
            for (int i = 0; i < stringboard.Length; i++) //expects 2#3#4#5#6#4#3#2#
            {
                if(stringboard[i].ToString() == "#" && y < 8)
                {
                    boardint[y, x] = Int32.Parse(piece);
                    x++;
                    if(x > 7)
                    {
                        y++;
                        x = 0;
                    }
                    piece = "";
                }else
                {
                    piece += stringboard[i].ToString();
                }
            }
            return boardint;
        }
    }
}
 