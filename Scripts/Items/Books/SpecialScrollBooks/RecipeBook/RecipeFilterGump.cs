using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class RecipeScrollFilterGump : Gump
    {
        private readonly Mobile m_From;
        private readonly RecipeBook m_Book;

        private const int LabelColor = 0x7FFF;
        private const int TitleLabelColor = 0xFFFFFF;

        private void AddFilterList(int x, int y, int[] xOffsets, int yOffset, int[,] filters, int xHeights, int filterValue, int filterIndex)
        {
            for (int i = 0; i < filters.GetLength(0); ++i)
            {
                int number = filters[i, 0];

                if (number == 0)
                    continue;

                bool isSelected = (filters[i, 1] == filterValue);

                AddHtmlLocalized(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 80, xHeights, number, isSelected ? 16927 : LabelColor, false, false);
                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
            }
        }

        private static readonly int[,] m_SkillFilters = new int[,]
        {
            { 1062229, 0 }, // All
            { 1150187, 1 }, // Blacksmith
            { 1150188, 2 }, // Tailor
            { 1044068, 3 }, // Fletching
            { 1002054, 4 }, // Carpentry
            { 1002090, 5 }, // Inscription
            { 1002063, 6 }, // Cooking
            { 1002000, 7 }, // Alchemy
            { 1002162, 8 }, // Tinkering
            { 1002057, 9 }, // Cartography
        };

        private static readonly int[,] m_ExpansionFilters = new int[,]
        {
            { 1062229, 0 }, // All
            { 1158817, 1 }, // Mondain's Legacy
            { 1095190, 2 }, // Stygian Abyss
            { 1158818, 3 }, // Time of Legends
        };

        private static readonly int[,] m_AmountFilters = new int[,]
        {
            { 1062229, 0 }, // All
            { 1158815, 1 }, // Owned
            { 1074235, 2 }, // Unknown
        };

        private static readonly int[][,] m_Filters = new int[][,]
        {
            m_SkillFilters,
            m_ExpansionFilters,
            m_AmountFilters
        };

        private static readonly int[] m_XOffsets_Skill = new int[] { 0, 125, 250, 375 };
        private static readonly int[] m_XOffsets_Expansion = new int[] { 0, 125, 250, 375 };
        private static readonly int[] m_XOffsets_Amount = new int[] { 0, 125, 250 };

        public RecipeScrollFilterGump(Mobile from, RecipeBook book)
            : base(12, 24)
        {
            from.CloseGump(typeof(RecipeBookGump));
            from.CloseGump(typeof(RecipeScrollFilterGump));

            m_From = from;
            m_Book = book;

            RecipeScrollFilter f = book.Filter;

            AddPage(0);

            AddBackground(10, 10, 600, 375, 0x13BE);

            AddImageTiled(18, 20, 583, 356, 0xA40);
            AddAlphaRegion(18, 20, 583, 356);

            AddImage(0, 0, 0x28DC);
            AddImage(590, 0, 0x28DC);
            AddImage(0, 365, 0x28DC);
            AddImage(590, 365, 0x28DC);

            AddHtmlLocalized(26, 64, 120, 32, 1158816, TitleLabelColor, false, false); // Crafting
            AddHtmlLocalized(270, 32, 200, 32, 1062223, TitleLabelColor, false, false); // Filter Preference

            AddHtmlLocalized(26, 64, 120, 32, 1158816, TitleLabelColor, false, false); // Crafting
            AddFilterList(25, 96, m_XOffsets_Skill, 32, m_SkillFilters, 32, f.Skill, 0);

            AddHtmlLocalized(26, 192, 120, 42, 1158814, TitleLabelColor, false, false); // Expansion
            AddFilterList(25, 224, m_XOffsets_Expansion, 32, m_ExpansionFilters, 42, f.Expansion, 1);

            AddHtmlLocalized(26, 256, 120, 32, 1062217, TitleLabelColor, false, false); // Amount
            AddFilterList(25, 288, m_XOffsets_Amount, 32, m_AmountFilters, 32, f.Amount, 2);

            AddHtmlLocalized(75, 352, 120, 32, 1062477, (((PlayerMobile)from).UseOwnFilter ? LabelColor : 16927), false, false); // Set Book Filter
            AddButton(40, 352, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(235, 352, 120, 32, 1062478, (((PlayerMobile)from).UseOwnFilter ? 16927 : LabelColor), false, false); // Set Your Filter
            AddButton(200, 352, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(405, 352, 120, 32, 1062231, TitleLabelColor, false, false); // Clear Filter
            AddButton(370, 352, 4005, 4007, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(540, 352, 50, 32, 1011046, TitleLabelColor, false, false); // APPLY
            AddButton(505, 352, 4017, 4018, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            RecipeScrollFilter f = m_Book.Filter;

            int index = info.ButtonID;

            switch (index)
            {
                case 0: // APPLY
                    {
                        m_From.SendGump(new RecipeBookGump(m_From, m_Book));

                        break;
                    }
                case 1: // Set Book Filter
                    {
                        ((PlayerMobile)m_From).UseOwnFilter = false;
                        m_From.SendGump(new RecipeScrollFilterGump(m_From, m_Book));

                        break;
                    }
                case 2: // Set Your Filter
                    {
                        ((PlayerMobile)m_From).UseOwnFilter = true;
                        m_From.SendGump(new RecipeScrollFilterGump(m_From, m_Book));

                        break;
                    }
                case 3: // Clear
                    {
                        f.Clear();
                        m_From.SendGump(new RecipeScrollFilterGump(m_From, m_Book));

                        break;
                    }
                default:
                    {
                        index -= 4;

                        int type = index % 4;
                        index /= 4;

                        int[][,] filter = m_Filters;

                        if (type >= 0 && type < filter.Length)
                        {
                            int[,] filters = filter[type];

                            if (index >= 0 && index < filters.GetLength(0))
                            {
                                if (filters[index, 0] == 0)
                                    break;

                                switch (type)
                                {
                                    case 0:
                                        f.Skill = filters[index, 1];
                                        break;
                                    case 1:
                                        f.Expansion = filters[index, 1];
                                        break;
                                    case 2:
                                        f.Amount = filters[index, 1];
                                        break;
                                }

                                m_From.SendGump(new RecipeScrollFilterGump(m_From, m_Book));
                            }
                        }

                        break;
                    }
            }
        }
    }
}
