using System.Net.Sockets;

namespace Client.Models
{
    public class User
    {
        public TcpClient Client { get; set; }
        public string Name { get; set; }
        
        public User(TcpClient client, string name)
        {
            Client = client;
            Name = name;
        }
    }
}
