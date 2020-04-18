using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class WarriorCasteQuest : BaseQuest
    {
        public WarriorCasteQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(TerathanWarrior), "terathan warriors", 10));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Warrior Caste */
        public override object Title => 1073078;
        /* The Terathan are an aggressive species. Left unchecked, they will swarm across our lands. 
        And where will that leave us? Compost in the hive, that's what! Stop them, stop them cold my 
        friend. Kill their warriors and you'll check their movement, that is certain. */
        public override object Description => 1073568;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* Unless you kill at least 10 Terathan Warriors, you won't have any impact on their hive. */
        public override object Uncomplete => 1073588;
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

    public class BigWormsQuest : BaseQuest
    {
        public BigWormsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(IceSerpent), "giant ice serpents", 10));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Big Worms */
        public override object Title => 1073088;
        /* It makes no sense! Cold blooded serpents cannot live in the ice! It's a biological impossibility! 
        They are an abomination against reason! Please, I beg you - kill them! Make them disappear for me! Do 
        this and I will reward you. */
        public override object Description => 1073578;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* You wouldn't try and just pretend you murdered 10 Giant Ice Worms, would you? */
        public override object Uncomplete => 1073598;
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

    public class OrcishEliteQuest : BaseQuest
    {
        public OrcishEliteQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(OrcBomber), "orc bombers", 6));
            AddObjective(new SlayObjective(typeof(OrcCaptain), "orc captain", 4));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Orcish Elite */
        public override object Title => 1073081;
        /* Foul brutes! No one loves an orc, but some of them are worse than the rest. Their Captains and their 
        Bombers, for instance, they're the worst of the lot. Kill a few of those, and the rest are just a rabble. 
        Exterminate a few of them and you'll make the world a sunnier place, don't you know. */
        public override object Description => 1073571;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* The only good orc is a dead orc - and 4 dead Captains and 6 dead Bombers is even better! */
        public override object Uncomplete => 1073591;
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

    public class Caelas : MondainQuester
    {
        [Constructable]
        public Caelas()
            : base("Elder Caelas", "the wise")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Caelas(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(WarriorCasteQuest),
                    typeof(BigWormsQuest),
                    typeof(ItsElementalQuest),
                    typeof(OrcishEliteQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x8381;
            HairItemID = 0x2FC0;
            HairHue = 0x2C8;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x1BB));
            AddItem(new Cloak(0x71B));
            AddItem(new RoyalCirclet());
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