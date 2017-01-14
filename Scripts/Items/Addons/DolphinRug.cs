using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class DolphinRugEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,]
        {
			  {14553, -3, 2, 0}, {14554, -3, 1, 0}, {14555, -3, 0, 0}// 1	2	3	
			, {14556, -3, -1, 0}, {14557, -2, 1, 0}, {14558, -2, 0, 0}// 4	5	6	
			, {14559, -2, -1, 0}, {14560, -2, 2, 0}, {14561, -1, 1, 0}// 7	8	9	
			, {14562, -1, 0, 0}, {14564, -1, 2, 0}, {14565, 0, 1, 0}// 10	11	12	
			, {14568, 0, 2, 0}, {14563, -1, -1, 0}, {14567, 0, -1, 0}// 13	14	15	
			, {14566, 0, 0, 0}, {14571, 1, -1, 0}, {14578, 3, 0, 0}// 16	17	18	
			, {14572, 1, 2, 0}, {14569, 1, 1, 0}, {14576, 2, 2, 0}// 19	20	21	
			, {14580, 3, 2, 0}, {14573, 2, 1, 0}, {14577, 3, 1, 0}// 22	23	24	
			, {14579, 3, -1, 0}, {14575, 2, -1, 0}, {14570, 1, 0, 0}// 25	26	27	
			, {14574, 2, 0, 0}// 28	
		};

        public override BaseAddonDeed Deed { get { return new DolphinRugEastAddonDeed(m_NextUse); } }

        private DateTime m_NextUse;

        [Constructable]
        public DolphinRugEastAddon() : this(DateTime.Now)
        {
        }

		[ Constructable ]
		public DolphinRugEastAddon(DateTime nextuse)
		{
            m_NextUse = nextuse;

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent( new AddonComponent( m_AddOnSimpleComponents[i,0] ), m_AddOnSimpleComponents[i,1], m_AddOnSimpleComponents[i,2], m_AddOnSimpleComponents[i,3] );
		}

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (m_NextUse < DateTime.Now)
            {
                Container cont = from.Backpack;

                MessageInABottle mib = new MessageInABottle();

                if (cont == null || !cont.TryDropItem(from, mib, false))
                {
                    from.BankBox.DropItem(mib);
                    from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                }
                else
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                m_NextUse = DateTime.Now + TimeSpan.FromDays(7);
            }
        }

        public DolphinRugEastAddon(Serial serial)
            : base(serial)
		{
		}


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
            writer.Write(m_NextUse);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            m_NextUse = reader.ReadDateTime();
		}
	}

	public class DolphinRugEastAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new DolphinRugEastAddon(m_NextUse); } }
        public override int LabelNumber { get { return 1150051; } } // A Dolphin Rug (East)

        private DateTime m_NextUse;

        [Constructable]
        public DolphinRugEastAddonDeed() : this(DateTime.Now)
        {
        }

		[Constructable]
		public DolphinRugEastAddonDeed(DateTime nextuse)
		{
            m_NextUse = nextuse;
		}

		public DolphinRugEastAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
            writer.Write(m_NextUse);
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            m_NextUse = reader.ReadDateTime();
		}
	}

    public class DolphinRugSouthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {14590, 1, -1, 0}, {14586, 1, -2, 0}, {14589, 0, -1, 0}// 1	2	3	
			, {14592, -1, -1, 0}, {14593, 0, 0, 0}, {14597, 0, 1, 0}// 4	5	6	
			, {14596, -1, 0, 0}, {14600, -1, 1, 0}, {14604, -1, 2, 0}// 7	8	9	
			, {14585, 0, -2, 0}, {14587, 2, -2, 0}, {14591, 2, -1, 0}// 10	11	12	
			, {14601, 0, 2, 0}, {14588, -1, -2, 0}, {14581, -1, -3, 0}// 13	14	15	
			, {14582, 0, -3, 0}, {14583, 1, -3, 0}, {14584, 2, -3, 0}// 16	17	18	
			, {14605, 0, 3, 0}, {14606, 1, 3, 0}, {14595, 2, 0, 0}// 19	20	21	
			, {14599, 2, 1, 0}, {14603, 2, 2, 0}, {14608, -1, 3, 0}// 22	23	24	
			, {14607, 2, 3, 0}, {14594, 1, 0, 0}, {14602, 1, 1, 0}// 25	26	27	
			, {14598, 1, 2, 0}// 28	
		};

        public override BaseAddonDeed Deed { get { return new DolphinRugSouthAddonDeed(m_NextUse); } }

        private DateTime m_NextUse;

        [Constructable]
        public DolphinRugSouthAddon()
            : this(DateTime.Now)
        {
        }

        [Constructable]
        public DolphinRugSouthAddon(DateTime nextuse)
        {
            m_NextUse = nextuse;

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (m_NextUse < DateTime.Now)
            {
                Container cont = from.Backpack;

                MessageInABottle mib = new MessageInABottle();

                if (cont == null || !cont.TryDropItem(from, mib, false))
                {
                    from.BankBox.DropItem(mib);
                    from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                }
                else
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                m_NextUse = DateTime.Now + TimeSpan.FromDays(7);
            }
        }

        public DolphinRugSouthAddon(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextUse = reader.ReadDateTime();
        }
    }

    public class DolphinRugSouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DolphinRugSouthAddon(m_NextUse); } }
        public override int LabelNumber { get { return 1150050; } } // A Dolphin Rug (South)

        private DateTime m_NextUse;

        [Constructable]
        public DolphinRugSouthAddonDeed()
            : this(DateTime.Now)
        {
        }

        [Constructable]
        public DolphinRugSouthAddonDeed(DateTime nextuse)
        {
            m_NextUse = nextuse;
        }

        public DolphinRugSouthAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
            writer.Write(m_NextUse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextUse = reader.ReadDateTime();
        }
    }
}