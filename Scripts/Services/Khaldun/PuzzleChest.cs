using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;

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
            for (int i = 0; i < this.m_Cylinders.Length; i++)
            {
                this.m_Cylinders[i] = RandomCylinder();
            }
        }

        public PuzzleChestSolution(PuzzleChestCylinder first, PuzzleChestCylinder second, PuzzleChestCylinder third, PuzzleChestCylinder fourth, PuzzleChestCylinder fifth)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
            this.Fourth = fourth;
            this.Fifth = fifth;
        }

        public PuzzleChestSolution(PuzzleChestSolution solution)
        {
            for (int i = 0; i < this.m_Cylinders.Length; i++)
            {
                this.m_Cylinders[i] = solution.m_Cylinders[i];
            }
        }

        public PuzzleChestSolution(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            int length = reader.ReadEncodedInt();
            for (int i = 0;; i++)
            {
                if (i < length)
                {
                    PuzzleChestCylinder cylinder = (PuzzleChestCylinder)reader.ReadInt();

                    if (i < this.m_Cylinders.Length)
                        this.m_Cylinders[i] = cylinder;
                }
                else if (i < this.m_Cylinders.Length)
                {
                    this.m_Cylinders[i] = RandomCylinder();
                }
                else
                {
                    break;
                }
            }
        }

        public PuzzleChestCylinder[] Cylinders
        {
            get
            {
                return this.m_Cylinders;
            }
        }
        public PuzzleChestCylinder First
        {
            get
            {
                return this.m_Cylinders[0];
            }
            set
            {
                this.m_Cylinders[0] = value;
            }
        }
        public PuzzleChestCylinder Second
        {
            get
            {
                return this.m_Cylinders[1];
            }
            set
            {
                this.m_Cylinders[1] = value;
            }
        }
        public PuzzleChestCylinder Third
        {
            get
            {
                return this.m_Cylinders[2];
            }
            set
            {
                this.m_Cylinders[2] = value;
            }
        }
        public PuzzleChestCylinder Fourth
        {
            get
            {
                return this.m_Cylinders[3];
            }
            set
            {
                this.m_Cylinders[3] = value;
            }
        }
        public PuzzleChestCylinder Fifth
        {
            get
            {
                return this.m_Cylinders[4];
            }
            set
            {
                this.m_Cylinders[4] = value;
            }
        }
        public static PuzzleChestCylinder RandomCylinder()
        {
            switch ( Utility.Random(8) )
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

            for (int i = 0; i < this.m_Cylinders.Length; i++)
            {
                if (this.m_Cylinders[i] == solution.m_Cylinders[i])
                {
                    cylinders++;

                    matchesSrc[i] = true;
                    matchesDst[i] = true;
                }
            }

            for (int i = 0; i < this.m_Cylinders.Length; i++)
            {
                if (!matchesSrc[i])
                {
                    for (int j = 0; j < solution.m_Cylinders.Length; j++)
                    {
                        if (this.m_Cylinders[i] == solution.m_Cylinders[j] && !matchesDst[j])
                        {
                            colors++;

                            matchesDst[j] = true;
                        }
                    }
                }
            }

            return cylinders == this.m_Cylinders.Length;
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_Cylinders.Length);
            for (int i = 0; i < this.m_Cylinders.Length; i++)
            {
                writer.Write((int)this.m_Cylinders[i]);
            }
        }
    }

    public class PuzzleChestSolutionAndTime : PuzzleChestSolution
    {
        private readonly DateTime m_When;
        public PuzzleChestSolutionAndTime(DateTime when, PuzzleChestSolution solution)
            : base(solution)
        {
            this.m_When = when;
        }

        public PuzzleChestSolutionAndTime(GenericReader reader)
            : base(reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_When = reader.ReadDeltaTime();
        }

        public DateTime When
        {
            get
            {
                return this.m_When;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.WriteDeltaTime(this.m_When);
        }
    }

    public abstract class PuzzleChest : BaseTreasureChest
    {
        public const int HintsCount = 3;
        public readonly TimeSpan CleanupTime = TimeSpan.FromHours(1.0);
        private readonly Dictionary<Mobile, PuzzleChestSolutionAndTime> m_Guesses = new Dictionary<Mobile, PuzzleChestSolutionAndTime>();
        private PuzzleChestSolution m_Solution;
        private PuzzleChestCylinder[] m_Hints = new PuzzleChestCylinder[HintsCount];
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
                return this.m_Solution;
            }
            set
            {
                this.m_Solution = value;
                this.InitHints();
            }
        }
        public PuzzleChestCylinder[] Hints
        {
            get
            {
                return this.m_Hints;
            }
        }
        public PuzzleChestCylinder FirstHint
        {
            get
            {
                return this.m_Hints[0];
            }
            set
            {
                this.m_Hints[0] = value;
            }
        }
        public PuzzleChestCylinder SecondHint
        {
            get
            {
                return this.m_Hints[1];
            }
            set
            {
                this.m_Hints[1] = value;
            }
        }
        public PuzzleChestCylinder ThirdHint
        {
            get
            {
                return this.m_Hints[2];
            }
            set
            {
                this.m_Hints[2] = value;
            }
        }
        public override string DefaultName
        {
            get
            {
                return null;
            }
        }
        public override bool CheckLocked(Mobile from)
        {
            if (this.Locked)
            {
                PuzzleChestSolution solution = this.GetLastGuess(from);
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
            this.m_Guesses.TryGetValue(m, out pcst);
            return pcst;
        }

        public void SubmitSolution(Mobile m, PuzzleChestSolution solution)
        {
            int correctCylinders, correctColors;

            if (solution.Matches(this.Solution, out correctCylinders, out correctColors))
            {
                this.LockPick(m);

                this.DisplayTo(m);
            }
            else
            {
                this.m_Guesses[m] = new PuzzleChestSolutionAndTime(DateTime.UtcNow, solution);

                m.SendGump(new StatusGump(correctCylinders, correctColors));

                this.DoDamage(m);
            }
        }

        public void DoDamage(Mobile to)
        {
            switch ( Utility.Random(4) )
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

            this.m_Guesses.Clear();
        }

        public void CleanupGuesses()
        {
            List<Mobile> toDelete = new List<Mobile>();

            foreach (KeyValuePair<Mobile, PuzzleChestSolutionAndTime> kvp in this.m_Guesses)
            {
                if (DateTime.UtcNow - kvp.Value.When > this.CleanupTime)
                    toDelete.Add(kvp.Key);
            }

            foreach (Mobile m in toDelete)
                this.m_Guesses.Remove(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            this.CleanupGuesses();

            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            this.m_Solution.Serialize(writer);

            writer.WriteEncodedInt((int)this.m_Hints.Length);
            for (int i = 0; i < this.m_Hints.Length; i++)
            {
                writer.Write((int)this.m_Hints[i]);
            }

            writer.WriteEncodedInt((int)this.m_Guesses.Count);
            foreach (KeyValuePair<Mobile, PuzzleChestSolutionAndTime> kvp in this.m_Guesses)
            {
                writer.Write(kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Solution = new PuzzleChestSolution(reader);

            int length = reader.ReadEncodedInt();
            for (int i = 0; i < length; i++)
            {
                PuzzleChestCylinder cylinder = (PuzzleChestCylinder)reader.ReadInt();

                if (length == this.m_Hints.Length)
                    this.m_Hints[i] = cylinder;
            }
            if (length != this.m_Hints.Length)
                this.InitHints();

            int guesses = reader.ReadEncodedInt();
            for (int i = 0; i < guesses; i++)
            {
                Mobile m = reader.ReadMobile();
                PuzzleChestSolutionAndTime sol = new PuzzleChestSolutionAndTime(reader);

                this.m_Guesses[m] = sol;
            }
        }

        protected override void SetLockLevel()
        {
            this.LockLevel = 0; // Can't be unlocked
        }

        protected override void GenerateTreasure()
        {
            this.DropItem(new Gold(600, 900));

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
                this.DropItem(gem);

            if (0.2 > Utility.RandomDouble())
                this.DropItem(new BagOfReagents(50));

            for (int i = 0; i < 2; i++)
            {
                Item item;

                if (Core.AOS)
                    item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();
                else
                    item = Loot.RandomArmorOrShieldOrWeapon();

                if (item is BaseWeapon)
                {
                    BaseWeapon weapon = (BaseWeapon)item;

                    if (Core.AOS)
                    {
                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
                    }
                    else
                    {
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(6);
                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                    }

                    this.DropItem(item);
                }
                else if (item is BaseArmor)
                {
                    BaseArmor armor = (BaseArmor)item;

                    if (Core.AOS)
                    {
                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);
                    }
                    else
                    {
                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(6);
                        armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                    }

                    this.DropItem(item);
                }
                else if (item is BaseHat)
                {
                    BaseHat hat = (BaseHat)item;

                    if (Core.AOS)
                    {
                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo(hat, attributeCount, min, max);
                    }

                    this.DropItem(item);
                }
                else if (item is BaseJewel)
                {
                    int attributeCount;
                    int min, max;

                    GetRandomAOSStats(out attributeCount, out min, out max);

                    BaseRunicTool.ApplyAttributesTo((BaseJewel)item, attributeCount, min, max);

                    this.DropItem(item);
                }
            }

            this.Solution = new PuzzleChestSolution();
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
            List<PuzzleChestCylinder> list = new List<PuzzleChestCylinder>(this.Solution.Cylinders.Length - 1);
            for (int i = 1; i < this.Solution.Cylinders.Length; i++)
                list.Add(this.Solution.Cylinders[i]);

            this.m_Hints = new PuzzleChestCylinder[HintsCount];
			
            for (int i = 0; i < this.m_Hints.Length; i++)
            {
                int pos = Utility.Random(list.Count);
                this.m_Hints[i] = list[pos];
                list.RemoveAt(pos);
            }
        }

        private class PuzzleGump : Gump
        {
            private readonly Mobile m_From;
            private readonly PuzzleChest m_Chest;
            private readonly PuzzleChestSolution m_Solution;
            public PuzzleGump(Mobile from, PuzzleChest chest, PuzzleChestSolution solution, int check)
                : base(50, 50)
            {
                this.m_From = from;
                this.m_Chest = chest;
                this.m_Solution = solution;

                this.Dragable = false;

                this.AddBackground(25, 0, 500, 410, 0x53);

                this.AddImage(62, 20, 0x67);

                this.AddHtmlLocalized(80, 36, 110, 70, 1018309, true, false); // A Puzzle Lock

                /* Correctly choose the sequence of cylinders needed to open the latch.  Each cylinder
                * may potentially be used more than once.  Beware!  A false attempt could be deadly!
                */
                this.AddHtmlLocalized(214, 26, 270, 90, 1018310, true, true);

                this.AddLeftCylinderButton(62, 130, PuzzleChestCylinder.LightBlue, 10);
                this.AddLeftCylinderButton(62, 180, PuzzleChestCylinder.Blue, 11);
                this.AddLeftCylinderButton(62, 230, PuzzleChestCylinder.Green, 12);
                this.AddLeftCylinderButton(62, 280, PuzzleChestCylinder.Orange, 13);

                this.AddRightCylinderButton(451, 130, PuzzleChestCylinder.Purple, 14);
                this.AddRightCylinderButton(451, 180, PuzzleChestCylinder.Red, 15);
                this.AddRightCylinderButton(451, 230, PuzzleChestCylinder.DarkBlue, 16);
                this.AddRightCylinderButton(451, 280, PuzzleChestCylinder.Yellow, 17);

                double lockpicking = from.Skills.Lockpicking.Base;
                if (lockpicking >= 60.0)
                {
                    this.AddHtmlLocalized(160, 125, 230, 24, 1018308, false, false); // Lockpicking hint:

                    this.AddBackground(159, 150, 230, 95, 0x13EC);

                    if (lockpicking >= 80.0)
                    {
                        this.AddHtmlLocalized(165, 157, 200, 40, 1018312, false, false); // In the first slot:
                        this.AddCylinder(350, 165, chest.Solution.First);

                        this.AddHtmlLocalized(165, 197, 200, 40, 1018313, false, false); // Used in unknown slot:
                        this.AddCylinder(350, 200, chest.FirstHint);

                        if (lockpicking >= 90.0)
                            this.AddCylinder(350, 212, chest.SecondHint);

                        if (lockpicking >= 100.0)
                            this.AddCylinder(350, 224, chest.ThirdHint);
                    }
                    else
                    {
                        this.AddHtmlLocalized(165, 157, 200, 40, 1018313, false, false); // Used in unknown slot:
                        this.AddCylinder(350, 160, chest.FirstHint);

                        if (lockpicking >= 70.0)
                            this.AddCylinder(350, 172, chest.SecondHint);
                    }
                }

                PuzzleChestSolution lastGuess = chest.GetLastGuess(from);
                if (lastGuess != null)
                {
                    this.AddHtmlLocalized(127, 249, 170, 20, 1018311, false, false); // Thy previous guess:

                    this.AddBackground(290, 247, 115, 25, 0x13EC);

                    this.AddCylinder(281, 254, lastGuess.First);
                    this.AddCylinder(303, 254, lastGuess.Second);
                    this.AddCylinder(325, 254, lastGuess.Third);
                    this.AddCylinder(347, 254, lastGuess.Fourth);
                    this.AddCylinder(369, 254, lastGuess.Fifth);
                }

                this.AddPedestal(140, 270, solution.First, 0, check == 0);
                this.AddPedestal(195, 270, solution.Second, 1, check == 1);
                this.AddPedestal(250, 270, solution.Third, 2, check == 2);
                this.AddPedestal(305, 270, solution.Fourth, 3, check == 3);
                this.AddPedestal(360, 270, solution.Fifth, 4, check == 4);

                this.AddButton(258, 370, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Chest.Deleted || info.ButtonID == 0 || !this.m_From.CheckAlive())
                    return;

                if (this.m_From.IsPlayer() && (this.m_From.Map != this.m_Chest.Map || !this.m_From.InRange(this.m_Chest.GetWorldLocation(), 2)))
                {
                    this.m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500446); // That is too far away.
                    return;
                }

                if (info.ButtonID == 1)
                {
                    this.m_Chest.SubmitSolution(this.m_From, this.m_Solution);
                }
                else
                {
                    if (info.Switches.Length == 0)
                        return;

                    int pedestal = info.Switches[0];
                    if (pedestal < 0 || pedestal >= this.m_Solution.Cylinders.Length)
                        return;

                    PuzzleChestCylinder cylinder;
                    switch ( info.ButtonID )
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

                    this.m_Solution.Cylinders[pedestal] = cylinder;

                    this.m_From.SendGump(new PuzzleGump(this.m_From, this.m_Chest, this.m_Solution, pedestal));
                }
            }

            private void AddLeftCylinderButton(int x, int y, PuzzleChestCylinder cylinder, int buttonID)
            {
                this.AddBackground(x, y, 30, 30, 0x13EC);
                this.AddCylinder(x - 7, y + 10, cylinder);
                this.AddButton(x + 38, y + 9, 0x13A8, 0x4B9, buttonID, GumpButtonType.Reply, 0);
            }

            private void AddRightCylinderButton(int x, int y, PuzzleChestCylinder cylinder, int buttonID)
            {
                this.AddBackground(x, y, 30, 30, 0x13EC);
                this.AddCylinder(x - 7, y + 10, cylinder);
                this.AddButton(x - 26, y + 9, 0x13A8, 0x4B9, buttonID, GumpButtonType.Reply, 0);
            }

            private void AddPedestal(int x, int y, PuzzleChestCylinder cylinder, int switchID, bool initialState)
            {
                this.AddItem(x, y, 0xB10);
                this.AddItem(x - 23, y + 12, 0xB12);
                this.AddItem(x + 23, y + 12, 0xB13);
                this.AddItem(x, y + 23, 0xB11);

                if (cylinder != PuzzleChestCylinder.None)
                {
                    this.AddItem(x, y + 2, 0x51A);
                    this.AddCylinder(x - 1, y + 19, cylinder);
                }
                else
                {
                    this.AddItem(x, y + 2, 0x521);
                }

                this.AddRadio(x + 7, y + 65, 0x867, 0x86A, initialState, switchID);
            }

            private void AddCylinder(int x, int y, PuzzleChestCylinder cylinder)
            {
                if (cylinder != PuzzleChestCylinder.None)
                    this.AddItem(x, y, (int)cylinder);
                else
                    this.AddItem(x + 9, y, (int)cylinder);
            }
        }

        private class StatusGump : Gump
        {
            public StatusGump(int correctCylinders, int correctColors)
                : base(50, 50)
            {
                this.AddBackground(15, 250, 305, 163, 0x53);
                this.AddBackground(28, 265, 280, 133, 0xBB8);

                this.AddHtmlLocalized(35, 271, 270, 24, 1018314, false, false); // Thou hast failed to solve the puzzle!

                this.AddHtmlLocalized(35, 297, 250, 24, 1018315, false, false); // Correctly placed colors:
                this.AddLabel(285, 297, 0x44, correctCylinders.ToString());

                this.AddHtmlLocalized(35, 323, 250, 24, 1018316, false, false); // Used colors in wrong slots:
                this.AddLabel(285, 323, 0x44, correctColors.ToString());

                this.AddButton(152, 369, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
            }
        }
    }

    [FlipableAttribute(0xE41, 0xE40)]
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

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [FlipableAttribute(0xE80, 0x9A8)]
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

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}