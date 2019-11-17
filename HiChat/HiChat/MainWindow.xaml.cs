using System;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;


namespace ClientWPF
{
    public partial class MainWindow : Window
    {
        delegate void Add(string s);

        IPEndPoint ipPoint;
        Socket socket;

        byte clientId = 0;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private void Listen(object sender, EventArgs e)
        {
            try
            {
                byte[] data;
                while (true)
                {
                    data = new byte[256];
                    socket.Receive(data, data.Length, 0);
                    story.Dispatcher.Invoke(new Add((s) => story.Text += s), data[0] + ": ");
                    string message = Encoding.Unicode.GetString(data, 1, data.Length - 2);
                    story.Dispatcher.Invoke(new Add((s) => story.Text += s), message + "\n");
                }
            }
            catch (Exception ex)
            {
                story.Dispatcher.Invoke(new Add((s) => story.Text += s), ex.Message + "\n");
            }
        }

        private void Talk(byte To, string mess)
        {
            try
            {
                byte client = To;
                string message = mess;
                byte[] str = Encoding.Unicode.GetBytes(message);
                byte[] data = new byte[str.Length + 1];
                for (int i = 1; i < data.Length; i++)
                {
                    data[i] = str[i - 1];
                }
                data[0] = client;
                socket.Send(data);
            }
            catch (Exception ex)
            {
                story.Text += ex.Message + '\n';
            }
        }

        static int port = 9090; 
        static string address = "185.251.38.207";
        //static string address = "127.0.0.1";

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            string message = tbMess.Text;
            int id;
            if (int.TryParse(tbId.Text, out id)) Talk((byte)id, message);
            tbMess.Text = "";
        }

        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += Listen;
            try
            {
                ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                byte[] data = new byte[256];
                int bytes = 0;
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    clientId = data[0];
                    numCl.Text = Convert.ToString((int)clientId);
                }
                while (socket.Available > 0);


                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
