using System;
using Server;
using Server.Spells.Necromancy;
using Server.Spells;

namespace Server.Mobiles
{
	public class NecroAI : MagicalAI
	{
		public override SkillName CastSkill { get { return SkillName.Necromancy; } }
		
		public NecroAI(BaseCreature m)
            : base(m)
        {
        }
		
		public override Spell GetRandomDamageSpell()
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

            return null;
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

            return null;
		}
		
		public override Spell GetRandomBuffSpell()
		{
			return new CurseWeaponSpell(m_Mobile, null);
		}
		
		public override Spell GetHealSpell()
		{
			m_Mobile.UseSkill(SkillName.SpiritSpeak);
			
			return null;
		}
	}
}