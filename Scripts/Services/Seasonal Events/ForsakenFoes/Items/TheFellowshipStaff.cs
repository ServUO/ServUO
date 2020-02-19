using System;

namespace Server.Items
{
    [Flipable(0xA343, 0xA344)]
    public class TheFellowshipStaff : BaseStaff
    {
        public override int LabelNumber { get { return 1159034; } } // The Fellowship Staff

        public static TheFellowshipStaff InstanceTram { get; set; }
        public static TheFellowshipStaff InstanceFel { get; set; }

        [Constructable]
        public TheFellowshipStaff()
            : base (0xA343)
        {
            Hue = 2721;
        }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.Block; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ForceOfNature; } }
        public override int AosStrengthReq { get { return 20; } }
        public override int AosMinDamage { get { return 15; } }
        public override int AosMaxDamage { get { return 18; } }
        public override float MlSpeed { get { return 3.25f; } }
        
        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 60; } }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            fire = cold = pois = nrgy = chaos = direct = 0;
            phys = 100;
        }

        public TheFellowshipStaff(Serial serial)
            : base(serial)
        {
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

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
