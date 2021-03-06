﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using HtmlAgilityPack;
using RestSharp;
using TidyNet;
using System.IO;
using System.Text;
using RestSharp.Extensions.MonoHttp;
using System.Text.RegularExpressions;

namespace AutoIntern
{
	public class LibIntern
	{
		RestResponseCookie cookie;
	 	public	List<Company> TotalCompanies;

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
			TotalCompanies = GetCompanies ();
		}

		public List<Company> GetCompanies ()
		{
			List<Company> companyList = new List<Company> ();
			RestClient client = new RestClient (@"http://internship.iitm.ac.in/students/comp_list.php");
			RestRequest request = new RestRequest (Method.GET);
			request.AddCookie (cookie.Name, cookie.Value);
			IRestResponse response = client.Execute (request);
			if (response.StatusCode != HttpStatusCode.OK)
				return new List<Company> ();
			string cleanHtml = CleanHtml (response.Content);

			//Parse table into Company objects
			HtmlDocument htmlDoc = new HtmlDocument ();
			htmlDoc.LoadHtml (cleanHtml);
			HtmlNode companyTable = htmlDoc.DocumentNode.SelectNodes ("//table") [3];

			ConArt.Out ("Starting data collection...");
			//Select rows and parse data within
			HtmlNodeCollection rowsList = companyTable.SelectNodes ("tr");
			for (int i = 1; i < rowsList.Count; ++i)
			{
				HtmlNodeCollection columnList = rowsList [i].SelectNodes ("td");
				Company company = new Company ();
				company.Name = columnList [1].InnerText.Replace ("\n", "");
				company.Profile = columnList [2].InnerText;
				string profileAddress = columnList [2].SelectSingleNode ("a").GetAttributeValue ("href", "");
				company.DetailUri = new Uri ("http://internship.iitm.ac.in/students/" + HttpUtility.HtmlDecode (profileAddress), UriKind.Absolute);
				DateTime.TryParse (columnList [3].InnerText, out company.TalkDate);
				DateTime.TryParse (columnList [4].InnerText, out company.ResumeDeadline);
				DateTime.TryParse (columnList [5].InnerText, out company.TestDate);
				DateTime.TryParse (columnList [6].InnerText, out company.GdDate);
				DateTime.TryParse (columnList [7].InnerText, out company.TalkDate);
				company.Status = Parsers.ParseStatus (columnList [7].InnerText);
				company.DetailSnippet = GetCompanyDetails (company);
				companyList.Add (company);

				//Update progress bar
				Console.CursorLeft = 0;
				Console.Write ( (int)i * 100 / rowsList.Count + "% Complete" );
			}
			Console.CursorLeft = 0;
			Console.WriteLine("                 ");

			return companyList;
		}

		public Company[] GetOpenCompanies (string companyRegexPattern)
		{
			return (TotalCompanies as List<Company>).Where (company => IsOpen (company.DetailSnippet, companyRegexPattern)).ToArray ();
		}

		bool IsOpen (string companyString, string branchQueryRegex)
		{
			Regex regex = new Regex (branchQueryRegex);
			return regex.IsMatch (companyString);
		}

		public long ParseSalary (string companyString)
		{
			const string regexString = @"Amount\s+:\s+\d+";
			Regex regex = new Regex (regexString);
			MatchCollection matches = regex.Matches (companyString);
			if (matches.Count > 0)
				return long.Parse (Regex.Match (matches [0].Value, @"\d+").Value);
			return 0;
		}

		string GetCompanyDetails (Company company)
		{
			RestClient client = new RestClient (company.DetailUri.ToString ());
			RestRequest request = new RestRequest (Method.GET);
			request.AddCookie (cookie.Name, cookie.Value);
			IRestResponse response = client.Execute (request);
			if (response.StatusCode != HttpStatusCode.OK)
				return string.Empty;
			string cleanHtml = CleanHtml (response.Content);
			HtmlDocument htmlDoc = new HtmlDocument ();
			htmlDoc.LoadHtml (cleanHtml);
			return HttpUtility.HtmlDecode (htmlDoc.DocumentNode.InnerText.Replace ("\n", string.Empty).Replace ("\r", string.Empty));
		}

		static string CleanHtml (string badHtmlString)
		{
			//Clean bad html using TIDY
			// http://sourceforge.net/projects/tidynet/
			Tidy tidy = new Tidy ();
			MemoryStream input = new MemoryStream ();
			MemoryStream output = new MemoryStream ();
			byte[] badHtml = Encoding.UTF8.GetBytes (badHtmlString);
			input.Write (badHtml, 0, badHtml.Length);
			input.Position = 0;
			TidyMessageCollection tidyMsg = new TidyMessageCollection ();
			tidy.Parse (input, output, tidyMsg);
			return Encoding.UTF8.GetString (output.ToArray ());
		}

		public bool Login (ref char[] rollno, ref char[] password)
		{
			RestClient client = new RestClient (@"http://internship.iitm.ac.in/students/login.php") {
				FollowRedirects = false
			};
			RestRequest request = new RestRequest (Method.POST);
			request.AddParameter ("rollno", new string (rollno), ParameterType.GetOrPost);
			request.AddParameter ("pass", new string (password), ParameterType.GetOrPost);
			request.AddParameter ("submit", "Login", ParameterType.GetOrPost);
			IRestResponse response = client.Execute (request);
			if (response.StatusCode != HttpStatusCode.Found)
				return false;
			cookie = response.Cookies.First ();
			WipeChar (ref rollno);
			WipeChar (ref password);
			return true;
		}

		char[] getSecureText ()
		{
			SecureString pwd = new SecureString ();
			while (true)
			{
				ConsoleKeyInfo i = Console.ReadKey (true);
				if (i.Key == ConsoleKey.Enter)
				{
					break;
				}
				if (i.Key == ConsoleKey.Backspace)
				{
					if (pwd.Length > 0)
					{
						pwd.RemoveAt (pwd.Length - 1);
						Console.Write ("\b \b");
					}
				} else
				{
					pwd.AppendChar (i.KeyChar);
					Console.Write ("*");
				}
			}
			Console.WriteLine ();
			return GetSecureArray (pwd);
		}

		char[] GetSecureArray (SecureString source)
		{
			int length = source.Length;
			IntPtr pointer = IntPtr.Zero;
			char[] chars = new char[length];

			if (length == 0)
				return new [] { 'N', 'N' };

			try
			{
				pointer = Marshal.SecureStringToBSTR (source);
				Marshal.Copy (pointer, chars, 0, length);
			} finally
			{
				if (pointer != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR (pointer);
				}
			}
			return chars;
		}

		void WipeChar (ref char[] secureText)
		{
			for (int i = 0; i < secureText.Length; ++i)
				secureText [i] = '\0';
		}
	}
}
