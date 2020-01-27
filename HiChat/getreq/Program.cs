using System;
using System.Net;
using System.IO;

namespace getreq
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				string name = Console.ReadLine();
				WebRequest request = WebRequest.Create("http://185.251.38.207/fromId.php?id=" + name);
				string info = "";
				WebResponse response = request.GetResponse();
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						info = reader.ReadToEnd();
					}
				}
				response.Close();
				Console.WriteLine(info.Split(';')[1]);
				Console.WriteLine(info.Split(';')[2]);
			}
		}
	}
}
