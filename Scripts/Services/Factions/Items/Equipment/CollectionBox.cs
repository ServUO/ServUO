using System;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.VvV;

namespace Server.Factions
{
    public class FactionCollectionBox : BaseCollectionItem
    {
        public Collection FactionCollection { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Faction Faction
        {
            get
            {
                switch (FactionCollection)
                {
                    case Collection.Minax: return Minax.Instance;
                    case Collection.TrueBritannians: return TrueBritannians.Instance;
                    case Collection.Shadowlords: return Shadowlords.Instance;
                    case Collection.CouncilOfMages: return CouncilOfMages.Instance;
                }

                return null;
            }
        }

        public FactionCollectionBox(Faction faction)
            : base(0xE7D)
        {
            if (faction == Minax.Instance)
                FactionCollection = Collection.Minax;
            else if (faction == TrueBritannians.Instance)
                FactionCollection = Collection.TrueBritannians;
            else if (faction == Shadowlords.Instance)
                FactionCollection = Collection.Shadowlords;
            else
                FactionCollection = Collection.CouncilOfMages;

            Hue = 0x48D;
            StartTier = 10000000;
            NextTier = 5000000;
            DailyDecay = 100000;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Alive)
            {
                if (from.NetState == null || !from.NetState.SupportsExpansion(Expansion.ML))
                {
                    from.SendLocalizedMessage(1073651); // You must have Mondain's Legacy before proceeding...			
                    return;
                }
                else if (!Factions.Settings.Enabled && from.AccessLevel < AccessLevel.GameMaster)
                {
                    from.SendLocalizedMessage(1042753, "Factions"); // ~1_SOMETHING~ has been temporarily disabled.
                    return;
                }

                if (from.InRange(Location, 2) && from is PlayerMobile && CanDonate((PlayerMobile)from))
                {
                    from.CloseGump(typeof(CommunityCollectionGump));
                    from.SendGump(new CommunityCollectionGump((PlayerMobile)from, this, Location));
                }
                else
                    from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override bool CanDonate(PlayerMobile player)
        {
            Faction faction = Faction.Find(player);

            if (faction == null)
            {
                return false;
            }

            return faction == this.Faction;
        }

        public FactionCollectionBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073436;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override Collection CollectionID
        {
            get
            {
                return FactionCollection;
            }
        }

        public override int MaxTier
        {
            get
            {
                return 12;
            }
        }

        private void LoadData()
        {
            Donations.Add(new CollectionItem(typeof(Silver), 0xEF2, 1017384, 0x0, 1));

            Rewards.Add(new FactionCollectionItem(typeof(StrongholdRune), 0x1F14, 1094700, 0, this.Faction, 150, 0));
            Rewards.Add(new FactionCollectionItem(typeof(ShrineGem), 0x1EA7, 1094711, 0, this.Faction, 100, 0));
            Rewards.Add(new FactionCollectionItem(typeof(SupernovaPotion), 3849, 1094718, 13, this.Faction, 100, 0));
            Rewards.Add(new FactionCollectionItem(typeof(GreaterStaminaPotion), 3849, 1094764, 437, this.Faction, 50, 0));
            Rewards.Add(new FactionCollectionItem(typeof(EnchantedBandage), 0xE21, 1094712, 0, this.Faction, 100, 0));
            Rewards.Add(new FactionCollectionItem(typeof(PowderOfPerseverance), 4102, 1094712, 2419, this.Faction, 300, 0));
            Rewards.Add(new FactionCollectionItem(typeof(MorphEarrings), 0x1087, 1094746, 0, this.Faction, 1000, 0));

            Rewards.Add(new FactionCollectionItem(typeof(PrimerOnArmsTalisman), 12121, 1094704, 0, this.Faction, 3000, 7));
            Rewards.Add(new FactionCollectionItem(typeof(ClaininsSpellbook), 3834, 1094705, 0x84D, this.Faction, 4000, 9));
            Rewards.Add(new FactionCollectionItem(typeof(CrimsonCincture), 5435, 1075043, 0x485, this.Faction, 2000, 4));
            Rewards.Add(new FactionCollectionItem(typeof(CrystallineRing), 4234, 1075096, 1152, this.Faction, 4000, 9));
            Rewards.Add(new FactionCollectionItem(typeof(HumanFeyLeggings), 5054, 1075041, 0, this.Faction, 1000, 1));
            Rewards.Add(new FactionCollectionItem(typeof(FoldedSteelGlasses), 12216, 1073380, 1150, this.Faction, 4000, 9));
            Rewards.Add(new FactionCollectionItem(typeof(HeartOfTheLion), 5141, 1070817, 1281, this.Faction, 2000, 4));
            Rewards.Add(new FactionCollectionItem(typeof(HuntersHeaddress), 5447, 1061595, 1428, this.Faction, 2000, 4));
            Rewards.Add(new FactionCollectionItem(typeof(KasaOfTheRajin), 10136, 1070969, 0, this.Faction, 1000, 1));
            Rewards.Add(new FactionCollectionItem(typeof(MaceAndShieldGlasses), 12216, 1073381, 477, this.Faction, 5000, 10));
            Rewards.Add(new FactionCollectionItem(typeof(VesperOrderShield), 7108, 1073258, 0, this.Faction, 4000, 9));
            Rewards.Add(new FactionCollectionItem(typeof(OrnamentOfTheMagician), 4230, 1061105, 1364, this.Faction, 5000, 10));
            Rewards.Add(new FactionCollectionItem(typeof(RingOfTheVile), 4234, 1061102, 1271, this.Faction, 2000, 4));
            Rewards.Add(new FactionCollectionItem(typeof(RuneBeetleCarapace), 10109, 1070968, 0, this.Faction, 1000, 1));
            Rewards.Add(new FactionCollectionItem(typeof(SpiritOfTheTotem), 5445, 1061599, 1109, this.Faction, 3000, 7));
            Rewards.Add(new FactionCollectionItem(typeof(Stormgrip), 10130, 1070970, 0, this.Faction, 1000, 1));
            Rewards.Add(new FactionCollectionItem(typeof(InquisitorsResolution), 5140, 1060206, 1266, this.Faction, 5000, 10));
            Rewards.Add(new FactionCollectionItem(typeof(TomeOfLostKnowledge), 3834, 1070971, 1328, this.Faction, 3000, 7));
            Rewards.Add(new FactionCollectionItem(typeof(WizardsCrystalGlasses), 4102, 1094756, 0, this.Faction, 3000, 7));
        }

        public override void IncreaseTier()
        {
            base.IncreaseTier();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)FactionCollection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            FactionCollection = (Collection)reader.ReadInt();

            LoadData();
        }
    }
}