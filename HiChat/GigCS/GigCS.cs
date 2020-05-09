using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace GigCS
{
	public class CS
	{
		private int port;
		private string address;
		public string name;
		private const int LenMess = 100;
		private const int LenData = 260;
		public bool init;


		private IPEndPoint ipPoint;
		private Socket socket;
		private RSACryptoServiceProvider RSAself;
		private RSACryptoServiceProvider RSA;
		public CS(string name, string key)
		{
			RSAself = new RSACryptoServiceProvider();
			RSA = new RSACryptoServiceProvider();
			address = "193.178.169.223";
			port = 9090;
			this.name = name;
			RSAself.FromXmlString(key);
			init = InitClient();
		}

		public bool InitClient()
		{
			try
			{
				ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect(ipPoint);
				UserInfo info = InfoFromName(name);
				if (info.id == 0) return false;
				uint id = info.id;
				byte[] data = new byte[4];
				AddIdToData(ref data, id);
				socket.Send(data);

				data = new byte[LenData - 4];
				socket.Receive(data, data.Length, 0);
				byte[] decry = RSAself.Decrypt(data, false);
				socket.Send(decry);
				//string message = Encoding.UTF8.GetString(encry, 0, encry.Length);
				//message = message.Replace((char)0, ' ');
				//Console.WriteLine(message);

				return true;
			}
			catch
			{
				return false;
			}
		}
		private void AddIdToData(ref byte[] data, uint id)
		{
			data[0] = (byte)((id) % 256);
			data[1] = (byte)((id / 256) % 256);
			data[2] = (byte)((id / 256 / 256) % 256);
			data[3] = (byte)((id / 256 / 256 / 256) % 256);
		}
		private uint IdFromData(byte[] data)
		{
			return (uint)data[0] + (uint)data[1] * 256 + (uint)data[2] * 256 * 256 + (uint)data[3] * 256 * 256 * 256;
		}
		public UserInfo InfoFromName(string name)
		{
			string info = "";
			WebRequest request = WebRequest.Create("http://193.178.169.223/fromName.php?name=" + name);
			WebResponse response = request.GetResponse();
			using (Stream stream = response.GetResponseStream())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					info = reader.ReadToEnd();
				}
			}
			response.Close();
			return new UserInfo(info.Split(';')[0], Convert.ToUInt32(info.Split(';')[1]), info.Split(';')[2]);
		}
		public UserInfo InfoFromId(uint id)
		{
			string info = "";
			WebRequest request = WebRequest.Create("http://193.178.169.223/fromId.php?id=" + Convert.ToString(id));
			WebResponse response = request.GetResponse();
			using (Stream stream = response.GetResponseStream())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					info = reader.ReadToEnd();
				}
			}
			response.Close();
			return new UserInfo(info.Split(';')[0], Convert.ToUInt32(info.Split(';')[1]), info.Split(';')[2]);
		}
		public bool Talk(Message mess)
		{
			try
			{
				UserInfo info = InfoFromName(mess.to);
				if (info.id == 0) return false;

				uint client = Convert.ToUInt32(info.id);

				RSAParameters rp = new RSAParameters();
				rp.Modulus = Convert.FromBase64String(info.key.Split('.')[0]);
				rp.Exponent = Convert.FromBase64String(info.key.Split('.')[1]);
				RSA.ImportParameters(rp);

				byte[] str = new byte[LenMess];
				byte[] cry;
				byte[] enc = Encoding.Unicode.GetBytes(mess.text);
				for (int i = 0; i < enc.Length; i++)
				{
					str[i] = enc[i];
				}
				cry = RSA.Encrypt(str, false);
				byte[] data = new byte[cry.Length + 4];
				for (int i = 4; i < data.Length; i++)
				{
					data[i] = cry[i - 4];
				}
				AddIdToData(ref data, client);
				socket.Send(data);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public Message WaitMessage()
		{
			byte[] data = new byte[LenData];
			socket.Receive(data, data.Length, 0);
			uint id = IdFromData(data);

			string fromname = "Не найдено";
			UserInfo info = InfoFromId(id);
			if (info.id != 0)
			{
				fromname = info.name;
			}
			byte[] data4 = new byte[LenData - 4];
			byte[] encry = new byte[LenMess];
			for (int i = 4; i < LenData; i++)
				data4[i - 4] = data[i];
			encry = RSAself.Decrypt(data4, false);
			string message = Encoding.Unicode.GetString(encry, 0, encry.Length);
			message = message.Replace((char)0, ' ');
			return new Message(fromname, name, message);
		}

	}
	public struct UserInfo
	{
		public string name;
		public uint id;
		public string key;
		public UserInfo(string name, uint id, string key)
		{
			this.name = name;
			this.id = id;
			this.key = key;
		}
	}
	public struct Message
	{
		public string from;
		public string to;
		public string text;
		public Message(string from, string to, string text)
		{
			this.from = from;
			this.to = to;
			this.text = text;
		}
	}
}
