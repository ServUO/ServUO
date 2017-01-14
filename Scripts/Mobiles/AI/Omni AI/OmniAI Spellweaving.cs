// Created by Peoharen
using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Spellweaving;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public void SpellweavingPower()
        {
            this.CheckFocus();

            if (this.CheckForAttuneWeapon()) // Always attune if needed
                return;
            else if (this.CheckForGiftOfRenewal()) // Always use renewal
                return;

            if (this.m_Mobile.Combatant == null)
                return;

            // TODO improve selecting the spell.

            int choices = 5; // default choices: allure, summon, melee, melee, thunderstorm

            if (this.m_Mobile.Mana > 50)
            {
                if (this.m_Mobile.Skills[SkillName.Spellweaving].Value > 90.0) // add word of death+, uses 50 mana
                    choices += 4;
                else if (this.m_Mobile.Skills[SkillName.Spellweaving].Value > 66.0) // add wildfire+, uses 50 mana
                    choices += 3;
                else if (this.m_Mobile.Skills[SkillName.Spellweaving].Value > 62.0) // add essence of wind+, uses 42 mana
                    choices += 2;
                else if (this.m_Mobile.Skills[SkillName.Spellweaving].Value > 44.0) // add empower, uses 50 mana
                    ++choices;
            }

            switch( Utility.Random(choices) )
            {
                case 0: // Allure
                    {
                        if (this.m_Mobile.Combatant is BaseCreature)
                        {
                            if (this.m_Mobile.Debug)
                                this.m_Mobile.Say(1436, "Casting Dryad Allure");

                            new DryadAllureSpell(this.m_Mobile, null).Cast();
                            break;
                        }
                        else
                            goto case 1;
                    }
                case 1: // Summon
                    {
                        if (this.m_Mobile.Followers < this.m_Mobile.FollowersMax)
                        {
                            Spell spell = this.GetSpellweavingSummon();

                            if (spell != null)
                            {
                                if (this.m_Mobile.Debug)
                                    this.m_Mobile.Say(1436, "Summoning help");

                                spell.Cast();
                            }

                            this.ForceTarget();
                            break;
                        }
                        else
                            goto case 2;
                    }
                case 2:
                case 3: // Do nothing, aka melee
                    {
                        break;
                    }
                case 4:
                    {
                        new ThunderstormSpell(this.m_Mobile, null).Cast();
                        break;
                    }
                case 5: // Empower, increases summons & healing.
                    {
                        if (ArcaneEmpowermentSpell.GetSpellBonus(this.m_Mobile, true) == 0)
                        {
                            new ArcaneEmpowermentSpell(this.m_Mobile, null).Cast();
                            break;
                        }
                        else
                            goto case 2;
                    }
                case 6: // Essence of Wind, cold aura and speed debuff.
                    {
                        if (m_Mobile.Combatant is Mobile && !EssenceOfWindSpell.IsDebuffed((Mobile)m_Mobile.Combatant))
                        {
                            new EssenceOfWindSpell(this.m_Mobile, null).Cast();
                            break;
                        }
                        else
                            goto case 2;
                    }
                case 7:
                    {
                        new WildfireSpell(this.m_Mobile, null).Cast();
                        this.ForceTarget();
                        break;
                    }
                case 8:
                    {
                        new WordOfDeathSpell(this.m_Mobile, null).Cast();
                        break;
                    }
            }

            return;
        }

        // Due to many spells having no target flag we have to force it.
        public void ForceTarget()
        {
            if (this.m_Mobile.Target != null && this.m_Mobile.Combatant != null)
                this.m_Mobile.Target.Invoke(this.m_Mobile, this.m_Mobile.Combatant);
        }

        public void CheckFocus()
        {
            ArcaneFocus focus = ArcanistSpell.FindArcaneFocus(this.m_Mobile);

            if (focus != null)
                return;

            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1436, "I have no Arcane Focus");

            BaseCreature bc = null;
            int power = 1;

            foreach (Mobile m in this.m_Mobile.GetMobilesInRange(10))
            {
                if (m == null)
                    continue;
                else if (m == this.m_Mobile)
                    continue;
                else if (!(m is BaseCreature))
                    continue;

                bc = (BaseCreature)m;

                if (bc.Skills[SkillName.Spellweaving].Value > 50.0)
                    if (this.m_Mobile.Controlled == bc.Controlled && this.m_Mobile.Summoned == bc.Summoned)
                        power++;
            }

            if (power > 6)
                power = 6;
            else if (power < 2) // No spellweavers found, setting to min required.
                power = 2;

            ArcaneFocus f = new ArcaneFocus(TimeSpan.FromHours(1), power);

            Container pack = this.m_Mobile.Backpack;

            if (pack == null)
            {
                this.m_Mobile.EquipItem(new Backpack());
                pack = this.m_Mobile.Backpack;
            }

            pack.DropItem(f);

            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1436, "I created an Arcane Focus, it's level is: " + power.ToString());
        }

        public Spell GetSpellweavingSummon()
        {
            if (this.m_Mobile.Skills[SkillName.Spellweaving].Value > 38.0)
            {
                if (this.m_Mobile.Serial.Value % 2 == 0)
                    return new SummonFeySpell(this.m_Mobile, null);
                else
                    return new SummonFiendSpell(this.m_Mobile, null);
            }
            else
                return new NatureFurySpell(this.m_Mobile, null);
        }

        public bool CheckForAttuneWeapon()
        {
            if (!AttuneWeaponSpell.IsAbsorbing(this.m_Mobile) && this.m_Mobile.Mana > 24)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1436, "Casting Attune Weapon");

                new AttuneWeaponSpell(this.m_Mobile, null).Cast();
                return true;
            }

            return false;
        }

        public bool CheckForGiftOfRenewal()
        {
            if (GiftOfRenewalSpell.m_Table.ContainsKey(this.m_Mobile) || !this.m_Mobile.CanBeginAction(typeof(GiftOfRenewalSpell)))
                return false;
            else if (this.m_Mobile.Skills[SkillName.Spellweaving].Value > 20.0 && this.m_Mobile.Mana > 24)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1436, "Casting Gift Of Renewal");

                new GiftOfRenewalSpell(this.m_Mobile, null).Cast();
                return true;
            }

            return false;
        }
    }
}