using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public sealed class StopMusic : Packet
    {
        public static readonly Packet Instance = Packet.SetStatic(new StopMusic());
        public StopMusic()
            : base(0x6D, 3)
        {
            this.m_Stream.Write((short)0x1FFF);
        }
    }

    [Flipable(0x2AF9, 0x2AFD)]
    public class DawnsMusicBox : Item, ISecurable
    {
        public static MusicName[] m_CommonTracks = new MusicName[]
        {
            MusicName.Samlethe, MusicName.Sailing, MusicName.Britain2, MusicName.Britain1,
            MusicName.Bucsden, MusicName.Forest_a, MusicName.Cove, MusicName.Death,
            MusicName.Dungeon9, MusicName.Dungeon2, MusicName.Cave01, MusicName.Combat3,
            MusicName.Combat1, MusicName.Combat2, MusicName.Jhelom, MusicName.Linelle,
            MusicName.LBCastle, MusicName.Minoc, MusicName.Moonglow, MusicName.Magincia,
            MusicName.Nujelm, MusicName.BTCastle, MusicName.Tavern04, MusicName.Skarabra,
            MusicName.Stones2, MusicName.Serpents, MusicName.Taiko, MusicName.Tavern01,
            MusicName.Tavern02, MusicName.Tavern03, MusicName.TokunoDungeon, MusicName.Trinsic,
            MusicName.OldUlt01, MusicName.Ocllo, MusicName.Vesper, MusicName.Victory,
            MusicName.Mountn_a, MusicName.Wind, MusicName.Yew, MusicName.Zento
        };
        public static MusicName[] m_UncommonTracks = new MusicName[]
        {
            MusicName.GwennoConversation, MusicName.DreadHornArea, MusicName.ElfCity,
            MusicName.GoodEndGame, MusicName.GoodVsEvil, MusicName.GreatEarthSerpents,
            MusicName.GrizzleDungeon, MusicName.Humanoids_U9, MusicName.MelisandesLair,
            MusicName.MinocNegative, MusicName.ParoxysmusLair, MusicName.Paws
        };
        public static MusicName[] m_RareTracks = new MusicName[]
        {
            MusicName.SelimsBar, MusicName.SerpentIsleCombat_U7, MusicName.ValoriaShips
        };
        private static readonly Dictionary<MusicName, DawnsMusicInfo> m_Info = new Dictionary<MusicName, DawnsMusicInfo>();
        private List<MusicName> m_Tracks;
        private SecureLevel m_Level;
        private Timer m_Timer;
        private int m_ItemID = 0;
        private int m_Count = 0;
        [Constructable]
        public DawnsMusicBox()
            : base(0x2AF9)
        {
            this.Weight = 1.0;

            this.m_Tracks = new List<MusicName>();

            while (this.m_Tracks.Count < 4)
            {
                MusicName name = RandomTrack(DawnsMusicRarity.Common);

                if (!this.m_Tracks.Contains(name))
                    this.m_Tracks.Add(name);
            }
        }

        public DawnsMusicBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075198;
            }
        }// Dawn’s Music Box
        public List<MusicName> Tracks
        {
            get
            {
                return this.m_Tracks;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        public static void Initialize()
        {
            m_Info.Add(MusicName.Samlethe, new DawnsMusicInfo(1075152, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Sailing, new DawnsMusicInfo(1075163, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Britain2, new DawnsMusicInfo(1075145, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Britain1, new DawnsMusicInfo(1075144, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Bucsden, new DawnsMusicInfo(1075146, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Forest_a, new DawnsMusicInfo(1075161, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Cove, new DawnsMusicInfo(1075176, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Death, new DawnsMusicInfo(1075171, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Dungeon9, new DawnsMusicInfo(1075160, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Dungeon2, new DawnsMusicInfo(1075175, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Cave01, new DawnsMusicInfo(1075159, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Combat3, new DawnsMusicInfo(1075170, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Combat1, new DawnsMusicInfo(1075168, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Combat2, new DawnsMusicInfo(1075169, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Jhelom, new DawnsMusicInfo(1075147, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Linelle, new DawnsMusicInfo(1075185, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.LBCastle, new DawnsMusicInfo(1075148, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Minoc, new DawnsMusicInfo(1075150, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Moonglow, new DawnsMusicInfo(1075177, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Magincia, new DawnsMusicInfo(1075149, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Nujelm, new DawnsMusicInfo(1075174, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.BTCastle, new DawnsMusicInfo(1075173, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Tavern04, new DawnsMusicInfo(1075167, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Skarabra, new DawnsMusicInfo(1075154, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Stones2, new DawnsMusicInfo(1075143, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Serpents, new DawnsMusicInfo(1075153, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Taiko, new DawnsMusicInfo(1075180, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Tavern01, new DawnsMusicInfo(1075164, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Tavern02, new DawnsMusicInfo(1075165, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Tavern03, new DawnsMusicInfo(1075166, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.TokunoDungeon, new DawnsMusicInfo(1075179, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Trinsic, new DawnsMusicInfo(1075155, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.OldUlt01, new DawnsMusicInfo(1075142, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Ocllo, new DawnsMusicInfo(1075151, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Vesper, new DawnsMusicInfo(1075156, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Victory, new DawnsMusicInfo(1075172, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Mountn_a, new DawnsMusicInfo(1075162, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Wind, new DawnsMusicInfo(1075157, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Yew, new DawnsMusicInfo(1075158, DawnsMusicRarity.Common));
            m_Info.Add(MusicName.Zento, new DawnsMusicInfo(1075178, DawnsMusicRarity.Common));

            m_Info.Add(MusicName.GwennoConversation, new DawnsMusicInfo(1075131, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.DreadHornArea, new DawnsMusicInfo(1075181, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.ElfCity, new DawnsMusicInfo(1075182, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.GoodEndGame, new DawnsMusicInfo(1075132, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.GoodVsEvil, new DawnsMusicInfo(1075133, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.GreatEarthSerpents, new DawnsMusicInfo(1075134, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.GrizzleDungeon, new DawnsMusicInfo(1075186, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.Humanoids_U9, new DawnsMusicInfo(1075135, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.MelisandesLair, new DawnsMusicInfo(1075183, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.MinocNegative, new DawnsMusicInfo(1075136, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.ParoxysmusLair, new DawnsMusicInfo(1075184, DawnsMusicRarity.Uncommon));
            m_Info.Add(MusicName.Paws, new DawnsMusicInfo(1075137, DawnsMusicRarity.Uncommon));

            m_Info.Add(MusicName.SelimsBar, new DawnsMusicInfo(1075138, DawnsMusicRarity.Rare));
            m_Info.Add(MusicName.SerpentIsleCombat_U7, new DawnsMusicInfo(1075139, DawnsMusicRarity.Rare));
            m_Info.Add(MusicName.ValoriaShips, new DawnsMusicInfo(1075140, DawnsMusicRarity.Rare));
        }

        public static DawnsMusicInfo GetInfo(MusicName name)
        {
            if (m_Info.ContainsKey(name))
                return m_Info[name];

            return null;
        }

        public static MusicName RandomTrack(DawnsMusicRarity rarity)
        {
            MusicName[] list = null;

            switch ( rarity )
            {
                default:
                case DawnsMusicRarity.Common:
                    list = m_CommonTracks;
                    break;
                case DawnsMusicRarity.Uncommon:
                    list = m_UncommonTracks;
                    break;
                case DawnsMusicRarity.Rare:
                    list = m_RareTracks;
                    break;
            }

            return list[Utility.Random(list.Length)];
        }

        public override void OnAfterDuped(Item newItem)
        {
            DawnsMusicBox box = newItem as DawnsMusicBox;

            if (box == null)
                return;

            box.m_Tracks = new List<MusicName>();
            box.m_Tracks.AddRange(this.m_Tracks);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int commonSongs = 0;
            int uncommonSongs = 0;
            int rareSongs = 0;

            for (int i = 0; i < this.m_Tracks.Count; i++)
            {
                DawnsMusicInfo info = GetInfo(this.m_Tracks[i]);

                switch ( info.Rarity )
                {
                    case DawnsMusicRarity.Common:
                        commonSongs++;
                        break;
                    case DawnsMusicRarity.Uncommon:
                        uncommonSongs++;
                        break;
                    case DawnsMusicRarity.Rare:
                        rareSongs++;
                        break;
                }
            }

            if (commonSongs > 0)
                list.Add(1075234, commonSongs.ToString()); // ~1_NUMBER~ Common Tracks
            if (uncommonSongs > 0)
                list.Add(1075235, uncommonSongs.ToString()); // ~1_NUMBER~ Uncommon Tracks
            if (rareSongs > 0)
                list.Add(1075236, rareSongs.ToString()); // ~1_NUMBER~ Rare Tracks
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list); // Set secure level
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack) && !this.IsLockedDown)
                from.SendLocalizedMessage(1061856); // You must have the item in your backpack or locked down in order to use it.
            else if (this.IsLockedDown && !this.HasAccces(from))
                from.SendLocalizedMessage(502436); // That is not accessible.
            else
            {
                from.CloseGump(typeof(DawnsMusicBoxGump));
                from.SendGump(new DawnsMusicBoxGump(this));
            }
        }

        public bool HasAccces(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.HasAccess(m));
        }

        public void PlayMusic(Mobile m, MusicName music)
        {
            if (this.m_Timer != null && this.m_Timer.Running)
                this.EndMusic(m);
            else
                this.m_ItemID = this.ItemID;

            m.Send(new PlayMusic(music));
            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 4, new TimerCallback(Animate));
        }

        public void EndMusic(Mobile m)
        {
            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();

            m.Send(StopMusic.Instance);

            if (this.m_Count > 0)
                this.ItemID = this.m_ItemID;

            this.m_Count = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)this.m_Tracks.Count);

            for (int i = 0; i < this.m_Tracks.Count; i++)
                writer.Write((int)this.m_Tracks[i]);

            writer.Write((int)this.m_Level);
            writer.Write((int)this.m_ItemID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            int count = reader.ReadInt();
            this.m_Tracks = new List<MusicName>();

            for (int i = 0; i < count; i++)
                this.m_Tracks.Add((MusicName)reader.ReadInt());

            this.m_Level = (SecureLevel)reader.ReadInt();
            this.m_ItemID = reader.ReadInt();
        }

        private void Animate()
        {
            this.m_Count++;

            if (this.m_Count >= 4)
            {
                this.m_Count = 0;
                this.ItemID = this.m_ItemID;
            }
            else
                this.ItemID++;
        }
    }
}