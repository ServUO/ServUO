using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Unoelil : MondainQuester
    { 
        [Constructable]
        public Unoelil()
            : base("Unoelil", "the bark weaver")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Unoelil(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(StopHarpingOnMeQuest),
                    typeof(TheFarEyeQuest),
                    typeof(NothingFancyQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8362;
            this.HairItemID = 0x2FCD;
            this.HairHue = 0x31D;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x1BB));
            this.AddItem(new Tunic(0x64F));
            this.AddItem(new ShortPants(0x1BB));
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