using System;
using Server;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1EBA, 0x1EBB )]
	public class TaxidermyKit : Item
	{
		public override int LabelNumber{ get{ return 1041279; } } // a taxidermy kit

		[Constructable]
		public TaxidermyKit() : base( 0x1EBA )
		{
			Weight = 1.0;
		}

		public TaxidermyKit( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.Skills[SkillName.Carpentry].Base < 90.0 )
			{
				from.SendLocalizedMessage( 1042594 ); // You do not understand how to use this.
			}
			else
			{
				from.SendLocalizedMessage( 1042595 ); // Target the corpse to make a trophy out of.
				from.Target = new CorpseTarget( this );
			}
		}

        public static TrophyInfo[] TrophyInfos { get { return m_Table; } }
        private static TrophyInfo[] m_Table = new TrophyInfo[]
        {
            new TrophyInfo( typeof( BrownBear ),	  0x1E60,		1041093, 1041107 ),
			new TrophyInfo( typeof( GreatHart ),	  0x1E61,		1041095, 1041109 ),
			new TrophyInfo( typeof( BigFish ),		  0x1E62,		1041096, 1041110 ),
			new TrophyInfo( typeof( Gorilla ),		  0x1E63,		1041091, 1041105 ),
			new TrophyInfo( typeof( Orc ),			  0x1E64,		1041090, 1041104 ),
			new TrophyInfo( typeof( PolarBear ),	  0x1E65,		1041094, 1041108 ),
			new TrophyInfo( typeof( Troll ),		  0x1E66,		1041092, 1041106 ),
            new TrophyInfo( typeof( RedHerring ),     0x1E62,		1113567, 1113569 ),
            new TrophyInfo( typeof( MudPuppy ),		  0x1E62,		1113568, 1113570 ),

            new TrophyInfo( typeof( AutumnDragonfish),     0,       1116124, 1116185 ),
            new TrophyInfo( typeof( BullFish ),            1,       1116129, 1116190 ),
            new TrophyInfo( typeof( FireFish ),            2,       1116127, 1116188 ),
            new TrophyInfo( typeof( GiantKoi ),            3,       1116122, 1116183 ),
            new TrophyInfo( typeof( LavaFish ),            4,       1116130, 1116191 ),
            new TrophyInfo( typeof( SummerDragonfish ),    5,       1116124, 1116186 ),  
            new TrophyInfo( typeof( UnicornFish ),         6,       1116120, 1116181 ),
            new TrophyInfo( typeof( AbyssalDragonfish ),   7,       1116140, 1116201 ),
            new TrophyInfo( typeof( BlackMarlin ),         8,       1116133, 1116194 ),
            new TrophyInfo( typeof( BlueMarlin ),          9,       1116131, 1116192 ),
            new TrophyInfo( typeof( GiantSamuraiFish ),    10,      1116138, 1116199 ),
            new TrophyInfo( typeof( Kingfish ),            11,      1116119, 1116180 ),
            new TrophyInfo( typeof( LanternFish ),         12,      1116142, 1116203 ),
            new TrophyInfo( typeof( SeekerFish ),          13,      1116145, 1116206 ),
            new TrophyInfo( typeof( SpringDragonfish ),    14,      1116139, 1116200 ),
            new TrophyInfo( typeof( StoneFish),            15,      1116135, 1116196 ),
            new TrophyInfo( typeof( WinterDragonfish),     16,      1116141, 1116202 ),
            new TrophyInfo( typeof( BlueLobster),          17,      1149812, 1149804 ),
            new TrophyInfo( typeof( BloodLobster),         18,      1149816, 1149808 ),
            new TrophyInfo( typeof( DreadLobster),         19,      1149817, 1149809 ),
            new TrophyInfo( typeof( VoidLobster),          20,      1149815, 1149807 ),
            new TrophyInfo( typeof( StoneCrab),            21,      1149811, 1149803 ),
            new TrophyInfo( typeof( SpiderCrab),           22,      1149813, 1149805 ),
            new TrophyInfo( typeof( TunnelCrab),           23,      1149818, 1149810 ),
            new TrophyInfo( typeof( VoidCrab ),            24,      1149814, 1149806 ),

            new TrophyInfo( typeof( CrystalFish ),         25,      1116126, 1116187 ),
            new TrophyInfo( typeof( FairySalmon ),         26,      1116123, 1116184 ),
            new TrophyInfo( typeof( GreatBarracuda ),      27,      1116134, 1116195 ),
            new TrophyInfo( typeof( HolyMackerel ),        28,      1116121, 1116182 ),
            new TrophyInfo( typeof( ReaperFish ),          29,      1116128, 1116189 ),
            new TrophyInfo( typeof( YellowtailBarracuda ), 30,      1116132, 1116193 ),
            new TrophyInfo( typeof( DungeonPike ),         31,      1116143, 1116204 ),
            new TrophyInfo( typeof( GoldenTuna ),          32,      1116137, 1116198 ),
            new TrophyInfo( typeof( RainbowFish ),         33,      1116144, 1116205 ),
            new TrophyInfo( typeof( ZombieFish ),          34,      1116136, 1116197 ),
        };

        public class TrophyInfo
        {
            public TrophyInfo( Type type, int id, int deedNum, int addonNum )
            {
                m_CreatureType = type;
                m_NorthID = id;
                m_DeedNumber = deedNum;
                m_AddonNumber = addonNum;
            }

            private Type m_CreatureType;
            private int m_NorthID;
            private int m_DeedNumber;
            private int m_AddonNumber;

            public Type CreatureType { get { return m_CreatureType; } }
            public int NorthID { get { return m_NorthID; } }
            public int DeedNumber { get { return m_DeedNumber; } }
            public int AddonNumber { get { return m_AddonNumber; } }
        }


		private class CorpseTarget : Target
		{
			private TaxidermyKit m_Kit;

			public CorpseTarget( TaxidermyKit kit ) : base( 3, false, TargetFlags.None )
			{
				m_Kit = kit;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Kit.Deleted )
					return;

				if ( !(targeted is Corpse) && !(targeted is BigFish) && !(targeted is BaseHighseasFish) && !(targeted is HuntingPermit))
				{
					from.SendLocalizedMessage( 1042600 ); // That is not a corpse!
				}
				else if ( targeted is Corpse && ((Corpse)targeted).VisitedByTaxidermist )
				{
					from.SendLocalizedMessage( 1042596 ); // That corpse seems to have been visited by a taxidermist already.
				}
				else if ( !m_Kit.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
				else if ( from.Skills[SkillName.Carpentry].Base < 90.0 )
				{
					from.SendLocalizedMessage( 1042603 ); // You would not understand how to use the kit.
                }
                #region Huntmasters Challenge
                else if (targeted is HuntingPermit)
                {
                    HuntingPermit lic = targeted as HuntingPermit;

                    if (from.Backpack == null || !lic.IsChildOf(from.Backpack))
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    else if (!lic.CanUseTaxidermyOn)
                    {
                        //TODO: Message?
                    }
                    else if (from.Backpack != null && from.Backpack.ConsumeTotal(typeof(Board), 10))
                    {
                        Server.Engines.HuntsmasterChallenge.HuntingTrophyInfo info = Server.Engines.HuntsmasterChallenge.HuntingTrophyInfo.Infos[lic.KillEntry.KillIndex];

                        if (info != null)
                        {
                            string name = lic.KillEntry.Owner != null ? lic.KillEntry.Owner.Name : from.Name;

                            if(info.Complex)
                                from.AddToBackpack(new HuntTrophyAddonDeed(name, info.MeasuredBy, lic.KillEntry.Measurement, info.SouthID, lic.KillEntry.DateKilled.ToShortDateString(), lic.KillEntry.Location, info.Species));
                            else
                                from.AddToBackpack(new HuntTrophyDeed(name, info.MeasuredBy, lic.KillEntry.Measurement, info.SouthID, lic.KillEntry.DateKilled.ToShortDateString(), lic.KillEntry.Location, info.Species, info.FlippedIDs));
                            
                            lic.ProducedTrophy = true;
                            m_Kit.Delete();
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1042598); // You do not have enough boards.
                        return;
                    }
                }
                #endregion
                else
				{
					object obj = targeted;

					if ( obj is Corpse )
						obj = ((Corpse)obj).Owner;

                    if ( obj != null )
                    {
                        for ( int i = 0; i < m_Table.Length; i++ )
                        {
                            if ( m_Table[i].CreatureType == obj.GetType() )
                            {
                                Container pack = from.Backpack;

                                if ( pack != null && pack.ConsumeTotal( typeof( Board ), 10 ) )
                                {
                                    from.SendLocalizedMessage( 1042278 ); // You review the corpse and find it worthy of a trophy.
                                    from.SendLocalizedMessage( 1042602 ); // You use your kit up making the trophy.

                                    Mobile hunter = null;
                                    int weight = 0;

                                    if ( targeted is BigFish )
                                    {
                                        BigFish fish = targeted as BigFish;

                                        hunter = fish.Fisher;
                                        weight = (int)fish.Weight;

                                        fish.Consume();
                                    }
                                    #region High Seas
                                    else if (targeted is RareFish)
                                    {
                                        RareFish fish = targeted as RareFish;

                                        hunter = fish.Fisher;
                                        weight = (int)fish.Weight;
                                        DateTime dateCaught = fish.DateCaught;

                                        from.AddToBackpack(new FishTrophyDeed(weight, hunter, dateCaught, m_Table[i].DeedNumber, m_Table[i].AddonNumber, m_Table[i].NorthID));

                                        fish.Delete();
                                        m_Kit.Delete();
                                        return;
                                    }

                                    else if (targeted is RareCrabAndLobster)
                                    {
                                        RareCrabAndLobster fish = targeted as RareCrabAndLobster;

                                        hunter = fish.Fisher;
                                        weight = (int)fish.Weight;
                                        DateTime dateCaught = fish.DateCaught;

                                        from.AddToBackpack(new FishTrophyDeed(weight, hunter, dateCaught, m_Table[i].DeedNumber, m_Table[i].AddonNumber, m_Table[i].NorthID));

                                        fish.Delete();
                                        m_Kit.Delete();
                                        return;
                                    }
                                    #endregion

                                    from.AddToBackpack( new TrophyDeed( m_Table[i], hunter, weight ) );

                                    if ( targeted is Corpse )
                                        ((Corpse)targeted).VisitedByTaxidermist = true;

                                    m_Kit.Delete();
                                    return;
                                }
                                else
                                {
                                    from.SendLocalizedMessage( 1042598 ); // You do not have enough boards.
                                    return;
                                }
                            }
                        }
                    }

					from.SendLocalizedMessage( 1042599 ); // That does not look like something you want hanging on a wall.
				}
			}
		}
	}

	public class TrophyAddon : Item, IAddon
	{
		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }

		private int m_WestID;
		private int m_NorthID;
		private int m_DeedNumber;
		private int m_AddonNumber;

		private Mobile m_Hunter;
		private int m_AnimalWeight;

		[CommandProperty( AccessLevel.GameMaster )]
		public int WestID{ get{ return m_WestID; } set{ m_WestID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NorthID{ get{ return m_NorthID; } set{ m_NorthID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DeedNumber{ get{ return m_DeedNumber; } set{ m_DeedNumber = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AddonNumber{ get{ return m_AddonNumber; } set{ m_AddonNumber = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Hunter{ get{ return m_Hunter; } set{ m_Hunter = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AnimalWeight{ get{ return m_AnimalWeight; } set{ m_AnimalWeight = value; InvalidateProperties(); } }

		public override int LabelNumber{ get{ return m_AddonNumber; } }

		[Constructable]
		public TrophyAddon( Mobile from, int itemID, int westID, int northID, int deedNumber, int addonNumber ) : this( from, itemID, westID, northID, deedNumber, addonNumber, null, 0 )
		{
		}

		public TrophyAddon( Mobile from, int itemID, int westID, int northID, int deedNumber, int addonNumber, Mobile hunter, int animalWeight ) : base( itemID )
		{
			m_WestID = westID;
			m_NorthID = northID;
			m_DeedNumber = deedNumber;
			m_AddonNumber = addonNumber;

			m_Hunter = hunter;
			m_AnimalWeight = animalWeight;

			Movable = false;

			MoveToWorld( from.Location, from.Map );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_AnimalWeight >= 20 )
			{
				if ( m_Hunter != null )
					list.Add( 1070857, m_Hunter.Name ); // Caught by ~1_fisherman~

				list.Add( 1070858, m_AnimalWeight.ToString() ); // ~1_weight~ stones
			}
		}

		public override void OnAosSingleClick( Mobile from )
		{
			ObjectPropertyList opl = this.PropertyList;

			if(this.AddonNumber==1041110)
            {
				from.Send( new UnicodeMessage( this.Serial, this.ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", "A large fish trophy" ) ); 

				if(this.Hunter != null)
					from.Send( new UnicodeMessage( this.Serial, this.ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", "Caught by "+this.Hunter.Name ) ); 

				from.Send( new UnicodeMessage( this.Serial, this.ItemID, MessageType.Label, 0x3B2, 3, "ENU", "", this.AnimalWeight+" stones" ) );
			} 
            else
            {
				if ( opl.Header > 0 )
					from.Send( new MessageLocalized( this.Serial, this.ItemID, MessageType.Label, 0x3B2, 3, opl.Header, this.Name, opl.HeaderArgs ) );
			}
		}

		public TrophyAddon( Serial serial ) : base( serial )
		{
		}

		public bool CouldFit( IPoint3D p, Map map )
		{
			if ( !map.CanFit( p.X, p.Y, p.Z, this.ItemData.Height ) )
				return false;

			if ( this.ItemID == m_NorthID )
				return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
			else
				return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (Mobile) m_Hunter );
			writer.Write( (int) m_AnimalWeight );

			writer.Write( (int) m_WestID );
			writer.Write( (int) m_NorthID );
			writer.Write( (int) m_DeedNumber );
			writer.Write( (int) m_AddonNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Hunter = reader.ReadMobile();
					m_AnimalWeight = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_WestID = reader.ReadInt();
					m_NorthID = reader.ReadInt();
					m_DeedNumber = reader.ReadInt();
					m_AddonNumber = reader.ReadInt();
					break;
				}
			}

			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( FixMovingCrate ) );
		}

		private void FixMovingCrate()
		{
			if ( this.Deleted )
				return;

			if ( this.Movable || this.IsLockedDown )
			{
				Item deed = this.Deed;

				if ( this.Parent is Item )
				{
					((Item)this.Parent).AddItem( deed );
					deed.Location = this.Location;
				}
				else
				{
					deed.MoveToWorld( this.Location, this.Map );
				}

				Delete();
			}
		}

		public Item Deed
		{
			get{ return new TrophyDeed( m_WestID, m_NorthID, m_DeedNumber, m_AddonNumber, m_Hunter, m_AnimalWeight ); }
		}

		public override void OnDoubleClick( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsCoOwner( from ) )
			{
				if ( from.InRange( GetWorldLocation(), 1 ) )
				{
					from.AddToBackpack( this.Deed );
					Delete();
				}
				else
				{
					from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
				}
			}
		}
	}

	[Flipable( 0x14F0, 0x14EF )]
	public class TrophyDeed : Item
	{
		private int m_WestID;
		private int m_NorthID;
		private int m_DeedNumber;
		private int m_AddonNumber;

		private Mobile m_Hunter;
		private int m_AnimalWeight;

		[CommandProperty( AccessLevel.GameMaster )]
		public int WestID{ get{ return m_WestID; } set{ m_WestID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int NorthID{ get{ return m_NorthID; } set{ m_NorthID = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DeedNumber{ get{ return m_DeedNumber; } set{ m_DeedNumber = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AddonNumber{ get{ return m_AddonNumber; } set{ m_AddonNumber = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Hunter{ get{ return m_Hunter; } set{ m_Hunter = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AnimalWeight{ get{ return m_AnimalWeight; } set{ m_AnimalWeight = value; InvalidateProperties(); } }

		public override int LabelNumber{ get{ return m_DeedNumber; } }

		[Constructable]
		public TrophyDeed( int westID, int northID, int deedNumber, int addonNumber ) : this( westID, northID, deedNumber, addonNumber, null, 0 )
		{
		}

		public TrophyDeed( int westID, int northID, int deedNumber, int addonNumber, Mobile hunter, int animalWeight ) : base( 0x14F0 )
		{
			m_WestID = westID;
			m_NorthID = northID;
			m_DeedNumber = deedNumber;
			m_AddonNumber = addonNumber;
			m_Hunter = hunter;
			m_AnimalWeight = animalWeight;
		}

        public TrophyDeed( TaxidermyKit.TrophyInfo info, Mobile hunter, int animalWeight )
            : this( info.NorthID + 7, info.NorthID, info.DeedNumber, info.AddonNumber, hunter, animalWeight )
        {
        }

		public TrophyDeed( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_AnimalWeight >= 20 )
			{
				if ( m_Hunter != null )
					list.Add( 1070857, m_Hunter.Name ); // Caught by ~1_fisherman~

				list.Add( 1070858, m_AnimalWeight.ToString() ); // ~1_weight~ stones
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (Mobile) m_Hunter );
			writer.Write( (int) m_AnimalWeight );

			writer.Write( (int) m_WestID );
			writer.Write( (int) m_NorthID );
			writer.Write( (int) m_DeedNumber );
			writer.Write( (int) m_AddonNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Hunter = reader.ReadMobile();
					m_AnimalWeight = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_WestID = reader.ReadInt();
					m_NorthID = reader.ReadInt();
					m_DeedNumber = reader.ReadInt();
					m_AddonNumber = reader.ReadInt();
					break;
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house != null && house.IsCoOwner( from ) )
				{
					bool northWall = BaseAddon.IsWall( from.X, from.Y - 1, from.Z, from.Map );
					bool westWall = BaseAddon.IsWall( from.X - 1, from.Y, from.Z, from.Map );

					if ( northWall && westWall )
					{
						switch ( from.Direction & Direction.Mask )
						{
							case Direction.North:
							case Direction.South: northWall = true; westWall = false; break;

							case Direction.East:
							case Direction.West:  northWall = false; westWall = true; break;

							default: from.SendMessage( "Turn to face the wall on which to hang this trophy." ); return;
						}
					}

					int itemID = 0;

					if ( northWall )
						itemID = m_NorthID;
					else if ( westWall )
						itemID = m_WestID;
					else
						from.SendLocalizedMessage( 1042626 ); // The trophy must be placed next to a wall.

					if ( itemID > 0 )
					{
                        Item trophy = new TrophyAddon(from, itemID, m_WestID, m_NorthID, m_DeedNumber, m_AddonNumber, m_Hunter, m_AnimalWeight);

                        if (m_DeedNumber == 1113567)
                            trophy.Hue = 1645;
                        else if (m_DeedNumber == 1113568)
                            trophy.Hue = 1032;

                        house.Addons[trophy] = from;
						Delete();
					}
				}
				else
				{
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}
	}
}