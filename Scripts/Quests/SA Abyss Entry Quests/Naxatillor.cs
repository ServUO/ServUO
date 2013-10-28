using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class TheArisenQuest : BaseQuest
    {
        public TheArisenQuest()
            : base()
        {
            if (0.30 > Utility.RandomDouble())
            {
                this.AddObjective(new SlayObjective(typeof(GargoyleShade), "Gargoyle Shade", 10));
            }
            else if (0.50 > Utility.RandomDouble())
            {
                this.AddObjective(new SlayObjective(typeof(EffetePutridGargoyle), "Effete Putrid Gargoyle", 10));
            }
            else
            {
                this.AddObjective(new SlayObjective(typeof(EffeteUndeadGargoyle), "Effete Undead Gargoyle", 10));
            }

            this.AddReward(new BaseReward(typeof(NecklaceofDiligence), 1113137));
        }

        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        /* TThe Arisen */
        public override object Title
        {
            get
            {
                return 1112538;
            }
        }
        public override object Description
        {
            get
            {
                return 1112539;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112540;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112517;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112543;
            }
        }
        public override void OnCompleted()
        {
            this.Owner.SendLocalizedMessage(1112542, null, 0x23); 						
            this.Owner.PlaySound(this.CompleteSound);
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

    public class Naxatillor : MondainQuester
    {
        [Constructable]
        public Naxatillor()
            : base("Naxatillor", "The Seer")
        {
        }

        public Naxatillor(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(TheArisenQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Body = 666;
            this.HairItemID = 16987;
            this.HairHue = 1801;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());

            this.AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
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