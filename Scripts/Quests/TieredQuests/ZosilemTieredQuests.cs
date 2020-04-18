using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class DabblingontheDarkSide : BaseQuest, ITierQuest
    {
        public DabblingontheDarkSide()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            AddObjective(new ObtainObjective(typeof(FairyDragonWing), "Fairy Dragon Wings", 10, 0x1084));
            AddObjective(new ObtainObjective(typeof(Dough), "Dough", 1, 0x103D));

            AddReward(new BaseReward(typeof(DeliciouslyTastyTreat), 2, "Deliciously Tasty Treat"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        public override object Title => 1112778;
        public override object Description => 1112963;
        public override object Refuse => 1112964;
        public override object Uncomplete => 1112965;
        public override object Complete => 1112966;
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

    public class TheBrainyAlchemist : BaseQuest, ITierQuest
    {
        public TheBrainyAlchemist()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(ArcaneGem), "Arcane Gem", 1, 0x1ea7));
            AddObjective(new ObtainObjective(typeof(UndeadGargHorn), "Undamaged Undead Gargoyle Horns", 10, 0x315C));
            AddObjective(new ObtainObjective(typeof(InspectedKegofTotalRefreshment), "Inspected Keg of Total Refreshment", 1, 0x1940));
            AddObjective(new ObtainObjective(typeof(InspectedKegofGreaterPoison), "Inspected Keg of Greater Poison", 1, 0x1940));

            AddReward(new BaseReward(typeof(InfusedAlchemistsGem), "Infused Alchemist's Gem"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        public override object Title => 1112779;
        public override object Description => 1112967;
        public override object Refuse => 1112968;
        public override object Uncomplete => 1112969;
        public override object Complete => 1112970;
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

    public class ArmorUp : BaseQuest, ITierQuest
    {
        public ArmorUp()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            AddObjective(new ObtainObjective(typeof(LeatherWolfSkin), "Leather Wolf Skin", 10, 0x3189));
            AddObjective(new ObtainObjective(typeof(UndamagedIronBeetleScale), "Undamaged IronBeetle Scale", 10, 0x5742));

            AddReward(new BaseReward(typeof(VialofArmorEssence), 1, "Vial Of Armor Essence"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        public override object Title => 1112780;
        public override object Description => 1112971;
        public override object Refuse => 1112972;
        public override object Uncomplete => 1112973;
        public override object Complete => 1112974;
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

    public class ToTurnBaseMetalIntoVerite : BaseQuest, ITierQuest
    {
        public ToTurnBaseMetalIntoVerite()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(UndeadGargoyleMedallions), "Undead Gargoyle Medallions", 5, 0x2AAA));
            AddObjective(new ObtainObjective(typeof(PileofInspectedVeriteIngots), "Pile of Inspected Verite Ingots", 1, 0x1BEA));

            AddReward(new BaseReward(typeof(ElixirofVeriteConversion), 1, "Elixir of Verite Conversion"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        public override object Title => 1112781;
        public override object Description => 1112975;
        public override object Refuse => 1112976;
        public override object Uncomplete => 1112977;
        public override object Complete => 1112978;
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

    public class PureValorite : BaseQuest, ITierQuest
    {
        public PureValorite()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(InfusedGlassStave), "Infused Glass Stave", 5, 0x2AAA));
            AddObjective(new ObtainObjective(typeof(PileofInspectedValoriteIngots), "Pile of Inspected Valorite Ingots", 1, 0x1BEA));

            AddReward(new BaseReward(typeof(ElixirofValoriteConversion), 1, "Elixir of Valorite Conversion"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        public override object Title => 1112783;
        public override object Description => 1112983;
        public override object Refuse => 1112984;
        public override object Uncomplete => 1112985;
        public override object Complete => 1112986;
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

    public class TheForbiddenFruit : BaseQuest, ITierQuest
    {
        public TheForbiddenFruit()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            AddObjective(new ObtainObjective(typeof(TreefellowWood), "TreefellowWood", 10, 0x1BDD));
            AddObjective(new ObtainObjective(typeof(Dough), "Dough", 1, 0x103D));

            AddReward(new BaseReward(typeof(IrresistiblyTastyTreat), "Irresistibly Tasty Treat"));
        }

        public TierQuestInfo TierInfo => TierQuestInfo.Zosilem;
        public override TimeSpan RestartDelay => TierQuestInfo.GetCooldown(TierInfo, GetType());

        public override object Title => 1112782;

        public override object Description => 1112979;
        public override object Refuse => 1112979;
        public override object Uncomplete => 1112980;
        public override object Complete => 1112982;
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
