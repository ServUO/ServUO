using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class GoneNativeQuest : BaseQuest
    { 
        public GoneNativeQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(MasterTheophilus), "master theophilus", 1));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Gone Native */
        public override object Title
        {
            get
            {
                return 1074855;
            }
        }
        /* Pathetic really.  I must say, a senior instructor going native -- forgetting about his students and 
        peers and engaging in such disgraceful behavior!  I'm speaking, of course, of Theophilus.  Master Theophilus 
        to you. He may have gone native but he still holds a Mastery Degree from Bedlam College!  But, well, that's 
        neither here nor there.  I need you to take care of my colleague.  Convince him of the error of his ways.  
        He may resist.  In fact, assume he will and kill him.  We'll get him resurrected and be ready to cure his 
        folly.  What do you say? */
        public override object Description
        {
            get
            {
                return 1074856;
            }
        }
        /* I understand.  A Master of Bedlam, even one entirely off his rocker, is too much for you to handle. */
        public override object Refuse
        {
            get
            {
                return 1074857;
            }
        }
        /* You had better get going.  Master Theophilus isn't likely to kill himself just to save me this embarrassment. */
        public override object Uncomplete
        {
            get
            {
                return 1074858;
            }
        }
        /* You look a bit worse for wear!  He put up a good fight did he?  Hah!  That's the spirit … a Master 
        of Bedlam is a match for most. */
        public override object Complete
        {
            get
            {
                return 1074859;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
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

    public class Gnosos : MondainQuester
    {
        [Constructable]
        public Gnosos()
            : base("Master Gnosos", "the necromancer")
        { 
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
            this.SetSkill(SkillName.EvalInt, 65.0, 88.0);
            this.SetSkill(SkillName.Inscribe, 60.0, 83.0);
            this.SetSkill(SkillName.Necromancy, 64.0, 100.0);
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.MagicResist, 65.0, 88.0);
            this.SetSkill(SkillName.SpiritSpeak, 36.0, 68.0);
        }

        public Gnosos(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(CommonBrigandsQuest),
                    typeof(GoneNativeQuest),
                    typeof(PointyEarsQuest),
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x83E8;
            this.HairItemID = 0x203B;
            this.FacialHairItemID = 0x2040;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x485));
            this.AddItem(new Robe(0x497));
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