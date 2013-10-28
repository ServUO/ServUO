using System;
using System.Collections;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector
{
    public enum ImageType
    {
        Betrayer,
        Bogling,
        BogThing,
        Gazer,
        Beetle,
        GiantBlackWidow,
        Scorpion,
        JukaMage,
        JukaWarrior,
        Lich,
        MeerMage,
        MeerWarrior,
        Mongbat,
        Mummy,
        Pixie,
        PlagueBeast,
        SandVortex,
        StoneGargoyle,
        SwampDragon,
        Wisp,
        Juggernaut
    }

    public class ImageTypeInfo
    {
        private static readonly ImageTypeInfo[] m_Table = new ImageTypeInfo[]
        {
            new ImageTypeInfo(9734, typeof(Betrayer), 75, 45),
            new ImageTypeInfo(9735, typeof(Bogling), 75, 45),
            new ImageTypeInfo(9736, typeof(BogThing), 60, 47),
            new ImageTypeInfo(9615, typeof(Gazer), 75, 45),
            new ImageTypeInfo(9743, typeof(Beetle), 60, 55),
            new ImageTypeInfo(9667, typeof(GiantBlackWidow), 55, 52),
            new ImageTypeInfo(9657, typeof(Scorpion), 65, 47),
            new ImageTypeInfo(9758, typeof(JukaMage), 75, 45),
            new ImageTypeInfo(9759, typeof(JukaWarrior), 75, 45),
            new ImageTypeInfo(9636, typeof(Lich), 75, 45),
            new ImageTypeInfo(9756, typeof(MeerMage), 75, 45),
            new ImageTypeInfo(9757, typeof(MeerWarrior), 75, 45),
            new ImageTypeInfo(9638, typeof(Mongbat), 70, 50),
            new ImageTypeInfo(9639, typeof(Mummy), 75, 45),
            new ImageTypeInfo(9654, typeof(Pixie), 75, 45),
            new ImageTypeInfo(9747, typeof(PlagueBeast), 60, 45),
            new ImageTypeInfo(9750, typeof(SandVortex), 60, 43),
            new ImageTypeInfo(9614, typeof(StoneGargoyle), 75, 45),
            new ImageTypeInfo(9753, typeof(SwampDragon), 50, 55),
            new ImageTypeInfo(8448, typeof(Wisp), 75, 45),
            new ImageTypeInfo(9746, typeof(Juggernaut), 55, 38)
        };
        private readonly int m_Figurine;
        private readonly Type m_Type;
        private readonly int m_X;
        private readonly int m_Y;
        public ImageTypeInfo(int figurine, Type type, int x, int y)
        {
            this.m_Figurine = figurine;
            this.m_Type = type;
            this.m_X = x;
            this.m_Y = y;
        }

        public int Figurine
        {
            get
            {
                return this.m_Figurine;
            }
        }
        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public int Name
        {
            get
            {
                return this.m_Figurine < 0x4000 ? 1020000 + this.m_Figurine : 1078872 + this.m_Figurine;
            }
        }
        public int X
        {
            get
            {
                return this.m_X;
            }
        }
        public int Y
        {
            get
            {
                return this.m_Y;
            }
        }
        public static ImageTypeInfo Get(ImageType image)
        {
            int index = (int)image;
            if (index >= 0 && index < m_Table.Length)
                return m_Table[index];
            else
                return m_Table[0];
        }

        public static ImageType[] RandomList(int count)
        {
            ArrayList list = new ArrayList(m_Table.Length);
            for (int i = 0; i < m_Table.Length; i++)
                list.Add((ImageType)i);

            ImageType[] images = new ImageType[count];

            for (int i = 0; i < images.Length; i++)
            {
                int index = Utility.Random(list.Count);
                images[i] = (ImageType)list[index];

                list.RemoveAt(index);
            }

            return images;
        }
    }
}