using System;

namespace Server.Items
{
    public class PerfectEmeraldRing : GoldRing
    {
        [Constructable]
        public PerfectEmeraldRing()
            : base()
        {
            this.Weight = 1.0;

            BaseRunicTool.ApplyAttributesTo(this, true, 0, Utility.RandomMinMax(2, 4), 0, 100);
			
            if (Utility.RandomBool())
                this.Resistances.Poison += 10;	
            else
                this.Attributes.SpellDamage += 5;
        }

        public PerfectEmeraldRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073459;
            }
        }// perfect emerald ring
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