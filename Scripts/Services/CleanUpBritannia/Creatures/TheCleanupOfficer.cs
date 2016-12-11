using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Gumps;
using System.Collections;
using Server.Network;
using Server.Engines.Points;

namespace Server.Engines.CleanUpBritannia
{
    public class TheCleanupOfficer : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool IsInvulnerable { get { return true; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [Constructable]
        public TheCleanupOfficer()
            : base("the Cleanup Officer")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            HairItemID = 0x2044;
            HairHue = 1644;
            FacialHairItemID = 0x203F;
            FacialHairHue = 1644;
        }

        public override void InitOutfit()
        {
            SetWearable(new Cloak(), 337);
            SetWearable(new ThighBoots());
            SetWearable(new LongPants(), 1409);
            SetWearable(new Doublet(), 50);
            SetWearable(new FancyShirt(), 1644);
            SetWearable(new Necklace());

            if (Backpack == null)
            {
                Item backpack = new Backpack();
                backpack.Movable = false;
                AddItem(backpack);
            }    
        }        

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1151317); // Clean Up Britannia Reward Trader
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(this.Location, 5))
                from.SendGump(new CleanUpBritanniaRewardGump(this, from as PlayerMobile));
        }

        public TheCleanupOfficer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }    
}
