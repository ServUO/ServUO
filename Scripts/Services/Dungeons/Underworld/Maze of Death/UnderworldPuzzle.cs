using Server.Gumps;
using Server.Network;
using System;

namespace Server.Items
{
    public class UnderworldPuzzleItem : BaseDecayingItem
    {
        public UnderworldPuzzleSolution Solution { get; }
        public UnderworldPuzzleSolution CurrentSolution { get; set; }

        public override int LabelNumber => 1113379;  // Puzzle Board

        [CommandProperty(AccessLevel.GameMaster)]
        public int Attempts { get; set; }

        public override int Lifespan => 1800;

        public override bool UseSeconds => false;

        [Constructable]
        public UnderworldPuzzleItem()
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
            Weight = 5.0;
            Hue = 0x281;

            Attempts = 0;

            Solution = new UnderworldPuzzleSolution();
            CurrentSolution = new UnderworldPuzzleSolution(Solution.Index);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            else
            {
                from.CloseGump(typeof(UnderworldPuzzleGump));
                from.SendGump(new UnderworldPuzzleGump(from, this));
            }
        }

        public bool SubmitSolution(Mobile m, UnderworldPuzzleSolution solution)
        {
            if (m == null)
                return false;

            if (solution.Matches(Solution))
            {
                Item item = Loot.Construct(m_Rewards[Utility.Random(m_Rewards.Length)]);

                if (item != null)
                {
                    if (item is VoidEssence || item is SilverSerpentVenom || item is ToxicVenomSac)
                        item.Amount = 30;

                    if (item is LuckyCoin)
                        item.Amount = Utility.RandomMinMax(2, 6);

                    if (m.Backpack == null || !m.Backpack.TryDropItem(m, item, false))
                        m.BankBox.DropItem(item);
                }

                m.PlaySound(0x3D);
                m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1113579, m.NetState); // Correct Code Entered. Crystal Lock Disengaged.

                Delete();
                return true;
            }
            else
            {
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1150177, m.NetState); // Incorrect Code Sequence. Access Denied.
                return false;
            }
        }

        private readonly Type[] m_Rewards =
        {
            typeof(VoidEssence),        typeof(SilverSerpentVenom), typeof(ScouringToxin),
            typeof(ToxicVenomSac),      typeof(MouldingBoard),      typeof(DoughBowl),
            typeof(HornedTotemPole),    typeof(LargeSquarePillow),  typeof(LargeDiamondPillow),
            typeof(DustyPillow),        typeof(StatuePedestal),		typeof(FlouredBreadBoard),
            typeof(LuckyCoin),
        };

        public override void OnDelete()
        {
            base.OnDelete();

            if (RootParent is Mobile m)
                m.CloseGump(typeof(UnderworldPuzzleGump));
        }

        public UnderworldPuzzleItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Delete();
        }
    }

    public enum PuzzlePiece
    {
        None,
        RedSingle,
        RedDouble,
        RedTriple,
        RedBar,
        BlueSingle,
        BlueDouble,
        BlueTriple,
        BlueBar,
        GreenSingle,
        GreenDouble,
        GreenTriple,
        GreenBar
    }

    public enum PuzzleColor
    {
        Red,
        Blue,
        Green
    }

    public class UnderworldPuzzleSolution
    {
        public const int Length = 4;

        public PuzzlePiece[] Rows { get; } = new PuzzlePiece[Length];
        public int Index { get; private set; }
        public int MaxAttempts { get; private set; }

        public PuzzlePiece First { get { return Rows[0]; } set { Rows[0] = value; } }
        public PuzzlePiece Second { get { return Rows[1]; } set { Rows[1] = value; } }
        public PuzzlePiece Third { get { return Rows[2]; } set { Rows[2] = value; } }
        public PuzzlePiece Fourth { get { return Rows[3]; } set { Rows[3] = value; } }

        public UnderworldPuzzleSolution()
        {
            PickRandom();
        }

        public UnderworldPuzzleSolution(int index)
        {
            LoadStartSolution(index);
        }

        public UnderworldPuzzleSolution(PuzzlePiece first, PuzzlePiece second, PuzzlePiece third, PuzzlePiece fourth)
        {
            First = first;
            Second = second;
            Third = third;
            Fourth = fourth;
        }

        public bool Matches(UnderworldPuzzleSolution check)
        {
            return GetMatches(check) >= 4;
        }

        public int GetMatches(UnderworldPuzzleSolution check)
        {
            int matches = 0;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] == check.Rows[i])
                    matches++;
            }

            return matches;
        }

        public void PickRandom()
        {
            Index = Utility.Random(16);
            switch (Index)
            {
                case 0: //Good To Go
                    First = PuzzlePiece.RedSingle;
                    Second = PuzzlePiece.BlueSingle;
                    Third = PuzzlePiece.RedSingle;
                    Fourth = PuzzlePiece.GreenSingle;
                    MaxAttempts = 6;
                    break;
                case 1: //Good To Go
                    First = PuzzlePiece.GreenDouble;
                    Second = PuzzlePiece.RedBar;
                    Third = PuzzlePiece.None;
                    Fourth = PuzzlePiece.BlueTriple;
                    MaxAttempts = 6;
                    break;
                case 2: //Good To Go
                    First = PuzzlePiece.None;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.RedBar;
                    Fourth = PuzzlePiece.RedTriple;
                    MaxAttempts = 4;
                    break;
                case 3: //Good To Go
                    First = PuzzlePiece.BlueDouble;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.GreenDouble;
                    Fourth = PuzzlePiece.GreenDouble;
                    MaxAttempts = 7;
                    break;
                case 4: //Good To Go
                    First = PuzzlePiece.BlueSingle;
                    Second = PuzzlePiece.GreenSingle;
                    Third = PuzzlePiece.GreenDouble;
                    Fourth = PuzzlePiece.RedBar;
                    MaxAttempts = 7;
                    break;
                case 5: //Good To Go
                    First = PuzzlePiece.GreenDouble;
                    Second = PuzzlePiece.BlueBar;
                    Third = PuzzlePiece.RedSingle;
                    Fourth = PuzzlePiece.BlueSingle;
                    MaxAttempts = 8;
                    break;
                case 6: //Good To Go
                    First = PuzzlePiece.GreenSingle;
                    Second = PuzzlePiece.RedSingle;
                    Third = PuzzlePiece.BlueDouble;
                    Fourth = PuzzlePiece.GreenBar;
                    MaxAttempts = 5;
                    break;
                case 7: //Good To Go
                    First = PuzzlePiece.BlueDouble;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.BlueTriple;
                    Fourth = PuzzlePiece.None;
                    MaxAttempts = 4;
                    break;
                case 8: //Good To Go
                    First = PuzzlePiece.GreenSingle;
                    Second = PuzzlePiece.GreenBar;
                    Third = PuzzlePiece.RedDouble;
                    Fourth = PuzzlePiece.RedSingle;
                    MaxAttempts = 7;
                    break;
                case 9: //Good to Go
                    First = PuzzlePiece.BlueSingle;
                    Second = PuzzlePiece.GreenDouble;
                    Third = PuzzlePiece.None;
                    Fourth = PuzzlePiece.GreenTriple;
                    MaxAttempts = 6;
                    break;
                case 10: //Good To Go
                    First = PuzzlePiece.BlueSingle;
                    Second = PuzzlePiece.RedSingle;
                    Third = PuzzlePiece.RedTriple;
                    Fourth = PuzzlePiece.None;
                    MaxAttempts = 6;
                    break;
                case 11: //Good To Go
                    First = PuzzlePiece.GreenSingle;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.GreenTriple;
                    Fourth = PuzzlePiece.GreenDouble;
                    MaxAttempts = 5;
                    break;
                case 12: //Good to Go
                    First = PuzzlePiece.RedTriple;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.None;
                    Fourth = PuzzlePiece.RedTriple;
                    MaxAttempts = 6;
                    break;
                case 13: //Good To Go
                    First = PuzzlePiece.None;
                    Second = PuzzlePiece.BlueTriple;
                    Third = PuzzlePiece.GreenSingle;
                    Fourth = PuzzlePiece.BlueDouble;
                    MaxAttempts = 7;
                    break;
                case 14: //Good To Go
                    First = PuzzlePiece.BlueTriple;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.BlueTriple;
                    Fourth = PuzzlePiece.None;
                    MaxAttempts = 6;
                    break;
                case 15: //Good to Go
                    First = PuzzlePiece.RedSingle;
                    Second = PuzzlePiece.BlueDouble;
                    Third = PuzzlePiece.RedDouble;
                    Fourth = PuzzlePiece.GreenSingle;
                    MaxAttempts = 6;
                    break;
            }
        }

        public void LoadStartSolution(int index)
        {
            Index = index;

            switch (index)
            {
                case 0:
                    First = PuzzlePiece.RedSingle;
                    Second = PuzzlePiece.RedSingle;
                    Third = PuzzlePiece.RedSingle;
                    Fourth = PuzzlePiece.RedSingle;
                    break;
                case 1:
                    First = PuzzlePiece.BlueBar;
                    Second = PuzzlePiece.RedDouble;
                    Third = PuzzlePiece.RedBar;
                    Fourth = PuzzlePiece.BlueSingle;
                    break;
                case 2:
                    First = PuzzlePiece.GreenBar;
                    Second = PuzzlePiece.BlueDouble;
                    Third = PuzzlePiece.RedTriple;
                    Fourth = PuzzlePiece.RedBar;
                    break;
                case 3:
                    First = PuzzlePiece.RedSingle;
                    Second = PuzzlePiece.GreenDouble;
                    Third = PuzzlePiece.BlueTriple;
                    Fourth = PuzzlePiece.RedBar;
                    break;
                case 4:
                    First = PuzzlePiece.GreenSingle;
                    Second = PuzzlePiece.RedBar;
                    Third = PuzzlePiece.RedDouble;
                    Fourth = PuzzlePiece.GreenSingle;
                    break;
                case 5:
                    First = PuzzlePiece.RedBar;
                    Second = PuzzlePiece.GreenBar;
                    Third = PuzzlePiece.RedBar;
                    Fourth = PuzzlePiece.BlueBar;
                    break;
                case 6:
                    First = PuzzlePiece.RedBar;
                    Second = PuzzlePiece.GreenDouble;
                    Third = PuzzlePiece.GreenDouble;
                    Fourth = PuzzlePiece.RedBar;
                    break;
                case 7:
                    First = PuzzlePiece.GreenBar;
                    Second = PuzzlePiece.BlueSingle;
                    Third = PuzzlePiece.BlueTriple;
                    Fourth = PuzzlePiece.RedSingle;
                    break;
                case 8:
                    First = PuzzlePiece.GreenBar;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.GreenTriple;
                    Fourth = PuzzlePiece.GreenSingle;
                    break;
                case 9:
                    First = PuzzlePiece.RedBar;
                    Second = PuzzlePiece.RedBar;
                    Third = PuzzlePiece.RedBar;
                    Fourth = PuzzlePiece.RedBar;
                    break;
                case 10:
                    First = PuzzlePiece.RedSingle;
                    Second = PuzzlePiece.None;
                    Third = PuzzlePiece.BlueDouble;
                    Fourth = PuzzlePiece.RedBar;
                    break;
                case 11:
                    First = PuzzlePiece.RedDouble;
                    Second = PuzzlePiece.GreenDouble;
                    Third = PuzzlePiece.RedDouble;
                    Fourth = PuzzlePiece.BlueDouble;
                    break;
                case 12:
                    First = PuzzlePiece.GreenTriple;
                    Second = PuzzlePiece.BlueBar;
                    Third = PuzzlePiece.GreenTriple;
                    Fourth = PuzzlePiece.RedBar;
                    break;
                case 13:
                    First = PuzzlePiece.RedSingle;
                    Second = PuzzlePiece.GreenBar;
                    Third = PuzzlePiece.GreenBar;
                    Fourth = PuzzlePiece.RedSingle;
                    break;
                case 14:
                    First = PuzzlePiece.GreenTriple;
                    Second = PuzzlePiece.GreenBar;
                    Third = PuzzlePiece.BlueBar;
                    Fourth = PuzzlePiece.GreenSingle;
                    break;
                case 15:
                    First = PuzzlePiece.BlueTriple;
                    Second = PuzzlePiece.GreenDouble;
                    Third = PuzzlePiece.BlueSingle;
                    Fourth = PuzzlePiece.RedDouble;
                    break;
            }
        }
    }

    public class UnderworldPuzzleGump : Gump
    {
        private readonly Mobile m_From;
        private readonly UnderworldPuzzleItem m_Item;
        private readonly UnderworldPuzzleSolution m_Solution;
        private readonly UnderworldPuzzleSolution m_CurrentSolution;
        private int m_Row;

        public UnderworldPuzzleGump(Mobile from, UnderworldPuzzleItem item)
            : this(from, item, 0)
        {
        }

        public UnderworldPuzzleGump(Mobile from, UnderworldPuzzleItem item, int row)
            : base(5, 30)
        {
            if (row > 3) row = 3;
            if (row < 0) row = 0;

            m_From = from;
            m_Item = item;
            m_Row = row;

            m_Solution = item.Solution;
            m_CurrentSolution = item.CurrentSolution;

            AddBackground( 55, 45, 500, 200, 0x2422 );
			AddImage( 75, 83, 0x2423 );
			AddImage( 65, 118, 0x2423 );
			AddImage( 75, 153, 0x2423 );
			AddImage( 65, 188, 0x2423 );
			AddImage( 108, 55, 0x2427 );
			AddImage( 86, 65, 0x2427 );
			AddBackground( 75, 65, 86, 153, 0x2422 );
			AddBackground( 192, 65, 137, 153, 0x2422 );
			AddBackground( 397, 65, 137, 153, 0x2422 );
			AddBackground( 55, 270, 195, 110, 0x2422 );
			AddImage( 205, 77, 0x52 );
			AddImage( 205, 110, 0x52 );
			AddImage( 205, 143, 0x52 );
			AddImage( 410, 77, 0x52 );
			AddImage( 410, 110, 0x52 );
			AddImage( 410, 143, 0x52 );
			AddImage( 5, 5, 0x28C8 );

            AddButton(160, 320, 0xF2, 0xF1, 8, GumpButtonType.Reply, 0); // Cancel
            AddButton(80, 320, 0xEF, 0xF0, 7, GumpButtonType.Reply, 0); // Apply
            AddButton(120, 345, 0x7DB, 0x7DB, 0, GumpButtonType.Reply, 0);	// Log out

            AddHtmlLocalized(72, 285, 170, 20, 1150180, false, false); // Command Functions: 
            AddHtml(200, 285, 100, 20, string.Format("{0}/{1}", m_Item.Attempts, m_Solution.MaxAttempts), false, false);

            if (from.Skills[SkillName.Lockpicking].Base >= 100.0)
            {
                int locked = m_Solution.GetMatches(m_CurrentSolution);
                AddHtmlLocalized(72, 300, 170, 20, 1150179, false, false); // Crystals Locked  : 
                AddHtml(200, 300, 100, 20, locked.ToString(), false, false);
            }

            AddButton(108, 82, row == 0 ? 208 : 209, row == 0 ? 209 : 208, 1, GumpButtonType.Reply, 0);
            AddButton(108, 115, row == 1 ? 208 : 209, row == 0 ? 209 : 208, 2, GumpButtonType.Reply, 0);
            AddButton(108, 148, row == 2 ? 208 : 209, row == 0 ? 209 : 208, 3, GumpButtonType.Reply, 0);
            AddButton(108, 181, row == 3 ? 208 : 209, row == 0 ? 209 : 208, 4, GumpButtonType.Reply, 0);

            AddPiece(0, true, m_Solution.First);
            AddPiece(1, true, m_Solution.Second);
            AddPiece(2, true, m_Solution.Third);
            AddPiece(3, true, m_Solution.Fourth);

            AddPiece(0, false, m_CurrentSolution.First);
            AddPiece(1, false, m_CurrentSolution.Second);
            AddPiece(2, false, m_CurrentSolution.Third);
            AddPiece(3, false, m_CurrentSolution.Fourth);

            for (int i = 0; i < 4; i++)
            {
                if (i == row && m_CurrentSolution.Rows[i] != PuzzlePiece.None && m_Item.Attempts < m_Solution.MaxAttempts)
                {
                    AddButton(88, 82 + (i * 33), 2650, 2650, 5, GumpButtonType.Reply, 0); //Up
                    AddButton(128, 82 + (i * 33), 2648, 2648, 6, GumpButtonType.Reply, 0); //Down
                }
                else
                {
                    AddImage(88, 82 + (i * 33), 2709);
                    AddImage(128, 82 + (i * 33), 2709);
                }
            }

            AddButton(88, 82 + (row * 35), 2650, 2650, 5, GumpButtonType.Reply, 0); //Up
            AddButton(128, 82 + (row * 35), 2648, 2648, 6, GumpButtonType.Reply, 0); //Down
        }

        private void AddPiece(int row, bool right, PuzzlePiece piece)
        {
            int id = GetPuzzlePieceID(piece);
            int x = right ? 410 : 205;
            int y = 76 + (33 * row);

            switch (piece)
            {
                case PuzzlePiece.None:
                    break;
                case PuzzlePiece.RedSingle:
                case PuzzlePiece.BlueSingle:
                case PuzzlePiece.GreenSingle:
                    AddImage(x + 40, y, id);
                    break;
                case PuzzlePiece.RedDouble:
                case PuzzlePiece.BlueDouble:
                case PuzzlePiece.GreenDouble:
                    AddImage(x, y, id);
                    AddImage(x + 80, y, id);
                    break;
                case PuzzlePiece.RedTriple:
                case PuzzlePiece.BlueTriple:
                case PuzzlePiece.GreenTriple:
                    AddImage(x, y, id);
                    AddImage(x + 40, y, id);
                    AddImage(x + 80, y, id);
                    break;
                case PuzzlePiece.RedBar:
                case PuzzlePiece.BlueBar:
                case PuzzlePiece.GreenBar:
                    AddImage(x, y, id);
                    break;
            }

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Item.Deleted || info.ButtonID == 0 || !m_From.CheckAlive())
                return;

            if (m_From.AccessLevel == AccessLevel.Player && !m_Item.IsChildOf(m_From.Backpack))
            {
                m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500446); // That is too far away.
                return;
            }

            switch (info.ButtonID)
            {
                case 0: break;
                default:
                    {
                        m_Row = info.ButtonID - 1;
                        break;
                    }
                case 5:
                    {
                        int nextRow = m_Row - 1;
                        if (nextRow < 0) nextRow = 3;

                        PuzzlePiece movingPiece = m_CurrentSolution.Rows[m_Row];
                        PuzzlePiece movingToPiece = m_CurrentSolution.Rows[nextRow];

                        //Can't move empty spaces
                        if (movingPiece == PuzzlePiece.None || m_Item.Attempts >= m_Solution.MaxAttempts)
                            break;

                        SplitPiecesUp(ref movingPiece, ref movingToPiece);

                        if (movingPiece != m_CurrentSolution.Rows[m_Row] || movingToPiece != m_CurrentSolution.Rows[nextRow])
                        {
                            m_CurrentSolution.Rows[m_Row] = movingPiece;
                            m_CurrentSolution.Rows[nextRow] = movingToPiece;

                            m_Item.CurrentSolution = m_CurrentSolution;
                            m_Item.Attempts++;
                        }
                        break;
                    }
                case 6:
                    {
                        int nextRow = m_Row + 1;
                        if (nextRow > 3) nextRow = 0;

                        PuzzlePiece movingPiece = m_CurrentSolution.Rows[m_Row];
                        PuzzlePiece movingToPiece = m_CurrentSolution.Rows[nextRow];

                        //Can't Move Empty Spaces
                        if (movingPiece == PuzzlePiece.None || m_Item.Attempts >= m_Solution.MaxAttempts)
                            break;

                        SplitPiecesDown(ref movingPiece, ref movingToPiece);

                        if (movingPiece != m_CurrentSolution.Rows[m_Row] || movingToPiece != m_CurrentSolution.Rows[nextRow])
                        {
                            m_CurrentSolution.Rows[m_Row] = movingPiece;
                            m_CurrentSolution.Rows[nextRow] = movingToPiece;

                            m_Item.Attempts++;

                            m_Item.CurrentSolution = m_CurrentSolution;
                        }
                        break;
                    }
                case 7:
                    {
                        if (m_Item.SubmitSolution(m_From, m_CurrentSolution))
                            return;
                        break;
                    }
                case 8:
                    {
                        int index = m_Item.Solution.Index;
                        m_Item.CurrentSolution = new UnderworldPuzzleSolution(index);
                        m_Item.Attempts = 0;
                        break;
                    }
            }

            m_From.SendGump(new UnderworldPuzzleGump(m_From, m_Item, m_Row));
        }

        private int GetTotalPieces(PuzzlePiece piece)
        {
            switch (piece)
            {
                default:
                    return 0;
                case PuzzlePiece.RedSingle: case PuzzlePiece.BlueSingle: case PuzzlePiece.GreenSingle: return 1;
                case PuzzlePiece.RedDouble: case PuzzlePiece.BlueDouble: case PuzzlePiece.GreenDouble: return 2;
                case PuzzlePiece.RedTriple: case PuzzlePiece.BlueTriple: case PuzzlePiece.GreenTriple: return 3;
                case PuzzlePiece.RedBar: case PuzzlePiece.BlueBar: case PuzzlePiece.GreenBar: return 4;
            }
        }

        private void SplitPiecesUp(ref PuzzlePiece movingPiece, ref PuzzlePiece movingToPiece)
        {
            int movingAmount = GetTotalPieces(movingPiece);
            int moveToAmount = GetTotalPieces(movingToPiece);

            if (movingToPiece == PuzzlePiece.None)
            {
                if ((movingAmount == 2 || movingAmount == 4))
                {
                    switch (movingPiece)
                    {
                        case PuzzlePiece.RedDouble:
                            movingToPiece = PuzzlePiece.BlueSingle;
                            movingPiece = PuzzlePiece.GreenSingle;
                            break;
                        case PuzzlePiece.BlueDouble:
                            movingToPiece = PuzzlePiece.GreenSingle;
                            movingPiece = PuzzlePiece.RedSingle;
                            break;
                        case PuzzlePiece.GreenDouble:
                            movingToPiece = PuzzlePiece.RedSingle;
                            movingPiece = PuzzlePiece.BlueSingle;
                            break;
                        case PuzzlePiece.RedBar:
                            movingToPiece = PuzzlePiece.BlueDouble;
                            movingPiece = PuzzlePiece.GreenDouble;
                            break;
                        case PuzzlePiece.BlueBar:
                            movingToPiece = PuzzlePiece.GreenDouble;
                            movingPiece = PuzzlePiece.RedDouble;
                            break;
                        case PuzzlePiece.GreenBar:
                            movingToPiece = PuzzlePiece.RedDouble;
                            movingPiece = PuzzlePiece.BlueDouble;
                            break;
                    }
                }

                return;
            }

            if (movingAmount + moveToAmount > 4)
            {
                PuzzlePiece movingTemp = movingPiece;
                PuzzlePiece movingToTemp = movingToPiece;

                movingPiece = movingToTemp;
                movingToPiece = movingTemp;

                return;
            }

            movingToPiece = CombinePieces(movingPiece, movingToPiece);
            movingPiece = PuzzlePiece.None;
        }

        private void SplitPiecesDown(ref PuzzlePiece movingPiece, ref PuzzlePiece movingToPiece)
        {
            int movingAmount = GetTotalPieces(movingPiece);
            int moveToAmount = GetTotalPieces(movingToPiece);

            if (movingToPiece == PuzzlePiece.None)
            {
                if ((movingAmount == 2 || movingAmount == 4))
                {
                    switch (movingPiece)
                    {
                        case PuzzlePiece.RedDouble:
                            movingToPiece = PuzzlePiece.BlueSingle;
                            movingPiece = PuzzlePiece.GreenSingle;
                            break;
                        case PuzzlePiece.BlueDouble:
                            movingToPiece = PuzzlePiece.GreenSingle;
                            movingPiece = PuzzlePiece.RedSingle;
                            break;
                        case PuzzlePiece.GreenDouble:
                            movingToPiece = PuzzlePiece.RedSingle;
                            movingPiece = PuzzlePiece.BlueSingle;
                            break;
                        case PuzzlePiece.RedBar:
                            movingToPiece = PuzzlePiece.BlueDouble;
                            movingPiece = PuzzlePiece.GreenDouble;
                            break;
                        case PuzzlePiece.BlueBar:
                            movingToPiece = PuzzlePiece.GreenDouble;
                            movingPiece = PuzzlePiece.RedDouble;
                            break;
                        case PuzzlePiece.GreenBar:
                            movingToPiece = PuzzlePiece.RedDouble;
                            movingPiece = PuzzlePiece.BlueDouble;
                            break;
                    }
                }

                return;
            }

            int t = 0;
            //PuzzlePiece greater = movingPiece;

            if (moveToAmount > movingAmount)
            {
                //greater = movingToPiece;
                t = (int)movingToPiece - movingAmount;
            }
            else if (movingAmount > moveToAmount)
            {
                //greater = movingPiece;
                //t = (int)movingToPiece + (movingAmount - moveToAmount);
                //t -= 1;
                if (movingAmount == 4 && moveToAmount == 3)
                    t = (int)movingToPiece - 2;
                else if (movingAmount == 4 && moveToAmount == 2)
                    t = (int)movingToPiece;
                else if (movingAmount == 4 && moveToAmount == 1)
                    t = (int)movingToPiece + 2;

                else if (movingAmount == 2 && moveToAmount == 1)
                    t = (int)movingToPiece;
                else if (movingAmount == 3 && moveToAmount == 2)
                    t = (int)movingToPiece - 1;
                else if (movingAmount == 3 && moveToAmount == 1)
                    t = (int)movingToPiece + 1;
            }
            else
            {
                movingPiece = PuzzlePiece.None;
                movingToPiece = PuzzlePiece.None;
                return;
            }

            movingToPiece = (PuzzlePiece)t;
            movingPiece = PuzzlePiece.None;
        }

        private PuzzlePiece CombinePieces(PuzzlePiece moving, PuzzlePiece movingInto)
        {
            int movingAmount = GetTotalPieces(moving);
            int moveToAmount = GetTotalPieces(movingInto);
            int combined = movingAmount + moveToAmount;

            if (movingAmount != moveToAmount)
            {
                int t = (int)movingInto + movingAmount; //combined + (int)moving;
                return (PuzzlePiece)t;
            }

            combined--;

            PuzzleColor movingCol = GetColor(moving);
            PuzzleColor movingIntoCol = GetColor(movingInto);

            switch (movingCol)
            {
                case PuzzleColor.Red:
                    switch (movingIntoCol)
                    {
                        case PuzzleColor.Red:
                            return PuzzlePiece.RedSingle + combined;
                        case PuzzleColor.Blue:
                            return PuzzlePiece.GreenSingle + combined;
                        case PuzzleColor.Green:
                            return PuzzlePiece.BlueSingle + combined;
                    }
                    break;
                case PuzzleColor.Blue:
                    switch (movingIntoCol)
                    {
                        case PuzzleColor.Red:
                            return PuzzlePiece.GreenSingle + combined;
                        case PuzzleColor.Blue:
                            return PuzzlePiece.BlueSingle + combined;
                        case PuzzleColor.Green:
                            return PuzzlePiece.RedSingle + combined;
                    }
                    break;
                case PuzzleColor.Green:
                    switch (movingIntoCol)
                    {
                        case PuzzleColor.Red:
                            return PuzzlePiece.BlueSingle + combined;
                        case PuzzleColor.Blue:
                            return PuzzlePiece.RedSingle + combined;
                        case PuzzleColor.Green:
                            return PuzzlePiece.GreenSingle + combined;
                    }
                    break;
            }

            return moving;
        }

        private PuzzleColor GetColor(PuzzlePiece piece)
        {
            switch (piece)
            {
                default:
                case PuzzlePiece.RedSingle:
                case PuzzlePiece.RedDouble:
                case PuzzlePiece.RedTriple:
                case PuzzlePiece.RedBar:
                    return PuzzleColor.Red;
                case PuzzlePiece.BlueSingle:
                case PuzzlePiece.BlueDouble:
                case PuzzlePiece.BlueTriple:
                case PuzzlePiece.BlueBar:
                    return PuzzleColor.Blue;
                case PuzzlePiece.GreenSingle:
                case PuzzlePiece.GreenDouble:
                case PuzzlePiece.GreenTriple:
                case PuzzlePiece.GreenBar:
                    return PuzzleColor.Green;
            }
        }

        private int GetPuzzlePieceID(PuzzlePiece piece)
        {
            switch (piece)
            {
                default:
                case PuzzlePiece.None: return 0x0;
                case PuzzlePiece.RedSingle:
                case PuzzlePiece.RedDouble:
                case PuzzlePiece.RedTriple: return 0x2A62;
                case PuzzlePiece.BlueSingle:
                case PuzzlePiece.BlueDouble:
                case PuzzlePiece.BlueTriple: return 0x2A3A;
                case PuzzlePiece.GreenSingle:
                case PuzzlePiece.GreenDouble:
                case PuzzlePiece.GreenTriple: return 0x2A4E;
                case PuzzlePiece.RedBar: return 0x2A58;
                case PuzzlePiece.BlueBar: return 0x2A30;
                case PuzzlePiece.GreenBar: return 0x2A44;
            }
        }
    }
}
