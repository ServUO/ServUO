using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Ninjitsu;
using Server.Network;

namespace Server.Items
{
    public enum EffectsType
    {
        BattleLust,
        SoulCharge,
        DamageEater,
        Splintering,
        Searing
    }

    /// <summary>
    /// Used for complex weapon/armor properties introduced in Stagyian Abyss
    /// </summary>
    public class PropertyEffect
    {
        private Mobile m_Mobile;
        private Item m_Owner;
        private EffectsType m_Effect;
        private TimeSpan m_Duration;
        private TimeSpan m_TickDuration;
        private Timer m_Timer;

        public Mobile Mobile { get { return m_Mobile; } }
        public Item Owner { get { return m_Owner; } }
        public EffectsType Effect { get { return m_Effect; } }
        public TimeSpan Duration { get { return m_Duration; } }
        public TimeSpan TickDuration { get { return m_TickDuration; } }
        public Timer Timer { get { return m_Timer; } }

        private static List<PropertyEffect> m_Effects = new List<PropertyEffect>();
        public static List<PropertyEffect> Effects { get { return m_Effects; } }

        public PropertyEffect(Mobile from, Item owner, EffectsType effect, TimeSpan duration, TimeSpan tickduration)
        {
            m_Mobile = from;
            m_Owner = owner;
            m_Effect = effect;
            m_Duration = duration;
            m_TickDuration = tickduration;

            m_Effects.Add(this);

            if (m_TickDuration > TimeSpan.MinValue)
                StartTimer();
        }

        public void RemoveEffects()
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

        public virtual void ModifyDamage(Mobile defender, ref int damIncrease)
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
                    m_Expires = DateTime.Now + effect.Duration;
                else
                    m_Expires = DateTime.MinValue;
            }

