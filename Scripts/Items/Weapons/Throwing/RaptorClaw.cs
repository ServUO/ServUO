using System;

namespace Server.Items
{
    public class RaptorClaw : Boomerang
    {
        [Constructable]
        public RaptorClaw()
            : base(0x8FF)
        {
            this.Name = ("Raptor Claw");
		
            this.Hue = 53;
            this.Slayer = SlayerName.Exorcism;
            this.Attributes.AttackChance = 12;			
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 45;
            this.WeaponAttributes.HitLeechStam = 40;
        }

        public RaptorClaw(Serial serial)
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