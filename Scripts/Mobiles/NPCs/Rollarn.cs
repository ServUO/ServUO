using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class BrotherlyLoveQuest : BaseQuest
    {
        public BrotherlyLoveQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(PersonalLetterAhie), "letter", 1, typeof(Ahie), "Ahie (The Heartwood)", 1800));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        public override bool DoneOnce => true;
        /* Brotherly Love */
        public override object Title => 1072369;
        /* *looks around nervously*  Do you travel to The Heartwood?  I have an urgent letter that must be delivered 
        there in the next 30 minutes - to Ahie the Cloth Weaver.  Will you undertake this journey? */
        public override object Description => 1072585;
        /* *looks disappointed* Let me know if you change your mind. */
        public override object Refuse => 1072587;
        /* You haven't lost the letter have you?  It must be delivered to Ahie directly.  Give it into no other hands. */
        public override object Uncomplete => 1072588;
        /* Yes, can I help you? */
        public override object Complete => 1074579;
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

        public override Type[] Quests => new Type[]
                {
                    typeof(DaemonicPrismQuest),
                    typeof(HowManyHeadsQuest),
                    typeof(GlassyFoeQuest),
                    typeof(HailstormQuest),
                    typeof(WarriorsOfTheGemkeeperQuest),
                    typeof(BrotherlyLoveQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Elf;

            Hue = 0x84DE;
            HairItemID = 0x2FC1;
            HairHue = 0x320;
        }

        public override void InitOutfit()
        {
            AddItem(new Shoes(0x1BB));
            AddItem(new Circlet());
            AddItem(new Cloak(0x296));
            AddItem(new LeafChest());
            AddItem(new LeafArms());

            Item item;

            item = new LeafLegs
            {
                Hue = 0x74E
            };
            AddItem(item);
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

        public override int LabelNumber => 1073128;// A personal letter addressed to: Ahie
        public override int Lifespan => 1800;
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