using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class DreadhornQuest : BaseQuest
    {
        public DreadhornQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(DreadHorn), "dread horn", 1));

            AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* Dreadhorn */
        public override object Title => 1074645;
        /* Can you comprehend it? I cannot, I confess.  The most pristine and perfect Lord of Sosaria 
        has fallen prey to the blight.  From the depths of my heart I mourn his corruption; my thoughts 
        are filled with pity for this glorious creature now tainted.  And my blood boils with fury at 
        those responsible for the innocent creature's undoing.  Will you find Dread Horn, as he is now 
        called, and free him from this misery? */
        public override object Description => 1074646;
        /* How can you not feel as I do? */
        public override object Refuse => 1074647;
        /* The lush and fertile land where Dread Horn now lives is twisted and tainted, a result of his 
        corruption.  The fey folk have sealed the land off through their magics, but you can enter through 
        an enchanted mushroom fairy circle. */
        public override object Uncomplete => 1074648;
        /* Thank you.  I haven't the words to express my gratitude. */
        public override object Complete => 1074649;
        public override bool CanOffer()
        {
            return MondainsLegacy.TwistedWeald;
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

    public class Calendor : MondainQuester
    {
        [Constructable]
        public Calendor()
            : base("Lorekeeper Calendor", "the keeper of tradition")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Calendor(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(DreadhornQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x847E;
            HairItemID = 0x2FD0;
            HairHue = 0x1F2;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x65A));
            AddItem(new ElvenShirt(0x728));
            AddItem(new Kilt(0x1BB));
            AddItem(new RoyalCirclet());
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