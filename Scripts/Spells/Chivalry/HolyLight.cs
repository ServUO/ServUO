using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Spells.Chivalry
{
    public class HolyLightSpell : PaladinSpell
    {
        public override DamageType SpellDamageType { get { return DamageType.SpellAOE; } }

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Holy Light", "Augus Luminos",
            -1,
            9002);

        public HolyLightSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.75);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 55.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public override int RequiredTithing
        {
            get
            {
                return 10;
            }
        }
        public override int MantraNumber
        {
            get
            {
                return 1060724;
            }
        }// Augus Luminos
        public override bool BlocksMovement
        {
            get
            {
                return false;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                List<Mobile> targets = new List<Mobile>();
                IPooledEnumerable eable = Caster.GetMobilesInRange(3);

                foreach (Mobile m in eable)
                    if (this.Caster != m && SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanBeHarmful(m, false) && (!Core.AOS || this.Caster.InLOS(m)))
                        targets.Add(m);

                eable.Free();

                this.Caster.PlaySound(0x212);
                this.Caster.PlaySound(0x206);

                Effects.SendLocationParticles(EffectItem.Create(this.Caster.Location, this.Caster.Map, EffectItem.DefaultDuration), 0x376A, 1, 29, 0x47D, 2, 9962, 0);
                Effects.SendLocationParticles(EffectItem.Create(new Point3D(this.Caster.X, this.Caster.Y, this.Caster.Z - 7), this.Caster.Map, EffectItem.DefaultDuration), 0x37C4, 1, 29, 0x47D, 2, 9502, 0);

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];

                    int damage = this.ComputePowerValue(10) + Utility.RandomMinMax(0, 2);

                    // TODO: Should caps be applied?
                    if (damage < 8)
                        damage = 8;
                    else if (damage > 24)
                        damage = 24;

                    this.Caster.DoHarmful(m);
                    SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
                }
            }

            this.FinishSequence();
        }
    }
}