using Server.Mobiles;
using Server.Network;
using Server.Spells.Ninjitsu;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class Bola : Item
    {
        [Constructable]
        public Bola()
            : this(1)
        {
        }

        [Constructable]
        public Bola(int amount)
            : base(0x26AC)
        {
            Weight = 4.0;
            Stackable = true;
            Amount = amount;
        }

        public Bola(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1040019, from.NetState); // The bola must be in your pack to use it.
            }
            else if (!from.CanBeginAction(typeof(Bola)))
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1049624, from.NetState); // // You have to wait a few moments before you can use another bola!
            }
            else if (from.Target is BolaTarget)
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1049631, from.NetState); // This bola is already being used.
            }
            else if (from.Mounted)
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1042053, from.NetState); // You can't use this while on a mount!
            }
            else if (from.Flying)
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1113414, from.NetState); // You can't use this while flying!
            }
            else if (AnimalForm.UnderTransformation(from))
            {
                PrivateOverheadMessage(MessageType.Regular, 946, 1070902, from.NetState); // You can't use this while in an animal form!
            }
            else
            {
                EtherealMount.StopMounting(from);

                Item one = from.FindItemOnLayer(Layer.OneHanded);
                Item two = from.FindItemOnLayer(Layer.TwoHanded);

                if (one != null)
                    from.AddToBackpack(one);

                if (two != null)
                    from.AddToBackpack(two);

                from.Target = new BolaTarget(this);
                from.LocalOverheadMessage(MessageType.Emote, 201, 1049632); // * You begin to swing the bola...*
                from.NonlocalOverheadMessage(MessageType.Emote, 201, 1049633, from.Name); // ~1_NAME~ begins to menacingly swing a bola...
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private static void ReleaseBolaLock(object state)
        {
            ((Mobile)state).EndAction(typeof(Bola));
        }

        private static void FinishThrow(object state)
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[0];
            Mobile to = (Mobile)states[1];
            Item bola = (Item)states[2];

            if (!from.Alive)
            {
                return;
            }
            if (!bola.IsChildOf(from.Backpack))
            {
                bola.PrivateOverheadMessage(MessageType.Regular, 946, 1040019, from.NetState); // The bola must be in your pack to use it.
            }
            else if (!from.InRange(to, 15) || !from.InLOS(to) || !from.CanSee(to))
            {
                from.PrivateOverheadMessage(MessageType.Regular, 946, 1042060, from.NetState); // You cannot see that target!
            }
            else if (!to.Mounted && !to.Flying && !AnimalForm.UnderTransformation(to))
            {
                to.PrivateOverheadMessage(MessageType.Regular, 946, 1049628, from.NetState); // You have no reason to throw a bola at that.
            }
            else
            {
                bola.Consume();

                from.Direction = from.GetDirectionTo(to);
                from.Animate(AnimationType.Attack, 4);
                from.MovingEffect(to, 0x26AC, 10, 0, false, false);

                new Bola().MoveToWorld(to.Location, to.Map);

                if (to is Neira || to is ChaosDragoon || to is ChaosDragoonElite)
                {
                    to.PrivateOverheadMessage(MessageType.Regular, 946, 1042047, from.NetState); // You fail to knock the rider from its mount.
                }
                else
                {
                    if (CheckHit(to, from))
                    {
                        to.Damage(Utility.RandomMinMax(10, 20), from);

                        if (from.Flying)
                            to.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113590, from.Name); // You have been grounded by ~1_NAME~!
                        else
                            to.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1049623, from.Name); // You have been knocked off of your mount by ~1_NAME~!

                        BaseMount.Dismount(to);

                        BaseMount.SetMountPrevention(to, BlockMountType.Dazed, TimeSpan.FromSeconds(10.0));
                    }
                }
            }
        }

        private static bool CheckHit(Mobile to, Mobile from)
        {
            double toChance = Math.Min(45 + BaseArmor.GetRefinedDefenseChance(to),
                                       AosAttributes.GetValue(to, AosAttribute.DefendChance)) + 1;

            double fromChance = AosAttributes.GetValue(from, AosAttribute.AttackChance) + 1;

            double hitChance = fromChance / (toChance * 2);

            if (Utility.RandomDouble() < hitChance)
            {
                if (BaseWeapon.CheckParry(to))
                {
                    to.FixedEffect(0x37B9, 10, 16);
                    to.Animate(AnimationType.Parry, 0);
                    return false;
                }

                return true;
            }

            to.NonlocalOverheadMessage(MessageType.Emote, 0x3B2, false, "*miss*");
            return false;
        }

        private class BolaTarget : Target
        {
            private readonly Bola m_Bola;
            public BolaTarget(Bola bola)
                : base(20, false, TargetFlags.Harmful)
            {
                m_Bola = bola;
            }

            protected override void OnTarget(Mobile from, object obj)
            {
                if (m_Bola.Deleted)
                    return;

                if ((obj is Item))
                {
                    ((Item)obj).PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1049628, from.NetState); // You have no reason to throw a bola at that.
                    return;
                }

                if (obj is Mobile)
                {
                    Mobile to = (Mobile)obj;

                    if (!m_Bola.IsChildOf(from.Backpack))
                    {
                        m_Bola.PrivateOverheadMessage(MessageType.Regular, 946, 1040019, from.NetState); // The bola must be in your pack to use it.
                    }
                    else if (from.Mounted)
                    {
                        m_Bola.PrivateOverheadMessage(MessageType.Regular, 946, 1042053, from.NetState); // You can't use this while on a mount!
                    }
                    else if (from.Flying)
                    {
                        m_Bola.PrivateOverheadMessage(MessageType.Regular, 946, 1113414, from.NetState); // You can't use this while flying!
                    }
                    else if (from == to)
                    {
                        from.SendLocalizedMessage(1005576); // You can't throw this at yourself.
                    }
                    else if (AnimalForm.UnderTransformation(from))
                    {
                        from.PrivateOverheadMessage(MessageType.Regular, 946, 1070902, from.NetState); // You can't use this while in an animal form!
                    }
                    else if (!to.Mounted && !to.Flying && !AnimalForm.UnderTransformation(to))
                    {
                        to.PrivateOverheadMessage(MessageType.Regular, 946, 1049628, from.NetState); // You have no reason to throw a bola at that.
                    }
                    else if (!from.CanBeHarmful(to))
                    {
                    }
                    else if (from.BeginAction(typeof(Bola)))
                    {
                        from.RevealingAction();

                        EtherealMount.StopMounting(from);

                        Item one = from.FindItemOnLayer(Layer.OneHanded);
                        Item two = from.FindItemOnLayer(Layer.TwoHanded);

                        if (one != null)
                            from.AddToBackpack(one);

                        if (two != null)
                            from.AddToBackpack(two);

                        from.DoHarmful(to);

                        BaseMount.SetMountPrevention(from, BlockMountType.BolaRecovery, TimeSpan.FromSeconds(10.0));
                        Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerStateCallback(ReleaseBolaLock), from);
                        Timer.DelayCall(TimeSpan.FromSeconds(3.0), new TimerStateCallback(FinishThrow), new object[] { from, to, m_Bola });
                    }
                    else
                    {
                        m_Bola.PrivateOverheadMessage(MessageType.Regular, 946, 1049624, from.NetState); // You have to wait a few moments before you can use another bola!
                    }
                }
            }
        }
    }
}
