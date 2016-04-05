using System;

namespace Server.Items
{
    public class StaffOfShatteredDreams : GlassStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public StaffOfShatteredDreams()
            : base()
        {
            this.Name = ("Staff Of Shattered Dreams");
		
            this.Hue = 1151;
		
            this.WeaponAttributes.HitDispel = 25;
            this.WeaponAttributes.SplinteringWeapon = 20;
            this.Attributes.WeaponDamage = 50;			
            this.WeaponAttributes.ResistFireBonus = 15;
            this.Attributes.CastSpeed = -1;
            this.Attributes.SpellChanneling = 1;	
        }

        public StaffOfShatteredDreams(Serial serial)
            : base(serial)
        {
        }

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