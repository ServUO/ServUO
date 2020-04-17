using Server.Engines.Quests;

namespace Server.Items
{
    public enum SutekIngredient
    {
        Feathers = 1023578,
        PowerCrystal = 1112811,
        DarkStone = 1112866,
        WhiteStone = 1112813,
        BrownStone = 1112814,
        IronWire = 1026262,
        SilverWire = 1026263,
        GoldWire = 1026264,
        CopperWire = 1026265,
        Leather = 1112812,
        Thorns = 1112818,
        GoldIngots = 1027146,
        CopperIngots = 1027140,
        IronIngots = 1027152,
        SilverIngots = 1027158,
        YellowPotion = 1023852,
        PurplePotion = 1023853,
        RedPotion = 1023851,
        BluePotion = 1023848,
        Scales = 1113463,
        Bones = 1023786,
        Shafts = 1027125,
        Gears = 1011200,
        BarrelHoops = 1011228,
        BarrelStaves = 1027857,
        SpiritEssence = 1055029,
        VoidEssence = 1112327,
        FetidEssence = 1031066,
        WoodenLogs = 1021217,
        Rope = 1020934,
        Beeswax = 1025154,
        WoodenBoards = 1021189,
        MeltedWax = 1016492,
        OilOfVitriol = 1077482,
        BlackPowder = 1112815,
        WhitePowder = 1112816,
        BluePowder = 1112817,
        Nails = 1024142
    }

    public class SutekIngredientInfo
    {
        private readonly SutekIngredient m_Ingredient;
        private readonly int m_ItemId, m_TextId, m_Hue;
        private Point3D m_Location;

        public SutekIngredient Ingredient => m_Ingredient;
        public int ItemId => m_ItemId;
        public int TextId => m_TextId;
        public int Hue => m_Hue;
        public Point3D Location => m_Location;

        public SutekIngredientInfo(SutekIngredient ingredient, Point3D location, int itemId, int textId)
            : this(ingredient, location, itemId, textId, 0)
        {
        }

        public SutekIngredientInfo(SutekIngredient ingredient, Point3D location, int itemId, int textId, int hue)
        {
            m_Ingredient = ingredient;
            m_Location = location;
            m_ItemId = itemId;
            m_TextId = textId;
            m_Hue = hue;
        }
    }

    public class SutekIngredientItem : Item
    {
        public override int LabelNumber => m_TextId;
        public override bool ForceShowProperties => true;

        private SutekIngredient m_Ingredient;
        private int m_TextId;

        [Constructable]
        public SutekIngredientItem(SutekIngredientInfo info)
            : base(info.ItemId)
        {
            Weight = 0.0;
            Movable = false;

            Hue = info.Hue;
            m_TextId = info.TextId;
            m_Ingredient = info.Ingredient;

            MoveToWorld(info.Location, Map.TerMur);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this, 2))
                MadScientistQuest.OnDoubleClickIngredient(from, m_Ingredient);
        }

        public SutekIngredientItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)m_Ingredient);
            writer.Write(m_TextId);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Ingredient = (SutekIngredient)reader.ReadInt();
            m_TextId = reader.ReadInt();
        }
    }
}