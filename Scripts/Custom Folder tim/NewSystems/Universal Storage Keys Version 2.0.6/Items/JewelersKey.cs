using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Solaris.ItemStore;							//for connection to resource store data objects
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    //item derived from BaseResourceKey
    public class JewelersKey : BaseStoreKey, IRewardItem
    {
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.Seer)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        //set the # of columns of entries to display on the gump.. default is 2
        public override int DisplayColumns { get { return 1; } }

        public override List<StoreEntry> EntryStructure
        {
            get
            {
                List<StoreEntry> entry = base.EntryStructure;

                #region Mondain's Legacy
                entry.Add(new ResourceEntry(typeof(DreadHornMane),"Dread Horn Mane",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(Putrefication),"Putrefaction",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(Taint),"Taint",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(Scourge),"Scourge",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(Muculent),"Muculent",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(CapturedEssence),"Captured Essence",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(Blight),"Blight",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(LardOfParoxysmus),"Lard Of Paroxysmus",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(EyeOfTheTravesty),"Eye Of The Travesty",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(BlueDiamond),"Blue Diamond",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(DarkSapphire),"Dark Sapphire",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(EcruCitrine),"Ecru Citrine",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(FireRuby),"Fire Ruby",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(PerfectEmerald),"Perfect Emerald",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(Turquoise),"Turquoise",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(WhitePearl),"White Pearl",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(BrilliantAmber),"Brilliant Amber",0,30,7,8));
                entry.Add(new ResourceEntry(typeof(LuminescentFungi),"Luminescent Fungi",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(SwitchItem),"Switch",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(ParasiticPlant),"Parasitic Plant",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(Corruption),"Corruption",0,30,5,8));
                entry.Add(new ResourceEntry(typeof(GrizzledBones),"Grizzled Bones",0,30,3,8));
                entry.Add(new ResourceEntry(typeof(BarkFragment),"Bark Fragment",0,30,5,8));
                #endregion

                entry.Add(new ResourceEntry(typeof(Amber),"Amber",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Amethyst),"Amethyst",0,25,-11,9));
                entry.Add(new ResourceEntry(typeof(Citrine),"Citrine",0,25,-13,9));
                entry.Add(new ResourceEntry(typeof(Diamond),"Diamond",0,25,-5,9));
                entry.Add(new ResourceEntry(typeof(Emerald),"Emerald",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Ruby),"Ruby",0,25,7,9));
                entry.Add(new ResourceEntry(typeof(Sapphire),"Sapphire",0,25,6,9));
                entry.Add(new ResourceEntry(typeof(StarSapphire),"Star Sapphire",0,25,-7,9));
                entry.Add(new ResourceEntry(typeof(Tourmaline),"Tourmaline",0,25,11,8));

                return entry;
            }
        }

        [Constructable]
        public JewelersKey() : base(0x0)        //hue 1154
        {
            Name = "Jeweler's Keys";
        }

        //this loads properties specific to the store, like the gump label, and whether it's a dynamic storage device
        protected override ItemStore GenerateItemStore()
        {
            //load the basic store info
            ItemStore store = base.GenerateItemStore();

            //properties of this storage device
            store.Label = "Jewel Storage";

            store.Dynamic = false;
            store.OfferDeeds = false;

            return store;
        }

        //serial constructor
        public JewelersKey(Serial serial) : base(serial)
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