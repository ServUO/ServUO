using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Server.Misc
{
	public class Email
	{
		/* In order to support emailing, fill in EmailServer and FromAddress:
        * Example:
        *  public static readonly string EmailServer = "mail.domain.com";
        *  public static readonly string FromAddress = "servuo@domain.com";
        * 
        * If you want to add crash reporting emailing, fill in CrashAddresses:
        * Example:
        *  public static readonly string CrashAddresses = "first@email.here,second@email.here,third@email.here";
        * 
        * If you want to add speech log page emailing, fill in SpeechLogPageAddresses:
        * Example:
        *  public static readonly string SpeechLogPageAddresses = "first@email.here,second@email.here,third@email.here";
        */
		public static readonly string EmailServer = Config.Get("Email.EmailServer", default(string));
		public static readonly int EmailPort = Config.Get("Email.EmailPort", 25);
		public static readonly bool EmailSsl = Config.Get("Email.EmailSsl", false);

		public static readonly string FromAddress = Config.Get("Email.FromAddress", default(string));
		public static readonly string CrashAddresses = Config.Get("Email.CrashAddresses", default(string));
		public static readonly string SpeechLogPageAddresses = Config.Get("Email.SpeechLogPageAddresses", default(string));

		public static readonly string EmailUsername = Config.Get("Email.EmailUsername", default(string));
		public static readonly string EmailPassword = Config.Get("Email.EmailPassword", default(string));

		private static readonly Regex _pattern = new Regex(@"^[a-z0-9.+_-]+@([a-z0-9-]+\.)+[a-z]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static SmtpClient _Client;

		public static bool IsValid(string address)
		{
			if (address != null && address.Length <= 320)
			{
				return _pattern.IsMatch(address);
			}

			return false;
		}

		public static void Configure()
		{
			if (EmailServer != null)
			{
				_Client = new SmtpClient(EmailServer, EmailPort);

				if (EmailUsername != null)
				{
					_Client.Credentials = new System.Net.NetworkCredential(EmailUsername, EmailPassword);
				}

				if (EmailSsl)
				{
					_Client.EnableSsl = true;
				}

				_Client.SendCompleted += SendCompleted;
			}
		}

		public static bool Send(MailMessage message)
		{
			try
			{
				_Client.SendAsync(message, message);
			}
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
				return false;
			}

			return true;
		}

		private static void SendCompleted(object _, AsyncCompletedEventArgs e)
		{
			var message = (MailMessage)e.UserState;

			if (!e.Cancelled)
			{
				Console.WriteLine("Sent e-mail '{0}' to '{1}'.", message.Subject, message.To);
			}
			else if (e.Error != null)
			{
				Console.WriteLine("Failure sending e-mail '{0}' to '{1}'\n{2}", message.Subject, message.To, e.Error);
				Diagnostics.ExceptionLogging.LogException(e.Error);
			}
			else
			{
				Console.WriteLine("Failure sending e-mail '{0}' to '{1}'", message.Subject, message.To);
			}
		}
	}
}
