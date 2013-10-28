using System;
using System.Collections.Generic;
using Server.Targeting;

namespace Server.Spells.Sixth
{
    public class MassCurseSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Mass Curse", "Vas Des Sanct",
            218,
            9031,
            false,
            Reagent.Garlic,
            Reagent.Nightshade,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        public MassCurseSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
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
            else if (SpellHelper.CheckTown(p, this.Caster) && this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                List<Mobile> targets = new List<Mobile>();

                Map map = this.Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 2);

                    foreach (Mobile m in eable)
                    {
                        if (Core.AOS && m == this.Caster)
                            continue;

                        if (SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanSee(m) && this.Caster.CanBeHarmful(m, false))
                            targets.Add(m);
                    }

                    eable.Free();
                }

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];

                    this.Caster.DoHarmful(m);

                    SpellHelper.AddStatCurse(this.Caster, m, StatType.Str);
                    SpellHelper.DisableSkillCheck = true;
                    SpellHelper.AddStatCurse(this.Caster, m, StatType.Dex);
                    SpellHelper.AddStatCurse(this.Caster, m, StatType.Int);
                    SpellHelper.DisableSkillCheck = false;

                    m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                    m.PlaySound(0x1FB);
					
                    this.HarmfulSpell(m);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MassCurseSpell m_Owner;
            public InternalTarget(MassCurseSpell owner)
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