using System;
using Server.Gumps;

namespace Server.Items
{
    public class HildebrandtDragonRugAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new HildebrandtDragonRugDeed(); } }

        [Constructable]
        public HildebrandtDragonRugAddon()
            : this(true)
        {
        }

        [Constructable]
        public HildebrandtDragonRugAddon(bool south)
        {
            if (south)
            {
                for (int i = 0; i < m_SouthInfo.Length / 4; i++)
                    AddComponent(new AddonComponent(m_SouthInfo[i, 0]), m_SouthInfo[i, 1], m_SouthInfo[i, 2], m_SouthInfo[i, 3]);
            }
            else
            {
                for (int i = 0; i < m_EastInfo.Length / 4; i++)
                    AddComponent(new AddonComponent(m_EastInfo[i, 0]), m_EastInfo[i, 1], m_EastInfo[i, 2], m_EastInfo[i, 3]);
            }
        }

        private static int[,] m_SouthInfo = new int[,] 
        {
			  {40909, -2, -2, 0}, {40908, -2, -1, 0}, {40898, -1, -1, 0}// 1	2	3	
			, {40897, -1, 0, 0}, {40896, -1, 1, 0}, {40895, -1, 2, 0}// 4	5	6	
			, {40887, 1, 0, 0}, {40886, 0, 1, 0}, {40885, 1, 1, 0}// 7	8	9	
			, {40904, 2, -2, 0}, {40894, 1, 2, 0}, {40888, 0, 0, 0}// 10	11	12	
			, {40891, 1, -2, 0}, {40892, 0, -2, 0}, {40893, 0, 2, 0}// 13	14	15	
			, {40899, -1, -2, 0}, {40900, 2, 2, 0}, {40901, 2, 1, 0}// 16	17	18	
			, {40902, 2, 0, 0}, {40903, 2, -1, 0}, {40889, 1, -1, 0}// 19	20	21	
			, {40890, 0, -1, 0}, {40905, -2, 2, 0}, {40906, -2, 1, 0}// 22	23	24	
			, {40907, -2, 0, 0}// 25	
		};

        private static int[,] m_EastInfo = new int[,] 
        {
			  {40926, 2, 1, 0}, {40918, 0, 2, 0}, {40919, 1, 2, 0}// 1	2	3	
			, {40920, -1, 2, 0}, {40910, 1, 1, 0}, {40911, 0, 1, 0}// 4	5	6	
			, {40930, -2, 2, 0}, {40921, -1, 1, 0}, {40931, -2, 1, 0}// 7	8	9	
			, {40925, 2, 2, 0}, {40914, 1, -1, 0}, {40915, 0, -1, 0}// 10	11	12	
			, {40916, 0, -2, 0}, {40917, 1, -2, 0}, {40913, 0, 0, 0}// 13	14	15	
			, {40934, -2, -2, 0}, {40927, 2, 0, 0}, {40932, -2, -1, 0}// 16	17	18	
			, {40933, -2, 0, 0}, {40922, -1, 0, 0}, {40923, -1, -1, 0}// 19	20	21	
			, {40928, 2, -1, 0}, {40929, 2, -2, 0}, {40924, -1, -2, 0}// 22	23	24	
			, {40912, 1, 0, 0}// 25	
		};

        public HildebrandtDragonRugAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HildebrandtDragonRugDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new HildebrandtDragonRugAddon(m_South); } }
        public override int LabelNumber { get { return 1157889; } } // Hildebrandt Dragon Rug

        private bool m_South;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool South
        {
            get { return m_South; }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1116332); // South 
            list.Add(1, 1116333); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_South = choice == 0;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        [Constructable]
        public HildebrandtDragonRugDeed()
        {
        }

        public HildebrandtDragonRugDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
