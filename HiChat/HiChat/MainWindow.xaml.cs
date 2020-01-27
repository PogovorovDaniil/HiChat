using System;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using GigCS;

namespace HiChar
{
    public partial class MainWindow : Window
    {
		private readonly BackgroundWorker worker = new BackgroundWorker();
        delegate void Add(string s);
		CS client;
		private void Listen(object sender, EventArgs e)
        {
            try
            {
                while (true)
                {
					Message mess = client.WaitMessage();
					AddToStory(mess.from + " >> " + mess.to + " : " + mess.text);
                }
            }
            catch
            {
				AddToStory("Ошибка при получении сообщения!");
            }
        }

		private void AddToStory(string Text)
		{
			story.Dispatcher.Invoke(new Add((s) =>
			{
				story.Text += s;
				Scroll.ScrollToEnd();
			}), Text + "\n");
		}
		private void SendButtonClick(object sender, RoutedEventArgs e)
        {
			Message newMess = new Message(client.name, tbId.Text, tbMess.Text);
			if (client.Talk(newMess))
			{
				AddToStory("To " + newMess.to + " : " + newMess.text);
				tbMess.Text = "";
			}
			else
			{
				AddToStory("Произошла ошибка при отправке!");
			}
        }

        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += Listen;
			try
			{
				FileStream fs = new FileStream("info", FileMode.Open);
				StreamReader sr = new StreamReader(fs);
				client = new CS(sr.ReadLine(), sr.ReadLine());
				if (!client.init) Close();
				sr.Close();
				fs.Close();
				numCl.Text = client.name;
				
				worker.RunWorkerAsync();
			}
			catch
			{
				Close();
			}
        }

		private void PressKey(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) SendButtonClick(sender, new RoutedEventArgs());
		}
	}
}