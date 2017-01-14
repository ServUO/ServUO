using System;

namespace Server.Items
{
    public class ValkyriesGlaive : SoulGlaive
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ValkyriesGlaive()
        {
			this.Attributes.SpellChanneling = 1;
			this.Slayer = SlayerName.Silver;
			this.WeaponAttributes.HitFireball = 40;
			this.Attributes.BonusStr = 5;
			this.Attributes.WeaponSpeed = 20;
			this.Attributes.WeaponDamage = 20;
			this.Hue = 1651; //Hue not exact
			this.Name = ("Valkyrie's Glaive");
        }

        public ValkyriesGlaive(Serial serial)
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

		public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
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