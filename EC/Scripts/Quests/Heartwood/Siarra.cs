using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class AllThatGlittersIsNotGoodQuest : BaseQuest
    { 
        public AllThatGlittersIsNotGoodQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(ShimmeringEffusion), "shimmering effusion", 10));
			
            this.AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* All That Glitters is Not Good */
        public override object Title
        {
            get
            {
                return 1073048;
            }
        }
        /* The most incredible tale has reached my ears!  Deep within the bowels of Sosaria, somewhere under the city of 
        Nu'Jelm, a twisted creature feeds.  What created this abomination, no one knows ... though there is some speculation 
        that the fumbling initial efforts to open the portal to The Heartwood, brought it into existence.  Regardless of it's 
        origin, it must be destroyed before it damages Sosaria.  Will you undertake this quest?  */
        public override object Description
        {
            get
            {
                return 1074654;
            }
        }
        /* Perhaps I thought too highly of you. */
        public override object Refuse
        {
            get
            {
                return 1074655;
            }
        }
        /* An explorer discovered the cave system under Nu'Jelm.  He made multiple trips into the place bringing back fascinating 
        crystals and artifacts that suggested the hollow place in Sosaria was inhabited by other creatures at some point.  You'll 
        need to follow in his footsteps to find this abomination and destroy it. */
        public override object Uncomplete
        {
            get
            {
                return 1074656;
            }
        }
        /* I am overjoyed with your efforts!  Your devotion to Sosaria is noted and appreciated. */
        public override object Complete
        {
            get
            {
                return 1074657;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.PrismOfLight;
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

    public class Siarra : MondainQuester
    { 
        [Constructable]
        public Siarra()
            : base("Lorekeeper Siarra", "the keeper of tradition")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Siarra(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(AllThatGlittersIsNotGoodQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8384;
            this.HairItemID = 0x2FC1;
            this.HairHue = 0x33;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x1BB));
            this.AddItem(new LeafTonlet());
            this.AddItem(new ElvenShirt());
            this.AddItem(new GemmedCirclet());
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