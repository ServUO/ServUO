// Created by Peoharen
using System;
using Server;
using Server.Accounting;
using Server.Mobiles;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class BankStorageIncreaseDeed : Item, IRewardItem
	{
		// Maximum bonus these deeds can give is...
		public const int BonusCap = 50;

		// Standard private member with public access a GM can manipulate.
		private int m_BonusSlots;

		[CommandProperty( AccessLevel.GameMaster )]
		public int BonusSlots
		{
			get { return m_BonusSlots; }
			set { m_BonusSlots = value; }
		}
        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }
		[Constructable]
		public BankStorageIncreaseDeed() : base( 0x14F0 )
		{
			Name = "Bank Storage Increase Deed";
			m_BonusSlots = 25;
		}

		// Adds a special messages at the bottom of the mouse over properties.
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( 1114057, ((m_BonusSlots).ToString() + " Bonus Bank Slots") ); // ~1_VAL~
		}

		// This is how the tag is encoded into the account.
		// It uses the mobile's serial, thus only the mobile that uses this can gain the benefit fo it.
		public static string GetTag( Mobile m )
		{
			if ( m == null )
				return String.Empty; // String.Empty is a faster way of checking for ""
			else
				return "Bonus Bank Slots[" + m.Serial.ToString() + "]";
		}

		// OVerride DoubleClick
		public override void OnDoubleClick( Mobile from )
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
			// Sanity, check for nulls
			if ( from.Backpack == null || Parent != from.Backpack )
				from.SendLocalizedMessage( 1080058 ); // This must be in your backpack to use it.
			else if ( from.Account != null )
			{
				// Convert IAccount to Account
				Account acc = (Account)from.Account;

				// Get the tag and set up for reading the value.
				string tag = GetTag( from );
				int currentbonus = 0;

				// Convert can create an exception if the value cannot be converted to an integer.
				// so we throw it into Try/Catch to prevent the crash.
				try { currentbonus = Convert.ToInt32( acc.GetTag( tag ) ); }
				catch {}

				// Check bonus slots vs cap.
				if ( currentbonus + m_BonusSlots > BonusCap )
					from.SendMessage( "You cannot use another one of these." );
				else
				{
					from.SendMessage( "You increase your maximum item limit for your bank." );
					currentbonus += m_BonusSlots;

					// More sanity, then set max items to default (125) + bonus.
					if ( from.BankBox != null )
						from.BankBox.MaxItems = from.BankBox.DefaultMaxItems + currentbonus;

					// Remove the tag and readd it with the new value.
					acc.RemoveTag( tag );
					acc.AddTag( tag, currentbonus.ToString() );
                    Delete();
				}
                
			}
		}

		// Initialize is call on class creation, IE a way to set up stuff.
		public static void Initialize()
		{
			// This is an event handler, the Login event is triggers when ever someone logs in.
			// By using Events/Delegates we can tap into them from any script without edits to the base script.
			EventSink.Login += delegate( LoginEventArgs e )
			{
				// Sanity
				if ( e.Mobile.Account != null )
				{
					// Same as before really.
					Account acc = (Account)e.Mobile.Account;
					int currentbonus = 0;

					try { currentbonus = Convert.ToInt32( acc.GetTag( GetTag( e.Mobile ) ) ); }
					catch {}

					if ( e.Mobile.BankBox != null && currentbonus > 0 )
						e.Mobile.BankBox.MaxItems = e.Mobile.BankBox.DefaultMaxItems + currentbonus;
				}
			};
		}

		public BankStorageIncreaseDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
            writer.Write((bool)m_IsRewardItem);
			// Save the bunus slots if the deed isn't used and instead traded or sold.
			writer.Write( (int) m_BonusSlots );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
			// Load the bonus slots.
			m_BonusSlots = reader.ReadInt();
		}
	}
}