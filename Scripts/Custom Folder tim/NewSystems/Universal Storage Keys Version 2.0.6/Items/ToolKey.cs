using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class ToolKey : BaseStoreKey, IRewardItem
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

                entry.Add(new ToolEntry(typeof(TinkerTools),"Tinkering",0,30,0,0));
				entry.Add(new ToolEntry(typeof(TinkersTools), "Tinkering", 0, 30, 0, 0));
				entry.Add(new ToolEntry(typeof(SmithHammer),new Type[] { typeof(Tongs),typeof(SledgeHammer) },"Blacksmithy",0,30,-5,0));
                entry.Add(new ToolEntry(typeof(SewingKit),"Tailoring",0,30,-5,3));
                entry.Add(new ToolEntry(typeof(Nails),new Type[] { typeof(Saw),typeof(Hammer),typeof(Scorp),typeof(DrawKnife),typeof(DovetailSaw),typeof(Froe),typeof(Inshave),typeof(JointingPlane) },"Carpentry",0,30,-10,5));
                entry.Add(new ToolEntry(typeof(FletcherTools),"Fletching",0,35,0,0));
                entry.Add(new ToolEntry(typeof(MortarPestle),"Alchemy",0,30,0,0));
                entry.Add(new ToolEntry(typeof(ScribesPen),"Inscription",0,30,0,0));
                entry.Add(new ToolEntry(typeof(Skillet),new Type[] { typeof(FlourSifter),typeof(RollingPin) },"Cooking",0,30,0,0));
                entry.Add(new ColumnSeparationEntry());
                entry.Add(new ToolEntry(typeof(MalletAndChisel),"Stone Crafting",0,30,0,0));
                entry.Add(new ToolEntry(typeof(Shovel),new Type[] { typeof(SturdyShovel),typeof(Pickaxe),typeof(SturdyPickaxe) },"Mining",0,35,-10,-10));
                entry.Add(new ToolEntry(typeof(Hatchet),"Lumberjacking",0,30,-5,0));
                entry.Add(new ToolEntry(typeof(GargoylesPickaxe),"Gargoyle's Pickaxe",0,30,-5,-5));
                entry.Add(new ToolEntry(typeof(ProspectorsTool),"Prospector's Tool",0,30,0,0));
                entry.Add(new ToolEntry(typeof(MapmakersPen),"Cartography",0,30,0,0));
                entry.Add(new ToolEntry(typeof(Blowpipe),"Glassblowing",0,40,0,0));
                entry.Add(new ToolEntry(typeof(TaxidermyKit),"Taxidermy",0,30,0,0));

                return entry;
            }
        }

        [Constructable]
        public ToolKey() : base(0x0)    //hue 45
        {
            ItemID = 7867;          //toolbox
            Name = "Tool Box";

            //tools withdrawn can have no less than 50 charges on them.
            _Store.MinWithdrawAmount = 50;
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Tool Storage";

            store.Dynamic = false;
            store.OfferDeeds = true;

            return store;
        }

        //serial constructor
        public ToolKey(Serial serial) : base(serial)
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