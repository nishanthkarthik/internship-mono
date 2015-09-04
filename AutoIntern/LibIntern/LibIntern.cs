using RestSharp;
using System.Linq;
using System.Net;
using System;
using System.Security;

namespace AutoIntern
{
	public class LibIntern
	{
		private	RestResponseCookie cookie;

		public LibIntern ()
		{
			char[] rollno, password;
			ConArt.Ask ("Roll no. ");
			rollno = getSecureText ();
			ConArt.Ask ("Password ");
			password = getSecureText ();
			if (!Login (ref rollno, ref password))
			{
				ConArt.Out ("Cannot Login", MessageType.Error);
				Environment.Exit (-1);
			}
			ConArt.Out ("Logged in");
		}

		public bool Login (ref char[] rollno, ref char[] password)
		{
			RestClient client = new RestClient (@"http://internship.iitm.ac.in/students/login.php");
			RestRequest request = new RestRequest (Method.GET);
			request.AddParameter ("rollno", rollno, ParameterType.GetOrPost);
			request.AddParameter ("pass", password, ParameterType.GetOrPost);
			request.AddParameter ("submit", "Login", ParameterType.GetOrPost);
			IRestResponse response = client.Execute (request);
			if (response.StatusCode != HttpStatusCode.OK)
				return false;
			cookie = response.Cookies.First ();
			WipeChar (ref rollno);
			WipeChar (ref password);
			return true;
		}

		private char[] getSecureText ()
		{
			SecureString pwd = new SecureString ();
			while (true)
			{
				ConsoleKeyInfo i = Console.ReadKey (true);
				if (i.Key == ConsoleKey.Enter)
				{
					break;
				} 
				else if (i.Key == ConsoleKey.Backspace)
				{
					if (pwd.Length > 0)
					{
						pwd.RemoveAt (pwd.Length - 1);
						Console.Write ("\b \b");
					}
				} 
				else
				{
					pwd.AppendChar (i.KeyChar);
					Console.Write ("*");
				}
			}
			return pwd.ToString ().ToCharArray ();
		}

		private void WipeChar (ref char[] secureText)
		{
			for (int i = 0; i < secureText.Length; ++i)
				secureText [i] = '\0';
		}
	}
}

