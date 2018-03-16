using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Engines.Quests;
using Server.Engines.Quests.Necro;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Fourth;

namespace Server.Mobiles
{
    public class NecroMageAI : MageAI
    {
        /*private Mobile m_Animated;

        public Mobile Animated
        {
            get { return m_Animated; }
            set { m_Animated = value; }
        }*/
		
		public override SkillName CastSkill { get { return SkillName.Magery; } }

        public NecroMageAI(BaseCreature m)
            : base(m)
        {
        }

        public override Spell GetRandomDamageSpell()
        {
            if(0.5 > Utility.RandomDouble())
			{
				int skill = (int)m_Mobile.Skills[SkillName.Necromancy].Value;
				int mana = m_Mobile.Mana;
				int select = 1;
				
				if(skill >= 65 && mana >= 29)
					select = 4;
				else if(skill >= 60 && mana >= 23)
                    select = 3;
				else if (skill >= 50 && mana >= 17)
					select = 2;
				
				switch (Utility.Random(select))
				{
					case 0: return new PainSpikeSpell(m_Mobile, null);
					case 1: return new PoisonStrikeSpell(m_Mobile, null);
					case 2: return new WitherSpell(m_Mobile, null);
					case 3: return new StrangleSpell(m_Mobile, null);
				}
			}
			
            return base.GetRandomDamageSpell();
        }

        public override Spell GetRandomCurseSpell()
        {
            int skill = (int)m_Mobile.Skills[SkillName.Necromancy].Value;
			int mana = m_Mobile.Mana;
			int select = 1;
			
			if(skill >= 30 && mana >= 17)
				select = 4;
			else if (skill >= 20 && mana >= 13)
				select = 3;
			else if (skill >= 20 && mana >= 11)
				select = 2;
			
			switch (Utility.Random(select))
			{
				case 0: return new CorpseSkinSpell(m_Mobile, null);
				case 1: return new EvilOmenSpell(m_Mobile, null);
				case 2: return new BloodOathSpell(m_Mobile, null);
				case 3: return new MindRotSpell(m_Mobile, null);
			}

            return base.GetRandomCurseSpell();
        }
		
		public override Spell GetRandomBuffSpell()
		{
			return new CurseWeaponSpell(m_Mobile, null);
		}

        public override Spell GetRandomSummonSpell()
		{
			if(m_Mobile.Skills[CastSkill].Value >= 40 && m_Mobile.Mana >= 23)
			{
                return new AnimateDeadSpell(m_Mobile, null);
			}
			
			return null;
        }

        protected override Spell CheckCastHealingSpell()
        {
            if (m_Mobile.Summoned || m_Mobile.Hits >= m_Mobile.HitsMax)
                return null;

            if(0.1 > Utility.RandomDouble())
				m_Mobile.UseSkill(SkillName.SpiritSpeak);
			else
				return base.CheckCastHealingSpell();
			
            return null;
        }

        public override bool DoActionGuard()
        {
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                if (m_Mobile.Debug)
                    m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                if (m_Mobile.Poisoned)
                {
                    new CureSpell(m_Mobile, null).Cast();
                }
                else if (!m_Mobile.Summoned)
                {
                    if (ScaleBySkill(HealChance, SkillName.Necromancy) > Utility.RandomDouble() && m_Mobile.Hits < m_Mobile.HitsMax - 30)
                    {
                        m_Mobile.UseSkill(SkillName.SpiritSpeak);
                    }
                    else if (ScaleBySkill(HealChance, SkillName.Magery) > Utility.RandomDouble())
                    {
                        if (m_Mobile.Hits < (m_Mobile.HitsMax - 50))
                        {
                            if (!new GreaterHealSpell(m_Mobile, null).Cast())
                                new HealSpell(m_Mobile, null).Cast();
                        }
                        else if (m_Mobile.Hits < (m_Mobile.HitsMax - 10))
                        {
                            new HealSpell(m_Mobile, null).Cast();
                        }
                    }
                    else
                    {
                        base.DoActionGuard();
                    }
                }
                else
                {
                    base.DoActionGuard();
                }
            }

            return true;
        }
    }
}