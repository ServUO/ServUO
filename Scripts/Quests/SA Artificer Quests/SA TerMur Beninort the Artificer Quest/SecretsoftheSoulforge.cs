using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class SecretsoftheSoulforge : BaseQuest
    { 
        public SecretsoftheSoulforge()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(MagicalResidue), "Magical Residue", 50, 0x2DB1));
          						
            this.AddReward(new BaseReward(typeof(ScrollBox3), 1, "Knowledge"));
        }

        public override object Title
        {
            get
            {
                return "Secrets of the Soulforge";
            }
        }
        public override object Description
        {
            get
            {
                return 1112522;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112523;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112547;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112524;
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