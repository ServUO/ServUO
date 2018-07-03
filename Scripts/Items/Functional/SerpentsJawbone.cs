using System;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using Server.ContextMenus;
using Server.Multis;

namespace Server.Items
{
    public class SerpentsJawbone : Item, ISecurable
	{
		public static Dictionary<int, Point3D> Locations { get; set; }
        private SecureLevel m_SecureLevel;
     
		public static void Initialize()
		{
			Locations = new Dictionary<int, Point3D>();
			
			Locations[1157135] = new Point3D(1156, 1143, -24); // The Village of Lakeshire		
            Locations[1157619] = new Point3D(644, 854, -56); // The Rat Fort		
			Locations[1157620] = new Point3D(1363, 1075, -13);  // Reg Volom			
			Locations[1016410] = new Point3D(1572, 1046, -8); // Twin Oaks Tavern			
			Locations[1157621] = new Point3D(984, 622, -80); // The Oasis			
			Locations[1078308] = new Point3D(1746, 1221, -1); // Blood Dungeon		
			Locations[1111764] = new Point3D(912, 1362, -21); // Cyclops Dungeon			
			Locations[1111765] = new Point3D(824, 774, -80); // Exodus Dungeon		
			Locations[1111766] = new Point3D(349, 1434, 16); // The Kirin Passage			
			Locations[1157622] = new Point3D(971, 303, 54); // Pass of Karnaugh			
			Locations[1157623] = new Point3D(1033, 1154, -24); // The Rat Cave		
			Locations[1078315] = new Point3D(541, 466, -72); // Terort Skitas			
			Locations[1111825] = new Point3D(1450, 1477, -29); // Twisted Weald			
			Locations[1113002] = new Point3D(642, 1307, -55); // Wisp Dungeon			
			Locations[1157624] = new Point3D(753, 497, -62); // Gwenno's Memorial			
			Locations[1157625] = new Point3D(1504, 628, -14); // Desert Gypsy Camp			
			Locations[1113000] = new Point3D(1785, 573, 71); // Rock Dungeon
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_SecureLevel;
            }
            set
            {
                this.m_SecureLevel = value;
            }
        }
 
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }
     
        public override int LabelNumber { get { return 1157654; } } // Serpent's Jawbone
		
		[Constructable]
		public SerpentsJawbone() : base(0x9F74)
		{
		}
        
        public override bool ForceShowProperties{ get{ return true; } }
		
		public override void OnDoubleClick(Mobile from)
		{
			if((IsLockedDown || IsSecure) && from.InRange(GetWorldLocation(), 3))
			{
				from.SendGump(new InternalGump(from as PlayerMobile, this));
			}
		}
		
		private class InternalGump : Gump
		{
			public Item Jawbone { get; set; }
            public PlayerMobile User { get; set; }
			
			public InternalGump(PlayerMobile pm, Item jawbone) : base(75, 75)
			{
				Jawbone = jawbone;
                User = pm;

                AddGumpLayout();
			}
			
			public void AddGumpLayout()
			{
				AddBackground( 0, 0, 400, 500, 9270 );
				
				AddHtmlLocalized( 0, 15, 400, 16, 1154645, "#1156704", 0xFFFF, false, false ); // Select your destination:

                ColUtility.For<int, Point3D>(SerpentsJawbone.Locations, (i, key, value) =>
				{
					AddHtmlLocalized(60, 45 + (i * 25), 250, 16, key, 0xFFFF, false, false);
					AddButton(20, 50 + (i * 25), 2117, 2118, key, GumpButtonType.Reply, 0);
				});
			}

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID > 0)
                {
                    int id = info.ButtonID;

                    if (SerpentsJawbone.Locations.ContainsKey(id))
                    {
                        Point3D p = SerpentsJawbone.Locations[id];

                        if (CheckTravel(p))
                        {
                            BaseCreature.TeleportPets(User, p, Map.Ilshenar);
                            User.Combatant = null;
                            User.Warmode = false;
                            User.Hidden = true;

                            User.MoveToWorld(p, Map.Ilshenar);

                            Effects.PlaySound(p, Map.Ilshenar, 0x1FE);
                        }
                    }
                }
            }
			
			private bool CheckTravel(Point3D p)
			{
				if ( !User.InRange( Jawbone.GetWorldLocation(), 1 ) || User.Map != Jawbone.Map )
				{
					User.SendLocalizedMessage( 1019002 ); // You are too far away to use the gate.
				}
				else if ( User.Murderer )
				{
					User.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
				}
				else if ( Server.Factions.Sigil.ExistsOn( User ) )
				{
					User.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
				}
				else if ( User.Criminal )
				{
					User.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				}
				else if ( Server.Spells.SpellHelper.CheckCombat( User ) )
				{
					User.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
				}
				else if ( User.Spell != null )
				{
					User.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
				}
				else if ( User.Map == Map.Ilshenar && User.InRange( p, 1 ) )
				{
					User.SendLocalizedMessage( 1019003 ); // You are already there.
				}
                else
                    return true;

                return false;
			}
		}
		
		public SerpentsJawbone(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
            writer.Write((int)this.m_SecureLevel); 
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
            this.m_SecureLevel = (SecureLevel)reader.ReadInt(); 
		}
	}
}
