#region References
using System;

using Server.Mobiles;
#endregion

namespace Server.Accounting
{
	public partial class Account
	{
		public static void Initialize()
		{
			EventSink.Connected += OnConnected;
			EventSink.Disconnected += OnDisconnected;
			EventSink.Login += OnLogin;
		}

		public static void OnConnected(ConnectedEventArgs e)
		{
			if (e.Mobile.Account is Account acc && acc.Young && acc.m_YoungTimer == null)
			{
				acc.m_YoungTimer = new YoungTimer(acc);
				acc.m_YoungTimer.Start();
			}
		}

		public static void OnDisconnected(DisconnectedEventArgs e)
		{
			if (e.Mobile.Account is Account acc)
			{
				if (acc.m_YoungTimer != null)
				{
					acc.m_YoungTimer.Stop();
					acc.m_YoungTimer = null;
				}

				if (e.Mobile is PlayerMobile m)
				{
					acc.m_TotalGameTime += DateTime.UtcNow - m.SessionStart;
				}
			}
		}

		public static void OnLogin(LoginEventArgs e)
		{
			if (e.Mobile is PlayerMobile m && m.Account is Account acc && m.Young && acc.Young)
			{
				var ts = YoungDuration - acc.TotalGameTime;
				var hours = Math.Max((int)ts.TotalHours, 0);

				m.SendAsciiMessage("You will enjoy the benefits and relatively safe status of a young player for {0} more hour{1}.", hours, hours != 1 ? "s" : "");
			}
		}
	}
}
