using System;

namespace Server.Ethics.Hero
{
    public sealed class HolySteed : Power
    {
        public HolySteed()
        {
            this.m_Definition = new PowerDefinition(
                30,
                "Holy Steed",
                "Trubechs Yeliab",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            if (from.Steed != null && from.Steed.Deleted)
                from.Steed = null;

            if (from.Steed != null)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You already have a holy steed.");
                return;
            }

            if ((from.Mobile.Followers + 1) > from.Mobile.FollowersMax)
            {
                from.Mobile.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return;
            }

            Mobiles.HolySteed steed = new Mobiles.HolySteed();

            if (Mobiles.BaseCreature.Summon(steed, from.Mobile, from.Mobile.Location, 0x217, TimeSpan.FromHours(1.0)))
            {
                from.Steed = steed;

                this.FinishInvoke(from);
            }
        }
    }
}