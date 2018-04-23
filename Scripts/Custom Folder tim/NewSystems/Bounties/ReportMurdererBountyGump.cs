using System;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using System.Linq;
using Server.Items;

namespace Server.Gumps
{
    public class ReportMurdererBountyGump : Gump
    {
        private int _idx;
        private readonly List<Mobile> _killers;
        private readonly Mobile _victim;

        [CallPriority(1)]
        public static void Initialize()
        {
            EventSink.PlayerDeath -= ReportMurdererGump.EventSink_PlayerDeath;
            EventSink.PlayerDeath += EventSink_PlayerDeath;
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            var m = e.Mobile;

            var killers = new List<Mobile>();
            var toGive = new List<Mobile>();

            foreach (var ai in m.Aggressors)
            {
                if (ai.Attacker.Player && ai.CanReportMurder && !ai.Reported)
                {
                    // Note: The default reportmurderer script only allows reporting the same player once every 10 minutes in the Core.SE expansion.
                    // This was also the case in the T2A era, so I'm not sure why it was set to a minimum expansion of Core.SE
                    if (!((PlayerMobile) m).RecentlyReported.Contains(ai.Attacker))
                    {
                        killers.Add(ai.Attacker);
                        ai.Reported = true;
                        ai.CanReportMurder = false;
                    }
                }

                if (ai.Attacker.Player && DateTime.UtcNow - ai.LastCombatTime < TimeSpan.FromSeconds(30.0) &&
                    !toGive.Contains(ai.Attacker))
                    toGive.Add(ai.Attacker);
            }

            toGive.AddRange(
                m.Aggressed.Where(
                    x =>
                        x.Defender is PlayerMobile && DateTime.UtcNow - x.LastCombatTime < TimeSpan.FromSeconds(30.0) &&
                        !toGive.Contains(x.Defender)).Select(x => x.Defender));

            foreach (var g in toGive)
            {
                var n = Notoriety.Compute(g, m);

                var ourKarma = g.Karma;
                var innocent = n == Notoriety.Innocent;
                var criminal = n == Notoriety.Criminal || n == Notoriety.Murderer;

                var fameAward = m.Fame/200;
                var karmaAward = 0;

                if (innocent)
                    karmaAward = ourKarma > -2500 ? -850 : -110 - m.Karma/100;
                else if (criminal)
                    karmaAward = 50;

                Titles.AwardFame(g, fameAward, false);
                Titles.AwardKarma(g, karmaAward, true);
            }

            if (m is PlayerMobile && ((PlayerMobile) m).NpcGuild == NpcGuild.ThievesGuild || killers.Count == 0)
                return;

            var gump = m.FindGump(typeof (ReportMurdererBountyGump)) as ReportMurdererBountyGump;
            if (gump != null)
                gump.TryAddKillers(killers);
            else
                new GumpTimer(m, killers).Start();
        }

        private class GumpTimer : Timer
        {
            private readonly Mobile _victim;
            private readonly List<Mobile> _killers;

            public GumpTimer(Mobile victim, List<Mobile> killers) : base(TimeSpan.FromSeconds(4.0))
            {
                _victim = victim;
                _killers = killers;
            }

            protected override void OnTick()
            {
                _victim.SendGump(new ReportMurdererBountyGump(_victim, _killers));
            }
        }

        public ReportMurdererBountyGump(Mobile victum, List<Mobile> killers) : this(victum, killers, 0)
        {
        }

        private ReportMurdererBountyGump(Mobile victum, List<Mobile> killers, int idx) : base(0, 0)
        {
            _killers = killers;
            _victim = victum;
            _idx = idx;
            BuildGump();
        }

        private void BuildGump()
        {
            AddBackground(265, 205, 393, 270, 70000);
            AddImage(265, 205, 1140);

            Closable = false;
            Resizable = false;

            AddPage(0);

            AddHtml(325, 255, 300, 60,
                "<BIG>Would you like to report " + _killers[_idx].Name + " as a murderer?</BIG>", false, false);

            var bountymax = Banker.GetBalance(_victim);

            if (_killers[_idx].Kills >= 4 && bountymax > 0)
            {
                AddHtml(325, 325, 300, 60, "<BIG>Optional Bounty: [" + bountymax + " max] </BIG>", false, false);
                AddImage(323, 343, 0x475);
                AddTextEntry(329, 346, 311, 16, 0, 1, "");
            }

            AddButton(385, 395, 0x47B, 0x47D, 1, GumpButtonType.Reply, 0);
            AddButton(465, 395, 0x478, 0x47A, 2, GumpButtonType.Reply, 0);
        }

        private void TryAddKillers(IEnumerable<Mobile> killers)
        {
            _killers.AddRange(
                killers.Where(
                    killer =>
                        !_killers.Contains(killer) && !((PlayerMobile) _victim).RecentlyReported.Contains(killer)));
        }

        public static void ReportedListExpiry_Callback(object state)
        {
            var states = (object[]) state;

            var from = (PlayerMobile) states[0];
            var killer = (Mobile) states[1];

            if (from.RecentlyReported.Contains(killer))
            {
                from.RecentlyReported.Remove(killer);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            var from = state.Mobile;

            switch (info.ButtonID)
            {
                case 1:
                {
                    var killer = _killers[_idx];
                    if (killer != null && !killer.Deleted)
                    {
                        killer.Kills++;
                        killer.ShortTermMurders++;


                        ((PlayerMobile) from).RecentlyReported.Add(killer);
                        Timer.DelayCall(TimeSpan.FromMinutes(10), new TimerStateCallback(ReportedListExpiry_Callback),
                            new object[] {from, killer});

                        var pk = (PlayerMobile) killer;

                        if (info.GetTextEntry(1) != null)
                        {
                            var c = info.GetTextEntry(1);
                            if (c != null)
                            {
                                var bounty = Math.Min( Utility.ToInt32(c.Text), Banker.GetBalance(from) );
								
                                if (bounty > 0)
                                {
                                    Banker.Withdraw(from, bounty);
                                    BountyInformation.AddBounty(pk, bounty, true);

                                    pk.SendMessage("{0} has placed a bounty of {1} {2} on your head!", from.Name,
                                        bounty, bounty == 1 ? "gold piece" : "gold pieces");
                                    pk.Say(500546); // I am now bounty hunted!

                                    from.SendMessage("You place a bounty of {0}gp on {1}'s head.", bounty, pk.Name);
                                }
                            }
                        }

                        pk.ResetKillTime();
                        pk.SendLocalizedMessage(1049067); //You have been reported for murder!

                        if (pk.Kills == 5)
                        {
                            pk.SendLocalizedMessage(502134); //You are now known as a murderer!
                        }
                        else if (SkillHandlers.Stealing.SuspendOnMurder && pk.Kills == 1 &&
                                 pk.NpcGuild == NpcGuild.ThievesGuild)
                        {
                            pk.SendLocalizedMessage(501562); // You have been suspended by the Thieves Guild.
                        }
                    }
                    break;
                }
                case 2:
                {
                    break;
                }
            }

            _idx++;
            if (_idx < _killers.Count)
                from.SendGump(new ReportMurdererBountyGump(from, _killers, _idx));
        }
    }
}