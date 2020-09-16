using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum PuzzleChestCylinder
    {
        None = 0xE73,
        LightBlue = 0x186F,
        Blue = 0x186A,
        Green = 0x186B,
        Orange = 0x186C,
        Purple = 0x186D,
        Red = 0x186E,
        DarkBlue = 0x1869,
        Yellow = 0x1870
    }

    public class PuzzleChestSolution
    {
        public const int Length = 5;
        private readonly PuzzleChestCylinder[] m_Cylinders = new PuzzleChestCylinder[Length];
        public PuzzleChestSolution()
        {
            for (int i = 0; i < m_Cylinders.Length; i++)
            {
                m_Cylinders[i] = RandomCylinder();
            }
        }

        public PuzzleChestSolution(PuzzleChestCylinder first, PuzzleChestCylinder second, PuzzleChestCylinder third, PuzzleChestCylinder fourth, PuzzleChestCylinder fifth)
        {
            First = first;
            Second = second;
            Third = third;
            Fourth = fourth;
            Fifth = fifth;
        }

        public PuzzleChestSolution(PuzzleChestSolution solution)
        {
            for (int i = 0; i < m_Cylinders.Length; i++)
            {
                m_Cylinders[i] = solution.m_Cylinders[i];
            }
        }

        public PuzzleChestSolution(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            int length = reader.ReadEncodedInt();
            for (int i = 0; ; i++)
            {
                if (i < length)
                {
                    PuzzleChestCylinder cylinder = (PuzzleChestCylinder)reader.ReadInt();

                    if (i < m_Cylinders.Length)
                        m_Cylinders[i] = cylinder;
                }
                else if (i < m_Cylinders.Length)
                {
                    m_Cylinders[i] = RandomCylinder();
                }
                else
                {
                    break;
                }
            }
        }

        public PuzzleChestCylinder[] Cylinders => m_Cylinders;
        public PuzzleChestCylinder First
        {
            get
            {
                return m_Cylinders[0];
            }
            set
            {
                m_Cylinders[0] = value;
            }
        }
        public PuzzleChestCylinder Second
        {
            get
            {
                return m_Cylinders[1];
            }
            set
            {
                m_Cylinders[1] = value;
            }
        }
        public PuzzleChestCylinder Third
        {
            get
            {
                return m_Cylinders[2];
            }
            set
            {
                m_Cylinders[2] = value;
            }
        }
        public PuzzleChestCylinder Fourth
        {
            get
            {
                return m_Cylinders[3];
            }
            set
            {
                m_Cylinders[3] = value;
            }
        }
        public PuzzleChestCylinder Fifth
        {
            get
            {
                return m_Cylinders[4];
            }
            set
            {
                m_Cylinders[4] = value;
            }
        }
        public static PuzzleChestCylinder RandomCylinder()
        {
            switch (Utility.Random(8))
            {
                case 0:
                    return PuzzleChestCylinder.LightBlue;
                case 1:
                    return PuzzleChestCylinder.Blue;
                case 2:
                    return PuzzleChestCylinder.Green;
                case 3:
                    return PuzzleChestCylinder.Orange;
                case 4:
                    return PuzzleChestCylinder.Purple;
                case 5:
                    return PuzzleChestCylinder.Red;
                case 6:
                    return PuzzleChestCylinder.DarkBlue;
                default:
                    return PuzzleChestCylinder.Yellow;
            }
        }

        public bool Matches(PuzzleChestSolution solution, out int cylinders, out int colors)
        {
            cylinders = 0;
            colors = 0;

            bool[] matchesSrc = new bool[solution.m_Cylinders.Length];
            bool[] matchesDst = new bool[solution.m_Cylinders.Length];

            for (int i = 0; i < m_Cylinders.Length; i++)
            {
                if (m_Cylinders[i] == solution.m_Cylinders[i])
                {
                    cylinders++;

                    matchesSrc[i] = true;
                    matchesDst[i] = true;
                }
            }

            for (int i = 0; i < m_Cylinders.Length; i++)
            {
                if (!matchesSrc[i])
                {
                    for (int j = 0; j < solution.m_Cylinders.Length; j++)
                    {
                        if (m_Cylinders[i] == solution.m_Cylinders[j] && !matchesDst[j])
                        {
                            colors++;

                            matchesDst[j] = true;
                        }
                    }
                }
            }

            return cylinders == m_Cylinders.Length;
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(m_Cylinders.Length);
            for (int i = 0; i < m_Cylinders.Length; i++)
            {
                writer.Write((int)m_Cylinders[i]);
            }
        }
    }

    public class PuzzleChestSolutionAndTime : PuzzleChestSolution
    {
        private readonly DateTime m_When;
        public PuzzleChestSolutionAndTime(DateTime when, PuzzleChestSolution solution)
            : base(solution)
        {
            m_When = when;
        }

        public PuzzleChestSolutionAndTime(GenericReader reader)
            : base(reader)
        {
            int version = reader.ReadEncodedInt();

            m_When = reader.ReadDeltaTime();
        }

        public DateTime When => m_When;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteDeltaTime(m_When);
        }
    }

    public abstract class PuzzleChest : BaseTreasureChest
    {
        public const int HintsCount = 3;
        public readonly TimeSpan CleanupTime = TimeSpan.FromHours(1.0);
        private readonly Dictionary<Mobile, PuzzleChestSolutionAndTime> m_Guesses = new Dictionary<Mobile, PuzzleChestSolutionAndTime>();
        private PuzzleChestSolution m_Solution;
        private PuzzleChestCylinder[] m_Hints = new PuzzleChestCylinder[HintsCount];

        public virtual int Label => 1018309;  // A Puzzle Lock

        public PuzzleChest(int itemID)
            : base(itemID)
        {
        }

        public PuzzleChest(Serial serial)
            : base(serial)
        {
        }

        public PuzzleChestSolution Solution
        {
            get
            {
                return m_Solution;
            }
            set
            {
                m_Solution = value;
                InitHints();
            }
        }
        public PuzzleChestCylinder[] Hints => m_Hints;
        public PuzzleChestCylinder FirstHint
        {
            get
            {
                return m_Hints[0];
            }
            set
            {
                m_Hints[0] = value;
            }
        }
        public PuzzleChestCylinder SecondHint
        {
            get
            {
                return m_Hints[1];
            }
            set
            {
                m_Hints[1] = value;
            }
        }
        public PuzzleChestCylinder ThirdHint
        {
            get
            {
                return m_Hints[2];
            }
            set
            {
                m_Hints[2] = value;
            }
        }
        public override string DefaultName => null;
        public override bool CheckLocked(Mobile from)
        {
            if (Locked)
            {
                PuzzleChestSolution solution = GetLastGuess(from);
                if (solution != null)
                    solution = new PuzzleChestSolution(solution);
                else
                    solution = new PuzzleChestSolution(PuzzleChestCylinder.None, PuzzleChestCylinder.None, PuzzleChestCylinder.None, PuzzleChestCylinder.None, PuzzleChestCylinder.None);

                from.CloseGump(typeof(PuzzleGump));
                from.CloseGump(typeof(StatusGump));
                from.SendGump(new PuzzleGump(from, this, solution, 0));

                return true;
            }
            else
            {
                return false;
            }
        }

        public PuzzleChestSolutionAndTime GetLastGuess(Mobile m)
        {
            PuzzleChestSolutionAndTime pcst = null;
            m_Guesses.TryGetValue(m, out pcst);
            return pcst;
        }

        public void SubmitSolution(Mobile m, PuzzleChestSolution solution)
        {
            int correctCylinders, correctColors;

            if (solution.Matches(Solution, out correctCylinders, out correctColors))
            {
                LockPick(m);

                DisplayTo(m);
            }
            else
            {
                m_Guesses[m] = new PuzzleChestSolutionAndTime(DateTime.UtcNow, solution);

                m.SendGump(new StatusGump(correctCylinders, correctColors));

                DoDamage(m);
            }
        }

        public virtual void DoDamage(Mobile to)
        {
            switch (Utility.Random(4))
            {
                case 0:
                    {
                        Effects.SendLocationEffect(to, to.Map, 0x113A, 20, 10);
                        to.PlaySound(0x231);
                        to.LocalOverheadMessage(MessageType.Regular, 0x44, 1010523); // A toxic vapor envelops thee.

                        to.ApplyPoison(to, Poison.Regular);

                        break;
                    }
                case 1:
                    {
                        Effects.SendLocationEffect(to, to.Map, 0x3709, 30);
                        to.PlaySound(0x54);
                        to.LocalOverheadMessage(MessageType.Regular, 0xEE, 1010524); // Searing heat scorches thy skin.

                        AOS.Damage(to, to, Utility.RandomMinMax(10, 40), 0, 100, 0, 0, 0);

                        break;
                    }
                case 2:
                    {
                        to.PlaySound(0x223);
                        to.LocalOverheadMessage(MessageType.Regular, 0x62, 1010525); // Pain lances through thee from a sharp metal blade.

                        AOS.Damage(to, to, Utility.RandomMinMax(10, 40), 100, 0, 0, 0, 0);

                        break;
                    }
                default:
                    {
                        to.BoltEffect(0);
                        to.LocalOverheadMessage(MessageType.Regular, 0xDA, 1010526); // Lightning arcs through thy body.

                        AOS.Damage(to, to, Utility.RandomMinMax(10, 40), 0, 0, 0, 0, 100);

                        break;
                    }
            }
        }

        public override void LockPick(Mobile from)
        {
            base.LockPick(from);

            m_Guesses.Clear();
        }

        public void CleanupGuesses()
        {
            List<Mobile> toDelete = new List<Mobile>();

            foreach (KeyValuePair<Mobile, PuzzleChestSolutionAndTime> kvp in m_Guesses)
            {
                if (DateTime.UtcNow - kvp.Value.When > CleanupTime)
                    toDelete.Add(kvp.Key);
            }

            foreach (Mobile m in toDelete)
                m_Guesses.Remove(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            CleanupGuesses();

            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            m_Solution.Serialize(writer);

            writer.WriteEncodedInt(m_Hints.Length);
            for (int i = 0; i < m_Hints.Length; i++)
            {
                writer.Write((int)m_Hints[i]);
            }

            writer.WriteEncodedInt(m_Guesses.Count);
            foreach (KeyValuePair<Mobile, PuzzleChestSolutionAndTime> kvp in m_Guesses)
            {
                writer.Write(kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Solution = new PuzzleChestSolution(reader);

            int length = reader.ReadEncodedInt();
            for (int i = 0; i < length; i++)
            {
                PuzzleChestCylinder cylinder = (PuzzleChestCylinder)reader.ReadInt();

                if (length == m_Hints.Length)
                    m_Hints[i] = cylinder;
            }
            if (length != m_Hints.Length)
                InitHints();

            int guesses = reader.ReadEncodedInt();
            for (int i = 0; i < guesses; i++)
            {
                Mobile m = reader.ReadMobile();
                PuzzleChestSolutionAndTime sol = new PuzzleChestSolutionAndTime(reader);

                m_Guesses[m] = sol;
            }
        }

        protected override void SetLockLevel()
        {
            LockLevel = 0; // Can't be unlocked
        }

        protected override void GenerateTreasure()
        {
            DropItem(new Gold(600, 900));

            List<Item> gems = new List<Item>();
            for (int i = 0; i < 9; i++)
            {
                Item gem = Loot.RandomGem();
                Type gemType = gem.GetType();

                foreach (Item listGem in gems)
                {
                    if (listGem.GetType() == gemType)
                    {
                        listGem.Amount++;
                        gem.Delete();
                        break;
                    }
                }

                if (!gem.Deleted)
                    gems.Add(gem);
            }

            foreach (Item gem in gems)
                DropItem(gem);

            if (0.2 > Utility.RandomDouble())
                DropItem(new BagOfReagents(50));

            for (int i = 0; i < 2; i++)
            {
                Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();

                if (item is BaseWeapon)
                {
                    BaseWeapon weapon = (BaseWeapon)item;

                    int attributeCount;
                    int min, max;

                    GetRandomAOSStats(out attributeCount, out min, out max);

                    BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);

                    DropItem(item);
                }
                else if (item is BaseArmor)
                {
                    BaseArmor armor = (BaseArmor)item;

                    int attributeCount;
                    int min, max;

                    GetRandomAOSStats(out attributeCount, out min, out max);

                    BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);

                    DropItem(item);
                }
                else if (item is BaseHat)
                {
                    BaseHat hat = (BaseHat)item;

                    int attributeCount;
                    int min, max;

                    GetRandomAOSStats(out attributeCount, out min, out max);

                    BaseRunicTool.ApplyAttributesTo(hat, attributeCount, min, max);

                    DropItem(item);
                }
                else if (item is BaseJewel)
                {
                    int attributeCount;
                    int min, max;

                    GetRandomAOSStats(out attributeCount, out min, out max);

                    BaseRunicTool.ApplyAttributesTo((BaseJewel)item, attributeCount, min, max);

                    DropItem(item);
                }
            }

            Solution = new PuzzleChestSolution();
        }

        private static void GetRandomAOSStats(out int attributeCount, out int min, out int max)
        {
            int rnd = Utility.Random(15);

            if (rnd < 1)
            {
                attributeCount = Utility.RandomMinMax(2, 6);
                min = 20;
                max = 70;
            }
            else if (rnd < 3)
            {
                attributeCount = Utility.RandomMinMax(2, 4);
                min = 20;
                max = 50;
            }
            else if (rnd < 6)
            {
                attributeCount = Utility.RandomMinMax(2, 3);
                min = 20;
                max = 40;
            }
            else if (rnd < 10)
            {
                attributeCount = Utility.RandomMinMax(1, 2);
                min = 10;
                max = 30;
            }
            else
            {
                attributeCount = 1;
                min = 10;
                max = 20;
            }
        }

        private void InitHints()
        {
            List<PuzzleChestCylinder> list = new List<PuzzleChestCylinder>(Solution.Cylinders.Length - 1);
            for (int i = 1; i < Solution.Cylinders.Length; i++)
                list.Add(Solution.Cylinders[i]);

            m_Hints = new PuzzleChestCylinder[HintsCount];

            for (int i = 0; i < m_Hints.Length; i++)
            {
                int pos = Utility.Random(list.Count);
                m_Hints[i] = list[pos];
                list.RemoveAt(pos);
            }
        }

        public class PuzzleGump : Gump
        {
            private readonly Mobile m_From;
            private readonly PuzzleChest m_Chest;
            private readonly PuzzleChestSolution m_Solution;
            public PuzzleGump(Mobile from, PuzzleChest chest, PuzzleChestSolution solution, int check)
                : base(50, 50)
            {
                m_From = from;
                m_Chest = chest;
                m_Solution = solution;

                Dragable = false;

                AddBackground(25, 0, 500, 410, 0x53);

                AddImage(62, 20, 0x67);

                AddHtmlLocalized(80, 36, 110, 70, 1018309, true, false); // A Puzzle Lock

                /* Correctly choose the sequence of cylinders needed to open the latch.  Each cylinder
                * may potentially be used more than once.  Beware!  A false attempt could be deadly!
                */
                AddHtmlLocalized(214, 26, 270, 90, 1018310, true, true);

                AddLeftCylinderButton(62, 130, PuzzleChestCylinder.LightBlue, 10);
                AddLeftCylinderButton(62, 180, PuzzleChestCylinder.Blue, 11);
                AddLeftCylinderButton(62, 230, PuzzleChestCylinder.Green, 12);
                AddLeftCylinderButton(62, 280, PuzzleChestCylinder.Orange, 13);

                AddRightCylinderButton(451, 130, PuzzleChestCylinder.Purple, 14);
                AddRightCylinderButton(451, 180, PuzzleChestCylinder.Red, 15);
                AddRightCylinderButton(451, 230, PuzzleChestCylinder.DarkBlue, 16);
                AddRightCylinderButton(451, 280, PuzzleChestCylinder.Yellow, 17);

                double lockpicking = from.Skills.Lockpicking.Base;
                if (lockpicking >= 60.0)
                {
                    AddHtmlLocalized(160, 125, 230, 24, 1018308, false, false); // Lockpicking hint:

                    AddBackground(159, 150, 230, 95, 0x13EC);

                    if (lockpicking >= 80.0)
                    {
                        AddHtmlLocalized(165, 157, 200, 40, 1018312, false, false); // In the first slot:
                        AddCylinder(350, 165, chest.Solution.First);

                        AddHtmlLocalized(165, 197, 200, 40, 1018313, false, false); // Used in unknown slot:
                        AddCylinder(350, 200, chest.FirstHint);

                        if (lockpicking >= 90.0)
                            AddCylinder(350, 212, chest.SecondHint);

                        if (lockpicking >= 100.0)
                            AddCylinder(350, 224, chest.ThirdHint);
                    }
                    else
                    {
                        AddHtmlLocalized(165, 157, 200, 40, 1018313, false, false); // Used in unknown slot:
                        AddCylinder(350, 160, chest.FirstHint);

                        if (lockpicking >= 70.0)
                            AddCylinder(350, 172, chest.SecondHint);
                    }
                }

                PuzzleChestSolution lastGuess = chest.GetLastGuess(from);
                if (lastGuess != null)
                {
                    AddHtmlLocalized(127, 249, 170, 20, 1018311, false, false); // Thy previous guess:

                    AddBackground(290, 247, 115, 25, 0x13EC);

                    AddCylinder(281, 254, lastGuess.First);
                    AddCylinder(303, 254, lastGuess.Second);
                    AddCylinder(325, 254, lastGuess.Third);
                    AddCylinder(347, 254, lastGuess.Fourth);
                    AddCylinder(369, 254, lastGuess.Fifth);
                }

                AddPedestal(140, 270, solution.First, 0, check == 0);
                AddPedestal(195, 270, solution.Second, 1, check == 1);
                AddPedestal(250, 270, solution.Third, 2, check == 2);
                AddPedestal(305, 270, solution.Fourth, 3, check == 3);
                AddPedestal(360, 270, solution.Fifth, 4, check == 4);

                AddButton(258, 370, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Chest.Deleted || info.ButtonID == 0 || !m_From.CheckAlive())
                    return;

                if (m_From.IsPlayer() && (m_From.Map != m_Chest.Map || !m_From.InRange(m_Chest.GetWorldLocation(), 2)))
                {
                    m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500446); // That is too far away.
                    return;
                }

                if (info.ButtonID == 1)
                {
                    m_Chest.SubmitSolution(m_From, m_Solution);
                }
                else
                {
                    if (info.Switches.Length == 0)
                        return;

                    int pedestal = info.Switches[0];
                    if (pedestal < 0 || pedestal >= m_Solution.Cylinders.Length)
                        return;

                    PuzzleChestCylinder cylinder;
                    switch (info.ButtonID)
                    {
                        case 10:
                            cylinder = PuzzleChestCylinder.LightBlue;
                            break;
                        case 11:
                            cylinder = PuzzleChestCylinder.Blue;
                            break;
                        case 12:
                            cylinder = PuzzleChestCylinder.Green;
                            break;
                        case 13:
                            cylinder = PuzzleChestCylinder.Orange;
                            break;
                        case 14:
                            cylinder = PuzzleChestCylinder.Purple;
                            break;
                        case 15:
                            cylinder = PuzzleChestCylinder.Red;
                            break;
                        case 16:
                            cylinder = PuzzleChestCylinder.DarkBlue;
                            break;
                        case 17:
                            cylinder = PuzzleChestCylinder.Yellow;
                            break;
                        default:
                            return;
                    }

                    m_Solution.Cylinders[pedestal] = cylinder;

                    m_From.SendGump(new PuzzleGump(m_From, m_Chest, m_Solution, pedestal));
                }
            }

            private void AddLeftCylinderButton(int x, int y, PuzzleChestCylinder cylinder, int buttonID)
            {
                AddBackground(x, y, 30, 30, 0x13EC);
                AddCylinder(x - 7, y + 10, cylinder);
                AddButton(x + 38, y + 9, 0x13A8, 0x4B9, buttonID, GumpButtonType.Reply, 0);
            }

            private void AddRightCylinderButton(int x, int y, PuzzleChestCylinder cylinder, int buttonID)
            {
                AddBackground(x, y, 30, 30, 0x13EC);
                AddCylinder(x - 7, y + 10, cylinder);
                AddButton(x - 26, y + 9, 0x13A8, 0x4B9, buttonID, GumpButtonType.Reply, 0);
            }

            private void AddPedestal(int x, int y, PuzzleChestCylinder cylinder, int switchID, bool initialState)
            {
                AddItem(x, y, 0xB10);
                AddItem(x - 23, y + 12, 0xB12);
                AddItem(x + 23, y + 12, 0xB13);
                AddItem(x, y + 23, 0xB11);

                if (cylinder != PuzzleChestCylinder.None)
                {
                    AddItem(x, y + 2, 0x51A);
                    AddCylinder(x - 1, y + 19, cylinder);
                }
                else
                {
                    AddItem(x, y + 2, 0x521);
                }

                AddRadio(x + 7, y + 65, 0x867, 0x86A, initialState, switchID);
            }

            private void AddCylinder(int x, int y, PuzzleChestCylinder cylinder)
            {
                if (cylinder != PuzzleChestCylinder.None)
                    AddItem(x, y, (int)cylinder);
                else
                    AddItem(x + 9, y, (int)cylinder);
            }
        }

        public class StatusGump : Gump
        {
            public StatusGump(int correctCylinders, int correctColors)
                : base(50, 50)
            {
                AddBackground(15, 250, 305, 163, 0x53);
                AddBackground(28, 265, 280, 133, 0xBB8);

                AddHtmlLocalized(35, 271, 270, 24, 1018314, false, false); // Thou hast failed to solve the puzzle!

                AddHtmlLocalized(35, 297, 250, 24, 1018315, false, false); // Correctly placed colors:
                AddLabel(285, 297, 0x44, correctCylinders.ToString());

                AddHtmlLocalized(35, 323, 250, 24, 1018316, false, false); // Used colors in wrong slots:
                AddLabel(285, 323, 0x44, correctColors.ToString());

                AddButton(152, 369, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
            }
        }
    }

    [Flipable(0xE41, 0xE40)]
    public class MetalGoldenPuzzleChest : PuzzleChest
    {
        [Constructable]
        public MetalGoldenPuzzleChest()
            : base(0xE41)
        {
        }

        public MetalGoldenPuzzleChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0xE80, 0x9A8)]
    public class StrongBoxPuzzle : PuzzleChest
    {
        [Constructable]
        public StrongBoxPuzzle()
            : base(0xE80)
        {
        }

        public StrongBoxPuzzle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
