using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class ASHKey : BaseStoreKey, IRewardItem
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

                entry.Add(new AncientSmithyHammerToolEntry(10,"+10 ASH",0,20,0,0));
                entry.Add(new AncientSmithyHammerToolEntry(15,"+15 ASH",0,20,0,0));
                entry.Add(new AncientSmithyHammerToolEntry(30,"+30 ASH",0,20,0,0));
                entry.Add(new AncientSmithyHammerToolEntry(60,"+60 ASH",0,20,0,0));

                return entry;
            }
        }

        [Constructable]
        public ASHKey() : base(0x0) //hue 1159
        {
            ItemID = 0x1EBB;			//square toolbox

            Name = "ASH Box";

            //ASH tools withdrawn can have no less than 5 charges on them.
            _Store.MinWithdrawAmount = 5;
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Ancient Smithy Hammer Tool Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;

            return store;
        }

        //serial constructor
        public ASHKey(Serial serial) : base(serial)
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