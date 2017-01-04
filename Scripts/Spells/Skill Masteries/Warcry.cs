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
                int duration = 120;

                if (skill >= 120)
                    duration = 60;
                else if (skill >= 100)
                    duration = 80;

                Caster.PrivateOverheadMessage(MessageType.Regular, 1150, false, "Prepare Yourself!", Caster.NetState);
                Caster.PlaySound(Caster.Female ? 0x338 : 0x44A);
                TimeSpan d;

                if (Caster.AccessLevel == AccessLevel.Player)
                    d = TimeSpan.FromMinutes(duration);
                else
                    d = TimeSpan.FromSeconds(10);

                AddToCooldown(d);

                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                BeginTimer();
            }

            FinishSequence();
        }

        public override void OnGotHit(Mobile attacker, ref int damage)
        {
            if (attacker.InRange(Caster, _Radius))
            {
                damage -= (int)((double)damage * ((double)_DamageMalus / 100.00));

                Caster.PlaySound(attacker.Female ? 0x338 : 0x44A);
                Caster.FixedEffect(0x3779, 10, 20, 1372, 0);

                Caster.SendLocalizedMessage(1156161, attacker.Name); // ~1_NAME~ attacks you for 50% damage.
            }
        }
    }
}