using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x27AC, 0x27F7)]
    public class Shuriken : Item, ICraftable, INinjaAmmo
    {
        private int m_UsesRemaining;
        private Poison m_Poison;
        private int m_PoisonCharges;
        [Constructable]
        public Shuriken()
            : this(1)
        {
        }

        [Constructable]
        public Shuriken(int amount)
            : base(0x27AC)
        {
            this.Weight = 1.0;

            this.m_UsesRemaining = amount;
        }

        public Shuriken(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get
            {
                return this.m_Poison;
            }
            set
            {
                this.m_Poison = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges
        {
            get
            {
                return this.m_PoisonCharges;
            }
            set
            {
                this.m_PoisonCharges = value;
                this.InvalidateProperties();
            }
        }
        public bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (this.m_Poison != null && this.m_PoisonCharges > 0)
                list.Add(1062412 + this.m_Poison.Level, this.m_PoisonCharges.ToString());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_UsesRemaining);

            Poison.Serialize(this.m_Poison, writer);
            writer.Write((int)this.m_PoisonCharges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_UsesRemaining = reader.ReadInt();

                        this.m_Poison = Poison.Deserialize(reader);
                        this.m_PoisonCharges = reader.ReadInt();

                        break;
                    }
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            if (quality == 2)
                this.UsesRemaining *= 2;

            return quality;
        }
    }
}