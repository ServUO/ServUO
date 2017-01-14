using Server;
using System;

namespace Server.Items
{
    public class CullingBlade : BoneHarvester
    {
        public override int LabelNumber { get { return 1116630; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public CullingBlade()
        {
            WeaponAttributes.HitManaDrain = 30;
            WeaponAttributes.HitFatigue = 30;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.RegenHits = 3;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 50;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = nrgy = pois = direct = 0;
            chaos = 100;
        }

        public CullingBlade(Serial serial)
            : base(serial)
        {
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
        }
    }
}