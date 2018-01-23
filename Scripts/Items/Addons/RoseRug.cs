using System;
using Server;
using Server.Accounting;
using Server.Engines.VeteranRewards;
using Server.Multis;

namespace Server.Items
{
    [TypeAlias("Server.Items.RoseRugEastAddon", "Server.Items.RoseRugSouthAddon")]
    public class RoseRugAddon : BaseAddon
    {
        private static int[,] _SouthLarge =
        {
              {14551, 2, -3, 0}, {14550, 0, -3, 0}, {14549, 1, -3, 0}   // 1	2	3	
			, {14552, -1, -3, 0}, {14542, 0, -1, 0}, {14543, 2, -1, 0}  // 4	5	6	
			, {14536, -1, 1, 0}, {14527, 0, 3, 0}, {14530, 0, 2, 0}     // 7	8	9	
			, {14537, 1, 0, 0}, {14535, 2, 1, 0}, {14544, -1, -1, 0}    // 10	11	12	
			, {14532, -1, 2, 0}, {14529, 1, 2, 0}, {14541, 1, -1, 0}    // 13	14	15	
			, {14547, 2, -2, 0}, {14548, -1, -2, 0}, {14526, 1, 3, 0}   // 16	17	18	
			, {14531, 2, 2, 0}, {14546, 0, -2, 0}, {14540, -1, 0, 0}    // 19	20	21	
			, {14528, -1, 3, 0}, {14534, 0, 1, 0}, {14525, 2, 3, 0}     // 22	23	24	
			, {14533, 1, 1, 0}, {14545, 1, -2, 0}, {14539, 2, 0, 0}     // 25	26	27	
			, {14538, 0, 0, 0}// 28	
		};

        private static int[,] _EastLarge =
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

        private static int[,] _SouthSmall =
        {
              {18249, 0, -1, 0},  {18248, -1, -1, 0}, {18250, 1, -1, 0} // 1	2	3	
			, {18251, -1, 0, 0},  {18252, 1, 2, 0},   {18253, 1, 0, 0}  // 4	5	6	
			, {18246, 0, -2, 0},  {18254, -1, 1, 0},  {18255, 0, 1, 0}  // 7	8	9	
			, {18256, 1, 1, 0},   {18257, -1, 2, 0},  {18258, 0, 2, 0}  // 10	11	12	
			, {18259, -1, -2, 0}, {18245, 0, 0, 0},   {18247, 1, -2, 0} // 13	14	15	
		};

        private static int[,] _EastSmall =
        {
              {18269, 1, 1, 0},  {18270, 1, 0, 0},   {18271, 1, -1, 0}  // 1	2	3	
			, {18272, -2, 0, 0}, {18273, -2, 1, 0},  {18274, -2, -1, 0} // 4	5	6	
			, {18268, 0, -1, 0}, {18260, 0, 0, 0},   {18261, 2, 0, 0}   // 7	8	9	
			, {18264, -1, 0, 0}, {18265, -1, -1, 0}, {18266, 0, 1, 0}   // 10	11	12	
			, {18267, 2, 1, 0},  {18262, 2, -1, 0},  {18263, -1, 1, 0}  // 13	14	15
        };

        public override BaseAddonDeed Deed { get { return new RoseRugAddonDeed(NextUse, RugType); } }

        private DateTime NextUse { get; set; }
        public RugType RugType { get; set; }

        [Constructable]
        public RoseRugAddon()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public RoseRugAddon(RugType type)
            : this(type, DateTime.UtcNow)
        {
        }

        [Constructable]
        public RoseRugAddon(RugType type, DateTime nextuse)
        {
            NextUse = nextuse;
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
            Account acct = from.Account as Account;

            if (acct != null && from.IsPlayer())
            {
                TimeSpan time = TimeSpan.FromDays(RewardSystem.RewardInterval.TotalDays * 6) - (DateTime.Now - acct.Created);

                if (time > TimeSpan.Zero)
                {
                    from.SendLocalizedMessage(1008126, true, Math.Ceiling(time.TotalDays / RewardSystem.RewardInterval.TotalDays).ToString()); // Your account is not old enough to use this item. Months until you can use this item :
                    return;
                }
            }

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsOwner(from))
            {
                if (NextUse < DateTime.UtcNow)
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

                    NextUse = DateTime.UtcNow + TimeSpan.FromDays(7);
                }
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }

        public RoseRugAddon(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(NextUse);
            writer.Write((int)RugType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    NextUse = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 0:
                    NextUse = reader.ReadDateTime();
                    break;
            }
        }
    }

    [TypeAlias("Server.Items.RoseRugSouthAddonDeed", "Server.Items.RoseRugEastAddonDeed")]
    public class RoseRugAddonDeed : BaseRugAddonDeed
    {
        public override BaseAddon Addon { get { return new RoseRugAddon(RugType, NextUse); } }

        public override int LabelNumber
        {
            get
            {
                switch ((int)RugType)
                {
                    default: return 1150048;
                    case 0:
                    case 2: return 1150049;
                }
            }
        }

        [Constructable]
        public RoseRugAddonDeed(DateTime nextuse, RugType type)
            : base(nextuse, type)
        {
        }

        [Constructable]
        public RoseRugAddonDeed(RugType type)
            : base(DateTime.UtcNow, type)
        {
        }

        [Constructable]
        public RoseRugAddonDeed()
            : this(RugType.EastLarge)
        {
        }

        public RoseRugAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
