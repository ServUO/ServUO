using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Engines.Quests;
using Server.Engines.Quests.Necro;
using Server.Misc;
using Server.Regions;
using Server.SkillHandlers;

namespace Server.Mobiles
{
    public class NecroMageAI : MageAI
    {
        private Mobile m_Animated;

        public Mobile Animated
        {
            get { return m_Animated; }
            set { m_Animated = value; }
        }

        public NecroMageAI(BaseCreature m)
            : base(m)
        {
        }

        public virtual double ScaleByNecromancy(double v)
        {
            return m_Mobile.Skills[SkillName.Necromancy].Value * v * 0.1;
        }

        public override bool DoActionWander()
        {
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                if (m_Mobile.Debug)
                    m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;

                NextCastTime = DateTime.Now;
            }
            else if (m_Mobile.Mana < m_Mobile.ManaMax)
            {
                m_Mobile.DebugSay("I am going to meditate");

                m_Mobile.UseSkill(SkillName.Meditation);
            }
            else
            {
                m_Mobile.DebugSay("I am wandering");

                m_Mobile.Warmode = false;

                base.DoActionWander();

                if (m_Mobile.Poisoned)
                {
                    new CureSpell(m_Mobile, null).Cast();
                }
                else if (!m_Mobile.Summoned)
                {
                    if (m_Mobile.Hits < m_Mobile.HitsMax)
                        m_Mobile.UseSkill(SkillName.SpiritSpeak);
                }
            }

            return true;
        }

        public override Spell GetRandomDamageSpell()
        {
            int necro = (int)Math.Min(5, (m_Mobile.Skills[SkillName.Necromancy].Value - 50) / 10);

            if (necro >= 0 && Utility.Random(4) < 3)
            {
                switch (Utility.Random(necro))
                {
                    case 0: return new PoisonStrikeSpell(m_Mobile, null);
                    case 1: return new WitherSpell(m_Mobile, null);
                    case 2: return new StrangleSpell(m_Mobile, null);
                    default: return new WitherSpell(m_Mobile, null);
                }
            }
            else
            {
                return base.GetRandomDamageSpell();
            }
        }

        public override Spell GetRandomCurseSpell()
        {
            int necro = (int)Math.Min(5, (m_Mobile.Skills[SkillName.Necromancy].Value - 20) / 10);

            if (necro >= 0 && Utility.Random(3) < 2)
            {
                switch (Utility.Random(necro))
                {
                    case 0: return new CorpseSkinSpell(m_Mobile, null);
                    case 1: return new EvilOmenSpell(m_Mobile, null);
                    case 2: return new BloodOathSpell(m_Mobile, null);
                    case 3: return new MindRotSpell(m_Mobile, null);
                    default: return new EvilOmenSpell(m_Mobile, null);
                }
            }
            else
            {
                return base.GetRandomCurseSpell();
            }
        }

        public override Spell GetRandomSummonSpell()
        {
            if (m_Mobile.Skills[SkillName.Necromancy].Value >= 100.0 && 0.25 > Utility.RandomDouble())
                return new VengefulSpiritSpell(m_Mobile, null);
            else
                return new AnimateDeadSpell(m_Mobile, null);
        }

        public override Spell ChooseSpell(IDamageable c)
        {
            if (!(c is Mobile))
                return base.ChooseSpell(c);

            Spell spell = CheckCastHealingSpell();

            if (spell != null)
                return spell;

            double damage = ((m_Mobile.Skills[SkillName.SpiritSpeak].Value - ((Mobile)c).Skills[SkillName.MagicResist].Value) / 10) + (c is PlayerMobile ? 18 : 30);

            if (damage > c.Hits)
                return new PainSpikeSpell(m_Mobile, null);
            else
                return base.ChooseSpell(c);
        }

        protected enum NecroComboType
        {
            None,
            Exp_FS_Omen_Poison_PS,
            Exp_MB_Omen_Poison_PS,
            Exp_EB_Omen_Poison_PS,
            Exp_FB_MA_Poison_PS,
            Exp_FB_Poison_PS,
            Exp_FB_MA_PS,
            Exp_Poison_FB_PS,
        }

        private NecroComboType m_NecroComboType;

