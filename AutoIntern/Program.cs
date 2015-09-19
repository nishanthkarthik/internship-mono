using System;
using System.Linq;
using System.Collections.Generic;

namespace AutoIntern
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//args matching
			string regexPattern;
			if (args.Length > 0)
				regexPattern = args.First ();
			else
				regexPattern = @"Dual Degree.+(All|Mech)";
			
			LibIntern intern = new LibIntern ();
			List<Company> companies = intern.TotalCompanies;

			//Applied companies list
			ConArt.Out ("Applied Companies");
			Company[] appliedCompanies = companies.Where (a => a.Status == RegisterStatus.Applied).ToArray ();
			if (appliedCompanies.Length > 0)
				for (int i = 0; i < appliedCompanies.Length; ++i)
					Console.WriteLine (appliedCompanies [i].Name);
			else
				ConArt.Out ("You have applied for no company", MessageType.Warning);

			Console.WriteLine ();
			//Open for Mech
			ConArt.Out ("Open Companies for Mech Duals");
			Company[] openCompanies = intern.GetOpenCompanies (regexPattern).Where(x => x.Status != RegisterStatus.Applied && x.Status != RegisterStatus.Closed).ToArray ();
			if (openCompanies.Length > 0)
				for (int i = 0; i < openCompanies.Length; ++i)
					Console.WriteLine (openCompanies [i].Name + " - Rs." + intern.ParseSalary(openCompanies[i].DetailSnippet));
			else
				ConArt.Out ("No open company found", MessageType.Warning);

			Console.WriteLine ();
			//Open for applications
			ConArt.Out ("Companies accepting registrations");
			Company[] regOpenCompanies = companies.Where (a => a.Status == RegisterStatus.Open).ToArray ();
			if (regOpenCompanies.Length > 0)
				for (int i = 0; i < regOpenCompanies.Length; ++i)
					Console.WriteLine (regOpenCompanies [i].Name);
			else
				ConArt.Out ("No company accepts registration now", MessageType.Warning);
		}
	}
}
