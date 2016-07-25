using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class FloorTrapKit : Item
    {
        public override int LabelNumber { get { return 1095135; } } // Floor trap Kit

        [Constructable]
        public FloorTrapKit()
            : base(0x4140)
        {
            Weight = 1.0;
            Stackable = true;
        }

        public FloorTrapKit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.Target = new InternalTarget(this);
        }

        private static bool CheckItems(Point3D loc, Map map, int range, int maxAmount, bool checkTraps)
        {
            var eable = map.GetItemsInRange(loc, range);
            int amount = 0;

            foreach (Item item in eable)
            {
                if (!checkTraps || item is FloorTrap)
                    amount++;
            }

            return amount < maxAmount;
        }

        private static Dictionary<Mobile, Timer> m_Assembling = new Dictionary<Mobile, Timer>();

        public static bool IsAssembling(Mobile m)
        {
            return m_Assembling.ContainsKey(m);
        }

        public static void StopAssembling(Mobile m, int message)
        {
            Timer t = m_Assembling[m];
            t.Stop();

            m_Assembling.Remove(m);

            m.SendLocalizedMessage(message);
        }

        private class InternalTarget : Target
        {
            private FloorTrapKit m_Trap;

            public InternalTarget(FloorTrapKit trap)
                : base(1, true, TargetFlags.None)
            {
                m_Trap = trap;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                LandTarget land = targeted as LandTarget;
                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                if (from.Skills[SkillName.Tinkering].Value < 60.0)
                    from.SendLocalizedMessage(1113318); // You do not have enough skill to set the trap.
                else if (IsAssembling(from))
                    from.SendLocalizedMessage(1113317); // You can only build one trap at a time.
                else if (m_Trap.Deleted || !m_Trap.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1113316); // You don't have the trap.
                else if (from.Flying || from.Mounted)
                    from.SendLocalizedMessage(1113319); // You cannot set the trap while riding or flying.
                else if (land == null)
                    from.SendLocalizedMessage(1113311); // There is something in the way.
                else if (pm.FloorTrapsPlaced >= 7)
                    from.SendLocalizedMessage(1113312); // You have the maximum number of traps active (limit 7). Remove one or wait for one to expire before trying again.
                else if (!CheckItems(land.Location, from.Map, 0, 1, false))
                    from.SendLocalizedMessage(1113311); // There is something in the way.
                else if (!CheckItems(land.Location, from.Map, 20, 7, true))
                    from.SendLocalizedMessage(1113320); // There are too many traps in this area (limit 7).
                else if (!CheckItems(land.Location, from.Map, 40, 14, true))
                    from.SendLocalizedMessage(1113313); // There are too many traps in this area (limit 14).
                else
                {
                    from.PlaySound(0x241);
                    from.SendLocalizedMessage(1113295); // You begin assembling the goblin trap.

                    Timer t = Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(
                        delegate
                        {
                            if (m_Trap.Deleted || !m_Trap.IsChildOf(from.Backpack))
                                from.SendLocalizedMessage(1113316); // You don't have the trap.
                            else
                            {
                                from.SendLocalizedMessage(1113294); // You carefully arm the goblin trap.
                                from.SendLocalizedMessage(1113297); // You hide the trap to the best of your ability.

                                FloorTrap trap = new FloorTrap(from.Skills[SkillName.Tinkering].Value, from.Skills[SkillName.Hiding].Value, from);
                                trap.Map = from.Map;
                                trap.MoveToWorld(land.Location);

                                from.PlaySound(0x22F);
                                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(delegate { trap.Visible = false; }));

                                m_Trap.Consume();

                                pm.FloorTrapsPlaced++;
                            }

                            m_Assembling.Remove(from);
                        }));

                    m_Assembling.Add(from, t);
                }
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502825); // That location is too far away
            }
        }
    }
}
