using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;


namespace GeneratorKeys
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Введите свой никнейм:");
			string name = Console.ReadLine();

			RSACryptoServiceProvider RsaKey = new RSACryptoServiceProvider(2048);
			string prik = RsaKey.ToXmlString(true);
			{
				WebRequest request = WebRequest.Create("http://193.178.169.223/Reg.php");
				request.Method = "POST";

				string mod = Convert.ToBase64String(RsaKey.ExportParameters(false).Modulus);
				mod = WebUtility.UrlEncode(mod);

				string exp = Convert.ToBase64String(RsaKey.ExportParameters(false).Exponent);
				exp = WebUtility.UrlEncode(exp);

				string data = "name=" + name + "&pubkey=" + mod + '.' + exp;
				byte[] byteArray = Encoding.UTF8.GetBytes(data);
				request.ContentType = "application/x-www-form-urlencoded";
				request.ContentLength = byteArray.Length;

				using (Stream dataStream = request.GetRequestStream())
				{
					dataStream.Write(byteArray, 0, byteArray.Length);
				}
				WebResponse response = request.GetResponse();

				string res = "";
				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						res += reader.ReadToEnd();
					}
				}
				if(res == "1")
				{
					FileStream fs = new FileStream("info", FileMode.Create);
					StreamWriter sw = new StreamWriter(fs);
					sw.Write(name + '\n' + prik);
					sw.Close();
					Console.WriteLine("Регистрация успешна!");
				}
				else
				{
					Console.WriteLine("Произошла ошибка!");
				}
				while (true) ;
			}
		}
	}
}
