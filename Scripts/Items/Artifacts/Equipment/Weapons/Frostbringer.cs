using System;

namespace Server.Items
{
    public class Frostbringer : Bow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Frostbringer()
        {
            Hue = 0x4F2;
            WeaponAttributes.HitDispel = 50;
            Attributes.RegenStam = 10;
            Attributes.WeaponDamage = 50;
        }

        public Frostbringer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061111;
            }
        }// Frostbringer
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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
            phys = fire = pois = nrgy = chaos = direct = 0;
            cold = 100;
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