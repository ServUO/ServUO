using System;

namespace Server.Items
{
    public class ParasiticPotion : BasePoisonPotion
    {
        [Constructable]
        public ParasiticPotion()
            : base(PotionEffect.Parasitic)
        {
            this.Hue = 0x17C;
        }

        public ParasiticPotion(Serial serial)
            : base(serial)
        {
        }

        public override Poison Poison
        {
            get
            {
                return Poison.Parasitic;
            }
        }/* public override Poison Poison{ get{ return Poison.Darkglow; } }  MUST be restored when prerequisites are done */
        public override double MinPoisoningSkill
        {
            get
            {
                return 95.0;
            }
        }
        public override double MaxPoisoningSkill
        {
            get
            {
                return 100.0;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072848;
            }
        }// Parasitic Poison
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}