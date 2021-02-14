using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class PlanningProfile
    {
        public BaseCreature Creature { get; }
        public List<PlanningEntry> Entries { get; }

        public PlanningProfile(BaseCreature bc)
        {
            Creature = bc;
            Entries = new List<PlanningEntry>();
        }

        public void Clear()
        {
            Entries.Clear();
        }

        public void AddToPlan(object tp, int value, int cost)
        {
            PlanningEntry entry = Entries.FirstOrDefault(e => e.TrainPoint == tp);

            if (entry != null)
                Entries.Remove(entry);

            Entries.Add(new PlanningEntry(tp, value, cost));

            if (tp is MagicalAbility ability && ability <= MagicalAbility.WrestlingMastery)
            {
                TrainingPoint trainingPoint = PetTrainingHelper.GetTrainingPoint(ability);

                foreach (PlanningEntry en in Entries)
                {
                    if (trainingPoint.Requirements != null && trainingPoint.Requirements.Length > 0)
                    {
                        foreach (TrainingPointRequirement req in trainingPoint.Requirements.Where(r => r != null))
                        {
                            if (req.Requirement is WeaponAbility && en.TrainPoint is WeaponAbility || req.Requirement is SpecialAbility && en.TrainPoint is SpecialAbility || req.Requirement is AreaEffect && en.TrainPoint is AreaEffect)
                            {
                                en.Value = 0;
                                en.Cost = 0;
                            }
                        }
                    }
                }
            }
        }

        public class PlanningEntry
        {
            public object TrainPoint { get; private set; }
            public int Value { get; set; }
            public int Cost { get; set; }

            public PlanningEntry(object tp, int value, int cost)
            {
                TrainPoint = tp;
                Value = value;
                Cost = cost;
            }
        }

        public PlanningProfile(BaseCreature bc, GenericReader reader)
        {
            int version = reader.ReadInt();

            Entries = new List<PlanningEntry>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                switch (reader.ReadInt())
                {
                    case 0: break;
                    case 1: Entries.Add(new PlanningEntry((MagicalAbility)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                    case 2: Entries.Add(new PlanningEntry(SpecialAbility.Abilities[reader.ReadInt()], reader.ReadInt(), reader.ReadInt())); break;
                    case 3: Entries.Add(new PlanningEntry(AreaEffect.Effects[reader.ReadInt()], reader.ReadInt(), reader.ReadInt())); break;
                    case 4: Entries.Add(new PlanningEntry(WeaponAbility.Abilities[reader.ReadInt()], reader.ReadInt(), reader.ReadInt())); break;
                    case 5: Entries.Add(new PlanningEntry((PetStat)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                    case 6: Entries.Add(new PlanningEntry((ResistanceType)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                    case 7: Entries.Add(new PlanningEntry((SkillName)reader.ReadInt(), reader.ReadInt(), reader.ReadInt())); break;
                }
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Entries.Count);

            for (int i = 0; i < Entries.Count; i++)
            {
                PlanningEntry entry = Entries[i];
                object o = entry.TrainPoint;

                if (o is MagicalAbility ability)
                {
                    writer.Write(1);
                    writer.Write((int)ability);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is SpecialAbility specialAbility)
                {
                    writer.Write(2);
                    writer.Write(Array.IndexOf(SpecialAbility.Abilities, specialAbility));
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is AreaEffect effect)
                {
                    writer.Write(3);
                    writer.Write(Array.IndexOf(AreaEffect.Effects, effect));
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is WeaponAbility weaponAbility)
                {
                    writer.Write(4);
                    writer.Write(Array.IndexOf(WeaponAbility.Abilities, weaponAbility));
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is PetStat stat)
                {
                    writer.Write(5);
                    writer.Write((int)stat);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is ResistanceType resistType)
                {
                    writer.Write(6);
                    writer.Write((int)resistType);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is SkillName skillName)
                {
                    writer.Write(7);
                    writer.Write((int)skillName);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else
                {
                    writer.Write(0);
                }
            }
        }
    }
}
