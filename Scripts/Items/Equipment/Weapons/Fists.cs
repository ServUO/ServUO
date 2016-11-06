using System;

namespace Server.Items
{
    public class Fists : BaseMeleeWeapon
    {
        public static void Initialize()
        {
            Mobile.DefaultWeapon = new Fists();

            EventSink.DisarmRequest += new DisarmRequestEventHandler(EventSink_DisarmRequest);
            EventSink.StunRequest += new StunRequestEventHandler(EventSink_StunRequest);
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }

        public override int AosStrengthReq
        {
            get
            {
                return 0;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 1;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 4;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 50;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.50f;
            }
        }

        public override int OldStrengthReq
        {
            get
            {
                return 0;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 1;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 8;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 30;
            }
        }

        public override int DefHitSound
        {
            get
            {
                return -1;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return -1;
            }
        }

        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Wrestling;
            }
        }
        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Fists;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Wrestle;
            }
        }

        public Fists()
            : base(0)
        {
            this.Visible = false;
            this.Movable = false;
            this.Quality = WeaponQuality.Regular;
        }

        public Fists(Serial serial)
            : base(serial)
        {
        }

        public override double GetDefendSkillValue(Mobile attacker, Mobile defender)
        {
            double wresValue = defender.Skills[SkillName.Wrestling].Value;
            double anatValue = defender.Skills[SkillName.Anatomy].Value;
            double evalValue = defender.Skills[SkillName.EvalInt].Value;
            double incrValue = (anatValue + evalValue + 20.0) * 0.5;

            if (incrValue > 120.0)
                incrValue = 120.0;

            if (wresValue > incrValue)
                return wresValue;
            else
                return incrValue;
        }

        private void CheckPreAOSMoves(Mobile attacker, Mobile defender)
        {
            if (attacker.StunReady)
            {
                if (attacker.CanBeginAction(typeof(Fists)))
                {
                    if (attacker.Skills[SkillName.Anatomy].Value >= 80.0 && attacker.Skills[SkillName.Wrestling].Value >= 80.0)
                    {
                        if (attacker.Stam >= 15)
                        {
                            attacker.Stam -= 15;

                            if (CheckMove(attacker, SkillName.Anatomy))
                            {
                                StartMoveDelay(attacker);

                                attacker.StunReady = false;

                                attacker.SendLocalizedMessage(1004013); // You successfully stun your opponent!
                                defender.SendLocalizedMessage(1004014); // You have been stunned!

                                defender.Freeze(TimeSpan.FromSeconds(4.0));
                            }
                            else
                            {
                                attacker.SendLocalizedMessage(1004010); // You failed in your attempt to stun.
                                defender.SendLocalizedMessage(1004011); // Your opponent tried to stun you and failed.
                            }
                        }
                        else
                        {
                            attacker.SendLocalizedMessage(1004009); // You are too fatigued to attempt anything.
                        }
                    }
                    else
                    {
                        attacker.SendLocalizedMessage(1004008); // You are not skilled enough to stun your opponent.
                        attacker.StunReady = false;
                    }
                }
            }
            else if (attacker.DisarmReady)
            {
                if (attacker.CanBeginAction(typeof(Fists)))
                {
                    if (defender.Player || defender.Body.IsHuman)
                    {
                        if (attacker.Skills[SkillName.ArmsLore].Value >= 80.0 && attacker.Skills[SkillName.Wrestling].Value >= 80.0)
                        {
                            if (attacker.Stam >= 15)
                            {
                                Item toDisarm = defender.FindItemOnLayer(Layer.OneHanded);

                                if (toDisarm == null || !toDisarm.Movable)
                                    toDisarm = defender.FindItemOnLayer(Layer.TwoHanded);

                                Container pack = defender.Backpack;

                                if (pack == null || toDisarm == null || !toDisarm.Movable)
                                {
                                    attacker.SendLocalizedMessage(1004001); // You cannot disarm your opponent.
                                }
                                else if (CheckMove(attacker, SkillName.ArmsLore))
                                {
                                    StartMoveDelay(attacker);

                                    attacker.Stam -= 15;
                                    attacker.DisarmReady = false;

                                    attacker.SendLocalizedMessage(1004006); // You successfully disarm your opponent!
                                    defender.SendLocalizedMessage(1004007); // You have been disarmed!

                                    pack.DropItem(toDisarm);
                                }
                                else
                                {
                                    attacker.Stam -= 15;

                                    attacker.SendLocalizedMessage(1004004); // You failed in your attempt to disarm.
                                    defender.SendLocalizedMessage(1004005); // Your opponent tried to disarm you but failed.
                                }
                            }
                            else
                            {
                                attacker.SendLocalizedMessage(1004003); // You are too fatigued to attempt anything.
                            }
                        }
                        else
                        {
                            attacker.SendLocalizedMessage(1004002); // You are not skilled enough to disarm your opponent.
                            attacker.DisarmReady = false;
                        }
                    }
                    else
                    {
                        attacker.SendLocalizedMessage(1004001); // You cannot disarm your opponent.
                    }
                }
            }
        }

        public override TimeSpan OnSwing(Mobile attacker, IDamageable defender)
        {
            if (!Core.AOS && defender is Mobile)
                this.CheckPreAOSMoves(attacker, (Mobile)defender);

            return base.OnSwing(attacker, defender);
        }

        /*public override void OnMiss( Mobile attacker, Mobile defender )
        {
        base.PlaySwingAnimation( attacker );
        }*/

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        /* Wrestling moves */

        private static bool CheckMove(Mobile m, SkillName other)
        {
            double wresValue = m.Skills[SkillName.Wrestling].Value;
            double scndValue = m.Skills[other].Value;

            /* 40% chance at 80, 80
            * 50% chance at 100, 100
            * 60% chance at 120, 120
            */

            double chance = (wresValue + scndValue) / 400.0;

            return (chance >= Utility.RandomDouble());
        }

        private static bool HasFreeHands(Mobile m)
        {
            Item item = m.FindItemOnLayer(Layer.OneHanded);

            if (item != null && !(item is Spellbook))
                return false;

            return m.FindItemOnLayer(Layer.TwoHanded) == null;
        }

        private static void EventSink_DisarmRequest(DisarmRequestEventArgs e)
        {
            if (Core.AOS)
                return;

            Mobile m = e.Mobile;

            #region Dueling
            if (!Engines.ConPVP.DuelContext.AllowSpecialAbility(m, "Disarm", true))
                return;
            #endregion

            double armsValue = m.Skills[SkillName.ArmsLore].Value;
            double wresValue = m.Skills[SkillName.Wrestling].Value;

            if (!HasFreeHands(m))
            {
                m.SendLocalizedMessage(1004029); // You must have your hands free to attempt to disarm your opponent.
                m.DisarmReady = false;
            }
            else if (armsValue >= 80.0 && wresValue >= 80.0)
            {
                m.DisruptiveAction();
                m.DisarmReady = !m.DisarmReady;
                m.SendLocalizedMessage(m.DisarmReady ? 1019013 : 1019014);
            }
            else
            {
                m.SendLocalizedMessage(1004002); // You are not skilled enough to disarm your opponent.
                m.DisarmReady = false;
            }
        }

        private static void EventSink_StunRequest(StunRequestEventArgs e)
        {
            if (Core.AOS)
                return;

            Mobile m = e.Mobile;

            #region Dueling
            if (!Engines.ConPVP.DuelContext.AllowSpecialAbility(m, "Stun", true))
                return;
            #endregion

            double anatValue = m.Skills[SkillName.Anatomy].Value;
            double wresValue = m.Skills[SkillName.Wrestling].Value;

            if (!HasFreeHands(m))
            {
                m.SendLocalizedMessage(1004031); // You must have your hands free to attempt to stun your opponent.
                m.StunReady = false;
            }
            else if (anatValue >= 80.0 && wresValue >= 80.0)
            {
                m.DisruptiveAction();
                m.StunReady = !m.StunReady;
                m.SendLocalizedMessage(m.StunReady ? 1019011 : 1019012);
            }
            else
            {
                m.SendLocalizedMessage(1004008); // You are not skilled enough to stun your opponent.
                m.StunReady = false;
            }
        }

        private class MoveDelayTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public MoveDelayTimer(Mobile m)
                : base(TimeSpan.FromSeconds(10.0))
            {
                this.m_Mobile = m;

                this.Priority = TimerPriority.TwoFiftyMS;

                this.m_Mobile.BeginAction(typeof(Fists));
            }

            protected override void OnTick()
            {
                this.m_Mobile.EndAction(typeof(Fists));
            }
        }

        private static void StartMoveDelay(Mobile m)
        {
            new MoveDelayTimer(m).Start();
        }
    }
}