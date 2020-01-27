using System;
using System.Text;
using System.Security.Cryptography;

namespace TestRSA
{
	class Program
	{
		static void Main()
		{
			RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048);
			string prik = RSA.ToXmlString(true);
			string pubk = RSA.ToXmlString(false);
			RSACryptoServiceProvider RSApub = new RSACryptoServiceProvider();
			RSACryptoServiceProvider RSApri = new RSACryptoServiceProvider();
			RSApri.FromXmlString(prik);
			RSApub.FromXmlString(pubk);

			while (true)
			{
				string text = Console.ReadLine();
				byte[] data = Encoding.UTF8.GetBytes(text);
				Console.WriteLine("Длина до шифрования - {0}",data.Length);
				data = RSApub.Encrypt(data, false);
				Console.WriteLine("Длина после шифрования - {0}", data.Length);
				Console.WriteLine(Encoding.UTF8.GetString(data));
				data = RSApri.Decrypt(data, false);
				Console.WriteLine(Encoding.UTF8.GetString(data));
			}
		}
	}
}
