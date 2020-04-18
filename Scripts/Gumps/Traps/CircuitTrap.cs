using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public interface ICircuitTrap
    {
        int GumpTitle { get; }
        int GumpDescription { get; }
        CircuitCount Count { get; }

        bool CanDecipher { get; }

        List<int> Path { get; set; }
        List<int> Progress { get; set; }

        void OnProgress(Mobile m, int pick);
        void OnFailed(Mobile m);
        void OnComplete(Mobile m);
        void OnSelfClose(Mobile m);
    }

    public enum CircuitCount
    {
        Nine = 9,
        Sixteen = 16,
        TwentyFive = 25,
        ThirtySix = 36
    }

    public class CircuitTrapGump : BaseGump
    {
        public ICircuitTrap Trap { get; set; }
        public List<int> Path => Trap != null ? Trap.Path : null;
        public List<int> Progress => Trap != null ? Trap.Progress : null;

        public CircuitCount Count => Trap != null ? Trap.Count : CircuitCount.Nine;
        public bool ShowNext { get; set; }

        public CircuitTrapGump(PlayerMobile from, ICircuitTrap item)
            : base(from, 5, 30)
        {
            Trap = item;
            from.CloseGump(GetType());
        }

        public override void AddGumpLayout()
        {
            int size;

            switch (Count)
            {
                default:
                case CircuitCount.Nine: size = 150; break;
                case CircuitCount.Sixteen: size = 190; break;
                case CircuitCount.TwentyFive: size = 230; break;
                case CircuitCount.ThirtySix: size = 270; break;
            }

            AddBackground(50, 0, 530, 410, 0xA28);
            AddBackground(95, 20, 442, 90, 0xA28);
            AddBackground(90, 115, size, size, 0xA28);
            AddBackground(100, 125, size - 20, size - 20, 0x1400);
            AddBackground(365, 120, 178, 210, 0x1400);

            AddImage(0, 0, 0x28C8);
            AddImage(547, 0, 0x28C9);
            AddImage(140, 40, 0x28D3);
            AddImage(420, 40, 0x28D3);
            AddImage(365, 115, 0x28D4);
            AddImage(365, 288, 0x28D4);
            AddImage(414, 189, 0x589);
            AddImage(435, 210, 0xA52);

            if (Path == null || Path.Count == 0)
            {
                Trap.Path = GetRandomPath();
            }

            if (Progress == null || Progress.Count == 0)
            {
                Trap.Progress = new List<int>();
                Trap.Progress.Add(0);
            }

            int sx = 110;
            int sy = 135;
            int count = (int)Count;
            double sq = Math.Sqrt(count);

            for (int i = 0; i < count; i++)
            {
                int line = (int)(i / sq);
                int col = (int)(i % sq);

                int x = sx + (col * 40);
                int y = sy + (line * 40);

                AddImage(x, y, i == count - 1 ? 0x9A8 : Progress.Contains(i) ? 0x868 : 0x25F8);

                if (line + 1 < sq)
                {
                    AddImage(x + 10, y + 27, 0x13F9);
                }

                if (col + 1 < sq)
                {
                    AddImage(x + 18, y + 12, 0x13FD);
                }

                if (i == Progress[Progress.Count - 1])
                {
                    AddImage(x + 8, y + 8, 0x13A8);
                }

                if (ShowNext && Progress.Count <= Path.Count && i == Path[Progress.Count])
                {
                    AddImage(x + 8, y + 8, 2361);
                }
            }

            if (ShowNext)
            {
                ShowNext = false;
            }

            AddHtmlLocalized(210, 35, 212, 45, Trap.GumpTitle, false, false);
            AddHtmlLocalized(210, 60, 212, 70, 1153748, false, false); // <center>Use the Directional Controls to</center>
            AddHtmlLocalized(210, 75, 212, 85, Trap.GumpDescription, false, false);

            if (Trap.CanDecipher && User.Skills[SkillName.Lockpicking].Base >= 100)
            {
                AddHtmlLocalized(405, 355, 150, 32, 1153750, false, false); // Attempt to Decipher the Circuit Path
                AddButton(365, 355, 4005, 4005, 5, GumpButtonType.Reply, 0);
            }

            AddButton(448, 185, 10700, 10701, 1, GumpButtonType.Reply, 0); // up
            AddButton(473, 222, 10710, 10711, 2, GumpButtonType.Reply, 0); // right
            AddButton(448, 243, 10720, 10721, 3, GumpButtonType.Reply, 0); // down 
            AddButton(408, 222, 10730, 10731, 4, GumpButtonType.Reply, 0); // left

        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                Trap.OnSelfClose(User);
                return;
            }

            if (info.ButtonID > 0 && info.ButtonID < 5)
            {
                int id = info.ButtonID;
                int current = Progress[Progress.Count - 1];
                int next = 35;
                int count = (int)Count;
                int perRow = (int)Math.Sqrt(count);
                int pick;

                if (Progress.Count > 0 && Progress.Count < Path.Count)
                    next = Path[Progress.Count];

                switch (id)
                {
                    default:
                    case 1: pick = current - perRow; break;
                    case 2: pick = current + 1; break;
                    case 3: pick = current + perRow; break;
                    case 4: pick = current - 1; break;
                }

                if (pick < 0 || pick > count - 1)      //Off board or already chosen spot
                {
                    User.PlaySound(0x5B6);
                    Refresh();
                }
                else if ((current == count - 2 || current == (count - 1) - perRow) && pick == count - 1)
                {
                    Trap.Path.Clear();
                    Trap.Progress.Clear();

                    Trap.OnComplete(User);
                }
                else if (pick == next)
                {
                    Trap.OnProgress(User, pick);
                    Trap.Progress.Add(pick);

                    Refresh();
                }
                else
                {
                    Trap.OnFailed(User);
                    Trap.Progress.Clear();
                }
            }

            else if (info.ButtonID == 5 && Trap.CanDecipher)
            {
                ShowNext = true;
                Refresh();
            }
        }

        private readonly int[] m_Possibles = new int[]
        {
            0,  1,  2,  3,  4,  5,
            6,  7,  8,  9,  10, 11,
            12, 13, 14, 15, 16, 17,
            18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35
        };

        private readonly int[][] Paths9 = new int[][]
        {
            new int[] { 0, 1, 2, 5, 8 },
            new int[] { 0, 1, 4, 5, 8 },
            new int[] { 0, 1, 4, 3, 6, 7, 8 },
            new int[] { 0, 1, 4, 7, 8},
            new int[] { 0, 3, 6, 7, 8 },
            new int[] { 0, 3, 4, 5, 8 },
            new int[] { 0, 3, 4, 7, 8 },
            new int[] { 0, 3, 6, 7, 4, 5, 8 },
            new int[] { 0, 3, 6, 7, 4, 1, 2, 5, 8 }
        };

        private readonly int[][] Paths16 = new int[][]
        {
            new int[] { 0, 1, 2, 3, 7, 11, 15 },
            new int[] { 0, 1, 2, 6, 7, 11, 15 },
            new int[] { 0, 1, 2, 3, 7, 6, 5, 4, 8, 9, 10, 11, 15 },
            new int[] { 0, 1, 5, 6, 7, 11, 15 },
            new int[] { 0, 1, 5, 4, 8, 12, 13, 14, 15 },
            new int[] { 0, 1, 5, 4, 8, 9, 10, 14, 15 },
            new int[] { 0, 4, 8, 12, 13, 14, 15 },
            new int[] { 0, 4, 8, 9, 10, 6, 2, 3, 7, 11, 15 },
            new int[] { 0, 4, 8, 9, 5, 6, 7, 11, 15 },
            new int[] { 0, 4, 5, 6, 2, 3, 7, 11, 15 },
            new int[] { 0, 4, 5, 6, 7, 11, 10, 9, 8, 12, 13, 14, 15 },
            new int[] { 0, 4, 5, 9, 10, 11, 15 },
        };

        private readonly int[][] Paths25 = new int[][]
        {
            new int[] { 0, 1, 2, 3, 4, 9, 14, 19, 24 },
            new int[] { 0, 1, 2, 7, 8, 9, 14, 13, 12, 11, 10, 15, 20, 21, 22, 23, 24 },
            new int[] { 0, 1, 2, 3, 4, 9, 14, 13, 12, 17, 22, 23, 24 },
            new int[] { 0, 1, 6, 7, 8, 13, 12, 17, 18, 19, 24 },
            new int[] { 0, 1, 6, 5, 10, 11, 12, 17, 18, 19, 24 },
            new int[] { 0, 1, 6, 5, 10, 15, 16, 17, 12, 7, 8, 9, 14, 19, 24},
            new int[] { 0, 5, 6, 7, 2, 3, 4, 9, 14, 19, 24 },
            new int[] { 0, 5, 6, 11, 12, 13, 18, 19, 24 },
            new int[] { 0, 5, 6, 11, 16, 21, 22, 17, 12, 7, 8, 9, 14, 19, 24 },
            new int[] { 0, 5, 10, 15, 20, 21, 22, 23, 24 },
            new int[] { 0, 5, 10, 11, 12, 13, 8, 7, 6, 1, 2, 3, 4, 9, 14, 19, 24 },
            new int[] { 0, 5, 10, 11, 16, 17, 18, 23, 24 },
        };

        private readonly int[][] Paths36 = new int[][]
        {
            new int[] { 0, 1, 2, 3, 4, 5, 11, 17, 23, 29, 35 },
            new int[] { 0, 6, 12, 18, 24, 30, 31, 32, 33, 34, 35 },
            new int[] { 0, 1, 2, 8, 14, 15, 16, 22, 28, 29, 35 },
            new int[] { 0, 1, 7, 13, 19, 20, 21, 27, 33, 34, 35 },
            new int[] { 0, 1, 7, 8, 14, 20, 26, 27, 33, 34, 35 },
            new int[] { 0, 1, 2, 3, 9, 10, 16, 15, 21, 27, 28, 34, 35 },
            new int[] { 0, 6, 12, 13, 19, 20, 26, 27, 28, 29, 35 },
            new int[] { 0, 6, 12, 18, 19, 25, 26, 20, 21, 22, 28, 34, 35 },
            new int[] { 0, 6, 7, 8, 14, 20, 21, 27, 28, 29, 35 },
            new int[] { 0, 6, 7, 13, 12, 18, 19, 20, 21, 27, 28, 34, 35 },
            new int[] { 0, 6, 12, 13, 19, 18, 24, 30, 31, 32, 33, 34, 35 },
            new int[] { 0, 1, 2, 8, 9, 15, 16, 10, 11, 17, 23, 29, 35 },
        };

        public List<int> GetRandomPath()
        {
            switch (Count)
            {
                default:
                case CircuitCount.Nine: return Paths9[Utility.Random(Paths9.Length)].ToList();
                case CircuitCount.Sixteen: return Paths16[Utility.Random(Paths16.Length)].ToList();
                case CircuitCount.TwentyFive: return Paths25[Utility.Random(Paths25.Length)].ToList();
                case CircuitCount.ThirtySix: return Paths36[Utility.Random(Paths36.Length)].ToList();
            }
        }
    }
}
