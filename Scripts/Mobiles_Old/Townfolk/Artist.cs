using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Artist : BaseCreature
    {
        [Constructable]
        public Artist()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.InitStats(31, 41, 51);

            this.SetSkill(SkillName.Healing, 36, 68);

            this.SpeechHue = Utility.RandomDyedHue();
            this.Title = "the artist";
            this.Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
            }
            this.AddItem(new Doublet(Utility.RandomDyedHue()));
            this.AddItem(new Sandals(Utility.RandomNeutralHue()));
            this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            this.AddItem(new HalfApron(Utility.RandomDyedHue()));

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            pack.Movable = false;

            this.AddItem(pack);
        }

        public Artist(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
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