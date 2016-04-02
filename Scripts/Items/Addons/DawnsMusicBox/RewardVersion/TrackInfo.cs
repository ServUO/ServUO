using System;
using System.Collections.Generic;

namespace Server.Items.MusicBox
{
    public enum TrackRarity
    {
        Common,
        UnCommon,
        Rare
    }

    public class TrackInfo
    {
        private static readonly TrackInfo[] m_Table = new TrackInfo[]
        {
            // There are currently 40 Common Music Box Gears
            new TrackInfo(56, 1075152,	MusicName.Samlethe, TrackRarity.Common),
            new TrackInfo(18, 1075163,	MusicName.Sailing, TrackRarity.Common),
            new TrackInfo(39, 1075145,	MusicName.Britain2, TrackRarity.Common),
            new TrackInfo(53, 1075144,	MusicName.Britain1, TrackRarity.Common),
            new TrackInfo(59, 1075146,	MusicName.Bucsden, TrackRarity.Common),
            new TrackInfo(240, 1075161,	MusicName.Forest_a, TrackRarity.Common),
            new TrackInfo(58, 1075176,	MusicName.Cove, TrackRarity.Common),
            new TrackInfo(20, 1075171,	MusicName.Death, TrackRarity.Common),
            new TrackInfo(50, 1075160,	MusicName.Dungeon9, TrackRarity.Common),
            new TrackInfo(58, 1075175,	MusicName.Dungeon2, TrackRarity.Common),
            new TrackInfo(58, 1075159,	MusicName.Cave01, TrackRarity.Common),
            new TrackInfo(52, 1075170,	MusicName.Combat3, TrackRarity.Common),
            new TrackInfo(50, 1075168,	MusicName.Combat1, TrackRarity.Common),
            new TrackInfo(63, 1075169,	MusicName.Combat2, TrackRarity.Common),
            new TrackInfo(36, 1075147,	MusicName.Jhelom, TrackRarity.Common),
            new TrackInfo(36, 1075185,	MusicName.Linelle, TrackRarity.Common),
            new TrackInfo(17, 1075148,	MusicName.LBCastle, TrackRarity.Common),
            new TrackInfo(66, 1075150,	MusicName.Minoc, TrackRarity.Common),
            new TrackInfo(60, 1075177,	MusicName.Moonglow, TrackRarity.Common),
            new TrackInfo(62, 1075149,	MusicName.Magincia, TrackRarity.Common),
            new TrackInfo(54, 1075174,	MusicName.Nujelm, TrackRarity.Common),
            new TrackInfo(98, 1075173,	MusicName.BTCastle, TrackRarity.Common),
            new TrackInfo(48, 1075167,	MusicName.Tavern04, TrackRarity.Common),
            new TrackInfo(48, 1075154,	MusicName.Skarabra, TrackRarity.Common),
            new TrackInfo(76, 1075143,	MusicName.Stones2, TrackRarity.Common),
            new TrackInfo(135, 1075153,	MusicName.Serpents, TrackRarity.Common),
            new TrackInfo(227, 1075180,	MusicName.Taiko, TrackRarity.Common),
            new TrackInfo(22, 1075164,	MusicName.Tavern01, TrackRarity.Common),
            new TrackInfo(18, 1075165,	MusicName.Tavern02, TrackRarity.Common),
            new TrackInfo(18, 1075166,	MusicName.Tavern03, TrackRarity.Common),
            new TrackInfo(88, 1075179,	MusicName.TokunoDungeon, TrackRarity.Common),
            new TrackInfo(61, 1075155,	MusicName.Trinsic, TrackRarity.Common),
            new TrackInfo(48, 1075142,	MusicName.OldUlt01, TrackRarity.Common),
            new TrackInfo(56, 1075151,	MusicName.Ocllo, TrackRarity.Common),
            new TrackInfo(36, 1075156,	MusicName.Vesper, TrackRarity.Common),
            new TrackInfo(17, 1075172,	MusicName.Victory, TrackRarity.Common),
            new TrackInfo(343, 1075162,	MusicName.Mountn_a, TrackRarity.Common),
            new TrackInfo(35, 1075157,	MusicName.Wind, TrackRarity.Common),
            new TrackInfo(60, 1075158,	MusicName.Yew, TrackRarity.Common),
            new TrackInfo(148, 1075178,	MusicName.Zento, TrackRarity.Common),
 			
            // There are currently 12 Uncommon Music Box Gears
            new TrackInfo(106, 1075131,	MusicName.GwennoConversation,	TrackRarity.UnCommon),
            new TrackInfo(120, 1075181,	MusicName.DreadHornArea, TrackRarity.UnCommon),
            new TrackInfo(321, 1075182,	MusicName.ElfCity, TrackRarity.UnCommon),
            new TrackInfo(128, 1075132,	MusicName.GoodEndGame, TrackRarity.UnCommon),
            new TrackInfo(114, 1075133,	MusicName.GoodVsEvil, TrackRarity.UnCommon),
            new TrackInfo(126, 1075134,	MusicName.GreatEarthSerpents,	TrackRarity.UnCommon),
            new TrackInfo(120,	1075186,	MusicName.GrizzleDungeon, TrackRarity.UnCommon),
            new TrackInfo(74, 1075135,	MusicName.Humanoids_U9, TrackRarity.UnCommon),
            new TrackInfo(120, 1075183,	MusicName.MelisandesLair, TrackRarity.UnCommon),
            new TrackInfo(68, 1075136,	MusicName.MinocNegative, TrackRarity.UnCommon),
            new TrackInfo(120, 1075184,	MusicName.ParoxysmusLair, TrackRarity.UnCommon),
            new TrackInfo(114, 1075137,	MusicName.Paws, TrackRarity.UnCommon),
 			
            // There are currently 4 Rare Music Box Gears.
            // "The Wanderer" was only available during the "Book of Truth" quest in early spring 2007.
            // So there are only 3 TrackInfo
            new TrackInfo(219, 1075138,	MusicName.SelimsBar, TrackRarity.Rare),
            new TrackInfo(285, 1075139,	MusicName.SerpentIsleCombat_U7,	TrackRarity.Rare),
            new TrackInfo(118,	1075140,	MusicName.ValoriaShips, TrackRarity.Rare)
        };
        private int m_Duration;
        private int m_Label;
        private MusicName m_Name;
        private TrackRarity m_Rarity;
        public TrackInfo(int duration, int label, MusicName name, TrackRarity rarity)
        {
            this.m_Duration = duration;
            this.m_Label = label;
            this.m_Name = name;
            this.m_Rarity = rarity;
        }

