using System;
using Server;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Items
{
    public class QuestHintItem : Item
    {
        private int m_Number;
        private string m_String;
        private int m_Range;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number { get { return m_Number; } set { m_Number = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string String { get { return m_String; } set { m_String = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Range { get { return m_Range; } set { m_Range = value; } }

        public virtual Type QuestType { get { return null; } }
        public virtual Type QuestItemType { get { return null; } }
        public virtual int DefaultRange { get { return 8; } }

        [Constructable]
        public QuestHintItem(int num)
            : base(7108)
        {
            Visible = false;
            Movable = false;

            m_Number = num;
            m_Range = DefaultRange;

            Name = "Quest Hint Item";
        }

        [Constructable]
        public QuestHintItem(string str)
            : base(7108)
        {
            Visible = false;
            Movable = false;

            m_String = str;
            m_Range = DefaultRange;
        }

        private Dictionary<Mobile, DateTime> m_Table = new Dictionary<Mobile, DateTime>();

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile from, Point3D oldLocation)
        {
            if (!from.Player)
                return;

            if (m_Table.ContainsKey(from) && m_Table[from] < DateTime.Now)
                m_Table.Remove(from);

            if (!m_Table.ContainsKey(from) && from.InRange(this.Location, m_Range))
            {
                if (QuestItemType != null && !FindItem())
                    return;

                if (QuestType != null && QuestHelper.GetQuest((PlayerMobile)from, QuestType) == null)
                    return;

                m_Table[from] = DateTime.Now + TimeSpan.FromMinutes(3);

                if (m_Number > 0)
                    from.SendLocalizedMessage(m_Number);
                else if (m_String != null)
                    from.SendMessage(m_String);
            }
        }

        private bool FindItem()
		{
            IPooledEnumerable eable = this.Map.GetItemsInRange(this.Location, m_Range * 2);
				
			foreach(Item item in eable)
			{
				if(item.GetType() == QuestItemType)
				{
					eable.Free();
					return true;
				}
			}
				
			eable.Free();
			return false;
		}

        public QuestHintItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(m_Range);

            writer.Write(m_Number);
            writer.Write(m_String);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Range = reader.ReadInt();
                    goto case 0;
                case 0:
                    m_Number = reader.ReadInt();
                    m_String = reader.ReadString();
                    break;
            }
            
        }
    }
}