using Server.Targeting;
using System;

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

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Tribulation", "In Jux Hur Rel",
                -1,
                9002
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 8;
        public override int RequiredMana => 24;
        public override bool PartyEffects => false;
        public override double TickTime => 2.0;

        public override SkillName CastSkill => SkillName.Discordance;

        private int m_PropertyBonus;
        private double m_DamageChance;
        private int m_DamageFactor;
        private int m_Rounds;

        public TribulationSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
            m_NextDamage = DateTime.UtcNow;
        }

        public override void OnCast()
        {
            BardSpell spell = GetSpell(Caster, GetType()) as BardSpell;

            if (spell != null)
            {
                spell.Expire();
                Caster.SendLocalizedMessage(1115774); //You halt your spellsong.
            }
            else
            {
                Caster.Target = new InternalTarget(this);
            }
        }

        public void OnTarget(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (Caster == m)
            {
                Caster.SendMessage("You cannot target yourself!");
            }
            else if (HasHarmfulEffects(m, GetType()))
            {
                Caster.SendLocalizedMessage(1115772); //Your target is already under the effect of this spellsong.
            }
            else if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                Target = m;

                HarmfulSpell(m);

                m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);

                double cast = Caster.Skills[CastSkill].Value;
                double dam = Caster.Skills[DamageSkill].Value;

                m_PropertyBonus = (int)((BaseSkillBonus * 2.75) + (CollectiveBonus * 1.667));               // 5 - 22 (32)
                m_DamageChance = (int)((BaseSkillBonus * 7.5) + (CollectiveBonus * 3));                     // 15 - 60 (84)
                m_DamageFactor = (int)((BaseSkillBonus * 4) + (CollectiveBonus * 3));                       // 8 - 32 (50)
                m_Rounds = 5 + (int)((BaseSkillBonus * .75) + (CollectiveBonus / 2));                       // 5 - 11 (14)

                // ~1_HCI~% Hit Chance.<br>~2_SDI~% Spell Damage.<br>Damage taken has a ~3_EXP~% chance to cause additional burst of physical damage.<br>
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.TribulationTarget, 1115740, 1115742, string.Format("{0}\t{1}\t{2}", m_PropertyBonus, m_PropertyBonus, (int)m_DamageChance)));

                // Target: ~1_val~ <br> Damage Factor: ~2_val~% <br> Damage Chance: ~3_val~%
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.TribulationCaster, 1115740, 1151388, string.Format("{0}\t{1}\t{2}", m.Name, m_DamageFactor, (int)m_DamageChance)));

                BeginTimer();
            }

            FinishSequence();
        }

        public override bool OnTick()
        {
            if (Target != null && Target.Alive && Target.Map != null)
                Target.FixedEffect(0x376A, 1, 32);

            if (m_Rounds-- <= 0)
            {
                Expire();
                return false;
            }

            return base.OnTick();
        }

        public override void EndEffects()
        {
            BuffInfo.RemoveBuff(Target, BuffIcon.TribulationTarget);
            BuffInfo.RemoveBuff(Caster, BuffIcon.TribulationCaster);
        }

        public override void OnTargetDamaged(Mobile attacker, Mobile victim, DamageType type, ref int damageTaken)
        {
            if (m_NextDamage > DateTime.UtcNow)
                return;

            if (m_DamageChance / 100 > Utility.RandomDouble())
            {
                m_NextDamage = DateTime.UtcNow + TimeSpan.FromSeconds(1);

                int damage = AOS.Scale(damageTaken, m_DamageFactor);
                damage = (int)(damage * GetSlayerBonus()); // 1.5 slayer bonus
                damage -= (int)(damage * DamageModifier(Target)); // resist modifier

                AOS.Damage(victim, Caster, damage, 0, 0, 0, 0, 0, 0, 100);
                victim.FixedParticles(0x374A, 10, 15, 5038, 1181, 0, EffectLayer.Head);
            }
        }

        public override int PropertyBonus()
        {
            return m_PropertyBonus;
        }

        private class InternalTarget : Target
        {
            private readonly TribulationSpell m_Owner;

            public InternalTarget(TribulationSpell spell) : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = spell;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.OnTarget((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
