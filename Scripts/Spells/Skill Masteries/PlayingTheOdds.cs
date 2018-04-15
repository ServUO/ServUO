using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.SkillMasteries
{
    public class PlayingTheOddsSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Playing the Odds", "",
                -1,
                9002
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 0; } } // get
        public override int RequiredMana { get { return 30; } }
        public override bool PartyEffects { get { return true; } }

        public override SkillName CastSkill { get { return SkillName.Archery; } }
        public override SkillName DamageSkill { get { return SkillName.Tactics; } }

        private int _HCIBonus;
        private int _SSIBonus;

        public PlayingTheOddsSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            base.SendCastEffect();

            Effects.SendTargetParticles(Caster, 0x3709, 10, 30, 2728, 0, 9907, EffectLayer.LeftFoot, 0);
        }

        public override bool CheckCast()
        {
            if (IsInCooldown(Caster, this.GetType()))
                return false;

            if (!CheckWeapon())
            {
                Caster.SendLocalizedMessage(1156000); // You must have an Archery weapon to use this ability!
                return false;
            }

            if (SkillMasterySpell.UnderPartyEffects(Caster, typeof(PlayingTheOddsSpell)))
            {
                Caster.SendLocalizedMessage(1062945); // That ability is already in effect.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            BaseWeapon wep = GetWeapon();

            if (wep != null && CheckSequence())
            {
                wep.PlaySwingAnimation(Caster);

                TimeSpan duration = TimeSpan.FromMinutes(1);

                double skill = (Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 2;

                _HCIBonus = (int)Math.Max(45, skill / 2.667);
                _SSIBonus = (int)Math.Max(30, skill / 4);

                string args = String.Format("{0}\t{1}\t{2}", Caster.Name, _HCIBonus.ToString(), _SSIBonus.ToString());
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.PlayingTheOddsDebuff, 1155913, 1156091, duration, Caster));
                //Your bow range has been reduced as you play the odds.

                foreach (Mobile m in GetParty().Where(mob => mob is PlayerMobile))
                {
                    m.PlaySound(0x101);
                    m.FixedEffect(0x13B2, 10, 20, 2728, 5);
                    m.FixedEffect(0x37C4, 10, 20, 2728, 5);

                    if(m != Caster)
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PlayingTheOdds, 1155913, 1155998, duration, m, args));
                        //~1_NAME~ grants you the following:<br>+~2_VAl~% Hit Chance Increase.<br>+~3_VAL~% Swing Speed Increase.
                }

                Caster.SendLocalizedMessage(1156091); // Your bow range has been reduced as you play the odds.

                Expires = DateTime.UtcNow + duration;
                BeginTimer();

                AddToCooldown(TimeSpan.FromSeconds(90));

                IPooledEnumerable eable = Caster.Map.GetMobilesInRange(Caster.Location, 5);
                List<Mobile> targets = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (Caster != m && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                    {
                        if (!Caster.InLOS(m))
                            continue;

                        targets.Add(m);
                    }
                }
                eable.Free();

                foreach (Mobile mob in targets)
                {
                    if (HitLower.ApplyDefense(mob))
                    {
                        if(wep is BaseRanged && !(wep is BaseThrown))
                            Caster.MovingEffect(mob, ((BaseRanged)wep).EffectID, 18, 1, false, false);

                        mob.PlaySound(0x28E);
                        Effects.SendTargetEffect(mob, 0x37BE, 1, 4, 0x23, 3);

                        Caster.DoHarmful(mob);
                    }
                }

                wep.InvalidateProperties();
            }

            FinishSequence();
        }

        public override void EndEffects()
        {
            if (PartyList != null)
            {
                PartyList.ForEach(m =>
                {
                    BuffInfo.RemoveBuff(m, BuffIcon.PlayingTheOdds);
                });
            }

            BaseWeapon wep = GetWeapon();

            if (wep != null)
                wep.InvalidateProperties();

            BuffInfo.RemoveBuff(Caster, BuffIcon.PlayingTheOddsDebuff);
            Caster.SendLocalizedMessage(1156092); // Your bow range has returned to normal.
        }

        public static int HitChanceBonus(Mobile m)
        {
            PlayingTheOddsSpell spell = GetSpellForParty(m, typeof(PlayingTheOddsSpell)) as PlayingTheOddsSpell;

            if (spell != null)
                return spell._HCIBonus;

            return 0;
        }

        public static int SwingSpeedBonus(Mobile m)
        {
            PlayingTheOddsSpell spell = GetSpellForParty(m, typeof(PlayingTheOddsSpell)) as PlayingTheOddsSpell;

            if (spell != null)
                return spell._SSIBonus;

            return 0;
        }

        public static int RangeModifier(BaseWeapon weapon)
        {
            if (weapon is BaseRanged && !(weapon is BaseThrown))
            {
                Mobile m = weapon.RootParent as Mobile;

                if (m != null)
                {
                    PlayingTheOddsSpell spell = GetSpell(m, typeof(PlayingTheOddsSpell)) as PlayingTheOddsSpell;

                    if (spell != null)
                    {
                        return weapon.DefMaxRange / 2;
                    }
                       
                }
            }

            return weapon.DefMaxRange;
        }
    }
}
