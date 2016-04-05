using System;

namespace Server.Items
{
    public class TheTaskmaster : WarFork
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TheTaskmaster()
        {
            this.Hue = 0x4F8;
            this.WeaponAttributes.HitPoisonArea = 100;
            this.Attributes.BonusDex = 5;
            this.Attributes.AttackChance = 15;
            this.Attributes.WeaponDamage = 50;
        }

        public TheTaskmaster(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061110;
            }
        }// The Taskmaster
        public override int ArtifactRarity
        {
            get
            {
                return 10;
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
            phys = fire = cold = nrgy = chaos = direct = 0;
            pois = 100;
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