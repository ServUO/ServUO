using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.BulkOrders
{
    public class BOBFilterGump : Gump
    {
        private static readonly int[,] m_MaterialFilters = new int[,]
        {
            { 1044067, 1 }, // Blacksmithy
            { 1062226, 3 }, // Iron
            { 1018332, 4 }, // Dull Copper
            { 1018333, 5 }, // Shadow Iron
            { 1018334, 6 }, // Copper
            { 1018335, 7 }, // Bronze

            { 0, 0 }, // --Blank--
            { 1018336, 8 }, // Golden
            { 1018337, 9 }, // Agapite
            { 1018338, 10 }, // Verite
            { 1018339, 11 }, // Valorite
            { 0, 0 }, // --Blank--

            { 1044094, 2 }, // Tailoring
            { 1044286, 12 }, // Cloth
            { 1062235, 13 }, // Leather
            { 1062236, 14 }, // Spined
            { 1062237, 15 }, // Horned
            { 1062238, 16 }// Barbed
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
        private static readonly int[] m_XOffsets_Type = new int[] { 0, 75, 170 };
        private static readonly int[] m_XOffsets_Quality = new int[] { 0, 75, 170 };
        private static readonly int[] m_XOffsets_Amount = new int[] { 0, 75, 180, 275 };
        private static readonly int[] m_XOffsets_Material = new int[] { 0, 105, 210, 305, 390, 485 };
        private static readonly int[] m_XWidths_Small = new int[] { 50, 50, 70, 50 };
        private static readonly int[] m_XWidths_Large = new int[] { 80, 50, 50, 50, 50, 50 };
        private const int LabelColor = 0x7FFF;
        private readonly PlayerMobile m_From;
        private readonly BulkOrderBook m_Book;
        public BOBFilterGump(PlayerMobile from, BulkOrderBook book)
            : base(12, 24)
        {
            from.CloseGump(typeof(BOBGump));
            from.CloseGump(typeof(BOBFilterGump));

            this.m_From = from;
            this.m_Book = book;

            BOBFilter f = (from.UseOwnFilter ? from.BOBFilter : book.Filter);

            this.AddPage(0);

            this.AddBackground(10, 10, 600, 439, 5054);

            this.AddImageTiled(18, 20, 583, 420, 2624);
            this.AddAlphaRegion(18, 20, 583, 420);

            this.AddImage(5, 5, 10460);
            this.AddImage(585, 5, 10460);
            this.AddImage(5, 424, 10460);
            this.AddImage(585, 424, 10460);

            this.AddHtmlLocalized(270, 32, 200, 32, 1062223, LabelColor, false, false); // Filter Preference

            this.AddHtmlLocalized(26, 64, 120, 32, 1062228, LabelColor, false, false); // Bulk Order Type
            this.AddFilterList(25, 96, m_XOffsets_Type, 40, m_TypeFilters, m_XWidths_Small, f.Type, 0);

            this.AddHtmlLocalized(320, 64, 50, 32, 1062215, LabelColor, false, false); // Quality
            this.AddFilterList(320, 96, m_XOffsets_Quality, 40, m_QualityFilters, m_XWidths_Small, f.Quality, 1);

            this.AddHtmlLocalized(26, 160, 120, 32, 1062232, LabelColor, false, false); // Material Type
            this.AddFilterList(25, 192, m_XOffsets_Material, 40, m_MaterialFilters, m_XWidths_Large, f.Material, 2);

            this.AddHtmlLocalized(26, 320, 120, 32, 1062217, LabelColor, false, false); // Amount
            this.AddFilterList(25, 352, m_XOffsets_Amount, 40, m_AmountFilters, m_XWidths_Small, f.Quantity, 3);

            this.AddHtmlLocalized(75, 416, 120, 32, 1062477, (from.UseOwnFilter ? LabelColor : 16927), false, false); // Set Book Filter
            this.AddButton(40, 416, 4005, 4007, 1, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(235, 416, 120, 32, 1062478, (from.UseOwnFilter ? 16927 : LabelColor), false, false); // Set Your Filter
            this.AddButton(200, 416, 4005, 4007, 2, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(405, 416, 120, 32, 1062231, LabelColor, false, false); // Clear Filter
            this.AddButton(370, 416, 4005, 4007, 3, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(540, 416, 50, 32, 1011046, LabelColor, false, false); // APPLY
            this.AddButton(505, 416, 4017, 4018, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            BOBFilter f = (this.m_From.UseOwnFilter ? this.m_From.BOBFilter : this.m_Book.Filter);

            int index = info.ButtonID;

            switch ( index )
            {
                case 0: // Apply
                    {
                        this.m_From.SendGump(new BOBGump(this.m_From, this.m_Book));

                        break;
                    }
                case 1: // Set Book Filter
                    {
                        this.m_From.UseOwnFilter = false;
                        this.m_From.SendGump(new BOBFilterGump(this.m_From, this.m_Book));

                        break;
                    }
                case 2: // Set Your Filter
                    {
                        this.m_From.UseOwnFilter = true;
                        this.m_From.SendGump(new BOBFilterGump(this.m_From, this.m_Book));

                        break;
                    }
                case 3: // Clear Filter
                    {
                        f.Clear();
                        this.m_From.SendGump(new BOBFilterGump(this.m_From, this.m_Book));

                        break;
                    }
                default:
                    {
                        index -= 4;

                        int type = index % 4;
                        index /= 4;

                        if (type >= 0 && type < m_Filters.Length)
                        {
                            int[,] filters = m_Filters[type];

                            if (index >= 0 && index < filters.GetLength(0))
                            {
                                if (filters[index, 0] == 0)
                                    break;

                                switch ( type )
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

                                this.m_From.SendGump(new BOBFilterGump(this.m_From, this.m_Book));
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

                this.AddHtmlLocalized(x + 35 + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), xWidths[i % xOffsets.Length], 32, number, isSelected ? 16927 : LabelColor, false, false);
                this.AddButton(x + xOffsets[i % xOffsets.Length], y + ((i / xOffsets.Length) * yOffset), 4005, 4007, 4 + filterIndex + (i * 4), GumpButtonType.Reply, 0);
            }
        }
    }
}