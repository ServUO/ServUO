using System;
using Server.Factions;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Engines.VvV;
using Server.Guilds;

namespace Server.SkillHandlers
{
    public class RemoveTrap
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (m.Skills[SkillName.Lockpicking].Value < 50)
            {
                m.SendLocalizedMessage(502366); // You do not know enough about locks.  Become better at picking locks.
            }
            else if (m.Skills[SkillName.DetectHidden].Value < 50)
            {
                m.SendLocalizedMessage(502367); // You are not perceptive enough.  Become better at detect hidden.
            }
            else
            {
                m.Target = new InternalTarget();

                m.SendLocalizedMessage(502368); // Wich trap will you attempt to disarm?
            }

            return TimeSpan.FromSeconds(10.0); // 10 second delay before beign able to re-use a skill
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    from.SendLocalizedMessage(502816); // You feel that such an action would be inappropriate
                }
                else if (targeted is TrapableContainer)
                {
                    TrapableContainer targ = (TrapableContainer)targeted;

                    from.Direction = from.GetDirectionTo(targ);

                    if (targ.TrapType == Server.Items.TrapType.None)
                    {
                        from.SendLocalizedMessage(502373); // That doesn't appear to be trapped
                        return;
                    }

                    from.PlaySound(0x241);
					
                    if (from.CheckTargetSkill(SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 30))
                    {
                        targ.TrapPower = 0;
                        targ.TrapLevel = 0;
                        targ.TrapType = Server.Items.TrapType.None;
                        from.SendLocalizedMessage(502377); // You successfully render the trap harmless
                    }
                    else
                    {
                        from.SendLocalizedMessage(502372); // You fail to disarm the trap... but you don't set it off
                    }
                }
                else if (targeted is BaseFactionTrap)
                {
                    BaseFactionTrap trap = (BaseFactionTrap)targeted;
                    Faction faction = Faction.Find(from);

                    FactionTrapRemovalKit kit = (from.Backpack == null ? null : from.Backpack.FindItemByType(typeof(FactionTrapRemovalKit)) as FactionTrapRemovalKit);

                    bool isOwner = (trap.Placer == from || (trap.Faction != null && trap.Faction.IsCommander(from)));

                    if (faction == null)
                    {
                        from.SendLocalizedMessage(1010538); // You may not disarm faction traps unless you are in an opposing faction
                    }
                    else if (faction == trap.Faction && trap.Faction != null && !isOwner)
                    {
                        from.SendLocalizedMessage(1010537); // You may not disarm traps set by your own faction!
                    }
                    else if (!isOwner && kit == null)
                    {
                        from.SendLocalizedMessage(1042530); // You must have a trap removal kit at the base level of your pack to disarm a faction trap.
                    }
                    else
                    {
                        if ((Core.ML && isOwner) || (from.CheckTargetSkill(SkillName.RemoveTrap, trap, 80.0, 100.0) && from.CheckTargetSkill(SkillName.Tinkering, trap, 80.0, 100.0)))
                        {
                            from.PrivateOverheadMessage(MessageType.Regular, trap.MessageHue, trap.DisarmMessage, from.NetState);

                            if (!isOwner)
                            {
                                int silver = faction.AwardSilver(from, trap.SilverFromDisarm);

                                if (silver > 0)
                                    from.SendLocalizedMessage(1008113, true, silver.ToString("N0")); // You have been granted faction silver for removing the enemy trap :
                            }

                            trap.Delete();
                        }
                        else
                        {
                            from.SendLocalizedMessage(502372); // You fail to disarm the trap... but you don't set it off
                        }

                        if (!isOwner && kit != null)
                            kit.ConsumeCharge(from);
                    }
                }
                else if (targeted is VvVTrap)
                {
                    VvVTrap trap = targeted as VvVTrap;

                    if (!ViceVsVirtueSystem.IsVvV(from))
                    {
                        from.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                    }
                    else
                    {
                        if (from == trap.Owner || ((from.Skills[SkillName.RemoveTrap].Value - 80.0) / 20.0) > Utility.RandomDouble())
                        {
                            VvVTrapKit kit = new VvVTrapKit(trap.TrapType);
                            trap.Delete();

                            if (!from.AddToBackpack(kit))
                                kit.MoveToWorld(from.Location, from.Map);

                            if (trap.Owner != null && from != trap.Owner)
                            {
                                Guild fromG = from.Guild as Guild;
                                Guild ownerG = trap.Owner.Guild as Guild;

                                if (fromG != null && fromG != ownerG && !fromG.IsAlly(ownerG) && ViceVsVirtueSystem.Instance != null 
                                    && ViceVsVirtueSystem.Instance.Battle != null && ViceVsVirtueSystem.Instance.Battle.OnGoing)
                                {
                                    ViceVsVirtueSystem.Instance.Battle.Update(from, UpdateType.Disarm);
                                }
                            }

                            from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1155413, from.NetState);
                        }
                        else if (.1 > Utility.RandomDouble())
                        {
                            trap.Detonate(from);
                        }
                    }
                }
                else if (targeted is GoblinFloorTrap)
                {
                    GoblinFloorTrap targ = (GoblinFloorTrap)targeted;

                    if (from.InRange(targ.Location, 3))
                    {
                        from.Direction = from.GetDirectionTo(targ);

                        if (targ.Owner == null)
                        {
                            Item item = new FloorTrapComponent();

                            if (from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
                                item.MoveToWorld(from.Location, from.Map);
                        }

                        targ.Delete();
                        from.SendLocalizedMessage(502377); // You successfully render the trap harmless
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502373); // That does'nt appear to be trapped
                }
            }
        }
    }
}