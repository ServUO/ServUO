using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Accounting;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class HouseIncreaseScroll : Item, IRewardItem
	{
        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }
		[Constructable]
		public HouseIncreaseScroll() : base( 0x14F0 )
		{
			base.Weight = 1.0;
			base.Name = "a house slot deed";
		}

		public HouseIncreaseScroll( Serial serial ) : base( serial )
		{
		}
  
                public void Use( Mobile from )
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }

            Account acct = from.Account as Account;
            if (acct.GetTag("maxHouses") == null)
            {
                acct.AddTag("maxHouses", "01");
            }
            int houses = Convert.ToInt32(acct.GetTag("maxHouses"));


			if ( Deleted )
				return;

			if ( IsChildOf( from.Backpack ) )
			{
				if ( houses > 2 )
				{
					from.SendMessage( "Your House slots are too high for this power scroll." ); // Your control slots are too high for this power scroll.
				}
				else
				{
						from.SendLocalizedMessage( 1049512 ); // You feel a surge of magic as the scroll enhances your powers!

						acct.SetTag( "maxHouses", "02" );

						from.PlaySound( 0x1EA );
						from.FixedEffect( 0x373A, 10, 15 );

						Delete();
					
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}
  
                public override void OnDoubleClick( Mobile from )
		{
			Use( from );
		}
  
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
            writer.Write((bool)m_IsRewardItem);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
		}

	}
}


