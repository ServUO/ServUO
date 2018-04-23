using System;
using Server;
using Server.Accounting;
using OrbServerSDK;
using Server.Engines.OrbRemoteServer;

namespace Server.Engines.UOArchitect
{
	public abstract class BaseOrbToolRequest : OrbRequest 
	{
		private Mobile m_OnlineMobile;

		public Mobile Mobile
		{
			get{ return m_OnlineMobile; }
		}

		public bool IsOnline
		{
			get
			{ 
				if(m_OnlineMobile == null)
					return false;
				else if(m_OnlineMobile.NetState == null)
					return false;
				else
					return true;
			}
		}

		public void FindOnlineMobile(OrbClientInfo client)
		{
			m_OnlineMobile = ((OrbClientState)client).OnlineMobile;
		}

	}
}
