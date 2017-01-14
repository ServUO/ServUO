using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheJoysOfLifeQuest : BaseQuest
    { 
        public TheJoysOfLifeQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(AriellesBauble), "arielle's baubles", 3, 0x23B));
						
            this.AddReward(new BaseReward(1072809)); // The boon of Arielle.
        }

        public override bool ForceRemember
        {
            get
            {
                return true;
            }
        }
        /* The Joys of Life */
        public override object Title
        {
            get
            {
                return 1072787;
            }
        }
        /* *giggle*  So serious, so grim!  *tickle*  Enjoy life!  Have fun!  Laugh!  Be merry!  *giggle*  Find three 
        of my baubles ... *giggle* I hid them! *giggles hysterically*  Hid them!  La la la!  Bring them quickly!  They 
        are magical and will hide themselves again if you are too slow. */
        public override object Description
        {
            get
            {
                return 1072832;
            }
        }
        /* *giggle* Too serious.  Too thinky! */
        public override object Refuse
        {
            get
            {
                return 1072833;
            }
        }
        /* Magical baubles hidden, find them as you're bidden!  *giggle* */
        public override object Uncomplete
        {
            get
            {
                return 1072834;
            }
        }
        /* *giggle* So pretty! */
        public override object Complete
        {
            get
            {
                return 1074177;
            }
        }
        public override void GiveRewards()
        { 
            base.GiveRewards();
			
            this.Owner.SendLocalizedMessage(1074944, null, 0x23); // You have gained the boon of Arielle!  You have been taught the importance of laughter and light spirits.  You are one step closer to claiming your elven heritage.
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

    public class DefendingTheHerdQuest : BaseQuest
    { 
        public DefendingTheHerdQuest()
            : base()
        { 
            this.AddObjective(new EscortObjective("Bravehorn's drinking pool"));
						
            this.AddReward(new BaseReward(1072806)); // The boon of Bravehorn.
        }

        public override bool ForceRemember
        {
            get
            {
                return true;
            }
        }
        /* Defending the Herd */
        public override object Title
        {
            get
            {
                return 1072785;
            }
        }
        /* *snort* ... guard-mates ... guard-herd *hoof stomp* ... defend-with-hoof-and-horn ... thirsty-drink.  
        *proud head-toss* */
        public override object Description
        {
            get
            {
                return 1072825;
            }
        }
        /* *snort* */
        public override object Refuse
        {
            get
            {
                return 1072826;
            }
        }
        /* *impatient hoof stomp* ... thirsty herd ... water scent. */
        public override object Uncomplete
        {
            get
            {
                return 1072827;
            }
        }
        public override void GiveRewards()
        { 
            base.GiveRewards();
			
            this.Owner.SendLocalizedMessage(1074942, null, 0x23); // You have gained the boon of Bravehorn!  You have glimpsed the nobility of those that sacrifice themselves for their people.  You are one step closer to claiming your elven heritage.
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

    public class CaretakerOfTheLandQuest : BaseQuest
    { 
        public CaretakerOfTheLandQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(SosariaSap), "sap of sosaria", 1, 0x1848));
						
            this.AddReward(new BaseReward(1072804)); // The boon of Strongroot.
        }

        public override bool ForceRemember
        {
            get
            {
                return true;
            }
        }
        /* Caretaker of the Land */
        public override object Title
        {
            get
            {
                return 1072783;
            }
        }
        /* Hrrrrr.  Hurrrr.  Huuuman.  *creaking branches*  Suuun on baaark, roooooots diiig deeeeeep, wiiind caaaresses 
        leeeaves … Hrrrrr.  Saaap of Sooosaria feeeeeeds us.  Hrrrrr.  Huuuman leeearn.  Caaaretaker of plaaants … teeend 
        … prooove.<br> */
        public override object Description
        {
            get
            {
                return 1072812;
            }
        }
        /* Hrrrrr.  Hrrrrr.  Huuuman. */
        public override object Refuse
        {
            get
            {
                return 1072813;
            }
        }
        /* Hrrrr. Hrrrr.  Roooooots neeeeeed saaap of Sooosaria.  Hrrrrr.  Roooooots tiiingle neeeaaar Yeeew.  Seeeaaarch.  Hrrrr! */
        public override object Uncomplete
        {
            get
            {
                return 1072814;
            }
        }
        /* Thiiirsty. Hurrr. Hurrr. */
        public override object Complete
        {
            get
            {
                return 1074175;
            }
        }
        public override void GiveRewards()
        { 
            base.GiveRewards();
			
            this.Owner.SendLocalizedMessage(1074941, null, 0x23); // You have gained the boon of Strongroot!  You have been approved by one whose roots touch the bones of Sosaria.  You are one step closer to claiming your elven heritage.
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

    public class SeasonsQuest : BaseQuest
    { 
        public SeasonsQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(RawFishSteak), "raw fish steaks", 20, 0x097A));
						
            this.AddReward(new BaseReward(1072803)); // The boon of Maul.
        }

        public override bool ForceRemember
        {
            get
            {
                return true;
            }
        }
        /* Seasons */
        public override object Title
        {
            get
            {
                return 1072782;
            }
        }
        /* *rumbling growl* *sniff* ... not-smell ... seek-fight ... not-smell ... fear-stench ... *rumble* ... cold-soon-time 
        comes ... hungry ... eat-fish ... sleep-soon-time ... *deep fang-filled yawn* ... much-fish. */
        public override object Description
        {
            get
            {
                return 1072802;
            }
        }
        /* *yawn* ... cold-soon-time ... *growl* */
        public override object Refuse
        {
            get
            {
                return 1072810;
            }
        }
        /* *sniff* *sniff* ... not-much-fish ... hungry ... *grumble* */
        public override object Uncomplete
        {
            get
            {
                return 1072811;
            }
        }
        /* *sniff* fish! much-fish! */
        public override object Complete
        {
            get
            {
                return 1074174;
            }
        }
        public override void GiveRewards()
        { 
            base.GiveRewards();
			
            this.Owner.SendLocalizedMessage(1074940, null, 0x23); // You have gained the boon of Maul!  Your understanding of the seasons grows.  You are one step closer to claiming your elven heritage.
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

    public class TheBalanceOfNatureQuest : BaseQuest
    { 
        public TheBalanceOfNatureQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(TimberWolf), "timber wolves", 15, "Huntsman's Forest"));
						
            this.AddReward(new BaseReward(1072807)); // The boon of the Huntsman.
        }

        public override bool ForceRemember
        {
            get
            {
                return true;
            }
        }
        /* The Balance of Nature */
        public override object Title
        {
            get
            {
                return 1072786;
            }
        }
        /* Ho, there human.  Why do you seek out the Huntsman?  The hunter serves the land by culling both predators and prey.  The hunter 
        maintains the essential balance of life and does not kill for sport or glory.  If you seek my favor, human, then demonstrate you 
        are capable of the duty.  Cull the wolves nearby. */
        public override object Description
        {
            get
            {
                return 1072829;
            }
        }
        /* Then begone. I have no time to waste on you, human. */
        public override object Refuse
        {
            get
            {
                return 1072830;
            }
        }
        /* The timber wolves are easily tracked, human. */
        public override object Uncomplete
        {
            get
            {
                return 1072831;
            }
        }
        public override void GiveRewards()
        { 
            base.GiveRewards();
			
            this.Owner.SendLocalizedMessage(1074943, null, 0x23); // You have gained the boon of the Huntsman!  You have been given a taste of the bittersweet duty of those who guard the balance.  You are one step closer to claiming your elven heritage.
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

    public class WisdomOfTheSphynxQuest : BaseQuest
    { 
        public WisdomOfTheSphynxQuest()
            : base()
        { 
            this.AddObjective(new InternalObjective());
						
            this.AddReward(new BaseReward(1072805)); // The boon of Enigma.
        }

        public override bool ForceRemember
        {
            get
            {
                return true;
            }
        }
        /* Wisdom of the Sphynx */
        public override object Title
        {
            get
            {
                return 1072784;
            }
        }
        /* I greet thee human and divine my boon thou seek.  Convey hence the object of my riddle and I shall reward thee 
        with thy desire.<br><br>Three lives have I.<br>Gentle enough to soothe the skin,<br>Light enough to caress the sky,
        <br>Hard enough to crack rocks<br>What am I? */
        public override object Description
        {
            get
            {
                return 1072822;
            }
        }
        /* As thou wish, human. */
        public override object Refuse
        {
            get
            {
                return 1072823;
            }
        }
        /* I give thee a hint then human.  The answer to my riddle must be held carefully or it cannot be contained at all.  
        Bring this elusive item to me in a suitable container. */
        public override object Uncomplete
        {
            get
            {
                return 1072824;
            }
        }
        /* Ah, thus it ends. */
        public override object Complete
        {
            get
            {
                return 1074176;
            }
        }
        public override void GiveRewards()
        { 
            base.GiveRewards();
			
            this.Owner.SendLocalizedMessage(1074945, null, 0x23); // You have gained the boon of Enigma!  You are wise enough to know how little you know.  You are one step closer to claiming your elven heritage.
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

        private class InternalObjective : ObtainObjective
        {
            public InternalObjective()
                : base(typeof(Pitcher), "answer to the riddle", 1)
            {
            }

            public override bool IsObjective(Item item)
            {
                if (base.IsObjective(item))
                {
                    Pitcher pitcher = (Pitcher)item;
					
                    if (pitcher.Content == BeverageType.Water && !pitcher.IsEmpty)
                        return true;
                }
				
                return false;
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
				
                writer.WriteEncodedInt((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
				
                int version = reader.ReadEncodedInt();
            }
        }
    }
}