using Server;
using System;
using Server.Targeting;

namespace Server.Items
{
    public class WhetstoneOfEnervation : Item
    {
        public override int LabelNumber { get { return 1151811; } } // Whetstone of Enervation

        [Constructable]
        public WhetstoneOfEnervation()
            : this(1)
        {
        }

        [Constructable]
        public WhetstoneOfEnervation(int amount) : base(0x1368)
        {
            this.Hue = 1458;
            this.Weight = 1;

            Stackable = true;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.BeginTarget(-1, false, TargetFlags.None, (m, targeted) =>
                    {
                        if (!IsChildOf(m.Backpack))
                            m.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                        else if (targeted is BaseWeapon)
                        {
                            BaseWeapon wep = targeted as BaseWeapon;

                            if(!wep.IsChildOf(m.Backpack))
                                m.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                            else if (wep.TimesImbued > 0)
                                m.SendLocalizedMessage(1046439); // Invalid target.
                            else if (wep.Attributes.WeaponDamage > 0)
                            {
                                wep.Attributes.WeaponDamage = 0;
                                m.SendLocalizedMessage(1151814); // You have removed the damage increase from this weapon.

                                this.Consume();
                            }
                            else
                                m.SendLocalizedMessage(1046439); // Invalid target.
                        }
                        else
                            m.SendLocalizedMessage(1046439); // Invalid target.
                    });
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public WhetstoneOfEnervation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID != 0x1368)
                ItemID = 0x1368;
        }
    }
}