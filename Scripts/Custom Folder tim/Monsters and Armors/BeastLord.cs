using System;

namespace Server.Items
{
    public class BeastLord : ShepherdsCrook
    {
        [Constructable]
        public BeastLord()
            : base()
        {
            this.Hue = 1153;
            this.Name = "BeastLord";
            //this.WeaponAttributes.HitLeechStam = 40;
            this.Attributes.RegenMana = 3;
            //this.WeaponAttributes.HitLeechMana = 55;
            this.WeaponAttributes.HitLeechHits = 55;
            this.WeaponAttributes.HitLightning = 50;

            this.WeaponAttributes.MageWeapon = 30;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.WeaponDamage = 60;
            //this.Attributes.DefendChance = 15;
        }

        public BeastLord(Serial serial)
            : base(serial)
        {
        }

        
        public override int ArtifactRarity
        {
            get
            {
                return 30;
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

            writer.Write((int)2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}