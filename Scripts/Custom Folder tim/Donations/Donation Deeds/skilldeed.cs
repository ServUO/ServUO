/**********************************************************
RunUO AoV C# script file
Official Age of Valor Script :: www.uovalor.com
Last modified by Red Squirrel on Jan-01-2012 10:09:10pm
Filepath: scripts\Items\Misc\skilldeed.cs
Lines of code: 307

Description: 

***********************************************************/


//	RunUO script: Skill Ball
//	Copyright (c) 2003 by Wulf C. Krueger <wk@mailstation.de>
//
//	This script is free software; you can redistribute it and/or modify
//	it under the terms of the GNU General Public License as published by
//	the Free Software Foundation; version 2 of the License applies.
//
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU General Public License for more details.
//
//	Please do NOT remove or change this header.

//	Official Defiance(c) skilldeed - by [Dev]Kamron aka XxSP1DERxX - Kamron@defianceuo.com


//* some modifications by red squirrel

using System;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class skilldeed : Item
	{
		public override bool DisplayLootType{ get{ return false; } }
		public override bool DisplayWeight{ get{ return false; } }
		
	
		private double m_SkillBonus = 5 + Utility.Random(6)*5;

		public string m_BaseName = "a skill gain deed +";
		public bool GumpOpen = false;
		Mobile m_owner;
		bool linked=false;

		[CommandProperty( AccessLevel.GameMaster )] 
		public double SkillBonus
		{
			get { return m_SkillBonus; }
			set 
			{ 
				m_SkillBonus = value;
				this.Name = m_BaseName + Convert.ToString(m_SkillBonus); 
			}
		}
		
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
		
			if(linked)
			{
			if(owner!=null)	list.Add(String.Format("This deed for {0} only",owner.Name) ); // physical damage ~1_val~%
			else list.Add("Invalid");
			}
			
		}
		
		[CommandProperty( AccessLevel.GameMaster )] 
		public Mobile owner
		{
			get { return m_owner; }
			set 
			{ 
				m_owner = value;linked=true;
			}
		}		
		
		public void DecrimentBonus(double bywhat)
		{		
			m_SkillBonus-=bywhat;
			
			m_SkillBonus=Math.Round(m_SkillBonus,1);

			if(m_SkillBonus<=0)
			{
			this.Delete();
			return;
			}
		this.Name = m_BaseName + Convert.ToString(m_SkillBonus);		
		}

		

		[Constructable]
		public skilldeed( int SkillBonus ) : base( 5360 )
		{
			m_SkillBonus = SkillBonus;
			LootType = LootType.Blessed;
			
			Hue = 259;
			
			
			Name = m_BaseName + Convert.ToString(SkillBonus); 			
		}
		
		[Constructable]
		public skilldeed(Mobile theowner, int SkillBonus ) : base( 5360 )
		{
			m_SkillBonus = SkillBonus;
			m_owner=theowner;
			linked=true;
			LootType = LootType.Blessed;
			
			Hue = 259;	
			
			Name = m_BaseName + Convert.ToString(SkillBonus); 			
		}		
		
		
		[Constructable]
		public skilldeed() : base( 5360 )
		{
			LootType = LootType.Blessed;
			Hue = 259;
					
			Name = m_BaseName + Convert.ToString(SkillBonus); 
		}

		public skilldeed( Serial serial ) : base( serial )
		{
		}

		public bool ValidateUse(Mobile from)
		{
			if(from is PlayerMobile && ((PlayerMobile)from).Young)
			{
			from.SendMessage("Young players cannot use this deed! Wait till you loose your young status then try using it.");
			return false;
			}
			if(owner!=null && from!=owner)
			{
			from.SendMessage("Only {0} ({1}) can use this deed. ",owner.Name,owner.Serial);
			return false;
			}		
			if(owner==null && linked)
			{
			from.SendMessage("This deed is for a character that has been deleted, and is thus, obsolete");
			return false;			
			}			
			if ( (this.SkillBonus == 0) && (from.AccessLevel < AccessLevel.GameMaster) ) 
			{
				from.SendMessage("This skill deed isn't charged. Please page for a GM.");
				return false;
			}
			else if ( (from.AccessLevel >= AccessLevel.GameMaster) && (this.SkillBonus == 0) ) 
			{
				from.SendGump( new PropertiesGump( from, this ) );
				return false;
			}

			if ( !IsChildOf( from.Backpack ) ) 
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.		
			return false;
			}
		return true;
		}
		
		
		public override void OnDoubleClick( Mobile from )
		{
			if(!ValidateUse(from))return;
			
			if (!GumpOpen) 
			{
				GumpOpen = true;
				from.SendGump( new skilldeedGump( from, this ) );
			}
			else if (GumpOpen)
				from.SendMessage("You're already using the deed.");
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
			writer.Write( m_SkillBonus );
			writer.Write(m_owner);
			writer.Write(linked);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_SkillBonus = reader.ReadDouble();
			
			if(version>0)m_owner=reader.ReadMobile();			
			if(version>1)linked=reader.ReadBool();			
		}
	}

	
	

	//end main skilldeed code - start "skilldeed in a bag"
	

	public class skilldeedbag : Bag
	{
		

		[Constructable]
		public skilldeedbag() : base()
		{
		Name="Bag of 5 random skill deeds";
		Hue = 4;
		
		for(int i=0;i<5;i++)DropItem(new skilldeed());		
		}
		
		
	
		public skilldeedbag( Serial serial ) : base( serial )
		{
		}
	
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			if(version<=0)
			{
				Hue = 259;
			}
		}
		
		

	
	
	}//end skilldeedinabag
	
}








