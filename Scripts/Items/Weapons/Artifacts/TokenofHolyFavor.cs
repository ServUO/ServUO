using System;

namespace Server.Items
{
    public class TokenOfHolyFavor : GoldBracelet
    {
        [Constructable]
        public TokenOfHolyFavor()
        {
            this.Name = ("Token Of Holy Favor");
		
            this.Hue = 96;
            this.Attributes.BonusHits = 5;
            this.Attributes.CastRecovery = 2;
            this.Attributes.CastSpeed = 1;
            this.Attributes.DefendChance = 10;
            this.Attributes.AttackChance = 10;
            this.Attributes.SpellDamage = 4;
            this.Resistances.Cold = 5;
            this.Resistances.Poison = 5;
        }

        public TokenOfHolyFavor(Serial serial)
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Hue == 0x12B)
                this.Hue = 0x554;
        }
    }
}