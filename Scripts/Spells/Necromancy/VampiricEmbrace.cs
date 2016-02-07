using System;
using Server.Items;

namespace Server.Spells.Necromancy
{
    public class VampiricEmbraceSpell : TransformationSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Vampiric Embrace", "Rel Xen An Sanct",
            203,
            9031,
            Reagent.BatWing,
            Reagent.NoxCrystal,
            Reagent.PigIron);
        public VampiricEmbraceSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(2.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 99.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 23;
            }
        }
        public override int Body
        {
            get
            {
                return this.Caster.Body;//Only way I found to combat the body change glitch for gargoyles.  May lead to polymorph issues with custom items, may need to add checks on said items if so.  Otherwise Gargoyles change to a humanoid bodyvalue on casting and when dieing glicth out and crash.
            }
        }
        public override int Hue
        {
            get
            {
                return 0x847E;
            }
        }
        public override int FireResistOffset
        {
            get
            {
                return -25;
            }
        }
        public override void GetCastSkills(out double min, out double max)
        {
            if (this.Caster.Skills[this.CastSkill].Value >= this.RequiredSkill)
            {
                min = 80.0;
                max = 120.0;
            }
            else
            {
                base.GetCastSkills(out min, out max);
            }
        }

        public override void DoEffect(Mobile m)
        {
            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x373A, 1, 17, 1108, 7, 9914, 0);
            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x376A, 1, 22, 67, 7, 9502, 0);
            Effects.PlaySound(m.Location, m.Map, 0x4B1);
			
			BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.VampiricEmbrace, 1028812));
			if (Caster.Skills.Necromancy.Value > 99.0)
				BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.PoisonImmunity, 1153785));
		}

		public override void RemoveEffect(Mobile m)
		{
			BuffInfo.RemoveBuff(Caster, BuffIcon.PoisonImmunity);
			BuffInfo.RemoveBuff(Caster, BuffIcon.VampiricEmbrace);
		}
    }
}