using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class TreasureHuntersKey : BaseStoreKey, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.Seer)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        public override int DisplayColumns { get { return 1; } }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

                entry.Add(new ResourceEntry(typeof(Lockpick),"Lockpicks"));
				//entry.Add(new ResourceEntry(typeof(GoldPan), "Gold Pans"));
				entry.Add(new ToolEntry(typeof(Shovel),new Type[] { typeof(SturdyShovel) },"Shovel",0,35,-10,-10));
                entry.Add(new ListEntry(typeof(TreasureMap),typeof(TreasureMapListEntry),"Treasure Maps"));
				entry.Add(new ListEntry(typeof(SOS), typeof(SOSListEntry), "SOS's"));
				entry.Add(new ListEntry(typeof(SpecialFishingNet),typeof(SpecialFishingNetListEntry),"Fishing Nets"));

                return entry;
            }
        }

        [Constructable]
        public TreasureHuntersKey() : base(0x0)     //hue 1861
        {
            ItemID = 0x14EE;            //rolled up map
            Name = "Treasure Hunter's Storage";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Treasure Hunter's Storage";

            store.Dynamic = false;
            store.OfferDeeds = false;
            return store;
        }

        //serial constructor
        public TreasureHuntersKey(Serial serial) : base(serial)
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