using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class CrystallineFragmentsQuest : BaseQuest
    { 
        public CrystallineFragmentsQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(CrystallineFragments), "crystalline fragments", 10));
			
            this.AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* Crystalline Fragments */
        public override object Title
        {
            get
            {
                return 1073054;
            }
        }
        /* You look strong and brave, my friend.  Are you strong and brave?  I only ask because I am known 
        to be too generous to those that find for me interesting -- things -- to use in my smithing.  What 
        do you say? */
        public override object Description
        {
            get
            {
                return 1074662;
            }
        }
        /* *nods* */
        public override object Refuse
        {
            get
            {
                return 1074663;
            }
        }
        /* I can't be generous, my friend, until you bring me those crystalline fragments. */
        public override object Uncomplete
        {
            get
            {
                return 1074665;
            }
        }
        /* My friend, you've returned -- with items for me, I hope?  I have a generous reward for you. */
        public override object Complete
        {
            get
            {
                return 1074667;
            }
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

    public class ProtectorsEssenceQuest : BaseQuest
    { 
        public ProtectorsEssenceQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(ProtectorsEssence), "protector's essences", 5, 0x1ED1));
			
            this.AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* Protector's Essence */
        public override object Title
        {
            get
            {
                return 1073052;
            }
        }
        /* You look strong and brave, my friend.  Are you strong and brave?  I only ask because I am known 
        to be too generous to those that find for me interesting -- things -- to use in my smithing.  What 
        do you say? */
        public override object Description
        {
            get
            {
                return 1074662;
            }
        }
        /* *nods* */
        public override object Refuse
        {
            get
            {
                return 1074663;
            }
        }
        /* I can't be generous, my friend, until you bring me those essences. */
        public override object Uncomplete
        {
            get
            {
                return 1074664;
            }
        }
        /* My friend, you've returned -- with items for me, I hope?  I have a generous reward for you. */
        public override object Complete
        {
            get
            {
                return 1074667;
            }
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

    public class HeartOfIceQuest : BaseQuest
    { 
        public HeartOfIceQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(IcyHeart), "icy hearts", 6, 0x1CED));
			
            this.AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* Heart of Ice */
        public override object Title
        {
            get
            {
                return 1073056;
            }
        }
        /* You look strong and brave, my friend.  Are you strong and brave?  I only ask because I am known 
        to be too generous to those that find for me interesting -- things -- to use in my smithing.  What 
        do you say? */
        public override object Description
        {
            get
            {
                return 1074662;
            }
        }
        /* *nods* */
        public override object Refuse
        {
            get
            {
                return 1074663;
            }
        }
        /* I can't be generous, my friend, until you bring me those icy hearts. */
        public override object Uncomplete
        {
            get
            {
                return 1074666;
            }
        }
        /* My friend, you've returned -- with items for me, I hope?  I have a generous reward for you. */
        public override object Complete
        {
            get
            {
                return 1074667;
            }
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

    public class Neil : MondainQuester
    { 
        [Constructable]
        public Neil()
            : base("Neil", "the iron worker")
        { 
            this.SetSkill(SkillName.Blacksmith, 65.0, 88.0);
            this.SetSkill(SkillName.Fencing, 45.0, 68.0);
            this.SetSkill(SkillName.Macing, 45.0, 68.0);
            this.SetSkill(SkillName.Swords, 45.0, 68.0);
            this.SetSkill(SkillName.Tactics, 36.0, 68.0);
            this.SetSkill(SkillName.Parry, 61.0, 93.0);
        }

        public Neil(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(CrystallineFragmentsQuest),
                    typeof(ProtectorsEssenceQuest),
                    typeof(HeartOfIceQuest)
                };
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
            this.m_SBInfos.Add(new SBBlacksmith());
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83F5;
            this.HairItemID = 0x203C;
            this.HairHue = 0x46F;
            this.FacialHairItemID = 0x203F;
            this.FacialHairHue = 0x46F;
        }

        public override void InitOutfit()
        {
            this.AddItem(new SmithHammer());
            this.AddItem(new ShortPants(0x3A));
            this.AddItem(new Bandana(0x30));
            this.AddItem(new Doublet(0x13));
            this.AddItem(new RingmailChest());
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