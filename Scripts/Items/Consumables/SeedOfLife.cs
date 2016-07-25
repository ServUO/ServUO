using System;
using System.Collections.Generic;
using Server;

namespace Server.Items
{
    public class SeedOfLife : Item
    {
        public static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(10.0);

        public override int LabelNumber { get { return 1094937; } } // Seed of Life

        [Constructable]
        public SeedOfLife()
            : base(0x1727)
        {
            this.Weight = 1.0;
            this.Hue = 0x491;
        }

        private static Dictionary<Mobile, Timer> m_CooldownTable = new Dictionary<Mobile, Timer>();

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
            else if (m_CooldownTable.ContainsKey(from))
            {
                Timer resetTimer = m_CooldownTable[from];
                TimeSpan left = resetTimer.Next - DateTime.UtcNow;

                if (left >= TimeSpan.FromMinutes(1.0))
                    from.SendLocalizedMessage(1079265, left.Minutes.ToString()); // You must wait ~1_minutes~ minutes before you can use this item.
                else
                    from.SendLocalizedMessage(1079263, left.Seconds.ToString()); // You must wait ~1_seconds~ seconds before you can use this item.
            }
            else if (from.Hits == from.HitsMax)
            {
                from.SendLocalizedMessage(1049547); // You are already at full health.
            }
            else
            {
                from.SendLocalizedMessage(1095126); // The bitter seed instantly restores some of your health!
                from.PlaySound(0x3C);

                from.Hits += Utility.RandomMinMax(25, 40);

                m_CooldownTable[from] = Timer.DelayCall(Cooldown, new TimerStateCallback<Mobile>(RemoveCooldown), from);

                Delete();
            }
        }

        private static void RemoveCooldown(Mobile m)
        {
            if (m_CooldownTable.ContainsKey(m))
                m_CooldownTable.Remove(m);
        }

        public SeedOfLife(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}