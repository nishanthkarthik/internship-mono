using System;
using System.Linq;
using System.Collections.Generic;

namespace AutoIntern
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			LibIntern intern = new LibIntern ();
			List<Company> x = intern.GetCompanies ();
			ConArt.Out ("Applied Companies");
			foreach (var item in x.Where(a => a.Status == RegisterStatus.Applied))
			{
				Console.WriteLine (item.Name);
			}
			ConArt.Out ("Open Companies for Mech Duals");
			foreach (Company company in intern.GetOpenCompanies(@"Dual Degree.+(All|Mech)"))
				Console.WriteLine (company.Name);
		}
	}
}
