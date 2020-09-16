using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{

    public enum TrainingMode
    {
        Regular,
        Planning
    }

    [PropertyObject]
    public class TrainingProfile
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public TrainingMode TrainingMode { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBegunTraining { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasIncreasedControlSlot { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrainedThisLevel { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasRecievedControlSlotWarning { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgress { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgressMax { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Creature { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgressPercentile => TrainingProgress / TrainingProgressMax;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlots => Creature.ControlSlots;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlotsMin => Creature.ControlSlotsMin;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlotsMax => Creature.ControlSlotsMax;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanApplyOptions => HasBegunTraining && TrainingProgressPercentile >= 1.0;

        private int _TrainingPoints;
        private int _StartingTrainingPoints;
        private PlanningProfile _Plan;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrainingPoints
        {
            get { return _TrainingPoints; }
            set
            {
                if (value <= 0)
                {
                    EndTraining();
                }
                else
                {
                    _TrainingPoints = value;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StartingTrainingPoints
        {
            get { return _StartingTrainingPoints; }
            set { _StartingTrainingPoints = value; }
        }

        public PlanningProfile PlanningProfile
        {
            get
            {
                if (_Plan == null)
                    _Plan = new PlanningProfile(Creature);

                return _Plan;
            }
        }

        public static TimeSpan PowerHourDuration = TimeSpan.FromHours(1);
        public static TimeSpan PowerHourDelay = TimeSpan.FromHours(24);

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PowerHourBegin { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InPowerHour { get; set; }

        public int PowerHourMultiplier
        {
            get
            {
                if (InPowerHour)
                {
                    return 2; // 2x gains from creatures during power hour
                }

                return 1;
            }
        }

        private static readonly int MaxTrainingProgress = 100;

        private readonly Dictionary<BaseCreature, int> _ProgressTable;

        public TrainingProfile(BaseCreature bc)
        {
            Creature = bc;

            _ProgressTable = new Dictionary<BaseCreature, int>();
        }

        private int AssignStartingTrainingPoints()
        {
            if (ControlSlots != ControlSlotsMin)
            {
                return 1501;
            }

            if (ControlSlotsMin == 1 && ControlSlotsMax == 2)
            {
                return 2556;
            }

            if (ControlSlotsMin == 1 && ControlSlotsMax == 3)
            {
                return 2381;
            }

            return 1501;
        }

        public void BeginTraining()
        {
            if (ControlSlots < ControlSlotsMax)
            {
                TrainingPoints = AssignStartingTrainingPoints();
                HasBegunTraining = true;

                TrainingProgress = 0;
                TrainingProgressMax = MaxTrainingProgress;
            }
        }

        public void OnTrain(PlayerMobile pm, int points)
        {
            int reqInc;

            if (!HasIncreasedControlSlot)
            {
                reqInc = GetRequirementIncrease(true);

                Creature.RemoveFollowers();
                Creature.ControlSlots++;
                Creature.AddFollowers();
                TrainedThisLevel = 0;

                pm.SendLocalizedMessage(1157537); // Your pet's control slot have been updated.
                HasIncreasedControlSlot = true;
            }
            else
            {
                TrainedThisLevel++;
                reqInc = GetRequirementIncrease(false);
            }

            Creature.CurrentTameSkill = Math.Min(BaseCreature.MaxTameRequirement, Creature.CurrentTameSkill + reqInc);
            TrainingPoints -= points;
        }

        public int GetRequirementIncrease(bool slotIncreased)
        {
            if (slotIncreased)
            {
                // First level
                if (ControlSlotsMin + 1 == ControlSlots)
                {
                    return 21;
                }
                // subsequent trains of that level
                else
                {
                    return 22;
                }
            }
            else
            {
                // First train of that level (after level increase, so actually 2nd train)
                if (TrainedThisLevel == 1)
                {
                    return 3;
                }
                // subsequent trains of that level
                else
                {
                    return 2;
                }
            }
        }

        public void EndTraining()
        {
            HasBegunTraining = false;
            _TrainingPoints = 0;

            HasRecievedControlSlotWarning = false;
            HasIncreasedControlSlot = false;
        }

        private bool CheckCanProgress(BaseCreature bc, double toGain)
        {
            if (_ProgressTable.ContainsKey(bc))
            {
                int gains = GetGainsPerCreature(toGain);

                if (_ProgressTable[bc] >= gains)
                {
                    return false;
                }
                else
                {
                    _ProgressTable[bc]++;
                    return true;
                }
            }

            _ProgressTable[bc] = 1;
            return true;
        }

        private int GetGainsPerCreature(double toGain)
        {
            int gains = 0;

            switch (ControlSlots)
            {
                case 1: gains = int.MaxValue; break;
                case 2: gains = int.MaxValue; break;
                case 3: gains = (int)((MaxTrainingProgress / toGain) / 2.0); break;
                default: gains = (int)((MaxTrainingProgress / toGain) / 4.0); break;
            }

            if (gains < int.MaxValue)
            {
                gains *= PowerHourMultiplier;
            }

            return gains;
        }

        public void CheckProgress(BaseCreature bc)
        {
            if (ControlSlots >= ControlSlotsMax || !HasBegunTraining || TrainingProgress >= TrainingProgressMax || Creature.ControlMaster == null)
                return;

            double ourDif = Creature.BardingDifficulty;
            double theirDif = bc.BardingDifficulty;
            Mobile master = Creature.ControlMaster;

            if (Utility.Random(100) < 8 - (1 + (ControlSlots - ControlSlotsMin)))
            {
                double toGain = GetAdvance(theirDif); // Math.Round(.25 + (Math.Max(0, (bc.BardingDifficulty / Creature.BardingDifficulty)) * 1.0), 2);

                if (ourDif - theirDif <= 50 && CheckCanProgress(bc, toGain))
                {
                    if (PowerHourBegin + PowerHourDelay < DateTime.UtcNow)
                    {
                        _ProgressTable.Clear();

                        PowerHourBegin = DateTime.UtcNow;
                        InPowerHour = true;
                        master.SendLocalizedMessage(1157569); // [Pet Training Power Hour]:  Your pet is under the effects of enhanced training progress for the next hour!
                    }
                    else if (InPowerHour && PowerHourBegin + PowerHourDuration < DateTime.UtcNow)
                    {
                        InPowerHour = false;
                        master.SendLocalizedMessage(1157570); // [Pet Training Power Hour]:  Your pet is no longer under the effects of enhanced training progress.
                    }

                    TrainingProgress = Math.Min(TrainingProgressMax, TrainingProgress + toGain);

                    if (!bc.Controlled && !bc.Summoned && master is PlayerMobile)
                    {
                        int cliloc = 1157574; // *The pet's battle experience has greatly increased!*

                        if (toGain < 1.3)
                            cliloc = 1157565; // *The pet's battle experience has slightly increased!*
                        else if (toGain < 2.5)
                            cliloc = 1157573; // *The pet's battle experience has fairly increased!*

                        if (master.HasGump(typeof(PetTrainingProgressGump)))
                        {
                            ResendProgressGump(master);
                        }
                        else
                        {
                            if (master.InRange(Creature.Location, 12))
                            {
                                BaseGump.SendGump(new PetTrainingProgressGump((PlayerMobile)master, Creature));
                            }
                        }

                        Creature.PrivateOverheadMessage(MessageType.Regular, 0x59, cliloc, master.NetState);

                        if (TrainingProgress >= TrainingProgressMax)
                        {
                            Creature.PrivateOverheadMessage(MessageType.Regular, 0x59, 1157543, master.NetState); // *The creature surges with battle experience and is ready to train!*

                            Engines.Quests.LeadingIntoBattleQuest.CheckComplete((PlayerMobile)master);
                        }
                    }
                    else
                    {
                        Creature.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157564, master.NetState); // *The pet does not appear to train from that*
                    }
                }
                else
                {
                    Creature.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157564, master.NetState); // *The pet does not appear to train from that*
                }
            }
        }

        private double GetAdvance(double difficulty)
        {
            double advance = difficulty / 64;

            if (advance >= 2.5)
                advance = 2.5;

            return advance;
        }

        public void ResendProgressGump(Mobile m)
        {
            if (m == null || m.NetState == null || !(m is PlayerMobile))
            {
                return;
            }

            PetTrainingProgressGump g = m.FindGump(typeof(PetTrainingProgressGump)) as PetTrainingProgressGump;

            if (g == null)
            {
                BaseGump.SendGump(new PetTrainingProgressGump((PlayerMobile)m, Creature));
            }
            else
            {
                g.Refresh();
            }
        }

        public override string ToString()
        {
            return "...";
        }

        public TrainingProfile(BaseCreature bc, GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    TrainedThisLevel = reader.ReadInt();
                    goto case 1;
                case 1:
                    PowerHourBegin = reader.ReadDateTime();
                    InPowerHour = reader.ReadBool();
                    break;
            }

            Creature = bc;

            if (reader.ReadInt() == 1)
            {
                _Plan = new PlanningProfile(bc, reader);
            }

            TrainingMode = (TrainingMode)reader.ReadInt();
            HasBegunTraining = reader.ReadBool();
            HasIncreasedControlSlot = reader.ReadBool();
            HasRecievedControlSlotWarning = reader.ReadBool();
            TrainingProgress = reader.ReadDouble();
            TrainingProgressMax = reader.ReadDouble();

            _StartingTrainingPoints = reader.ReadInt();
            _TrainingPoints = reader.ReadInt();

            _ProgressTable = new Dictionary<BaseCreature, int>();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(2);

            writer.Write(TrainedThisLevel);

            writer.Write(PowerHourBegin);
            writer.Write(InPowerHour);

            if (_Plan != null)
            {
                writer.Write(1);
                _Plan.Serialize(writer);
            }
            else
            {
                writer.Write(0);
            }

            writer.Write((int)TrainingMode);
            writer.Write(HasBegunTraining);
            writer.Write(HasIncreasedControlSlot);
            writer.Write(HasRecievedControlSlotWarning);
            writer.Write(TrainingProgress);
            writer.Write(TrainingProgressMax);

            writer.Write(_StartingTrainingPoints);
            writer.Write(_TrainingPoints);
        }
    }
}
