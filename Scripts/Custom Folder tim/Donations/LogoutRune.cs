/***********************************************************************************************
 * Logout Rune Version 1.0 Written by Admin Jubal of Ebonspire http://ebonspire.game-server.cc
 *
 * Description:
 * Ever been on a shard where you can't recall or from certain locations, but you have to leave
 * RIGHT NOW?  This rune is the answer to that problem.  It allows the player to mark the rune
 * one time only for a set location, must be an inn location.  Upon use the player is
 * teleported to the marked location, logged out, and their account is banned for one hour. to
 * a DateTime is used to store the last usage and the rune may only be used once every 7 days
 * to prevent exploitation.  The rune does not require any mage skills to use.
 * 
 * Specifications:
 * Weight is set to 0 so it does not affect a players weight when carrying it.
 * Each rune is marked for a specific character, no other character can use anothers rune.
 * The rune may be deleted trashed or traded, but still only the original owner may use the rune.
 * All properties may be modified by a GM and viewed by a Counselor.
 * A gump is presented upon use of the Rune before logging the character out to warn them of
 * what will happen upon use, canceling the gump will not do anything, pressing ok will log them
 * out.
 * 
 * Installation:
 * Drop this file into your custom scripts folder.  If you wish to include a rune in everyone's
 * backpack use the command [global addtopack LogoutRune where PlayerMobile  this will add the
 * item to every players backpack whether online or not.  To add the item to newly created
 * characters modify the CharacterCreation.cs script where other standard items are added and
 * add this line PackItem( new LogoutRune() ); then each new character created will receive one.
 * 
 * Redistribution and modification:
 * This script may be redistributed or modified and redistributed provided that this header
 * remains in tact and full credits are carried from one modification to another.
 * 
 * Support:
 * Support for this item will be given on any forums that the author submits this to personally
 * as well as by email to Jubal@ebonspire.game-server.cc
 * 
 * Credits:
 * EA Games for creating Ultima Online, the most common client used to connect to our server.
 * RunUO for creating such a highly modifiable and stable server.
 * Admin Runestaff Ebonspire Owner, for the original Idea.
 * Admin Jubal Ebonspire programmer, for the actual coding.
 ***********************************************************************************************/

using System;
using Server;
using Server.Accounting;
using Server.Mobiles;
using Server.Gumps;
using Server.Regions;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	[FlipableAttribute( 0x1f14, 0x1f15, 0x1f16, 0x1f17 )]
    public class LogoutRune : Item, IRewardItem
	{
		private DateTime m_LastUse = DateTime.MinValue;
		private Point3D m_MarkLocation = new Point3D(0, 0, 0);
		private Map m_MarkMap = Map.Internal;

        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public DateTime LastUse
		{
			get{ return m_LastUse; }
			set{ m_LastUse = value; }
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public Point3D MarkLocation
		{
			get{ return m_MarkLocation; }
			set{ m_MarkLocation = value; }
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public Map MarkMap
		{
			get{ return m_MarkMap; }
			set{ m_MarkMap = value; }
		}

		[Constructable]
		public LogoutRune() : base( 0x1F14 )
		{
			this.Hue = 1166;
			this.Name = "Emergency Logout Rune";
			this.Weight = 0;
			this.LootType = LootType.Blessed;
		}

		/*public override bool OnDragLift(Mobile from)
		{
			if( m_Owner == null )
			{
				m_Owner = from;
				BlessedFor = from;
			}
			return base.OnDragLift (from);
		}*/

		public override void OnDoubleClick(Mobile from)
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
			else
			{
				if( m_MarkLocation.X == 0 && m_MarkLocation.Y == 0 )
				{
					//if( from.Region.IsInInn( from.Location ) )
					//{
						//m_Owner = from;
						BlessedFor = from;
						m_MarkLocation = from.Location;
						m_MarkMap = from.Region.Map;
						from.SendMessage( "You have marked your rune, it may not be remarked again." );
					//}
					//else
					//{
					//	from.SendMessage(" You must be in an inn to mark this rune. ");
					//}
				}
				else if( m_LastUse.AddDays(7) > DateTime.Now )
				{
					from.SendMessage( "You may not use this again until {0} at {1}.", m_LastUse.AddDays(7).ToShortDateString(), m_LastUse.AddDays(7).ToShortTimeString() );
				}
				else
				{
					from.SendGump( new LogoutRuneGump( this ) );
				}
			}
			base.OnDoubleClick (from);
		}

		public LogoutRune( Serial s ) : base ( s )
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);
			writer.Write( (int) 0 ); // version
            writer.Write((bool)m_IsRewardItem);
			writer.Write( (DateTime)m_LastUse );
			writer.Write( (Point3D)m_MarkLocation );
			writer.Write( (Map)m_MarkMap );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();
			switch( version )
			{
				case 0:
                    m_IsRewardItem = reader.ReadBool();
					m_LastUse = reader.ReadDateTime();
					m_MarkLocation = reader.ReadPoint3D();
					m_MarkMap = reader.ReadMap();
					break;
			}
		}
	}

	public class LogoutRuneGump : Server.Gumps.Gump
	{
		private LogoutRune m_LogoutRune;

		public LogoutRuneGump(LogoutRune logoutrune) : base(40, 40)
		{
			m_LogoutRune = logoutrune;
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(0, 0, 413, 187, 9200);
			this.AddLabel(158, 12, 54, @"WARNING!");
			this.AddHtml( 11, 42, 392, 92, @"This rune is to be used only in an emergency requiring you to log out unexpectedly.  Use of this rune is limited to once every 7 days.  Upon use your account will be banned for one hour.", (bool)false, (bool)false);
			this.AddButton(13, 144, 247, 248, (int)Buttons.btn_OK, GumpButtonType.Reply, 0);
			this.AddButton(337, 144, 241, 242, (int)Buttons.btn_Cancel, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			Mobile from = sender.Mobile;
			switch(info.ButtonID)
			{
				case (int)Buttons.btn_OK:
					m_LogoutRune.LastUse = DateTime.Now;
					from.MoveToWorld( m_LogoutRune.MarkLocation, m_LogoutRune.MarkMap );
					Account a = sender.Account as Account;
					sender.Dispose();
					a.Banned = true;
					a.SetUnspecifiedBan( from );
					a.SetBanTags( from, DateTime.Now, TimeSpan.FromHours(1) );
					break;
				case (int)Buttons.btn_Cancel:
					break;
			}
		}


		public enum Buttons
		{
			btn_OK,
			btn_Cancel,
		}
	}
}
