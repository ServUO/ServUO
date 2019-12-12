using System;

namespace Server.Items
{
    [FlipableAttribute(0x1443, 0x1442)]
    public class TheDeceiver : TwoHandedAxe
    {
        public override int LabelNumber { get { return 1157344; } } // the deceiver
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TheDeceiver() 
        {
            ExtendedWeaponAttributes.HitSparks = 20;
            WeaponAttributes.HitLowerAttack = 20;
            WeaponAttributes.HitEnergyArea = 75;
            WeaponAttributes.HitLowerDefend = 20;
            WeaponAttributes.HitLeechStam = 30;
            Attributes.LowerManaCost = 8;
            Attributes.WeaponDamage = 75;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            fire = cold = nrgy = chaos = direct = pois = 0;
        }

        public TheDeceiver(Serial serial)
            : base(serial)
        {
        }
        
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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