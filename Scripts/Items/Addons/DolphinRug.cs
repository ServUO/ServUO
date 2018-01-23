using System;
using Server;

namespace Server.Items
{
    [TypeAlias("Server.Items.DolphinRugEastAddon", "Server.Items.DolphinRugSouthAddon")]
	public class DolphinRugAddon : BaseAddon
	{
        private static int[,] _SouthLarge = new int[,] 
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

        private static int[,] _EastLarge = new int[,]
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

        private static int[,] _SouthSmall = 
        {	
              {18283, 1, 0, 0}, {18276, 1, 1, 0},   {18289, 1, 2, 0}    // 1	2	3	
			, {18288, 0, 2, 0}, {18287, -1, -2, 0}, {18286, 1, -2, 0}   // 4	5	6	
			, {18285, 0, -2, 0}, {18284, -1, 2, 0}, {18282, -1, 1, 0}   // 7	8	9	
			, {18281, -1, 0, 0}, {18280, 1, -1, 0}, {18279, 0, -1, 0}   // 10	11	12	
			, {18278, -1, -1, 0}, {18277, 0, 1, 0}, {18275, 0, 0, 0}    // 13	14	15	
		};

        private static int[,] _EastSmall = 
        {
              {18393, 2, -1, 0},  {18392, -2, -1, 0}, {18391, -2, 1, 0} // 1	2	3	
			, {18390, 1, -1, 0},  {18300, 1, 0, 0},   {18299, 1, 1, 0}  // 4	5	6	
			, {18298, 0, -1, 0},  {18297, 2, 0, 0},   {18296, 0, 1, 0}  // 7	8	9	
			, {18295, -1, -1, 0}, {18294, -1, 0, 0},  {18293, -1, 1, 0} // 10	11	12	
			, {18292, 2, 1, 0},   {18291, -2, 0, 0},  {18290, 0, 0, 0}  // 13	14	15	
        };

        public override BaseAddonDeed Deed { get { return new DolphinRugAddonDeed(NextUse, RugType); } }

        private DateTime NextUse { get; set; }
        public RugType RugType { get; set; }

        [Constructable]
        public DolphinRugAddon()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public DolphinRugAddon(RugType type)
            : this(type, DateTime.UtcNow)
        {
        }

		[ Constructable ]
		public DolphinRugAddon(RugType type, DateTime nextuse)
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
            if (NextUse < DateTime.UtcNow)
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

                NextUse = DateTime.UtcNow + TimeSpan.FromDays(7);
            }
        }

        public DolphinRugAddon(Serial serial)
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

    [TypeAlias("Server.Items.DolphinRugSouthAddonDeed", "Server.Items.DolphinRugEastAddonDeed")]
    public class DolphinRugAddonDeed : BaseRugAddonDeed
    {
        public override BaseAddon Addon { get { return new DolphinRugAddon(RugType, NextUse); } }

        public override int LabelNumber
        {
            get
            {
                switch ((int)RugType)
                {
                    default: return 1150050;
                    case 0:
                    case 2: return 1150051;
                }
            }
        }

        [Constructable]
        public DolphinRugAddonDeed(DateTime nextuse, RugType type)
            : base(DateTime.UtcNow, type)
        {
        }

        [Constructable]
        public DolphinRugAddonDeed(RugType type)
            : base(DateTime.UtcNow, type)
        {
        }

        [Constructable]
        public DolphinRugAddonDeed()
            : this(RugType.EastLarge)
        {
        }

        public DolphinRugAddonDeed(Serial serial)
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
