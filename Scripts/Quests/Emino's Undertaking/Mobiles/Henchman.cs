using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class Henchman : BaseCreature
    {
        [Constructable]
        public Henchman()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.InitStats(45, 30, 5);

            this.Hue = Utility.RandomSkinHue();
            this.Body = 0x190;
            this.Name = "a henchman";

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);

            this.AddItem(new LeatherNinjaJacket());
            this.AddItem(new LeatherNinjaPants());
            this.AddItem(new NinjaTabi());

            if (Utility.RandomBool())
                this.AddItem(new Kama());
            else
                this.AddItem(new Tessen());

            this.SetSkill(SkillName.Swords, 50.0);
            this.SetSkill(SkillName.Tactics, 50.0);
        }

        public Henchman(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
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