using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class GrandpaCharley : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public GrandpaCharley()
            : base("the farmer")
        {
            Name = "Grandpa Charley";

            SetSkill(SkillName.ItemID, 70.0, 80.0);
        }

        public GrandpaCharley(Serial serial)
            : base(serial)
        {
        }

        public override bool IsActiveVendor => false;
        public override bool CanTeach => true;
        public override bool IsInvulnerable => true;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = false;
            Race = Race.Human;

            Hue = 0x8410;
            HairItemID = 0x203B;
            HairHue = 0x3B2;
            FacialHairItemID = 0x203E;
            FacialHairHue = 0x3B2;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new ShepherdsCrook());
            AddItem(new Shoes(0x72F));
            AddItem(new LongPants(0x519));
            AddItem(new FancyShirt(0x600));
            AddItem(new WideBrimHat(0x6B1));
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