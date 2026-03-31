using System;

namespace Server.Items
{
    public class AxeOfTheHeavens : DoubleAxe
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AxeOfTheHeavens()
        {
            Hue = 0x4D5;
            WeaponAttributes.HitLightning = 50;
            WeaponAttributes.HitLowerDefend = 50;
            WeaponAttributes.HitLeechMana = 80;
            WeaponAttributes.HitLeechHits = 80;
            WeaponAttributes.BattleLust = 1;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponDamage = 50;

            switch (Utility.Random(5))
            {
                case 0: AosElementDamages.Physical = 100; break;
                case 1: AosElementDamages.Physical = 0; AosElementDamages.Fire = 100; break;
                case 2: AosElementDamages.Physical = 0; AosElementDamages.Cold = 100; break;
                case 3: AosElementDamages.Physical = 0; AosElementDamages.Energy = 100; break;
                case 4: AosElementDamages.Physical = 0; AosElementDamages.Poison = 100; break;
            }
        }

        public AxeOfTheHeavens(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061106;
            }
        }// Axe of the Heavens
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