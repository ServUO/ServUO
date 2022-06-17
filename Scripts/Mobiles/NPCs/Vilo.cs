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
            SetWearable(new ElvenBoots(), 0x901, 1);
            SetWearable(new WoodlandBelt(), 0x592, 1);
            SetWearable(new WoodlandChest(), dropChance: 1);
            SetWearable(new WoodlandGloves(), dropChance: 1);
            SetWearable(new WoodlandLegs(), dropChance: 1);
            SetWearable(new WoodlandGorget(), dropChance: 1);
            SetWearable(new WoodlandArms(), dropChance: 1);
            SetWearable(new VultureHelm(), dropChance: 1);
			SetWearable(new OrnateAxe(), dropChance: 1);
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