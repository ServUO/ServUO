using System;

namespace Server.Factions
{
    public class VendorDefinition
    {
        private static readonly VendorDefinition[] m_Definitions = new VendorDefinition[]
        {
            new VendorDefinition(typeof(FactionBottleVendor), 0xF0E,
                5000,
                1000,
                10,
                new TextDefinition(1011549, "POTION BOTTLE VENDOR"),
                new TextDefinition(1011544, "Buy Potion Bottle Vendor")),
            new VendorDefinition(typeof(FactionBoardVendor), 0x1BD7,
                3000,
                500,
                10,
                new TextDefinition(1011552, "WOOD VENDOR"),
                new TextDefinition(1011545, "Buy Wooden Board Vendor")),
            new VendorDefinition(typeof(FactionOreVendor), 0x19B8,
                3000,
                500,
                10,
                new TextDefinition(1011553, "IRON ORE VENDOR"),
                new TextDefinition(1011546, "Buy Iron Ore Vendor")),
            new VendorDefinition(typeof(FactionReagentVendor), 0xF86,
                5000,
                1000,
                10,
                new TextDefinition(1011554, "REAGENT VENDOR"),
                new TextDefinition(1011547, "Buy Reagent Vendor")),
            new VendorDefinition(typeof(FactionHorseVendor), 0x20DD,
                5000,
                1000,
                1,
                new TextDefinition(1011556, "HORSE BREEDER"),
                new TextDefinition(1011555, "Buy Horse Breeder"))
        };
        private readonly Type m_Type;
        private readonly int m_Price;
        private readonly int m_Upkeep;
        private readonly int m_Maximum;
        private readonly int m_ItemID;
        private readonly TextDefinition m_Header;
        private readonly TextDefinition m_Label;
        public VendorDefinition(Type type, int itemID, int price, int upkeep, int maximum, TextDefinition header, TextDefinition label)
        {
            this.m_Type = type;

            this.m_Price = price;
            this.m_Upkeep = upkeep;
            this.m_Maximum = maximum;
            this.m_ItemID = itemID;

            this.m_Header = header;
            this.m_Label = label;
        }

        public static VendorDefinition[] Definitions
        {
            get
            {
                return m_Definitions;
            }
        }
        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public int Price
        {
            get
            {
                return this.m_Price;
            }
        }
        public int Upkeep
        {
            get
            {
                return this.m_Upkeep;
            }
        }
        public int Maximum
        {
            get
            {
                return this.m_Maximum;
            }
        }
        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
        }
        public TextDefinition Header
        {
            get
            {
                return this.m_Header;
            }
        }
        public TextDefinition Label
        {
            get
            {
                return this.m_Label;
            }
        }
    }
}