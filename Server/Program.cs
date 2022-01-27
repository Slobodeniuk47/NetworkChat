using Client.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 2020);
        static List<User> clients = new List<User>();
        static void Main(string[] args)
        {
            Console.Title = "Server";
            listener.Start();
            Console.WriteLine("Server has been started!");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Task.Factory.StartNew(() =>
                {
                    var sr = new StreamReader(client.GetStream());
                    while (client.Connected)
                    {
                        var line = sr.ReadLine();
                        var nick = line;
                            if(clients.FirstOrDefault(s=> s.Name == nick) == null)
                            {
                                clients.Add(new User(client, nick));
                                Console.WriteLine($"New connection: {nick}");
                                SendToAllClients("Connected: " + nick);
                                break;
                            }
                            else
                            {
                                var sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;
                                sw.WriteLine("User is online! Change User!");
                                client.Client.Disconnect(false);
                            }                       
                    }

                    while (client.Connected)
                    {
                        try
                        {
                            sr = new StreamReader(client.GetStream());
                            var line = sr.ReadLine();
                            SendToAllClients(line);
                            Console.WriteLine(line);                          
                        }
                        catch (Exception) { }
                    }
                });
                
            }
        }
        static async void SendToAllClients(string msg)
        {
            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        var sw = new StreamWriter(clients[i].Client.GetStream());
                        sw.AutoFlush = true;
                        sw.WriteLine(msg);
                    }
                    catch (Exception) { }
                }
            });
        }
    }
}
