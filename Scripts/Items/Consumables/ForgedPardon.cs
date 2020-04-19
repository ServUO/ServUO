using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ForgedPardon : Item
    {
        public static Dictionary<Mobile, DateTime> Table => m_Table;
        private static readonly Dictionary<Mobile, DateTime> m_Table = new Dictionary<Mobile, DateTime>();

        public override int LabelNumber => 1116234;

        [Constructable]
        public ForgedPardon()
            : base(10289)
        {
            Hue = 1177;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042004); //That must be in your pack for you to use it.
            else if (from.Kills <= 0)
                from.SendMessage("You have no use for this item.");
            else if (CanUsePardon(from))
            {
                from.Kills--;

                from.SendLocalizedMessage(1116208); // Your murder count has been successfully updated.
                Delete();
                m_Table[from] = DateTime.UtcNow + TimeSpan.FromHours(24);
            }
        }

        public static bool CanUsePardon(Mobile from)
        {
            Defrag();
            if (m_Table.ContainsKey(from))
            {
                from.SendLocalizedMessage(1116587); //You must wait 24 hours before using another forged pardon.
                return false;
            }
            if (Spells.SpellHelper.CheckCombat(from))
            {
                from.SendLocalizedMessage(1116588); //You cannot use a forged pardon while in combat.
                return false;
            }
            return true;
        }

        public static void Defrag()
        {
            List<Mobile> toRemove = new List<Mobile>();
            foreach (KeyValuePair<Mobile, DateTime> kvp in m_Table)
            {
                if (kvp.Value < DateTime.UtcNow)
                    toRemove.Add(kvp.Key);
            }

            foreach (Mobile mob in toRemove)
                m_Table.Remove(mob);
        }

        public ForgedPardon(Serial serial)
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
            int version = reader.ReadInt();
        }

        public static void Save(GenericWriter writer)
        {
            writer.Write(0);

            Defrag();
            writer.Write(m_Table.Count);
            foreach (KeyValuePair<Mobile, DateTime> kvp in m_Table)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public static void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile();
                DateTime dt = reader.ReadDateTime();

                if (mob != null && dt > DateTime.UtcNow)
                    m_Table.Add(mob, dt);
            }
        }
    }
}
