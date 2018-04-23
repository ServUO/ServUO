using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
	public class Avatar
	{
        private static Hashtable s_Avatars = new Hashtable();

        public static Hashtable Avatars { get { return s_Avatars; } }
        public static ArrayList AvaKeys { get { return new ArrayList(s_Avatars.Keys); } }

        public static void Initialize()
        {
            new Avatar(0x69, 8, 8);
            new Avatar(0x6A, 8, 8);
            new Avatar(0x6B, 8, 8);
            new Avatar(0x6C, 8, 8);
            new Avatar(0x6D, 8, 8);
            new Avatar(0x6E, 8, 8);
            new Avatar(0x6F, 8, 8);
            new Avatar(0x70, 8, 8);
            new Avatar(0x71, 25, 25);
            new Avatar(0x8BA, 20, 17);
            new Avatar(0x8C0, 18, 18);
            new Avatar(0x8C1, 18, 18);
            new Avatar(0x8C2, 18, 18);
            new Avatar(0x8C3, 18, 18);
            new Avatar(0x8C4, 18, 18);
            new Avatar(0x8C5, 18, 18);
            new Avatar(0x8C6, 18, 18);
            new Avatar(0x8C7, 18, 18);
            new Avatar(0x8C8, 18, 18);
            new Avatar(0x8C9, 18, 18);
            new Avatar(0x8CA, 18, 18);
            new Avatar(0x8CB, 18, 18);
            new Avatar(0x8CC, 18, 18);
            new Avatar(0x8CD, 18, 18);
            new Avatar(0x8CE, 18, 18);
            new Avatar(0x8CF, 18, 18);
            new Avatar(0x8D0, 18, 18);
            new Avatar(0x8D1, 18, 18);
            new Avatar(0x8D2, 18, 18);
            new Avatar(0x8D3, 18, 18);
            new Avatar(0x8D4, 18, 18);
            new Avatar(0x8D5, 18, 18);
            new Avatar(0x8D6, 18, 18);
            new Avatar(0x8D7, 18, 18);
            new Avatar(0x8D8, 18, 18);
            new Avatar(0x8D9, 18, 18);
            new Avatar(0x8DA, 18, 18);
            new Avatar(0x8DB, 18, 18);
            new Avatar(0x8DC, 18, 18);
            new Avatar(0x8DD, 18, 18);
            new Avatar(0x8DE, 18, 18);
            new Avatar(0x8DF, 18, 18);
            new Avatar(0x8E0, 18, 18);
            new Avatar(0x8E1, 18, 18);
            new Avatar(0x8E2, 18, 18);
            new Avatar(0x8E3, 18, 18);
            new Avatar(0x8E4, 18, 18);
            new Avatar(0x8E5, 18, 18);
            new Avatar(0x8E6, 18, 18);
            new Avatar(0x8E7, 18, 18);
            new Avatar(0x8E8, 18, 18);
            new Avatar(0x8E9, 18, 18);
            new Avatar(0x8EA, 18, 18);
            new Avatar(0x8EB, 18, 18);
            new Avatar(0x8EC, 18, 18);
            new Avatar(0x8ED, 18, 18);
            new Avatar(0x8EE, 18, 18);
            new Avatar(0x8EF, 18, 18);
            new Avatar(0x8F0, 18, 18);
            new Avatar(0x8F1, 18, 18);
            new Avatar(0x8F2, 18, 18);
            new Avatar(0x8F3, 18, 18);
            new Avatar(0x8F4, 18, 18);
            new Avatar(0x8F5, 18, 18);
            new Avatar(0x8F6, 18, 18);
            new Avatar(0x8F7, 18, 18);
            new Avatar(0x8F8, 18, 18);
            new Avatar(0x8F9, 18, 18);
            new Avatar(0x8FA, 18, 18);
            new Avatar(0x8FB, 18, 18);
            new Avatar(0x8FC, 18, 18);
            new Avatar(0x8FD, 18, 18);
            new Avatar(0x8FE, 18, 18);
            new Avatar(0x8FF, 18, 18);
            new Avatar(0x139D, 18, 16);
            new Avatar(0x15A9, 10, 10);
            new Avatar(0x15AB, 10, 10);
            new Avatar(0x15AD, 10, 10);
            new Avatar(0x15AF, 10, 10);
            new Avatar(0x15B1, 10, 10);
            new Avatar(0x15B3, 10, 10);
            new Avatar(0x15B5, 10, 10);
            new Avatar(0x15B7, 10, 10);
            new Avatar(0x15B9, 10, 10);
            new Avatar(0x15BB, 10, 10);
            new Avatar(0x15BD, 10, 10);
            new Avatar(0x15BF, 10, 10);
            new Avatar(0x15C1, 10, 10);
            new Avatar(0x15C3, 10, 10);
            new Avatar(0x15C5, 10, 10);
            new Avatar(0x15C7, 10, 10);
            new Avatar(0x15C9, 10, 10);
            new Avatar(0x15CB, 10, 10);
            new Avatar(0x15CD, 10, 10);
            new Avatar(0x15CF, 10, 10);
            new Avatar(0x15D1, 10, 10);
            new Avatar(0x15D3, 10, 10);
            new Avatar(0x15D5, 10, 10);
            new Avatar(0x15D7, 10, 10);
            new Avatar(0x15E8, 10, 10);
            new Avatar(0x264C, 5, 20);
            new Avatar(0x28D2, 25, 16);
            new Avatar(0x28E0, 21, 17);
            new Avatar(0x2B03, 20, 17);
            new Avatar(0x2B04, 20, 17);
            new Avatar(0x2B05, 20, 17);
            new Avatar(0x2B08, 20, 17);
            new Avatar(0x2B09, 20, 17);
            new Avatar(0x2B2D, 20, 17);
            new Avatar(0x5000, 18, 18);
            new Avatar(0x5001, 18, 18);
            new Avatar(0x5002, 18, 18);
            new Avatar(0x5003, 18, 18);
            new Avatar(0x5004, 18, 18);
            new Avatar(0x5005, 18, 18);
            new Avatar(0x5006, 18, 18);
            new Avatar(0x5007, 18, 18);
            new Avatar(0x5008, 18, 18);
            new Avatar(0x5009, 18, 18);
            new Avatar(0x500A, 18, 18);
            new Avatar(0x500B, 18, 18);
            new Avatar(0x500C, 18, 18);
            new Avatar(0x500D, 18, 18);
            new Avatar(0x500E, 18, 18);
            new Avatar(0x500F, 18, 18);
            new Avatar(0x5010, 18, 18);
            new Avatar(0x5100, 18, 18);
            new Avatar(0x5101, 18, 18);
            new Avatar(0x5102, 18, 18);
            new Avatar(0x5103, 18, 18);
            new Avatar(0x5104, 18, 18);
            new Avatar(0x5105, 18, 18);
            new Avatar(0x5106, 18, 18);
            new Avatar(0x5107, 18, 18);
            new Avatar(0x5108, 18, 18);
            new Avatar(0x5109, 18, 18);
            new Avatar(0x5200, 18, 18);
            new Avatar(0x5201, 18, 18);
            new Avatar(0x5202, 18, 18);
            new Avatar(0x5203, 18, 18);
            new Avatar(0x5204, 18, 18);
            new Avatar(0x5205, 18, 18);
            new Avatar(0x5206, 18, 18);
            new Avatar(0x5207, 18, 18);
            new Avatar(0x5208, 18, 18);
            new Avatar(0x5209, 18, 18);
            new Avatar(0x520A, 18, 18);
            new Avatar(0x520B, 18, 18);
            new Avatar(0x520C, 18, 18);
            new Avatar(0x520D, 18, 18);
            new Avatar(0x520E, 18, 18);
            new Avatar(0x520F, 18, 18);
            new Avatar(0x5210, 18, 18);
            new Avatar(0x5211, 18, 18);
            new Avatar(0x5212, 18, 18);
            new Avatar(0x5213, 18, 18);
            new Avatar(0x5214, 18, 18);
            new Avatar(0x5215, 18, 18);
            new Avatar(0x5216, 18, 18);
            new Avatar(0x5217, 18, 18);
            new Avatar(0x5218, 18, 18);
            new Avatar(0x5219, 18, 18);
            new Avatar(0x521A, 18, 18);
            new Avatar(0x521B, 18, 18);
            new Avatar(0x521C, 18, 18);
            new Avatar(0x5308, 18, 18);
            new Avatar(0x5309, 18, 18);
            new Avatar(0x5320, 18, 18);
            new Avatar(0x5321, 18, 18);
            new Avatar(0x5322, 18, 18);
            new Avatar(0x5323, 18, 18);
            new Avatar(0x5324, 18, 18);
            new Avatar(0x5325, 18, 18);
            new Avatar(0x5326, 18, 18);
            new Avatar(0x5327, 18, 18);
            new Avatar(0x5420, 18, 18);
            new Avatar(0x5421, 18, 18);
            new Avatar(0x5422, 18, 18);
            new Avatar(0x5423, 18, 18);
            new Avatar(0x5424, 18, 18);
            new Avatar(0x5425, 18, 18);
            new Avatar(0x5426, 18, 18);
            new Avatar(0x59D8, 18, 18);
            new Avatar(0x59D9, 18, 18);
            new Avatar(0x59DA, 18, 18);
            new Avatar(0x59DB, 18, 18);
            new Avatar(0x59DC, 18, 18);
            new Avatar(0x59DD, 18, 18);
            new Avatar(0x59DE, 18, 18);
            new Avatar(0x59DF, 18, 18);
            new Avatar(0x59E1, 18, 18);
            new Avatar(0x59E2, 18, 18);
            new Avatar(0x59E3, 18, 18);
            new Avatar(0x59E4, 18, 18);
            new Avatar(0x59E5, 18, 18);
            new Avatar(0x59E6, 18, 18);
            new Avatar(0x59E7, 18, 18);
            new Avatar(104354, 15, 16);

            General.LoadAvatarFile();
        }

        public static Avatar GetAvatar(Mobile m)
        {
            if (s_Avatars[Data.GetData(m).Avatar] == null)
                Data.GetData(m).Avatar = (int)AvaKeys[0];

            return (Avatar)s_Avatars[Data.GetData(m).Avatar];
        }

        private int c_Id, c_X, c_Y;

        public int Id { get { return c_Id; } }
        public int X { get { return c_X; } }
        public int Y { get { return c_Y; } }

        public Avatar(int id, int x, int y)
        {
            c_Id = id;
            c_X = x;
            c_Y = y;

            s_Avatars[id] = this;
        }
	}
}