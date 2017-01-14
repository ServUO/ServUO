using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class ToTurnBaseMetalIntoVerite : BaseQuest
    { 
        public ToTurnBaseMetalIntoVerite()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(UndeadGargoyleMedallions), "Undead Gargoyle Medallions", 5, 0x2AAA));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedVeriteIngots), "Pile of Inspected Verite Ingots", 1, 0x1BEA));
                        			
            this.AddReward(new BaseReward(typeof(ElixirofVeriteConversion), 1, "Elixir of Verite Conversion"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(PureValorite);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        public override object Title
        {
            get
            {
                return "To Turn Base Metal Into Verite";
            }
        }
        public override object Description
        {
            get
            {
                return 1112975;
            }
        }
        public override object Refuse
        {
            get
            {
                return "You are Scared from this Task !! Muahahah";
            }
        }
        public override object Uncomplete
        {
            get
            {
                return "I am sorry that you have not accepted!";
            }
        }
        public override object Complete
        {
            get
            {
                return 1112978;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}