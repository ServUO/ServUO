using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class WoodKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ResourceEntry(typeof(Board),new Type[] { typeof(Log) },"Plain"));

                #region Mondain's Legacy
                entry.Add(new ResourceEntry(typeof(OakBoard),new Type[] { typeof(OakLog) },"Oak Board"));
                entry.Add(new ResourceEntry(typeof(AshBoard),new Type[] { typeof(AshLog) },"Ash Board"));
                entry.Add(new ResourceEntry(typeof(YewBoard),new Type[] { typeof(YewLog) },"Yew Board"));
                entry.Add(new ResourceEntry(typeof(HeartwoodBoard),new Type[] { typeof(HeartwoodLog) },"Heartwood Board"));
                entry.Add(new ResourceEntry(typeof(BloodwoodBoard),new Type[] { typeof(BloodwoodLog) },"Bloodwood Board"));
                entry.Add(new ResourceEntry(typeof(FrostwoodBoard),new Type[] { typeof(FrostwoodLog) },"Frostwood Board"));
                #endregion
				
				//entry.Add(new ResourceEntry(typeof(EbonyBoard),new Type[] { typeof(EbonyLog) },"Ebony Board"));
				//entry.Add(new ResourceEntry(typeof(BambooBoard),new Type[] { typeof(BambooLog) },"Bamboo Board"));
				//entry.Add(new ResourceEntry(typeof(PurpleHeartBoard),new Type[] { typeof(PurpleHeartLog) },"PurpleHeart Board"));
				//entry.Add(new ResourceEntry(typeof(RedwoodBoard),new Type[] { typeof(RedwoodLog) },"Redwood Board"));
                //entry.Add(new ResourceEntry(typeof(Kindling),"Kindling"));
                entry.Add(new ResourceEntry(typeof(Shaft),"Shaft"));
                entry.Add(new ResourceEntry(typeof(Feather),"Feather"));
                entry.Add(new ResourceEntry(typeof(Arrow),"Arrow"));
                entry.Add(new ResourceEntry(typeof(Bolt),"Bolt"));

                return entry;
            }
        }

        [Constructable]
        public WoodKey() : base(0x0)        //hue 88
        {
            ItemID = 0x1BD9;            //pile of wood
            Name = "Wood Storage";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Wood Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;

            return store;
        }

        //serial constructor
        public WoodKey(Serial serial) : base(serial)
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

            //writer.Write( 0 );
            writer.Write((int)0); // version

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