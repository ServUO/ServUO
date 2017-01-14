using System;

namespace Server.Items
{
    public class PixieSwatter : Scepter
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PixieSwatter()
        {
            this.Hue = 0x8A;
            this.WeaponAttributes.HitPoisonArea = 75;
            this.Attributes.WeaponSpeed = 30;
            
            this.WeaponAttributes.UseBestSkill = 1;
            this.WeaponAttributes.ResistFireBonus = 12;
            this.WeaponAttributes.ResistEnergyBonus = 12;

            this.Slayer = SlayerName.Fey;
        }

        public PixieSwatter(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070854;
            }
        }// Pixie Swatter
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
            fire = 100;

            cold = pois = phys = nrgy = chaos = direct = 0;
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