        public override Spell DoCombo(Mobile c)
        {
            Spell spell = null;

            if (m_NecroComboType == NecroComboType.None)
            {
                m_NecroComboType = (NecroComboType)Utility.RandomMinMax(1, 7);
                m_Combo = 0;
                m_Mobile.DebugSay("Doing {0} Combo", m_NecroComboType);
            }

            if (m_Combo == 0)
            {
                switch (m_NecroComboType)
                {
                    case NecroComboType.Exp_FS_Omen_Poison_PS:
                    case NecroComboType.Exp_MB_Omen_Poison_PS:
                    case NecroComboType.Exp_EB_Omen_Poison_PS:
                    case NecroComboType.Exp_FB_MA_Poison_PS:
                    case NecroComboType.Exp_FB_Poison_PS:
                    case NecroComboType.Exp_FB_MA_PS:
                    case NecroComboType.Exp_Poison_FB_PS: spell = new ExplosionSpell(m_Mobile, null); break;
                }

                ++m_Combo;
            }
            else if (m_Combo == 1)
            {
                switch (m_NecroComboType)
                {
                    case NecroComboType.Exp_FS_Omen_Poison_PS: spell = new FlameStrikeSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_MB_Omen_Poison_PS: spell = new MindBlastSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_EB_Omen_Poison_PS: spell = new EnergyBoltSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_FB_MA_Poison_PS:
                    case NecroComboType.Exp_FB_Poison_PS:
                    case NecroComboType.Exp_FB_MA_PS: spell = new FireballSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_Poison_FB_PS: spell = new PoisonSpell(m_Mobile, null); break;
                }

                ++m_Combo;
            }
            else if (m_Combo == 2)
            {
                switch (m_NecroComboType)
                {
                    case NecroComboType.Exp_FS_Omen_Poison_PS:
                    case NecroComboType.Exp_MB_Omen_Poison_PS:
                    case NecroComboType.Exp_EB_Omen_Poison_PS: spell = new EvilOmenSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_FB_MA_Poison_PS: spell = new MagicArrowSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_FB_Poison_PS: spell = new PoisonSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_FB_MA_PS: spell = new MagicArrowSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_Poison_FB_PS: spell = new FireballSpell(m_Mobile, null); break;
                }

                ++m_Combo;
            }
            else if (m_Combo == 3)
            {
                switch (m_NecroComboType)
                {
                    case NecroComboType.Exp_FS_Omen_Poison_PS:
                    case NecroComboType.Exp_MB_Omen_Poison_PS:
                    case NecroComboType.Exp_EB_Omen_Poison_PS:
                    case NecroComboType.Exp_FB_MA_Poison_PS:
                    case NecroComboType.Exp_FB_Poison_PS: spell = new PoisonSpell(m_Mobile, null); break;
                    case NecroComboType.Exp_FB_MA_PS:
                    case NecroComboType.Exp_Poison_FB_PS:
                        if (Utility.RandomBool())
                            spell = new PoisonStrikeSpell(m_Mobile, null);
                        else
                            spell = new PainSpikeSpell(m_Mobile, null);
                        EndCombo();
                        return spell;
                }

                ++m_Combo;
            }

            else if (m_Combo == 4)
            {
                switch (m_NecroComboType)
                {
                    case NecroComboType.Exp_FS_Omen_Poison_PS:
                    case NecroComboType.Exp_MB_Omen_Poison_PS:
                    case NecroComboType.Exp_EB_Omen_Poison_PS:
                    case NecroComboType.Exp_FB_MA_Poison_PS:
                    case NecroComboType.Exp_FB_Poison_PS:
                    case NecroComboType.Exp_FB_MA_PS:
                    case NecroComboType.Exp_Poison_FB_PS:
                        if (Utility.RandomBool())
                            spell = new PoisonStrikeSpell(m_Mobile, null);
                        else
                            spell = new PainSpikeSpell(m_Mobile, null);
                        EndCombo();
                        return spell;
                }
            }

            return spell;
        }

        public override void EndCombo()
        {
            m_Combo = -1;
            m_NecroComboType = NecroComboType.None;
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
                    if (ScaleByNecromancy(HealChance) > Utility.RandomDouble())
                    {
                        if (m_Mobile.Hits < (m_Mobile.HitsMax - 30))
                            m_Mobile.UseSkill(SkillName.SpiritSpeak);
                    }
                    else if (ScaleByMagery(HealChance) > Utility.RandomDouble())
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
