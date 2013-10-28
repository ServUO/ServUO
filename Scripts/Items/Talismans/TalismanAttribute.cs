using System;

namespace Server.Items
{
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
                this.m_Type = copy.Type;
                this.m_Name = copy.Name;
                this.m_Amount = copy.Amount;
            }
        }

        public TalismanAttribute(Type type, TextDefinition name)
            : this(type, name, 0)
        {
        }

        public TalismanAttribute(Type type, TextDefinition name, int amount)
        {
            this.m_Type = type;
            this.m_Name = name;
            this.m_Amount = amount;
        }

        public TalismanAttribute(GenericReader reader)
        {
            int version = reader.ReadInt();

            SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

            if (GetSaveFlag(flags, SaveFlag.Type))
                this.m_Type = ScriptCompiler.FindTypeByFullName(reader.ReadString(), false);

            if (GetSaveFlag(flags, SaveFlag.Name))
                this.m_Name = TextDefinition.Deserialize(reader);

            if (GetSaveFlag(flags, SaveFlag.Amount))
                this.m_Amount = reader.ReadEncodedInt();
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
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Amount
        {
            get
            {
                return this.m_Amount;
            }
            set
            {
                this.m_Amount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsEmpty
        {
            get
            {
                return this.m_Type == null;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsItem
        {
            get
            {
                return this.m_Type != null && this.m_Type.Namespace.Equals("Server.Items");
            }
        }
        public override string ToString()
        {
            if (this.m_Type != null)
                return this.m_Type.Name;

            return "None";
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); // version

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.Type, this.m_Type != null);
            SetSaveFlag(ref flags, SaveFlag.Name, this.m_Name != null);
            SetSaveFlag(ref flags, SaveFlag.Amount, this.m_Amount != 0);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Type))
                writer.Write(this.m_Type.FullName);

            if (GetSaveFlag(flags, SaveFlag.Name))
                TextDefinition.Serialize(writer, this.m_Name);

            if (GetSaveFlag(flags, SaveFlag.Amount))
                writer.WriteEncodedInt(this.m_Amount);
        }

        public int DamageBonus(Mobile to)
        {
            if (to != null && to.GetType() == this.m_Type)
                return this.m_Amount;

            return 0;
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