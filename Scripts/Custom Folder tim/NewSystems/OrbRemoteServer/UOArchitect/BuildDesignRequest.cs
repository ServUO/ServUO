using System;
using Server;
using OrbServerSDK;
using Server.Engines.OrbRemoteServer;
using UOArchitectInterface;

namespace Server.Engines.UOArchitect
{
	public class BuildDesignRequest : BaseOrbToolRequest 
	{
		private DesignItemCol m_Items;

		public DesignItemCol Items
		{
			get{ return m_Items; }
		}

		public static void Initialize()
		{
			OrbServer.Register("UOAR_BuildDesign", typeof(BuildDesignRequest), AccessLevel.GameMaster, true);
		}

		public override void OnRequest(OrbClientInfo clientInfo, OrbRequestArgs args)
		{
			FindOnlineMobile(clientInfo);

			if(args == null)
				SendResponse(null);
			else if(!(args is BuildRequestArgs))
				SendResponse(null);
			else if(!this.IsOnline)
				SendResponse(null);

			m_Items = ((BuildRequestArgs)args).Items;

			Mobile.SendMessage("Target the ground where you want to place the building");
			this.Mobile.Target = new BuildDesignTarget(this);
		}

	}
}
