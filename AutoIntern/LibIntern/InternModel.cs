using System;

namespace AutoIntern
{
	public class Company
	{
		string name;
		string profile;
		DateTime talkDate;
		DateTime resumeDeadline;
		DateTime gdDate;
	}

	public enum RegisterStatus
	{
		Closed,
		Open,
		Applied,
		NotAvailable
	}
}

