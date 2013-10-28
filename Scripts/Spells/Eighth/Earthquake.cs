using System;
using System.Collections.Generic;

namespace Server.Spells.Eighth
{
    public class EarthquakeSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Earthquake", "In Vas Por",
            233,
            9012,
            false,
            Reagent.Bloodmoss,
            Reagent.Ginseng,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        public EarthquakeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override void OnCast()
        {
            if (SpellHelper.CheckTown(this.Caster, this.Caster) && this.CheckSequence())
            {
                List<Mobile> targets = new List<Mobile>();

                Map map = this.Caster.Map;

                if (map != null)
                    foreach (Mobile m in this.Caster.GetMobilesInRange(1 + (int)(this.Caster.Skills[SkillName.Magery].Value / 15.0)))
                        if (this.Caster != m && SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanBeHarmful(m, false) && (!Core.AOS || this.Caster.InLOS(m)))
                            targets.Add(m);

                this.Caster.PlaySound(0x220);

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];

                    int damage;

                    if (Core.AOS)
                    {
                        damage = m.Hits / 2;

                        if (!m.Player)
                            damage = Math.Max(Math.Min(damage, 100), 15);
                        damage += Utility.RandomMinMax(0, 15);
                    }
                    else
                    {
                        damage = (m.Hits * 6) / 10;

                        if (!m.Player && damage < 10)
                            damage = 10;
                        else if (damage > 75)
                            damage = 75;
                    }

                    this.Caster.DoHarmful(m);
                    SpellHelper.Damage(TimeSpan.Zero, m, this.Caster, damage, 100, 0, 0, 0, 0);
                }
            }

            this.FinishSequence();
        }
    }
}