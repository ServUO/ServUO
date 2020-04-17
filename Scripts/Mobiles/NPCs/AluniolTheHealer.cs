using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Aluniol : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Aluniol()
            : base("the healer")
        {
            Name = "Aluniol";
        }

        public Aluniol(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => false;
        public override bool IsInvulnerable => true;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x8383;
            HairItemID = 0x2FBF;
            HairHue = 0x323;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x1BB));
            AddItem(new MaleElvenRobe(0x47E));
            AddItem(new WildStaff());
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