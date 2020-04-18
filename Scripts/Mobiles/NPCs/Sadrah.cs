using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class FleeAndFatigueQuest : BaseQuest
    {
        public FleeAndFatigueQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(RefreshPotion), "refresh potion", 10, 0xF0B));

            AddReward(new BaseReward(typeof(AlchemistsSatchel), 1074282)); // Craftsman's Satchel
        }

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(3);
        /* Flee and Fatigue */
        public override object Title => 1075487;
        /* I was just *coughs* ambushed near the moongate. *wheeze* Why do I pay my taxes? Where were the guards? 
        You then, you an Alchemist? If you can make me a few Refresh potions, I will be back on my feet and can 
        give those lizards the what for! Find a mortar and pestle, a good amount of black pearl, and ten empty 
        bottles to store the finished potions in. Just use the mortar and pestle and the rest will surely come 
        to you. When you return, the favor will be repaid. */
        public override object Description => 1075488;
        /* Fine fine, off with *cough* thee then! The next time you see a lizardman though, give him a whallop for me, eh? */
        public override object Refuse => 1075489;
        /* Just remember you need to use your mortar and pestle while you have empty bottles and some black pearl. 
        Refresh potions are what I need. */
        public override object Uncomplete => 1075490;
        /* *glug* *glug* Ahh... Yes! Yes! That feels great! Those lizardmen will never know what hit 'em! Here, take 
        this, I can get more from the lizards. */
        public override object Complete => 1075491;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
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

        public override Type[] Quests => new Type[]
                {
                    typeof(FleeAndFatigueQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x8406;
            HairItemID = 0x203D;
            HairHue = 0x901;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Longsword());
            AddItem(new Boots(0x901));
            AddItem(new Shirt(0x127));
            AddItem(new Cloak(0x65));
            AddItem(new Skirt(0x52));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
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
            Hue = BaseReward.SatchelHue();

            AddItem(new Bloodmoss(10));
            AddItem(new MortarPestle());
        }

        public AlchemistsSatchel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}