using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Network;

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
        private const int StartTrainingPoints = 1500;

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

        public TrainingProfile(BaseCreature bc)
        {
            Creature = bc;
        }

        public void BeginTraining()
        {
            if (ControlSlots < ControlSlotsMax)
            {
                TrainingPoints = StartTrainingPoints;
                HasBegunTraining = true;

                TrainingProgress = 0;
                TrainingProgressMax = 100;
            }
        }

        public void OnTrain()
        {
            if (!HasIncreasedControlSlot)
            {
                Creature.ControlSlots++;
                HasIncreasedControlSlot = true;
            }
        }

        public void EndTraining()
        {
            HasBegunTraining = false;
            _TrainingPoints = 0;

            HasRecievedControlSlotWarning = false;
            HasIncreasedControlSlot = false;
        }

        public void CheckProgress(BaseCreature bc)
        {
            if (ControlSlots >= ControlSlotsMax || !HasBegunTraining || TrainingProgress >= TrainingProgressMax)
                return;

            int dif = (int)(Creature.BardingDifficulty - bc.BardingDifficulty);
            int level = 1 + (ControlSlots - ControlSlotsMin);

            if (Utility.Random(100) < (6 - level))
            {
                if (dif >= -50)
                {
                    double toAdd = Math.Round(.25 + (Math.Max(2, (bc.BardingDifficulty / Creature.BardingDifficulty)) * 2.5), 2);

                    TrainingProgress = Math.Min(TrainingProgressMax, TrainingProgress + toAdd);

                    if (Creature.ControlMaster != null)
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

                        Creature.PrivateOverheadMessage(MessageType.Regular, 452, cliloc, Creature.ControlMaster.NetState);

                        if (TrainingProgress >= TrainingProgressMax)
                        {
                            Creature.PrivateOverheadMessage(MessageType.Regular, 452, 1157543, Creature.ControlMaster.NetState); // *The creature surges with battle experience and is ready to train!*
                        }
                    }
                }
                else if (Creature.ControlMaster != null)
                {
                    Creature.PrivateOverheadMessage(MessageType.Regular, 452, 1157564, Creature.ControlMaster.NetState); // *The pet does not appear to train from that*
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

            Creature = bc;

            TrainingMode = (TrainingMode)reader.ReadInt();
            HasBegunTraining = reader.ReadBool();
            HasIncreasedControlSlot = reader.ReadBool();
            HasRecievedControlSlotWarning = reader.ReadBool();
            TrainingProgress = reader.ReadDouble();
            TrainingProgressMax = reader.ReadDouble();

            _TrainingPoints = reader.ReadInt();

        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write((int)TrainingMode);
            writer.Write(HasBegunTraining);
            writer.Write(HasIncreasedControlSlot);
            writer.Write(HasRecievedControlSlotWarning);
            writer.Write(TrainingProgress);
            writer.Write(TrainingProgressMax);

            writer.Write(_TrainingPoints);
        }
    }
}
