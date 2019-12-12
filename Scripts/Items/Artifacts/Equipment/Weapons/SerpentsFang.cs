using System;

namespace Server.Items
{
    public class SerpentsFang : Kryss
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SerpentsFang()
        {
            Hue = 0x488;
            WeaponAttributes.HitPoisonArea = 100;
            WeaponAttributes.ResistPoisonBonus = 20;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 50;
        }

        public SerpentsFang(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061601;
            }
        }// Serpent's Fang
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
            fire = cold = nrgy = chaos = direct = 0;
            phys = 25;
            pois = 75;
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