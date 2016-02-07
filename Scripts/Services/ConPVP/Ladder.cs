using System;
using System.Collections;

namespace Server.Engines.ConPVP
{
    public class LadderController : Item
    {
        private Ladder m_Ladder;
        [Constructable]
        public LadderController()
            : base(0x1B7A)
        {
            this.Visible = false;
            this.Movable = false;

            this.m_Ladder = new Ladder();

            if (Ladder.Instance == null)
                Ladder.Instance = this.m_Ladder;
        }

        public LadderController(Serial serial)
            : base(serial)
        {
        }

        //[CommandProperty( AccessLevel.GameMaster )]
        public Ladder Ladder
        {
            get
            {
                return this.m_Ladder;
            }
            set
            {
            }
        }
        public override string DefaultName
        {
            get
            {
                return "ladder controller";
            }
        }
        public override void Delete()
        {
            if (Ladder.Instance != this.m_Ladder)
                base.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            this.m_Ladder.Serialize(writer);

            writer.Write((bool)(Server.Engines.ConPVP.Ladder.Instance == this.m_Ladder));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Ladder = new Ladder(reader);

                        if (version < 1 || reader.ReadBool())
                            Server.Engines.ConPVP.Ladder.Instance = this.m_Ladder;

                        break;
                    }
            }
        }
    }

    public class Ladder
    {
        private static readonly int[] m_ShortLevels = new int[]
        {
            1,
            2,
            3, 3,
            4, 4,
            5, 5, 5,
            6, 6, 6,
            7, 7, 7, 7,
            8, 8, 8, 8,
            9, 9, 9, 9, 9
        };
        private static readonly int[] m_BaseXP = new int[]
        {
            0, 100, 200, 400, 600, 900, 1200, 1600, 2000, 2500
        };
        private static readonly int[] m_LossFactors = new int[]
        {
            10,
            11, 11,
            25, 25,
            43, 43,
            67, 67
        };
        private static readonly int[,] m_OffsetScalar = new int[,]
        {
            /* { win, los } */
            /* -6 */ { 175, 25 },
            /* -5 */ { 165, 35 },
            /* -4 */ { 155, 45 },
            /* -3 */ { 145, 55 },
            /* -2 */ { 130, 70 },
            /* -1 */ { 115, 85 },
            /*  0 */ { 100, 100 },
            /* +1 */ { 90, 110 },
            /* +2 */ { 80, 120 },
            /* +3 */ { 70, 130 },
            /* +4 */ { 60, 140 },
            /* +5 */ { 50, 150 },
            /* +6 */ { 40, 160 }
        };
        private static Ladder m_Instance;
        private readonly Hashtable m_Table;
        private readonly ArrayList m_Entries;
        public Ladder()
        {
            this.m_Table = new Hashtable();
            this.m_Entries = new ArrayList();
        }

        public Ladder(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        int count = reader.ReadEncodedInt();

                        this.m_Table = new Hashtable(count);
                        this.m_Entries = new ArrayList(count);

                        for (int i = 0; i < count; ++i)
                        {
                            LadderEntry entry = new LadderEntry(reader, this, version);

                            if (entry.Mobile != null)
                            {
                                this.m_Table[entry.Mobile] = entry;
                                entry.Index = this.m_Entries.Count;
                                this.m_Entries.Add(entry);
                            }
                        }

                        if (version == 0)
                        {
                            this.m_Entries.Sort();

                            for (int i = 0; i < this.m_Entries.Count; ++i)
                            {
                                LadderEntry entry = (LadderEntry)this.m_Entries[i];

                                entry.Index = i;
                            }
                        }

                        break;
                    }
            }
        }

        public static Ladder Instance
        {
            get
            {
                return m_Instance;
            }
            set
            {
                m_Instance = value;
            }
        }
        public static int GetLevel(int xp)
        {
            if (xp >= 22500)
                return 50;
            else if (xp >= 2500)
                return (10 + ((xp - 2500) / 500));
            else if (xp < 0)
                xp = 0;

            return m_ShortLevels[xp / 100];
        }

        public static void GetLevelInfo(int level, out int xpBase, out int xpAdvance)
        {
            if (level >= 10)
            {
                xpBase = 2500 + ((level - 10) * 500);
                xpAdvance = 500;
            }
            else
            {
                xpBase = m_BaseXP[level - 1];
                xpAdvance = m_BaseXP[level] - xpBase;
            }
        }

        public static int GetLossFactor(int level)
        {
            if (level >= 10)
                return 100;

            return m_LossFactors[level - 1];
        }

        public static int GetOffsetScalar(int ourLevel, int theirLevel, bool win)
        {
            int x = ourLevel - theirLevel;

            if (x < -6 || x > +6)
                return 0;

            int y = win ? 0 : 1;

            return m_OffsetScalar[x + 6, y];
        }

        public static int GetExperienceGain(LadderEntry us, LadderEntry them, bool weWon)
        {
            if (us == null || them == null)
                return 0;

            int ourLevel = GetLevel(us.Experience);
            int theirLevel = GetLevel(them.Experience);

            int scalar = GetOffsetScalar(ourLevel, theirLevel, weWon);

            if (scalar == 0)
                return 0;

            int xp = 25 * scalar;

            if (!weWon)
                xp = (xp * GetLossFactor(ourLevel)) / 100;

            xp /= 100;

            if (xp <= 0)
                xp = 1;

            return xp * (weWon ? 1 : -1);
        }

        public ArrayList ToArrayList()
        {
            return this.m_Entries;
        }

        public void UpdateEntry(LadderEntry entry)
        {
            int index = entry.Index;

            if (index >= 0 && index < this.m_Entries.Count)
            {
                // sanity
                int c;

                while ((index - 1) >= 0 && (c = entry.CompareTo(this.m_Entries[index - 1])) < 0)
                    index = this.Swap(index, index - 1);

                while ((index + 1) < this.m_Entries.Count && (c = entry.CompareTo(this.m_Entries[index + 1])) > 0)
                    index = this.Swap(index, index + 1);
            }
        }

        public LadderEntry Find(Mobile mob)
        {
            LadderEntry entry = (LadderEntry)this.m_Table[mob];

            if (entry == null)
            {
                this.m_Table[mob] = entry = new LadderEntry(mob, this);
                entry.Index = this.m_Entries.Count;
                this.m_Entries.Add(entry);
            }

            return entry;
        }

        public LadderEntry FindNoCreate(Mobile mob)
        {
            return this.m_Table[mob] as LadderEntry;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)1); // version;

            writer.WriteEncodedInt((int)this.m_Entries.Count);

            for (int i = 0; i < this.m_Entries.Count; ++i)
                ((LadderEntry)this.m_Entries[i]).Serialize(writer);
        }

        private int Swap(int idx, int newIdx)
        {
            object hold = this.m_Entries[idx];

            this.m_Entries[idx] = this.m_Entries[newIdx];
            this.m_Entries[newIdx] = hold;

            ((LadderEntry)this.m_Entries[idx]).Index = idx;
            ((LadderEntry)this.m_Entries[newIdx]).Index = newIdx;

            return newIdx;
        }
    }

    public class LadderEntry : IComparable
    {
        private readonly Mobile m_Mobile;
        private readonly Ladder m_Ladder;
        private int m_Experience;
        private int m_Wins;
        private int m_Losses;
        private int m_Index;
        public LadderEntry(Mobile mob, Ladder ladder)
        {
            this.m_Ladder = ladder;
            this.m_Mobile = mob;
        }

        public LadderEntry(GenericReader reader, Ladder ladder, int version)
        {
            this.m_Ladder = ladder;

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Mobile = reader.ReadMobile();
                        this.m_Experience = reader.ReadEncodedInt();
                        this.m_Wins = reader.ReadEncodedInt();
                        this.m_Losses = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int Experience
        {
            get
            {
                return this.m_Experience;
            }
            set
            {
                this.m_Experience = value;
                this.m_Ladder.UpdateEntry(this);
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int Wins
        {
            get
            {
                return this.m_Wins;
            }
            set
            {
                this.m_Wins = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int Losses
        {
            get
            {
                return this.m_Losses;
            }
            set
            {
                this.m_Losses = value;
            }
        }
        public int Index
        {
            get
            {
                return this.m_Index;
            }
            set
            {
                this.m_Index = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Rank
        {
            get
            {
                return this.m_Index;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.Write((Mobile)this.m_Mobile);
            writer.WriteEncodedInt((int)this.m_Experience);
            writer.WriteEncodedInt((int)this.m_Wins);
            writer.WriteEncodedInt((int)this.m_Losses);
        }

        public int CompareTo(object obj)
        {
            return ((LadderEntry)obj).m_Experience - this.m_Experience;
        }
    }
}