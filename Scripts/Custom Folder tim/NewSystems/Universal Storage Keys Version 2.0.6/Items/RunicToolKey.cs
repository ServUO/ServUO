using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class RunicToolKey : BaseStoreKey, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.Seer)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        public override int DisplayColumns { get { return 3; } }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

               /*/ entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(DullCopperRunicHammer) }, CraftResource.DullCopper,"Dull Copper",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(ShadowIronRunicHammer) },CraftResource.ShadowIron,"Shadow Iron",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(CopperRunicHammer) },CraftResource.Copper,"Copper",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(BronzeRunicHammer) },CraftResource.Bronze,"Bronze",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(GoldRunicHammer) },CraftResource.Gold,"Gold",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(AgapiteRunicHammer) },CraftResource.Agapite,"Agapite",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(VeriteRunicHammer) },CraftResource.Verite,"Verite",0,30,-5,0));
                entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(ValoriteRunicHammer) },CraftResource.Valorite,"Valorite",0,30,-5,0));
				
				entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(BlazeRunicHammer) },CraftResource.Blaze,"Blaze",0,30,-5,0));
				entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(IceRunicHammer) },CraftResource.Ice,"Ice",0,30,-5,0));
				entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(ToxicRunicHammer) },CraftResource.Toxic,"Toxic",0,30,-5,0));
				entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(ElectrumRunicHammer) },CraftResource.Electrum,"Electrum",0,30,-5,0));
				entry.Add(new RunicToolEntry(typeof(RunicHammer), new Type[] { typeof(PlatinumRunicHammer) },CraftResource.Platinum,"Platinum",0,30,-5,0));

                entry.Add(new ColumnSeparationEntry());

                entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(SpinedRunicSewingKit) },CraftResource.SpinedLeather,"Spined",0,30,-5,3));
                entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(HornedRunicSewingKit) },CraftResource.HornedLeather,"Horned",0,30,-5,3));
                entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(BarbedRunicSewingKit) },CraftResource.BarbedLeather,"Barbed",0,30,-5,3));
				
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(PolarRunicSewingKit) },CraftResource.PolarLeather,"Polar",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(SyntheticRunicSewingKit) },CraftResource.SyntheticLeather,"Synthetic",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(BlazeRunicSewingKit) },CraftResource.BlazeLeather,"Blaze",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(DaemonicRunicSewingKit) },CraftResource.DaemonicLeather,"Daemonic",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(ShadowRunicSewingKit) },CraftResource.ShadowLeather,"Shadow",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(FrostRunicSewingKit) },CraftResource.FrostLeather,"Frost",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicSewingKit), new Type[] { typeof(EtherealRunicSewingKit) },CraftResource.EtherealLeather,"Ethereal",0,30,-5,3));

				entry.Add(new ColumnSeparationEntry());
				
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(OakRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.OakWood,"Oak",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(AshRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.AshWood,"Ash",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(YewRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.YewWood,"Yew",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(HeartwoodRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Heartwood,"Heartwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(BloodwoodRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Bloodwood,"Bloodwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(FrostwoodRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Frostwood,"Frostwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(EbonyRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Ebony,"Ebony",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(BambooRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Bamboo,"Bamboo",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(PurpleHeartRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.PurpleHeart,"PurpleHeart",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(RedwoodRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Redwood,"Redwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicFletcherTool), new Type[] { typeof(PetrifiedRunicFletcherTools), typeof(RunicFletcherTools) },CraftResource.Petrified,"Petrified",0,30,-5,3));
				
				entry.Add(new ColumnSeparationEntry());
				
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(DullCopperRunicTinkerTools) },CraftResource.DullCopper,"DullCopper",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(ShadowIronRunicTinkerTools) },CraftResource.ShadowIron,"ShadowIron",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(CopperRunicTinkerTools) },CraftResource.Copper,"Copper",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(BronzeRunicTinkerTools) },CraftResource.Bronze,"Bronze",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(GoldRunicTinkerTools) },CraftResource.Gold,"Gold",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(AgapiteRunicTinkerTools) },CraftResource.Agapite,"Agapite",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(VeriteRunicTinkerTools) },CraftResource.Verite,"Verite",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(ValoriteRunicTinkerTools) },CraftResource.Valorite,"Valorite",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(BlazeRunicTinkerTools) },CraftResource.Blaze,"Blaze",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(IceRunicTinkerTools) },CraftResource.Ice,"Ice",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(ToxicRunicTinkerTools) },CraftResource.Toxic,"Toxic",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(ElectrumRunicTinkerTools) },CraftResource.Electrum,"Electrum",0,30,5,0));
				entry.Add(new RunicToolEntry(typeof(RunicTinkerTools), new Type[] { typeof(PlatinumRunicTinkerTools) },CraftResource.Platinum,"Platinum",0,30,5,0));
				
				entry.Add(new ColumnSeparationEntry());
				
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(OakRunicDovetailSaw) },CraftResource.OakWood,"Oak",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(AshRunicDovetailSaw) },CraftResource.AshWood,"Ash",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(YewRunicDovetailSaw) },CraftResource.YewWood,"Yew",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(HeartwoodRunicDovetailSaw) },CraftResource.Heartwood,"Heartwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(BloodwoodRunicDovetailSaw) },CraftResource.Bloodwood,"Bloodwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(FrostwoodRunicDovetailSaw) },CraftResource.Frostwood,"Frostwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(EbonyRunicDovetailSaw) },CraftResource.Ebony,"Ebony",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(BambooRunicDovetailSaw) },CraftResource.Bamboo,"Bamboo",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(PurpleHeartRunicDovetailSaw) },CraftResource.PurpleHeart,"PurpleHeart",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(RedwoodRunicDovetailSaw) },CraftResource.Redwood,"Redwood",0,30,-5,3));
				entry.Add(new RunicToolEntry(typeof(RunicDovetailSaw), new Type[] { typeof(PetrifiedRunicDovetailSaw) },CraftResource.Petrified,"Petrified",0,30,-5,3));
				/*/
                return entry;
            }
        }

        [Constructable]
        public RunicToolKey() : base(0x0)       // hue 65
        {
            ItemID = 0x1EBA;            //square toolkit
            Name = "Runic Tool Box";

            //runic tools withdrawn can have no less than 5 charges on them.
            _Store.MinWithdrawAmount = 5;
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Runic Tool Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;

            return store;
        }

        //serial constructor
        public RunicToolKey(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        //events

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write((bool)m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }
}