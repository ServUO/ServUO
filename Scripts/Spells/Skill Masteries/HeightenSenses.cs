using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;

 /*Toggle ability that provides the Parrying Master with increased chance to parry based on parry skill, 
   best weapon skill and mastery level that consumes mana while active.*/
 
namespace Server.Spells.SkillMasteries
{
	public class HeightenedSensesSpell : SkillMasterySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
			"Heightened Senses", "",
			-1,
			9002
		);

		public override double UpKeep { get { return 10; } }
		public override int RequiredMana{ get { return 10; } }
		public override double TickTime { get { return 3; } }
		public override bool BlocksMovement{ get{ return false; } }
        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(1.0); } }

		public override SkillName CastSkill { get { return SkillName.Parry; } }

		public HeightenedSensesSpell(Mobile caster, Item scroll)
			: base(caster, scroll, m_Info)
		{
		}
		
		public override bool CheckCast()
		{
			HeightenedSensesSpell spell = GetSpell(Caster, this.GetType()) as HeightenedSensesSpell;
			
			if(spell != null)
			{
				spell.Expire();
				return false;
			}

            if (!HasShieldOrWeapon())
                return false;
			
			return base.CheckCast();
		}

		public override void OnCast()
		{
			if(CheckSequence())
			{
				Caster.FixedParticles( 0x376A, 9, 32, 5030, 1168, 0, EffectLayer.Waist, 0);
				Caster.PlaySound(0x5BC);
			 
                Caster.SendLocalizedMessage(1156023); // Your senses heighten! 
			
				BeginTimer();
			
				BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.HeightenedSenses, 1155925, 1156062, String.Format("{0}\t{1}", PropertyBonus().ToString(), ScaleUpkeep().ToString()))); // +~1_ARG~% Parry Bonus.<br>Mana Upkeep Cost: ~2_VAL~.
			}
			
			FinishSequence();
		}

        public override bool OnTick()
        {
            if (!HasShieldOrWeapon())
            {
                Expire();
                return false;
            }

            return base.OnTick();
        }

        public bool HasShieldOrWeapon()
        {
            if (!Caster.Player)
                return true;

            BaseShield shield = Caster.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            if (shield == null)
            {
                BaseWeapon weapon = Caster.Weapon as BaseWeapon;

                if (weapon == null || weapon is Fists)
                {
                    Caster.SendLocalizedMessage(1156096); // You must be wielding a shield to use this ability!
                    return false;
                }
            }

            return true;
        }

        protected override void DoEffects()
        {
            Caster.FixedParticles(0x376A, 9, 32, 5005, 1167, 0, EffectLayer.Waist, 0);
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.HeightenedSenses);
        }
		
		public override int PropertyBonus()
		{
            return (int)((Caster.Skills[CastSkill].Value + GetWeaponSkill() + (GetMasteryLevel() * 40)) / 3) / 10;
		}
		
		public static double GetParryBonus(Mobile m)
		{
			HeightenedSensesSpell spell = GetSpell(m, typeof(HeightenedSensesSpell)) as HeightenedSensesSpell;
			
			if(spell != null)
				return (double)spell.PropertyBonus() / 100.0;
				
			return 0;
		}
	}
}