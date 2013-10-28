using System;
using Server.Spells;
using Server.Spells.Mystic;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public void MysticPower()
        {
            Spell spell = null;

            spell = this.GetMysticSpell();

            if (spell != null)
                spell.Cast();

            return;
        }

        public Spell GetMysticSpell()
        {
            Spell spell = null;

            switch( Utility.Random(8) )
            {
                case 0:
                case 1:
                    {
                        if (this.CheckForSleep(this.m_Mobile.Combatant))
                        {
                            this.m_Mobile.DebugSay("Casting Sleep");
                            spell = new SleepSpell(this.m_Mobile, null);
                            break;
                        }
                        else
                            goto case 7;
                    }
                case 2:
                    {
                        if (this.m_Mobile.Followers < 2)
                        {
                            int whichone = Utility.Random(3);

                            if (this.m_Mobile.Skills[SkillName.Mysticism].Value > 80.0 && whichone > 0)
                            {
                                this.m_Mobile.DebugSay("Casting Rising Colossus");
                                spell = new RisingColossusSpell(this.m_Mobile, null);
                            }
                            else if (this.m_Mobile.Skills[SkillName.Mysticism].Value > 30.0)
                            {
                                this.m_Mobile.DebugSay("Casting Animated Weapon");
                                spell = new AnimatedWeaponSpell(this.m_Mobile, null);
                            }
                        }

                        if (spell != null)
                            break;
                        else
                            goto case 7;
                    }
                case 3:
                    {
                        if (this.m_CanShapeShift && this.m_Mobile.Skills[SkillName.Mysticism].Value > 30.0)
                        {
                            this.m_Mobile.DebugSay("Casting Stone Form");
                            spell = new StoneFormSpell(this.m_Mobile, null);
                            break;
                        }
                        else
                            goto case 7;
                    }
                case 4:
                case 5:
                    {
                        if (SpellPlagueSpell.GetSpellPlague(this.m_Mobile.Combatant) == null && this.m_Mobile.Skills[SkillName.Mysticism].Value > 70.0)
                        {
                            this.m_Mobile.DebugSay("Casting Spell Plague");
                            spell = new SpellPlagueSpell(this.m_Mobile, null);
                            break;
                        }
                        else
                            goto case 7;
                    }
                case 6:
                case 7:
                    {
                        switch( Utility.Random((int)(this.m_Mobile.Skills[SkillName.Mysticism].Value / 20)) )
                        {
                            default:
                                spell = new NetherBoltSpell(this.m_Mobile, null);
                                break;
                            case 1:
                                spell = new EagleStrikeSpell(this.m_Mobile, null);
                                break;
                            case 2:
                                spell = new BombardSpell(this.m_Mobile, null);
                                break;
                            case 3:
                                spell = new HailStormSpell(this.m_Mobile, null);
                                break;
                            case 4:
                                spell = new NetherCycloneSpell(this.m_Mobile, null);
                                break;
                        }

                        break;
                    }
            }

            return spell;
        }

        public bool CheckForSleep(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm == null && m is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)m;

                pm = bc.ControlMaster as PlayerMobile;

                if (pm == null)
                    pm = bc.SummonMaster as PlayerMobile;
            }

            if (pm != null || !pm.Asleep)
                return true;
            else
                return false;
        }
    }
}