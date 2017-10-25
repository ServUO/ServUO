using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Ninjitsu;
using Server.Network;
using System.Linq;

namespace Server.Items
{
    public enum EffectsType
    {
        BattleLust,
        SoulCharge,
        DamageEater,
        Splintering,
        Searing,
        Bane,
        BoneBreaker, 
        Swarm,
        Sparks,
    }

    /// <summary>
    /// Used for complex weapon/armor properties introduced in Stagyian Abyss
    /// </summary>
    public class PropertyEffect
    {
        private Mobile m_Mobile;
        private Mobile m_Victim;
        private Item m_Owner;
        private EffectsType m_Effect;
        private TimeSpan m_Duration;
        private TimeSpan m_TickDuration;
        private Timer m_Timer;

        public Mobile Mobile { get { return m_Mobile; } }
        public Mobile Victim { get { return m_Victim; } }
        public Item Owner { get { return m_Owner; } }
        public EffectsType Effect { get { return m_Effect; } }
        public TimeSpan Duration { get { return m_Duration; } }
        public TimeSpan TickDuration { get { return m_TickDuration; } }
        public Timer Timer { get { return m_Timer; } }

        private static List<PropertyEffect> m_Effects = new List<PropertyEffect>();
        public static List<PropertyEffect> Effects { get { return m_Effects; } }

        public PropertyEffect(Mobile from, Mobile victim, Item owner, EffectsType effect, TimeSpan duration, TimeSpan tickduration)
        {
            m_Mobile = from;
            m_Victim = victim;
            m_Owner = owner;
            m_Effect = effect;
            m_Duration = duration;
            m_TickDuration = tickduration;

            m_Effects.Add(this);

            if (m_TickDuration > TimeSpan.MinValue)
                StartTimer();
        }

        public virtual void RemoveEffects()
        {
            StopTimer();

            if(m_Effects.Contains(this))
                m_Effects.Remove(this);
        }

        public void StartTimer()
        {
            if (m_Timer == null)
            {
                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }
        }

        public void StopTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public bool IsEquipped()
        {
            if (m_Owner == null)
                return false;

            return m_Mobile.FindItemOnLayer(m_Owner.Layer) == m_Owner;
        }

        public virtual void OnTick()
        {
        }

        public virtual void OnDamaged(int damage)
        {
        }

        public virtual void OnDamage(int damage, int phys, int fire, int cold, int poison, int energy, int direct)
        {
        }

        private class InternalTimer : Timer
        {
            private PropertyEffect m_Effect;
            private DateTime m_Expires;

            public InternalTimer(PropertyEffect effect)
                : base(effect.TickDuration, effect.TickDuration)
            {
                m_Effect = effect;

                if (effect != null && effect.Duration > TimeSpan.MinValue)
                    m_Expires = DateTime.UtcNow + effect.Duration;
                else
                    m_Expires = DateTime.MinValue;
            }

            protected override void OnTick()
            {
                if (m_Effect.Mobile == null || (m_Effect.Mobile.Deleted || !m_Effect.Mobile.Alive || m_Effect.Mobile.IsDeadBondedPet))
                {
                    m_Effect.RemoveEffects();
                }
                else if (m_Effect.Victim != null && (m_Effect.Victim.Deleted || !m_Effect.Victim.Alive || m_Effect.Mobile.IsDeadBondedPet))
                {
                    m_Effect.RemoveEffects();
                }
                else
                {
                    m_Effect.OnTick();

                    if (m_Expires > DateTime.MinValue && m_Expires <= DateTime.UtcNow)
                    {
                        m_Effect.RemoveEffects();
                    }
                }
            }
        }

        public static bool IsUnderEffects(Mobile from, EffectsType effect)
        {
            foreach (PropertyEffect e in m_Effects)
            {
                if (e.Mobile == from && e.Effect == effect)
                    return true;
            }
            return false;
        }

        public static T GetContext<T>(Mobile from, EffectsType type) where T : PropertyEffect
        {
            return m_Effects.FirstOrDefault(e => e.Mobile == from && e.Effect == type) as T;
        }

        public static T GetContext<T>(Mobile from, Mobile victim, EffectsType type) where T : PropertyEffect
        {
            return m_Effects.FirstOrDefault(e => e.Mobile == from && e.Victim == victim && e.Effect == type) as T;
        }

        public static IEnumerable<T> GetContexts<T>(Mobile victim, EffectsType type) where T : PropertyEffect
        {
            foreach (PropertyEffect effect in m_Effects.OfType<T>().Where(e => e.Victim == victim))
            {
                yield return effect as T;
            }
        }
    }

    public class SoulChargeContext : PropertyEffect
    {
        private bool m_Active;

