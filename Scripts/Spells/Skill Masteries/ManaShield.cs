using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Spells.Spellweaving;

namespace Server.Spells.SkillMasteries
{
    public class ManaShieldSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Mana Shield", "Faerkulggen",
                204,
                9061
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 0; } }
        public override int RequiredMana { get { return 40; } }
        public override bool PartyEffects { get { return false; } }

        public override SkillName CastSkill { get { return SkillName.Spellweaving; } }
        public override SkillName DamageSkill { get { return SkillName.Meditation; } }

        public double Chance { get; set; }

        public ManaShieldSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);
        }

        public override bool CheckCast()
        {
            if (Caster is PlayerMobile && !((PlayerMobile)Caster).Spellweaving)
            {
                Caster.SendLocalizedMessage(1073220); // You must have completed the epic arcanist quest to use this ability.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                SkillMasterySpell spell = GetSpell(Caster, this.GetType());

                if (spell != null)
                    spell.Expire();

                double skill = ((Caster.Skills[CastSkill].Value + ArcanistSpell.GetFocusLevel(Caster) * 20) / 2) + (GetMasteryLevel() * 20) + 20;
                Chance = (skill / 13.0) / 100.0;

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(600);
                BeginTimer();

                Caster.PlaySound(0x20C);
                Caster.FixedParticles(0x3779, 1, 30, 9964, 3, 3, EffectLayer.Waist);

                IEntity from = new Entity(Serial.Zero, new Point3D(Caster.X, Caster.Y, Caster.Z), Caster.Map);
                IEntity to = new Entity(Serial.Zero, new Point3D(Caster.X, Caster.Y, Caster.Z + 50), Caster.Map);
                Effects.SendMovingParticles(from, to, 5494, 1, 0, false, false, 33, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ManaShield, 1155902, 1156056, TimeSpan.FromSeconds(600), Caster, String.Format("{0}\t{1}\t{1}", ((int)(Chance * 100)).ToString(), "50"))); // ~1_CHANCE~% chance to reduce incoming damage by ~2_DAMAGE~%. Costs ~2_DAMAGE~% of original damage in mana.
            }

            FinishSequence();
        }

        public override void EndEffects()
        {
            Caster.SendLocalizedMessage(1156087); // Your Mana Shield has expired.

            BuffInfo.RemoveBuff(Caster, BuffIcon.ManaShield);
        }

        public static void CheckManaShield(Mobile m, ref int damage)
        {
            SkillMasterySpell spell = GetSpell(m, typeof(ManaShieldSpell));

            if (spell is ManaShieldSpell)
            {
                if (((ManaShieldSpell)spell).Chance >= Utility.RandomDouble())
                {
                    int toShield = damage / 2;

                    if (m.Mana >= toShield)
                    {
                        m.Mana -= toShield;
                        damage -= toShield;
                    }
                    else
                    {
                        damage -= m.Mana;
                        m.Mana = 0;
                    }
                }
            }
        }
    }
}