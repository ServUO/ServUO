using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public class OnslaughtSpell : SkillMasteryMove
    {
        public override int BaseMana => 20;
        public override double RequiredSkill => 90.0;

        public override SkillName MoveSkill => SkillName.Swords;
        public override TextDefinition AbilityMessage => new TextDefinition(1156007);  // *You ready an onslaught!*

        public override bool Validate(Mobile from)
        {
            if (!CheckWeapon(from))
            {
                from.SendLocalizedMessage(1156006); // You must have a swordsmanship weapon equipped to use this ability.
                return false;
            }

            return base.Validate(from);
        }

        public override void OnUse(Mobile from)
        {
            from.PlaySound(0x1EC);

            from.FixedEffect(0x3779, 10, 20, 1372, 0);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon != null && !HasOnslaught(attacker, defender))
            {
                ClearCurrentMove(attacker);

                int phys, fire, cold, pois, nrgy, chaos, direct;
                weapon.GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

                int highest = phys;
                int type = 0;

                if (fire > phys)
                {
                    type = 1;
                    highest = fire;
                }

                if (cold > highest)
                {
                    type = 2;
                    highest = cold;
                }

                if (pois > highest)
                {
                    type = 3;
                    highest = pois;
                }

                if (nrgy > highest)
                {
                    type = 4;
                    highest = nrgy;
                }

                ResistanceType resistType = (ResistanceType)type;

                int amount = (int)((attacker.Skills[MoveSkill].Value + attacker.Skills[SkillName.Tactics].Value) / 12);
                int duration = (MasteryInfo.GetMasteryLevel(attacker, MoveSkill) * 2) + 1;

                if (defender is PlayerMobile)
                    amount /= 2;

                ResistanceMod mod = new ResistanceMod(resistType, -amount);
                defender.AddResistanceMod(mod);

                attacker.PrivateOverheadMessage(MessageType.Regular, 1150, 1156008, attacker.NetState);  // You deliver an onslaught of sword strikes!
                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.Onslaught, 1156009, 1156010, TimeSpan.FromSeconds(duration), defender, string.Format("{0}\t{1}", amount.ToString(), resistType.ToString()))); // -~2_VAL~% ~1_RESIST~ Debuff.

                defender.FixedEffect(0x37B9, 10, 5, 632, 0);

                if (_Table == null)
                    _Table = new Dictionary<Mobile, Mobile>();

                _Table[attacker] = defender;

                Timer.DelayCall(TimeSpan.FromSeconds(duration), () =>
                    {
                        defender.RemoveResistanceMod(mod);
                        _Table.Remove(attacker);
                    });
            }
        }

        public override void OnClearMove(Mobile from)
        {
            BuffInfo.RemoveBuff(from, BuffIcon.Onslaught);
        }

        private static Dictionary<Mobile, Mobile> _Table;

        public static bool HasOnslaught(Mobile attacker, Mobile victim)
        {
            return _Table != null && _Table.ContainsKey(attacker) && _Table[attacker] == victim;
        }
    }
}
