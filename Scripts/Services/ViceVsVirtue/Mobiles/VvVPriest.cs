using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Guilds;
using System.Collections.Generic;

namespace Server.Engines.VvV
{
    public class VvVPriest : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        protected List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }
        public override void InitSBInfo() { }

        [CommandProperty(AccessLevel.GameMaster)]
        public VvVType VvVType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public VvVBattle Battle { get; set; }

        [Constructable]
        public VvVPriest(VvVType type, VvVBattle battle) : base(type == VvVType.Vice ? "the Priest of Vice" : "the Priest of Virtue")
        {
            this.VvVType = type;
            Battle = battle;
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if(ViceVsVirtueSystem.Instance == null || Battle == null)
                return false;

            VvVPlayerEntry entry = ViceVsVirtueSystem.Instance.GetPlayerEntry<VvVPlayerEntry>(from);

            if (from.InRange(Location, 2) && entry != null && ViceVsVirtueSystem.IsVvV(from) && dropped is VvVSigil)
            {
                VvVSigil sigil = dropped as VvVSigil;
                Battle.Update(null, entry, this.VvVType == VvVType.Vice ? UpdateType.TurnInVice : UpdateType.TurnInVirtue);

                sigil.Delete();
                Battle.Sigil = null;
            }

            return false;
        }

        public override void InitOutfit()
        {
            Robe robe = new Robe();
            robe.ItemID = 9860;
            robe.Name = this.VvVType == VvVType.Virtue ? "Robe of Virtue" : "Robe of Vice";

            SetWearable(robe, this.VvVType == VvVType.Virtue ? 1462 : 1461); // TODO: Get Hues
        }

        public VvVPriest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)VvVType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.VvVType = (VvVType)reader.ReadInt();
        }
    }
}