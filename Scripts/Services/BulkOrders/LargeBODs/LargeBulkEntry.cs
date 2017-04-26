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
            this.m_Owner = owner;
            this.m_Details = details;
        }

        public static SmallBulkEntry[] LargeRing
        {
            get
            {
                return GetEntries("Blacksmith", "largering");
            }
        }
        public static SmallBulkEntry[] LargePlate
        {
            get
            {
                return GetEntries("Blacksmith", "largeplate");
            }
        }
        public static SmallBulkEntry[] LargeChain
        {
            get
            {
                return GetEntries("Blacksmith", "largechain");
            }
        }
        public static SmallBulkEntry[] LargeAxes
        {
            get
            {
                return GetEntries("Blacksmith", "largeaxes");
            }
        }
        public static SmallBulkEntry[] LargeFencing
        {
            get
            {
                return GetEntries("Blacksmith", "largefencing");
            }
        }
        public static SmallBulkEntry[] LargeMaces
        {
            get
            {
                return GetEntries("Blacksmith", "largemaces");
            }
        }
        public static SmallBulkEntry[] LargePolearms
        {
            get
            {
                return GetEntries("Blacksmith", "largepolearms");
            }
        }
        public static SmallBulkEntry[] LargeSwords
        {
            get
            {
                return GetEntries("Blacksmith", "largeswords");
            }
        }
        public static SmallBulkEntry[] BoneSet
        {
            get
            {
                return GetEntries("Tailoring", "boneset");
            }
        }
        public static SmallBulkEntry[] Farmer
        {
            get
            {
                return GetEntries("Tailoring", "farmer");
            }
        }
        public static SmallBulkEntry[] FemaleLeatherSet
        {
            get
            {
                return GetEntries("Tailoring", "femaleleatherset");
            }
        }
        public static SmallBulkEntry[] FisherGirl
        {
            get
            {
                return GetEntries("Tailoring", "fishergirl");
            }
        }
        public static SmallBulkEntry[] Gypsy
        {
            get
            {
                return GetEntries("Tailoring", "gypsy");
            }
        }
        public static SmallBulkEntry[] HatSet
        {
            get
            {
                return GetEntries("Tailoring", "hatset");
            }
        }
        public static SmallBulkEntry[] Jester
        {
            get
            {
                return GetEntries("Tailoring", "jester");
            }
        }
        public static SmallBulkEntry[] Lady
        {
            get
            {
                return GetEntries("Tailoring", "lady");
            }
        }
        public static SmallBulkEntry[] MaleLeatherSet
        {
            get
            {
                return GetEntries("Tailoring", "maleleatherset");
            }
        }
        public static SmallBulkEntry[] Pirate
        {
            get
            {
                return GetEntries("Tailoring", "pirate");
            }
        }
        public static SmallBulkEntry[] ShoeSet
        {
            get
            {
                return GetEntries("Tailoring", "shoeset");
            }
        }
        public static SmallBulkEntry[] StuddedSet
        {
            get
            {
                return GetEntries("Tailoring", "studdedset");
            }
        }
        public static SmallBulkEntry[] TownCrier
        {
            get
            {
                return GetEntries("Tailoring", "towncrier");
            }
        }
        public static SmallBulkEntry[] Wizard
        {
            get
            {
                return GetEntries("Tailoring", "wizard");
            }
        }
        #region Publics 95 BODs
        public static SmallBulkEntry[] LargeCircle1
        {
            get
            {
                return GetEntries("Inscription", "LargeCircle1");
            }
        }
        public static SmallBulkEntry[] LargeCircle1and2
        {
            get
            {
                return GetEntries("Inscription", "LargeCircle1and2");
            }
        }
        public static SmallBulkEntry[] LargeNecromancy1
        {
            get
            {
                return GetEntries("Inscription", "LargeNecromancy1");
            }
        }
        public static SmallBulkEntry[] LargeNecromancy2
        {
            get
            {
                return GetEntries("Inscription", "LargeNecromancy2");
            }
        }
        public static SmallBulkEntry[] LargeNecromancy3
        {
            get
            {
                return GetEntries("Inscription", "LargeNecromancy3");
            }
        }
        public static SmallBulkEntry[] LargeCircle4
        {
            get
            {
                return GetEntries("Inscription", "LargeCircle4");
            }
        }
        public static SmallBulkEntry[] LargeCircle5
        {
            get
            {
                return GetEntries("Inscription", "LargeCircle5");
            }
        }
        public static SmallBulkEntry[] LargeCircle7
        {
            get
            {
                return GetEntries("Inscription", "LargeCircle7");
            }
        }
        public static SmallBulkEntry[] LargeCircle8
        {
            get
            {
                return GetEntries("Inscription", "LargeCircle8");
            }
        }
        public static SmallBulkEntry[] LargeBooks
        {
            get
            {
                return GetEntries("Inscription", "LargeBooks");
            }
        }
       
        public static SmallBulkEntry[] LargeWeapons
        {
            get
            {
                return GetEntries("Carpentry", "LargeWeapons");
            }
        }
        public static SmallBulkEntry[] LargeWoodFurniture
        {
            get
            {
                return GetEntries("Carpentry", "LargeWoodFurniture");
            }
        }
        public static SmallBulkEntry[] LargeCabinets
        {
            get
            {
                return GetEntries("Carpentry", "LargeCabinets");
            }
        }
        public static SmallBulkEntry[] LargeArmoire
        {
            get
            {
                return GetEntries("Carpentry", "LargeArmoire");
            }
        }
        public static SmallBulkEntry[] LargeInstruments
        {
            get
            {
                return GetEntries("Carpentry", "LargeInstruments");
            }
        }
        public static SmallBulkEntry[] LargeChests
        {
            get
            {
                return GetEntries("Carpentry", "LargeChests");
            }
        }
        public static SmallBulkEntry[] LargeElvenWeapons
        {
            get
            {
                return GetEntries("Carpentry", "LargeElvenWeapons");
            }
        }

        public static SmallBulkEntry[] LargeAmmunition
        {
            get
            {
                return GetEntries("Fletching", "LargeAmmunition");
            }
        }
        public static SmallBulkEntry[] LargeHumanBows1
        {
            get
            {
                return GetEntries("Fletching", "LargeHumanBows1");
            }
        }
        public static SmallBulkEntry[] LargeHumanBows2
        {
            get
            {
                return GetEntries("Fletching", "LargeHumanBows2");
            }
        }
        public static SmallBulkEntry[] LargeElvenBows1
        {
            get
            {
                return GetEntries("Fletching", "LargeElvenBows1");
            }
        }
        public static SmallBulkEntry[] LargeElvenBows2
        {
            get
            {
                return GetEntries("Fletching", "LargeElvenBows2");
            }
        }

        public static SmallBulkEntry[] LargeExplosive
        {
            get
            {
                return GetEntries("Alchemy", "largeexplosive");
            }
        }
        public static SmallBulkEntry[] LargeGreater
        {
            get
            {
                return GetEntries("Alchemy", "largegreater");
            }
        }
        public static SmallBulkEntry[] LargeLesser
        {
            get
            {
                return GetEntries("Alchemy", "largelesser");
            }
        }
        public static SmallBulkEntry[] LargeRegular
        {
            get
            {
                return GetEntries("Alchemy", "largeregular");
            }
        }
        public static SmallBulkEntry[] LargeToxic
        {
            get
            {
                return GetEntries("Alchemy", "largetoxic");
            }
        }
        public static SmallBulkEntry[] LargeKeyGlobe
        {
            get
            {
                return GetEntries("Tinkering", "LargeKeyGlobe");
            }
        }
        public static SmallBulkEntry[] LargeTools
        {
            get
            {
                return GetEntries("Tinkering", "LargeTools");
            }
        }
        public static SmallBulkEntry[] LargeJewelry
        {
            get
            {
                return GetEntries("Tinkering", "LargeJewelry");
            }
        }
        public static SmallBulkEntry[] LargeDining
        {
            get
            {
                return GetEntries("Tinkering", "LargeDining");
            }
        }
        public static SmallBulkEntry[] LargeBarbeque
        {
            get
            {
                return GetEntries("Cooking", "LargeBarbeque");
            }
        }
        public static SmallBulkEntry[] LargeDough
        {
            get
            {
                return GetEntries("Cooking", "LargeDough");
            }
        }
        public static SmallBulkEntry[] LargeFruits
        {
            get
            {
                return GetEntries("Cooking", "LargeFruits");
            }
        }
        public static SmallBulkEntry[] LargeMiso
        {
            get
            {
                return GetEntries("Cooking", "LargeMiso");
            }
        }
        public static SmallBulkEntry[] LargeSushi
        {
            get
            {
                return GetEntries("Cooking", "LargeSushi");
            }
        }
        public static SmallBulkEntry[] LargeSweets
        {
            get
            {
                return GetEntries("Cooking", "LargeSweets");
            }
        }
        public static SmallBulkEntry[] LargeUnbakedPies
        {
            get
            {
                return GetEntries("Cooking", "LargeUnbakedPies");
            }
        }
        #endregion
        public LargeBOD Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        public int Amount
        {
            get
            {
                return this.m_Amount;
            }
            set
            {
                this.m_Amount = value;
                if (this.m_Owner != null)
                    this.m_Owner.InvalidateProperties();
            }
        }
        public SmallBulkEntry Details
        {
            get
            {
                return this.m_Details;
            }
        }
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
            this.m_Owner = owner;
            this.m_Amount = reader.ReadInt();

            Type realType = null;

            string type = reader.ReadString();

            if (type != null)
                realType = ScriptCompiler.FindTypeByFullName(type);

            this.m_Details = new SmallBulkEntry(realType, reader.ReadInt(), reader.ReadInt(), version == 0 ? 0 : reader.ReadInt());
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(this.m_Amount);
            writer.Write(this.m_Details.Type == null ? null : this.m_Details.Type.FullName);
            writer.Write(this.m_Details.Number);
            writer.Write(this.m_Details.Graphic);
            writer.Write(this.m_Details.Hue);
        }
    }
}
