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
                AddObjective(new SlayObjective(typeof(GargoyleShade), "Gargoyle Shade", 10));
            }
            else if (0.50 > Utility.RandomDouble())
            {
                AddObjective(new SlayObjective(typeof(EffetePutridGargoyle), "Effete Putrid Gargoyle", 10));
            }
            else
            {
                AddObjective(new SlayObjective(typeof(EffeteUndeadGargoyle), "Effete Undead Gargoyle", 10));
            }

            AddReward(new BaseReward(typeof(NecklaceofDiligence), 1113137));
        }

        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }

        /* The Arisen */
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
            Owner.SendLocalizedMessage(1112542, null, 0x23); 						
            Owner.PlaySound(CompleteSound);
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

    [TypeAlias("Server.Engines.Quests.Naxatillor")]
    public class Naxatilor : MondainQuester
    {
        [Constructable]
        public Naxatilor()
            : base("Naxatillor", "The Seer")
        {
        }

        public Naxatilor(Serial serial)
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
            InitStats(100, 100, 25);
			
            Female = false;
            CantWalk = true;
            Body = 666;
            HairItemID = 16987;
            HairHue = 1801;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(FixSpawner);
            }
        }

        private void FixSpawner()
        {
            SpawnerPersistence.Replace("Naxatillor", "Naxatilor", null);
        }
    }
}