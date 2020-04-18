using System;

namespace Server.Items
{
    public interface ITalismanProtection
    {
        TalismanAttribute Protection { get; set; }
        void SetProtection(Type type, TextDefinition name, int amount);
    }

    public interface ITalismanKiller
    {
        TalismanAttribute Killer { get; set; }
        void SetKiller(Type type, TextDefinition name, int amount);
    }

    [PropertyObject]
    public class TalismanAttribute
    {
        private Type m_Type;
        private TextDefinition m_Name;
        private int m_Amount;
        public TalismanAttribute()
            : this(null, 0, 0)
        {
        }

        public TalismanAttribute(TalismanAttribute copy)
        {
            if (copy != null)
            {
                m_Type = copy.Type;
                m_Name = copy.Name;
                m_Amount = copy.Amount;
            }
        }

        public TalismanAttribute(Type type, TextDefinition name)
            : this(type, name, 0)
        {
        }

        public TalismanAttribute(Type type, TextDefinition name, int amount)
        {
            m_Type = type;
            m_Name = name;
            m_Amount = amount;
        }

        public TalismanAttribute(GenericReader reader)
        {
            int version = reader.ReadInt();

            SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

            if (GetSaveFlag(flags, SaveFlag.Type))
                m_Type = ScriptCompiler.FindTypeByFullName(reader.ReadString(), false);

            if (GetSaveFlag(flags, SaveFlag.Name))
                m_Name = TextDefinition.Deserialize(reader);

            if (GetSaveFlag(flags, SaveFlag.Amount))
                m_Amount = reader.ReadEncodedInt();
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Type = 0x00000001,
            Name = 0x00000002,
            Amount = 0x00000004,
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Type Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Amount
        {
            get
            {
                return m_Amount;
            }
            set
            {
                m_Amount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEmpty => m_Type == null;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsItem => m_Type != null && m_Type.Namespace.Equals("Server.Items");
        public override string ToString()
        {
            if (m_Type != null)
                return m_Type.Name;

            return "None";
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0); // version

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.Type, m_Type != null);
            SetSaveFlag(ref flags, SaveFlag.Name, m_Name != null);
            SetSaveFlag(ref flags, SaveFlag.Amount, m_Amount != 0);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Type))
                writer.Write(m_Type.FullName);

            if (GetSaveFlag(flags, SaveFlag.Name))
                TextDefinition.Serialize(writer, m_Name);

            if (GetSaveFlag(flags, SaveFlag.Amount))
                writer.WriteEncodedInt(m_Amount);
        }

        public int DamageBonus(Mobile to)
        {
            if (to != null && to.GetType() == m_Type)
                return m_Amount;

            return 0;
        }

        public int ScaleDamage(Mobile from, int damage)
        {
            if (from != null && (from.GetType() == m_Type || (m_Type != null && from.GetType().IsSubclassOf(m_Type))))
                return (int)(damage * (1 - m_Amount / 100.0));

            return damage;
        }

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }
    }
}