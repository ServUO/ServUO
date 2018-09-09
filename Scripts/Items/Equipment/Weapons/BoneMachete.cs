using System;

namespace Server.Items
{
    public class BoneMachete : ElvenMachete
    {
        public override int InitMinHits { get { return 1; } }
        public override int InitMaxHits { get { return 3; } }

        [Constructable]
        public BoneMachete()
        {
        }

        public BoneMachete(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1020526;
            }
        }// bone machete

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                if (MaxHitPoints > 6)
                {
                    HitPoints = MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

                    if (Quality == ItemQuality.Exceptional)
                    {
                        MaxHitPoints *= 2;
                        HitPoints = MaxHitPoints;
                    }
                }
            }
        }
    }
}