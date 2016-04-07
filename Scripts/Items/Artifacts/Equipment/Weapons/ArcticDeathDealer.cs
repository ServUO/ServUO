using System;

namespace Server.Items
{
    public class ArcticDeathDealer : WarMace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArcticDeathDealer()
        {
            this.Hue = 0x480;
            this.WeaponAttributes.HitHarm = 33;
            this.WeaponAttributes.HitLowerAttack = 40;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 40;
            this.WeaponAttributes.ResistColdBonus = 10;
        }

        public ArcticDeathDealer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063481;
            }
        }
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
            cold = 50;
            phys = 50;

            pois = fire = nrgy = chaos = direct = 0;
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