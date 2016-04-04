using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class PixieDustToDustQuest : BaseQuest
    { 
        public PixieDustToDustQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Pixie), "pixies", 10));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Pixie dust to dust */
        public override object Title
        {
            get
            {
                return 1073661;
            }
        }
        /* Is there anything more foul than a pixie? They have cruel eyes and a mind for mischief, I say. I don't 
        care if some think they're cute -- I say kill them and let the Avatar sort them out. In fact, if you were 
        to kill a few pixies, I'd make sure you had a few coins to rub together, if you get my meaning. */
        public override object Description
        {
            get
            {
                return 1073700;
            }
        }
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse
        {
            get
            {
                return 1073733;
            }
        }
        /* There's too much cuteness in the world -- kill those pixies! */
        public override object Uncomplete
        {
            get
            {
                return 1073741;
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

    public class Oolua : MondainQuester
    { 
        [Constructable]
        public Oolua()
            : base("Lorekeeper Oolua", "the keeper of tradition")
        { 
        }

        public Oolua(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ParoxysmusSuccubiQuest),
                    typeof(ParoxysmusMolochQuest),
                    typeof(ParoxysmusDaemonsQuest),
                    typeof(ParoxysmusArcaneDaemonsQuest),
                    typeof(CausticComboQuest),
                    typeof(PlagueLordQuest),
                    typeof(OrcSlayingQuest),
                    typeof(DreadhornQuest),
                    typeof(PixieDustToDustQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.CantWalk = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x853F;
            this.HairItemID = 0x2FCC;
            this.HairHue = 0x388;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x70E));
            this.AddItem(new WildStaff());
            this.AddItem(new GemmedCirclet());
            this.AddItem(new Cloak(0x1BB));
            this.AddItem(new Skirt(0x3));
            this.AddItem(new FancyShirt(0x70A));
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