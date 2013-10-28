using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class InTheWeeds : BaseQuest
    { 
        public InTheWeeds()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(FNPitchfork), "Farmer Nash's Pitchfork", 1, 0xE87));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* In The Weeds */
        public override object Title
        {
            get
            {
                return 1113499;
            }
        }
        /* Help Farmer Nash find his pitchfork by pulling the weeds in his garden until you uncover it. (Pull weeds by double clicking them) When you find it, return it to him for your reward. I hate to trouble you, but sometimes a problem needs a plow and sometimes it needs a sword. I am good with a plow, but terrible with a sword. 
        I have been plagued with a strange weed for some time. Every day I have to clean them out of my garden and carry them away with my pitch fork. Yesterday I was working there and… well, I must have nodded off because when I woke my pitchfork was gone! 
        I have heard talk of thieves who seek treasure in the sacred tomb, but I really don’t think they took my pitchfork, in fact I think it just got lost in the weeds! 
        I would find it myself, but now that we are so close to the edge of the world, many wild creatures are lurking about and some might be hiding in these weeds. I’ve seen the creatures that have been roaming these parts recently and I fear for my life! The problem is, if I don’t get my crop in the ground soon, we won’t make it through the winter. Will you help?*/
        public override object Description
        {
            get
            {
                return 1113500;
            }
        }
        /* I understand.  I certainly don’t want you to do something you don’t want to do. */
        public override object Refuse
        {
            get
            {
                return 1113501;
            }
        }
        /* Did you find my pitchfork?  I'm sure it is under those weeds somewhere.  It was a gift from King Draxinusom when he assigned me this job, I can’t bear to lose it! */
        public override object Uncomplete
        {
            get
            {
                return 1113502;
            }
        }
        /* Oh, thank you!  Here is your reward as promised.  I will get right back to work in a few minutes. */
        public override object Complete
        {
            get
            {
                return 1113503;
            }
        }
        public override bool CanOffer()
        {
            return Core.SA;
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

    public class FarmerNash : MondainQuester
    {
        [Constructable]
        public FarmerNash()
            : base("Farmer Nash")
        { 
        }

        public FarmerNash(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(InTheWeeds)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Gargoyle;
			
            this.Hue = 0x840C;
            this.HairItemID = 0x2045;
            this.HairHue = 0x453;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals(0x74A));
            this.AddItem(new Robe(0x498));
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