using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class TheExchangeQuest : BaseQuest
    {
        public TheExchangeQuest()
            : base()

        {
            this.AddObjective(new ObtainObjective(typeof(WhiteChocolate), "White Chocolates", 5, 0xF11));
            this.AddObjective(new ObtainObjective(typeof(DarkSapphire), "Dark Sapphire", 1, 0x3192));

            this.AddReward(new BaseReward(typeof(AverageImbuingBag), 1113768));//Average Imbuing Bag
            this.AddReward(new BaseReward("Loyalty Rating")); 
        }

        /*The Exchange*/
        public override object Title
        {
            get
            {
                return 1113777;
            }
        }
        //Hello there! Hail and well met, and all of that. I must apologize in advance for being
        //so impatient, but you must help me! You see, my mother and my eldest sister are visiting
        //soon, and I haven’t seen them in quite awhile, so I want to present them both with a
        //surprise when they arrive.<br><br>My sister absolutely adores white chocolate, but 
        //gargoyles don’t seem to care for it much, so I haven’t been able to find any here.
        //It was recently my mother’s birthday, and I know that she would love some finely 
        //crafted gargish jewelry, but the jeweler hasn’t had her favorite jewel in stock for 
        //quite some time. If you could help me obtain five pieces of white chocolate and one
        //dark sapphire, I will reward you with a bag of hard to obtain imbuing ingredients.
        public override object Description
        {
            get
            {
                return 1113778;
            }
        }
        //Oh, no, you must help me! Please say that you will!
        public override object Refuse
        {
            get
            {
                return 1113779;
            }
        }
        //Remember, I need five pieces of white chocolate, and one dark sapphire. Please do hurry!
        public override object Uncomplete
        {
            get
            {
                return 1113780;
            }
        }
        //Oh, thank you so very much! I cannot begin to thank you enough for helping me find 
        //these presents. Here is your reward. You’ll have to excuse me while I set this dark
        //sapphire in a setting that will best highlight the cut. Farewell!
        public override object Complete
        {
            get
            {
                return 1113781;
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

    public class AWorthyPropositionQuest : BaseQuest
    {
        public AWorthyPropositionQuest()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(BambooFlute), "Bamboo Flutes", 10, 0x2805));
            this.AddObjective(new ObtainObjective(typeof(ElvenFletchings), "Elven Fletching", 1, 0x5737));

            this.AddReward(new BaseReward(typeof(ValuableImbuingBag), 1113769));//Valuable Imbuing Bag
            this.AddReward(new BaseReward("Loyalty Rating")); 
        }

        /* A Worthy Proposition */
        public override object Title
        {
            get
            {
                return 1113782;
            }
        }

        //Hello, welcome to the shop. I don't own it, but the gargoyles here are as keen to 
        //learn from me as I am from them. They've been helping me with the work on my latest
        //invention, but I am short some parts. Perhaps you could help me?<br><br>I have heard 
        //that the bamboo flutes of the Tokuno Islands are exceptionally strong for their weight, 
        //and nothing can beat elven fletching for strength in holding them together. If you 
        //would bring me, say, ten bamboo flutes and some elven fletching, I have some valuable
        //imbuing ingredients I’ll give you in exchange. What do you say?
        public override object Description
        {
            get
            {
                return 1113783;
            }
        }
        //Well, if you change your mind, I’ll be here.
        public override object Refuse
        {
            get
            {
                return 1113784;
            }
        }
        //Hmm, what is that? Oh yes, I would like you to bring me ten bamboo flutes and some elven
        //fletching for my fly… er, my invention.
        public override object Uncomplete
        {
            get
            {
                return 1113785;
            }
        }
        //These are of fine quality! I think they will work just fine to reinforce the floor of the 
        //basket. What’s that? Did I say basket? I meant, bakery! Yes, I am inventing a, um, floor 
        //for a bakery. There is a great need for that, you know! Ok, now please leave so I can get 
        //back to work. Thank you, bye, bye!
        public override object Complete
        {
            get
            {
                return 1113786;
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

    public class Aliabeth : MondainQuester
    {

        public override void InitSBInfo()
        {
            this.SBInfos.Add(new SBTinker());
        }

        [Constructable]
        public Aliabeth()
            : base("Aliabeth", "the Tinker")
        {
            this.SetSkill(SkillName.Lockpicking, 60.0, 83.0);
            this.SetSkill(SkillName.RemoveTrap, 75.0, 98.0);
            this.SetSkill(SkillName.Tinkering, 64.0, 100.0);
        }

        public Aliabeth(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(AWorthyPropositionQuest),
                    typeof(TheExchangeQuest),
                };
            }
        }
        public override void InitBody()
        {
            this.Female = true;
            //this.CantWalk = true;
            this.Race = Race.Human;

            base.InitBody();
        }
        public override void InitOutfit()
        {
            this.AddItem(new Backpack());

            this.AddItem(new Kilt(Utility.RandomNeutralHue()));
            this.AddItem(new Shirt(Utility.RandomNeutralHue()));
            this.AddItem(new Sandals());
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