using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public class EtherealBurstSpell : SkillMasterySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Ethereal Blast", "Uus Ort Grav",
				-1,
				9002,
                Reagent.Bloodmoss,
                Reagent.Ginseng,
                Reagent.MandrakeRoot
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 0; } }
		public override int RequiredMana{ get { return 0; } }
		public override bool PartyEffects { get { return false; } }

        public override SkillName CastSkill { get { return SkillName.Magery; } }
        public override SkillName DamageSkill { get { return SkillName.EvalInt; } }

        public EtherealBurstSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
		{
		}

		public override void OnCast()
		{
            if (CheckSequence())
            {
                Caster.Mana = Caster.ManaMax;

                int duration = 120;
                double skill = ((double)(Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 2.1) + GetMasteryLevel() * 2;

                if (skill >= 120)
                    duration = 30;

                if (skill >= 100)
                    duration = 60;

                if (duration >= 60)
                    duration = 90;

                AddToCooldown(TimeSpan.FromMinutes(duration));

                Caster.PlaySound(0x102);
                Effects.SendTargetParticles(Caster, 0x376A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
                Caster.SendLocalizedMessage(1155789); // You feel completely rejuvinated!
            }

            FinishSequence();
		}
	}
}