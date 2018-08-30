#region References

using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;

#endregion

namespace Server.Items
{
    public class DoorBell : Item, ISecurable // Create the item class which is derived from the base item class
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public override string DefaultName { get { return "a doorbell"; } }

        [Constructable]
        public DoorBell()
            : base(19548)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Level = SecureLevel.CoOwners;
        }

        public DoorBell(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            reader.ReadInt();
        }

        public bool CheckAccess(Mobile m)
        {
            if (!IsLockedDown || m.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
            {
                return false;
            }

            return house != null && house.HasSecureAccess(m, Level);
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (IsLockedDown && CheckAccess(from))
            {
                from.SendGump(new DoorBellUI(this, MusicNames, MusicLabels));
            }
            else if (!IsLockedDown)
            {
                from.SendMessage(54, "This must be locked down for you to use it.");
            }
            else
            {
                from.SendMessage(54, "You must be coowned to this house to use the doorbell.");
            }
        }

        private static readonly Dictionary<MusicName, int> m_Info = new Dictionary<MusicName, int>();

        private List<MusicName> MusicNames = new List<MusicName>()
        {
            MusicName.Samlethe,
            MusicName.Sailing,
            MusicName.Britain2,
            MusicName.Britain1,
            MusicName.Bucsden,
            MusicName.Forest_a,
            MusicName.Cove,
            MusicName.Death,
            MusicName.Dungeon9,
            MusicName.Dungeon2,
            MusicName.Cave01,
            MusicName.Combat3,
            MusicName.Combat1,
            MusicName.Combat2,
            MusicName.Jhelom,
            MusicName.Linelle, 
            MusicName.LBCastle, 
            MusicName.Minoc, 
            MusicName.Moonglow,
            MusicName.Magincia,
            MusicName.Nujelm,
            MusicName.BTCastle,
            MusicName.Tavern04,
            MusicName.Skarabra,
            MusicName.Stones2,
            MusicName.Serpents,
            MusicName.Taiko,
            MusicName.Tavern01,
            MusicName.Tavern02,
            MusicName.Tavern03,
            MusicName.TokunoDungeon,
            MusicName.Trinsic,
            MusicName.OldUlt01,
            MusicName.Ocllo,
            MusicName.Vesper,
            MusicName.Victory,
            MusicName.Mountn_a,
            MusicName.Wind,
            MusicName.Yew,
            MusicName.Zento,

            MusicName.GwennoConversation,
            MusicName.DreadHornArea,
            MusicName.ElfCity,
            MusicName.GoodEndGame,
            MusicName.GoodVsEvil,
            MusicName.GreatEarthSerpents,
            MusicName.GrizzleDungeon,
            MusicName.Humanoids_U9,
            MusicName.MelisandesLair,
            MusicName.MinocNegative,
            MusicName.ParoxysmusLair,
            MusicName.Paws,

            MusicName.SelimsBar,
            MusicName.SerpentIsleCombat_U7,
            MusicName.ValoriaShips
        };

        private List<int> MusicLabels = new List<int>()
        {
            1075152,
            1075163,
            1075145,
            1075144,
            1075146,
            1075161,
            1075176,
            1075171,
            1075160,
            1075175,
            1075159,
            1075170,
            1075168,
            1075169,
            1075147,
            1075185,
            1075148,
            1075150,
            1075177,
            1075149,
            1075174,
            1075173,
            1075167,
            1075154,
            1075143,
            1075153,
            1075180,
            1075164,
            1075165,
            1075166,
            1075179,
            1075155,
            1075142,
            1075151,
            1075156,
            1075172,
            1075162,
            1075157,
            1075158,
            1075178,
            1075131,
            1075181,
            1075182,
            1075132,
            1075133,
            1075134,
            1075186,
            1075135,
            1075183,
            1075136,
            1075184,
            1075137,
            1075138,
            1075139,
            1075140,
        };
    }
}