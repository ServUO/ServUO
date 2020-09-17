using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Necromancer : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Necromancer()
            : base("the Necromancer")
        {
            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Inscribe, 80.0, 100.0);
            SetSkill(SkillName.Necromancy, 80.0, 100.0);
            SetSkill(SkillName.Meditation, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);

            Hue = 0x3C6;
        }

        public Necromancer(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBNecromancer());
        }


        public override void InitOutfit()
        {
            base.InitOutfit();
            AddItem(new Items.Shoes(0x151));
            AddItem(new Items.Robe(0x455));
            AddItem(new Items.FancyShirt(0x455));

            Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A))
            {
                Hue = 0x3c6,
                Layer = Layer.Hair,
                Movable = false
            };
            AddItem(hair);

            Item beard = new Item(0x0)
            {
                Layer = Layer.FacialHair
            };
            AddItem(beard);
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