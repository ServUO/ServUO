using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class TroubleOnTheWingQuest : BaseQuest
    { 
        public TroubleOnTheWingQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Gargoyle), "gargoyles", 12, "Sanctuary"));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Trouble on the Wing */
        public override object Title
        {
            get
            {
                return 1072371;
            }
        }
        /* Those gargoyles need to get knocked down a peg or two, if you ask me.  They're always flying 
        over here and lobbing things at us. What a nuisance.  Drop a dozen of them for me, would you? */
        public override object Description
        {
            get
            {
                return 1072593;
            }
        }
        /* Don't tell me you're a gargoyle sympathizer?  *spits* */
        public override object Refuse
        {
            get
            {
                return 1072594;
            }
        }
        /* Those blasted gargoyles hang around the old tower.  That's the best place to hunt them down. */
        public override object Uncomplete
        {
            get
            {
                return 1072595;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Sanctuary;
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

    public class Koole : MondainQuester
    { 
        [Constructable]
        public Koole()
            : base("Koole", "the arcanist")
        { 
        }

        public Koole(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(DishBestServedColdQuest),
                    typeof(TroubleOnTheWingQuest),
                    typeof(MaraudersQuest),
                    typeof(DisciplineQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x83E5;
            this.HairItemID = 0x2FBF;
            this.HairHue = 0x386;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Boots(0x901));
            this.AddItem(new RoyalCirclet());
            this.AddItem(new LeafTonlet());
			
            Item item;
			
            item = new LeafChest();
            item.Hue = 0x1BB;
            this.AddItem(item);
			
            item = new LeafArms();
            item.Hue = 0x1BB;
            this.AddItem(item);
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