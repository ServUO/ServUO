using System;

namespace Server.Items
{
    public abstract class BaseStrengthPotion : BasePotion
    {
        public BaseStrengthPotion(PotionEffect effect)
            : base(0xF09, effect)
        {
        }

        public BaseStrengthPotion(Serial serial)
            : base(serial)
        {
        }

        public abstract int StrOffset { get; }
        public abstract TimeSpan Duration { get; }
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

        public bool DoStrength(Mobile from)
        {
            // TODO: Verify scaled; is it offset, duration, or both?
            int scale = Scale(from, this.StrOffset);
            if (Spells.SpellHelper.AddStatOffset(from, StatType.Str, scale, this.Duration))
            {
                from.FixedEffect(0x375A, 10, 15);
                from.PlaySound(0x1E7);

                BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Strength, 1075845, this.Duration, from, scale.ToString()));

                return true;
            }

            from.SendLocalizedMessage(502173); // You are already under a similar effect.
            return false;
        }

        public override void Drink(Mobile from)
        {
            if (this.DoStrength(from))
            {
                BasePotion.PlayDrinkEffect(from);

                if (!Engines.ConPVP.DuelContext.IsFreeConsume(from))
                    this.Consume();
            }
        }
    }
}