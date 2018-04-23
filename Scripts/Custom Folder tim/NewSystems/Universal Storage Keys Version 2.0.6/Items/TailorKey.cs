using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class TailorKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ResourceEntry(typeof(Leather),new Type[] { typeof(Hides) },"Leather",0,30,0,0));
                entry.Add(new ResourceEntry(typeof(SpinedLeather),new Type[] { typeof(SpinedHides) },"Spined Leather",0,30,0,0));
                entry.Add(new ResourceEntry(typeof(HornedLeather),new Type[] { typeof(HornedHides) },"Horned Leather",0,30,0,0));
                entry.Add(new ResourceEntry(typeof(BarbedLeather),new Type[] { typeof(BarbedHides) },"Barbed Leather",0,30,0,0));
                entry.Add(new ColumnSeparationEntry());
				
				//entry.Add(new ResourceEntry(typeof(PolarLeather),new Type[] { typeof(PolarHides) },"Polar Leather",0,30,0,0));  
				//entry.Add(new ResourceEntry(typeof(SyntheticLeather),new Type[] { typeof(SyntheticHides) },"Synthetic Leather",0,30,0,0));
				//entry.Add(new ResourceEntry(typeof(BlazeLeather),new Type[] { typeof(BlazeHides) },"Blaze Leather",0,30,0,0));
				//entry.Add(new ResourceEntry(typeof(DaemonicLeather),new Type[] { typeof(DaemonicHides) },"Daemonic Leather",0,30,0,0));
				//entry.Add(new ResourceEntry(typeof(ShadowLeather),new Type[] { typeof(ShadowHides) },"Shadow Leather",0,30,0,0));
				//entry.Add(new ResourceEntry(typeof(FrostLeather),new Type[] { typeof(FrostHides) },"Frost Leather",0,30,0,0));
				//entry.Add(new ResourceEntry(typeof(EtherealLeather),new Type[] { typeof(EtherealHides) },"Ethereal Leather",0,30,0,0));
				

                entry.Add(new ResourceEntry(typeof(UncutCloth),new Type[] { typeof(Cloth) },"Cloth",0,30,0,0));
                entry.Add(new ResourceEntry(typeof(BoltOfCloth),"Bolt of Cloth",0,50,0,0));
                entry.Add(new ResourceEntry(typeof(LightYarn),new Type[] { typeof(DarkYarn),typeof(LightYarnUnraveled) },"Yarn"));
                entry.Add(new ResourceEntry(typeof(SpoolOfThread),"Spool of Thread"));
                entry.Add(new ResourceEntry(typeof(Wool),"Wool"));
                entry.Add(new ResourceEntry(typeof(Cotton),"Cotton"));
                entry.Add(new ResourceEntry(typeof(Flax),"Flax"));
                entry.Add(new ColumnSeparationEntry());

                entry.Add(new ResourceEntry(typeof(RedScales),"Red Scales"));
                entry.Add(new ResourceEntry(typeof(YellowScales),"Yellow Scales"));
                entry.Add(new ResourceEntry(typeof(BlackScales),"Black Scales"));
                entry.Add(new ResourceEntry(typeof(GreenScales),"Green Scales"));
                entry.Add(new ResourceEntry(typeof(WhiteScales),"White Scales"));
                entry.Add(new ResourceEntry(typeof(BlueScales),"Blue Scales"));
				
				 //entry.Add(new ResourceEntry(typeof(CopperScales),"Copper Scales"));
				 //entry.Add(new ResourceEntry(typeof(SilverScales),"Silver Scales"));
				 //entry.Add(new ResourceEntry(typeof(GoldScales),"Gold Scales"));

                return entry;
            }
        }

        [Constructable]
        public TailorKey() : base(0x0)      // hue 68
        {
            ItemID = 3997;          //sewingkit
            Name = "Tailor Store";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Tailor Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;
            return store;
        }

        //serial constructor
        public TailorKey(Serial serial) : base(serial)
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