        public SoulChargeContext(Mobile from, Item item)
            : base(from, null, item, EffectsType.SoulCharge, TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(40))
        {
            m_Active = true;
        }

        public override void OnDamaged(int damage)
        {
            if (m_Active && IsEquipped() && this.Mobile != null)
            {
                double mod = BaseFishPie.IsUnderEffects(this.Mobile, FishPieEffect.SoulCharge) ? .50 : .30;
                this.Mobile.Mana += (int)Math.Min(this.Mobile.ManaMax, damage * mod);
                m_Active = false;

                Server.Effects.SendTargetParticles(this.Mobile, 0x375A, 0x1, 0xA, 0x71, 0x2, 0x1AE9, (EffectLayer)0, 0);

                this.Mobile.SendLocalizedMessage(1113636); //The soul charge effect converts some of the damage you received into mana.
            }
        }

        public static void CheckHit(Mobile attacker, Mobile defender, int damage)
        {
            BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            if (shield != null && shield.ArmorAttributes.SoulCharge > 0 && shield.ArmorAttributes.SoulCharge > Utility.Random(100))
            {
                SoulChargeContext sc = PropertyEffect.GetContext<SoulChargeContext>(defender, EffectsType.SoulCharge);

                if (sc == null)
                    sc = new SoulChargeContext(defender, shield);

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
            : base(mobile, null, null, EffectsType.DamageEater, TimeSpan.MinValue, TimeSpan.FromSeconds(10))
        {
            m_Charges = 0;
        }

        public override void OnDamage(int damage, int phys, int fire, int cold, int poison, int energy, int direct)
        {
            if (m_Charges >= 20)
                return;

            double pd = 0; double fd = 0; 
            double cd = 0; double pod = 0; 
            double ed = 0; double dd = 0;

            double k = (double)GetValue(DamageType.Kinetic,  this.Mobile) / 100;
            double f = (double)GetValue(DamageType.Fire, this.Mobile) / 100;
            double c = (double)GetValue(DamageType.Cold, this.Mobile) / 100;
            double p = (double)GetValue(DamageType.Poison, this.Mobile) / 100;
            double e = (double)GetValue(DamageType.Energy, this.Mobile) / 100;
            double a = (double)GetValue(DamageType.AllTypes, this.Mobile) / 100;

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

            this.Mobile.Heal((int)dam);
            this.Mobile.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            this.Mobile.PlaySound(0x1F2);
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
                case DamageType.Kinetic: return (int)SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterKinetic);
                case DamageType.Fire: return (int)SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterFire);
                case DamageType.Cold: return (int)SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterCold);
                case DamageType.Poison: return (int)SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterPoison);
                case DamageType.Energy: return (int)SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterEnergy);
                case DamageType.AllTypes: return (int)SAAbsorptionAttributes.GetValue(from, SAAbsorptionAttribute.EaterDamage);
            }
            return 0;
        }

        public static void CheckDamage(Mobile from, int damage, int phys, int fire, int cold, int pois, int ergy, int direct)
        {
            DamageEaterContext context = PropertyEffect.GetContext<DamageEaterContext>(from, EffectsType.DamageEater);

            if (context == null && HasValue(from))
                context = new DamageEaterContext(from);

            if (context != null)
                context.OnDamage(damage, phys, fire, cold, pois, ergy, direct);
        }
    }

    public class SplinteringWeaponContext : PropertyEffect
    {
        public SplinteringWeaponContext(Mobile from, Mobile defender, Item weapon)
            : base(from, defender, weapon, EffectsType.Splintering, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4))
        {
            StartForceWalk(defender);
            BleedAttack.BeginBleed(defender, from, true);

            defender.SendLocalizedMessage(1112486); // A shard of the brittle weapon has become lodged in you!
            from.SendLocalizedMessage(1113077); // A shard of your blade breaks off and sticks in your opponent!

            Server.Effects.PlaySound(defender.Location, defender.Map, 0x1DF);

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.SplinteringEffect, 1154670, 1152144, TimeSpan.FromSeconds(10), defender));
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

        public static bool CheckHit(Mobile attacker, Mobile defender, Item weapon)
        {
            if (defender == null)
                return false;

            SplinteringWeaponContext context = PropertyEffect.GetContext<SplinteringWeaponContext>(attacker, defender, EffectsType.Splintering);

            if (context == null)
            {
                new SplinteringWeaponContext(attacker, defender, weapon);
                return true;
            }

            return false;
        }
    }

    public class SearingWeaponContext : PropertyEffect
    {
        public static int Damage { get { return Utility.RandomMinMax(10, 15); } }

        public SearingWeaponContext(Mobile from, Mobile defender)
            : base(from, defender, null, EffectsType.Searing, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4))
        {
            from.SendLocalizedMessage(1151177); //The searing attack cauterizes the wound on impact.
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            SearingWeaponContext context = PropertyEffect.GetContext<SearingWeaponContext>(attacker, defender, EffectsType.Searing);

            if (context == null)
                new SearingWeaponContext(attacker, defender);
        }

        public static bool HasContext(Mobile defender)
        {
            return PropertyEffect.GetContext<SearingWeaponContext>(defender, EffectsType.Searing) != null;
        }
    }

    public class BoneBreakerContext : PropertyEffect
    {
        public static Dictionary<Mobile, DateTime> _Immunity;

        public BoneBreakerContext(Mobile attacker, Mobile defender, Item weapon)
            : base(attacker, defender, weapon, EffectsType.BoneBreaker, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(1))
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

            AddImmunity(Victim);
        }

        public static int CheckHit(Mobile attacker, Mobile defender)
        {
            int mana = (int)(30.0 * ((double)(AosAttributes.GetValue(attacker, AosAttribute.LowerManaCost) + BaseArmor.GetInherentLowerManaCost(attacker)) / 100.0));
            int damage = 0;

            if (attacker.Mana >= mana)
            {
                attacker.Mana -= mana;
                damage += 50;

                defender.SendLocalizedMessage(1157317); // The attack shatters your bones!
            }

            if (IsImmune(defender))
            {
                attacker.SendLocalizedMessage(1157316); // Your target is currently immune to bone breaking!
                return damage;
            }

            if (20 > Utility.Random(100))
            {
                BoneBreakerContext context = PropertyEffect.GetContext<BoneBreakerContext>(attacker, defender, EffectsType.BoneBreaker);

                if (context == null)
                {
                    new BoneBreakerContext(attacker, defender, null);
                    defender.SendLocalizedMessage(1157363); // Your bones are broken! Stamina drain over time!

                    defender.PlaySound(0x204);
                    defender.FixedEffect(0x376A, 9, 32);

                    defender.FixedEffect(0x3779, 10, 20, 1365, 0);
                }
            }

            return damage;
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

    public class SwarmContext : PropertyEffect
    {
        public static Dictionary<Mobile, DateTime> _Immunity;

        private int _ID;

        public SwarmContext(Mobile attacker, Mobile defender, Item weapon)
            : base(attacker, defender, weapon, EffectsType.Swarm, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(2))
        {
            _ID = Utility.RandomMinMax(2331, 2339);
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            if (IsImmune(defender))
            {
                attacker.SendLocalizedMessage(1157322); // Your target is currently immune to swarm!
                return;
            }

            SwarmContext context = PropertyEffect.GetContext<SwarmContext>(attacker, defender, EffectsType.Swarm);

            if (context == null)
            {
                context = new SwarmContext(attacker, defender, null);

                if(defender.NetState != null)
                    defender.PrivateOverheadMessage(MessageType.Regular, 1150, 1157321, defender.NetState); // *You are engulfed in a swarm of insects!*

                Server.Effects.SendTargetEffect(defender, context._ID, 40); 

                defender.PlaySound(0x00E);
                defender.PlaySound(0x1BC);
            }
        }

        public override void OnTick()
        {
            if (Victim == null)
            {
                RemoveEffects();
                return;
            }

            AOS.Damage(Victim, Mobile, Utility.RandomMinMax(10, 20), 100, 0, 0, 0, 0);
            Victim.SendLocalizedMessage(1157362); // Biting insects are attacking you!

            Server.Effects.SendTargetEffect(Victim, _ID, 40);
        }

        public override void RemoveEffects()
        {
            base.RemoveEffects();

            //AddImmunity(Victim);
        }

        public static void CheckRemove(Mobile victim)
        {
            ColUtility.ForEach(PropertyEffect.GetContexts<SwarmContext>(victim, EffectsType.Swarm), context =>
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
            : base(attacker, defender, weapon, EffectsType.Sparks, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
        {
        }

        public static void CheckHit(Mobile attacker, Mobile defender)
        {
            if (IsImmune(defender))
            {
                attacker.SendLocalizedMessage(1157324); // Your target is currently immune to sparks!
                return;
            }

            SparksContext context = PropertyEffect.GetContext<SparksContext>(attacker, defender, EffectsType.Sparks);

            if (context == null)
            {
                context = new SparksContext(attacker, defender, null);

                attacker.PlaySound(0x20A);
                defender.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
            }
        }

        public override void OnTick()
        {
            AOS.Damage(Victim, Mobile, Utility.RandomMinMax(20, 40), 0, 0, 0, 0, 100);
        }

        public override void RemoveEffects()
        {
            base.RemoveEffects();

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
