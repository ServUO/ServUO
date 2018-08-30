using System;
using Server;
using Server.Mobiles;
using Server.Engines.VeteranRewards;

namespace Server.Items
{

	[Flipable( 0x14F0, 0x14EF )]
    public class FemaleMannequinDeed : Item, IRewardItem
	{
        private bool m_IsRewardItem;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

		[Constructable]
		public FemaleMannequinDeed() : base( 0x14F0 )
		{
			Name = "A Female Mannequin Deed";
			LootType = LootType.Blessed;
		}

		public FemaleMannequinDeed( Serial serial ) : base( serial )
		{
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

		public override void OnDoubleClick( Mobile from )
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
			if ( IsChildOf( from.Backpack ) )
			{
				Mannequin m = new Mannequin( from, true );
				m.Map = from.Map;
				m.Location = from.Location;
				m.Direction = from.Direction;
				this.Delete();
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}
	}
}
