using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Spells.SkillMasteries;
using System;

namespace Server.Misc
{
    public class SkillCheck
    {
        private static readonly TimeSpan _StatGainDelay;
        private static readonly TimeSpan _PetStatGainDelay;

        private static readonly int _PlayerChanceToGainStats;
        private static readonly int _PetChanceToGainStats;

        public static bool GGSActive => !Siege.SiegeShard;

        static SkillCheck()
        {
            _PlayerChanceToGainStats = Config.Get("PlayerCaps.PlayerChanceToGainStats", 5);
            _PetChanceToGainStats = Config.Get("PlayerCaps.PetChanceToGainStats", 5);

            _StatGainDelay = TimeSpan.FromSeconds(0.5);
            _PetStatGainDelay = TimeSpan.FromSeconds(0.5);
        }

        public static void Initialize()
        {
            Mobile.SkillCheckLocationHandler = Mobile_SkillCheckLocation;
            Mobile.SkillCheckDirectLocationHandler = Mobile_SkillCheckDirectLocation;

            Mobile.SkillCheckTargetHandler = Mobile_SkillCheckTarget;
            Mobile.SkillCheckDirectTargetHandler = Mobile_SkillCheckDirectTarget;
        }

        public static bool Mobile_SkillCheckLocation(Mobile from, SkillName skillName, double minSkill, double maxSkill)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            //TODO: Is there any other place this can go?
            if (skillName == SkillName.Fishing && BaseGalleon.FindGalleonAt(from, from.Map) is TokunoGalleon)
                value += 1;

            if (value < minSkill)
                return false; // Too difficult

            if (value >= maxSkill)
                return true; // No challenge

            double chance = (value - minSkill) / (maxSkill - minSkill);

            CrystalBallOfKnowledge.TellSkillDifficulty(from, skillName, chance);

            return CheckSkill(from, skill, chance);
        }

        public static bool Mobile_SkillCheckDirectLocation(Mobile from, SkillName skillName, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            CrystalBallOfKnowledge.TellSkillDifficulty(from, skillName, chance);

            if (chance < 0.0)
                return false; // Too difficult

            if (chance >= 1.0)
                return true; // No challenge

            return CheckSkill(from, skill, chance);
        }

        /// <summary>
        /// This should be a successful skill check, where a system can register several skill gains at once. Only system
        /// using this currently is UseAllRes for CraftItem.cs
        /// </summary>
        /// <param name="from"></param>
        /// <param name="maxSkill"></param>
        /// <param name="amount"></param>
        /// <param name="sk"></param>
        /// <param name="minSkill"></param>
        /// <returns></returns>
        public static bool CheckSkill(Mobile from, SkillName sk, double minSkill, double maxSkill, int amount)
        {
            if (from.Skills.Cap == 0)
                return false;

            Skill skill = from.Skills[sk];
            double value = skill.Value;
            int gains = 0;

            for (int i = 0; i < amount; i++)
            {
                double chance = (value - minSkill) / (maxSkill - minSkill);
                double gc = GetGainChance(from, skill, chance, Utility.Random(100) <= (int)(chance * 100)) / (value / 4);

                if (AllowGain(from, skill))
                {
                    if (from.Alive && (skill.Base + (value - skill.Value) < 10.0 || Utility.RandomDouble() <= gc || CheckGGS(from, skill)))
                    {
                        gains++;
                        value += 0.1;

                        UpdateGGS(from, skill);
                    }
                }

            }

            if (gains > 0)
            {
                Gain(from, skill, gains);
                EventSink.InvokeSkillCheck(new SkillCheckEventArgs(from, skill, true));
                return true;
            }

            return false;
        }

        public static bool CheckSkill(Mobile from, Skill skill, double chance)
        {
            if (from.Skills.Cap == 0)
                return false;

            bool success = Utility.Random(100) <= (int)(chance * 100);
            double gc = GetGainChance(from, skill, chance, success);

            if (AllowGain(from, skill))
            {
                if (from.Alive && (skill.Base < 10.0 || Utility.RandomDouble() <= gc || CheckGGS(from, skill)))
                {
                    Gain(from, skill);
                }
            }

            EventSink.InvokeSkillCheck(new SkillCheckEventArgs(from, skill, success));

            return success;
        }

        private static double GetGainChance(Mobile from, Skill skill, double chance, bool success)
        {
            double gc = (double)(from.Skills.Cap - from.Skills.Total) / from.Skills.Cap;

            gc += (skill.Cap - skill.Base) / skill.Cap;
            gc /= 2;

            gc += (1.0 - chance) * (success ? 0.5 : 0.0);
            gc /= 2;

            gc *= skill.Info.GainFactor;

            if (gc < 0.01)
                gc = 0.01;

            // Pets get a 100% bonus
            if (from is BaseCreature bc && bc.Controlled)
                gc += gc * 1.00;

            if (gc > 1.00)
                gc = 1.00;

            return gc;
        }

