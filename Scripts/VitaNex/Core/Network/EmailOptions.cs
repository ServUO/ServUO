#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Net;
using System.Net.Mail;

using Server;
#endregion

namespace VitaNex.Network
{
	public class EmailOptions : PropertyObject
	{
		[CommandProperty(AccessLevel.Administrator)]
		public bool Auth { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string From { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string To { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string Host { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int Port { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string User { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string Pass { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Valid => Port > 0 && !String.IsNullOrWhiteSpace(Host) && !String.IsNullOrWhiteSpace(From) && !String.IsNullOrWhiteSpace(To);

		public EmailOptions()
		{
			Auth = false;
			From = String.Empty;
			To = String.Empty;
			Host = String.Empty;
			Port = 25;
			User = String.Empty;
			Pass = String.Empty;
		}

		public EmailOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Auth = false;
			From = String.Empty;
			To = String.Empty;
			Host = String.Empty;
			Port = 0;
			User = String.Empty;
			Pass = String.Empty;
		}

		public override void Reset()
		{
			Auth = false;
			From = String.Empty;
			To = String.Empty;
			Host = String.Empty;
			Port = 25;
			User = String.Empty;
			Pass = String.Empty;
		}

		public override string ToString()
		{
			return "Email Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Auth);
					writer.Write(From);
					writer.Write(To);
					writer.Write(Host);
					writer.Write(Port);
					writer.Write(User);
					writer.Write(Pass);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Auth = reader.ReadBool();
					From = reader.ReadString();
					To = reader.ReadString();
					Host = reader.ReadString();
					Port = reader.ReadInt();
					User = reader.ReadString();
					Pass = reader.ReadString();
				}
				break;
			}
		}

		public MailMessage CreateMessage()
		{
			return new MailMessage(From ?? String.Empty, To ?? String.Empty);
		}

		public SmtpClient CreateClient()
		{
			var c = new SmtpClient(Host ?? String.Empty, Port > 0 ? Port : 0);

			if (Auth)
			{
				c.Credentials = new NetworkCredential(User ?? String.Empty, Pass ?? String.Empty);
				c.UseDefaultCredentials = false;
			}

			return c;
		}

		public static implicit operator MailMessage(EmailOptions o)
		{
			return o != null ? o.CreateMessage() : new MailMessage();
		}

		public static implicit operator SmtpClient(EmailOptions o)
		{
			return o != null ? o.CreateClient() : new SmtpClient();
		}
	}
}