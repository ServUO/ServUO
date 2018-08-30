using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item inherited from BaseResourceKey
    [Flipable(0x26BB,0x26C5)]
    public class MeatKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ResourceEntry(typeof(RawRibs),"Raw Ribs"));
                entry.Add(new ResourceEntry(typeof(RawLambLeg),"Raw Lamb Leg"));
                entry.Add(new ResourceEntry(typeof(RawChickenLeg),"Raw Chicken Leg"));
                entry.Add(new ResourceEntry(typeof(RawBird),"Raw Bird"));
                entry.Add(new ResourceEntry(typeof(RawFishSteak),"Raw Fishsteak"));
                entry.Add(new ResourceEntry(typeof(Bacon),"Bacon"));
                entry.Add(new ResourceEntry(typeof(SlabOfBacon),"Slab of Bacon"));
                entry.Add(new ResourceEntry(typeof(FishSteak),"Fishsteak"));
                entry.Add(new ResourceEntry(typeof(CookedBird),"Cooked Bird"));
                entry.Add(new ResourceEntry(typeof(Sausage),"Sausage"));
                entry.Add(new ResourceEntry(typeof(Ribs),"Ribs"));
                entry.Add(new ResourceEntry(typeof(Ham),"Ham"));
                entry.Add(new ResourceEntry(typeof(Ribs),"Ribs"));
                entry.Add(new ResourceEntry(typeof(LambLeg),"Lamb Leg"));
                entry.Add(new ResourceEntry(typeof(ChickenLeg),"Chicken Leg"));

                return entry;
            }
        }

        [Constructable]
        public MeatKey() : base(0x0)        // 1687
        {
            ItemID = 0x26BB;                //bone harvester
            Name = "Butcher's Hook";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Butcher's Tools";

            store.OfferDeeds = false;
            return store;
        }

        //serial constructor
        public MeatKey(Serial serial) : base(serial)
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