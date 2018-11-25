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
        public override double SlayerBonus { get { return 3.0; } }

        private int m_StatMod;
        private int m_Damage;
        private int m_Rounds;

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

                int rounds = 5 + (int)((double)BaseSkillBonus * .75);

                m_StatMod = (int)((BaseSkillBonus * 2) + CollectiveBonus);
                m_Damage = (int)((BaseSkillBonus * 4.5) + (CollectiveBonus * 2));
                m_Rounds = 5 + (int)((BaseSkillBonus * .75) + (CollectiveBonus / 2));

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

        public override bool OnTick()
        {
            bool tick = base.OnTick();

            if (Target == null || !Caster.InRange(Target.Location, PartyRange))
                return false;

            int damage = m_Damage;

            if (!Target.Player)
                damage += AOS.Scale(damage, 50); // pvm = 1.5x

            damage = (int)((double)damage * GetSlayerBonus()); // 3.0x slayer bonus
            damage -= (int)((double)damage * DamageModifier(Target)); // resist modifier

            AOS.Damage(Target, Caster, damage, 100, 0, 0, 0, 0); // Now only does physical

            if (Target != null && Target.Alive && Target.Map != null)
                Target.FixedEffect(0x376A, 1, 32);

            if (m_Rounds-- == 0)
            {
                Expire();
            }

            return tick;
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