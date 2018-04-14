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

        private int _DefenseMod;

        public int AttackModifier { get { return (GetMasteryLevel() * 6) * Phase; } }
        public int DefenseModifier
        {
            get
            {
                return _DefenseMod;
            }
            set
            {
                _DefenseMod = Math.Min((GetMasteryLevel() * 6) * 3, value);
            }
        }

        public int DefenseModDisplayed { get { return Math.Min(18, DefenseModifier); } }

        public int _Phase;
        public int Phase
        {
            get { return _Phase; }
            set
            {
                if (value > 3)
                    _Phase = 1;
                else
                    _Phase = value;
            }
        }

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
            if (!CheckSequence())
                return;

            Phase = 1;
            DefenseModifier = GetMasteryLevel() * 6;

            Caster.PlaySound(0x101);
            Caster.FixedEffect(0x37C4, 0x1, 0x8, 0x4EB, 0);

			Caster.PrivateOverheadMessage(MessageType.Regular, 1150, 1155988, Caster.NetState); // *You enter a thrusting stance!*

            BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Thrust, 1155989, 1155990, String.Format("{0}\t{1}\t{2}", AttackModifier.ToString(), DefenseModifier.ToString(), ScaleMana(30).ToString())));
			//Your next physical attack will be increased by +~1_VAL~% damage while reducing your victim's physical attack damage by ~2_VAL~%.<br>Mana Upkeep Cost: ~3_VAL~.

            FinishSequence();
			BeginTimer();
        }
		
		public override void OnHit(Mobile defender, ref int damage)
		{
            int currentMod = AttackModifier;

			if(Target != defender)
			{
                Phase = 1;
                DefenseModifier = (GetMasteryLevel() * 6);

				Target = defender;     
                new InternalTimer(this, defender);
			}
			else
			{
                DefenseModifier += (GetMasteryLevel() * 6);
                Phase++;
			}

            damage = (int)((double)damage + ((double)damage * ((double)currentMod / 100.0)));
            defender.FixedEffect(0x36BD, 0x1, 0xE, 0x776, 0);

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.ThrustDebuff, 1155989, 1156234, TimeSpan.FromSeconds(8), defender, DefenseModifier.ToString()));
            // All damage from your physical attacks have been reduced by ~1_val~%.

			BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Thrust, 1155989, 1155990, String.Format("{0}\t{1}\t{2}", AttackModifier.ToString(), (GetMasteryLevel() * 6).ToString(), ScaleMana(30).ToString())));
			//Your next physical attack will be increased by +~1_VAL~% damage while reducing your victim's physical attack damage by ~2_VAL~%.<br>Mana Upkeep Cost: ~3_VAL~.

            if (!CheckMana())
            {
                Reset();
                Expire();
            }
		}
		
		public override void OnGotHit(Mobile attacker, ref int damage)
		{
			if(Target == attacker && DefenseModifier > 0)
				damage = (int)((double)damage - ((double)damage * ((double)DefenseModifier / 100.0)));
		}

        private bool CheckMana()
        {
            int mana = ScaleMana(GetMana());

            if (Caster.Mana < mana)
            {
                return false;
            }

            Caster.Mana -= mana;
            return true;
        }
		
		private void Reset()
		{
			DefenseModifier = 0;
            Target = null;

            BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Thrust, 1155989, 1155990, String.Format("{0}\t{1}\t{2}", AttackModifier.ToString(), (GetMasteryLevel() * 6).ToString(), ScaleMana(30).ToString())));
            //Your next physical attack will be increased by +~1_VAL~% damage while reducing your victim's physical attack damage by ~2_VAL~%.<br>Mana Upkeep Cost: ~3_VAL~.

            if (Target != null)
            {
                BuffInfo.RemoveBuff(Target, BuffIcon.ThrustDebuff);
            }
		}

        private class InternalTimer : Timer
        {
            public Mobile Target { get; set; }
            public DateTime Expires { get; set; }
            public ThrustSpell Spell { get; set; }

            public int DamageModifier { get; set; }

            public InternalTimer(ThrustSpell spell, Mobile target)
                : base(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250))
            {
                Target = target;
                Spell = spell;
                DamageModifier = spell.DefenseModifier;

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(8);
                Start();
            }

            protected override void OnTick()
            {
                if (Expires < DateTime.UtcNow)
                {
                    Spell.Reset();
                    Stop();
                }
            }
        }
    }
}