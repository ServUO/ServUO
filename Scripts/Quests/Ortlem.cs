using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class MysticsJourneyQuest : BaseQuest
    {
        /* A Mystic's Journey */
        public override object Title => 1112561;

        /* Obtain the following Mysticism reagents: Dragon's Blood and Daemon Bone - and give
		 * them to Ortlem for your reward.<br><center>------</center><br>I am called Ortlem.
		 * I am a Mystic. Mysticism is our way of magic. I need you to obtain the following
		 * Mysticism reagents: Dragon's Blood and Daemon Bone.<BR><BR>These reagents will
		 * allow our Mystics to cast powerful spells to protect Ter-Mur. You can gather
		 * Dragon's Blood and Daemon Bones in Ter-Mur. Dragon's Blood is gathered by skinning
		 * reptilian creatures. Daemon Bones will appear on the corpse of any void demons
		 * that you slay.<BR><BR>Return to me with these reagents, and I will reward you. */
        public override object Description => 1112563;

        /* I understand your fear. I wish you no harm. If I must, I am willing to find
		 * another to help us. */
        public override object Refuse => 1112564;

        /* Good to see you, again. I am glad no harm has come to you in collecting the four
		 * rare reagents of Mysticism. Please obtain them soon. They are important to the
		 * protection of Ter-Mur. */
        public override object Uncomplete => 1112565;

        /* I appreciate your work in collecting these reagents. I am glad to be able to
		 * count on you. Be assured that these reagents will help continue the protection
		 * of Ter-Mur. As promised, here is your reward. */
        public override object Complete => 1112566;

        public MysticsJourneyQuest()
        {
            AddObjective(new ObtainObjective(typeof(DragonBlood), "Dragon's Blood", 100, 0x4077));
            AddObjective(new ObtainObjective(typeof(DaemonBone), "Daemon Bone", 100, 0xF80));

            AddReward(new BaseReward(1112530));
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            Owner.SendLocalizedMessage(1074360, "#1112568"); // You receive a reward: Crystal Ball of Knowledge
            Owner.AddToBackpack(new CrystalBallOfKnowledge());
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

    public class Ortlem : MondainQuester
    {
        private static readonly Type[] m_Quests = { typeof(MysticsJourneyQuest) };
        public override Type[] Quests => m_Quests;

        public override bool IsActiveVendor => true;

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBMystic());
        }

        [Constructable]
        public Ortlem()
            : base("Ortlem", "the Mystic")
        {
            SetSkill(SkillName.EvalInt, 65.0, 90.0);
            SetSkill(SkillName.Meditation, 65.0, 90.0);
            SetSkill(SkillName.MagicResist, 65.0, 90.0);
            SetSkill(SkillName.Mysticism, 65.0, 90.0);
        }

        public Ortlem(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1112562); // Become adept in Mysticism. Help save Ter-Mur!
        }

        public override void InitBody()
        {
            Female = false;
            CantWalk = true;
            Race = Race.Gargoyle;

            Hue = 0x86ED;
            HairItemID = 0x4258;
            HairHue = 0x38A;
        }

        public override void InitOutfit()
        {
            AddItem(new GlassStaff());
            AddItem(new GargishClothChest(0x64F));
            AddItem(new GargishClothArms(0x64F));
            AddItem(new GargishClothKilt(0x643));
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
