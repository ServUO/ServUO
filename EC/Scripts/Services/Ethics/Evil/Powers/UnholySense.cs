using System;
using System.Text;

namespace Server.Ethics.Evil
{
    public sealed class UnholySense : Power
    {
        public UnholySense()
        {
            this.m_Definition = new PowerDefinition(
                0,
                "Unholy Sense",
                "Drewrok Velgo",
                "");
        }

        public override void BeginInvoke(Player from)
        {
            Ethic opposition = Ethic.Hero;

            int enemyCount = 0;

            int maxRange = 18 + from.Power;

            Player primary = null;

            foreach (Player pl in opposition.Players)
            {
                Mobile mob = pl.Mobile;

                if (mob == null || mob.Map != from.Mobile.Map || !mob.Alive)
                    continue;

                if (!mob.InRange(from.Mobile, Math.Max(18, maxRange - pl.Power)))
                    continue;

                if (primary == null || pl.Power > primary.Power)
                    primary = pl;

                ++enemyCount;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("You sense ");
            sb.Append(enemyCount == 0 ? "no" : enemyCount.ToString());
            sb.Append(enemyCount == 1 ? " enemy" : " enemies");

            if (primary != null)
            {
                sb.Append(", and a strong presense");

                switch ( from.Mobile.GetDirectionTo(primary.Mobile) )
                {
                    case Direction.West:
                        sb.Append(" to the west.");
                        break;
                    case Direction.East:
                        sb.Append(" to the east.");
                        break;
                    case Direction.North:
                        sb.Append(" to the north.");
                        break;
                    case Direction.South:
                        sb.Append(" to the south.");
                        break;
                    case Direction.Up:
                        sb.Append(" to the north-west.");
                        break;
                    case Direction.Down:
                        sb.Append(" to the south-east.");
                        break;
                    case Direction.Left:
                        sb.Append(" to the south-west.");
                        break;
                    case Direction.Right:
                        sb.Append(" to the north-east.");
                        break;
                }
            }
            else
            {
                sb.Append('.');
            }

            from.Mobile.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x59, false, sb.ToString());

            this.FinishInvoke(from);
        }
    }
}