using System;

namespace Server.Items
{
    public class AbyssalBlade : StoneWarSword
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AbyssalBlade()
        {
            this.Name = ("Abyssal Blade");

            this.Hue = 2404;
            this.WeaponAttributes.HitManaDrain = 50;
            this.WeaponAttributes.HitFatigue = 50;
            this.WeaponAttributes.HitLeechHits = 60;
            this.WeaponAttributes.HitLeechStam = 60;
            this.WeaponAttributes.HitLeechMana = 60;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 60;
            this.AosElementDamages.Chaos = 100;
        }

        public AbyssalBlade(Serial serial)
            : base(serial)
        {
        }
		
		 public override float MlSpeed
        {
            get
            {
                return 3.75f;
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