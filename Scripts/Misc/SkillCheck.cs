#region References
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Regions;
using Server.Spells.SkillMasteries;
using System;
#endregion

namespace Server.Misc
{
    public class SkillCheck
    {
        private static readonly TimeSpan _StatGainDelay;
        private static readonly TimeSpan _PetStatGainDelay;

        private static readonly int _PlayerChanceToGainStats;
        private static readonly int _PetChanceToGainStats;

        private static readonly bool _AntiMacroCode;

        /// <summary>
        ///     How long do we remember targets/locations?
        /// </summary>
        public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes(5.0);

        /// <summary>
        ///     How many times may we use the same location/target for gain
        /// </summary>
        public const int Allowance = 3;

        /// <summary>
        ///     The size of each location, make this smaller so players dont have to move as far
        /// </summary>
        private const int LocationSize = 4;

        public static bool GGSActive => !Siege.SiegeShard;

        static SkillCheck()
        {
            _AntiMacroCode = Config.Get("PlayerCaps.EnableAntiMacro", false);

            _StatGainDelay = Config.Get("PlayerCaps.PlayerStatTimeDelay", TimeSpan.FromMinutes(15.0));
            _PetStatGainDelay = Config.Get("PlayerCaps.PetStatTimeDelay", TimeSpan.FromMinutes(5.0));

            _PlayerChanceToGainStats = Config.Get("PlayerCaps.PlayerChanceToGainStats", 5);
            _PetChanceToGainStats = Config.Get("PlayerCaps.PetChanceToGainStats", 5);

            if (!Config.Get("PlayerCaps.EnablePlayerStatTimeDelay", false))
                _StatGainDelay = TimeSpan.FromSeconds(0.5);

            if (!Config.Get("PlayerCaps.EnablePetStatTimeDelay", false))
                _PetStatGainDelay = TimeSpan.FromSeconds(0.5);
        }

        private static readonly bool[] UseAntiMacro =
        {
			// true if this skill uses the anti-macro code, false if it does not
			false, 	// Alchemy = 0,
			true, 	// Anatomy = 1,
			true, 	// AnimalLore = 2,
			true, 	// ItemID = 3,
			true, 	// ArmsLore = 4,
			false, 	// Parry = 5,
			true, 	// Begging = 6,
			false, 	// Blacksmith = 7,
			false, 	// Fletching = 8,
			true, 	// Peacemaking = 9,
			true, 	// Camping = 10,
			false, 	// Carpentry = 11,
			false, 	// Cartography = 12,
			false, 	// Cooking = 13,
			true, 	// DetectHidden = 14,
			true, 	// Discordance = 15,
			true, 	// EvalInt = 16,
			true, 	// Healing = 17,
			true, 	// Fishing = 18,
			true, 	// Forensics = 19,
			true, 	// Herding = 20,
			true, 	// Hiding = 21,
			true, 	// Provocation = 22,
			false, 	// Inscribe = 23,
			true, 	// Lockpicking = 24,
			true, 	// Magery = 25,
			true, 	// MagicResist = 26,
			false, 	// Tactics = 27,
			true, 	// Snooping = 28,
			true, 	// Musicianship = 29,
			true, 	// Poisoning = 30,
			false, 	// Archery = 31,
			true, 	// SpiritSpeak = 32,
			true, 	// Stealing = 33,
			false, 	// Tailoring = 34,
			true, 	// AnimalTaming = 35,
			true, 	// TasteID = 36,
			false, 	// Tinkering = 37,
			true, 	// Tracking = 38,
			true, 	// Veterinary = 39,
			false, 	// Swords = 40,
			false, 	// Macing = 41,
			false, 	// Fencing = 42,
			false, 	// Wrestling = 43,
			true, 	// Lumberjacking = 44,
			true, 	// Mining = 45,
			true, 	// Meditation = 46,
			true, 	// Stealth = 47,
			true, 	// RemoveTrap = 48,
			true, 	// Necromancy = 49,
			false, 	// Focus = 50,
			true, 	// Chivalry = 51
			true, 	// Bushido = 52
			true, 	// Ninjitsu = 53
			true, 	// Spellweaving = 54
            true, 	// Mysticism = 55
			true, 	// Imbuing = 56
			false  // Throwing = 57
        };

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

            return CheckSkill(from, skill, new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize), chance);
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

