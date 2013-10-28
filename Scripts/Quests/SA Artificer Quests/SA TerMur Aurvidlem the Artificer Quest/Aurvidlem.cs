using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Aurvidlem : MondainQuester
    {
        [Constructable]
        public Aurvidlem()
            : base("Aurvidlem", "the Artificer")
        { 
            this.SetSkill(SkillName.Imbuing, 60.0, 80.0);
        }

        public Aurvidlem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(KnowledgeoftheSoulforge)
                };
            }
        }
        public override void InitBody()
        { 
            this.HairItemID = 0x2044;//
            this.HairHue = 1153;
            this.FacialHairItemID = 0x204B;
            this.FacialHairHue = 1153;
            this.Body = 666;            
            this.Blessed = true;
        }

        public override void InitOutfit()
        { 
            this.AddItem(new Backpack());		
            this.AddItem(new Boots());
            this.AddItem(new LongPants(0x6C7));
            this.AddItem(new FancyShirt(0x6BB));
            this.AddItem(new Cloak(0x59));		
        }

        public override void Advertise()
        {
            this.Say(1112525);  // Come to be Artificer. I have a task for you. 
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