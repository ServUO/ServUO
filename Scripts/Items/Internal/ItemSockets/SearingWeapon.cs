
using Server.ContextMenus;

namespace Server.Items
{
    public class SearingWeapon : ItemSocket
    {
        public bool Extinguished { get; set; } = true;

        public SearingWeapon()
        {
        }

        public SearingWeapon(BaseWeapon wep)
        {
            wep.NegativeAttributes.Brittle = 1;
            wep.MaxHitPoints = 200;
            wep.HitPoints = 200;
            wep.Hue = 2500;
        }

        public override void OnRemoved()
        {
            if (Owner is BaseWeapon)
            {
                ((BaseWeapon)Owner).NegativeAttributes.Brittle = 0;
                Owner.Hue = ((BaseWeapon)Owner).GetElementalDamageHue();
            }
        }

        public static void OnWeaponRemoved(BaseWeapon wep)
        {
            SearingWeapon socket = wep.GetSocket<SearingWeapon>();

            if (socket != null && !socket.Extinguished)
            {
                socket.Extinguished = true;
                wep.Hue = 2500;
            }
        }

        public void ToggleExtinguish(Mobile from)
        {
            if (Extinguished)
            {
                Extinguished = false;
                Owner.Hue = 1174;

                from.SendLocalizedMessage(1151175); // You ignite your weapon.
            }
            else
            {
                Extinguished = true;
                Owner.Hue = 2500;

                from.SendLocalizedMessage(1151176); // You extinguish your weapon.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Extinguished);
        }

        public static bool CanSear(BaseWeapon weapon)
        {
            SearingWeapon socket = weapon.GetSocket<SearingWeapon>();

            return socket != null && !socket.Extinguished;
        }

        public override void Deserialize(Item owner, GenericReader reader)
        {
            base.Deserialize(owner, reader);
            reader.ReadInt(); // version

            Extinguished = reader.ReadBool();
        }

        public class ToggleExtinguishEntry : ContextMenuEntry
        {
            public BaseWeapon Weapon { get; set; }
            public Mobile From { get; set; }

            public ToggleExtinguishEntry(Mobile from, BaseWeapon weapon)
                : base(weapon.GetSocket<SearingWeapon>().Extinguished ? 1151173 : 1151174, -1)
            {
                From = from;
                Weapon = weapon;
            }

            public override void OnClick()
            {
                if (Weapon.Parent == From)
                {
                    SearingWeapon socket = Weapon.GetSocket<SearingWeapon>();

                    if (socket != null)
                    {
                        socket.ToggleExtinguish(From);
                    }
                }
            }
        }
    }
}
