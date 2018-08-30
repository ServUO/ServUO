using System;
using Server.Mobiles;
using Server.Gumps;
using Server;
using System.IO;
using Server.Mobiles;
using Server.Gumps;
using System.Collections;
 
namespace Server.Misc
{
    public class DisplaySaveGump
    {
        public static void Initialize()
        {
            EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);
        }
 
        public static void EventSink_WorldSave(WorldSaveEventArgs e)
		{
		
				 foreach (Server.Network.NetState ns in Server.Network.NetState.Instances)
                    {
                        if (ns.Mobile == null)
                                continue;

                        ns.Mobile.SendGump(new Server.Gumps.SaveGump());
						
						// change ( 2.0 ) to how mamy seconds you want the gump to remain open
						Timer.DelayCall( TimeSpan.FromSeconds( 3.5 ), new TimerStateCallback( CloseGump ), ns.Mobile );
                    }

                  World.Save();	
		}
		
			public static void CloseGump(object state)
			{
				foreach (Server.Network.NetState ns in Server.Network.NetState.Instances)
                {
					if (ns.Mobile == null) { continue; }
					
					if(ns.Mobile.HasGump(typeof(SaveGump))) { ns.Mobile.CloseGump(typeof(SaveGump)); }
				}
			}
    }
}





namespace Server.Gumps
{
    public class SaveGump : Gump
    {
        public SaveGump() : base( 50, 50 )
        {
            this.Closable=true;
            this.Disposable=true;
            this.Dragable=true;
            this.Resizable=false;
            this.AddPage(0);
            this.AddBackground(5, 5, 415, 100, 9270);
            this.AddLabel(165, 30, 2062, String.Format( "{0}", Server.Misc.ServerList.ServerName ) );
            this.AddLabel(105, 55, 1165, @"The world is saving...   Please wait.");
            this.AddImage(25, 25, 5608);
            this.AddItem(360, 50, 6168);
        }
    }
}