using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Vilo : MondainQuester
    {
        [Constructable]
        public Vilo()
            : base("Vilo", "the guard")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Vilo(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
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
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x8579;
            HairItemID = 0x2FC0;
            HairHue = 0x389;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x901));
            AddItem(new WoodlandBelt(0x592));
            AddItem(new WoodlandChest());
            AddItem(new WoodlandGloves());
            AddItem(new WoodlandLegs());
            AddItem(new WoodlandGorget());
            AddItem(new WoodlandArms());
            AddItem(new VultureHelm());
            AddItem(new OrnateAxe());
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