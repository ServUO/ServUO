using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Network;
using Server.Gumps;

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
        public bool HasRecievedControlSlotWarning { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgress { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgressMax { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Creature { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double TrainingProgressPercentile { get { return TrainingProgress / TrainingProgressMax; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlots { get { return Creature.ControlSlots; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlotsMin { get { return Creature.ControlSlotsMin; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ControlSlotsMax { get { return Creature.ControlSlotsMax; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanApplyOptions { get { return HasBegunTraining && TrainingProgressPercentile >= 1.0; } }

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

        public double PowerHourMultiplier
        {
            get
            {
                if (InPowerHour)
                {
                    return 2.0; // 2x gains from creatures during power hour
                }

                return 1.0;
            }
        }

        private static readonly int BaseGainsPerCreature = 50;

        private Dictionary<BaseCreature, int> _ProgressTable;

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
                TrainingProgressMax = 100;
            }
        }

        public void OnTrain(PlayerMobile pm, int points)
        {
            if (!HasIncreasedControlSlot)
            {
                Creature.RemoveFollowers();
                Creature.ControlSlots++;
                Creature.AddFollowers();

                pm.SendLocalizedMessage(1157537); // Your pet's control slot have been updated.

                HasIncreasedControlSlot = true;

                Creature.AdjustTameRequirements();
            }

            TrainingPoints -= points;
        }

        public void EndTraining()
        {
            HasBegunTraining = false;
            _TrainingPoints = 0;

            HasRecievedControlSlotWarning = false;
            HasIncreasedControlSlot = false;
        }

        private bool CheckCanProgress(BaseCreature bc)
        {
            if (_ProgressTable.ContainsKey(bc))
            {
                int gains = GetGainsPerCreature();

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

        private int GetGainsPerCreature()
        {
            int level = 1 + (ControlSlots - ControlSlotsMin);

            return (int)(((double)BaseGainsPerCreature / ((double)level * 1.5)) * PowerHourMultiplier);
        }

        public void CheckProgress(BaseCreature bc)
        {
            if (ControlSlots >= ControlSlotsMax || !HasBegunTraining || TrainingProgress >= TrainingProgressMax || Creature.ControlMaster == null)
                return;

            int dif = (int)(Creature.BardingDifficulty - bc.BardingDifficulty);
            int level = 1 + (ControlSlots - ControlSlotsMin);

            if (Utility.Random(100) < (8 - level))
            {
                if (dif <= 50 && CheckCanProgress(bc))
                {
                    if (PowerHourBegin + PowerHourDelay < DateTime.UtcNow)
                    {
                        _ProgressTable.Clear();
                        
                        PowerHourBegin = DateTime.UtcNow;
                        InPowerHour = true;
                        Creature.ControlMaster.SendLocalizedMessage(1157569); // [Pet Training Power Hour]:  Your pet is under the effects of enhanced training progress for the next hour!
                    }
                    else if (InPowerHour && PowerHourBegin + PowerHourDuration < DateTime.UtcNow)
                    {
                        InPowerHour = false;
                        Creature.ControlMaster.SendLocalizedMessage(1157570); // [Pet Training Power Hour]:  Your pet is no longer under the effects of enhanced training progress.
                    }

                    double toAdd = Math.Round(.25 + (Math.Max(0, (bc.BardingDifficulty / Creature.BardingDifficulty)) * 1.0), 2);

                    TrainingProgress = Math.Min(TrainingProgressMax, TrainingProgress + toAdd);

                    if (!bc.Controlled && !bc.Summoned)
                    {
                        int cliloc = 1157574; // *The pet's battle experience has greatly increased!*

                        if (toAdd < 1.3)
                            cliloc = 1157565; // *The pet's battle experience has slightly increased!*
                        else if (toAdd < 2.5)
                            cliloc = 1157573; // *The pet's battle experience has fairly increased!*

                        if (Creature.ControlMaster.HasGump(typeof(PetTrainingProgressGump)))
                        {
                            ResendProgressGump(Creature.ControlMaster);
                        }

                        Creature.PrivateOverheadMessage(MessageType.Regular, 0x59, cliloc, Creature.ControlMaster.NetState);

                        if (TrainingProgress >= TrainingProgressMax)
                        {
                            Creature.PrivateOverheadMessage(MessageType.Regular, 0x59, 1157543, Creature.ControlMaster.NetState); // *The creature surges with battle experience and is ready to train!*

                            Server.Engines.Quests.LeadingIntoBattleQuest.CheckComplete(Creature.ControlMaster as PlayerMobile);
                        }
                    }
                    else
                    {
                        Creature.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157564, Creature.ControlMaster.NetState); // *The pet does not appear to train from that*
                    }
                }
                else
                {
                    Creature.PrivateOverheadMessage(MessageType.Regular, 0x21, 1157564, Creature.ControlMaster.NetState); // *The pet does not appear to train from that*
                }
            }
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
                Server.Gumps.BaseGump.SendGump(new PetTrainingProgressGump((PlayerMobile)m, Creature));
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
            writer.Write(1);

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
