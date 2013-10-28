using System;

namespace Server.Items
{
    public class SwordOfShatteredHopes : GlassSword
    {
        [Constructable]
        public SwordOfShatteredHopes()
            : base(0x90C)
        {
            this.Name = ("Sword Of Shattered Hopes");
		
            this.Hue = 91;	
			
            this.WeaponAttributes.HitDispel = 25;
            //WeaponAttributes.SplinteringWeapon = 20;
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}