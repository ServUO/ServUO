using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class PercolemTheHunterTierOneQuest : BaseQuest
    { 
        public PercolemTheHunterTierOneQuest()
            : base()
        { 
            if (0.50 > Utility.RandomDouble())
            {
                this.AddObjective(new SlayObjective(typeof(LowlandBoura), "Lowland Boura's", 15));
            }
            else
            {
                this.AddObjective(new SlayObjective(typeof(RuddyBoura), "Ruddy Boura's", 20));
            }

            this.AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        ///////////////////////////
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierOne2Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(30);
            }
        }
        /* Boura,Boura */
        public override object Title
        {
            get
            {
                return 1112784;
            }
        }
        public override object Description
        {
            get
            {
                return 1112798;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112799;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112800;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112801;
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

    public class PercolemTheHunterTierOne2Quest : BaseQuest
    { 
        public PercolemTheHunterTierOne2Quest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Raptor), "Raptor's", 20));

            this.AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierOne3Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(30);
            }
        }
        /* Raptorlicious */
        public override object Title
        {
            get
            {
                return 1112785;
            }
        }
        public override object Description
        {
            get
            {
                return 1112803;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112804;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112805;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112806;
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

    public class PercolemTheHunterTierOne3Quest : BaseQuest
    {
        public PercolemTheHunterTierOne3Quest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Slith), "Slith's", 20));		
							
            this.AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierTwo1Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(30);
            }
        }
        /* The Slith Wars */
        public override object Title
        {
            get
            {
                return 1112786;
            }
        }
        public override object Description
        {
            get
            {
                return 1112807;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112808;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112809;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112810;
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

    /////////////////////////////////////////Tier 2//////////////////////////////////////////
    public class PercolemTheHunterTierTwo1Quest : BaseQuest
    {
        public PercolemTheHunterTierTwo1Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(HighPlainsBoura), "High Plains Boura", 20));

            this.AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierTwo2Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        /* Boura, Boura, and more Boura */
        public override object Title
        {
            get
            {
                return 1112787;
            }
        }
        public override object Description
        {
            get
            {
                return 1112823;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112824;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112825;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112826;
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

    public class PercolemTheHunterTierTwo2Quest : BaseQuest
    {
        public PercolemTheHunterTierTwo2Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(ToxicSlith), "Toxic Slith's", 20));

            this.AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierTwo3Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        /* Revenge of the Slith */
        public override object Title
        {
            get
            {
                return 1112788;
            }
        }
        public override object Description
        {
            get
            {
                return 1112827;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112828;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112829;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112830;
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

    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PercolemTheHunterTierTwo3Quest : BaseQuest
    {
        public PercolemTheHunterTierTwo3Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(FireAnt), "Fire Ant's", 20));

            this.AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierTwo4Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        /* Weve got an Ant Problem */
        public override object Title
        {
            get
            {
                return 1112789;
            }
        }
        public override object Description
        {
            get
            {
                return 1112831;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112832;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112833;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112834;
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

    public class PercolemTheHunterTierTwo4Quest : BaseQuest
    {
        public PercolemTheHunterTierTwo4Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(KepetchAmbusher), "Kepetch Ambusher's", 20));////////////////

            this.AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierThree1Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        /* Ambushing the Ambusher's */
        public override object Title
        {
            get
            {
                return 1112790;
            }
        }
        public override object Description
        {
            get
            {
                return 1112835;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112836;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112837;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112838;
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

    ////////////////////////////////////////Tier 3///////////////////////////////////////
    public class PercolemTheHunterTierThree1Quest : BaseQuest
    {
        public PercolemTheHunterTierThree1Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(PutridUndeadGargoyle), "Putrid Undead Gargoyle's", 20));

            this.AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierThree2Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(12);
            }
        }
        /* It Makes Me Sick */
        public override object Title
        {
            get
            {
                return 1112791;
            }
        }
        public override object Description
        {
            get
            {
                return 1112839;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112840;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112841;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112842;
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

    //////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PercolemTheHunterTierThree2Quest : BaseQuest
    {
        public PercolemTheHunterTierThree2Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(MaddeningHorror), "Maddening Horror's", 20));/////////////

            this.AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override Type NextQuest
        {
            get
            {
                return typeof(PercolemTheHunterTierThree3Quest);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(12);
            }
        }
        /* It's A Mad, Mad, World */
        public override object Title
        {
            get
            {
                return 1112792;
            }
        }
        public override object Description
        {
            get
            {
                return 1112843;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112844;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112845;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112846;
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

    public class PercolemTheHunterTierThree3Quest : BaseQuest
    {
        public PercolemTheHunterTierThree3Quest()
            : base()
        {
            this.AddObjective(new SlayObjective(typeof(DreamWraith), "Dream Wraith's", 20));

            this.AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
        }

        public override QuestChain ChainID
        {
            get
            {
                return QuestChain.PercolemTheHunter;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(12);
            }
        }
        /* The Dreamer's */
        public override object Title
        {
            get
            {
                return 1112793;
            }
        }
        public override object Description
        {
            get
            {
                return 1112847;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112848;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112849;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112850;
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
}