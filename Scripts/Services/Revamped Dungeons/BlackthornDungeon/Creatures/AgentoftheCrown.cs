using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Gumps;
using System.Collections;
using Server.Network;
using Server.Engines.Points;

namespace Server.Engines.Blackthorn
{
    public class AgentOfTheCrown : BaseTurnInMobile
    {
        public override int TitleLocalization { get { return 1154520; } } // Click a minor artifact to turn in for reward points.
        public override int CancelLocalization { get { return 1154519; } }	// Bring me items bearing the crest of Minax and I will reward you with valuable items.     
        public override int TurnInLocalization { get { return 1154571; } } // Turn In Minax Artifacts
        public override int ClaimLocalization { get { return 1154572; } } // Claim Blackthorn Artifacts

        [Constructable]
        public AgentOfTheCrown() : base("the Agent Of The Crown")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            HairItemID = 0x2047;
            HairHue = 0x46D;
        }

        public override void InitOutfit()
        {
            SetWearable(new ChainChest(), 2106);
            SetWearable(new ThighBoots(), 2106);
            SetWearable(new Obi(), 1775);
            SetWearable(new BodySash(), 1775);
            SetWearable(new GoldRing());
            SetWearable(new Epaulette());

            // QuiverOfInfinityBase
            Item item = new CloakBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new GargishClothWingArmorBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            // RoyalBritannianBase

            item = new DoubletBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new GargishSashBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new SurcoatBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new TunicBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            // RuneBeetleCarapaceBase

            item = new ChainmailTunicBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new DragonBreastplateBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new GargishPlatemailChestBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new GargishStoneChestBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new PlatemailTunicBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            item = new RingmailTunicBearingTheCrestOfBlackthorn();
            item.Movable = false;
            PackItem(item);

            // ShroudOfTheCondemnedBase

            item = new EpauletteBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            // MysticsGarbBase

            item = new EpauletteBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            // CloakOfSilenceBase

            item = new EpauletteBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            // CloakOfPowerBase

            item = new EpauletteBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn4();
            item.Movable = false;
            PackItem(item);

            // CloakOfLifeBase

            item = new EpauletteBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn5();
            item.Movable = false;
            PackItem(item);

            // CloakOfDeathBase

            item = new EpauletteBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn6();
            item.Movable = false;
            PackItem(item);

            // ConjurersGarbBase

            item = new EpauletteBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new FancyDressBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new FemaleKimonoBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new GargishEpauletteBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new GargishFancyBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new GargishRobeBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new GildedDressBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new HoodedRobeBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new MaleKimonoBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new PlainDressBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            item = new RobeBearingTheCrestOfBlackthorn7();
            item.Movable = false;
            PackItem(item);

            // NightEyesBase

            item = new BandanaBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new BascinetBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new CircletBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new FeatheredHatBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new GargishEarringsBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new GargishGlassesBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new JesterHatBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new NorseHelmBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new PlateHelmBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new RoyalCircletBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new SkullcapBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new TricorneHatBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new WizardsHatBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            // MaceAndShieldBase

            item = new BandanaBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new BascinetBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new CircletBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new FeatheredHatBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new GargishEarringsBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new GargishGlassesBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new JesterHatBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new NorseHelmBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new PlateHelmBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new RoyalCircletBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new SkullcapBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new TricorneHatBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new WizardsHatBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            // FoldedSteelBase

            item = new BandanaBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new BascinetBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new CircletBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new FeatheredHatBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new GargishEarringsBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new GargishGlassesBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new JesterHatBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new NorseHelmBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new PlateHelmBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new RoyalCircletBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new SkullcapBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new TricorneHatBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            item = new WizardsHatBearingTheCrestOfBlackthorn3();
            item.Movable = false;
            PackItem(item);

            // TangleBase

            item = new GargishHalfApronBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new LeatherNinjaBeltBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new ObiBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            item = new WoodlandBeltBearingTheCrestOfBlackthorn1();
            item.Movable = false;
            PackItem(item);

            // CrimsonCinctureBase

            item = new GargishHalfApronBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new LeatherNinjaBeltBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new ObiBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);

            item = new WoodlandBeltBearingTheCrestOfBlackthorn2();
            item.Movable = false;
            PackItem(item);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1154517); // Minax Artifact Turn in Officer
        }

        public override void AwardPoints(PlayerMobile pm, int amount)
        {
            PointsSystem.Blackthorn.AwardPoints(pm, amount);
        }

        public override bool IsRedeemableItem(Item item)
        {
            if (item is BaseWeapon && ((BaseWeapon)item).ReforgedSuffix == ReforgedSuffix.Minax)
                return true;
            if (item is BaseArmor && ((BaseArmor)item).ReforgedSuffix == ReforgedSuffix.Minax)
                return true;
            if (item is BaseJewel && ((BaseJewel)item).ReforgedSuffix == ReforgedSuffix.Minax)
                return true;
            if (item is BaseClothing && ((BaseClothing)item).ReforgedSuffix == ReforgedSuffix.Minax)
                return true;

            return false;
        }

        public override void SendRewardGump(Mobile m)
        {
            if (m.Player && m.CheckAlive())
                m.SendGump(new BlackthornRewardGump(this, m as PlayerMobile));
        }

        public AgentOfTheCrown(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
