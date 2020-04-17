using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class CrystallineFragmentsQuest : BaseQuest
    {
        public CrystallineFragmentsQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(CrystallineFragments), "crystalline fragments", 10));

            AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* Crystalline Fragments */
        public override object Title => 1073054;
        /* You look strong and brave, my friend.  Are you strong and brave?  I only ask because I am known 
        to be too generous to those that find for me interesting -- things -- to use in my smithing.  What 
        do you say? */
        public override object Description => 1074662;
        /* *nods* */
        public override object Refuse => 1074663;
        /* I can't be generous, my friend, until you bring me those crystalline fragments. */
        public override object Uncomplete => 1074665;
        /* My friend, you've returned -- with items for me, I hope?  I have a generous reward for you. */
        public override object Complete => 1074667;
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

    public class ProtectorsEssenceQuest : BaseQuest
    {
        public ProtectorsEssenceQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(ProtectorsEssence), "protector's essences", 5, 0x1ED1));

            AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* Protector's Essence */
        public override object Title => 1073052;
        /* You look strong and brave, my friend.  Are you strong and brave?  I only ask because I am known 
        to be too generous to those that find for me interesting -- things -- to use in my smithing.  What 
        do you say? */
        public override object Description => 1074662;
        /* *nods* */
        public override object Refuse => 1074663;
        /* I can't be generous, my friend, until you bring me those essences. */
        public override object Uncomplete => 1074664;
        /* My friend, you've returned -- with items for me, I hope?  I have a generous reward for you. */
        public override object Complete => 1074667;
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

    public class HeartOfIceQuest : BaseQuest
    {
        public HeartOfIceQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(IcyHeart), "icy hearts", 6, 0x1CED));

            AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* Heart of Ice */
        public override object Title => 1073056;
        /* You look strong and brave, my friend.  Are you strong and brave?  I only ask because I am known 
        to be too generous to those that find for me interesting -- things -- to use in my smithing.  What 
        do you say? */
        public override object Description => 1074662;
        /* *nods* */
        public override object Refuse => 1074663;
        /* I can't be generous, my friend, until you bring me those icy hearts. */
        public override object Uncomplete => 1074666;
        /* My friend, you've returned -- with items for me, I hope?  I have a generous reward for you. */
        public override object Complete => 1074667;
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

    public class Neil : MondainQuester
    {
        [Constructable]
        public Neil()
            : base("Neil", "the iron worker")
        {
            SetSkill(SkillName.Blacksmith, 65.0, 88.0);
            SetSkill(SkillName.Fencing, 45.0, 68.0);
            SetSkill(SkillName.Macing, 45.0, 68.0);
            SetSkill(SkillName.Swords, 45.0, 68.0);
            SetSkill(SkillName.Tactics, 36.0, 68.0);
            SetSkill(SkillName.Parry, 61.0, 93.0);
        }

        public Neil(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(CrystallineFragmentsQuest),
                    typeof(ProtectorsEssenceQuest),
                    typeof(HeartOfIceQuest)
                };
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBlacksmith());
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83F5;
            HairItemID = 0x203C;
            HairHue = 0x46F;
            FacialHairItemID = 0x203F;
            FacialHairHue = 0x46F;
        }

        public override void InitOutfit()
        {
            AddItem(new SmithHammer());
            AddItem(new ShortPants(0x3A));
            AddItem(new Bandana(0x30));
            AddItem(new Doublet(0x13));
            AddItem(new RingmailChest());
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