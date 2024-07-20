using System;

namespace MessagingToolkit.QRCode.Helper
{
	
	public class ContentConverter
	{
		
		internal static char n = '\n';
		
		public static String Convert(String targetString)
		{
			if (targetString == null)
				return targetString;
			if (targetString.IndexOf("MEBKM:") > - 1)
				targetString = ConvertDocomoBookmark(targetString);
			if (targetString.IndexOf("MECARD:") > - 1)
				targetString = ConvertDocomoAddressBook(targetString);
			if (targetString.IndexOf("MATMSG:") > - 1)
				targetString = ConvertDocomoMailto(targetString);
			if (targetString.IndexOf("http\\://") > - 1)
				targetString = ReplaceString(targetString, "http\\://", "\nhttp://");
			return targetString;
		}
		
		private static String ConvertDocomoBookmark(String targetString)
		{
			targetString = RemoveString(targetString, "MEBKM:");
			targetString = RemoveString(targetString, "TITLE:");
			targetString = RemoveString(targetString, ";");
			targetString = RemoveString(targetString, "URL:");
			return targetString;
		}
		
		private static String ConvertDocomoAddressBook(String targetString)
		{
			
			targetString = RemoveString(targetString, "MECARD:");
			targetString = RemoveString(targetString, ";");
			targetString = ReplaceString(targetString, "N:", "NAME1:");
			targetString = ReplaceString(targetString, "SOUND:", n + "NAME2:");
			targetString = ReplaceString(targetString, "TEL:", n + "TEL1:");
			targetString = ReplaceString(targetString, "EMAIL:", n + "MAIL1:");
			targetString = targetString + n;
			return targetString;
		}
		
		private static String ConvertDocomoMailto(String s)
		{
			String s1 = s;
			char c = '\n';
			s1 = RemoveString(s1, "MATMSG:");
			s1 = RemoveString(s1, ";");
			s1 = ReplaceString(s1, "TO:", "MAILTO:");
			s1 = ReplaceString(s1, "SUB:", c + "SUBJECT:");
			s1 = ReplaceString(s1, "BODY:", c + "BODY:");
			s1 = s1 + c;
			return s1;
		}
		
		private static String ReplaceString(String s, String s1, String s2)
		{
			String s3 = s;
			for (int i = s3.IndexOf(s1, 0); i > - 1; i = s3.IndexOf(s1, i + s2.Length))
				s3 = s3.Substring(0, (i) - (0)) + s2 + s3.Substring(i + s1.Length);
			
			return s3;
		}
		
		private static String RemoveString(String s, String s1)
		{
			return ReplaceString(s, s1, "");
		}
	}
}