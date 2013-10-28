using System;
using Server.Items;

namespace Server.Mobiles
{
    public class HarborMaster : BaseCreature
    {
        [Constructable]
        public HarborMaster()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.InitStats(31, 41, 51);

            this.SetSkill(SkillName.Mining, 36, 68);

            this.SpeechHue = Utility.RandomDyedHue();
            this.Hue = Utility.RandomSkinHue();
            this.Blessed = true;

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
                this.Title = "the Harbor Mistress";
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                this.Title = "the Harbor Master";
            }
            this.AddItem(new Shirt(Utility.RandomDyedHue()));
            this.AddItem(new Boots());
            this.AddItem(new LongPants(Utility.RandomNeutralHue()));
            this.AddItem(new QuarterStaff());

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            pack.Movable = false;

            this.AddItem(pack);
        }

        public HarborMaster(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return false;
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