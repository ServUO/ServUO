using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class SkullRugEastAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {14456, 0, 2, 0}, {14455, 0, -1, 0}, {14441, -3, 2, 0}// 1	2	3	
			, {14451, -1, -1, 0}, {14466, 3, 0, 0}, {14442, -3, 1, 0}// 4	5	6	
			, {14445, -2, 1, 0}, {14468, 3, 2, 0}, {14453, 0, 1, 0}// 7	8	9	
			, {14444, -3, -1, 0}, {14460, 1, 2, 0}, {14467, 3, -1, 0}// 10	11	12	
			, {14461, 2, 1, 0}, {14462, 2, 0, 0}, {14459, 1, -1, 0}// 13	14	15	
			, {14447, -2, -1, 0}, {14450, -1, 0, 0}, {14454, 0, 0, 0}// 16	17	18	
			, {14452, -1, 2, 0}, {14448, -2, 2, 0}, {14446, -2, 0, 0}// 19	20	21	
			, {14449, -1, 1, 0}, {14463, 2, -1, 0}, {14458, 1, 0, 0}// 22	23	24	
			, {14464, 2, 2, 0}, {14465, 3, 1, 0}, {14457, 1, 1, 0}// 25	26	27	
			, {14443, -3, 0, 0}// 28	
		};

        public override BaseAddonDeed Deed { get { return new SkullRugEastAddonDeed(m_NextUse); } }

        private DateTime m_NextUse;

        [Constructable]
        public SkullRugEastAddon() : this(DateTime.Now)
        {
        }

		[ Constructable ]
		public SkullRugEastAddon(DateTime nextuse)
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
                Map facet;
                int level = Utility.RandomMinMax(1, 4);

                switch (Utility.Random(6))
                {
                    default:
                    case 0: facet = Map.Trammel; break;
                    case 1: facet = Map.Ilshenar; break;
                    case 2: facet = Map.Malas; break;
                    case 3: facet = Map.Felucca; break;
                    case 4: facet = Map.Tokuno; break;
                    case 5: facet = Map.TerMur; break;
                }

                TreasureMap map = new TreasureMap(level, facet);

                if (cont == null || !cont.TryDropItem(from, map, false))
                {
                    from.BankBox.DropItem(map);
                    from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                }
                else
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                m_NextUse = DateTime.Now + TimeSpan.FromDays(7);
            }
        }

        public SkullRugEastAddon(Serial serial)
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

	public class SkullRugEastAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new SkullRugEastAddon(m_NextUse); } }
        public override int LabelNumber { get { return 1150047; } } // A Skull Rug (East)

        private DateTime m_NextUse;

        [Constructable]
        public SkullRugEastAddonDeed() : this(DateTime.Now)
        {
        }

		[Constructable]
		public SkullRugEastAddonDeed(DateTime nextuse)
		{
            m_NextUse = nextuse;
		}

		public SkullRugEastAddonDeed( Serial serial ) : base( serial )
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

    public class SkullRugSouthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			  {14495, -1, -3, 0}, {14494, 0, -3, 0}, {14493, 1, -3, 0}// 1	2	3	
			, {14496, 2, -3, 0}, {14486, 0, -1, 0}, {14487, -1, -1, 0}// 4	5	6	
			, {14480, 2, 1, 0}, {14471, 0, 3, 0}, {14474, 0, 2, 0}// 7	8	9	
			, {14481, 1, 0, 0}, {14479, -1, 1, 0}, {14488, 2, -1, 0}// 10	11	12	
			, {14476, 2, 2, 0}, {14473, 1, 2, 0}, {14485, 1, -1, 0}// 13	14	15	
			, {14491, -1, -2, 0}, {14492, 2, -2, 0}, {14470, 1, 3, 0}// 16	17	18	
			, {14475, -1, 2, 0}, {14490, 0, -2, 0}, {14484, 2, 0, 0}// 19	20	21	
			, {14472, -1, 3, 0}, {14478, 0, 1, 0}, {14469, 2, 3, 0}// 22	23	24	
			, {14477, 1, 1, 0}, {14489, 1, -2, 0}, {14483, -1, 0, 0}// 25	26	27	
			, {14482, 0, 0, 0}// 28	
		};

        public override BaseAddonDeed Deed { get { return new SkullRugSouthAddonDeed(m_NextUse); } }

        private DateTime m_NextUse;

        [Constructable]
        public SkullRugSouthAddon()
            : this(DateTime.Now)
        {
        }

        [Constructable]
        public SkullRugSouthAddon(DateTime nextuse)
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
                Map facet;
                int level = Utility.RandomMinMax(1, 4);

                switch (Utility.Random(6))
                {
                    default:
                    case 0: facet = Map.Trammel; break;
                    case 1: facet = Map.Ilshenar; break;
                    case 2: facet = Map.Malas; break;
                    case 3: facet = Map.Felucca; break;
                    case 4: facet = Map.Tokuno; break;
                    case 5: facet = Map.TerMur; break;
                }

                TreasureMap map = new TreasureMap(level, facet);

                if (cont == null || !cont.TryDropItem(from, map, false))
                {
                    from.BankBox.DropItem(map);
                    from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                }
                else
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                m_NextUse = DateTime.Now + TimeSpan.FromDays(7);
            }
        }

        public SkullRugSouthAddon(Serial serial)
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

    public class SkullRugSouthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SkullRugSouthAddon(m_NextUse); } }
        public override int LabelNumber { get { return 1150046; } } // A Skull Rug (Sast)

        private DateTime m_NextUse;

        [Constructable]
        public SkullRugSouthAddonDeed()
            : this(DateTime.Now)
        {
        }

        [Constructable]
        public SkullRugSouthAddonDeed(DateTime nextuse)
        {
            m_NextUse = nextuse;
        }

        public SkullRugSouthAddonDeed(Serial serial)
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