using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class InTheBellyOfTheBeastQuest : BaseQuest
    { 
        public InTheBellyOfTheBeastQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(LuckyDagger), "lucky dagger", 1));
			
            this.AddReward(new BaseReward(typeof(SmithsCraftsmanSatchel), 1074282));
        }

        /* In the Belly of the Beast */
        public override object Title
        {
            get
            {
                return 1073049;
            }
        }
        /* Oh, the trauma!  *weeps loudly*  My lucky dagger has been lost.  It was given to me by my father, as a 
        final gift before he died.  That blade has been an heirloom of my family for generations.  I must have it 
        back.  *sniffles pathetically*  Please, find my lucky dagger. */
        public override object Description
        {
            get
            {
                return 1074658;
            }
        }
        /* *wailing cries* Then begone if you will not help a poor man in need. */
        public override object Refuse
        {
            get
            {
                return 1074659;
            }
        }
        /* *sniffles*  The dagger was stolen by some dishonest man.  Or perhaps I dropped it.  That doesn't matter 
        though.  All that matters is that you find my dagger and return it. */
        public override object Uncomplete
        {
            get
            {
                return 1074660;
            }
        }
        /* You've found it?  My lucky dagger! */
        public override object Complete
        {
            get
            {
                return 1074661;
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

    public class Frazer : MondainQuester
    { 
        [Constructable]
        public Frazer()
            : base("Frazer", "the vagabond")
        { 
            this.SetSkill(SkillName.ItemID, 64.0, 100.0);
        }

        public Frazer(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(InTheBellyOfTheBeastQuest),
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
            this.m_SBInfos.Add(new SBJewel());
            this.m_SBInfos.Add(new SBTinker(this));
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x840F;
            this.HairItemID = 0x204A;
            this.HairHue = 0x45A;
            this.FacialHairItemID = 0x204D;
            this.FacialHairHue = 0x45A;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Shoes(0x735));
            this.AddItem(new LongPants(0x4C0));
            this.AddItem(new FancyShirt(0x3));
            this.AddItem(new JesterHat(0x74A));
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