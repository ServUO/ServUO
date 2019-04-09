using System;
using Server.Targeting;

namespace Server.Items
{
    public class FocusingGemOfVirtueBane : Item
    {
        public override int LabelNumber { get { return 1150004; } } // Focusing Gem of Virtue Bane

        private static readonly TimeSpan Cooldown = TimeSpan.FromHours(6);

        [Constructable]
        public FocusingGemOfVirtueBane()
            : base(0x1F1E)
        {
            Hue = 2508;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.BeginTarget(2, false, TargetFlags.None, new TargetCallback(OnTarget));
            }
        }

        public void OnTarget(Mobile from, object obj)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (obj is Item)
            {
                Item item = (Item)obj;

                if (!item.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    return;
                }

                if (item is BaseWeapon)
                {
                    BaseWeapon weapon = (BaseWeapon)obj;

                    weapon.Hue = 2500;
                    weapon.ExtendedWeaponAttributes.Focus = 1;
                    weapon.NegativeAttributes.Brittle = 1;
                }
                else
                {
                    from.SendLocalizedMessage(1149988); // That can only be used on a weapon.
                }
            }
        }

        public FocusingGemOfVirtueBane(Serial serial)
            : base(serial)
        {
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

