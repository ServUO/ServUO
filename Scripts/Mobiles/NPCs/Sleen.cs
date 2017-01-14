using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class NecessitysMotherQuest : BaseQuest
    { 
        public NecessitysMotherQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(TinkerTools), "tinker's tools", 10, 0x1EB8));
			
            this.AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Necessity's Mother */
        public override object Title
        {
            get
            {
                return 1073906;
            }
        }
        /* What a thing, this human need to tinker. It seems there is no end to what might be produced with a set of 
        Tinker's Tools. Who knows what an elf might build with some? Could you obtain some tinker's tools and bring 
        them to me? In exchange, I offer you elven lore and knowledge.  */
        public override object Description
        {
            get
            {
                return 1074096;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me tinker's tools. */
        public override object Uncomplete
        {
            get
            {
                return 1073952;
            }
        }
        /* Now, I shall see what an elf can invent! */
        public override object Complete
        {
            get
            {
                return 1073977;
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

    public class TickTockQuest : BaseQuest
    { 
        public TickTockQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Clock), "clock", 10, 0x104B));
			
            this.AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Tick Tock */
        public override object Title
        {
            get
            {
                return 1073907;
            }
        }
        /* Elves find it remarkable the human preoccupation with the passage of time. To have built instruments to try and 
        capture time -- it is a fascinating notion. I would like to see how a clock is put together. Maybe you could provide 
        some clocks for my experimentation? */
        public override object Description
        {
            get
            {
                return 1074097;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me clocks. */
        public override object Uncomplete
        {
            get
            {
                return 1073953;
            }
        }
        /* Enjoy my thanks for your service. */
        public override object Complete
        {
            get
            {
                return 1073978;
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

    public class ReptilianDentistQuest : BaseQuest
    { 
        public ReptilianDentistQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(CoilsFang), "coil's fang", 1));
			
            this.AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Reptilian Dentist */
        public override object Title
        {
            get
            {
                return 1074280;
            }
        }
        /* I'm working on a striking necklace -- something really unique -- and I know just what I need to finish it up.  
        A huge fang!  Won't that catch the eye?  I would like to employ you to find me such an item, perhaps a snake would 
        make the ideal donor.  I'll make it worth your while, of course. */
        public override object Description
        {
            get
            {
                return 1074710;
            }
        }
        /* I understand.  I don't like snakes much either.  They're so creepy. */
        public override object Refuse
        {
            get
            {
                return 1074723;
            }
        }
        /* Those really big snakes like swamps, I've heard.  You might try the blighted grove. */
        public override object Uncomplete
        {
            get
            {
                return 1074722;
            }
        }
        /* Do you have it?  *gasp* What a tooth!  Here … I must get right to work. */
        public override object Complete
        {
            get
            {
                return 1074721;
            }
        }
        public override bool CanOffer()
        { 
            return MondainsLegacy.BlightedGrove;
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

    public class Sleen : MondainQuester
    { 
        [Constructable]
        public Sleen()
            : base("Sleen", "the trinket weaver")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Sleen(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ArchSupportQuest),
                    typeof(StopHarpingOnMeQuest),
                    typeof(TheFarEyeQuest),
                    typeof(NecessitysMotherQuest),
                    typeof(TickTockQuest),
                    typeof(FromTheGaultierCollectionQuest),
                    typeof(ReptilianDentistQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x83E6;
            this.HairItemID = 0x2FC0;
            this.HairHue = 0x386;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new FullApron(0x1BB));
            this.AddItem(new ShortPants(0x71));
            this.AddItem(new Cloak(0x73C));
            this.AddItem(new ElvenShirt());
            this.AddItem(new SmithHammer());
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