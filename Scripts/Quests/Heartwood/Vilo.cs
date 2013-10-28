using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Vilo : MondainQuester
    { 
        [Constructable]
        public Vilo()
            : base("Vilo", "the guard")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Vilo(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(IndustriousAsAnAntLionQuest),
                    typeof(ChillInTheAirQuest),
                    typeof(TaleOfTailQuest),
                    typeof(DeadManWalkingQuest),
                    typeof(SpecimensQuest),
                    typeof(FriendlyNeighborhoodSpiderkillerQuest),
                    typeof(ItsGhastlyJobQuest),
                    typeof(UnholyConstructQuest),
                    typeof(KingOfBearsQuest),
                    typeof(UnholyKnightsQuest),
                    typeof(FeatherInYerCapQuest),
                    typeof(RollTheBonesQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8579;
            this.HairItemID = 0x2FC0;
            this.HairHue = 0x389;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new WoodlandBelt(0x592));
            this.AddItem(new WoodlandChest());
            this.AddItem(new WoodlandGloves());
            this.AddItem(new WoodlandLegs());
            this.AddItem(new WoodlandGorget());
            this.AddItem(new WoodlandArms());			
            this.AddItem(new VultureHelm());
            this.AddItem(new OrnateAxe());			
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