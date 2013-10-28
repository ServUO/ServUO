using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class DabblingontheDarkSide : BaseQuest
    { 
        public DabblingontheDarkSide()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            this.AddObjective(new ObtainObjective(typeof(FairyDragonWing), "Fairy Dragon Wings", 10, 0x1084));
            this.AddObjective(new ObtainObjective(typeof(Dough), "Dough", 1, 0x103D));
						
            this.AddReward(new BaseReward(typeof(DeliciouslyTastyTreat), 2, "Deliciously Tasty Treat"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(TheBrainyAlchemist);
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
                return "Dabbling on the Dark Side";
            }
        }
        public override object Description
        {
            get
            {
                return 1112963;
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
                return 1112966;
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