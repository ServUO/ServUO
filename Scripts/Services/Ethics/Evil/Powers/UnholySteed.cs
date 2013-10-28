using System;

namespace Server.Ethics.Evil
{
    public sealed class UnholySteed : Power
    {
        public UnholySteed()
        {
            this.m_Definition = new PowerDefinition(
                30,
                "Unholy Steed",
                "Trubechs Yeliab",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            if (from.Steed != null && from.Steed.Deleted)
                from.Steed = null;

            if (from.Steed != null)
            {
                from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "You already have an unholy steed.");
                return;
            }

            if ((from.Mobile.Followers + 1) > from.Mobile.FollowersMax)
            {
                from.Mobile.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return;
            }

            Mobiles.UnholySteed steed = new Mobiles.UnholySteed();

            if (Mobiles.BaseCreature.Summon(steed, from.Mobile, from.Mobile.Location, 0x217, TimeSpan.FromHours(1.0)))
            {
                from.Steed = steed;

                this.FinishInvoke(from);
            }
        }
    }
}