// Created by Peoharen
using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Bushido;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public void BushidoPower()
        {
            if (0.5 > Utility.RandomDouble() && !(Confidence.IsConfident(this.m_Mobile) || CounterAttack.IsCountering(this.m_Mobile) || Evasion.IsEvading(this.m_Mobile)))
                this.UseBushidoStance();
            else
                this.UseBushidoMove();
        }

        public void UseBushidoStance()
        {
            Spell spell = null;

            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(2117, "Using a samurai stance");

            BaseWeapon weapon = this.m_Mobile.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            int whichone = Utility.RandomMinMax(1, 3);

            if (whichone == 3 && this.m_Mobile.Skills[SkillName.Bushido].Value >= 60.0)
                spell = new Evasion(this.m_Mobile, null);
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Bushido].Value >= 40.0)
                spell = new CounterAttack(this.m_Mobile, null);
            else if (whichone >= 1 && this.m_Mobile.Skills[SkillName.Bushido].Value >= 25.0)
                spell = new Confidence(this.m_Mobile, null);

            if (spell != null)
                spell.Cast();
        }

        public void UseBushidoMove()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(2117, "Using a samurai or special move strike");

            Mobile comb = this.m_Mobile.Combatant as Mobile;

            if (comb == null)
                return;

            BaseWeapon weapon = this.m_Mobile.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            int whichone = Utility.RandomMinMax(1, 4);

            if (whichone == 4 && this.m_Mobile.Skills[SkillName.Bushido].Value >= 70.0)
                SamuraiMove.SetCurrentMove(this.m_Mobile, new MomentumStrike());
            else if (whichone >= 3 && this.m_Mobile.Skills[SkillName.Bushido].Value >= 50.0)
                SamuraiMove.SetCurrentMove(this.m_Mobile, new LightningStrike());
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Bushido].Value >= 25.0 && comb.Hits <= this.m_Mobile.DamageMin)
                SamuraiMove.SetCurrentMove(this.m_Mobile, new HonorableExecution());
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Tactics].Value >= 90.0 && weapon != null)
                WeaponAbility.SetCurrentAbility(this.m_Mobile, weapon.PrimaryAbility);
            else if (this.m_Mobile.Skills[SkillName.Tactics].Value >= 60.0 && weapon != null)
                WeaponAbility.SetCurrentAbility(this.m_Mobile, weapon.SecondaryAbility);
        }
    }
}