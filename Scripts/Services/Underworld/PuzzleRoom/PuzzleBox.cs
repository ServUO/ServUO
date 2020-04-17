using Server.Mobiles;
using System;

namespace Server.Items
{
    public enum PuzzleType
    {
        WestBox,   // Maze #1
        EastBox,   // Maze #2
        NorthBox   // Mastermind
    }

    public class PuzzleBox : Item
    {
        private PuzzleType m_PuzzleType;

        [CommandProperty(AccessLevel.GameMaster)]
        public PuzzleType PuzzleType { get { return m_PuzzleType; } set { m_PuzzleType = value; } }

        public override int LabelNumber => 1113486;  // a puzzle box

        [Constructable]
        public PuzzleBox(PuzzleType type) : base(2472)
        {
            m_PuzzleType = type;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(Location, 3))
                from.SendLocalizedMessage(3000268); // that is too far away.
            else if (from.Backpack != null)
            {
                Type needed;
                Item puzzle = null;

                Item key = from.Backpack.FindItemByType(typeof(MagicKey));
                Item puzzleItem1 = from.Backpack.FindItemByType(typeof(MazePuzzleItem));
                Item puzzleItem2 = from.Backpack.FindItemByType(typeof(MastermindPuzzleItem));

                if (key == null)
                {
                    int x = Utility.RandomMinMax(1095, 1099);
                    int y = Utility.RandomMinMax(1178, 1179);
                    int z = -1;

                    Point3D loc = from.Location;
                    Point3D p = new Point3D(x, y, z);
                    BaseCreature.TeleportPets(from, p, Map.TerMur);
                    from.MoveToWorld(p, Map.TerMur);

                    from.PlaySound(0x1FE);
                    Effects.SendLocationParticles(EffectItem.Create(loc, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(p, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
                }

                if (puzzleItem1 != null || puzzleItem2 != null)
                {
                    from.SendMessage("You already have a puzzle board.");
                    return;
                }

                switch (m_PuzzleType)
                {
                    default:
                    case PuzzleType.WestBox:
                    case PuzzleType.EastBox: needed = typeof(MagicKey); break;
                    case PuzzleType.NorthBox: needed = typeof(GoldPuzzleKey); break;
                }

                Item item = from.Backpack.FindItemByType(needed);

                if (item != null && key is MagicKey)
                {
                    if (m_PuzzleType == PuzzleType.NorthBox)
                        puzzle = new MastermindPuzzleItem((MagicKey)key);
                    else
                        puzzle = new MazePuzzleItem((MagicKey)key);
                }

                if (puzzle != null)
                {
                    if (!from.Backpack.TryDropItem(from, puzzle, true))
                        puzzle.Delete();
                    else
                        from.SendMessage("You recieve a puzzle board.");
                }
                else
                    from.SendMessage("You do not have the required key to get that puzzle board.");
            }
        }

        public PuzzleBox(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // ver
            writer.Write((int)m_PuzzleType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_PuzzleType = (PuzzleType)reader.ReadInt();
        }
    }
}