using System;

namespace Server.Items
{
    public enum PresetMapType
    {
        Britain,
        BritainToSkaraBrae,
        BritainToTrinsic,
        BucsDen,
        BucsDenToMagincia,
        BucsDenToOcllo,
        Jhelom,
        Magincia,
        MaginciaToOcllo,
        Minoc,
        MinocToYew,
        MinocToVesper,
        Moonglow,
        MoonglowToNujelm,
        Nujelm,
        NujelmToMagincia,
        Ocllo,
        SerpentsHold,
        SerpentsHoldToOcllo,
        SkaraBrae,
        TheWorld,
        Trinsic,
        TrinsicToBucsDen,
        TrinsicToJhelom,
        Vesper,
        VesperToNujelm,
        Yew,
        YewToBritain
    }

    public class PresetMap : MapItem
    {
        private int m_LabelNumber;
        [Constructable]
        public PresetMap(PresetMapType type)
        {
            int v = (int)type;

            if (v >= 0 && v < PresetMapEntry.Table.Length)
                this.InitEntry(PresetMapEntry.Table[v]);
        }

        public PresetMap(PresetMapEntry entry)
        {
            this.InitEntry(entry);
        }

        public PresetMap(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return (this.m_LabelNumber == 0 ? base.LabelNumber : this.m_LabelNumber);
            }
        }
        public void InitEntry(PresetMapEntry entry)
        {
            this.m_LabelNumber = entry.Name;

            this.Width = entry.Width;
            this.Height = entry.Height;

            this.Bounds = entry.Bounds;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)this.m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_LabelNumber = reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class PresetMapEntry
    {
        private static readonly PresetMapEntry[] m_Table = new PresetMapEntry[]
        {
            new PresetMapEntry(1041189, 200, 200, 1092, 1396, 1736, 1924), // map of Britain
            new PresetMapEntry(1041203, 200, 200, 0256, 1792, 1736, 2560), // map of Britain to Skara Brae
            new PresetMapEntry(1041192, 200, 200, 1024, 1280, 2304, 3072), // map of Britain to Trinsic
            new PresetMapEntry(1041183, 200, 200, 2500, 1900, 3000, 2400), // map of Buccaneer's Den
            new PresetMapEntry(1041198, 200, 200, 2560, 1792, 3840, 2560), // map of Buccaneer's Den to Magincia
            new PresetMapEntry(1041194, 200, 200, 2560, 1792, 3840, 3072), // map of Buccaneer's Den to Ocllo
            new PresetMapEntry(1041181, 200, 200, 1088, 3572, 1528, 4056), // map of Jhelom
            new PresetMapEntry(1041186, 200, 200, 3530, 2022, 3818, 2298), // map of Magincia
            new PresetMapEntry(1041199, 200, 200, 3328, 1792, 3840, 2304), // map of Magincia to Ocllo
            new PresetMapEntry(1041182, 200, 200, 2360, 0356, 2706, 0702), // map of Minoc
            new PresetMapEntry(1041190, 200, 200, 0000, 0256, 2304, 3072), // map of Minoc to Yew
            new PresetMapEntry(1041191, 200, 200, 2467, 0572, 2878, 0746), // map of Minoc to Vesper
            new PresetMapEntry(1041188, 200, 200, 4156, 0808, 4732, 1528), // map of Moonglow
            new PresetMapEntry(1041201, 200, 200, 3328, 0768, 4864, 1536), // map of Moonglow to Nujelm
            new PresetMapEntry(1041185, 200, 200, 3446, 1030, 3832, 1424), // map of Nujelm
            new PresetMapEntry(1041197, 200, 200, 3328, 1024, 3840, 2304), // map of Nujelm to Magincia
            new PresetMapEntry(1041187, 200, 200, 3582, 2456, 3770, 2742), // map of Ocllo
            new PresetMapEntry(1041184, 200, 200, 2714, 3329, 3100, 3639), // map of Serpent's Hold
            new PresetMapEntry(1041200, 200, 200, 2560, 2560, 3840, 3840), // map of Serpent's Hold to Ocllo
            new PresetMapEntry(1041180, 200, 200, 0524, 2064, 0960, 2452), // map of Skara Brae
            new PresetMapEntry(1041204, 200, 200, 0000, 0000, 5199, 4095), // map of The World
            new PresetMapEntry(1041177, 200, 200, 1792, 2630, 2118, 2952), // map of Trinsic
            new PresetMapEntry(1041193, 200, 200, 1792, 1792, 3072, 3072), // map of Trinsic to Buccaneer's Den
            new PresetMapEntry(1041195, 200, 200, 0256, 1792, 2304, 4095), // map of Trinsic to Jhelom
            new PresetMapEntry(1041178, 200, 200, 2636, 0592, 3064, 1012), // map of Vesper
            new PresetMapEntry(1041196, 200, 200, 2636, 0592, 3840, 1536), // map of Vesper to Nujelm
            new PresetMapEntry(1041179, 200, 200, 0236, 0741, 0766, 1269), // map of Yew
            new PresetMapEntry(1041202, 200, 200, 0000, 0512, 1792, 2048)// map of Yew to Britain
        };
        private readonly int m_Name;
        private readonly int m_Width;
        private readonly int m_Height;
        private readonly Rectangle2D m_Bounds;
        public PresetMapEntry(int name, int width, int height, int xLeft, int yTop, int xRight, int yBottom)
        {
            this.m_Name = name;
            this.m_Width = width;
            this.m_Height = height;
            this.m_Bounds = new Rectangle2D(xLeft, yTop, xRight - xLeft, yBottom - yTop);
        }

        public static PresetMapEntry[] Table
        {
            get
            {
                return m_Table;
            }
        }
        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public int Width
        {
            get
            {
                return this.m_Width;
            }
        }
        public int Height
        {
            get
            {
                return this.m_Height;
            }
        }
        public Rectangle2D Bounds
        {
            get
            {
                return this.m_Bounds;
            }
        }
    }
}