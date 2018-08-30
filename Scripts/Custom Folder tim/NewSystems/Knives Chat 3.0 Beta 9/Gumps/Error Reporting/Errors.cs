using System;
using System.Collections;
using System.Web.Mail;
using System.Diagnostics;
using System.Threading;
using Server;
using Server.Network;

namespace Knives.Chat3
{
	public class Errors
	{
		private static ArrayList s_ErrorLog = new ArrayList();
		private static ArrayList s_Checked = new ArrayList();

		public static ArrayList ErrorLog{ get{ return s_ErrorLog; } }
		public static ArrayList Checked{ get{ return s_Checked; } }

		public static void Initialize()
		{
            RUOVersion.AddCommand("ChatErrors", AccessLevel.Counselor, new ChatCommandHandler(OnErrors));
            RUOVersion.AddCommand("ce", AccessLevel.Counselor, new ChatCommandHandler(OnErrors));

            EventSink.Login += new LoginEventHandler( OnLogin );
		}

		private static void OnErrors( CommandInfo e )
		{
			if ( e.ArgString == null || e.ArgString == "" )
				new ErrorsGump( e.Mobile );
			else
				Report( e.ArgString + " - " + e.Mobile.Name );
		}

		private static void OnLogin( LoginEventArgs e )
		{
			if ( e.Mobile.AccessLevel != AccessLevel.Player
			&& s_ErrorLog.Count != 0
			&& !s_Checked.Contains( e.Mobile ) )
				new ErrorsNotifyGump( e.Mobile );
		}

        public static void Report(string error)
        {
            s_ErrorLog.Add(String.Format("<B>{0}</B><BR>{1}<BR>", DateTime.Now, error));

            Events.InvokeError(new ErrorEventArgs(error));

            s_Checked.Clear();

            Notify();
        }

        public static void Report(string error, Exception e)
        {
            s_ErrorLog.Add(String.Format("<B>{0}</B><BR>{1}<BR>", DateTime.Now, error));

            Events.InvokeError(new ErrorEventArgs(error));

            s_Checked.Clear();

            Notify();

            s_Error = error;
            s_Exception = e;
            new Thread(new ThreadStart(SendEmail)).Start();
        }

        private static void Notify()
		{
			foreach( NetState state in NetState.Instances )
			{
				if ( state.Mobile == null )
					continue;

				if ( state.Mobile.AccessLevel != AccessLevel.Player )
					Notify( state.Mobile );
			}
		}

        private static string s_Error = "";
        private static Exception s_Exception;

        private static void SendEmail()
        {
            string txt = s_Error;
            Exception e = s_Exception;

            try
            {
                MailMessage mail = new MailMessage();
                mail.To = "kmwill23@hotmail.com";
                mail.From = Server.Misc.ServerList.ServerName;
                mail.Subject = "Automated Chat Error Report";
                mail.Body = Server.Misc.ServerList.ServerName + " \r \r " + txt + " \r \r " + e.Message + " \r \r " + e.Source + " \r \r " + e.StackTrace;
                string schema = "http://schemas.microsoft.com/cdo/configuration/";
                mail.Fields.Add(schema + "smtpserverport", "465");
                mail.Fields.Add(schema + "smtpusessl", "true");
                mail.Fields[schema + "smtpauthenticate"] = "1";
                mail.Fields[schema + "sendusername"] = "kniveschat@gmail.com";
                mail.Fields[schema + "sendpassword"] = "openmail";
                SmtpMail.SmtpServer = "smtp.gmail.com";
                SmtpMail.Send(mail);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Email failed to send.");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }

		public static void Notify( Mobile m )
		{
			if ( m.HasGump( typeof( ErrorsGump ) ) )
				new ErrorsGump( m );
			else
				new ErrorsNotifyGump( m );
		}
	}
}