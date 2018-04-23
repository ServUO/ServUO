using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item inherited from BaseResourceKey
    public class ChefKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ResourceEntry(typeof(SackFlour),"Flour",0,25,0,0));
                entry.Add(new ResourceEntry(typeof(Eggs),"Eggs"));
                entry.Add(new ResourceEntry(typeof(JarHoney),"Jar of Honey"));
                entry.Add(new BeverageEntry(typeof(Pitcher),BeverageType.Water,"Water",0,20,-3,0));
                entry.Add(new ResourceEntry(typeof(Dough),"Dough"));
                entry.Add(new ResourceEntry(typeof(SweetDough),"Sweet Dough"));
                entry.Add(new ResourceEntry(typeof(CakeMix),"Cake Mix"));
                entry.Add(new ResourceEntry(typeof(CookieMix),"Cookie Mix"));
                entry.Add(new ResourceEntry(typeof(Apple),"Apple"));
                entry.Add(new ResourceEntry(typeof(Pear),"Pear"));
                entry.Add(new ResourceEntry(typeof(Peach),"Peach"));
                entry.Add(new ResourceEntry(typeof(Pumpkin),"Pumpkin"));
                entry.Add(new ResourceEntry(typeof(WoodenBowlOfCarrots),"Bowl of Carrots"));
                entry.Add(new ResourceEntry(typeof(WoodenBowlOfCorn),"Bowl of Corn"));
                entry.Add(new ResourceEntry(typeof(WoodenBowlOfLettuce),"Bowl of Lettuce"));
                entry.Add(new ResourceEntry(typeof(WoodenBowlOfPeas),"Bowl of Peas"));
                entry.Add(new ResourceEntry(typeof(WoodenBowlOfStew),"Bowl of Stew"));
                entry.Add(new ResourceEntry(typeof(TribalBerry),"Tribal Berry"));
                entry.Add(new ResourceEntry(typeof(CheeseWheel),"Cheese Wheel"));

                return entry;
            }
        }

        [Constructable]
        public ChefKey() : base(0x0)    // hue 5
        {
            ItemID = 0x9ED;             //cauldron
            Name = "Chef's Storage";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Chef's Tools";

            store.OfferDeeds = false;
            return store;
        }

        //serial constructor
        public ChefKey(Serial serial) : base(serial)
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