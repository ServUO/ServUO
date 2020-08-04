#region References
using Server.Spells;
using Server.Spells.Mysticism;
#endregion

namespace Server.Mobiles
{
    public class MysticAI : MageAI
    {
        public override SkillName CastSkill => SkillName.Mysticism;

        public override bool UsesMagery => m_Mobile.Skills[SkillName.Magery].Base >= 20.0 && !m_Mobile.Controlled;

        public MysticAI(BaseCreature m)
            : base(m)
        { }

        public override Spell GetRandomDamageSpell()
        {
            if (UsesMagery && 0.5 > Utility.RandomDouble())
            {
                return base.GetRandomDamageSpell();
            }

            int mana = m_Mobile.Mana;
            int select = 1;

            if (mana >= 50)
                select = 5;
            else if (mana >= 20)
                select = 3;
            else if (mana >= 9)
                select = 2;

            switch (Utility.Random(select))
            {
                case 0:
                    return new NetherBoltSpell(m_Mobile, null);
                case 1:
                    return new EagleStrikeSpell(m_Mobile, null);
                case 2:
                    return new BombardSpell(m_Mobile, null);
                case 3:
                    return new HailStormSpell(m_Mobile, null);
                case 4:
                    return new NetherCycloneSpell(m_Mobile, null);
            }

            return null;
        }

        public override Spell GetRandomCurseSpell()
        {
            if (UsesMagery && 0.5 > Utility.RandomDouble())
            {
                return base.GetRandomCurseSpell();
            }

            int mana = m_Mobile.Mana;
            int select = 1;

            if (mana >= 40)
                select = 4;
            else if (mana >= 14)
                select = 3;
            else if (mana >= 8)
                select = 2;

            switch (Utility.Random(select))
            {
                case 0:
                    return new PurgeMagicSpell(m_Mobile, null);
                case 1:
                    return new SleepSpell(m_Mobile, null);
                case 2:
                    return new MassSleepSpell(m_Mobile, null);
                case 3:
                    return new SpellPlagueSpell(m_Mobile, null);
            }

            return null;
        }

        public override Spell GetHealSpell()
        {
            if (UsesMagery && 0.5 > Utility.RandomDouble())
            {
                return base.GetHealSpell();
            }

            if (m_Mobile.Mana >= 20)
                return new CleansingWindsSpell(m_Mobile, null);

            return null;
        }

        public override Spell GetCureSpell()
        {
            if (UsesMagery)
            {
                return base.GetCureSpell();
            }

            return null;
        }

        public override Spell GetRandomBuffSpell()
        {
            if (UsesMagery)
            {
                return base.GetRandomBuffSpell();
            }

            return null;
        }

        public override Spell RandomCombatSpell()
        {
            Spell spell = CheckCastHealingSpell();

            if (spell != null)
                return spell;

            switch (Utility.Random(6))
            {
                case 0: // Curse
                    {
                        m_Mobile.DebugSay("Cursing Thou!");
                        spell = GetRandomCurseSpell();
                        break;
                    }
                case 1:
                case 2:
                case 3:
                case 4:
                case 5: // damage
                    {
                        m_Mobile.DebugSay("Just doing damage");
                        spell = GetRandomDamageSpell();
                    }
                    break;
            }

            return spell;
        }

        protected override bool ProcessTarget()
        {
            Targeting.Target t = m_Mobile.Target;

            if (t == null)
                return false;

            if (t is HailStormSpell.InternalTarget || t is NetherCycloneSpell.InternalTarget)
            {
                if (m_Mobile.Combatant != null && m_Mobile.InRange(m_Mobile.Combatant.Location, 8))
                {
                    t.Invoke(m_Mobile, m_Mobile.Combatant);
                }
                else
                    t.Invoke(m_Mobile, m_Mobile);

                return true;
            }

            return base.ProcessTarget();
        }
    }
}
