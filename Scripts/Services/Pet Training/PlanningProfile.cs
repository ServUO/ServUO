using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

namespace Server.Mobiles
{
    public class PlanningProfile
    {
        public BaseCreature Creature { get; private set; }
        public List<PlanningEntry> Entries { get; private set; }

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
            var entry = Entries.FirstOrDefault(e => e.TrainPoint == tp);

            if (entry != null)
                Entries.Remove(entry);

            Entries.Add(new PlanningEntry(tp, value, cost));

            if (tp is MagicalAbility && (MagicalAbility)tp <= MagicalAbility.WrestlingMastery)
            {
                var trainingPoint = PetTrainingHelper.GetTrainingPoint(tp);

                foreach (var en in Entries)
                {
                    if (trainingPoint.Requirements != null && trainingPoint.Requirements.Length > 0)
                    {
                        foreach (var req in trainingPoint.Requirements.Where(r => r != null))
                        {
                            if ((req.Requirement is WeaponAbility && en.TrainPoint is WeaponAbility) ||
                               (req.Requirement is SpecialAbility && en.TrainPoint is SpecialAbility) ||
                                (req.Requirement is AreaEffect && en.TrainPoint is AreaEffect))
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

            for(int i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                object o = entry.TrainPoint;

                if (o is MagicalAbility)
                {
                    writer.Write(1);
                    writer.Write((int)(MagicalAbility)o);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is SpecialAbility)
                {
                    writer.Write(2);
                    writer.Write(Array.IndexOf(SpecialAbility.Abilities, (SpecialAbility)o));
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is AreaEffect)
                {
                    writer.Write(3);
                    writer.Write(Array.IndexOf(AreaEffect.Effects, (AreaEffect)o));
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is WeaponAbility)
                {
                    writer.Write(4);
                    writer.Write(Array.IndexOf(WeaponAbility.Abilities, (WeaponAbility)o));
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is PetStat)
                {
                    writer.Write(5);
                    writer.Write((int)(PetStat)o);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is ResistanceType)
                {
                    writer.Write(6);
                    writer.Write((int)(ResistanceType)o);
                    writer.Write(entry.Value);
                    writer.Write(entry.Cost);
                }
                else if (o is SkillName)
                {
                    writer.Write(7);
                    writer.Write((int)(SkillName)o);
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
