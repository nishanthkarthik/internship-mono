using System;

namespace AutoIntern
{
	public class Company
	{
		public string Name;
		public string Profile;
		public DateTime TalkDate;
		public DateTime ResumeDeadline;
        public DateTime TestDate;
		public DateTime GdDate;
        public RegisterStatus Status;
		public Uri DetailUri;
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
