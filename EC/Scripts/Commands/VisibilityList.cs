using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Commands
{
    public class VisibilityList
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(OnLogin);

            CommandSystem.Register("Vis", AccessLevel.Counselor, new CommandEventHandler(Vis_OnCommand));
            CommandSystem.Register("VisList", AccessLevel.Counselor, new CommandEventHandler(VisList_OnCommand));
            CommandSystem.Register("VisClear", AccessLevel.Counselor, new CommandEventHandler(VisClear_OnCommand));
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)e.Mobile;

                pm.VisibilityList.Clear();
            }
        }

        [Usage("Vis")]
        [Description("Adds or removes a targeted player from your visibility list.  Anyone on your visibility list will be able to see you at all times, even when you're hidden.")]
        public static void Vis_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                e.Mobile.Target = new VisTarget();
                e.Mobile.SendMessage("Select person to add or remove from your visibility list.");
            }
        }

        [Usage("VisList")]
        [Description("Shows the names of everyone in your visibility list.")]
        public static void VisList_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)e.Mobile;
                List<Mobile> list = pm.VisibilityList;

                if (list.Count > 0)
                {
                    pm.SendMessage("You are visible to {0} mobile{1}:", list.Count, list.Count == 1 ? "" : "s");

                    for (int i = 0; i < list.Count; ++i)
                        pm.SendMessage("#{0}: {1}", i + 1, list[i].Name);
                }
                else
                {
                    pm.SendMessage("Your visibility list is empty.");
                }
            }
        }

        [Usage("VisClear")]
        [Description("Removes everyone from your visibility list.")]
        public static void VisClear_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)e.Mobile;
                List<Mobile> list = new List<Mobile>(pm.VisibilityList);
				
                pm.VisibilityList.Clear();
                pm.SendMessage("Your visibility list has been cleared.");

                for (int i = 0; i < list.Count; ++i)
                {
                    Mobile m = list[i];

                    if (!m.CanSee(pm) && Utility.InUpdateRange(m, pm))
                        m.Send(pm.RemovePacket);
                }
            }
        }

        private class VisTarget : Target
        {
            public VisTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from is PlayerMobile && targeted is Mobile)
                {
                    PlayerMobile pm = (PlayerMobile)from;
                    Mobile targ = (Mobile)targeted;

                    if (targ.AccessLevel <= from.AccessLevel)
                    {
                        List<Mobile> list = pm.VisibilityList;

                        if (list.Contains(targ))
                        {
                            list.Remove(targ);
                            from.SendMessage("{0} has been removed from your visibility list.", targ.Name);
                        }
                        else
                        {
                            list.Add(targ);
                            from.SendMessage("{0} has been added to your visibility list.", targ.Name);
                        }

                        if (Utility.InUpdateRange(targ, from))
                        {
                            NetState ns = targ.NetState;

                            if (ns != null)
                            {
                                if (targ.CanSee(from))
                                {
                                    if (ns.StygianAbyss)
                                        ns.Send(new MobileIncoming(targ, from));
                                    else
                                        ns.Send(new MobileIncomingOld(targ, from));

                                    if (ObjectPropertyList.Enabled)
                                    {
                                        ns.Send(from.OPLPacket);

                                        foreach (Item item in from.Items)
                                            ns.Send(item.OPLPacket);
                                    }
                                }
                                else
                                {
                                    ns.Send(from.RemovePacket);
                                }
                            }
                        }
                    }
                    else
                    {
                        from.SendMessage("They can already see you!");
                    }
                }
                else
                {
                    from.SendMessage("Add only mobiles to your visibility list.");
                }
            }
        }
    }
}