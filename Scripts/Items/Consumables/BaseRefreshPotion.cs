using System;

namespace Server.Items
{
    public abstract class BaseRefreshPotion : BasePotion
    {
        public BaseRefreshPotion(PotionEffect effect)
            : base(0xF0B, effect)
        {
        }

        public BaseRefreshPotion(Serial serial)
            : base(serial)
        {
        }

        public abstract double Refresh { get; }
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

        public override void Drink(Mobile from)
        {
            if (from.Stam < from.StamMax)
            {
                from.Stam += Scale(from, (int)(this.Refresh * from.StamMax));

                BasePotion.PlayDrinkEffect(from);

                if (!Engines.ConPVP.DuelContext.IsFreeConsume(from))
                    this.Consume();
            }
            else
            {
                from.SendMessage("You decide against drinking this potion, as you are already at full stamina.");
            }
        }
    }
}