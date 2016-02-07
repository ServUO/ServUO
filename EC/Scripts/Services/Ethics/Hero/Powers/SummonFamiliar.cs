using System;
using Server.Mobiles;

namespace Server.Ethics.Hero
{
    public sealed class SummonFamiliar : Power
    {
        public SummonFamiliar()
        {
            this.m_Definition = new PowerDefinition(
                5,
                "Summon Familiar",
                "Trubechs Vingir",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            if (from.Familiar != null && from.Familiar.Deleted)
                from.Familiar = null;

            if (from.Familiar != null)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You already have a holy familiar.");
                return;
            }

            if ((from.Mobile.Followers + 1) > from.Mobile.FollowersMax)
            {
                from.Mobile.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return;
            }

            HolyFamiliar familiar = new HolyFamiliar();

            if (Mobiles.BaseCreature.Summon(familiar, from.Mobile, from.Mobile.Location, 0x217, TimeSpan.FromHours(1.0)))
            {
                from.Familiar = familiar;

                this.FinishInvoke(from);
            }
        }
    }
}