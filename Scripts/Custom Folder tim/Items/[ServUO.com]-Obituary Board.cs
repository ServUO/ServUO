// Originally done by unknown author. Edited and updated by fcondon aka Exale

using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using System.Text;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using System.IO;
using Server.Mobiles;
using System.Threading;
using Server.Gumps;
using Server.Accounting;

namespace Server
{
    public class Announce
    {
        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(OnDeath);
        }
		
        public static void OnDeath(PlayerDeathEventArgs args)
        {
            Mobile m = args.Mobile;

            if (args.Mobile.AccessLevel < AccessLevel.GameMaster)
            {
				Mobile mob = m.LastKiller;

				if ( !Directory.Exists( "Data/Deaths" ) )
					Directory.CreateDirectory( "Data/Deaths" );

				string deaths = null;
				string deathToll = null;
				string path = "Data/Deaths/deaths.txt";

				if ( File.Exists( path ))
				{
					StreamReader r = new StreamReader( path, System.Text.Encoding.Default, false );
					deathToll = r.ReadLine();
					r.Close();
					deathToll = deathToll.ToString();

					string[] result = deathToll.Split(new string[] { "<BR>------------------------------<BR>" }, StringSplitOptions.RemoveEmptyEntries);
					int iDeath = 0;
					foreach (string str in result)
					{
						if ( iDeath < 99 ){deaths = deaths + "<BR>------------------------------<BR>" + str;}
						iDeath = iDeath + 1;
					}
				}

				using ( StreamWriter op = new StreamWriter("Data/Deaths/deaths.txt", false) )
				{
					if ( ( mob == m ) && ( mob != null ) )
					{
						World.Broadcast(0x21, true, "{0} has killed themself!", args.Mobile.Name);
						op.WriteLine("{0} killed themself!" + deaths, args.Mobile.Name);
					}
					else if ( ( mob != null ) && ( mob is PlayerMobile ) )
					{
						World.Broadcast(0x21, true, "{0} has been slain by {1}!", args.Mobile.Name, mob.Name);
						op.WriteLine("{0} was slain by {1}!" + deaths, args.Mobile.Name, mob.Name);
					}
					else if ( mob != null )
					{
						World.Broadcast(0x21, true, "{0} has been killed by {1}!", args.Mobile.Name, mob.Name);
						op.WriteLine("{0} was killed by {1}!" + deaths, args.Mobile.Name, mob.Name);
					}
					else
					{
						World.Broadcast(0x21, true, "{0} has been killed!", args.Mobile.Name);
						op.WriteLine("{0} was killed!" + deaths, args.Mobile.Name);
					}
				}
            }
        }
    }
}

namespace Server.Items
{
	[Flipable(0x1E5E, 0x1E5F)]
	public class DeathBoard : Item
	{
		[Constructable]
		public DeathBoard( ) : base( 0x1E5E )
		{
			Weight = 1.0;
			Name = "Obituaries";
			//Hue = 0xB85;
			Hue = 0;
			Movable = false;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "A Listing Of Recent Deaths" );
		}

		public class KillsGump : Gump
		{
			
			public static void Initialize()
			{
				CommandSystem.Register( "DeathList", AccessLevel.Player, new CommandEventHandler( DeathBoard_OnCommand ) );
			}

			private static void DeathBoard_OnCommand( CommandEventArgs e ) 
			{ 
				e.Mobile.SendGump( new KillsGump( e.Mobile ) ); 
			}
			
			public KillsGump( Mobile from ): base( 100, 100 )
			{
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;
				this.AddPage(0);
				this.AddImage(0, 0, 9380);
				this.AddImage(114, 0, 9381);
				this.AddImage(372, 0, 9382);
				this.AddImage(171, 0, 9381);
				this.AddImage(0, 140, 9386);
				this.AddImage(114, 140, 9387);
				this.AddImage(171, 140, 9387);
				this.AddImage(372, 140, 9388);
				this.AddImage(34, 36, 30501);
				this.AddImage(228, 0, 9381);
				this.AddImage(284, 0, 9381);
				this.AddImage(319, 0, 9381);
				this.AddImage(227, 140, 9387);
				this.AddImage(278, 140, 9387);
				this.AddImage(322, 140, 9387);
				this.AddButton(35, 221, 2151, 2152, 1, GumpButtonType.Reply, 0);
				this.AddLabel(72, 225, 0, @"REFRESH");

				string deaths = null;
				string path = "Data/Deaths/deaths.txt";

				if ( File.Exists( path ))
				{
					StreamReader r = new StreamReader( path, System.Text.Encoding.Default, false );
					deaths = r.ReadLine();
					r.Close();
					deaths = deaths.ToString();
					this.AddHtml( 162, 40, 294, 201, @"<basefont color=black>" + deaths + "</basefont>", (bool)false, (bool)true);
				}
				else
				{
					this.AddHtml( 162, 40, 294, 201, @"<basefont color=black>There are no recent deaths.</basefont>", (bool)false, (bool)true);
				}
			}

			public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
			{
				Mobile from = sender.Mobile;

				switch ( info.ButtonID )
				{
					case 0: { break; }
					case 1:
					{
						from.CloseGump( typeof( KillsGump ) );
						from.SendGump( new KillsGump( from ) );
						break;
					} 
				}
			}
		}

		public override void OnDoubleClick( Mobile e )
		{
			e.SendGump( new KillsGump( e ) );
		}

		public DeathBoard(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}