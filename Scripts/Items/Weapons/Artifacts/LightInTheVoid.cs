using System;

namespace Server.Items
{
    public class LightInTheVoid : GargishTalwar
    {
        [Constructable]
        public LightInTheVoid()
        {
			this.Slayer = SlayerName.Silver;
			this.WeaponAttributes.HitLightning = 45;
			this.WeaponAttributes.HitLowerDefend = 30;
			this.Attributes.BonusStr = 8;
			this.Attributes.AttackChance = 10;
			this.Attributes.CastSpeed = 1;
			this.Attributes.WeaponSpeed = 20;
			this.Attributes.WeaponDamage = 35;
			this.Hue = 1072; //Hue not exact
			this.Name = ("Light in the Void");
        }

        public LightInTheVoid(Serial serial)
            : base(serial)
        {
        }

		public override int ArtifactRarity
        {
            get
            {
                return 5;
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