using Server.Targeting;
using System;

namespace Server.Items
{
    public class FocusingGemOfVirtueBane : Item
    {
        public override int LabelNumber => 1150004;  // Focusing Gem of Virtue Bane

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Cooldown { get; private set; }

        [Constructable]
        public FocusingGemOfVirtueBane()
            : base(0x1F1E)
        {
            Hue = 2508;
        }

        public override void OnDoubleClick(Mobile from)
        {
            bool messagecheck = true;

            if (Cooldown < DateTime.Now)
            {
                from.SendLocalizedMessage(1149987); // Using this on your weapon will make it a rage focused weapon and turn it brittle.
                Cooldown = DateTime.Now + TimeSpan.FromMinutes(10);
                messagecheck = false;
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }
            else
            {
                if (messagecheck)
                {
                    from.BeginTarget(2, false, TargetFlags.None, OnTarget);
                }
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

                    if (weapon.ExtendedWeaponAttributes.Focus == 0)
                    {
                        weapon.Hue = 2500;
                        weapon.ExtendedWeaponAttributes.Focus = 1;
                        weapon.NegativeAttributes.Brittle = 1;
                        Delete();
                    }
                    else
                    {
                        from.SendLocalizedMessage(1149989); // That property already exists on that item.
                    }
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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}

