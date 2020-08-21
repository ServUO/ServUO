using Server.Mobiles;
using Server.Network;
using Server.Spells.SkillMasteries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    /// <summary>
    /// Used for complex weapon/armor properties introduced in Stagyian Abyss
    /// </summary>
    public class PropertyEffect
    {
        public Mobile Attacker { get; set; }
        public Mobile Victim { get; set; }
        public Item Owner { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan TickDuration { get; set; }
        public Timer Timer { get; set; }

        public static List<PropertyEffect> Effects { get; set; } = new List<PropertyEffect>();

        public PropertyEffect(Mobile from, Mobile victim, Item owner, TimeSpan duration, TimeSpan tickduration)
        {
            Attacker = from;
            Victim = victim;
            Owner = owner;
            Duration = duration;
            TickDuration = tickduration;
        }

        protected static void AddEffects(PropertyEffect effect)
        {
            Effects.Add(effect);

            if (effect.TickDuration > TimeSpan.MinValue)
            {
                effect.StartTimer();
            }
        }

        public virtual void RemoveEffects()
        {
            StopTimer();

            if (Effects.Contains(this))
            {
                Effects.Remove(this);
            }
        }

        public void StartTimer()
        {
            if (Timer == null)
            {
                Timer = new InternalTimer(this);
                Timer.Start();
            }
        }

        public void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public bool IsEquipped()
        {
            if (Owner == null)
            {
                return false;
            }

            return Attacker.FindItemOnLayer(Owner.Layer) == Owner;
        }

        public virtual void OnTick()
        {
        }

        private class InternalTimer : Timer
        {
            private readonly PropertyEffect Effect;
            private readonly DateTime m_Expires;

            public InternalTimer(PropertyEffect effect)
                : base(effect.TickDuration, effect.TickDuration)
            {
                Effect = effect;

                if (effect != null && effect.Duration > TimeSpan.MinValue)
                    m_Expires = DateTime.UtcNow + effect.Duration;
                else
                    m_Expires = DateTime.MinValue;
            }

            protected override void OnTick()
            {
                if (Effect.Attacker == null || (Effect.Attacker.Deleted || !Effect.Attacker.Alive || Effect.Attacker.IsDeadBondedPet))
                {
                    Effect.RemoveEffects();
                }
                else if (Effect.Victim != null && (Effect.Victim.Deleted || !Effect.Victim.Alive || Effect.Attacker.IsDeadBondedPet))
                {
                    Effect.RemoveEffects();
                }
                else
                {
                    Effect.OnTick();

                    if (m_Expires > DateTime.MinValue && m_Expires <= DateTime.UtcNow)
                    {
                        Effect.RemoveEffects();
                    }
                }
            }
        }

        public static bool VictimIsUnderEffects<T>(Mobile from)
        {
            return Effects.Any(e => e.Victim == from && e.GetType() == typeof(T));
        }

        public static T GetContextForAttacker<T>(Mobile from) where T : PropertyEffect
        {
            return Effects.FirstOrDefault(e => e.Attacker == from && e.GetType() == typeof(T)) as T;
        }

        public static T GetContextForVictim<T>(Mobile from) where T : PropertyEffect
        {
            return Effects.FirstOrDefault(e => e.Victim == from && e.GetType() == typeof(T)) as T;
        }

        public static T GetContext<T>(Mobile from, Mobile victim) where T : PropertyEffect
        {
            return Effects.FirstOrDefault(e => e.Attacker == from && e.Victim == victim && e.GetType() == typeof(T)) as T;
        }

        public static IEnumerable<T> GetContextsForVictim<T>(Mobile victim) where T : PropertyEffect
        {
            foreach (PropertyEffect effect in Effects.OfType<T>().Where(e => e.Victim == victim))
            {
                yield return effect as T;
            }
        }
    }

    public class SoulChargeContext : PropertyEffect
    {
        private bool m_Active;

        public SoulChargeContext(Mobile from, Item item)
            : base(from, null, item, TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(40))
        {
            m_Active = true;
        }

        public void OnDamaged(int damage)
        {
            if (m_Active && IsEquipped() && Attacker != null)
            {
                double mod = BaseFishPie.IsUnderEffects(Attacker, FishPieEffect.SoulCharge) ? .50 : .30;
                Attacker.Mana += (int)Math.Min(Attacker.ManaMax, damage * mod);
                m_Active = false;

                Server.Effects.SendTargetParticles(Attacker, 0x375A, 0x1, 0xA, 0x71, 0x2, 0x1AE9, 0, 0);

                Attacker.SendLocalizedMessage(1113636); //The soul charge effect converts some of the damage you received into mana.
            }
        }

        public static void CheckHit(Mobile attacker, Mobile defender, int damage)
        {
            BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            if (shield != null && shield.ArmorAttributes.SoulCharge > 0 && shield.ArmorAttributes.SoulCharge > Utility.Random(100))
            {
                SoulChargeContext sc = GetContextForVictim<SoulChargeContext>(defender);

                if (sc == null)
                {
                    AddEffects(sc = new SoulChargeContext(defender, shield));
                }

                sc.OnDamaged(damage);
            }
        }
    }

    public enum DamageType
    {
        Kinetic,
        Fire,
        Cold,
        Poison,
        Energy,
        AllTypes
    }

    public class DamageEaterContext : PropertyEffect
    {
        private int m_Charges;

        public DamageEaterContext(Mobile mobile)
            : base(mobile, null, null, TimeSpan.MinValue, TimeSpan.FromSeconds(10))
        {
            m_Charges = 0;
        }

        public void OnDamage(int damage, int phys, int fire, int cold, int poison, int energy, int direct)
        {
            if (m_Charges >= 20)
                return;

            double pd = 0; double fd = 0;
            double cd = 0; double pod = 0;
            double ed = 0; double dd = 0;

            double k = (double)GetValue(DamageType.Kinetic, Attacker) / 100;
            double f = (double)GetValue(DamageType.Fire, Attacker) / 100;
            double c = (double)GetValue(DamageType.Cold, Attacker) / 100;
            double p = (double)GetValue(DamageType.Poison, Attacker) / 100;
            double e = (double)GetValue(DamageType.Energy, Attacker) / 100;
            double a = (double)GetValue(DamageType.AllTypes, Attacker) / 100;

            if (phys > 0 && (k > 0 || a > 0))
            {
                pd = damage * ((double)phys / 100);

                if (k >= a)
                    DelayHeal(Math.Min(pd * k, pd * .3));
                else
                    DelayHeal(Math.Min(pd * a, pd * .18));

                m_Charges++;
            }

            if (fire > 0 && (f > 0 || a > 0))
            {
                fd = damage * ((double)fire / 100);

                if (f >= a)
                    DelayHeal(Math.Min(fd * f, fd * .3));
                else
                    DelayHeal(Math.Min(fd * a, fd * .18));

                m_Charges++;
            }

            if (cold > 0 && (c > 0 || a > 0))
            {
                cd = damage * ((double)cold / 100);

                if (c >= a)
                    DelayHeal(Math.Min(cd * c, cd * .3));
                else
                    DelayHeal(Math.Min(cd * a, cd * .18));

                m_Charges++;
            }

            if (poison > 0 && (p > 0 || a > 0))
            {
                pod = damage * ((double)poison / 100);

                if (p >= a)
                    DelayHeal(Math.Min(pod * p, pod * .3));
                else
                    DelayHeal(Math.Min(pod * a, pod * .18));

                m_Charges++;
            }

            if (energy > 0 && (e > 0 || a > 0))
            {
                ed = damage * ((double)energy / 100);

                if (e >= a)
                    DelayHeal(Math.Min(ed * e, ed * .3));
                else
                    DelayHeal(Math.Min(ed * a, ed * .18));

                m_Charges++;
            }

            if (direct > 0 && a > 0)
            {
                dd = damage * ((double)direct / 100);

                DelayHeal(Math.Min(dd * a, dd * .18));
                m_Charges++;
            }
        }

        public void DelayHeal(double toHeal)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerStateCallback(DoHeal), toHeal);
        }

        public void DoHeal(object obj)
        {
            double dam = (double)obj;

            if (dam < 0)
                return;

            Attacker.Heal((int)dam, Attacker, false);
            Attacker.SendLocalizedMessage(1113617); // Some of the damage you received has been converted to heal you.
            Server.Effects.SendPacket(Attacker.Location, Attacker.Map, new ParticleEffect(EffectType.FixedFrom, Attacker.Serial, Serial.Zero, 0x375A, Attacker.Location, Attacker.Location, 1, 10, false, false, 33, 0, 2, 6889, 1, Attacker.Serial, 45, 0));
            m_Charges--;
        }

        public override void OnTick()
        {
            if (m_Charges <= 0)
                RemoveEffects();
        }

        public static bool HasValue(Mobile from)
        {
            if (GetValue(DamageType.Kinetic, from) > 0)
                return true;
            if (GetValue(DamageType.Fire, from) > 0)
                return true;
            if (GetValue(DamageType.Cold, from) > 0)
                return true;
            if (GetValue(DamageType.Poison, from) > 0)
                return true;
            if (GetValue(DamageType.Energy, from) > 0)
                return true;
            if (GetValue(DamageType.AllTypes, from) > 0)
                return true;
            return false;
        }

        public static int GetValue(DamageType type, Mobile from)
        {
            if (from == null)
                return 0;

            switch (type)
            {
                case DamageType.Kinetic: return SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterKinetic);
                case DamageType.Fire: return SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterFire);
                case DamageType.Cold: return SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterCold);
                case DamageType.Poison: return SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterPoison);
                case DamageType.Energy: return SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterEnergy);
                case DamageType.AllTypes: return SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterDamage);
            }
            return 0;
        }

        public static void CheckDamage(Mobile from, int damage, int phys, int fire, int cold, int pois, int ergy, int direct)
        {
            DamageEaterContext context = GetContextForVictim<DamageEaterContext>(from);

            if (context == null && HasValue(from))
                AddEffects(context = new DamageEaterContext(from));

            if (context != null)
                context.OnDamage(damage, phys, fire, cold, pois, ergy, direct);
        }
    }

    public class SplinteringWeaponContext : PropertyEffect
    {
        public static List<Mobile> BleedImmune { get; set; } = new List<Mobile>();

        public SplinteringWeaponContext(Mobile from, Mobile defender, Item weapon)
            : base(from, defender, weapon, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4))
        {
            StartForceWalk(defender);

            if (!(defender is PlayerMobile) || !IsBleedImmune(defender))
            {
                BleedAttack.BeginBleed(defender, from, true);
                AddBleedImmunity(defender);
            }

            defender.SendLocalizedMessage(1112486); // A shard of the brittle weapon has become lodged in you!
            from.SendLocalizedMessage(1113077); // A shard of your blade breaks off and sticks in your opponent!

            Server.Effects.PlaySound(defender.Location, defender.Map, 0x1DF);

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.SplinteringEffect, 1154670, 1152144, TimeSpan.FromSeconds(4), defender));
        }

        public override void OnTick()
        {
            base.OnTick();

            BuffInfo.RemoveBuff(Victim, BuffIcon.SplinteringEffect);
        }

        public void StartForceWalk(Mobile m)
        {
            if (m.NetState != null && m.AccessLevel < AccessLevel.GameMaster)
                m.SendSpeedControl(SpeedControlType.WalkSpeed);
        }

        public void EndForceWalk(Mobile m)
        {
            m.SendSpeedControl(SpeedControlType.Disable);
        }

        public override void RemoveEffects()
        {
            EndForceWalk(Victim);
            Victim.SendLocalizedMessage(1112487); // The shard is successfully removed.

            base.RemoveEffects();
        }

        public static bool CheckHit(Mobile attacker, Mobile defender, WeaponAbility ability, Item weapon)
        {
            if (defender == null || ability == WeaponAbility.Disarm || ability == WeaponAbility.InfectiousStrike || SkillMasterySpell.HasSpell(attacker, typeof(SkillMasterySpell)) || VictimIsUnderEffects<SplinteringWeaponContext>(defender))
                return false;

            SplinteringWeaponContext context = GetContext<SplinteringWeaponContext>(attacker, defender);

            if (context == null)
            {
                AddEffects(new SplinteringWeaponContext(attacker, defender, weapon));
                return true;
            }

            return false;
        }

        public static bool IsBleedImmune(Mobile m)
        {
            return BleedImmune.Contains(m);
        }

        public static void AddBleedImmunity(Mobile m)
        {
            if (!(m is PlayerMobile) || BleedImmune.Contains(m))
                return;

            BleedImmune.Add(m);
            Timer.DelayCall(TimeSpan.FromSeconds(16), () => BleedImmune.Remove(m));
        }
    }

    public class SearingWeaponContext : PropertyEffect
    {
        public static int Damage => Utility.RandomMinMax(10, 15);

        public SearingWeaponContext(Mobile from, Mobile defender)
            : base(from, defender, null, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4))
        {
            from.SendLocalizedMessage(1151177); //The searing attack cauterizes the wound on impact.
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            SearingWeaponContext context = GetContext<SearingWeaponContext>(attacker, defender);

            if (context == null)
                AddEffects(new SearingWeaponContext(attacker, defender));
        }

        public static bool HasContext(Mobile defender)
        {
            return VictimIsUnderEffects<SearingWeaponContext>(defender);
        }
    }

    public class BoneBreakerContext : PropertyEffect
    {
        public static Dictionary<Mobile, DateTime> _Immunity;
        private static TimeSpan _EffectsDuration = TimeSpan.FromSeconds(4);
        private static TimeSpan _ImmunityDuration = TimeSpan.FromSeconds(60);

        public BoneBreakerContext(Mobile attacker, Mobile defender, Item weapon)
            : base(attacker, defender, weapon, _EffectsDuration, TimeSpan.FromSeconds(1))
        {
        }

        public override void OnTick()
        {
            int toReduce = Victim.StamMax / 10;

            if (Victim.Stam - toReduce >= 3)
                Victim.Stam -= toReduce;
            else
                Victim.Stam = Math.Max(1, Victim.Stam - 1);
        }

        public override void RemoveEffects()
        {
            base.RemoveEffects();

            BuffInfo.RemoveBuff(Victim, BuffIcon.BoneBreaker);

            AddImmunity(Victim);
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            BoneBreakerContext context = GetContext<BoneBreakerContext>(attacker, defender);

            if (IsImmune(defender) || context != null)
            {
                attacker.SendLocalizedMessage(1157316); // Your target is currently immune to bone breaking!
            }
            else if (20 > Utility.Random(100))
            {
                AddEffects(new BoneBreakerContext(attacker, defender, null));
                defender.SendLocalizedMessage(1157317); // The attack shatters your bones!

                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.BoneBreaker, 1157318, 1157363));
                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.BoneBreakerImmune, 1157318, 1157364, _EffectsDuration + _ImmunityDuration, defender));

                defender.PlaySound(0x204);
                defender.FixedEffect(0x376A, 9, 32);

                defender.FixedEffect(0x3779, 10, 20, 1365, 0);
            }
            else if (attacker.Skills[SkillName.Tactics].Value >= 60.0)
            {
                int mana = (int)(30.0 * ((AosAttributes.GetValue(attacker, AosAttribute.LowerManaCost) + BaseArmor.GetInherentLowerManaCost(attacker)) / 100.0));

                if (attacker.Mana >= mana)
                {
                    attacker.Mana -= mana;
                    AOS.Damage(defender, attacker, 100, 100, 0, 0, 0, 0);

                    defender.SendLocalizedMessage(1157317); // The attack shatters your bones!
                }
            }
        }

        public static bool IsImmune(Mobile m)
        {
            if (_Immunity == null)
                return false;

            List<Mobile> list = new List<Mobile>(_Immunity.Keys);

            foreach (Mobile mob in list)
            {
                if (_Immunity[mob] < DateTime.UtcNow)
                    _Immunity.Remove(mob);
            }

            ColUtility.Free(list);

            return _Immunity.ContainsKey(m);
        }

        public static void AddImmunity(Mobile m)
        {
            if (_Immunity == null)
                _Immunity = new Dictionary<Mobile, DateTime>();

            _Immunity[m] = DateTime.UtcNow + _ImmunityDuration;
        }
    }

    public class SwarmContext : PropertyEffect
    {
        public static Dictionary<Mobile, DateTime> _Immunity;

        private readonly int _ID;

        public SwarmContext(Mobile attacker, Mobile defender, Item weapon)
            : base(attacker, defender, weapon, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(5))
        {
            _ID = Utility.RandomMinMax(2331, 2339);

            DoEffects();
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            if (IsImmune(defender))
            {
                attacker.SendLocalizedMessage(1157322); // Your target is currently immune to swarm!
                return;
            }

            SwarmContext context = GetContext<SwarmContext>(attacker, defender);

            if (context != null)
            {
                context.RemoveEffects();
            }

            AddEffects(context = new SwarmContext(attacker, defender, null));

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.Swarm, 1157328, 1157362));

            defender.NonlocalOverheadMessage(MessageType.Regular, 0x5C, 1114447, defender.Name); // * ~1_NAME~ is stung by a swarm of insects *
            defender.LocalOverheadMessage(MessageType.Regular, 0x5C, 1071905); // * The swarm of insects bites and stings your flesh! *
        }

        public override void OnTick()
        {
            if (Victim == null || !Victim.Alive)
            {
                RemoveEffects();
                return;
            }

            if (Victim.FindItemOnLayer(Layer.OneHanded) is Torch)
            {
                if (Victim.NetState != null)
                    Victim.LocalOverheadMessage(MessageType.Regular, 0x61, 1071925); // * The open flame begins to scatter the swarm of insects! *
            }
            else
            {
                DoEffects();
            }
        }

        private void DoEffects()
        {
            AOS.Damage(Victim, Attacker, 10, 0, 0, 0, 0, 0, 0, 100);
            Victim.SendLocalizedMessage(1157362); // Biting insects are attacking you!
            Server.Effects.SendTargetEffect(Victim, _ID, 40);

            Victim.PlaySound(0x00E);
            Victim.PlaySound(0x1BC);
        }

        public override void RemoveEffects()
        {
            base.RemoveEffects();

            BuffInfo.RemoveBuff(Victim, BuffIcon.Swarm);
            //AddImmunity(Victim);
        }

        public static void CheckRemove(Mobile victim)
        {
            ColUtility.ForEach(GetContextsForVictim<SwarmContext>(victim), context =>
            {
                context.RemoveEffects();
            });
        }

        public static bool IsImmune(Mobile m)
        {
            if (_Immunity == null)
                return false;

            List<Mobile> list = new List<Mobile>(_Immunity.Keys);

            foreach (Mobile mob in list)
            {
                if (_Immunity[mob] < DateTime.UtcNow)
                    _Immunity.Remove(mob);
            }

            ColUtility.Free(list);

            return _Immunity.ContainsKey(m);
        }

        public static void AddImmunity(Mobile m)
        {
            if (_Immunity == null)
                _Immunity = new Dictionary<Mobile, DateTime>();

            _Immunity[m] = DateTime.UtcNow + TimeSpan.FromSeconds(60);
        }
    }

    public class SparksContext : PropertyEffect
    {
        public static Dictionary<Mobile, DateTime> _Immunity;

        public SparksContext(Mobile attacker, Mobile defender, Item weapon)
            : base(attacker, defender, weapon, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
        {
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            if (IsImmune(defender))
            {
                attacker.SendLocalizedMessage(1157324); // Your target is currently immune to sparks!
                return;
            }

            SparksContext context = GetContext<SparksContext>(attacker, defender);

            if (context == null)
            {
                AddEffects(context = new SparksContext(attacker, defender, null));

                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.Sparks, 1157330, 1157361));

                attacker.PlaySound(0x20A);
                defender.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
            }
        }

        public override void OnTick()
        {
            AOS.Damage(Victim, Attacker, Utility.RandomMinMax(20, 40), 0, 0, 0, 0, 100);
        }

        public override void RemoveEffects()
        {
            base.RemoveEffects();

            BuffInfo.RemoveBuff(Victim, BuffIcon.Sparks);
            //AddImmunity(Victim);
        }

        public static bool IsImmune(Mobile m)
        {
            if (_Immunity == null)
                return false;

            List<Mobile> list = new List<Mobile>(_Immunity.Keys);

            foreach (Mobile mob in list)
            {
                if (_Immunity[mob] < DateTime.UtcNow)
                    _Immunity.Remove(mob);
            }

            ColUtility.Free(list);

            return _Immunity.ContainsKey(m);
        }

        public static void AddImmunity(Mobile m)
        {
            if (_Immunity == null)
                _Immunity = new Dictionary<Mobile, DateTime>();

            _Immunity[m] = DateTime.UtcNow + TimeSpan.FromSeconds(60);
        }
    }
}
