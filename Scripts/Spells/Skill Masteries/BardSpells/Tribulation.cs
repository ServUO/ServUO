using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;

/*Target Hit Chance reduced by up to 33%, Spell Damaged reduced by 33%, Damage 
 Taken can trigger additional damage between 20-60% of the damage taken as 
 physical once per second. (Discordance Based) (Chance to Trigger damage 
 Musicianship Based) affected by the target's magic resistance. (PvP and 
 specific Mobs only)*/

namespace Server.Spells.SkillMasteries
{
	public class TribulationSpell : BardSpell
	{
		private DateTime m_NextDamage;
		
		private static SpellInfo m_Info = new SpellInfo(
				"Tribulation", "In Jux Hur Rel",
				-1,
				9002
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 10; } }
		public override int RequiredMana{ get { return 24; } }
		public override bool PartyEffects { get { return false; } }
        public override double TickTime { get { return 1.5; } }

        public override SkillName CastSkill { get { return SkillName.Discordance; } }

        private int m_PropertyBonus;
        private double m_DamageChance;
        private double m_SlayerBonus;

		public TribulationSpell( Mobile caster, Item scroll ) : base(caster, scroll, m_Info)
		{
			m_NextDamage = DateTime.UtcNow;
		}
		
		public override void OnCast()
		{
            BardSpell spell = SkillMasterySpell.GetSpell(Caster, this.GetType()) as BardSpell;
			
			if(spell != null)
			{
				spell.Expire();
                Caster.SendLocalizedMessage(1115774); //You halt your spellsong.
			}
			else
			{
				Caster.Target = new InternalTarget(this);
			}
		}
		
		public void OnTarget( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if (Caster == m)
			{
                Caster.SendMessage("You cannot target yourself!");
			}
            else if (BardSpell.HasHarmfulEffects(m, this.GetType()))
            {
                Caster.SendLocalizedMessage(1115772); //Your target is already under the effect of this spellsong.
            }
            else if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                Target = m;
                //Caster.SendLocalizedMessage(1234567); //TODO: Message?

                HarmfulSpell(m);

                m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);

                m_PropertyBonus = (int)Math.Max(5, ((BaseSkillBonus * 23) + (CollectiveBonus * 8))) * -1;
                m_DamageChance = Math.Max(20.0, (Caster.Skills[DamageSkill].Value / 2.0)) / 100;
                m_SlayerBonus = 1;

                string args = String.Format("{0}\t{1}\t{2}", m_PropertyBonus, m_PropertyBonus, (int)(m_DamageChance * 100));
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.TribulationTarget, 1115740, 1115743, args.ToString()));
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.TribulationCaster, 1115740, 1115742, args.ToString()));

                ISlayer slayer = Instrument as ISlayer;

                if (slayer != null)
                {
                    SlayerEntry se1 = SlayerGroup.GetEntryByName(slayer.Slayer);
                    SlayerEntry se2 = SlayerGroup.GetEntryByName(slayer.Slayer2);

                    if ((se1 != null && se1.Slays(Target)) || (se2 != null && se2.Slays(Target)))
                    {
                        m_SlayerBonus = 1.5;
                    }
                }

                BeginTimer();
            }
			
			FinishSequence();
		}

        public override bool OnTick()
        {
            if(Target != null && Target.Alive && Target.Map != null)
                Target.FixedEffect(0x376A, 1, 32);

            return base.OnTick();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Target, BuffIcon.TribulationTarget);
            BuffInfo.RemoveBuff(Caster, BuffIcon.TribulationCaster);
        }
		
		/// <summary>
		/// Called in BaseCreature.cs - Damage after damage
		/// </summary>
		/// <param name="victim"></param>
		/// <param name="damageTaken"></param>
		public override void DoDamage(Mobile victim, int damageTaken)
		{
			if(m_NextDamage > DateTime.UtcNow || !Caster.Player)
				return;
				
            if (m_DamageChance > Utility.RandomDouble())
			{
				double mod = Math.Max(20, ((Caster.Skills[CastSkill].Value / 120) * 60));
				mod /= 100;
				int damage = (int)(damageTaken * mod);
				
				double reduce = DamageModifier(victim);
				
				damage -= (int)(damage * reduce);
                damage = (int)((double)damage * m_SlayerBonus);
                
                AOS.Damage(victim, Caster, damage, 100, 0, 0, 0, 0);
				
				m_NextDamage = DateTime.UtcNow + TimeSpan.FromSeconds(1);
			}
		}
		
		/// <summary>
		/// Called in AOS.cs - HCI Malus
		/// </summary>
		/// <returns></returns>
		public override int PropertyBonus()
		{
			return m_PropertyBonus;
		}
		
		/// <summary>
		/// Called in Spell.cs - absorbed damage after all modifiers
		/// </summary>
		/// <param name="damage"></param>
		public override void AbsorbDamage(ref int damage)
		{
			double mod = (BaseSkillBonus * 25) + (CollectiveBonus * 8);
			mod /= 100;
			
			damage -= (int)(damage * mod);
		}
		
		private class InternalTarget : Target
		{
			private TribulationSpell m_Owner;
			
			public InternalTarget(TribulationSpell spell) : base(10, false, TargetFlags.Harmful)
			{
				m_Owner = spell;
			}
			
			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.OnTarget( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}
