using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Raises your defenses for a short time. Requires Bushido or Ninjitsu skill.
    /// </summary>
    /// 
    // spell/melee
    // 0 parry - 70/80
    // 100 parry - 40/65
    // 120 parry - 20/55
    // .6875

    public class Block : WeaponAbility
    {
        private static Dictionary<Mobile, BlockInfo> _Table;

        public override int BaseMana => 20;

        public override int AccuracyBonus => -15;

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
        }

        public static bool IsBlocking(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }

        public static int GetBonus(Mobile targ)
        {
            if (targ == null || _Table == null)
                return 0;

            if (_Table.ContainsKey(targ))
                return _Table[targ]._DCIBonus;

            return 0;
        }

        public static int GetSpellReduction(Mobile m)
        {
            if (m == null || _Table == null)
                return 0;

            if (_Table.ContainsKey(m))
            {
                return _Table[m]._SpellReduction;
            }

            return 0;
        }

        public static int GetMeleeReduction(Mobile m)
        {
            if (m == null || _Table == null)
                return 0;

            if (_Table.ContainsKey(m))
            {
                return _Table[m]._MeleeReduction;
            }

            return 0;
        }

        public static void BeginBlock(Mobile m, int dciBonus, int spellblock, int meleeblock)
        {
            EndBlock(m);

            if (_Table == null)
                _Table = new Dictionary<Mobile, BlockInfo>();

            BlockInfo info = new BlockInfo(dciBonus, spellblock, meleeblock);
            _Table[m] = info;

            string args = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", dciBonus, spellblock, meleeblock, "15", "30");

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Block, 1151291, 1151292, TimeSpan.FromSeconds(6), m, args));
            // Next incoming damage reduced.<br>Defense Chance Increase: +~1_val~%<br>Incoming Spell Damage: -~2_val~%<br>Incoming Attack Damage: -~3_val~%<br>Hit Chance Penalty: ~4_val~%<br>Damage Penalty: ~5_val~%

            Timer.DelayCall(TimeSpan.FromSeconds(6), () =>
            {
                if (IsBlocking(m))
                    EndBlock(m);
            });
        }

        public static void EndBlock(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);

                BuffInfo.RemoveBuff(m, BuffIcon.Block);

                m.SendLocalizedMessage(1150286); // You no longer try to block the next attack.

                if (_Table.Count == 0)
                    _Table = null;
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063345); // You block an attack!
            defender.SendLocalizedMessage(1063346); // Your attack was blocked!

            attacker.FixedParticles(0x37C4, 1, 16, 0x251D, 0x39D, 0x3, EffectLayer.RightHand);

            int parry = (int)attacker.Skills[SkillName.Parry].Value;

            bool creature = attacker is BaseCreature;
            double skill = creature ? attacker.Skills[SkillName.Bushido].Value :
                                      Math.Max(attacker.Skills[SkillName.Bushido].Value, attacker.Skills[SkillName.Ninjitsu].Value);

            int dcibonus = (int)(10.0 * ((skill - 50.0) / 70.0 + 5));
            int spellblock = parry <= 70 ? 70 : parry <= 100 ? 40 : 20;
            int meleeblock = parry <= 70 ? 80 : parry <= 100 ? 65 : 55;

            BeginBlock(attacker, dcibonus, spellblock, meleeblock);

            if (creature)
                PetTrainingHelper.OnWeaponAbilityUsed((BaseCreature)attacker, SkillName.Bushido);
        }

        private class BlockInfo
        {
            public readonly int _DCIBonus;
            public readonly int _SpellReduction;
            public readonly int _MeleeReduction;

            public BlockInfo(int bonus, int spellred, int meleered)
            {
                _DCIBonus = bonus;
                _SpellReduction = spellred;
                _MeleeReduction = meleered;
            }
        }
    }
}
