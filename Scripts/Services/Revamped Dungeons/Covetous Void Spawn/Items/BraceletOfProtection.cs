using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class BraceletOfProtection : GoldBracelet
    {
        public override int LabelNumber { get { return 1152730; } } // Bracelet of Protection

        [Constructable]
        public BraceletOfProtection()
            : this(true)
        {
        }

        [Constructable]
        public BraceletOfProtection(bool antique)
        {
            Hue = 1157;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 10;
            Attributes.DefendChance = 5;

            switch (Utility.Random(6))
            {
                case 0: AbsorptionAttributes.EaterKinetic = 15; break;
                case 1: AbsorptionAttributes.EaterFire = 15; break;
                case 2: AbsorptionAttributes.EaterCold = 15; break;
                case 3: AbsorptionAttributes.EaterPoison = 15; break;
                case 4: AbsorptionAttributes.EaterEnergy = 15; break;
                case 5: AbsorptionAttributes.EaterDamage = 15; break;
            }

            if (antique)
            {
                MaxHitPoints = 250;
                NegativeAttributes.Antique = 1;
            }
            else
                MaxHitPoints = 255;

            HitPoints = MaxHitPoints;
        }

        public BraceletOfProtection(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}