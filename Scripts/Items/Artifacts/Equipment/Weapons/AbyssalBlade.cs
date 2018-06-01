using System;

namespace Server.Items
{
    public class AbyssalBlade : StoneWarSword
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public AbyssalBlade()
        {
            Name = ("Abyssal Blade");

            Hue = 2404;
            WeaponAttributes.HitManaDrain = 50;
            WeaponAttributes.HitFatigue = 50;
            WeaponAttributes.HitLeechHits = 60;
            WeaponAttributes.HitLeechStam = 60;
            WeaponAttributes.HitLeechMana = 60;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 60;
            AosElementDamages.Chaos = 100;
        }

        public AbyssalBlade(Serial serial)
            : base(serial)
        {
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
