using Server.Items;
using System;

namespace Server.Engines.Quests
{

    public class PeptaQuest : BaseQuest
    {
        public PeptaQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(FreshGinger), "Fresh Ginger", 1, 0x2BE3));
            AddObjective(new ObtainObjective(typeof(TribalBerry), "Tribal Berries", 2, 0x9D0));

            AddReward(new BaseReward(typeof(SatietyCure), 1080542));
        }

        /* I Think I Overate */
        public override object Title => 1080543;
        /* Arghhhh... *stomach gurgles* I think I ate too much... Oh no! I have to make sure the rest of the food is alright!
           Ever since the Council was assassinated *gurgle* I've had to taste everything that comes through this kitchen. 
           Will you help me? I need to make some of my 'Satiety Cure.' If you find the ingredients I'll make you some too!  */
        public override object Description => 1080544;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You don't have the ingredients yet?  Please, I need them soon!  Fresh Ginger should be available from a farmer if you
           ask and I hear that Tribal Berries are used by some of the savages for their ceremonies! *gurk*  More food is on its
           way, so hurry! */
        public override object Uncomplete => 1071183;
        /* Thanks for helping me out.  Here's the reward I promised you.*/
        public override object Complete => 1072272;
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

    public class Pepta : MondainQuester
    {
        [Constructable]
        public Pepta()
            : base("Pepta", "The Royal Tastetester")
        {
        }

        public Pepta(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(PeptaQuest)
                };
        public override void InitBody()
        {
            Female = true;
            Race = Race.Human;

            Hue = 0x83F4;
            HairItemID = 0x2049;
            HairHue = 0x46A;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x346));
            AddItem(new PlainDress(0x27B));
            AddItem(new HalfApron());
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