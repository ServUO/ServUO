using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class SpecialAmmoKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ResourceEntry(typeof(PoisonArrow),"Poison Arrow"));
				entry.Add(new ResourceEntry(typeof(ExplosiveArrow),"Explosive Arrow"));
				entry.Add(new ResourceEntry(typeof(ArmorPiercingArrow),"Arm Pierce Ar"));
				entry.Add(new ResourceEntry(typeof(FreezeArrow),"Freeze Arrow"));
				entry.Add(new ResourceEntry(typeof(LightningArrowAmmo),"Lightning Arrow"));
				entry.Add(new ResourceEntry(typeof(PoisonBolt),"Poison Bolt"));
				entry.Add(new ResourceEntry(typeof(ExplosiveBolt),"Explosive Bolt"));
				entry.Add(new ResourceEntry(typeof(ArmorPiercingBolt),"Arm Pierce Bo"));
				entry.Add(new ResourceEntry(typeof(FreezeBolt),"Freeze Bolt"));
				entry.Add(new ResourceEntry(typeof(LightningBolt),"Lightning Bolt"));
				
                return entry;
            }
        }

        [Constructable]
        public SpecialAmmoKey() : base(0x0)          // hue 1151
        {
            ItemID = 0x170B;                // crate
            Name = "Special Arrow/Bolt Key";
			this.Hue = 2323;
			LootType = LootType.Blessed;
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
        public SpecialAmmoKey(Serial serial) : base(serial)
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