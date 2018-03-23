using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public class StaggerSpell : SkillMasteryMove
    {
        public override int BaseMana { get { return 20; } }
        public override double RequiredSkill { get { return 90.0; } }

        public override SkillName MoveSkill { get { return SkillName.Macing; } }
        public override TextDefinition AbilityMessage { get { return new TextDefinition(1155980); } } // *You ready yourself to stagger your opponent!*
        public override TimeSpan CooldownPeriod { get { return TimeSpan.FromSeconds(2); } }

        private static Dictionary<Mobile, int> _Table;

        public StaggerSpell()
        {
        }

        public override bool Validate(Mobile from)
        {
            if (!CheckWeapon(from))
            {
                from.SendLocalizedMessage(1155983); // You must have a mace weapon equipped to use this ability!
                return false;
            }

            return base.Validate(from);
        }

        public override void OnUse(Mobile from)
        {
            if (from.Player)
            {
                from.PlaySound(from.Female ? 0x338 : 0x44A);
            }
            else if (from is BaseCreature)
            {
                from.PlaySound(((BaseCreature)from).GetAngerSound());
            }

            from.FixedParticles(0x373A, 10, 15, 5018, 2719, 0, EffectLayer.Waist);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentMove(attacker);

            attacker.PrivateOverheadMessage(MessageType.Regular, 1150, 1155984, attacker.NetState);

            defender.FixedEffect(0x3779, 20, 10, 2719, 0);

            double skills = (attacker.Skills[MoveSkill].Value + attacker.Skills[SkillName.Tactics].Value + (MasteryInfo.GetMasteryLevel(attacker, MoveSkill) * 40)) / 3;

            AddToTable(defender, (int)(skills / 2));
            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.Stagger, 1155981, 1155982, TimeSpan.FromSeconds(10), defender, ((int)skills / 2).ToString()));

            defender.Delta(MobileDelta.WeaponDamage);

            AddToCooldown(attacker);
        }

        public override double GetDamageScalar(Mobile attacker, Mobile defender)
        {
            return defender is PlayerMobile ? 1.25 : 1.5;
        }

        public static void AddToTable(Mobile defender, int amount)
        {
            if (_Table != null && _Table.ContainsKey(defender))
                return;

            if (_Table == null)
                _Table = new Dictionary<Mobile, int>();

            _Table[defender] = amount;

            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                {
                    _Table.Remove(defender);
                });
        }

        public static int GetStagger(Mobile from)
        {
            if (_Table != null && _Table.ContainsKey(from))
            {
                return _Table[from];
            }

            return 0;
        }
    }
}