            protected override void OnTick()
            {
                m_Effect.OnTick();

                if (m_Expires > DateTime.MinValue && m_Expires <= DateTime.Now)
                {
                    m_Effect.RemoveEffects();
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

        public static void RemoveContext(Mobile from, EffectsType type)
        {
            PropertyEffect effect = GetContext(from, type);

            if (effect != null)
                effect.RemoveEffects();
        }

        public static PropertyEffect GetContext(Mobile from, EffectsType type)
        {
            foreach (PropertyEffect e in m_Effects)
            {
                if (e.Mobile == from && e.Effect == type)
                    return e;
            }

            return null;
        }
    }

    /*public class BattleLustContext : PropertyEffect
    {
        private int m_DamageMod;
        private int m_Tick;
        private int m_DamageDone;
        private int m_Aggressors;

        public int DamageMod { get { return m_DamageMod; } }

        public BattleLustContext(Mobile from, Item item)
            : base(from, item, EffectsType.BattleLust, TimeSpan.MinValue, TimeSpan.FromSeconds(2))
        {
            m_DamageMod = 0;
            m_Tick = 0;
            m_Aggressors = 0;
        }

        public override void OnTick()
        {
            if (SAWeaponAttributes.GetValue(this.Mobile, SAWeaponAttribute.BattleLust) == 0)
            {
                RemoveEffects();
                return;
            }

            m_Tick++;

            if (m_Aggressors < this.Mobile.Aggressed.Count + this.Mobile.Aggressors.Count)
            {
                m_Aggressors = this.Mobile.Aggressed.Count + this.Mobile.Aggressors.Count;
                m_DamageMod += m_Aggressors * 15;
            }

            m_DamageMod += Math.Max(0, m_DamageDone / 25);

            if (m_Tick % 6 == 0)
                m_DamageMod -= m_Tick / 6;

            if (m_DamageMod <= 0 || m_Tick > 60)
                RemoveEffects();

            m_DamageDone = 0;
        }

        public override void OnDamaged(int damage)
        {
            m_DamageDone += damage;

            if(this.Mobile != null)
                this.Mobile.SendLocalizedMessage(1113748); // The damage you received fuels your battle fury.
        }

        public override void ModifyDamage(Mobile defender, ref int damIncrease)
        {
            if (defender is PlayerMobile)
                damIncrease += (int)Math.Min(45, m_DamageMod);
            else
                damIncrease += (int)Math.Min(90, m_DamageMod);

            //TODO: Message?
        }

        /// <summary>
        /// Called when a person is hit damage. It then checks for a Battle Lust Context for the one recieving damage.
        /// </summary>
        /// <param name="defender"></param>
        /// <param name="damage"></param>
        public static void CheckHit(Mobile defender, int damage)
        {
            BaseWeapon weapon = defender.Weapon as BaseWeapon;

            if (damage < 30 || weapon == null || weapon.WeaponAttributesSA.BattleLust <= 0)
                return;

            BattleLustContext defenderBL = (BattleLustContext)PropertyEffect.GetContext(defender, EffectsType.BattleLust);

            if (defenderBL == null)
                defenderBL = new BattleLustContext(defender, weapon);

            if (defenderBL != null)
                defenderBL.OnDamaged(damage);
        }

        /// <summary>
        /// This is called when a person hits another mobile with a weapon. 
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="percentBonus"></param>
        /// <param name="weapon"></param>
        public static void CheckModifyDamage(Mobile attacker, Mobile defender, ref int percentBonus, BaseWeapon weapon)
        {
            if (SAWeaponAttributes.GetValue(attacker, SAWeaponAttribute.BattleLust) <= 0)
                return;

            BattleLustContext attackerBL = (BattleLustContext)PropertyEffect.GetContext(attacker, EffectsType.BattleLust);

            if (attackerBL == null)
                attackerBL = new BattleLustContext(attacker, weapon);

            if(attackerBL != null)
                attackerBL.ModifyDamage(defender, ref percentBonus);
        }
    }*/

    public class SoulChargeContext : PropertyEffect
    {
        private bool m_Active;

        public SoulChargeContext(Mobile from, Item item)
            : base(from, item, EffectsType.SoulCharge, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15))
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
                this.Mobile.SendLocalizedMessage(1113636); //The soul charge effect converts some of the damage you received into mana.
            }
        }

        public static void CheckHit(Mobile attacker, Mobile defender, int damage)
        {
            BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;
            if (shield != null && shield.ArmorAttributes.SoulCharge > 0 && shield.ArmorAttributes.SoulCharge > Utility.Random(100))
            {
                SoulChargeContext sc = (SoulChargeContext)PropertyEffect.GetContext(defender, EffectsType.SoulCharge);

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
            : base(mobile, null, EffectsType.DamageEater, TimeSpan.MinValue, TimeSpan.FromSeconds(10))
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
            DamageEaterContext context = (DamageEaterContext)PropertyEffect.GetContext(from, EffectsType.DamageEater);

            if (context == null && HasValue(from))
                context = new DamageEaterContext(from);

            if (context != null)
                context.OnDamage(damage, phys, fire, cold, pois, ergy, direct);
        }
    }

    public class SplinteringWeaponContext : PropertyEffect
    {
        private Mobile m_Bleeder;
        private int m_Level;
        private bool m_Bleeding;

        public Mobile Bleeder { get { return m_Bleeder; } }
        public bool Bleeding { get { return m_Bleeding; } }

        public SplinteringWeaponContext(Mobile from, Mobile defender, Item weapon)
            : base(from, weapon, EffectsType.Splintering, TimeSpan.MinValue, TimeSpan.FromSeconds(2))
        {
            m_Bleeding = true;
            m_Bleeder = defender;
            m_Level = 0;

            StartForceWalk(m_Bleeder);

            BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.SplinteringEffect, 1154670, 1152396));
        }

        public override void OnTick()
        {
            m_Level++;

            if (m_Bleeding)
                DoBleed(m_Bleeder, this.Mobile, 6 - m_Level);

            if (m_Level > 4)
            {
                EndBleed(m_Bleeder, true);
                EndForceWalk(m_Bleeder);
                RemoveEffects();
                BuffInfo.RemoveBuff(this.Mobile, BuffIcon.SplinteringEffect);
                return;
            }
        }

        public void StartForceWalk(Mobile m)
        {
            if (m.NetState != null && !TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm))
                && m.AccessLevel < AccessLevel.GameMaster)
                m.Send(SpeedControl.WalkSpeed);
        }

        public void EndForceWalk(Mobile m)
        {
            if ( m.NetState != null && !TransformationSpellHelper.UnderTransformation( m, typeof( AnimalForm ) )
				&& !TransformationSpellHelper.UnderTransformation( m, typeof( Server.Spells.Spellweaving.ReaperFormSpell ) ) )
				m.Send( SpeedControl.Disable );
        }

        public void DoBleed(Mobile m, Mobile from, int level)
        {
            if (m.Alive)
            {
                int damage = Utility.RandomMinMax(level, level * 2);

                if (!m.Player)
                    damage *= 2;

                m.PlaySound(0x133);
                AOS.Damage(m, from, damage, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                Blood blood = new Blood();

                blood.ItemID = Utility.Random(0x122A, 5);

                blood.MoveToWorld(m.Location, m.Map);
            }
            else
            {
                EndBleed(m, false);
                RemoveEffects();

                BuffInfo.RemoveBuff(m, BuffIcon.SplinteringEffect);
            }
        }

        public void EndBleed(Mobile m, bool message)
        {
            if (message)
                m.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!

            m_Bleeding = false;
        }

        public static void EndBleeding(Mobile m)
        {
            foreach (PropertyEffect effect in PropertyEffect.Effects)
            {
                if (effect is SplinteringWeaponContext && ((SplinteringWeaponContext)effect).Bleeder == m && ((SplinteringWeaponContext)effect).Bleeding)
                    ((SplinteringWeaponContext)effect).EndBleed(m, true);
            }
        }

        public static bool IsBleeding(Mobile m)
        {
            foreach (PropertyEffect effect in PropertyEffect.Effects)
            {
                if (effect is SplinteringWeaponContext && ((SplinteringWeaponContext)effect).Bleeder == m && ((SplinteringWeaponContext)effect).Bleeding)
                    return true;
            }

            return false;
        }

        public static bool CheckHit(Mobile attacker, Mobile defender, Item weapon)
        {
            SplinteringWeaponContext context = (SplinteringWeaponContext)PropertyEffect.GetContext(attacker, EffectsType.Splintering);

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

        public SearingWeaponContext(Mobile from)
            : base(from, null, EffectsType.Searing, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4))
        {
            from.SendLocalizedMessage(1151177); //The searing attack cauterizes the wound on impact.
        }

        public static void CheckHit(Mobile defender)
        {
            SearingWeaponContext context = (SearingWeaponContext)PropertyEffect.GetContext(defender, EffectsType.Searing);

            if (context == null)
                new SearingWeaponContext(defender);
        }

        public static bool HasContext(Mobile defender)
        {
            return PropertyEffect.GetContext(defender, EffectsType.Searing) != null;
        }
    }
}
