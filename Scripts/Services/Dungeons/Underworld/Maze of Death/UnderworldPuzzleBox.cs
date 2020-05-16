using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class UnderworldPuzzleBox : MetalChest
    {
        private static readonly Dictionary<Mobile, DateTime> m_Table = new Dictionary<Mobile, DateTime>();

        [Constructable]
        public UnderworldPuzzleBox()
        {
            Movable = false;
            ItemID = 3712;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(Location, 3))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (from.IsPlayer() && IsInCooldown(from))
            {
                from.SendLocalizedMessage(1113386); // You are too tired to attempt solving more puzzles at this time.
                return;
            }

            if (from.Backpack != null)
            {
                var item = from.Backpack.FindItemByType(typeof(UnderworldPuzzleItem));

                if (item != null)
                {
                    from.SendLocalizedMessage(501885); // You already own one of those!
                }
                else
                {
                    UnderworldPuzzleItem puzzle = new UnderworldPuzzleItem();

                    from.AddToBackpack(puzzle);
                    from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    puzzle.SendTimeRemainingMessage(from);

                    if (from.AccessLevel == AccessLevel.Player)
                        m_Table[from] = DateTime.UtcNow + TimeSpan.FromHours(24);
                }
            }
        }

        public static bool IsInCooldown(Mobile from)
        {
            if (m_Table.ContainsKey(from))
            {
                if (m_Table[from] < DateTime.UtcNow)
                    m_Table.Remove(from);
            }

            return m_Table.ContainsKey(from);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            from.SendLocalizedMessage(1113513); // You cannot put items there.
            return false;
        }

        public override void DisplayTo(Mobile to)
        {
            to.SendLocalizedMessage(1005213); // You can't do that
        }

        public UnderworldPuzzleBox(Serial serial)
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
        }
    }
}
