using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class GuiltyQuest : BaseQuest
    {
        public GuiltyQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Gregorio), "gregorio", 1));

            AddReward(new BaseReward(typeof(AmuletOfRighteousness), 1075313)); // Amulet of Righteousness
        }

        public override bool DoneOnce => true;
        /* Guilty */
        public override object Title => 1075311;
        /* I was born and raised in Buc’s Den. Have you been there? Then you know what a lawless place it is. Five years 
        ago, my mother and father were killed by brigands there. I was only a child, and so I was sent here to live with 
        my uncle.<br>Recently, a man moved into town, and I’m sure this man is the one who killed my parents. The sheriff 
        will do nothing, he doesn’t believe me. What can I do? I’m no warrior. I can’t use magic. I don’t have the courage 
        to avenge my family. Please, if you have any love at all for justice, I beg of you, seek out and slay this criminal! */
        public override object Description => 1075312;
        /* Why? This man has killed many innocent people. Would you let him walk around, free? */
        public override object Refuse => 1075314;
        /* This brigand, Gregorio, you can find him in the farmland to the east of town. He usually wears a red skull cap 
        and carries a pitchfork.  */
        public override object Uncomplete => 1075315;
        /* He is dead, then? Good! I can think of no one more deserving. Thank you for your help. I feel that a great weight 
        has been lifted from me. Here, I would like you to take this necklace. It belonged to my mother. She said it has some 
        magic, but I have never been able to discover how it works. Perhaps you can. */
        public override object Complete => 1075316;
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

    public class Natalie : MondainQuester
    {
        [Constructable]
        public Natalie()
            : base("Natalie", "the lady of Skara Brae")
        {
        }

        public Natalie(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(GuiltyQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x8400;
            HairItemID = 0x2045;
            HairHue = 0x740;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x727));
            AddItem(new FancyShirt(0x53C));
            AddItem(new Skirt(0x534));
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