        public int Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
            }
        }
        public int Label
        {
            get
            {
                return this.m_Label;
            }
            set
            {
                this.m_Label = value;
            }
        }
        public MusicName Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public TrackRarity Rarity
        {
            get
            {
                return this.m_Rarity;
            }
            set
            {
                this.m_Rarity = value;
            }
        }
        /// <summary>
        /// Static. Method to obtain a TrackInfo from a MusicName.
        /// </summary>
        /// <param name="name"></param> MusicName to retrieve its TrackInfo</param>
        /// <returns>a TrackInfo if name is valid; first TrackInfo in the list otherwise.</returns>
        public static TrackInfo GetInfo(MusicName name)
        {
            foreach (TrackInfo ti in m_Table)
            {
                if (ti.Name == name)
                    return ti;
            }
			
            return m_Table[0];
        }

        /// <summary>
        /// Static. Method to obtain a TrackInfo from a label.
        /// </summary>
        /// <param name="name"></param> MusicName to retrieve its TrackInfo</param>
        /// <returns>a TrackInfo if label is valid; first TrackInfo in the list otherwise.</returns>
        public static TrackInfo GetInfo(int label)
        {
            foreach (TrackInfo ti in m_Table)
            {
                if (ti.Label == label)
                    return ti;
            }
			
            return m_Table[0];
        }

        /// <summary>
        /// Static. Method to obtain a MusicName with a choosen TrackRarity.
        /// </summary>
        /// <param name="rarity"></param>TrackRarity for the MusicName</param>
        /// <returns>a random MusicName available in the Dawn's music box with appropriate rarity.</returns>
        public static MusicName RandomSong(TrackRarity rarity)
        {
            List<MusicName> list = new List<MusicName>();
			
            foreach (TrackInfo ti in m_Table)
            {
                if (ti.Rarity == rarity)
                    list.Add(ti.Name);
            }
			
            int random = Utility.Random(list.Count);
			
            return list[random];
        }

        /// <summary>
        /// Static. Method to obtain a random MusicName.
        /// </summary>
        /// <returns>a random MusicName available in the Dawn's music box.</returns>
        public static MusicName RandomSong()
        {
            int random = Utility.Random(m_Table.Length);
			
            return m_Table[random].Name;
        }
    }
}