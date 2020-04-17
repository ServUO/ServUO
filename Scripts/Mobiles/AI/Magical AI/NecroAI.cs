#region References
using Server.Spells;
using Server.Spells.Necromancy;
#endregion

namespace Server.Mobiles
{
    public class NecroAI : MageAI
    {
        public override SkillName CastSkill => SkillName.Necromancy;
        public override bool UsesMagery => false;

        public NecroAI(BaseCreature m)
            : base(m)
        { }

        public override Spell GetRandomDamageSpell()
        {
            int mana = m_Mobile.Mana;
            int select = 1;

            if (mana >= 29)
                select = 4;
            else if (mana >= 23)
                select = 3;
            else if (mana >= 17)
                select = 2;

            switch (Utility.Random(select))
            {
                case 0:
                    return new PainSpikeSpell(m_Mobile, null);
                case 1:
                    return new PoisonStrikeSpell(m_Mobile, null);
                case 2:
                    return new WitherSpell(m_Mobile, null);
                case 3:
                    return new StrangleSpell(m_Mobile, null);
            }

            return null;
        }

        public override Spell GetRandomSummonSpell()
        {
            if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.Mana >= 23)
            {
                return new AnimateDeadSpell(m_Mobile, null);
            }

            return null;
        }

        public override Spell GetRandomCurseSpell()
        {
            int mana = m_Mobile.Mana;
            int select = 1;

            if (mana >= 17)
                select = 5;
            else if (mana >= 13)
                select = 4;
            else if (mana >= 11)
                select = 3;

            switch (Utility.Random(select))
            {
                case 0:
                    return new CurseWeaponSpell(m_Mobile, null);
                case 1:
                    Spell spell;

                    if (NecroMageAI.CheckCastCorpseSkin(m_Mobile))
                        spell = new CorpseSkinSpell(m_Mobile, null);
                    else
                        spell = new CurseWeaponSpell(m_Mobile, null);

                    return spell;
                case 2:
                    return new EvilOmenSpell(m_Mobile, null);
                case 3:
                    return new BloodOathSpell(m_Mobile, null);
                case 4:
                    return new MindRotSpell(m_Mobile, null);
            }

            return null;
        }

        public override Spell GetCureSpell()
        {
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