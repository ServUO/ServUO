using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class GatheringOfEvidence : BaseQuest
    {
        public override bool DoneOnce { get { return true; } }

        public GatheringOfEvidence() : base()
        {
            this.AddObjective(new ObtainObjective(typeof(VoidCrystalOfCorruptedArcaneEssence), "Void Crystal of Corrupted Arcane Essence", 1));
            this.AddObjective(new ObtainObjective(typeof(VoidCrystalOfCorruptedSpiritualEssence), "Void Crystal of Corrupted Spiritual Essence", 1));
            this.AddObjective(new ObtainObjective(typeof(VoidCrystalOfCorruptedMysticalEssence), "Void Crystal of Corrupted Mystical Essence", 1));

            this.AddReward(new BaseReward(typeof(ResonantShieldOfVengeance), "Resonant Shield of Vengeance"));
            this.AddReward(new BaseReward(typeof(WindOfCorruption), "Wind of Corruption"));
        }

        public override object Title
        {
            get
            {
                return 1150316; // Gathering of Evidence (Gargoyle Rewards)
            }
        }

        /*There seems to be unrest spreading among the refugees surrounding the Royal city. There are 
         * rumors of gargoyles willingly leaving their encampments and disappearing into the abyss. 
         * Through our own investigations, we have learned that there are militant gargoyle camps in 
         * the abyss where their magically gifted members are tapping into a new source of magic. We 
         * suspect they are tapping the void energies and require you to obtain proof so that we can 
         * prepare a plan of action to deal with any threat they may pose. Bring back all of the following;
         * Void Crystal of Corrupted Arcane Essence, Void Crystal of Corrupted Spiritual Essence, and a 
         * Void Crystal of Corrupted Mystical Essence.*/
        public override object Description
        {
            get
            {
                return 1150317;
            }
        }

        public override object Refuse
        {
            get
            {
                return "That's a shame.";
            }
        }

        //Have you retrieved the essences? We cannot prepare our strategy without solid evidence.
        public override object Uncomplete
        {
            get
            {
                return 1150319;
            }
        }

        /*Well Done! This is just what we needed. We can ill afford these separatist encampments 
         * upsetting the delicate balance with the void consuming our world.*/
        public override object Complete
        {
            get
            {
                return 1150320;
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

    public class GatheringProof : BaseQuest
    {
        public override bool DoneOnce { get { return true; } }

        public GatheringProof()
            : base()
        {
            this.AddObjective(new ObtainObjective(typeof(VoidCrystalOfCorruptedArcaneEssence), "Void Crystal of Corrupted Arcane Essence", 1));
            this.AddObjective(new ObtainObjective(typeof(VoidCrystalOfCorruptedSpiritualEssence), "Void Crystal of Corrupted Spiritual Essence", 1));
            this.AddObjective(new ObtainObjective(typeof(VoidCrystalOfCorruptedMysticalEssence), "Void Crystal of Corrupted Mystical Essence", 1));

            this.AddReward(new BaseReward(typeof(ResonantShieldOfVengeanceHuman), "Resonant Shield of Vengeance"));
            this.AddReward(new BaseReward(typeof(WindOfCorruptionHuman), "Wind of Corruption"));
        }

        public override object Title
        {
            get
            {
                return 1150384; // Gathering Proof (Human Rewards)
            }
        }

        /*There seems to be unrest spreading among the refugees surrounding the Royal city. There are 
         * rumors of gargoyles willingly leaving their encampments and disappearing into the abyss. 
         * Through our own investigations, we have learned that there are militant gargoyle camps in 
         * the abyss where their magically gifted members are tapping into a new source of magic. We 
         * suspect they are tapping the void energies and require you to obtain proof so that we can 
         * prepare a plan of action to deal with any threat they may pose. Bring back all of the following;
         * Void Crystal of Corrupted Arcane Essence, Void Crystal of Corrupted Spiritual Essence, and a 
         * Void Crystal of Corrupted Mystical Essence.*/
        public override object Description
        {
            get
            {
                return 1150317;
            }
        }

        public override object Refuse
        {
            get
            {
                return "That's a shame.";
            }
        }

        //Have you retrieved the essences? We cannot prepare our strategy without solid evidence.
        public override object Uncomplete
        {
            get
            {
                return 1150319;
            }
        }

        /*Well Done! This is just what we needed. We can ill afford these separatist encampments 
         * upsetting the delicate balance with the void consuming our world.*/
        public override object Complete
        {
            get
            {
                return 1150320;
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

    public class Xeninlor : MondainQuester
    {
        [Constructable]
        public Xeninlor()
            : base("Xeninlor", "the Security Advisor")
        {
        }

        public Xeninlor(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(GatheringOfEvidence)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            Race = Race.Gargoyle;

            this.Body = 666;
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());

            this.AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
            this.AddItem(new SerpentStoneStaff());
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

    public class Prassel : MondainQuester
    {
        [Constructable]
        public Prassel()
            : base("Prassel", "the Security Liason")
        {
        }

        public Prassel(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(GatheringProof)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = false;
            Race = Race.Human;

            this.Body = 0x190;
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());

            this.AddItem(new Tunic(Utility.RandomNeutralHue()));
            this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            this.AddItem(new Boots());
            this.AddItem(new Halberd());
        }

        public override bool CheckTerMur()
        {
            return false;
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