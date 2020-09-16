using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    public class BOBFilterGump : Gump
    {
        private static readonly int[,] m_MaterialFilters = new int[,]
            {
                { 1044067,  1 }, // Blacksmithy
				{ 1062226,  3 }, // Iron
				{ 1018332,  4 }, // Dull Copper
				{ 1018333,  5 }, // Shadow Iron
				{ 1018334,  6 }, // Copper
				{ 1018335,  7 }, // Bronze

				{       0,  0 }, // --Blank--
				{ 1018336,  8 }, // Golden
				{ 1018337,  9 }, // Agapite
				{ 1018338, 10 }, // Verite
				{ 1018339, 11 }, // Valorite
				{       0,  0 }, // --Blank--

				{ 1044094,  2 }, // Tailoring
				{ 1044286, 12 }, // Cloth
				{ 1062235, 13 }, // Leather
				{ 1062236, 14 }, // Spined
				{ 1062237, 15 }, // Horned
				{ 1062238, 16 }  // Barbed
			};

        private static readonly int[,] m_MaterialFiltersNew = new int[,]
        {
            { 1044067, 1 }, // Blacksmithy
            { 1062226, 9 }, // Iron
            { 1018332, 10 }, // Dull Copper
            { 1018333, 11 }, // Shadow Iron
            { 1018334, 12 }, // Copper
            { 1018335, 13 }, // Bronze

            { 0, 0 }, // --Blank--
            { 1018336, 14 }, // Golden
            { 1018337, 15 }, // Agapite
            { 1018338, 16 }, // Verite
            { 1018339, 17 }, // Valorite
            { 0, 0 }, // --Blank--

            { 1044094, 2 }, // Tailoring
            { 1044286, 18 }, // Cloth
            { 1062235, 19 }, // Leather
            { 1062236, 20 }, // Spined
            { 1062237, 21 }, // Horned
            { 1062238, 22 }, // Barbed

            { 1044097, 3 }, // Tinkering
            { 1062226, 23 }, // Iron
            { 1018332, 24 }, // Dull Copper
            { 1018333, 25 }, // Shadow Iron
            { 1018334, 26 }, // Copper
            { 1018335, 27 }, // Bronze

            { 0, 0 }, // --Blank--
            { 1018336, 28 }, // Golden
            { 1018337, 29 }, // Agapite
            { 1018338, 30 }, // Verite
            { 1018339, 31 }, // Valorite
            { 0, 0 }, // --Blank--

            { 1044071, 4 }, // Carpentry
            { 1079435, 32 }, // Wood
            { 1071428, 33 }, // Oak
            { 1071429, 34 }, // Ash
            { 1071430, 35 }, // Yew
            { 0, 0 }, // --Blank--

            { 0, 0 }, // --Blank--
            { 1071431, 36 }, // Bloodwood
            { 1071432, 37 }, // Heartwood
            { 1071433, 38 }, // Frostwood
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--

            { 1044068, 5 }, // Fletching
            { 1079435, 39 }, // Wood
            { 1071428, 40 }, // Oak
            { 1071429, 41 }, // Ash
            { 1071430, 42 }, // Yew
            { 0, 0 }, // --Blank--

            { 0, 0 }, // --Blank--
            { 1071431, 43 }, // Bloodwood
            { 1071432, 44 }, // Heartwood
            { 1071433, 45 }, // Frostwood
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--

            { 1044060, 6 }, // Alchemy
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--

            { 1044083, 7 }, // Inscription
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--

            { 1044073, 8 }, // Cooking
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
            { 0, 0 }, // --Blank--
        };

        private static readonly int[,] m_TypeFilters = new int[,]
        {
            { 1062229, 0 }, // All
            { 1062224, 1 }, // Small
            { 1062225, 2 }// Large
        };

        private static readonly int[,] m_QualityFilters = new int[,]
        {
            { 1062229, 0 }, // All
            { 1011542, 1 }, // Normal
            { 1060636, 2 }// Exceptional
        };

        private static readonly int[,] m_AmountFilters = new int[,]
        {
            { 1062229, 0 }, // All
            { 1049706, 1 }, // 10
            { 1016007, 2 }, // 15
            { 1062239, 3 }// 20
        };

        private static readonly int[][,] m_Filters = new int[][,]
        {
            m_TypeFilters,
            m_QualityFilters,
            m_MaterialFilters,
            m_AmountFilters
        };

        private static readonly int[][,] m_FiltersNew = new int[][,]
        {
            m_TypeFilters,
            m_QualityFilters,
            m_MaterialFiltersNew,
            m_AmountFilters
        };

        private static readonly int[] m_XOffsets_Type = new int[] { 0, 75, 170 };
        private static readonly int[] m_XOffsets_Quality = new int[] { 0, 75, 170 };
        private static readonly int[] m_XOffsets_Amount = new int[] { 0, 75, 180, 275 };
        private static readonly int[] m_XOffsets_Material = new int[] { 0, 108, 212, 307, 392, 487 };
        private static readonly int[] m_XWidths_Small = new int[] { 50, 50, 70, 50 };
        private static readonly int[] m_XWidths_Large = new int[] { 80, 60, 60, 60, 60, 60 };

        private const int LabelColor = 0x7FFF;
        private readonly PlayerMobile m_From;
        private readonly BulkOrderBook m_Book;

        public BOBFilterGump(PlayerMobile from, BulkOrderBook book)
            : base(12, 24)
        {
            from.CloseGump(typeof(BOBGump));
            from.CloseGump(typeof(BOBFilterGump));

            m_From = from;
            m_Book = book;

            BOBFilter f = (from.UseOwnFilter ? from.BOBFilter : book.Filter);

            AddPage(0);

            AddBackground(10, 10, 600, 695, 5054);

            AddImageTiled(18, 20, 583, 676, 2624);
            AddAlphaRegion(18, 20, 583, 676);

            AddImage(5, 5, 10460);
            AddImage(585, 5, 10460);
            AddImage(5, 690, 10460);
            AddImage(585, 690, 10460);

            AddHtmlLocalized(270, 32, 200, 32, 1062223, LabelColor, false, false); // Filter Preference

            AddHtmlLocalized(26, 64, 120, 32, 1062228, LabelColor, false, false); // Bulk Order Type
            AddFilterList(25, 96, m_XOffsets_Type, 40, m_TypeFilters, m_XWidths_Small, f.Type, 0);

            AddHtmlLocalized(320, 64, 50, 32, 1062215, LabelColor, false, false); // Quality
            AddFilterList(320, 96, m_XOffsets_Quality, 40, m_QualityFilters, m_XWidths_Small, f.Quality, 1);

            AddHtmlLocalized(26, 130, 120, 32, 1062232, LabelColor, false, false); // Material Type
            AddFilterList(25, 162, m_XOffsets_Material, 35, BulkOrderSystem.NewSystemEnabled ? m_MaterialFiltersNew : m_MaterialFilters, m_XWidths_Large, f.Material, 2);

            AddHtmlLocalized(26, 608, 120, 32, 1062217, LabelColor, false, false); // Amount
            AddFilterList(25, 640, m_XOffsets_Amount, 40, m_AmountFilters, m_XWidths_Small, f.Quantity, 3);

            AddHtmlLocalized(75, 670, 120, 32, 1062477, (from.UseOwnFilter ? LabelColor : 16927), false, false); // Set Book Filter
            AddButton(40, 670, 4005, 4007, 1, GumpButtonType.Reply, 0);

            AddHtmlLocalized(235, 670, 120, 32, 1062478, (from.UseOwnFilter ? 16927 : LabelColor), false, false); // Set Your Filter
            AddButton(200, 670, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(405, 670, 120, 32, 1062231, LabelColor, false, false); // Clear Filter
            AddButton(370, 670, 4005, 4007, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(540, 670, 50, 32, 1011046, LabelColor, false, false); // APPLY
            AddButton(505, 670, 4017, 4018, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            BOBFilter f = (m_From.UseOwnFilter ? m_From.BOBFilter : m_Book.Filter);

            int index = info.ButtonID;

            switch (index)
            {
                case 0: // Apply
                    {
                        m_From.SendGump(new BOBGump(m_From, m_Book));

                        break;
                    }
                case 1: // Set Book Filter
                    {
                        m_From.UseOwnFilter = false;
                        m_From.SendGump(new BOBFilterGump(m_From, m_Book));

                        break;
                    }
                case 2: // Set Your Filter
                    {
                        m_From.UseOwnFilter = true;
                        m_From.SendGump(new BOBFilterGump(m_From, m_Book));

                        break;
                    }
                case 3: // Clear Filter
                    {
                        f.Clear();
                        m_From.SendGump(new BOBFilterGump(m_From, m_Book));

                        break;
                    }
                default:
                    {
                        index -= 4;

                        int type = index % 4;
                        index /= 4;

                        int[][,] filter = BulkOrderSystem.NewSystemEnabled ? m_FiltersNew : m_Filters;

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
                                        f.Type = filters[index, 1];
                                        break;
                                    case 1:
                                        f.Quality = filters[index, 1];
                                        break;
                                    case 2:
                                        f.Material = filters[index, 1];
                                        break;
                                    case 3:
                                        f.Quantity = filters[index, 1];
                                        break;
                                }

                                m_From.SendGump(new BOBFilterGump(m_From, m_Book));
                            }
                        }

                        break;
                    }
            }
        }

        private void AddFilterList(int x, int y, int[] xOffsets, int yOffset, int[,] filters, int[] xWidths, int filterValue, int filterIndex)
        {
            for (int i = 0; i < filters.GetLength(0); ++i)
            {
                int number = filters[i, 0];

                if (number == 0)
                    continue;

                bool isSelected = (filters[i, 1] == filterValue);

                if (!isSelected && (i % xOffsets.Length) == 0)
                    isSelected = (filterValue == 0);

                AddHtmlLocalized(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], 32, number, isSelected ? 16927 : LabelColor, false, false);
                AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
            }
        }
    }
}