using System;
using Server;
using System.Collections.Generic;
using Server.Spells;
using Server.Network;
using Server.Mobiles;

namespace Server.Spells.SkillMasteries
{
    public class WarcrySpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(

            "Warcry", "",
                -1,
                9002
            );

        public override int RequiredMana { get { return 40; } }
        public override SkillName CastSkill { get { return SkillName.Bushido; } }

        private int _DamageMalus;
        private int _Radius;

        public WarcrySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                int skill = (int)(Caster.Skills[CastSkill].Value + GetWeaponSkill() + (GetMasteryLevel() * 40)) / 3;

                _Radius = skill / 40;
                _DamageMalus = (int)((double)skill / 2.4);

                Caster.PublicOverheadMessage(MessageType.Regular, Caster.SpeechHue, false, "Prepare Yourself!");

                if (Caster.Player)
                {
                    Caster.PlaySound(Caster.Female ? 0x338 : 0x44A);
                }
                else if (Caster is BaseCreature)
                {
                    Caster.PlaySound(((BaseCreature)Caster).GetAngerSound());
                }

                TimeSpan d;

                if (Caster.AccessLevel == AccessLevel.Player)
                    d = TimeSpan.FromMinutes(20);
                else
                    d = TimeSpan.FromSeconds(10);

                AddToCooldown(d);

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                BeginTimer();

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Warcry, 1155906, 1156058, TimeSpan.FromSeconds(10), Caster, String.Format("{0}\t{1}", _Radius.ToString(), _DamageMalus.ToString())));
                //Reduces all incoming attack damage from opponents who hear the war cry within ~1_RANGE~ tiles by ~2_val~%.
            }

            FinishSequence();
        }

        //public override void OnGotHit(Mobile attacker, ref int damage)
        public override void OnDamaged(Mobile attacker, Mobile victim, DamageType type, ref int damage)
        {
            if (attacker.InRange(Caster, _Radius))
            {
                damage -= (int)((double)damage * ((double)_DamageMalus / 100.00));

                if (Caster.Player)
                {
                    Caster.PlaySound(attacker.Female ? 0x338 : 0x44A);
                }
                else if (Caster is BaseCreature)
                {
                    Caster.PlaySound(((BaseCreature)Caster).GetAngerSound());
                }

                Caster.FixedEffect(0x3779, 10, 20, 1372, 4);

                //Caster.SendLocalizedMessage(1156161, attacker.Name); // ~1_NAME~ attacks you for 50% damage.
            }
        }
    }
}