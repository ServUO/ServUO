using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Kevin : HumilityQuestMobile
    {
        public override int Greeting { get { return 1075759; } }

        public override bool IsActiveVendor { get { return true; } }
        public override bool CanTeach { get { return true; } }

        [Constructable]
        public Kevin()
            : base("Kevin", "the butcher")
        {
            SetSkill(SkillName.Anatomy, 45.0, 68.0);
        }

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBButcher());
        }

        public Kevin(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.Race = Race.Human;
            this.Body = 0x190;

            this.Hue = Race.RandomSkinHue();
            this.HairItemID = Race.RandomHair(false);
            this.HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Server.Items.HalfApron());
            AddItem(new Server.Items.Cleaver());
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