using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

/*Target Strength Reduced by up to 32, 20 – 60 Damage (Physical), every 2 seconds, 
affected by the target's magic resistance. (PvP and specific Mobs only).*/

namespace Server.Spells.SkillMasteries
{
	public class DespairSpell : BardSpell
	{
		public static readonly string ModName = "Despair";
	
		private static SpellInfo m_Info = new SpellInfo(
				"Despair", "Kal Des Mani Tym",
				-1,
				9002
			);

		public override double RequiredSkill{ get { return 90; } }
		public override double UpKeep { get { return 12; } }
		public override int RequiredMana{ get { return 26; } }
		public override bool PartyEffects { get { return false; } }
        public override SkillName CastSkill { get { return SkillName.Discordance; } }

        private int m_StatMod;
        private int m_Damage;

		public DespairSpell( Mobile caster, Item scroll ) : base(caster, scroll, m_Info)
		{
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
            else if (!m.Alive)
            {
                Caster.SendLocalizedMessage(1115773); // Your target is dead.
            }
            else if (Caster == m)
            {
                // TODO: Message?
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

                m_StatMod = (int)((BaseSkillBonus * 25) + (CollectiveBonus * 7)) * -1;
                m_Damage = GetDamage();

                string args = String.Format("{0}\t{1}", m_StatMod, m_Damage);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.DespairTarget, 1115741, 1115743, args.ToString()));
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.DespairCaster, 1115741, 1115743, args.ToString()));

                BeginTimer();
            }
			
			FinishSequence();
		}

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Target, BuffIcon.DespairTarget);
            BuffInfo.RemoveBuff(Caster, BuffIcon.DespairCaster);
        }

		public override void AddStatMods()
		{
			int offset = m_StatMod;
			
			if(Target != null)
				Target.AddStatMod(new StatMod(StatType.Str, ModName, offset, TimeSpan.Zero));
		}
		
		public override void RemoveStatMods()
		{
			if(Target != null)
				Target.RemoveStatMod(ModName);
		}

        private int GetDamage()
        {
            int music = (int)Caster.Skills[DamageSkill].Value;
            int disc = (int)Caster.Skills[CastSkill].Value;
            int prov = (int)Caster.Skills[SkillName.Provocation].Value;
            int peac = (int)Caster.Skills[SkillName.Peacemaking].Value;

            int damage = 27;

            if (music >= 100)
                damage += 6 * ((music / 10) - 10);

            if (disc >= 100)
                damage += 3 * ((disc / 10) - 10);
            if (prov >= 100)
                damage += 3 * ((prov / 10) - 10);
            if (peac >= 100)
                damage += 3 * ((peac / 10) - 10);

            return damage;
        }
		
		public override bool OnTick()
		{
			base.OnTick();
			
			if(!Caster.Player || Target == null || !Caster.InRange(Target.Location, PartyRange))
				return false;

            int damage = m_Damage;
			double modifier = DamageModifier(Target);
			
			damage -= (int)((double)damage * modifier);

            AOS.Damage(Target, Caster, damage, 100, 0, 0, 0, 0); // Now only does physical
            return true;
			/*switch(((PlayerMobile)Caster).MasteryDamType)
			{
				case ResistanceType.Physical: 	AOS.Damage(Target, Caster, damage, 100, 0, 0, 0, 0); break;
				case ResistanceType.Fire: 		AOS.Damage(Target, Caster, damage, 0, 100, 0, 0, 0); break;
				case ResistanceType.Cold: 		AOS.Damage(Target, Caster, damage, 0, 0, 100, 0, 0); break;
				case ResistanceType.Poison: 	AOS.Damage(Target, Caster, damage, 0, 0, 0, 100, 0); break;
				case ResistanceType.Energy: 	AOS.Damage(Target, Caster, damage, 0, 0, 0, 0, 100); break;
			}*/
		}
		
		private class InternalTarget : Target
		{
			private DespairSpell m_Owner;
			
			public InternalTarget(DespairSpell spell) : base(10, false, TargetFlags.Harmful)
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