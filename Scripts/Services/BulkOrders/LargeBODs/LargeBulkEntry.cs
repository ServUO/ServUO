using System;
using System.Collections;

namespace Server.Engines.BulkOrders
{
    public class LargeBulkEntry
    {
        private static Hashtable m_Cache;
        private readonly SmallBulkEntry m_Details;
        private LargeBOD m_Owner;
        private int m_Amount;

        public LargeBulkEntry(LargeBOD owner, SmallBulkEntry details)
        {
            m_Owner = owner;
            m_Details = details;
        }

        public static SmallBulkEntry[] LargeRing => GetEntries("Blacksmith", "largering");
        public static SmallBulkEntry[] LargePlate => GetEntries("Blacksmith", "largeplate");
        public static SmallBulkEntry[] LargeChain => GetEntries("Blacksmith", "largechain");
        public static SmallBulkEntry[] LargeAxes => GetEntries("Blacksmith", "largeaxes");
        public static SmallBulkEntry[] LargeFencing => GetEntries("Blacksmith", "largefencing");
        public static SmallBulkEntry[] LargeMaces => GetEntries("Blacksmith", "largemaces");
        public static SmallBulkEntry[] LargePolearms => GetEntries("Blacksmith", "largepolearms");
        public static SmallBulkEntry[] LargeSwords => GetEntries("Blacksmith", "largeswords");
        public static SmallBulkEntry[] BoneSet => GetEntries("Tailoring", "boneset");
        public static SmallBulkEntry[] Farmer => GetEntries("Tailoring", "farmer");
        public static SmallBulkEntry[] FemaleLeatherSet => GetEntries("Tailoring", "femaleleatherset");
        public static SmallBulkEntry[] FisherGirl => GetEntries("Tailoring", "fishergirl");
        public static SmallBulkEntry[] Gypsy => GetEntries("Tailoring", "gypsy");
        public static SmallBulkEntry[] HatSet => GetEntries("Tailoring", "hatset");
        public static SmallBulkEntry[] Jester => GetEntries("Tailoring", "jester");
        public static SmallBulkEntry[] Lady => GetEntries("Tailoring", "lady");
        public static SmallBulkEntry[] MaleLeatherSet => GetEntries("Tailoring", "maleleatherset");
        public static SmallBulkEntry[] Pirate => GetEntries("Tailoring", "pirate");
        public static SmallBulkEntry[] ShoeSet => GetEntries("Tailoring", "shoeset");
        public static SmallBulkEntry[] StuddedSet => GetEntries("Tailoring", "studdedset");
        public static SmallBulkEntry[] TownCrier => GetEntries("Tailoring", "towncrier");
        public static SmallBulkEntry[] Wizard => GetEntries("Tailoring", "wizard");
        #region Publics 95 BODs
        public static SmallBulkEntry[] LargeCircle1 => GetEntries("Inscription", "LargeCircle1");
        public static SmallBulkEntry[] LargeCircle1and2 => GetEntries("Inscription", "LargeCircle1and2");
        public static SmallBulkEntry[] LargeNecromancy1 => GetEntries("Inscription", "LargeNecromancy1");
        public static SmallBulkEntry[] LargeNecromancy2 => GetEntries("Inscription", "LargeNecromancy2");
        public static SmallBulkEntry[] LargeNecromancy3 => GetEntries("Inscription", "LargeNecromancy3");
        public static SmallBulkEntry[] LargeCircle4 => GetEntries("Inscription", "LargeCircle4");
        public static SmallBulkEntry[] LargeCircle5 => GetEntries("Inscription", "LargeCircle5");
        public static SmallBulkEntry[] LargeCircle7 => GetEntries("Inscription", "LargeCircle7");
        public static SmallBulkEntry[] LargeCircle8 => GetEntries("Inscription", "LargeCircle8");
        public static SmallBulkEntry[] LargeBooks => GetEntries("Inscription", "LargeBooks");

        public static SmallBulkEntry[] LargeWeapons => GetEntries("Carpentry", "LargeWeapons");
        public static SmallBulkEntry[] LargeWoodFurniture => GetEntries("Carpentry", "LargeWoodFurniture");
        public static SmallBulkEntry[] LargeCabinets => GetEntries("Carpentry", "LargeCabinets");
        public static SmallBulkEntry[] LargeArmoire => GetEntries("Carpentry", "LargeArmoire");
        public static SmallBulkEntry[] LargeInstruments => GetEntries("Carpentry", "LargeInstruments");
        public static SmallBulkEntry[] LargeChests => GetEntries("Carpentry", "LargeChests");
        public static SmallBulkEntry[] LargeElvenWeapons => GetEntries("Carpentry", "LargeElvenWeapons");

