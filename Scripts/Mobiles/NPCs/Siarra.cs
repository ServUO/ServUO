using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class AllThatGlittersIsNotGoodQuest : BaseQuest
    {
        public AllThatGlittersIsNotGoodQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(ShimmeringEffusion), "shimmering effusion", 10));

            AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* All That Glitters is Not Good */
        public override object Title => 1073048;
        /* The most incredible tale has reached my ears!  Deep within the bowels of Sosaria, somewhere under the city of 
        Nu'Jelm, a twisted creature feeds.  What created this abomination, no one knows ... though there is some speculation 
        that the fumbling initial efforts to open the portal to The Heartwood, brought it into existence.  Regardless of it's 
        origin, it must be destroyed before it damages Sosaria.  Will you undertake this quest?  */
        public override object Description => 1074654;
        /* Perhaps I thought too highly of you. */
        public override object Refuse => 1074655;
        /* An explorer discovered the cave system under Nu'Jelm.  He made multiple trips into the place bringing back fascinating 
        crystals and artifacts that suggested the hollow place in Sosaria was inhabited by other creatures at some point.  You'll 
        need to follow in his footsteps to find this abomination and destroy it. */
        public override object Uncomplete => 1074656;
        /* I am overjoyed with your efforts!  Your devotion to Sosaria is noted and appreciated. */
        public override object Complete => 1074657;
        public override bool CanOffer()
        {
            return MondainsLegacy.PrismOfLight;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
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
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Siarra(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(AllThatGlittersIsNotGoodQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x8384;
            HairItemID = 0x2FC1;
            HairHue = 0x33;
        }

        public override void InitOutfit()
        {
            AddItem(new Sandals(0x1BB));
            AddItem(new LeafTonlet());
            AddItem(new ElvenShirt());
            AddItem(new GemmedCirclet());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}