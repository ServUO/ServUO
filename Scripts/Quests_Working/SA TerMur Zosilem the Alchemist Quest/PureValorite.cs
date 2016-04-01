using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class PureValorite : BaseQuest
    { 
        public PureValorite()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(InfusedGlassStave), "Infused Glass Stave", 5, 0x2AAA));
            this.AddObjective(new ObtainObjective(typeof(PileofInspectedValoriteIngots), "Pile of Inspected Valorite Ingots", 1, 0x1BEA));
                        			
            this.AddReward(new BaseReward(typeof(ElixirofValoriteConversion), 1, "Elixir of Valorite Conversion"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(TheForbiddenFruit);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromDays(1);
            }
        }
        public override object Title
        {
            get
            {
                return "Pure Valorite";
            }
        }
        public override object Description
        {
            get
            {
                return 1112983;
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
                return 1112986;
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