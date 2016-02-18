using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class ArchCureSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Arch Cure", "Vas An Nox",
            215,
            9061,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.MandrakeRoot);
        public ArchCureSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fourth;
            }
        }
        // Arch cure is now 1/4th of a second faster
        public override TimeSpan CastDelayBase
        {
            get
            {
                return base.CastDelayBase - TimeSpan.FromSeconds(0.25);
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!this.Caster.CanSee(p))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                List<Mobile> targets = new List<Mobile>();

                Map map = this.Caster.Map;
                Mobile directTarget = p as Mobile;

                if (map != null)
                {
                    bool feluccaRules = (map.Rules == MapRules.FeluccaRules);

                    // You can target any living mobile directly, beneficial checks apply
                    if (directTarget != null && this.Caster.CanBeBeneficial(directTarget, false))
                        targets.Add(directTarget);

                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 2);

                    foreach (Mobile m in eable)
                    {
                        if (m == directTarget)
                            continue;

                        if (this.AreaCanTarget(m, feluccaRules))
                            targets.Add(m);
                    }

                    eable.Free();
                }

                Effects.PlaySound(p, this.Caster.Map, 0x299);

                if (targets.Count > 0)
                {
                    int cured = 0;

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        Mobile m = targets[i];

                        this.Caster.DoBeneficial(m);

                        Poison poison = m.Poison;

                        if (poison != null)
                        {
                            int chanceToCure = 10000 + (int)(this.Caster.Skills[SkillName.Magery].Value * 75) - ((poison.RealLevel + 1) * 1750);
                            chanceToCure /= 100;
                            chanceToCure -= 1;

                            if (chanceToCure > Utility.Random(100) && m.CurePoison(this.Caster))
                                ++cured;
                        }

                        m.FixedParticles(0x373A, 10, 15, 5012, EffectLayer.Waist);
                        m.PlaySound(0x1E0);
                    }

                    if (cured > 0)
                        this.Caster.SendLocalizedMessage(1010058); // You have cured the target of all poisons!
                }
            }

            this.FinishSequence();
        }

        private static bool IsInnocentTo(Mobile from, Mobile to)
        {
            return (Notoriety.Compute(from, (Mobile)to) == Notoriety.Innocent);
        }

        private static bool IsAllyTo(Mobile from, Mobile to)
        {
            return (Notoriety.Compute(from, (Mobile)to) == Notoriety.Ally);
        }

        private bool AreaCanTarget(Mobile target, bool feluccaRules)
        {
            /* Arch cure area effect won't cure aggressors, victims, murderers, criminals or monsters.
            * In Felucca, it will also not cure summons and pets.
            * For red players it will only cure themselves and guild members.
            */
            if (!this.Caster.CanBeBeneficial(target, false))
                return false;

            if (Core.AOS && target != this.Caster)
            {
                if (this.IsAggressor(target) || this.IsAggressed(target))
                    return false;

                if ((!IsInnocentTo(this.Caster, target) || !IsInnocentTo(target, this.Caster)) && !IsAllyTo(this.Caster, target))
                    return false;

                if (feluccaRules && !(target is PlayerMobile))
                    return false;
            }

            return true;
        }

        private bool IsAggressor(Mobile m)
        {
            foreach (AggressorInfo info in this.Caster.Aggressors)
            {
                if (m == info.Attacker && !info.Expired)
                    return true;
            }

            return false;
        }

        private bool IsAggressed(Mobile m)
        {
            foreach (AggressorInfo info in this.Caster.Aggressed)
            {
                if (m == info.Defender && !info.Expired)
                    return true;
            }

            return false;
        }

        private class InternalTarget : Target
        {
            private readonly ArchCureSpell m_Owner;
            public InternalTarget(ArchCureSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    this.m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}