using System;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    [FlipableAttribute(0x13E4, 0x13E3)]
    public class SocketHammer : Item
    {
        private int m_UsesRemaining;// if set to less than zero, becomes unlimited uses
        [Constructable]
        public SocketHammer()
            : this(50)
        {
        }

        [Constructable]
        public SocketHammer(int nuses)
            : base(0x13E4)
        {
            this.Name = "A Socket Hammer";
            this.Hue = 5;
            this.UsesRemaining = nuses;
        }

        public SocketHammer(Serial serial)
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
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_UsesRemaining >= 0)
                list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.UsesRemaining == 0)
            {
                from.SendMessage("This hammer is now useless");
                return;
            }
            if (this.IsChildOf(from.Backpack) || this.Parent == from)
            {
                from.Target = new XmlSockets.AddSocketToTarget();
                if (this.UsesRemaining > 0)
                    this.UsesRemaining--;
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_UsesRemaining = reader.ReadInt();
        }
    }
}