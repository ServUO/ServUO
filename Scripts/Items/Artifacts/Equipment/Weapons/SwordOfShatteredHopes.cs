using System;

namespace Server.Items
{
    public class SwordOfShatteredHopes : GlassSword
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SwordOfShatteredHopes()
            : base()
        {
            this.Name = ("Sword Of Shattered Hopes");
		
            this.Hue = 91;	
			
            this.WeaponAttributes.HitDispel = 25;
            this.WeaponAttributes.SplinteringWeapon = 20;
            this.Attributes.WeaponSpeed = 30;	
            this.Attributes.WeaponDamage = 50;			
            this.WeaponAttributes.ResistFireBonus = 15;
        }

        public SwordOfShatteredHopes(Serial serial)
            : base(serial)
        {
        }

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
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

			if (version < 1)
				WeaponAttributes.SplinteringWeapon = 20;
        }
    }
}