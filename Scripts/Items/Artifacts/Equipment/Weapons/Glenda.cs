using System;

namespace Server.Items
{
    public class Glenda : Club, ITokunoDyable
	{
        public override int LabelNumber { get { return 1157346; } } // glenda
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public Glenda()
        {
            ExtendedWeaponAttributes.BoneBreaker = 1;

            this.WeaponAttributes.HitLeechMana = 20;
            this.WeaponAttributes.HitLowerDefend = 70;
            this.Attributes.BonusStr = 16;
            this.Attributes.WeaponDamage = 100;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            fire = cold = nrgy = chaos = direct = pois = 0;
        }

        public Glenda(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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