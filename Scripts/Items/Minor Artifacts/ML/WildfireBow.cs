using System;

namespace Server.Items
{
    public class WildfireBow : ElvenCompositeLongbow
    {
        [Constructable]
        public WildfireBow()
            : base()
        {
            this.Hue = 0x489;
			
            this.SkillBonuses.SetValues(0, SkillName.Archery, 10);
            this.WeaponAttributes.ResistFireBonus = 25;
			
            this.Velocity = 15;			
        }

        public WildfireBow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075044;
            }
        }// Wildfire Bow
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = cold = pois = nrgy = chaos = direct = 0;
            fire = 100;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}