namespace Server.Gumps
{
	public class skilldeedGump : Gump
	{
		private const int FieldsPerPage = 14;

		private Skill m_Skill;
		private skilldeed m_skb;

		public skilldeedGump ( Mobile from, skilldeed skb ) : base ( 20, 30 )
		{
			m_skb = skb;
			
			AddPage ( 0 );
			AddBackground( 0, 0, 260, 351, 5054 );
			
			AddImageTiled( 10, 10, 240, 23, 0x52 );
			AddImageTiled( 11, 11, 238, 21, 0xBBC );
			
			AddLabel( 65, 11, 0, "Skills you can raise" );
			
			AddPage( 1 );
			
			int page = 1;
			int index = 0;
			
			Skills skills = from.Skills;
			
			int number;
			if ( Core.AOS )
				number = 0;
			else
				number = 3;
			
			for ( int i = 0; i < ( skills.Length - number ); ++i ) 
			{
				if ( index >= FieldsPerPage ) 
				{
					AddButton( 231, 13, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1 );
					 
					++page;
					index = 0;
					 
					AddPage( page );
					 
					AddButton( 213, 13, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1 );
				}
			  
				Skill skill = skills[i];
			  
				if ( ((skill.Base) < skill.Cap) && (skill.Lock == SkillLock.Up)) 
				{	 
					AddImageTiled( 10, 32 + (index * 22), 240, 23, 0x52 );
					AddImageTiled( 11, 33 + (index * 22), 238, 21, 0xBBC );
					 
					AddLabelCropped( 13, 33 + (index * 22), 150, 21, 0, skill.Name );
					AddImageTiled( 180, 34 + (index * 22), 50, 19, 0x52 );
					AddImageTiled( 181, 35 + (index * 22), 48, 17, 0xBBC );
					AddLabelCropped( 182, 35 + (index * 22), 234, 21, 0, skill.Base.ToString( "F1" ) );
					 
					if ( from.AccessLevel >= AccessLevel.Player )
						AddButton( 231, 35 + (index * 22), 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0 );
					else
						AddImage( 231, 35 + (index * 22), 0x2622 );
					 
					++index;
				}
			}
		}
	  
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			if ( (from == null) || (m_skb.Deleted) )
				return;

			m_skb.GumpOpen = false;
			
			if(!m_skb.ValidateUse(from))return;

			
			if(m_skb.owner!=null && from!=m_skb.owner)
			{
			from.SendMessage("Only {0} ({1}) can use this deed. ",m_skb.owner.Name,m_skb.owner.Serial);
			return;
			}
			
			if ( info.ButtonID > 0 ) 
			{
				m_Skill = from.Skills[(info.ButtonID-1)];
			  
				if ( m_Skill == null )
					return;
			
				double count = from.Skills.Total / 10;
				double cap = from.SkillsCap / 10;
				ArrayList decreased = new ArrayList();
				double decreaseamount = 0.0;
				double bonuscopy = m_skb.SkillBonus;
				double bonustouse = bonuscopy;
				if ( (count + bonuscopy) > cap ) 
				{
					for (int i = 0;i < from.Skills.Length;i++ )
					{
						if (from.Skills[i].Lock == SkillLock.Down)
						{
							decreased.Add(from.Skills[i]);
							decreaseamount += from.Skills[i].Base;
							//break;
						}
					}
					if (decreased.Count == 0)
					{
						from.SendMessage("You have exceeded the skill cap and do not have a skill set to be decreased.");
						return;
					}
				}

				if(m_Skill.Base + bonustouse >= m_Skill.Cap)
				{
				bonustouse = m_Skill.Cap-m_Skill.Base;
				}
				
				double bonustouse2 = bonustouse;
				
				if (m_Skill.Base + bonustouse <= m_Skill.Cap) 
				{
					if ((cap - count) + decreaseamount >= bonustouse)
					{
						if (cap - count >= bonustouse)
						{
							m_Skill.Base += bonustouse;
							bonustouse = 0;	
						}
						else
						{
							m_Skill.Base += bonustouse;
							bonustouse -= cap - count;

							foreach( Skill s in decreased)
							{
								if (s.Base >= bonustouse)
								{
									s.Base -= bonustouse;
									bonustouse = 0;
								}
								else
								{
									bonustouse -= s.Base;
									s.Base = 0;
								}
							
								if (bonustouse == 0)
									break;
							}
						}
						
						
						m_skb.DecrimentBonus(bonustouse2);

					}
					else	from.SendMessage("You must have enough skill set down to compensate for the skill gain.");

				}
				else from.SendMessage("You have to choose another skill.");
			}
		}
	}
}



























