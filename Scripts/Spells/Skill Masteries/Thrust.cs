using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;

/*Toggle ability that provides increased physical attack damage and decreases targets physical attack damage based on 
  the mastery level of the fencer.  This ability consumes mana while active. This ability does not stack with the special move Feint.*/ 

namespace Server.Spells.SkillMasteries
{
    public class ThrustSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Thrust", "",
                -1,
                9002
            );
 
        public override int RequiredMana { get { return 30; } }
        public override SkillName CastSkill { get { return SkillName.Fencing; } }
		public override SkillName DamageSkill { get { return SkillName.Tactics; } }
		
		public const int MaxAttack = 54;
		public const int MaxDefense = 50;
		
		public int AttackModifier { get; set; }
		public int DefenseModifier { get; set; }
			
        public ThrustSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.PlaySound(0x524);
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Caster, BuffIcon.Thrust);
        }
		
		public override bool CheckCast()
		{
			BaseWeapon wep = GetWeapon();
			
			if(!CheckWeapon())
			{
				Caster.SendLocalizedMessage(1155992); // You must have a fencing weapon equipped to use this ability!
				return false;
			}
			
			ThrustSpell spell = GetSpell(Caster, this.GetType()) as ThrustSpell;
			
			if(spell != null)
			{
				spell.Expire();
				return false;
			}
			
			return base.CheckCast();
		}
 
        public override void OnCast()
        {
            Caster.PlaySound(0x101);
            Caster.FixedEffect(0x37C4, 10, 20, 2724, 3);

            AttackModifier = GetMasteryLevel() * 6;
            DefenseModifier = GetMasteryLevel() * 6;

			Caster.PrivateOverheadMessage(MessageType.Regular, 1150, 1155988, Caster.NetState); // *You enter a thrusting stance!*
			
			BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Thrust, 1155989, 1155990, String.Format("{0}\t{1}\t{2}", AttackModifier.ToString(), DefenseModifier.ToString(), ScaleMana(30).ToString())));
			//Your next physical attack will be increased by +~1_VAL~% damage while reducing your victim's physical attack damage by ~2_VAL~%.<br>Mana Upkeep Cost: ~3_VAL~.
			
			BeginTimer();
        }
		
		public override void OnHit(Mobile defender, ref int damage)
		{
			int mana = ScaleMana( GetMana() );
			
			if(Caster.Mana < mana)
			{
				Expire();
				return;
			}
			
			Caster.Mana -= mana;
			
			if(Target != defender)
			{
				AttackModifier = GetMasteryLevel() * 6;
				DefenseModifier = GetMasteryLevel() * 6;
				
				Target = defender;
                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.ThrustDebuff, 1155989, BuffInfo.Blank, ""));
				
				Server.Timer.DelayCall(TimeSpan.FromSeconds(8), () =>
				{
					Reset();
				});
			}
			else
			{
				AttackModifier = Math.Min(MaxAttack, AttackModifier + (GetMasteryLevel() * 6));
				DefenseModifier = Math.Min(MaxDefense, DefenseModifier + (GetMasteryLevel() * 6));
			}
			
			BuffInfo.RemoveBuff(Caster, BuffIcon.Thrust);
			
			BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Thrust, 1155989, 1155990, String.Format("{0}\t{1}\t{2}", AttackModifier.ToString(), DefenseModifier.ToString(), ScaleMana(30).ToString())));
			//Your next physical attack will be increased by +~1_VAL~% damage while reducing your victim's physical attack damage by ~2_VAL~%.<br>Mana Upkeep Cost: ~3_VAL~.
			
			damage = (int)((double)damage + ((double)damage * ((double)DefenseModifier / 100.0)));
            defender.FixedEffect(0x36BD, 20, 10, 2725, 5);
		}
		
		public override void OnGotHit(Mobile attacker, ref int damage)
		{
			if(Target == attacker && DefenseModifier > 0)
				damage = (int)((double)damage - ((double)damage * ((double)DefenseModifier / 100.0)));
		}
		
		private void Reset()
		{
			AttackModifier = 0;
			DefenseModifier = 0;

            if (Target != null)
            {
                BuffInfo.RemoveBuff(Target, BuffIcon.ThrustDebuff);
                Target = null;
            }
		}
    }
}