        public static bool Mobile_SkillCheckTarget(
            Mobile from,
            SkillName skillName,
            object target,
            double minSkill,
            double maxSkill)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < minSkill)
                return false; // Too difficult

            if (value >= maxSkill)
                return true; // No challenge

            double chance = (value - minSkill) / (maxSkill - minSkill);

            CrystalBallOfKnowledge.TellSkillDifficulty(from, skillName, chance);

            return CheckSkill(from, skill, chance);
        }

        public static bool Mobile_SkillCheckDirectTarget(Mobile from, SkillName skillName, object target, double chance)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return false;

            CrystalBallOfKnowledge.TellSkillDifficulty(from, skillName, chance);

            if (chance < 0.0)
                return false; // Too difficult

            if (chance >= 1.0)
                return true; // No challenge

            return CheckSkill(from, skill, chance);
        }

        private static bool AllowGain(Mobile from, Skill skill)
        {
            if (Engines.VvV.ViceVsVirtueSystem.InSkillLoss(from))
                return false;

            if (from is PlayerMobile mobile)
            {
                if (skill.Info.SkillID == (int)SkillName.Archery && mobile.Race == Race.Gargoyle)
                {
                    return false;
                }

                if (skill.Info.SkillID == (int)SkillName.Throwing && mobile.Race != Race.Gargoyle)
                {
                    return false;
                }
            }

            return true;
        }

        public enum Stat
        {
            Str,
            Dex,
            Int
        }

        public static void Gain(Mobile from, Skill skill)
        {
            Gain(from, skill, (int)(from.Region.SkillGain(from) * 10));
        }

        public static void Gain(Mobile from, Skill skill, int toGain)
        {
            if (from.Region.IsPartOf<Jail>())
                return;

            if (from is BaseCreature creature && creature.IsDeadPet)
                return;

            if (skill.SkillName == SkillName.Focus && from is BaseCreature baseCreature && !baseCreature.Controlled)
                return;

            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                Skills skills = from.Skills;

                if (from is PlayerMobile mobile && Siege.SiegeShard)
                {
                    int minsPerGain = Siege.MinutesPerGain(mobile, skill);

                    if (minsPerGain > 0)
                    {
                        if (Siege.CheckSkillGain(mobile, minsPerGain, skill))
                        {
                            CheckReduceSkill(skills, toGain, skill);

                            if (skills.Total + toGain <= skills.Cap)
                            {
                                skill.BaseFixedPoint += toGain;
                            }
                        }

                        return;
                    }
                }

                if (toGain == 1 && skill.Base <= 10.0)
                {
                    toGain = Utility.Random(4) + 1;
                }

                if (from is PlayerMobile playerMobile && QuestHelper.EnhancedSkill(playerMobile, skill))
                {
                    toGain *= Utility.RandomMinMax(2, 4);
                }

                #region Scroll of Alacrity
                if (from is PlayerMobile pm && skill.SkillName == pm.AcceleratedSkill && pm.AcceleratedStart > DateTime.UtcNow)
                {
                    pm.SendLocalizedMessage(1077956); // You are infused with intense energy. You are under the effects of an accelerated skill gain scroll.

                    toGain = Utility.RandomMinMax(2, 5);
                }
                #endregion

                #region Skill Masteries
                else if (from is BaseCreature bc && !(bc is Engines.Despise.DespiseCreature) && (bc.Controlled || bc.Summoned))
                {
                    Mobile master = bc.GetMaster();

                    if (master != null)
                    {
                        WhisperingSpell spell = SkillMasterySpell.GetSpell(master, typeof(WhisperingSpell)) as WhisperingSpell;

                        if (spell != null && master.InRange(bc.Location, spell.PartyRange) && master.Map == bc.Map && spell.EnhancedGainChance >= Utility.Random(100))
                        {
                            toGain = Utility.RandomMinMax(2, 5);
                        }
                    }
                }
                #endregion

                if (from is PlayerMobile)
                {
                    CheckReduceSkill(skills, toGain, skill);
                }

                if (!from.Player || (skills.Total + toGain <= skills.Cap))
                {
                    skill.BaseFixedPoint = Math.Min(skill.CapFixedPoint, skill.BaseFixedPoint + toGain);

                    EventSink.InvokeSkillGain(new SkillGainEventArgs(from, skill, toGain));

                    if (from is PlayerMobile)
                    {
                        UpdateGGS(from, skill);
                    }
                }
            }

            if (from is PlayerMobile mobileFrom)
            {
                QuestHelper.CheckSkill(mobileFrom, skill);
            }

            if (skill.Lock == SkillLock.Up && (!Siege.SiegeShard || !(from is PlayerMobile) || Siege.CanGainStat((PlayerMobile)from)))
            {
                TryStatGain(skill.Info, from);
            }
        }

        private static void CheckReduceSkill(Skills skills, int toGain, Skill gainSKill)
        {
            if (skills.Total / skills.Cap >= Utility.RandomDouble())
            {
                foreach (Skill toLower in skills)
                {
                    if (toLower != gainSKill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain)
                    {
                        toLower.BaseFixedPoint -= toGain;
                        break;
                    }
                }
            }
        }

        public static void TryStatGain(SkillInfo info, Mobile from)
        {
            // Chance roll
            double chance;

            if (from is BaseCreature creature && creature.Controlled)
            {
                chance = _PetChanceToGainStats / 100.0;
            }
            else
            {
                chance = _PlayerChanceToGainStats / 100.0;
            }

            if (Utility.RandomDouble() >= chance)
            {
                return;
            }

            // Selection
            StatLockType primaryLock = StatLockType.Locked;
            StatLockType secondaryLock = StatLockType.Locked;

            switch (info.Primary)
            {
                case StatCode.Str:
                    primaryLock = from.StrLock;
                    break;
                case StatCode.Dex:
                    primaryLock = from.DexLock;
                    break;
                case StatCode.Int:
                    primaryLock = from.IntLock;
                    break;
            }

            switch (info.Secondary)
            {
                case StatCode.Str:
                    secondaryLock = from.StrLock;
                    break;
                case StatCode.Dex:
                    secondaryLock = from.DexLock;
                    break;
                case StatCode.Int:
                    secondaryLock = from.IntLock;
                    break;
            }

            // Gain
            // Decision block of both are selected to gain
            if (primaryLock == StatLockType.Up && secondaryLock == StatLockType.Up)
            {
                if (Utility.Random(4) == 0)
                    GainStat(from, (Stat)info.Secondary);
                else
                    GainStat(from, (Stat)info.Primary);
            }
            else // Will not do anything if neither are selected to gain
            {
                if (primaryLock == StatLockType.Up)
                    GainStat(from, (Stat)info.Primary);
                else if (secondaryLock == StatLockType.Up)
                    GainStat(from, (Stat)info.Secondary);
            }
        }

        public static bool CanLower(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str:
                    return from.StrLock == StatLockType.Down && from.RawStr > 10;

                case Stat.Dex:
                    return from.DexLock == StatLockType.Down && from.RawDex > 10;

                case Stat.Int:
                    return from.IntLock == StatLockType.Down && from.RawInt > 10;
            }

            return false;
        }

        public static bool CanRaise(Mobile from, Stat stat, bool atTotalCap)
        {
            switch (stat)
            {
                case Stat.Str:
                    if (from.RawStr < from.StrCap)
                    {
                        if (atTotalCap && from is PlayerMobile)
                        {
                            return CanLower(from, Stat.Dex) || CanLower(from, Stat.Int);
                        }

                        return true;
                    }

                    return false;

                case Stat.Dex:
                    if (from.RawDex < from.DexCap)
                    {
                        if (atTotalCap && from is PlayerMobile)
                        {
                            return CanLower(from, Stat.Str) || CanLower(from, Stat.Int);
                        }

                        return true;
                    }

                    return false;

                case Stat.Int:
                    if (from.RawInt < from.IntCap)
                    {
                        if (atTotalCap && from is PlayerMobile)
                        {
                            return CanLower(from, Stat.Str) || CanLower(from, Stat.Dex);
                        }

                        return true;
                    }

                    return false;
            }

            return false;
        }

        public static void IncreaseStat(Mobile from, Stat stat)
        {
            bool atTotalCap = from.RawStatTotal >= from.StatCap;

            switch (stat)
            {
                case Stat.Str:
                    {
                        if (CanRaise(from, Stat.Str, atTotalCap))
                        {
                            if (atTotalCap)
                            {
                                if (CanLower(from, Stat.Dex) && (from.RawDex < from.RawInt || !CanLower(from, Stat.Int)))
                                    --from.RawDex;
                                else if (CanLower(from, Stat.Int))
                                    --from.RawInt;
                            }

                            ++from.RawStr;

                            if (from is BaseCreature creature && creature.HitsMaxSeed > -1 && creature.HitsMaxSeed < creature.StrCap)
                            {
                                creature.HitsMaxSeed++;
                            }

                            if (Siege.SiegeShard && from is PlayerMobile mobile)
                            {
                                Siege.IncreaseStat(mobile);
                            }
                        }

                        break;
                    }
                case Stat.Dex:
                    {
                        if (CanRaise(from, Stat.Dex, atTotalCap))
                        {
                            if (atTotalCap)
                            {
                                if (CanLower(from, Stat.Str) && (from.RawStr < from.RawInt || !CanLower(from, Stat.Int)))
                                    --from.RawStr;
                                else if (CanLower(from, Stat.Int))
                                    --from.RawInt;
                            }

                            ++from.RawDex;

                            if (from is BaseCreature creature && creature.StamMaxSeed > -1 && creature.StamMaxSeed < creature.DexCap)
                            {
                                creature.StamMaxSeed++;
                            }

                            if (Siege.SiegeShard && from is PlayerMobile mobile)
                            {
                                Siege.IncreaseStat(mobile);
                            }
                        }

                        break;
                    }
                case Stat.Int:
                    {
                        if (CanRaise(from, Stat.Int, atTotalCap))
                        {
                            if (atTotalCap)
                            {
                                if (CanLower(from, Stat.Str) && (from.RawStr < from.RawDex || !CanLower(from, Stat.Dex)))
                                    --from.RawStr;
                                else if (CanLower(from, Stat.Dex))
                                    --from.RawDex;
                            }

                            ++from.RawInt;

                            if (from is BaseCreature creature && creature.ManaMaxSeed > -1 && creature.ManaMaxSeed < creature.IntCap)
                            {
                                creature.ManaMaxSeed++;
                            }

                            if (Siege.SiegeShard && from is PlayerMobile mobile)
                            {
                                Siege.IncreaseStat(mobile);
                            }
                        }

                        break;
                    }
            }
        }

        public static void GainStat(Mobile from, Stat stat)
        {
            if (!CheckStatTimer(from, stat))
                return;

            IncreaseStat(from, stat);
        }

        public static bool CheckStatTimer(Mobile from, Stat stat)
        {
            switch (stat)
            {
                case Stat.Str:
                    {
                        if (from is BaseCreature creature && creature.Controlled)
                        {
                            if (creature.LastStrGain + _PetStatGainDelay >= DateTime.UtcNow)
                                return false;
                        }
                        else if (from.LastStrGain + _StatGainDelay >= DateTime.UtcNow)
                            return false;

                        from.LastStrGain = DateTime.UtcNow;
                        break;
                    }
                case Stat.Dex:
                    {
                        if (from is BaseCreature creature && creature.Controlled)
                        {
                            if (creature.LastDexGain + _PetStatGainDelay >= DateTime.UtcNow)
                                return false;
                        }
                        else if (from.LastDexGain + _StatGainDelay >= DateTime.UtcNow)
                            return false;

                        from.LastDexGain = DateTime.UtcNow;
                        break;
                    }
                case Stat.Int:
                    {
                        if (from is BaseCreature creature && creature.Controlled)
                        {
                            if (creature.LastIntGain + _PetStatGainDelay >= DateTime.UtcNow)
                                return false;
                        }
                        else if (from.LastIntGain + _StatGainDelay >= DateTime.UtcNow)
                            return false;

                        from.LastIntGain = DateTime.UtcNow;
                        break;
                    }
            }
            return true;
        }

        private static bool CheckGGS(Mobile from, Skill skill)
        {
            if (!GGSActive)
                return false;

            if (from is PlayerMobile && skill.NextGGSGain < DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        public static void UpdateGGS(Mobile from, Skill skill)
        {
            if (!GGSActive)
                return;

            int list = (int)Math.Min(GGSTable.Length - 1, skill.Base / 5);
            int column = from.Skills.Total >= 7000 ? 2 : from.Skills.Total >= 3500 ? 1 : 0;

            skill.NextGGSGain = DateTime.UtcNow + TimeSpan.FromMinutes(GGSTable[list][column]);
        }

        private static readonly int[][] GGSTable =
        {
            new[] {1, 3, 5}, // 0.0 - 4.9
			new[] {4, 10, 18}, new[] {7, 17, 30}, new[] {9, 24, 44}, new[] {12, 31, 57}, new[] {14, 38, 90}, new[] {17, 45, 84},
            new[] {20, 52, 96}, new[] {23, 60, 106}, new[] {25, 66, 120}, new[] {27, 72, 138}, new[] {33, 90, 162},
            new[] {55, 150, 264}, new[] {78, 216, 390}, new[] {114, 294, 540}, new[] {144, 384, 708}, new[] {180, 492, 900},
            new[] {228, 606, 1116}, new[] {276, 744, 1356}, new[] {336, 894, 1620}, new[] {396, 1056, 1920},
            new[] {468, 1242, 2280}, new[] {540, 1440, 2580}, new[] {618, 1662, 3060}
        };
    }
}
