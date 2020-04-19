using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Linq;

/*The animal tamer attempts to guide their pet on the path of skill gain, increasing the pet's skill gains based on the tamer's 
  animal taming skill, animal lore skill, and mastery level.  This ability functions similarly to a scroll of alacrity.*/

namespace Server.Spells.SkillMasteries
{
    public class WhisperingSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Whispering", "",
                -1,
                9002
            );

        public override int RequiredMana => 40;

        public override SkillName CastSkill => SkillName.AnimalTaming;
        public override SkillName DamageSkill => SkillName.AnimalLore;
        public override bool RevealOnTick => false;

        private int _EnhancedGainChance;
        public int EnhancedGainChance => _EnhancedGainChance;

        public WhisperingSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (IsInCooldown(Caster, GetType()))
                return false;

            if (GetSpell(Caster, GetType()) != null) // does this expire or not let you cast it again?>
            {
                Caster.SendLocalizedMessage(1155889); // You are already under the effect of this ability.
                return false;
            }

            if (Caster is PlayerMobile && ((PlayerMobile)Caster).AllFollowers == null || ((PlayerMobile)Caster).AllFollowers.Where(m => !(m is Engines.Despise.DespiseCreature)).Count() == 0)
            {
                Caster.SendLocalizedMessage(1156112); // This ability requires you to have pets.
                return false;
            }

            return base.CheckCast();
        }

        public override void SendCastEffect()
        {
            base.SendCastEffect();

            Caster.PrivateOverheadMessage(MessageType.Regular, 0x35, false, "You guide your pet's behaviors, enhancing its skill gains!", Caster.NetState);
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                if (Caster is PlayerMobile)
                {
                    foreach (Mobile m in ((PlayerMobile)Caster).AllFollowers.Where(m => m.Map == Caster.Map && Caster.InRange(m.Location, PartyRange) && !(m is Engines.Despise.DespiseCreature)))
                    {
                        Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                        Effects.PlaySound(m.Location, m.Map, 0x243);

                        Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(m.X - 6, m.Y - 6, m.Z + 15), m.Map), m, 0x36D4, 7, 0, false, true, 1494, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                        Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(m.X - 4, m.Y - 6, m.Z + 15), m.Map), m, 0x36D4, 7, 0, false, true, 1494, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                        Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(m.X - 6, m.Y - 4, m.Z + 15), m.Map), m, 0x36D4, 7, 0, false, true, 1494, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                        Effects.SendTargetParticles(m, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
                    }
                }

                Caster.SendSound(0x64E);

                TimeSpan duration = TimeSpan.FromSeconds(600);
                Expires = DateTime.UtcNow + duration;
                BeginTimer();

                _EnhancedGainChance = (int)(BaseSkillBonus / 1.26);

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Whispering, 1155932, 1156106, duration, Caster, _EnhancedGainChance.ToString()));

                AddToCooldown(TimeSpan.FromMinutes(30));
            }

            FinishSequence();
        }
    }
}
