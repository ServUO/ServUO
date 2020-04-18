using System.Collections.Generic;

/*
 * Called in MondainsQuestGump.cs to show simple string for simple quest objectives.
 */

namespace Server.Engines.Quests
{
    public abstract class SimpleObjective : BaseObjective
    {
        public abstract List<string> Descriptions { get; }

        public SimpleObjective(int amount, int seconds)
            : base(amount, seconds)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}