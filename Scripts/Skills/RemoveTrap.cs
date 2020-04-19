using Server.Engines.VvV;
using Server.Guilds;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.SkillHandlers
{
    public interface IRemoveTrapTrainingKit
    {
        void OnRemoveTrap(Mobile m);
    }

    public class RemoveTrap
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = OnUse;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTarget();

            m.SendLocalizedMessage(502368); // Wich trap will you attempt to disarm?

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
                else if (targeted is IRemoveTrapTrainingKit)
                {
                    ((IRemoveTrapTrainingKit)targeted).OnRemoveTrap(from);
                }
                else if (targeted is LockableContainer && ((LockableContainer)targeted).Locked)
                {
                    from.SendLocalizedMessage(501283); // That is locked.
                }
                else if (targeted is TrapableContainer)
                {
                    TrapableContainer targ = (TrapableContainer)targeted;

                    from.Direction = from.GetDirectionTo(targ);

                    if (targ.TrapType == TrapType.None)
                    {
                        from.SendLocalizedMessage(502373); // That doesn't appear to be trapped
                    }
                    else if (targ is TreasureMapChest && TreasureMapInfo.NewSystem)
                    {
                        TreasureMapChest tChest = (TreasureMapChest)targ;

                        if (tChest.Owner != from)
                        {
                            from.SendLocalizedMessage(1159010); // That is not your chest!
                        }
                        else if (IsDisarming(from, tChest))
                        {
                            from.SendLocalizedMessage(1159059); // You are already manipulating the trigger mechanism...
                        }
                        else if (IsBeingDisarmed(tChest))
                        {
                            from.SendLocalizedMessage(1159063); // That trap is already being disarmed.
                        }
                        else if (tChest.AncientGuardians.Any(g => !g.Deleted))
                        {
                            from.PrivateOverheadMessage(MessageType.Regular, 1150, 1159060, from.NetState); // *Your attempt fails as the the mechanism jams and you are attacked by an Ancient Chest Guardian!*
                        }
                        else
                        {
                            from.PlaySound(0x241);

                            from.PrivateOverheadMessage(MessageType.Regular, 1150, 1159057, from.NetState); // *You delicately manipulate the trigger mechanism...*

                            StartChestDisarmTimer(from, tChest);
                        }
                    }
                    else
                    {
                        from.PlaySound(0x241);

                        if (from.CheckTargetSkill(SkillName.RemoveTrap, targ, targ.TrapPower - 10, targ.TrapPower + 10))
                        {
                            targ.TrapPower = 0;
                            targ.TrapLevel = 0;
                            targ.TrapType = TrapType.None;
                            targ.InvalidateProperties();
                            from.SendLocalizedMessage(502377); // You successfully render the trap harmless
                        }
                        else
                        {
                            from.SendLocalizedMessage(502372); // You fail to disarm the trap... but you don't set it off
                        }
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

                            from.PrivateOverheadMessage(MessageType.Regular, 1154, 1155413, from.NetState);
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

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                if (targeted is TreasureMapChest && TreasureMapInfo.NewSystem)
                {
                    // put here to prevent abuse
                    if (from.NextSkillTime > Core.TickCount)
                    {
                        from.NextSkillTime = Core.TickCount;
                    }

                    from.SendLocalizedMessage(1159058); // You are too far away from the chest to manipulate the trigger mechanism.
                }
                else
                {
                    base.OnTargetOutOfRange(from, targeted);
                }
            }
        }

        public static Dictionary<Mobile, RemoveTrapTimer> _Table;

        public static void StartChestDisarmTimer(Mobile from, TreasureMapChest chest)
        {
            if (_Table == null)
            {
                _Table = new Dictionary<Mobile, RemoveTrapTimer>();
            }

            _Table[from] = new RemoveTrapTimer(from, chest, from.Skills[SkillName.RemoveTrap].Value >= 100);
        }

        public static void EndChestDisarmTimer(Mobile from)
        {
            if (_Table != null && _Table.ContainsKey(from))
            {
                RemoveTrapTimer timer = _Table[from];

                if (timer != null)
                {
                    timer.Stop();
                }

                _Table.Remove(from);

                if (_Table.Count == 0)
                {
                    _Table = null;
                }
            }
        }

        public static bool IsDisarming(Mobile from, TreasureMapChest chest)
        {
            if (_Table == null)
                return false;

            return _Table.ContainsKey(from);
        }

        public static bool IsBeingDisarmed(TreasureMapChest chest)
        {
            if (_Table == null)
                return false;

            return _Table.Values.Any(timer => timer.Chest == chest);
        }
    }

    public class RemoveTrapTimer : Timer
    {
        public Mobile From { get; set; }
        public TreasureMapChest Chest { get; set; }

        public DateTime SafetyEndTime { get; set; } // Used for 100 Remove Trap
        public int Stage { get; set; } // Used for 99.9- Remove Trap

        public bool GMRemover { get; set; }

        public RemoveTrapTimer(Mobile from, TreasureMapChest chest, bool gmRemover)
            : base(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
        {
            From = from;
            Chest = chest;
            GMRemover = gmRemover;

            if (gmRemover)
            {
                TimeSpan duration;

                switch ((TreasureLevel)chest.Level)
                {
                    default:
                    case TreasureLevel.Stash: duration = TimeSpan.FromSeconds(20); break;
                    case TreasureLevel.Supply: duration = TimeSpan.FromSeconds(60); break;
                    case TreasureLevel.Cache: duration = TimeSpan.FromSeconds(180); break;
                    case TreasureLevel.Hoard: duration = TimeSpan.FromSeconds(420); break;
                    case TreasureLevel.Trove: duration = TimeSpan.FromSeconds(540); break;
                }

                SafetyEndTime = Chest.DigTime + duration;
            }

            Start();
        }

        protected override void OnTick()
        {
            if (Chest.Deleted)
            {
                RemoveTrap.EndChestDisarmTimer(From);
            }
            if (!From.Alive)
            {
                From.SendLocalizedMessage(1159061); // Your ghostly fingers cannot manipulate the mechanism...
                RemoveTrap.EndChestDisarmTimer(From);
            }
            else if (!From.InRange(Chest.GetWorldLocation(), 16) || Chest.Deleted)
            {
                From.SendLocalizedMessage(1159058); // You are too far away from the chest to manipulate the trigger mechanism.
                RemoveTrap.EndChestDisarmTimer(From);
            }
            else if (GMRemover)
            {
                From.RevealingAction();

                if (SafetyEndTime < DateTime.UtcNow)
                {
                    DisarmTrap();
                }
                else
                {
                    if (From.CheckTargetSkill(SkillName.RemoveTrap, Chest, 80, 120 + (Chest.Level * 10)))
                    {
                        DisarmTrap();
                    }
                    else
                    {
                        Chest.SpawnAncientGuardian(From);
                    }
                }

                RemoveTrap.EndChestDisarmTimer(From);
            }
            else
            {
                From.RevealingAction();

                double min = Math.Ceiling(From.Skills[SkillName.RemoveTrap].Value * .75);

                if (From.CheckTargetSkill(SkillName.RemoveTrap, Chest, min, min > 50 ? min + 50 : 100))
                {
                    DisarmTrap();
                    RemoveTrap.EndChestDisarmTimer(From);
                }
                else
                {
                    Chest.SpawnAncientGuardian(From);

                    if (From.Alive)
                    {
                        From.PrivateOverheadMessage(MessageType.Regular, 1150, 1159057, From.NetState); // *You delicately manipulate the trigger mechanism...*
                    }
                }
            }
        }

        private void DisarmTrap()
        {
            Chest.TrapPower = 0;
            Chest.TrapLevel = 0;
            Chest.TrapType = TrapType.None;
            Chest.InvalidateProperties();

            From.PrivateOverheadMessage(MessageType.Regular, 1150, 1159009, From.NetState); // You successfully disarm the trap!
        }
    }
}
