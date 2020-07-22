using Server.Engines.VvV;
using System.Collections.Generic;

namespace Server.Items
{
    [TypeAlias("Server.Engines.VvV.MorphEarrings")]
    public class MorphEarrings : GoldEarrings
    {
        public override int LabelNumber => 1094746; // Morph Earrings

        [Constructable]
        public MorphEarrings()
        {
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                ValidateEquipment((Mobile)parent);
            }
        }

        private void ValidateEquipment(Mobile m)
        {
            if (m == null)
                return;

            Race race = m.Race;
            bool didDrop = false;

            List<Item> list = new List<Item>(m.Items);

            foreach (Item item in list)
            {
                if (!race.ValidateEquipment(item))
                {
                    if (!didDrop)
                    {
                        didDrop = true;
                    }

                    if (m.Backpack == null || !m.Backpack.TryDropItem(m, item, false))
                    {
                        m.BankBox.DropItem(item);
                    }
                }
            }

            ColUtility.Free(list);

            if (didDrop)
            {
                m.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
            }
        }

        public MorphEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && ViceVsVirtueSystem.Enabled)
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
        }
    }
}
