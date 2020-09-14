using Server.Network;
using Server.Regions;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class GoblinFloorTrap : BaseTrap, IRevealableItem
    {
        private Mobile m_Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        public override int LabelNumber => 1113296;  // Armed Floor Trap
        public bool CheckWhenHidden => true;

        [Constructable]
        public GoblinFloorTrap() : this(null)
        {
        }

        [Constructable]
        public GoblinFloorTrap(Mobile from) : base(0x4004)
        {
            m_Owner = from;
            Visible = false;
        }

        public override bool PassivelyTriggered => true;
        public override TimeSpan PassiveTriggerDelay => TimeSpan.FromSeconds(1.0);
        public override int PassiveTriggerRange => 1;
        public override TimeSpan ResetDelay => TimeSpan.FromSeconds(1.0);

        public override void OnTrigger(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player || !from.Alive)
                return;

            if (m_Owner != null)
            {
                if (!m_Owner.CanBeHarmful(from) || m_Owner == from)
                    return;

                if (m_Owner.Guild != null && m_Owner.Guild == from.Guild)
                    return;
            }

            from.SendSound(0x22B);
            from.SendLocalizedMessage(1095157); // You stepped onto a goblin trap!

            Spells.SpellHelper.Damage(TimeSpan.FromSeconds(0.30), from, from, Utility.RandomMinMax(50, 75), 100, 0, 0, 0, 0);

            if (m_Owner != null)
                from.DoHarmful(m_Owner);

            Visible = true;
            Timer.DelayCall(TimeSpan.FromSeconds(10), Rehide_Callback);

            PublicOverheadMessage(MessageType.Regular, 0x65, 500813); // [Trapped]

            new Blood().MoveToWorld(from.Location, from.Map);
        }

        public virtual bool CheckReveal(Mobile m)
        {
            return m.CheckTargetSkill(SkillName.DetectHidden, this, 50.0, 100.0);
        }

        public virtual void OnRevealed(Mobile m)
        {
            Unhide();
        }

        public virtual bool CheckPassiveDetect(Mobile m)
        {
            if (Visible && 0.05 > Utility.RandomDouble())
            {
                if (m.NetState != null)
                {
                    Packet p = new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x65, 3, 500813, Name, string.Empty);
                    p.Acquire();
                    m.NetState.Send(p);
                    Packet.Release(p);

                    return true;
                }
            }

            return false;
        }

        public void Unhide()
        {
            Visible = true;

            Timer.DelayCall(TimeSpan.FromSeconds(10), Rehide_Callback);
        }

        public void Rehide_Callback()
        {
            Visible = false;
        }

        public GoblinFloorTrap(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Owner = reader.ReadMobile();
        }
    }

    public class GoblinFloorTrapKit : Item
    {
        [Constructable]
        public GoblinFloorTrapKit() : base(16704)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            Region r = from.Region;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
            }
            else if (from.Skills[SkillName.Tinkering].Value < 80)
            {
                from.SendLocalizedMessage(1113318); // You do not have enough skill to set the trap.
            }
            else if (from.Mounted || from.Flying)
            {
                from.SendLocalizedMessage(1113319); // You cannot set the trap while riding or flying.
            }
            else if (r is GuardedRegion && !((GuardedRegion)r).IsDisabled())
            {
                from.SendMessage("You cannot place a trap in a guard region.");
            }
            else
            {
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly GoblinFloorTrapKit m_Kit;

            public InternalTarget(GoblinFloorTrapKit kit) : base(-1, false, TargetFlags.None)
            {
                m_Kit = kit;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is IPoint3D)
                {
                    Point3D p = new Point3D((IPoint3D)targeted);
                    Region r = Region.Find(p, from.Map);

                    if (from.Skills[SkillName.Tinkering].Value < 80)
                    {
                        from.SendLocalizedMessage(1113318); // You do not have enough skill to set the trap.
                    }
                    else if (from.Mounted || from.Flying)
                    {
                        from.SendLocalizedMessage(1113319); // You cannot set the trap while riding or flying.
                    }
                    else if (r is GuardedRegion && !((GuardedRegion)r).IsDisabled())
                    {
                        from.SendMessage("You cannot place a trap in a guard region.");
                    }
                    if (from.InRange(p, 2))
                    {
                        GoblinFloorTrap trap = new GoblinFloorTrap(from);

                        trap.MoveToWorld(p, from.Map);
                        from.SendLocalizedMessage(1113294);  // You carefully arm the goblin trap.
                        from.SendLocalizedMessage(1113297);  // You hide the trap to the best of your ability.            

                        m_Kit.Consume();
                    }
                    else
                        from.SendLocalizedMessage(500446); // That is too far away.
                }
            }
        }

        public GoblinFloorTrapKit(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

