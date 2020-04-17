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
            this.Name = "Grandpa Charley";

            this.SetSkill(SkillName.ItemID, 70.0, 80.0);
        }

        public GrandpaCharley(Serial serial)
            : base(serial)
        {
        }

        public override bool IsActiveVendor => false;
        public override bool CanTeach => true;
        public override bool IsInvulnerable => true;
        protected override List<SBInfo> SBInfos => this.m_SBInfos;
        public override void InitSBInfo()
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.CantWalk = false;
            this.Race = Race.Human;

            this.Hue = 0x8410;
            this.HairItemID = 0x203B;
            this.HairHue = 0x3B2;
            this.FacialHairItemID = 0x203E;
            this.FacialHairHue = 0x3B2;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new ShepherdsCrook());
            this.AddItem(new Shoes(0x72F));
            this.AddItem(new LongPants(0x519));
            this.AddItem(new FancyShirt(0x600));
            this.AddItem(new WideBrimHat(0x6B1));
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