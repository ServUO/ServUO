using System;

namespace Server.Items
{
    public class ChannelersDefender : GlassSword
    {
        [Constructable]
        public ChannelersDefender()
        {
            this.Name = ("Channeler's Defender");
		
            this.Hue = 95;	
            this.Attributes.DefendChance = 10;				
            this.Attributes.AttackChance = 5;	
            this.Attributes.LowerManaCost = 5;
            this.Attributes.WeaponSpeed = 20;					
            this.Attributes.CastRecovery = 1;		
            this.Attributes.SpellChanneling = 1;	
            this.WeaponAttributes.HitLowerAttack = 60;
            this.AosElementDamages.Energy = 100;		
        }

        public ChannelersDefender(Serial serial)
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