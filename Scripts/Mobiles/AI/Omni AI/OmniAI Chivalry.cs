// Created by Peoharen
using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Chivalry;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public void ChivalryPower()
        {
            if (Utility.Random(100) > 30)
            {
                Spell spell = null;

                spell = this.GetPaladinSpell();

                if (spell != null)
                    spell.Cast();
            }
            else
                this.UseWeaponStrike();

            return;
        }

        public Spell GetPaladinSpell()
        {
            if (this.CheckForRemoveCurse() == true && Utility.RandomDouble() > 0.25)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1154, "Casting Remove Curse");

                return new RemoveCurseSpell(this.m_Mobile, null);
            }

            int whichone = Utility.RandomMinMax(1, 4);

            if (whichone == 4 && this.m_Mobile.Skills[SkillName.Chivalry].Value >= 55.0 && this.m_Mobile.Mana >= 10)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1154, "Casting Holy Light");

                return new HolyLightSpell(this.m_Mobile, null);
            }
            else if (whichone >= 3 && this.CheckForDispelEvil())
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1154, "Casting Dispel Evil");

                return new DispelEvilSpell(this.m_Mobile, null);
            }
            else if (whichone >= 2 && !(DivineFurySpell.UnderEffect(this.m_Mobile)) && this.m_Mobile.Skills[SkillName.Chivalry].Value >= 35.0)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1154, "Casting Divine Fury");

                return new DivineFurySpell(this.m_Mobile, null);
            }
            else if (this.CheckForConsecrateWeapon())
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(1154, "Casting Consecrate Weapon");

                return new ConsecrateWeaponSpell(this.m_Mobile, null);
            }
            else
                return null;
        }

        public bool CheckForConsecrateWeapon()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1154, "Checking to bless my weapon");

            if (this.m_Mobile.Skills[SkillName.Chivalry].Value < 15.0 || this.m_Mobile.Mana <= 9)
                return false;

            BaseWeapon weapon = this.m_Mobile.Weapon as BaseWeapon;

            if (weapon != null && !weapon.Consecrated)
                return true;
            else
                return false;
        }

        public bool CheckForDispelEvil()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1154, "Checking to dispel evil");

            if (this.m_Mobile.Skills[SkillName.Chivalry].Value < 35.0 || this.m_Mobile.Mana <= 9)
                return false;

            bool cast = false;

            foreach (Mobile m in this.m_Mobile.GetMobilesInRange(4))
            {
                if (m != null)
                {
                    if (m is BaseCreature && ((BaseCreature)m).Summoned && !((BaseCreature)m).IsAnimatedDead)
                        cast = true;
                    else if (m is BaseCreature && !((BaseCreature)m).Controlled && ((BaseCreature)m).Karma < 0)
                        cast = true;
                    else if (TransformationSpellHelper.GetContext(m) != null)
                        cast = true;
                }
                continue;
            }

            return cast;
        }

        public bool CheckForRemoveCurse()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(1154, "Checking for remove curse");

            if (this.m_Mobile.Skills[SkillName.Chivalry].Value < 5.0 || this.m_Mobile.Mana <= 19)
                return false;

            StatMod mod;

            mod = this.m_Mobile.GetStatMod("[Magic] Str Offset");

            if (mod == null)
                mod = this.m_Mobile.GetStatMod("[Magic] Dex Offset");

            if (mod == null)
                mod = this.m_Mobile.GetStatMod("[Magic] Int Offset");

            if (mod != null && mod.Offset < 0)
                return true;

            Mobile foe = this.m_Mobile.Combatant;

            if (foe == null)
                return false;

            //There is no way to know if they are under blood oath or strangle without editing the spells so we just check for necro skills instead.
            if (foe.Skills[SkillName.Necromancy].Value > 20.0 && Utility.RandomDouble() > 0.6)
                return true;

            return false;
        }
    }
}