        public static SmallBulkEntry[] LargeAmmunition => GetEntries("Fletching", "LargeAmmunition");
        public static SmallBulkEntry[] LargeHumanBows1 => GetEntries("Fletching", "LargeHumanBows1");
        public static SmallBulkEntry[] LargeHumanBows2 => GetEntries("Fletching", "LargeHumanBows2");
        public static SmallBulkEntry[] LargeElvenBows1 => GetEntries("Fletching", "LargeElvenBows1");
        public static SmallBulkEntry[] LargeElvenBows2 => GetEntries("Fletching", "LargeElvenBows2");

        public static SmallBulkEntry[] LargeExplosive => GetEntries("Alchemy", "largeexplosive");
        public static SmallBulkEntry[] LargeGreater => GetEntries("Alchemy", "largegreater");
        public static SmallBulkEntry[] LargeLesser => GetEntries("Alchemy", "largelesser");
        public static SmallBulkEntry[] LargeRegular => GetEntries("Alchemy", "largeregular");
        public static SmallBulkEntry[] LargeToxic => GetEntries("Alchemy", "largetoxic");
        public static SmallBulkEntry[] LargeKeyGlobe => GetEntries("Tinkering", "LargeKeyGlobe");
        public static SmallBulkEntry[] LargeTools => GetEntries("Tinkering", "LargeTools");
        public static SmallBulkEntry[] LargeJewelry => GetEntries("Tinkering", "LargeJewelry");
        public static SmallBulkEntry[] LargeDining => GetEntries("Tinkering", "LargeDining");
        public static SmallBulkEntry[] LargeBarbeque => GetEntries("Cooking", "LargeBarbeque");
        public static SmallBulkEntry[] LargeDough => GetEntries("Cooking", "LargeDough");
        public static SmallBulkEntry[] LargeFruits => GetEntries("Cooking", "LargeFruits");
        public static SmallBulkEntry[] LargeMiso => GetEntries("Cooking", "LargeMiso");
        public static SmallBulkEntry[] LargeSushi => GetEntries("Cooking", "LargeSushi");
        public static SmallBulkEntry[] LargeSweets => GetEntries("Cooking", "LargeSweets");
        public static SmallBulkEntry[] LargeUnbakedPies => GetEntries("Cooking", "LargeUnbakedPies");
        #endregion
        public LargeBOD Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
        public int Amount
        {
            get
            {
                return m_Amount;
            }
            set
            {
                m_Amount = value;
                if (m_Owner != null)
                    m_Owner.InvalidateProperties();
            }
        }
        public SmallBulkEntry Details => m_Details;
        public static SmallBulkEntry[] GetEntries(string type, string name)
        {
            if (m_Cache == null)
                m_Cache = new Hashtable();

            Hashtable table = (Hashtable)m_Cache[type];

            if (table == null)
                m_Cache[type] = table = new Hashtable();

            SmallBulkEntry[] entries = (SmallBulkEntry[])table[name];

            if (entries == null)
                table[name] = entries = SmallBulkEntry.LoadEntries(type, name);

            return entries;
        }

        public static LargeBulkEntry[] ConvertEntries(LargeBOD owner, SmallBulkEntry[] small)
        {
            LargeBulkEntry[] large = new LargeBulkEntry[small.Length];

            for (int i = 0; i < small.Length; ++i)
                large[i] = new LargeBulkEntry(owner, small[i]);

            return large;
        }

        public LargeBulkEntry(LargeBOD owner, GenericReader reader, int version)
        {
            m_Owner = owner;
            m_Amount = reader.ReadInt();

            Type realType = null;

            string type = reader.ReadString();

            if (type != null)
                realType = ScriptCompiler.FindTypeByFullName(type);

            m_Details = new SmallBulkEntry(realType, reader.ReadInt(), reader.ReadInt(), version == 0 ? 0 : reader.ReadInt());
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_Amount);
            writer.Write(m_Details.Type == null ? null : m_Details.Type.FullName);
            writer.Write(m_Details.Number);
            writer.Write(m_Details.Graphic);
            writer.Write(m_Details.Hue);
        }
    }
}
