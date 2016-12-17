using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Spells.Spellweaving
{
    public class ThunderstormSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Thunderstorm", "Erelonia",
            -1);
        private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        public ThunderstormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 10.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 32;
            }
        }
        public static int GetCastRecoveryMalus(Mobile m)
        {
            return m_Table.ContainsKey(m) ? 6 : 0;
        }

        public static void DoExpire(Mobile m)
        {
            Timer t;

            if (m_Table.TryGetValue(m, out t))
            {
                t.Stop();
                m_Table.Remove(m);

                BuffInfo.RemoveBuff(m, BuffIcon.Thunderstorm);
            }
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.PlaySound(0x5CE);

                double skill = this.Caster.Skills[SkillName.Spellweaving].Value;

                int damage = Math.Max(11, 10 + (int)(skill / 24)) + this.FocusLevel;

                int sdiBonus = AosAttributes.GetValue(this.Caster, AosAttribute.SpellDamage);
						
                int pvmDamage = damage * (100 + sdiBonus);
                pvmDamage /= 100;

                if (sdiBonus > 15)
                    sdiBonus = 15;
						
                int pvpDamage = damage * (100 + sdiBonus);
                pvpDamage /= 100;

                int range = 2 + this.FocusLevel;
                TimeSpan duration = TimeSpan.FromSeconds(5 + this.FocusLevel);

                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in this.Caster.GetMobilesInRange(range))
                {
                    if (this.Caster != m && SpellHelper.ValidIndirectTarget(this.Caster, m) && this.Caster.CanBeHarmful(m, false) && this.Caster.InLOS(m))
                        targets.Add(m);
                }

                for (int i = 0; i < targets.Count; i++)
                {
                    Mobile m = targets[i];

                    this.Caster.DoHarmful(m);

                    Spell oldSpell = m.Spell as Spell;

                    SpellHelper.Damage(this, m, (m.Player && this.Caster.Player) ? pvpDamage : pvmDamage, 0, 0, 0, 0, 100);

                    if (oldSpell != null && oldSpell != m.Spell)
                    {
                        if (!this.CheckResisted(m))
                        {
                            m_Table[m] = Timer.DelayCall<Mobile>(duration, DoExpire, m);

                            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Thunderstorm, 1075800, duration, m, GetCastRecoveryMalus(m)));
                        }
                    }
                }
            }

            this.FinishSequence();
        }
    }
}