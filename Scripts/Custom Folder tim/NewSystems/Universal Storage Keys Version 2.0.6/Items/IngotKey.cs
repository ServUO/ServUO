using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class IngotKey : BaseStoreKey, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.Seer)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

                entry.Add(new ResourceEntry(typeof(IronIngot),"Iron"));
                entry.Add(new ResourceEntry(typeof(DullCopperIngot),"Dull"));
                entry.Add(new ResourceEntry(typeof(ShadowIronIngot),"Shadow"));
                entry.Add(new ResourceEntry(typeof(CopperIngot),"Copper"));
                entry.Add(new ResourceEntry(typeof(BronzeIngot),"Bronze"));
                entry.Add(new ResourceEntry(typeof(GoldIngot),"Gold"));
                entry.Add(new ResourceEntry(typeof(AgapiteIngot),"Agapite"));
                entry.Add(new ResourceEntry(typeof(VeriteIngot),"Verite"));
                entry.Add(new ResourceEntry(typeof(ValoriteIngot),"Valorite"));
				
				//entry.Add(new ResourceEntry(typeof(BlazeIngot),"Blaze"));
				//entry.Add(new ResourceEntry(typeof(IceIngot),"Ice"));
				//entry.Add(new ResourceEntry(typeof(ToxicIngot),"Toxic"));
				//entry.Add(new ResourceEntry(typeof(ElectrumIngot),"Electrum"));
				//entry.Add(new ResourceEntry(typeof(PlatinumIngot),"Platinum"));

                return entry;
            }
        }

        [Constructable]
        public IngotKey() : base(0x0)       //hue 0x14
        {
            ItemID = 0x1BE8;            //pile of ingots
            Name = "Ingot Keys";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Ingot Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;
            return store;
        }

        //serial constructor
        public IngotKey(Serial serial) : base(serial)
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