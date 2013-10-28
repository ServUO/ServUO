using System;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    [FlipableAttribute(0x13E4, 0x13E3)]
    public class ExceptionalSocketHammer : Item
    {
        private int m_UsesRemaining;// if set to less than zero, becomes unlimited uses
        private int m_Level;
        [Constructable]
        public ExceptionalSocketHammer()
            : this(50)
        {
        }

        [Constructable]
        public ExceptionalSocketHammer(int nuses)
            : base(0x13E4)
        {
            this.Name = "An Exceptional Socket Hammer";
            this.Hue = 5;
            this.UsesRemaining = nuses;
            int rand = Utility.Random(100);
            if (rand < 5)
            {
                this.m_Level = Utility.Random(4) + 1;
            }
            else if (rand < 10)
            {
                this.m_Level = Utility.Random(3) + 1;
            }
            else if (rand < 20)
            {
                this.m_Level = Utility.Random(2) + 1;
            }
            else if (rand < 40)
            {
                this.m_Level = Utility.Random(1) + 1;
            }
            else
            {
                this.m_Level = 1;
            }
        }

        public ExceptionalSocketHammer(Serial serial)
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
        public int Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_UsesRemaining >= 0)
                list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            list.Add(1060658, "Level\t{0}", this.m_Level); // ~1_val~: ~2_val~
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
                from.Target = new XmlSockets.AddSocketToTarget(this.m_Level);
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

            writer.Write((int)1);
            // version 1
            writer.Write(this.m_Level);
            // version 0
            writer.Write(this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch(version)
            {
                case 1:
                    this.m_Level = reader.ReadInt();
                    goto case 0;
                case 0:
                    this.m_UsesRemaining = reader.ReadInt();
                    break;
            }
        }
    }
}