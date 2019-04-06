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
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
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
            InitStats(100, 100, 25);
			
            Female = false;
            Race = Race.Elf;
			
            Hue = 0x8835;
            HairItemID = 0x2FBF;
            HairHue = 0x2C2;
        }

        public override void InitOutfit()
        {
            AddItem(new Sandals(0x901));
            AddItem(new GemmedCirclet());
            AddItem(new LongPants(0x340));
            AddItem(new SmithHammer());
			
            Item item;
			
            item = new LeafChest();
            item.Hue = 0x344;
            AddItem(item);
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