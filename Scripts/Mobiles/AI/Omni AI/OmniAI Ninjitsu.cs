// Created by Peoharen
using System;
using Server.Spells;
using Server.Spells.Ninjitsu;

namespace Server.Mobiles
{
    public partial class OmniAI : BaseAI
    {
        public DateTime m_NextShurikenThrow;
        public int m_SmokeBombs;
        public bool m_HasSetSmokeBombs;
        public void NinjitsuPower()
        {
            if (!this.m_HasSetSmokeBombs)
            {
                this.m_HasSetSmokeBombs = true;
                this.m_SmokeBombs = Utility.RandomMinMax(3, 5);
            }

            Spell spell = null;

            if (this.m_Mobile.Hidden)
                this.GetHiddenNinjaMove();
            else if (0.2 > Utility.RandomDouble())
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(995, "Casting Mirror Image");

                spell = new MirrorImage(this.m_Mobile, null);
            }
            else
                this.GetNinjaMove();

            if (spell != null)
                spell.Cast();

            if (DateTime.UtcNow > this.m_NextShurikenThrow && this.m_Mobile.Combatant != null && this.m_Mobile.InRange(this.m_Mobile.Combatant, 12))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.Say(995, "Throwing a shuriken");

                this.m_Mobile.Direction = this.m_Mobile.GetDirectionTo(this.m_Mobile.Combatant);

                if (this.m_Mobile.Body.IsHuman)
                    this.m_Mobile.Animate(this.m_Mobile.Mounted ? 26 : 9, 7, 1, true, false, 0);

                this.m_Mobile.PlaySound(0x23A);
                this.m_Mobile.MovingEffect(this.m_Mobile.Combatant, 0x27AC, 1, 0, false, false);

                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(ShurikenDamage));

                this.m_NextShurikenThrow = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15));
            }
        }

        public void GetHiddenNinjaMove()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(995, "Using a hidden ninja strike");

            int whichone = Utility.RandomMinMax(1, 3);

            if (whichone == 3 && this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 80.0)
                NinjaMove.SetCurrentMove(this.m_Mobile, new KiAttack());
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 30.0)
                NinjaMove.SetCurrentMove(this.m_Mobile, new SurpriseAttack());
            else if (this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 20.0)
                NinjaMove.SetCurrentMove(this.m_Mobile, new Backstab());
        }

        public void GetNinjaMove()
        {
            if (this.m_Mobile.Debug)
                this.m_Mobile.Say(995, "Using a ninja strike");

            int whichone = Utility.RandomMinMax(1, 3);

            if (whichone == 3 && this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 85.0)
                NinjaMove.SetCurrentMove(this.m_Mobile, new DeathStrike());
            else if (whichone >= 2 && this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 60.0)
                NinjaMove.SetCurrentMove(this.m_Mobile, new FocusAttack());
            else
                this.UseWeaponStrike();
        }

        public virtual void ShurikenDamage()
        {
            Mobile target = this.m_Mobile.Combatant as Mobile;

            if (target != null)
            {
                this.m_Mobile.DoHarmful(target);
                AOS.Damage(target, this.m_Mobile, Utility.RandomMinMax(3, 5), 100, 0, 0, 0, 0);

                if (this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 120.0)
                    target.ApplyPoison(this.m_Mobile, Poison.Lethal);
                else if (this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 101.0)
                    target.ApplyPoison(this.m_Mobile, Poison.Deadly);
                else if (this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 100.0)
                    target.ApplyPoison(this.m_Mobile, Poison.Greater);
                else if (this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 70.0)
                    target.ApplyPoison(this.m_Mobile, Poison.Regular);
                else if (this.m_Mobile.Skills[SkillName.Ninjitsu].Value >= 50.0)
                    target.ApplyPoison(this.m_Mobile, Poison.Lesser);
            }
        }
    }
}