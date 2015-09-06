using System;
using System.Linq;
using System.Collections.Generic;

namespace AutoIntern
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World");
			LibIntern intern = new LibIntern ();
			List<Company> x = intern.GetCompanies ();
			ConArt.Out ("Applied Companies");
			foreach (var item in x.Where(a => a.Status == RegisterStatus.Applied))
			{
				Console.WriteLine (item.Name);
			}
			string sampleTextToBeRegexed = intern.GetCompanyDetails (x [0]);
		}
	}
}