            return CheckSkill(from, skill, new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize), chance);
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

                if (AllowGain(from, skill, new Point2D(from.Location.X / LocationSize, from.Location.Y / LocationSize)))
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

        public static bool CheckSkill(Mobile from, Skill skill, object obj, double chance)
        {
            if (from.Skills.Cap == 0)
                return false;

            bool success = Utility.Random(100) <= (int)(chance * 100);
            double gc = GetGainChance(from, skill, chance, success);

            if (AllowGain(from, skill, obj))
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
            if (from is BaseCreature && ((BaseCreature)from).Controlled)
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

            return CheckSkill(from, skill, target, chance);
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

            return CheckSkill(from, skill, target, chance);
        }

        private static bool AllowGain(Mobile from, Skill skill, object obj)
        {
            if (Engines.VvV.ViceVsVirtueSystem.InSkillLoss(from)) //Changed some time between the introduction of AoS and SE.
                return false;

            if (from is PlayerMobile)
            {
                if (skill.Info.SkillID == (int)SkillName.Archery && from.Race == Race.Gargoyle)
                    return false;

                if (skill.Info.SkillID == (int)SkillName.Throwing && from.Race != Race.Gargoyle)
                    return false;

                if (_AntiMacroCode && UseAntiMacro[skill.Info.SkillID])
                    return ((PlayerMobile)from).AntiMacroCheck(skill, obj);
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

            if (from is BaseCreature && ((BaseCreature)from).IsDeadPet)
                return;

            if (skill.SkillName == SkillName.Focus && from is BaseCreature && !((BaseCreature)from).Controlled)
                return;

            if (skill.Base < skill.Cap && skill.Lock == SkillLock.Up)
            {
                Skills skills = from.Skills;

                if (from is PlayerMobile && Siege.SiegeShard)
                {
                    int minsPerGain = Siege.MinutesPerGain(from, skill);

                    if (minsPerGain > 0)
                    {
                        if (Siege.CheckSkillGain((PlayerMobile)from, minsPerGain, skill))
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
                    toGain = Utility.Random(4) + 1;

                #region Mondain's Legacy
                if (from is PlayerMobile && QuestHelper.EnhancedSkill((PlayerMobile)from, skill))
                {
                    toGain *= Utility.RandomMinMax(2, 4);
                }
                #endregion

                #region Scroll of Alacrity
                if (from is PlayerMobile && skill.SkillName == ((PlayerMobile)from).AcceleratedSkill &&
                    ((PlayerMobile)from).AcceleratedStart > DateTime.UtcNow)
                {
                    // You are infused with intense energy. You are under the effects of an accelerated skillgain scroll.
                    ((PlayerMobile)from).SendLocalizedMessage(1077956);

                    toGain = Utility.RandomMinMax(2, 5);
                }
                #endregion

                #region Skill Masteries
                else if (from is BaseCreature && !(from is Engines.Despise.DespiseCreature) && (((BaseCreature)from).Controlled || ((BaseCreature)from).Summoned))
                {
                    Mobile master = ((BaseCreature)from).GetMaster();

                    if (master != null)
                    {
                        WhisperingSpell spell = SkillMasterySpell.GetSpell(master, typeof(WhisperingSpell)) as WhisperingSpell;

                        if (spell != null && master.InRange(from.Location, spell.PartyRange) && master.Map == from.Map &&
                            spell.EnhancedGainChance >= Utility.Random(100))
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

            if (from is PlayerMobile)
            {
                QuestHelper.CheckSkill((PlayerMobile)from, skill);
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

            if (from is BaseCreature && ((BaseCreature)from).Controlled)
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

                            if (from is BaseCreature && ((BaseCreature)from).HitsMaxSeed > -1 && ((BaseCreature)from).HitsMaxSeed < from.StrCap)
                            {
                                ((BaseCreature)from).HitsMaxSeed++;
                            }

                            if (Siege.SiegeShard && from is PlayerMobile)
                            {
                                Siege.IncreaseStat((PlayerMobile)from);
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

                            if (from is BaseCreature && ((BaseCreature)from).StamMaxSeed > -1 && ((BaseCreature)from).StamMaxSeed < from.DexCap)
                            {
                                ((BaseCreature)from).StamMaxSeed++;
                            }

                            if (Siege.SiegeShard && from is PlayerMobile)
                            {
                                Siege.IncreaseStat((PlayerMobile)from);
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

                            if (from is BaseCreature && ((BaseCreature)from).ManaMaxSeed > -1 && ((BaseCreature)from).ManaMaxSeed < from.IntCap)
                            {
                                ((BaseCreature)from).ManaMaxSeed++;
                            }

                            if (Siege.SiegeShard && from is PlayerMobile)
                            {
                                Siege.IncreaseStat((PlayerMobile)from);
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
                        if (from is BaseCreature && ((BaseCreature)from).Controlled)
                        {
                            if (from.LastStrGain + _PetStatGainDelay >= DateTime.UtcNow)
                                return false;
                        }
                        else if (from.LastStrGain + _StatGainDelay >= DateTime.UtcNow)
                            return false;

                        from.LastStrGain = DateTime.UtcNow;
                        break;
                    }
                case Stat.Dex:
                    {
                        if (from is BaseCreature && ((BaseCreature)from).Controlled)
                        {
                            if (from.LastDexGain + _PetStatGainDelay >= DateTime.UtcNow)
                                return false;
                        }
                        else if (from.LastDexGain + _StatGainDelay >= DateTime.UtcNow)
                            return false;

                        from.LastDexGain = DateTime.UtcNow;
                        break;
                    }
                case Stat.Int:
                    {
                        if (from is BaseCreature && ((BaseCreature)from).Controlled)
                        {
                            if (from.LastIntGain + _PetStatGainDelay >= DateTime.UtcNow)
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
