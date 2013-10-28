using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class Tamm : MondainQuester
    { 
        [Constructable]
        public Tamm()
            : base("Tamm", "the guard")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Tamm(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheyreBreedingLikeRabbitsQuest),
                    typeof(ThinningTheHerdQuest),
                    typeof(TheyllEatAnythingQuest),
                    typeof(NoGoodFishStealingQuest),
                    typeof(HeroInTheMakingQuest),
                    typeof(WildBoarCullQuest),
                    typeof(ForcedMigrationQuest),
                    typeof(BullfightingSortOfQuest),
                    typeof(FineFeastQuest),
                    typeof(OverpopulationQuest),
                    typeof(DeadManWalkingQuest),
                    typeof(ForkedTonguesQuest),
                    typeof(TrollingForTrollsQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8353;
            this.HairItemID = 0x2FBF;
            this.HairHue = 0x386;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new ElvenCompositeLongbow());
            this.AddItem(new HidePants());
            this.AddItem(new HidePauldrons());
            this.AddItem(new HideChest());
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