using System;

namespace Server.Engines.Quests.Hag
{
    public class HangoverCure : Item
    {
		public override bool IsArtifact { get { return true; } }
        private int m_Uses;
        [Constructable]
        public HangoverCure()
            : base(0xE2B)
        {
            this.Weight = 1.0;
            this.Hue = 0x2D;

            this.m_Uses = 20;
        }

        public HangoverCure(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1055060;
            }
        }// Grizelda's Extra Strength Hangover Cure
        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses
        {
            get
            {
                return this.m_Uses;
            }
            set
            {
                this.m_Uses = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                this.SendLocalizedMessageTo(from, 1042038); // You must have the object in your backpack to use it.
                return;
            }

            if (this.m_Uses > 0)
            {
                from.PlaySound(0x2D6);
                from.SendLocalizedMessage(501206); // An awful taste fills your mouth.

                if (from.BAC > 0)
                {
                    from.BAC = 0;
                    from.SendLocalizedMessage(501204); // You are now sober!
                }

                this.m_Uses--;
            }
            else
            {
                this.Delete();
                from.SendLocalizedMessage(501201); // There wasn't enough left to have any effect.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.WriteEncodedInt(this.m_Uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Uses = reader.ReadEncodedInt();
                        break;
                    }
                case 0:
                    {
                        this.m_Uses = 20;
                        break;
                    }
            }
        }
    }
}