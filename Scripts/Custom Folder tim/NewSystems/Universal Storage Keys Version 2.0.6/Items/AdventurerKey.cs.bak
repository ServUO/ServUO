using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class AdventurerKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ResourceEntry(typeof(Bandage),"Bandage"));
                entry.Add(new ResourceEntry(typeof(Lockpick),"Lockpick"));
                entry.Add(new ResourceEntry(typeof(ZoogiFungus),"Zoogi Fungus"));
                entry.Add(new ResourceEntry(typeof(PowderOfTranslocation),"Powder of Trans."));
                entry.Add(new ResourceEntry(typeof(GreenThorns),"Green Thorns"));
                entry.Add(new ResourceEntry(typeof(FertileDirt),"Fertile Dirt"));
                entry.Add(new ResourceEntry(typeof(BolaBall),"Bola Balls"));
                entry.Add(new ResourceEntry(typeof(Bola),"Bola"));
                entry.Add(new ResourceEntry(typeof(ArcaneGem),"Arcane Gems"));

                return entry;
            }
        }

        [Constructable]
        public AdventurerKey() : base(0x0)          // hue 1151
        {
            ItemID = 0x170B;                // crate
            Name = "Adventurer's Boots";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Adventurer's Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;

            return store;
        }

        //serial constructor
        public AdventurerKey(Serial serial) : base(serial)
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