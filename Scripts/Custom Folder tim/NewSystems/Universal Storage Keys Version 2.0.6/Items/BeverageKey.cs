using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class BeverageKey : BaseStoreKey, IRewardItem
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

                //TODO: this may need some fixing up
                entry.Add(new ResourceEntry(typeof(Pitcher),"Empty Pitcher",0,20,-3,0));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Ale,"Ale",0,20,-3,0));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Cider,"Cider",0,20,-3,0));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Liquor,"Liquor",0,20,-3,0));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Milk,"Milk",0,20,-3,0));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Water,"Water",0,20,-3,0));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Wine,"Wine",0,20,-3,0));

                return entry;
            }
        }

        [Constructable]
        public BeverageKey() : base(0x0)        //hue 701
        {
            ItemID = 0x9A96;            //Machine Coffee
            Name = "Beverage Store";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Beverage Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;

            return store;
        }

        //serial constructor
        public BeverageKey(Serial serial) : base(serial)
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