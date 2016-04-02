using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Aeluva : MondainQuester
    { 
        [Constructable]
        public Aeluva()
            : base("Aeluva", "the arcanist")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Aeluva(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] { typeof(PatienceQuest) };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8835;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x387;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new ElvenShirt());
            this.AddItem(new Skirt());
            this.AddItem(new Circlet());
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