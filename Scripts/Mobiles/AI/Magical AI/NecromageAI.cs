#region References
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Second;
using Server.Targeting;
using Server.Spells.SkillMasteries;
#endregion

namespace Server.Mobiles
{
    public class NecroMageAI : MageAI
    {
        public override SkillName CastSkill => SkillName.Magery;

        public NecroMageAI(BaseCreature m)
            : base(m)
        { }

        public override Spell GetRandomDamageSpell()
        {
            if (0.5 > Utility.RandomDouble())
            {
                return base.GetRandomDamageSpell();
            }

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

        public override Spell GetRandomCurseSpell()
        {
            if (0.5 > Utility.RandomDouble())
            {
                return base.GetRandomCurseSpell();
            }

            int mana = m_Mobile.Mana;
            int select = 1;

            if (mana >= 17)
                select = 4;
            else if (mana >= 13)
                select = 3;
            else if (mana >= 11)
                select = 2;

            switch (Utility.Random(select))
            {
                case 0:
                    Spell spell;

                    if (CheckCastCorpseSkin(m_Mobile) && Utility.RandomBool())
                        spell = new CorpseSkinSpell(m_Mobile, null);
                    else
                        spell = new EvilOmenSpell(m_Mobile, null);

                    return spell;
                case 1:
                    return new EvilOmenSpell(m_Mobile, null);
                case 2:
                    return new BloodOathSpell(m_Mobile, null);
                case 3:
                    return new MindRotSpell(m_Mobile, null);
            }

            return null;
        }

        public override Spell GetRandomBuffSpell()
        {
            if (0.5 > Utility.RandomDouble())
            {
                return base.GetRandomBuffSpell();
            }

            if (!SmartAI && Utility.RandomBool())
            {
                return new CurseWeaponSpell(m_Mobile, null);
            }

            if (Utility.RandomBool())
            {
                return new ConduitSpell(m_Mobile, null);
            }

            return GetRandomSummonSpell();
        }

        public override Spell GetRandomSummonSpell()
        {
            if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.Mana >= 23)
            {
                return new AnimateDeadSpell(m_Mobile, null);
            }

            return null;
        }

        protected override Spell CheckCastHealingSpell()
        {
            if (m_Mobile.Summoned || m_Mobile.Hits >= m_Mobile.HitsMax)
                return null;

            if (0.1 > Utility.RandomDouble())
                m_Mobile.UseSkill(SkillName.SpiritSpeak);
            else
                return base.CheckCastHealingSpell();

            return null;
        }

        public override bool DoActionGuard()
        {
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false))
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
                    if (ScaleBySkill(HealChance, SkillName.Necromancy) > Utility.RandomDouble() &&
                        m_Mobile.Hits < m_Mobile.HitsMax - 30)
                    {
                        m_Mobile.UseSkill(SkillName.SpiritSpeak);
                    }
                    else if (ScaleBySkill(HealChance, SkillName.Magery) > Utility.RandomDouble())
                    {
                        if (m_Mobile.Hits < m_Mobile.HitsMax - 50)
                        {
                            if (!new GreaterHealSpell(m_Mobile, null).Cast())
                                new HealSpell(m_Mobile, null).Cast();
                        }
                        else if (m_Mobile.Hits < m_Mobile.HitsMax - 10)
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

        public static bool CheckCastCorpseSkin(BaseCreature bc)
        {
            return bc.ColdDamage != 100 && bc.PhysicalDamage != 100;
        }

        public override bool IsBeneficial(Target targ)
        {
            if (targ is SkillMasterySpell.MasteryTarget && ((SkillMasterySpell.MasteryTarget)targ).Owner is ConduitSpell)
            {
                return true;
            }

            return base.IsBeneficial(targ);
        }
    }
}
