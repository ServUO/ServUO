using System;
using Server.Items;

namespace Server.Ethics.Hero
{
    public sealed class HolyItem : Power
    {
        public HolyItem()
        {
            this.m_Definition = new PowerDefinition(
                5,
                "Holy Item",
                "Vidda K'balc",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            from.Mobile.BeginTarget(12, false, Targeting.TargetFlags.None, new TargetStateCallback(Power_OnTarget), from);
            from.Mobile.SendMessage("Which item do you wish to imbue?");
        }

        private void Power_OnTarget(Mobile fromMobile, object obj, object state)
        {
            Player from = state as Player;

            Item item = obj as Item;

            if (item == null)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You may not imbue that.");
                return;
            }

            if (item.Parent != from.Mobile)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You may only imbue items you are wearing.");
                return;
            }

            if ((item.SavedFlags & 0x300) != 0)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "That has already beem imbued.");
                return;
            }

            bool canImbue = (item is Spellbook || item is BaseClothing || item is BaseArmor || item is BaseWeapon) && (item.Name == null);

            if (canImbue)
            {
                if (!this.CheckInvoke(from))
                    return;

                item.Hue = Ethic.Hero.Definition.PrimaryHue;
                item.SavedFlags |= 0x100;

                from.Mobile.FixedEffect(0x375A, 10, 20);
                from.Mobile.PlaySound(0x209);

                this.FinishInvoke(from);
            }
            else
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You may not imbue that.");
            }
        }
    }
}