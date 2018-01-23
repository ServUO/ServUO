using System;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    [TypeAlias("Server.Items.SkullRugEastAddon", "Server.Items.SkullRugSouthAddon")]
	public class SkullRugAddon : BaseAddon
	{
        private static int[,] _EastLarge = new int[,] 
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

        private static int[,] _SouthLarge =
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

        private static int[,] _EastSmall = 
        {	
              {18198, 1, 2, 0},   {18199, 0, 2, 0},  {18200, -1, 2, 0}  // 1	2	3	
			, {18209, 1, 1, 0},   {18210, 0, 1, 0},  {18211, -1, 1, 0}  // 4	5	6	
			, {18244, -1, -2, 0}, {18243, 0, -2, 0}, {18242, 1, -2, 0}  // 7	8	9	
			, {18241, -1, -1, 0}, {18240, 0, -1, 0}, {18239, 1, -1, 0}  // 10	11	12	
			, {18238, -1, 0, 0},  {18236, 1, 0, 0},  {18237, 0, 0, 0}   // 13	14	15	
		};

        private static int[,] _SouthSmall = 
        {
              {18197, 1, 1, 0},   {18196, -2, 1, 0}, {18195, -1, 1, 0}  // 1	2	3	
			, {18194, 2, 1, 0},   {18193, 0, 1, 0},  {18192, 1, 0, 0}   // 4	5	6	
			, {18191, 0, -1, 0},  {18190, 2, -1, 0}, {18189, 1, -1, 0}  // 7	8	9	
			, {18188, -1, -1, 0}, {18187, -1, 0, 0}, {18186, 2, 0, 0}   // 10	11	12	
			, {18185, -2, -1, 0}, {18184, -2, 0, 0}, {18183, 0, 0, 0}   // 13	14	15	
        };

        public override BaseAddonDeed Deed { get { return new SkullRugAddonDeed(RugType, m_NextUse); } }

        private DateTime m_NextUse;

        public RugType RugType { get; set; }

        [Constructable]
        public SkullRugAddon()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public SkullRugAddon(RugType type)
            : this(type, DateTime.UtcNow)
        {
        }

		[ Constructable ]
		public SkullRugAddon(RugType type, DateTime nextuse)
		{
            m_NextUse = nextuse;
            RugType = type;

            int[,] list;

            switch (type)
            {
                default:
                case RugType.EastLarge: list = _EastLarge; break;
                case RugType.SouthLarge: list = _SouthLarge; break;
                case RugType.EastSmall: list = _EastSmall; break;
                case RugType.SouthSmall: list = _SouthSmall; break;
            }

            for (int i = 0; i < list.Length / 4; i++)
                AddComponent(new AddonComponent(list[i, 0]), list[i, 1], list[i, 2], list[i, 3]);
		}

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (m_NextUse < DateTime.UtcNow)
            {
                Container cont = from.Backpack;
                Map facet;
                int level = Utility.RandomMinMax(1, 4);

                int choose = Siege.SiegeShard ? Utility.RandomMinMax(1, 5) : Utility.Random(6);

                switch (choose)
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

                m_NextUse = DateTime.UtcNow + TimeSpan.FromDays(7);
            }
        }

        public SkullRugAddon(Serial serial)
            : base(serial)
		{
		}


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(m_NextUse);
            writer.Write((int)RugType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_NextUse = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 0:
                    m_NextUse = reader.ReadDateTime();
                    break;
            }
        }
	}

    [TypeAlias("Server.Items.SkullRugSouthAddonDeed", "Server.Items.SkullRugEastAddonDeed")]
    public class SkullRugAddonDeed : BaseAddonDeed, IRewardOption
	{
        public override BaseAddon Addon { get { return new SkullRugAddon(RugType, m_NextUse); } }

        public override int LabelNumber
        {
            get
            {
                switch ((int)RugType)
                {
                    default: return 1150046;
                    case 0:
                    case 2: return 1150047;
                }
            }
        }

        private DateTime m_NextUse;

        [CommandProperty(AccessLevel.GameMaster)]
        public RugType RugType { get; set; }

        [Constructable]
        public SkullRugAddonDeed()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public SkullRugAddonDeed(RugType type)
            : this(type, DateTime.UtcNow)
        {
        }

		[Constructable]
		public SkullRugAddonDeed(RugType type, DateTime nextuse)
		{
            RugType = type;
            m_NextUse = nextuse;
            LootType = LootType.Blessed;
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076583)); // Please select your rug size
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, "Skull Rug East 7x7");
            list.Add(2, "Skull Rug South 7x7");
            list.Add(3, "Skull Rug East 3x5");
            list.Add(4, "Skull Rug South 3x5");
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            RugType = (RugType)choice - 1;

            if (!Deleted && IsChildOf(from.Backpack))
                base.OnDoubleClick(from);
        }

        public SkullRugAddonDeed(Serial serial)
            : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(m_NextUse);
            writer.Write((int)RugType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_NextUse = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 0:
                    m_NextUse = reader.ReadDateTime();
                    break;
            }
        }
	}
}