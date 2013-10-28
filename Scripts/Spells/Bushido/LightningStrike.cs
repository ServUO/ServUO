using System;
using Server.Mobiles;

namespace Server.Spells.Bushido
{
    public class LightningStrike : SamuraiMove
    {
        public LightningStrike()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 5;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 50.0;
            }
        }
        public override TextDefinition AbilityMessage
        {
            get
            {
                return new TextDefinition(1063167);
            }
        }// You prepare to strike quickly.
        public override bool DelayedContext
        {
            get
            {
                return true;
            }
        }
        public override bool ValidatesDuringHit
        {
            get
            {
                return false;
            }
        }
        public override int GetAccuracyBonus(Mobile attacker)
        {
            return 50;
        }

        public override bool Validate(Mobile from)
        {
            bool isValid = base.Validate(from);
            if (isValid)
            {
                PlayerMobile ThePlayer = from as PlayerMobile;
                ThePlayer.ExecutesLightningStrike = this.BaseMana;
            }
            return isValid;
        }

        public override bool IgnoreArmor(Mobile attacker)
        {
            double bushido = attacker.Skills[SkillName.Bushido].Value;
            double criticalChance = (bushido * bushido) / 72000.0;
            return (criticalChance >= Utility.RandomDouble());
        }

        public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            /* no mana drain before actual hit */
            bool enoughMana = this.CheckMana(attacker, false);
            return this.Validate(attacker);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            ClearCurrentMove(attacker);
            if (this.CheckMana(attacker, true))
            {
                attacker.SendLocalizedMessage(1063168); // You attack with lightning precision!
                defender.SendLocalizedMessage(1063169); // Your opponent's quick strike causes extra damage!
                defender.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
                defender.PlaySound(0x51D);
                this.CheckGain(attacker);
                this.SetContext(attacker);
            }
        }

        public override void OnClearMove(Mobile attacker)
        {
            PlayerMobile ThePlayer = attacker as PlayerMobile; // this can be deletet if the PlayerMobile parts are moved to Server.Mobile 
            ThePlayer.ExecutesLightningStrike = 0;
        }
    }
}