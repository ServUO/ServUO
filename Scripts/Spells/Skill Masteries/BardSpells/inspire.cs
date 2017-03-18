using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

/*Party Hit chance increase by up to 15%, Damage increase by up to 40%, 
SDI increased by up to 15% (PvP Cap 15)(Provocation Based)*/

namespace Server.Spells.SkillMasteries
{
	public class InspireSpell : BardSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Inspire", "Unus Por",
				-1,
				9002
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 4; } }
		public override int RequiredMana{ get { return 16; } }
        public override SkillName CastSkill { get { return SkillName.Provocation; } }
        public override bool PartyEffects { get { return true; } }

        private int m_PropertyBonus;
        private int m_DamageBonus;

		public InspireSpell( Mobile caster, Item scroll ) : base(caster, scroll, m_Info)
		{
		}
		
		public override void OnCast()
		{
			BardSpell spell = SkillMasterySpell.GetSpell(Caster, this.GetType()) as BardSpell;
			
			if(spell != null)
			{
				spell.Expire();
                Caster.SendLocalizedMessage(1115774); //You halt your spellsong.
			}
			else if ( CheckSequence() )
			{
                m_PropertyBonus = (int)((BaseSkillBonus * 12) + (CollectiveBonus * 3));
                m_DamageBonus = (int)Math.Min(40, ((BaseSkillBonus * 30) + (CollectiveBonus * 10)));

                System.Collections.Generic.List<Mobile> list = GetParty();

                foreach (Mobile m in list)
                {
                    m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    m.SendLocalizedMessage(1115736); // You feel inspired by the bard's spellsong.

                    string args = String.Format("{0}\t{1}\t{2}", m_PropertyBonus, m_PropertyBonus, m_DamageBonus);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Inspire, 1115683, 1115729, args.ToString()));
                }

                list.Clear();
                list.TrimExcess();

				BeginTimer();
			}
			
			FinishSequence();
		}

        public override void EndEffects()
        {
            if (PartyList != null)
            {
                foreach (Mobile m in PartyList) //Original Party list
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.Inspire);
                }
            }

            BuffInfo.RemoveBuff(Caster, BuffIcon.Inspire);
        }
		
		/// <summary>
		/// Called in AOS.cs - Hit Chance/Spell Damage Bonus
		/// </summary>
		/// <returns>HCI/SDI Bonus</returns>
		public override int PropertyBonus()
		{
			return m_PropertyBonus;
		}
		
		/// <summary>
		/// Called in AOS.cs - Weapon Damage Bonus
		/// </summary>
		/// <returns>DamInc Bonus</returns>
		public override int DamageBonus()
		{
			return m_DamageBonus;
		}
	}
}