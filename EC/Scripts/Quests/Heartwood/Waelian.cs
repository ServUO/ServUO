using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Waelian : MondainQuester
    { 
        [Constructable]
        public Waelian()
            : base("Waelian", "the trinket weaver")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Waelian(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ArchSupportQuest),
                    typeof(StopHarpingOnMeQuest),
                    typeof(TheFarEyeQuest),
                    typeof(NecessitysMotherQuest),
                    typeof(TickTockQuest),
                    typeof(FromTheGaultierCollectionQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8835;
            this.HairItemID = 0x2FBF;
            this.HairHue = 0x2C2;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x901));
            this.AddItem(new GemmedCirclet());
            this.AddItem(new LongPants(0x340));
            this.AddItem(new SmithHammer());
			
            Item item;
			
            item = new LeafChest();
            item.Hue = 0x344;
            this.AddItem(item);
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