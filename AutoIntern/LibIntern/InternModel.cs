using System;

namespace AutoIntern
{
	public class Company
	{
		public string name;
		public string profile;
		public DateTime talkDate;
		public DateTime resumeDeadline;
        public DateTime testDate;
		public DateTime gdDate;
        public RegisterStatus status;
    }

	public static class Parsers
	{
		public static RegisterStatus ParseStatus(string s)
		{
			if (s.Contains("Not open"))
                return RegisterStatus.NotAvailable;
			else if (s.Contains("Register"))
                return RegisterStatus.Open;
			else if (s.Contains("Closed"))
                return RegisterStatus.Closed;
			else if (s.Contains("Uploaded"))
                return RegisterStatus.Applied;
            return RegisterStatus.NotAvailable;
        }
	}

	public enum RegisterStatus
	{
		Closed,
		Open,
		Applied,
		NotAvailable
	}
}
