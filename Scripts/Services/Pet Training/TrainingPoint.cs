namespace Server.Mobiles
{
    public class TrainingPoint
    {
        public object TrainPoint { get; }
        public double Weight { get; }
        public int Start { get; }
        public int Max { get; }
        public TextDefinition Name { get; }
        public TextDefinition Description { get; }

        public TrainingPointRequirement[] Requirements { get; }

        public TrainingPoint(object trainpoint, double weight, int start, int max, TextDefinition name, TextDefinition description, params TrainingPointRequirement[] requirements)
        {
            TrainPoint = trainpoint;
            Weight = weight;
            Start = start;
            Max = max;

            Name = name;
            Description = description;

            Requirements = requirements;
        }

        public int GetMax(BaseCreature bc)
        {
            if (TrainPoint is PetStat stat && stat == PetStat.BaseDamage)
            {
                return PetTrainingHelper.GetMaxDamagePerSecond(bc);
            }

            return Max;
        }
    }

    public class TrainingPointRequirement
    {
        public object Requirement { get; set; }
        public int Cost { get; set; }
        public TextDefinition Name { get; set; }

        public TrainingPointRequirement(object requirement, int cost, TextDefinition name)
        {
            Requirement = requirement;
            Cost = cost;
            Name = name;
        }
    }
}
