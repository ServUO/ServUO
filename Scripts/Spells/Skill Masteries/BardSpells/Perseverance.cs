using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

/*Party Defense Chance increased by up to 22%, Damage reduced by up to 22%.
Casting focus bonus 1-6%.*/

namespace Server.Spells.SkillMasteries
{
	public class PerseveranceSpell : BardSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Perserverance", "Unus Jux Sanct",
				-1,
				9002
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 5; } }
		public override int RequiredMana{ get { return 18; } }
		public override bool PartyEffects { get { return true; } }
        public override SkillName CastSkill { get { return SkillName.Peacemaking; } }

        private int m_PropertyBonus;
        private int m_PropertyBonus2;
        private double m_DamageMod;

		public PerseveranceSpell( Mobile caster, Item scroll ) : base(caster, scroll, m_Info)
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
                m_PropertyBonus = (int)((BaseSkillBonus * 16) + (CollectiveBonus * 6));
                m_PropertyBonus2 = (int)((BaseSkillBonus * 4) + (CollectiveBonus * 2));
                m_DamageMod = ((BaseSkillBonus * 16) + (CollectiveBonus * 6)) / 100;

                foreach (Mobile m in GetParty())
                {
                    m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    m.SendLocalizedMessage(1115739); // The bard's spellsong fills you with a feeling of invincibility.

                    string args = String.Format("{0}\t{1}\t{2}", m_PropertyBonus, (int)(m_DamageMod * 100), m_PropertyBonus2);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Perseverance, 1115615, 1115732, args.ToString()));
                }

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
                    BuffInfo.RemoveBuff(m, BuffIcon.Perseverance);
                }
            }

            BuffInfo.RemoveBuff(Caster, BuffIcon.Perseverance);
        }
		
		/// <summary>
		/// Defense Chance Bonus
		/// </summary>
		/// <returns>Defense Chance Bonus</returns>
		public override int PropertyBonus()
		{
            return m_PropertyBonus;
		}

        /// <summary>
        /// Casting Focus
        /// </summary>
        /// <returns>Casting Focus</returns>
		public override int PropertyBonus2()
		{
            return m_PropertyBonus2;
		}
		
		/// <summary>
		/// modifies total damage dealt
		/// </summary>
		/// <param name="damage"></param>
		public void AbsorbDamage(ref int damage)
		{
			damage -= (int)(damage * m_DamageMod);
		}
	}
}