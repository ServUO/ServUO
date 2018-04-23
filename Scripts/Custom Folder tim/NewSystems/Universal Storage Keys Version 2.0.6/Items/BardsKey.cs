using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class BardsKey : BaseStoreKey, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.Seer)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        public override int DisplayColumns { get { return 2; } }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

                entry.Add(new InstrumentEntry(typeof(Lute), ItemQuality.Exceptional, "Ex. Lute"));
                entry.Add(new InstrumentEntry(typeof(Tambourine), ItemQuality.Exceptional, "Ex. Tambourine"));
                entry.Add(new InstrumentEntry(typeof(TambourineTassel), ItemQuality.Exceptional, "Ex. T. Tambourine"));
                entry.Add(new InstrumentEntry(typeof(Drums), ItemQuality.Exceptional, "Ex. Drums"));
                entry.Add(new InstrumentEntry(typeof(LapHarp), ItemQuality.Exceptional, "Ex. Lap Harp", 0, 30, -10, 0));
                entry.Add(new InstrumentEntry(typeof(Harp), ItemQuality.Exceptional, "Ex. Harp", 0, 60, -40, 0));
                entry.Add(new InstrumentEntry(typeof(BambooFlute), ItemQuality.Exceptional, "Ex. Bamboo Flute"));
                entry.Add(new ListEntry(typeof(BaseInstrument),typeof(InstrumentListEntry),"Instruments"));

                return entry;
            }
        }

        [Constructable]
        public BardsKey() : base(0x0)       //hue 1152
        {
            ItemID = 0xEB6;         //music stand
            Name = "Bard's Stand";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Bard's Storage";

            store.Dynamic = false;
            store.OfferDeeds = false;
            return store;
        }

        //serial constructor
        public BardsKey(Serial serial) : base(serial)
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