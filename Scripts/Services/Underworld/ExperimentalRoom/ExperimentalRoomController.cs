using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ExperimentalRoomController : Item
    {
        private static ExperimentalRoomController m_Instance;
        public static ExperimentalRoomController Instance => m_Instance;

        private static Dictionary<Mobile, DateTime> m_Table;

        public ExperimentalRoomController() : base(7107)
        {
            m_Table = new Dictionary<Mobile, DateTime>();
            Visible = false;
            Movable = false;

            m_Instance = this;
        }

        public static void AddToTable(Mobile from)
        {
            if (from == null)
                return;

            m_Table[from] = DateTime.UtcNow + TimeSpan.FromHours(24);
        }

        public static bool IsInCooldown(Mobile from)
        {
            Defrag();

            return m_Table.ContainsKey(from);
        }

        public static void Defrag()
        {
            List<Mobile> list = new List<Mobile>();

            foreach (KeyValuePair<Mobile, DateTime> kvp in m_Table)
            {
                if (kvp.Value <= DateTime.UtcNow)
                    list.Add(kvp.Key);
            }

            foreach (Mobile m in list)
                m_Table.Remove(m);
        }

        public static bool HasItem(Mobile from, Type type)
        {
            if (from == null || from.Backpack == null)
                return false;

            Item item = from.Backpack.FindItemByType(type);

            return item != null;
        }

        public ExperimentalRoomController(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            Defrag();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Table = new Dictionary<Mobile, DateTime>();

            m_Instance = this;
        }
    }
}