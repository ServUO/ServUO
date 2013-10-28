using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class BlackOrderBadgesQuest : BaseQuest
    { 
        public BlackOrderBadgesQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(SerpentFangSectBadge), "serpent fang badges", 5));
            this.AddObjective(new ObtainObjective(typeof(TigerClawSectBadge), "tiger claw badges", 5));
            this.AddObjective(new ObtainObjective(typeof(DragonFlameSectBadge), "dragon flame badges", 5));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Black Order Badges */
        public override object Title
        {
            get
            {
                return 1072903;
            }
        }
        /* What's that? *alarmed gasp*  Do not speak of the Black Order so loudly, they might hear and take offense.  *whispers*  
        I collect the badges of their sects, if you wish to seek them out and slay them.  Bring five of each and I will reward you. */
        public override object Description
        {
            get
            {
                return 1072962;
            }
        }
        /* *whisper* It's a very dangerous task.  Let me know if you change your mind. */
        public override object Refuse
        {
            get
            {
                return 1072971;
            }
        }
        /* *whisper* The Citadel entrance is disguised as a fishing village.  The magical portal into the stronghold itself is 
        moved frequently.  You'll need to search for it. */
        public override object Uncomplete
        {
            get
            {
                return 1072972;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

    public class EvidenceQuest : BaseQuest
    { 
        public EvidenceQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(OrdersFromMinax), "orders from minax", 1));
			
            this.AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* Evidence */
        public override object Title
        {
            get
            {
                return 1072906;
            }
        }
        /* We believe the Black Order has fallen under the sway of Minax, somehow.  Seek evidence that proves our theory 
        by piercing the secrets of the Citadel. */
        public override object Description
        {
            get
            {
                return 1072964;
            }
        }
        /* Many fear to tangle with the wicked sorceress.  I understand and appreciate your concerns. */
        public override object Refuse
        {
            get
            {
                return 1072975;
            }
        }
        /* I don't know where inside The Citadel such evidence could be found.  Perhaps the most guarded sanctum is 
        the place to look. */
        public override object Uncomplete
        {
            get
            {
                return 1072976;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.Citadel;
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

    public class Sarakki : MondainQuester
    {
        [Constructable]
        public Sarakki()
            : base("Sarakki", "the notary")
        { 
        }

        public Sarakki(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(BlackOrderBadgesQuest),
                    typeof(EvidenceQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Human;
			
            this.Hue = 0x841E;
            this.HairItemID = 0x2049;
            this.HairHue = 0x1BB;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x740));
            this.AddItem(new FancyShirt(0x72C));
            this.AddItem(new Skirt(0x53C));
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