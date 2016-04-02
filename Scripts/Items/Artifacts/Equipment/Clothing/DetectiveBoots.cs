using System;

namespace Server.Items
{
    public class DetectiveBoots : Boots
    {
        private int m_Level;
        [Constructable]
        public DetectiveBoots()
        {
            this.Hue = 0x455;
            this.Level = Utility.RandomMinMax(0, 2);
        }

        public DetectiveBoots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094894 + this.m_Level;
            }
        }// [Quality] Detective of the Royal Guard [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
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
                this.m_Level = Math.Max(Math.Min(2, value), 0);
                this.Attributes.BonusInt = 2 + this.m_Level;
                this.InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.Level = this.Attributes.BonusInt - 2;
        }
    }
}