using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class BrotherlyLoveQuest : BaseQuest
    { 
        public BrotherlyLoveQuest()
            : base()
        { 
            this.AddObjective(new DeliverObjective(typeof(PersonalLetterAhie), "letter", 1, typeof(Ahie), "Ahie (The Heartwood)", 1800));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* Brotherly Love */
        public override object Title
        {
            get
            {
                return 1072369;
            }
        }
        /* *looks around nervously*  Do you travel to The Heartwood?  I have an urgent letter that must be delivered 
        there in the next 30 minutes - to Ahie the Cloth Weaver.  Will you undertake this journey? */
        public override object Description
        {
            get
            {
                return 1072585;
            }
        }
        /* *looks disappointed* Let me know if you change your mind. */
        public override object Refuse
        {
            get
            {
                return 1072587;
            }
        }
        /* You haven't lost the letter have you?  It must be delivered to Ahie directly.  Give it into no other hands. */
        public override object Uncomplete
        {
            get
            {
                return 1072588;
            }
        }
        /* Yes, can I help you? */
        public override object Complete
        {
            get
            {
                return 1074579;
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

    public class Rollarn : MondainQuester
    { 
        [Constructable]
        public Rollarn()
            : base("Lorekeeper Rollarn", "the keeper of tradition")
        { 
        }

        public Rollarn(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(DaemonicPrismQuest),
                    typeof(HowManyHeadsQuest),
                    typeof(GlassyFoeQuest),
                    typeof(HailstormQuest),
                    typeof(WarriorsOfTheGemkeeperQuest),
                    typeof(BrotherlyLoveQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x84DE;
            this.HairItemID = 0x2FC1;
            this.HairHue = 0x320;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Shoes(0x1BB));
            this.AddItem(new Circlet());
            this.AddItem(new Cloak(0x296));
            this.AddItem(new LeafChest());
            this.AddItem(new LeafArms());
			
            Item item;
			
            item = new LeafLegs();
            item.Hue = 0x74E;
            this.AddItem(item);
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

    public class PersonalLetterAhie : BaseQuestItem
    {
        [Constructable]
        public PersonalLetterAhie()
            : base(0x14ED)
        {
        }

        public PersonalLetterAhie(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073128;
            }
        }// A personal letter addressed to: Ahie
        public override int Lifespan
        {
            get
            {
                return 1800;
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