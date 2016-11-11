using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;

/*Party Hit Points increased by up to 20 + 6(Collection Bonus), Party healed for 9-20 dmg every 4 seconds. 
(Provocation Based). Party Strength, Dex, Int, Increased by Up to 8.*/

namespace Server.Spells.SkillMasteries
{
	public class InvigorateSpell : BardSpell
	{
		public static readonly string StatModName = "Invigorate";
	
		private DateTime m_NextHeal;
	
		private static SpellInfo m_Info = new SpellInfo(
				"Invigorate", "An Zu",
				-1,
				9002
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 5; } }
		public override int RequiredMana{ get { return 22; } }
		public override bool PartyEffects { get { return true; } }
        public override SkillName CastSkill { get { return SkillName.Provocation; } }

        private int m_HPBonus;
        private int m_StatBonus;

		public InvigorateSpell( Mobile caster, Item scroll ) : base(caster, scroll, m_Info)
		{
			m_NextHeal = DateTime.Now + TimeSpan.FromSeconds(4);
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
                m_HPBonus = (int)((20 * BaseSkillBonus) + (CollectiveBonus * 6));
                m_StatBonus = (int)((BaseSkillBonus * 8) + (CollectiveBonus * 6));
                System.Collections.Generic.List<Mobile> list = GetParty();

                foreach (Mobile m in list)
                {
                    m.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                    m.SendLocalizedMessage(1115737); // You feel invigorated by the bard's spellsong.

                    string args = String.Format("{0}\t{1}\t{2}\t{3}", m_StatBonus, m_StatBonus, m_StatBonus, m_StatBonus);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Invigorate, 1115613, 1115730, args.ToString()));
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
                    BuffInfo.RemoveBuff(m, BuffIcon.Invigorate);
                }
            }

            BuffInfo.RemoveBuff(Caster, BuffIcon.Invigorate);
        }
		
		public override bool OnTick()
		{
			base.OnTick();
			
			if(m_NextHeal > DateTime.Now)
                return false;
			
			foreach(Mobile m in GetParty())
			{
				if(Caster.InRange(m.Location, PartyRange))
				{
					int healRange = (int)((BaseSkillBonus * 12) + (CollectiveBonus * 8));

                    if(m.Hits < m.HitsMax)
					    m.Heal(Utility.RandomMinMax(healRange - 2, healRange + 2));

                    Caster.DoBeneficial(m);

                    m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                    m.PlaySound(0x1F2);
				}
			}

            m_NextHeal = DateTime.Now + TimeSpan.FromSeconds(4);
            return true;
		}

        private List<Mobile> m_Mods = new List<Mobile>();

		public override void AddStatMods()
		{
			int offset = m_StatBonus;
            System.Collections.Generic.List<Mobile> list = GetParty();

            foreach (Mobile m in list)
            {
                m.AddStatMod(new StatMod(StatType.Str, StatModName + "str", offset, TimeSpan.Zero));
                m.AddStatMod(new StatMod(StatType.Dex, StatModName + "dex", offset, TimeSpan.Zero));
                m.AddStatMod(new StatMod(StatType.Int, StatModName + "int", offset, TimeSpan.Zero));

                m_Mods.Add(m);
            }

            list.Clear();
            list.TrimExcess();
		}
		
		public override void RemoveStatMods()
		{
            foreach (Mobile m in m_Mods)
            {
                m.RemoveStatMod(StatModName + "str");
                m.RemoveStatMod(StatModName + "dex");
                m.RemoveStatMod(StatModName + "int");
            }

            m_Mods.Clear();
		}
		
		/// <summary>
		/// Called in AOS.cs - HP Bonus
		/// </summary>
		/// <returns></returns>
		public override int StatBonus()
		{
            return m_HPBonus;
		}
	}
}