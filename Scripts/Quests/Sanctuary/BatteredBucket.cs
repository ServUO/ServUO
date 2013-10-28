using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class LostAndFoundQuest : BaseQuest
    { 
        public LostAndFoundQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(BatteredBucket) , "battered bucket", 1, typeof(Dallid), "Dallid (Sanctuary)", 600));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Lost and Found */
        public override object Title
        {
            get
            {
                return 1072370;
            }
        }
        /* The battered, old bucket is inscribed with barely legible writing that indicates it 
        belongs to someone named "Dallid".  Maybe they'd pay for its return? */
        public override object Description
        {
            get
            {
                return 1072589;
            }
        }
        /* You're right, who cares if Dallid might pay for his battered old bucket back.  
        This way you can carry it around with you! */
        public override object Refuse
        {
            get
            {
                return 1072590;
            }
        }
        /* Whoever this "Dallid" might be, he's probably looking for his bucket. */
        public override object Uncomplete
        {
            get
            {
                return 1072591;
            }
        }
        /* Is that my bucket? I had to ditch my favorite bucket when a group of ratmen jumped me! */
        public override object Complete
        {
            get
            {
                return 1074580;
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

    public class BatteredBucket : BaseQuestItem
    {
        [Constructable]
        public BatteredBucket()
            : base(0x2004)
        {
        }

        public BatteredBucket(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(LostAndFoundQuest)
                };
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1073129;
            }
        }// A battered bucket
        public override int Lifespan
        {
            get
            {
                return 600;
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