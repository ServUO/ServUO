using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class TheKingOfClothingQuest : BaseQuest
    { 
        public TheKingOfClothingQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Kilt), "kilts", 10, 0x1537));
			
            this.AddReward(new BaseReward(typeof(TailorsCraftsmanSatchel), 1074282));
        }

        /* The King of Clothing */
        public override object Title
        {
            get
            {
                return 1073902;
            }
        }
        /* I have heard noble tales of a fine and proud human garment. An article of clothing 
        fit for both man and god alike. It is called a "kilt" I believe? Could you fetch for 
        me some of these kilts so I that I might revel in their majesty and glory? */
        public override object Description
        {
            get
            {
                return 1074092;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me kilts. */
        public override object Uncomplete
        {
            get
            {
                return 1073948;
            }
        }
        /* I say truly - that is a magnificent garment! You have more than earned a reward. */
        public override object Complete
        {
            get
            {
                return 1073974;
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

    public class ThePuffyShirtQuest : BaseQuest
    { 
        public ThePuffyShirtQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(FancyShirt), "fancy shirts", 10, 0x1EFD));
			
            this.AddReward(new BaseReward(typeof(TailorsCraftsmanSatchel), 1074282));
        }

        /* The Puffy Shirt */
        public override object Title
        {
            get
            {
                return 1073903;
            }
        }
        /* We elves believe that beauty is expressed in all things, including the garments we 
        wear. I wish to understand more about human aesthetics, so please kind traveler - could 
        you bring to me magnificent examples of human fancy shirts? For my thanks, I could teach 
        you more about the beauty of elven vestements. */
        public override object Description
        {
            get
            {
                return 1074093;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me fancy shirts. */
        public override object Uncomplete
        {
            get
            {
                return 1073949;
            }
        }
        /* I appreciate your service. Now, see what elven hands can create. */
        public override object Complete
        {
            get
            {
                return 1073973;
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

    public class FromTheGaultierCollectionQuest : BaseQuest
    { 
        public FromTheGaultierCollectionQuest()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(StuddedBustierArms), "studded bustiers", 10, 0x1C0C));
			
            this.AddReward(new BaseReward(typeof(TailorsCraftsmanSatchel), 1074282));
        }

        /* From the Gaultier Collection */
        public override object Title
        {
            get
            {
                return 1073905;
            }
        }
        /* It is my understanding, the females of humankind actually wear on certain occasions a 
        studded bustier? This is not simply a fanciful tale? Remarkable! It sounds hideously 
        uncomfortable as well as ludicrously impracticle. But perhaps, I simply do not understand 
        the nuances of human clothing. Perhaps, I need to see such a studded bustier for myself? */
        public override object Description
        {
            get
            {
                return 1074095;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me studded bustiers. */
        public override object Uncomplete
        {
            get
            {
                return 1073951;
            }
        }
        /* Truly, it is worse than I feared. Still, I appreciate your efforts on my behalf. */
        public override object Complete
        {
            get
            {
                return 1073976;
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

    public class HuteCoutureQuest : BaseQuest
    { 
        public HuteCoutureQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(FlowerGarland), "flower garlands", 10, 0x2306));
			
            this.AddReward(new BaseReward(typeof(TailorsCraftsmanSatchel), 1074282));
        }

        /* H'ute Couture */
        public override object Title
        {
            get
            {
                return 1073901;
            }
        }
        /* Most human apparel is interesting to elven eyes. But there is one garment - the flower garland - 
        which sounds very elven indeed. Could I see how a human crafts such an object of beauty? In exchange, 
        I could share with you the wonders of elven garments. */
        public override object Description
        {
            get
            {
                return 1074091;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me flower garlands. */
        public override object Uncomplete
        {
            get
            {
                return 1073947;
            }
        }
        /* I appreciate your service. Now, see what elven hands can create. */
        public override object Complete
        {
            get
            {
                return 1073973;
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

    public class Ahie : MondainQuester
    {
        [Constructable]
        public Ahie()
            : base("Ahie", "the cloth weaver")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Ahie(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheKingOfClothingQuest),
                    typeof(ThePuffyShirtQuest),
                    typeof(FromTheGaultierCollectionQuest),
                    typeof(HuteCoutureQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x853F;
            this.HairItemID = 0x2FCD;
            this.HairHue = 0x90;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ThighBoots(0x901));
            this.AddItem(new FancyShirt(0x72B));
            this.AddItem(new Cloak(0x1C));
            this.AddItem(new Skirt(0x62));
            this.AddItem(new Circlet());
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