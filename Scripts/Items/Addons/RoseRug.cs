using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class RoseRugEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {14512, 0, -1, 0}, {14511, 0, 2, 0}, {14497, -3, 2, 0}// 1	2	3	
			, {14507, -1, 2, 0}, {14498, -3, 1, 0}, {14501, -2, 1, 0}// 4	5	6	
			, {14509, 0, 1, 0}, {14500, -3, -1, 0}, {14516, 1, -1, 0}// 7	8	9	
			, {14517, 2, 1, 0}, {14518, 2, 0, 0}, {14515, 1, 2, 0}// 10	11	12	
			, {14503, -2, 2, 0}, {14506, -1, 0, 0}, {14510, 0, 0, 0}// 13	14	15	
			, {14508, -1, -1, 0}, {14504, -2, -1, 0}, {14502, -2, 0, 0}// 16	17	18	
			, {14505, -1, 1, 0}, {14519, 2, 2, 0}, {14514, 1, 0, 0}// 19	20	21	
			, {14520, 2, -1, 0}, {14513, 1, 1, 0}, {14499, -3, 0, 0}// 22	23	24	
			, {14522, 3, 0, 0}, {14521, 3, 1, 0}, {14523, 3, 2, 0}// 25	26	27	
			, {14524, 3, -1, 0}// 28	
		};

        public override BaseAddonDeed Deed { get { return new RoseRugEastAddonDeed(m_NextUse); } }

        private DateTime m_NextUse;

        [Constructable]
        public RoseRugEastAddon() : this(DateTime.Now)
        {
        }

		[ Constructable ]
		public RoseRugEastAddon(DateTime nextuse)
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

                Engines.Plants.Seed seed = new Engines.Plants.Seed();

                if (cont == null || !cont.TryDropItem(from, seed, false))
                {
                    from.BankBox.DropItem(seed);
                    from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                }
                else
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                m_NextUse = DateTime.Now + TimeSpan.FromDays(7);
            }
        }

        public RoseRugEastAddon(Serial serial)
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

	public class RoseRugEastAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new RoseRugEastAddon(m_NextUse); } }
        public override int LabelNumber { get { return 1150049; } } // A Rose Rug (East)

        private DateTime m_NextUse;

        [Constructable]
        public RoseRugEastAddonDeed() : this(DateTime.Now)
        {
        }

		[Constructable]
		public RoseRugEastAddonDeed(DateTime nextuse)
		{
            m_NextUse = nextuse;
		}

		public RoseRugEastAddonDeed( Serial serial ) : base( serial )
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

    public class RoseRugSouthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {14551, 2, -3, 0}, {14550, 0, -3, 0}, {14549, 1, -3, 0}// 1	2	3	
			, {14552, -1, -3, 0}, {14542, 0, -1, 0}, {14543, 2, -1, 0}// 4	5	6	
			, {14536, -1, 1, 0}, {14527, 0, 3, 0}, {14530, 0, 2, 0}// 7	8	9	
			, {14537, 1, 0, 0}, {14535, 2, 1, 0}, {14544, -1, -1, 0}// 10	11	12	
			, {14532, -1, 2, 0}, {14529, 1, 2, 0}, {14541, 1, -1, 0}// 13	14	15	
			, {14547, 2, -2, 0}, {14548, -1, -2, 0}, {14526, 1, 3, 0}// 16	17	18	
			, {14531, 2, 2, 0}, {14546, 0, -2, 0}, {14540, -1, 0, 0}// 19	20	21	
			, {14528, -1, 3, 0}, {14534, 0, 1, 0}, {14525, 2, 3, 0}// 22	23	24	
			, {14533, 1, 1, 0}, {14545, 1, -2, 0}, {14539, 2, 0, 0}// 25	26	27	
			, {14538, 0, 0, 0}// 28	
		};

        public override BaseAddonDeed Deed { get { return new RoseRugSouthAddonDeed(m_NextUse); } }

        private DateTime m_NextUse;

        [Constructable]
        public RoseRugSouthAddon()
            : this(DateTime.Now)
        {
        }

        [Constructable]
        public RoseRugSouthAddon(DateTime nextuse)
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

                Engines.Plants.Seed seed = new Engines.Plants.Seed();

                if (cont == null || !cont.TryDropItem(from, seed, false))
                {
                    from.BankBox.DropItem(seed);
                    from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                }
                else
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                m_NextUse = DateTime.Now + TimeSpan.FromDays(7);
            }
        }

        public RoseRugSouthAddon(Serial serial)
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

    public class RoseRugSouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new RoseRugSouthAddon(m_NextUse); } }
        public override int LabelNumber { get { return 1150048; } } // A Rose Rug (South)

        private DateTime m_NextUse;

        [Constructable]
        public RoseRugSouthAddonDeed()
            : this(DateTime.Now)
        {
        }

        [Constructable]
        public RoseRugSouthAddonDeed(DateTime nextuse)
        {
            m_NextUse = nextuse;
        }

        public RoseRugSouthAddonDeed(Serial serial)
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