using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.ViewModel
{
    public class ClientVM : INotifyPropertyChanged
    {
        private object _lock = new object();
        private ObservableCollection<string> _str;
        public ObservableCollection<string> ListString { get { return _str; } set { _str = value; OnNotify("ListString"); } }

        private string _ip;
        public string IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
            }
        }

        private int _port;
        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
            }
        }
        private string _msg;
        public string Msg
        {
            get { return _msg; }
            set
            {
                _msg = value;
                OnNotify("Msg");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotify([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public TcpClient client;
        public StreamReader sr;
        public StreamWriter sw;
        public ClientVM()
        {
            ListString = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(ListString, _lock);
            IP = "127.0.0.1";
            Port = 2020;
            Username = "User";
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                 try
                 {
                    if (client?.Connected == true)
                    {                          
                       string line = sr.ReadLine();
                       if (line != null)
                       {
                          ListString.Add($"{line}");
                       }
                       else
                       {
                          client.Close();
                        }
                    }
                    Task.Delay(10).Wait();
                 }
                 catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            });
        }
        public AsyncCommand ConnectCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {

                    return Task.Factory.StartNew(() =>
                    {
                        if (client == null || client?.Connected == false)
                        {
                            try
                            {
                                client = new TcpClient();
                                client.Connect(IP, Port);
                                
                                sr = new StreamReader(client.GetStream());
                                sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;

                                sw.WriteLine($"{Username}!!!");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    });
                }, () => client == null || client?.Connected == false);
            }
        }
        public AsyncCommand SendCommand
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            sw.WriteLine($"[{Username}]: {Msg}"); //Відправляє повідомлення на сервер.
                            Msg = "";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });
                }, () => client?.Connected == true, !string.IsNullOrWhiteSpace(Msg));
            }
        }
    }
}
