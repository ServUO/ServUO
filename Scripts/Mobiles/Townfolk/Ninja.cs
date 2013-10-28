using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Ninja : BaseCreature
    {
        [Constructable]
        public Ninja()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Title = "the ninja";

            this.InitStats(100, 100, 25);

            this.SetSkill(SkillName.Fencing, 64.0, 80.0);
            this.SetSkill(SkillName.Macing, 64.0, 80.0);
            this.SetSkill(SkillName.Ninjitsu, 60.0, 80.0);
            this.SetSkill(SkillName.Parry, 64.0, 80.0);
            this.SetSkill(SkillName.Tactics, 64.0, 85.0);
            this.SetSkill(SkillName.Swords, 64.0, 85.0);

            this.SpeechHue = Utility.RandomDyedHue();

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

            if (!this.Female)
                this.AddItem(new LeatherNinjaHood());

            this.AddItem(new LeatherNinjaPants());
            this.AddItem(new LeatherNinjaBelt());
            this.AddItem(new LeatherNinjaJacket());
            this.AddItem(new NinjaTabi());

            int hairHue = Utility.RandomNondyedHue();

            Utility.AssignRandomHair(this, hairHue);

            if (Utility.Random(7) != 0)
                Utility.AssignRandomFacialHair(this, hairHue);

            this.PackGold(250, 300);
        }

        public Ninja(Serial serial)
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}