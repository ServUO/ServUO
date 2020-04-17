using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierOneQuest")]
    public class BouraBouraQuest : BaseQuest, ITierQuest
    {
        public BouraBouraQuest()
            : base()
        {
            if (0.50 > Utility.RandomDouble())
            {
                AddObjective(new SlayObjective(typeof(LowlandBoura), "Lowland Boura's", 15));
            }
            else
            {
                AddObjective(new SlayObjective(typeof(RuddyBoura), "Ruddy Boura's", 20));
            }

            AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* Boura,Boura */
        public override object Title => 1112784;
        public override object Description => 1112798;
        public override object Refuse => 1112799;
        public override object Uncomplete => 1112800;
        public override object Complete => 1112801;

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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierOne2Quest")]
    public class RaptorliciousQuest : BaseQuest, ITierQuest
    {
        public RaptorliciousQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Raptor), "Raptor's", 20));

            AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* Raptorlicious */
        public override object Title => 1112785;
        public override object Description => 1112803;
        public override object Refuse => 1112804;
        public override object Uncomplete => 1112805;
        public override object Complete => 1112806;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierOne3Quest")]
    public class TheSlithWarsQuest : BaseQuest, ITierQuest
    {
        public TheSlithWarsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Slith), "Slith's", 20));

            AddReward(new BaseReward(typeof(DustyAdventurersBackpack), 1113189)); // Dusty Adventurers Backpack
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* The Slith Wars */
        public override object Title => 1112786;
        public override object Description => 1112807;
        public override object Refuse => 1112808;
        public override object Uncomplete => 1112809;
        public override object Complete => 1112810;
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

    // Tier 2
    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierTwo1Quest")]
    public class BouraBouraAndMoreBouraQuest : BaseQuest, ITierQuest
    {
        public BouraBouraAndMoreBouraQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(HighPlainsBoura), "High Plains Boura", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }


        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* Boura, Boura, and more Boura */
        public override object Title => 1112787;
        public override object Description => 1112823;
        public override object Refuse => 1112824;
        public override object Uncomplete => 1112825;
        public override object Complete => 1112826;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierTwo2Quest")]
    public class RevengeOfTheSlithQuest : BaseQuest, ITierQuest
    {
        public RevengeOfTheSlithQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(ToxicSlith), "Toxic Slith's", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }


        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* Revenge of the Slith */
        public override object Title => 1112788;
        public override object Description => 1112827;
        public override object Refuse => 1112828;
        public override object Uncomplete => 1112829;
        public override object Complete => 1112830;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierTwo3Quest")]
    public class WeveGotAnAntProblemQuest : BaseQuest, ITierQuest
    {
        public WeveGotAnAntProblemQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(FireAnt), "Fire Ant's", 20));

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }


        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* Weve got an Ant Problem */
        public override object Title => 1112789;
        public override object Description => 1112831;
        public override object Refuse => 1112832;
        public override object Uncomplete => 1112833;
        public override object Complete => 1112834;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierTwo4Quest")]
    public class AmbushingTheAmbushersQuest : BaseQuest, ITierQuest
    {
        public AmbushingTheAmbushersQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(KepetchAmbusher), "Kepetch Ambusher's", 20));////////////////

            AddReward(new BaseReward(typeof(DustyExplorersBackpack), 1113190)); // Dusty Explorers Backpack
        }


        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* Ambushing the Ambusher's */
        public override object Title => 1112790;
        public override object Description => 1112835;
        public override object Refuse => 1112836;
        public override object Uncomplete => 1112837;
        public override object Complete => 1112838;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierThree1Quest")]
    public class ItMakesMeSickQuest : BaseQuest, ITierQuest
    {
        public ItMakesMeSickQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(PutridUndeadGargoyle), "Putrid Undead Gargoyle's", 20));

            AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
        }


        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* It Makes Me Sick */
        public override object Title => 1112791;
        public override object Description => 1112839;
        public override object Refuse => 1112840;
        public override object Uncomplete => 1112841;
        public override object Complete => 1112842;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierThree2Quest")]
    public class ItsAMadMadWorldQuest : BaseQuest, ITierQuest
    {
        public ItsAMadMadWorldQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(MaddeningHorror), "Maddening Horror's", 20));/////////////

            AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* It's A Mad, Mad, World */
        public override object Title => 1112792;
        public override object Description => 1112843;
        public override object Refuse => 1112844;
        public override object Uncomplete => 1112845;
        public override object Complete => 1112846;
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

    [TypeAlias("Server.Engines.Quests.PercolemTheHunterTierThree3Quest")]
    public class TheDreamersQuest : BaseQuest, ITierQuest
    {
        public TheDreamersQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(DreamWraith), "Dream Wraith's", 20));

            AddReward(new BaseReward(typeof(DustyHuntersBackpack), 1113191)); // Dusty Hunter's Backpack
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        /* The Dreamer's */
        public override object Title => 1112793;
        public override object Description => 1112847;
        public override object Refuse => 1112848;
        public override object Uncomplete => 1112849;
        public override object Complete => 1112850;
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
