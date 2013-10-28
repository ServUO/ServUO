using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class TheBrainyAlchemist : BaseQuest
    { 
        public TheBrainyAlchemist()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(ArcaneGem), "Arcane Gem", 1, 0x1ea7));
            this.AddObjective(new ObtainObjective(typeof(UndeadGargHorn), "Undamaged Undead Gargoyle Horns", 10, 0x2F5F));
            this.AddObjective(new ObtainObjective(typeof(InspectedKegofTotalRefreshment), "Inspected Keg of Total Refreshment", 1, 0x1940));
            this.AddObjective(new ObtainObjective(typeof(InspectedKegofGreaterPoison), "Inspected Keg of Greater Poison", 1, 0x1940));
									
            this.AddReward(new BaseReward(typeof(InfusedAlchemistsGem), "Infused Alchemist's Gem"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(ArmorUp);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(30);
            }
        }
        public override object Title
        {
            get
            {
                return "The Brainy Alchemist";
            }
        }
        public override object Description
        {
            get
            {
                return 1112967;
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
                return 1112970;
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