using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class FleeAndFatigueQuest : BaseQuest
    { 
        public FleeAndFatigueQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(RefreshPotion), "refresh potion", 10, 0xF0B));
						
            this.AddReward(new BaseReward(typeof(AlchemistsSatchel), 1074282)); // Craftsman's Satchel
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(3);
            }
        }
        /* Flee and Fatigue */
        public override object Title
        {
            get
            {
                return 1075487;
            }
        }
        /* I was just *coughs* ambushed near the moongate. *wheeze* Why do I pay my taxes? Where were the guards? 
        You then, you an Alchemist? If you can make me a few Refresh potions, I will be back on my feet and can 
        give those lizards the what for! Find a mortar and pestle, a good amount of black pearl, and ten empty 
        bottles to store the finished potions in. Just use the mortar and pestle and the rest will surely come 
        to you. When you return, the favor will be repaid. */
        public override object Description
        {
            get
            {
                return 1075488;
            }
        }
        /* Fine fine, off with *cough* thee then! The next time you see a lizardman though, give him a whallop for me, eh? */
        public override object Refuse
        {
            get
            {
                return 1075489;
            }
        }
        /* Just remember you need to use your mortar and pestle while you have empty bottles and some black pearl. 
        Refresh potions are what I need. */
        public override object Uncomplete
        {
            get
            {
                return 1075490;
            }
        }
        /* *glug* *glug* Ahh... Yes! Yes! That feels great! Those lizardmen will never know what hit 'em! Here, take 
        this, I can get more from the lizards. */
        public override object Complete
        {
            get
            {
                return 1075491;
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

    public class Sadrah : MondainQuester
    {
        [Constructable]
        public Sadrah()
            : base("Sadrah", "the courier")
        { 
        }

        public Sadrah(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(FleeAndFatigueQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x8406;
            this.HairItemID = 0x203D;
            this.HairHue = 0x901;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new Longsword());
            this.AddItem(new Boots(0x901));
            this.AddItem(new Shirt(0x127));
            this.AddItem(new Cloak(0x65));
            this.AddItem(new Skirt(0x52));
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

    public class AlchemistsSatchel : Backpack
    {
        [Constructable]
        public AlchemistsSatchel()
            : base()
        {
            this.Hue = BaseReward.SatchelHue();
			
            this.AddItem(new Bloodmoss(10));
            this.AddItem(new MortarPestle());
        }

        public AlchemistsSatchel(Serial serial)
            : base(serial)
        {
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