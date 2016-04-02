using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class ALittleSomething : BaseQuest
    { 
        public ALittleSomething()
            : base()
        {
			this.AddObjective(new ObtainObjective(typeof(BrilliantAmber), "Brilliant Amber", 1, 0x3199));
          						
            this.AddReward(new BaseReward(typeof(MeagerImbuingBag), 1, "Meager Imbuing Bag"));
        }

        public override object Title
        {
            get
            {
                return "A Little Something";
            }
        }
        public override object Description
        {
            get
            {
                return 1113773;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1113774;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1113775;
            }
        }
        public override object Complete
        {
            get
            {
                return 1113776;
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