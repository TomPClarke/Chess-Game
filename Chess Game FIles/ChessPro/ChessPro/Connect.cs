using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ChessPro
{
    public class Connect
    {
        public int main() {
            IPAddress ip = System.Net.IPAddress.Any;
            TcpListener server = new TcpListener(ip, 8080);
            TcpClient client = default(TcpClient);
            string msg = "0";
            try
            {
                server.Start();

            }catch(Exception ex)
            {

            }

            while (!msg.Contains("1"))
            {
                client = server.AcceptTcpClient();
                byte[] buffer = new byte[100];
                NetworkStream stream = client.GetStream();

                stream.Read(buffer, 0, buffer.Length);

                msg = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            }
            return Int32.Parse(msg);
        }
    }
}
