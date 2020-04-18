#region References
using Server.Spells;
using Server.Spells.Chivalry;
#endregion

namespace Server.Mobiles
{
    public class PaladinAI : MageAI
    {
        public override SkillName CastSkill => SkillName.Chivalry;
        public override bool UsesMagery => false;
        public override double HealChance => .1;

        public PaladinAI(BaseCreature m)
            : base(m)
        { }

        public override Spell GetRandomDamageSpell()
        {
            if (m_Mobile.Mana > 10 && 0.1 > Utility.RandomDouble())
                return new HolyLightSpell(m_Mobile, null);

            return null;
        }

        public override Spell GetRandomCurseSpell()
        {
            if (m_Mobile.Mana > 10)
                return new DispelEvilSpell(m_Mobile, null);

            return null;
        }

        public override Spell GetRandomBuffSpell()
        {
            int mana = m_Mobile.Mana;
            int select = 1;

            if (mana >= 15)
                select = 3;

            if (mana >= 20 && !EnemyOfOneSpell.UnderEffect(m_Mobile))
                select = 4;

            switch (Utility.Random(select))
            {
                case 0:
                    return new RemoveCurseSpell(m_Mobile, null);
                case 1:
                    return new DivineFurySpell(m_Mobile, null);
                case 2:
                    return new ConsecrateWeaponSpell(m_Mobile, null);
                case 3:
                    return new EnemyOfOneSpell(m_Mobile, null);
            }

            return new ConsecrateWeaponSpell(m_Mobile, null);
        }

        public override Spell GetHealSpell()
        {
            if (m_Mobile.Mana > 10)
                return new CloseWoundsSpell(m_Mobile, null);

            return null;
        }

        public override Spell GetCureSpell()
        {
            if (m_Mobile.Mana > 10)
                return new CleanseByFireSpell(m_Mobile, null);

            return null;
        }

        protected override bool ProcessTarget()
        {
            if (m_Mobile.Target == null)
                return false;

            m_Mobile.Target.Invoke(m_Mobile, m_Mobile);
            return true;
        }
    }
}
