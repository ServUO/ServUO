// 2/26/08 - Scripted by Shakhan. I used many scripts as examples for this. I want to be sure to 
// give credit to Karmageddon, some of his scripts were the foundation of this. Gump was partially
// created using Gump Studio 1.8.
// 3/1/08 - Edited by Shakhan. Added page manipulation and fixed some minor errors. Thanks to haazen and
// Fenn for all the help with the page manipulation.
using System;
using System.Collections;
using Server;
using Server.Prompts;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Multis;
using Server.Regions;
using Server.Engines.Craft;
using Server.Engines.Plants;
using Server.Commands;


namespace Server.Items
{	
	public class SeedCompanion : Item
    {
        #region Integers
        
        //Page 2 Variables
        private int m_BCAqua;
		private int m_BCBlack;
		private int m_BCBlue;
		private int m_BCFRed;
		private int m_BCGreen;
		private int m_BCMagenta;
		private int m_BCOrange;
        private int m_BCPink;
		private int m_BCPlain;
		private int m_BCPurple;
        private int m_BCRed;
        private int m_BCWhite;
        private int m_BCYellow;
        private int m_BCBBlue;
        private int m_BCBGreen;
        private int m_BCBPurple;
        private int m_BCBOrange;
        private int m_BCBRed;
        private int m_BCBYellow;
		//Page 3 Variables
        private int m_BullAqua;
        private int m_BullBlack;
        private int m_BullBlue;
        private int m_BullFRed;
        private int m_BullGreen;
        private int m_BullMagenta;
        private int m_BullOrange;
        private int m_BullPink;
        private int m_BullPlain;
        private int m_BullPurple;
        private int m_BullRed;
        private int m_BullWhite;
        private int m_BullYellow;
        private int m_BullBBlue;
        private int m_BullBGreen;
        private int m_BullBPurple;
        private int m_BullBOrange;
        private int m_BullBRed;
        private int m_BullBYellow;
        //Page 4
        private int m_CampAqua;
        private int m_CampBlack;
        private int m_CampBlue;
        private int m_CampFRed;
        private int m_CampGreen;
        private int m_CampMagenta;
        private int m_CampOrange;
        private int m_CampPink;
        private int m_CampPlain;
        private int m_CampPurple;
        private int m_CampRed;
        private int m_CampWhite;
        private int m_CampYellow;
        private int m_CampBBlue;
        private int m_CampBGreen;
        private int m_CampBPurple;
        private int m_CampBOrange;
        private int m_CampBRed;
        private int m_CampBYellow;
        //Page 5
        private int m_CentAqua;
        private int m_CentBlack;
        private int m_CentBlue;
        private int m_CentFRed;
        private int m_CentGreen;
        private int m_CentMagenta;
        private int m_CentOrange;
        private int m_CentPink;
        private int m_CentPlain;
        private int m_CentPurple;
        private int m_CentRed;
        private int m_CentWhite;
        private int m_CentYellow;
        private int m_CentBBlue;
        private int m_CentBGreen;
        private int m_CentBPurple;
        private int m_CentBOrange;
        private int m_CentBRed;
        private int m_CentBYellow;
        //Page 6
        private int m_EleAqua;
        private int m_EleBlack;
        private int m_EleBlue;
        private int m_EleFRed;
        private int m_EleGreen;
        private int m_EleMagenta;
        private int m_EleOrange;
        private int m_ElePink;
        private int m_ElePlain;
        private int m_ElePurple;
        private int m_EleRed;
        private int m_EleWhite;
        private int m_EleYellow;
        private int m_EleBBlue;
        private int m_EleBGreen;
        private int m_EleBPurple;
        private int m_EleBOrange;
        private int m_EleBRed;
        private int m_EleBYellow;
        //Page 7
        private int m_FernAqua;
        private int m_FernBlack;
        private int m_FernBlue;
        private int m_FernFRed;
        private int m_FernGreen;
        private int m_FernMagenta;
        private int m_FernOrange;
        private int m_FernPink;
        private int m_FernPlain;
        private int m_FernPurple;
        private int m_FernRed;
        private int m_FernWhite;
        private int m_FernYellow;
        private int m_FernBBlue;
        private int m_FernBGreen;
        private int m_FernBPurple;
        private int m_FernBOrange;
        private int m_FernBRed;
        private int m_FernBYellow;
        //Page 8
        private int m_LiliesAqua;
        private int m_LiliesBlack;
        private int m_LiliesBlue;
        private int m_LiliesFRed;
        private int m_LiliesGreen;
        private int m_LiliesMagenta;
        private int m_LiliesOrange;
        private int m_LiliesPink;
        private int m_LiliesPlain;
        private int m_LiliesPurple;
        private int m_LiliesRed;
        private int m_LiliesWhite;
        private int m_LiliesYellow;
        private int m_LiliesBBlue;
        private int m_LiliesBGreen;
        private int m_LiliesBPurple;
        private int m_LiliesBOrange;
        private int m_LiliesBRed;
        private int m_LiliesBYellow;
        //Page 9
        private int m_PampAqua;
        private int m_PampBlack;
        private int m_PampBlue;
        private int m_PampFRed;
        private int m_PampGreen;
        private int m_PampMagenta;
        private int m_PampOrange;
        private int m_PampPink;
        private int m_PampPlain;
        private int m_PampPurple;
        private int m_PampRed;
        private int m_PampWhite;
        private int m_PampYellow;
        private int m_PampBBlue;
        private int m_PampBGreen;
        private int m_PampBPurple;
        private int m_PampBOrange;
        private int m_PampBRed;
        private int m_PampBYellow;
        //Page 10
        private int m_PTPAqua;
        private int m_PTPBlack;
        private int m_PTPBlue;
        private int m_PTPFRed;
        private int m_PTPGreen;
        private int m_PTPMagenta;
        private int m_PTPOrange;
        private int m_PTPPink;
        private int m_PTPPlain;
        private int m_PTPPurple;
        private int m_PTPRed;
        private int m_PTPWhite;
        private int m_PTPYellow;
        private int m_PTPBBlue;
        private int m_PTPBGreen;
        private int m_PTPBPurple;
        private int m_PTPBOrange;
        private int m_PTPBRed;
        private int m_PTPBYellow;
        //Page 11
        private int m_PopAqua;
        private int m_PopBlack;
        private int m_PopBlue;
        private int m_PopFRed;
        private int m_PopGreen;
        private int m_PopMagenta;
        private int m_PopOrange;
        private int m_PopPink;
        private int m_PopPlain;
        private int m_PopPurple;
        private int m_PopRed;
        private int m_PopWhite;
        private int m_PopYellow;
        private int m_PopBBlue;
        private int m_PopBGreen;
        private int m_PopBPurple;
        private int m_PopBOrange;
        private int m_PopBRed;
        private int m_PopBYellow;
        //Page 12
        private int m_PPCAqua;
        private int m_PPCBlack;
        private int m_PPCBlue;
        private int m_PPCFRed;
        private int m_PPCGreen;
        private int m_PPCMagenta;
        private int m_PPCOrange;
        private int m_PPCPink;
        private int m_PPCPlain;
        private int m_PPCPurple;
        private int m_PPCRed;
        private int m_PPCWhite;
        private int m_PPCYellow;
        private int m_PPCBBlue;
        private int m_PPCBGreen;
        private int m_PPCBPurple;
        private int m_PPCBOrange;
        private int m_PPCBRed;
        private int m_PPCBYellow;
        //Page 13
        private int m_RushesAqua;
        private int m_RushesBlack;
        private int m_RushesBlue;
        private int m_RushesFRed;
        private int m_RushesGreen;
        private int m_RushesMagenta;
        private int m_RushesOrange;
        private int m_RushesPink;
        private int m_RushesPlain;
        private int m_RushesPurple;
        private int m_RushesRed;
        private int m_RushesWhite;
        private int m_RushesYellow;
        private int m_RushesBBlue;
        private int m_RushesBGreen;
        private int m_RushesBPurple;
        private int m_RushesBOrange;
        private int m_RushesBRed;
        private int m_RushesBYellow;
        //Page 14
        private int m_SnakeAqua;
        private int m_SnakeBlack;
        private int m_SnakeBlue;
        private int m_SnakeFRed;
        private int m_SnakeGreen;
        private int m_SnakeMagenta;
        private int m_SnakeOrange;
        private int m_SnakePink;
        private int m_SnakePlain;
        private int m_SnakePurple;
        private int m_SnakeRed;
        private int m_SnakeWhite;
        private int m_SnakeYellow;
        private int m_SnakeBBlue;
        private int m_SnakeBGreen;
        private int m_SnakeBPurple;
        private int m_SnakeBOrange;
        private int m_SnakeBRed;
        private int m_SnakeBYellow;
        //Page 15
        private int m_SPalmAqua;
        private int m_SPalmBlack;
        private int m_SPalmBlue;
        private int m_SPalmFRed;
        private int m_SPalmGreen;
        private int m_SPalmMagenta;
        private int m_SPalmOrange;
        private int m_SPalmPink;
        private int m_SPalmPlain;
        private int m_SPalmPurple;
        private int m_SPalmRed;
        private int m_SPalmWhite;
        private int m_SPalmYellow;
        private int m_SPalmBBlue;
        private int m_SPalmBGreen;
        private int m_SPalmBPurple;
        private int m_SPalmBOrange;
        private int m_SPalmBRed;
        private int m_SPalmBYellow;
        //Page 16
        private int m_SdropsAqua;
        private int m_SdropsBlack;
        private int m_SdropsBlue;
        private int m_SdropsFRed;
        private int m_SdropsGreen;
        private int m_SdropsMagenta;
        private int m_SdropsOrange;
        private int m_SdropsPink;
        private int m_SdropsPlain;
        private int m_SdropsPurple;
        private int m_SdropsRed;
        private int m_SdropsWhite;
        private int m_SdropsYellow;
        private int m_SdropsBBlue;
        private int m_SdropsBGreen;
        private int m_SdropsBPurple;
        private int m_SdropsBOrange;
        private int m_SdropsBRed;
        private int m_SdropsBYellow;
        //Page 17
        private int m_TBCAqua;
        private int m_TBCBlack;
        private int m_TBCBlue;
        private int m_TBCFRed;
        private int m_TBCGreen;
        private int m_TBCMagenta;
        private int m_TBCOrange;
        private int m_TBCPink;
        private int m_TBCPlain;
        private int m_TBCPurple;
        private int m_TBCRed;
        private int m_TBCWhite;
        private int m_TBCYellow;
        private int m_TBCBBlue;
        private int m_TBCBGreen;
        private int m_TBCBPurple;
        private int m_TBCBOrange;
        private int m_TBCBRed;
        private int m_TBCBYellow;
        //Page 18
        private int m_WaterAqua;
        private int m_WaterBlack;
        private int m_WaterBlue;
        private int m_WaterFRed;
        private int m_WaterGreen;
        private int m_WaterMagenta;
        private int m_WaterOrange;
        private int m_WaterPink;
        private int m_WaterPlain;
        private int m_WaterPurple;
        private int m_WaterRed;
        private int m_WaterWhite;
        private int m_WaterYellow;
        private int m_WaterBBlue;
        private int m_WaterBGreen;
        private int m_WaterBPurple;
        private int m_WaterBOrange;
        private int m_WaterBRed;
        private int m_WaterBYellow;
#endregion
        #region CommandProperties
        
        //Page 2
		[CommandProperty( AccessLevel.GameMaster )]
		public int BCAqua{ get{ return m_BCAqua; } set{ m_BCAqua = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCBlack { get { return m_BCBlack; } set { m_BCBlack = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int BCBlue{ get{ return m_BCBlue; } set{ m_BCBlue = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCFRed { get { return m_BCFRed; } set { m_BCFRed = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCGreen { get { return m_BCGreen; } set { m_BCGreen = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCMagenta { get { return m_BCMagenta; } set { m_BCMagenta = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCOrange { get { return m_BCOrange; } set { m_BCOrange = value; InvalidateProperties(); } }
        		
		[CommandProperty( AccessLevel.GameMaster )]
        public int BCPink { get { return m_BCPink; } set { m_BCPink = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCPlain { get { return m_BCPlain; } set { m_BCPlain = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
        public int BCPurple { get { return m_BCPurple; } set { m_BCPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCRed { get { return m_BCRed; } set { m_BCRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCWhite { get { return m_BCWhite; } set { m_BCWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCYellow { get { return m_BCYellow; } set { m_BCYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCBBlue { get { return m_BCBBlue; } set { m_BCBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCBGreen { get { return m_BCBGreen; } set { m_BCBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCBPurple { get { return m_BCBPurple; } set { m_BCBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BCBOrange { get { return m_BCBOrange; } set { m_BCBOrange = value; InvalidateProperties(); } }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public int BCBRed { get { return m_BCBRed; } set { m_BCBRed = value; InvalidateProperties(); } }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public int BCBYellow { get { return m_BCBYellow; } set { m_BCBYellow = value; InvalidateProperties(); } }
        
        //Page 3
        [CommandProperty(AccessLevel.GameMaster)]
        public int BullAqua { get { return m_BullAqua; } set { m_BullAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBlack { get { return m_BullBlack; } set { m_BullBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBlue { get { return m_BullBlue; } set { m_BullBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullFRed { get { return m_BullFRed; } set { m_BullFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullGreen { get { return m_BullGreen; } set { m_BullGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullMagenta { get { return m_BullMagenta; } set { m_BullMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullOrange { get { return m_BullOrange; } set { m_BullOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullPink { get { return m_BullPink; } set { m_BullPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullPlain { get { return m_BullPlain; } set { m_BullPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullPurple { get { return m_BullPurple; } set { m_BullPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullRed { get { return m_BullRed; } set { m_BullRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullWhite { get { return m_BullWhite; } set { m_BullWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullYellow { get { return m_BullYellow; } set { m_BullYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBBlue { get { return m_BullBBlue; } set { m_BullBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBGreen { get { return m_BullBGreen; } set { m_BullBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBPurple { get { return m_BullBPurple; } set { m_BullBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBOrange { get { return m_BullBOrange; } set { m_BullBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBRed { get { return m_BullBRed; } set { m_BullBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BullBYellow { get { return m_BullBYellow; } set { m_BullBYellow = value; InvalidateProperties(); } }
        
        //Page 4
        [CommandProperty(AccessLevel.GameMaster)]
        public int CampAqua { get { return m_CampAqua; } set { m_CampAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBlack { get { return m_CampBlack; } set { m_CampBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBlue { get { return m_CampBlue; } set { m_CampBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampFRed { get { return m_CampFRed; } set { m_CampFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampGreen { get { return m_CampGreen; } set { m_CampGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampMagenta { get { return m_CampMagenta; } set { m_CampMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampOrange { get { return m_CampOrange; } set { m_CampOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampPink { get { return m_CampPink; } set { m_CampPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampPlain { get { return m_CampPlain; } set { m_CampPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampPurple { get { return m_CampPurple; } set { m_CampPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampRed { get { return m_CampRed; } set { m_CampRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampWhite { get { return m_CampWhite; } set { m_CampWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampYellow { get { return m_CampYellow; } set { m_CampYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBBlue { get { return m_CampBBlue; } set { m_CampBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBGreen { get { return m_CampBGreen; } set { m_CampBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBPurple { get { return m_CampBPurple; } set { m_CampBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBOrange { get { return m_CampBOrange; } set { m_CampBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBRed { get { return m_CampBRed; } set { m_CampBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CampBYellow { get { return m_CampBYellow; } set { m_CampBYellow = value; InvalidateProperties(); } }
       
        //Page 5
        [CommandProperty(AccessLevel.GameMaster)]
        public int CentAqua { get { return m_CentAqua; } set { m_CentAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBlack { get { return m_CentBlack; } set { m_CentBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBlue { get { return m_CentBlue; } set { m_CentBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentFRed { get { return m_CentFRed; } set { m_CentFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentGreen { get { return m_CentGreen; } set { m_CentGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentMagenta { get { return m_CentMagenta; } set { m_CentMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentOrange { get { return m_CentOrange; } set { m_CentOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentPink { get { return m_CentPink; } set { m_CentPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentPlain { get { return m_CentPlain; } set { m_CentPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentPurple { get { return m_CentPurple; } set { m_CentPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentRed { get { return m_CentRed; } set { m_CentRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentWhite { get { return m_CentWhite; } set { m_CentWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentYellow { get { return m_CentYellow; } set { m_CentYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBBlue { get { return m_CentBBlue; } set { m_CentBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBGreen { get { return m_CentBGreen; } set { m_CentBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBPurple { get { return m_CentBPurple; } set { m_CentBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBOrange { get { return m_CentBOrange; } set { m_CentBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBRed { get { return m_CentBRed; } set { m_CentBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CentBYellow { get { return m_CentBYellow; } set { m_CentBYellow = value; InvalidateProperties(); } }
 
        //Page 6
        [CommandProperty(AccessLevel.GameMaster)]
        public int EleAqua { get { return m_EleAqua; } set { m_EleAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBlack { get { return m_EleBlack; } set { m_EleBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBlue { get { return m_EleBlue; } set { m_EleBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleFRed { get { return m_EleFRed; } set { m_EleFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleGreen { get { return m_EleGreen; } set { m_EleGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleMagenta { get { return m_EleMagenta; } set { m_EleMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleOrange { get { return m_EleOrange; } set { m_EleOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ElePink { get { return m_ElePink; } set { m_ElePink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ElePlain { get { return m_ElePlain; } set { m_ElePlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ElePurple { get { return m_ElePurple; } set { m_ElePurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleRed { get { return m_EleRed; } set { m_EleRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleWhite { get { return m_EleWhite; } set { m_EleWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleYellow { get { return m_EleYellow; } set { m_EleYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBBlue { get { return m_EleBBlue; } set { m_EleBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBGreen { get { return m_EleBGreen; } set { m_EleBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBPurple { get { return m_EleBPurple; } set { m_EleBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBOrange { get { return m_EleBOrange; } set { m_EleBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBRed { get { return m_EleBRed; } set { m_EleBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EleBYellow { get { return m_EleBYellow; } set { m_EleBYellow = value; InvalidateProperties(); } }
       
        //Page 7
        [CommandProperty(AccessLevel.GameMaster)]
        public int FernAqua { get { return m_FernAqua; } set { m_FernAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBlack { get { return m_FernBlack; } set { m_FernBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBlue { get { return m_FernBlue; } set { m_FernBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernFRed { get { return m_FernFRed; } set { m_FernFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernGreen { get { return m_FernGreen; } set { m_FernGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernMagenta { get { return m_FernMagenta; } set { m_FernMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernOrange { get { return m_FernOrange; } set { m_FernOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernPink { get { return m_FernPink; } set { m_FernPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernPlain { get { return m_FernPlain; } set { m_FernPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernPurple { get { return m_FernPurple; } set { m_FernPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernRed { get { return m_FernRed; } set { m_FernRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernWhite { get { return m_FernWhite; } set { m_FernWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernYellow { get { return m_FernYellow; } set { m_FernYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBBlue { get { return m_FernBBlue; } set { m_FernBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBGreen { get { return m_FernBGreen; } set { m_FernBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBPurple { get { return m_FernBPurple; } set { m_FernBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBOrange { get { return m_FernBOrange; } set { m_FernBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBRed { get { return m_FernBRed; } set { m_FernBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FernBYellow { get { return m_FernBYellow; } set { m_FernBYellow = value; InvalidateProperties(); } }
       
        //Page 8
        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesAqua { get { return m_LiliesAqua; } set { m_LiliesAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBlack { get { return m_LiliesBlack; } set { m_LiliesBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBlue { get { return m_LiliesBlue; } set { m_LiliesBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesFRed { get { return m_LiliesFRed; } set { m_LiliesFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesGreen { get { return m_LiliesGreen; } set { m_LiliesGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesMagenta { get { return m_LiliesMagenta; } set { m_LiliesMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesOrange { get { return m_LiliesOrange; } set { m_LiliesOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesPink { get { return m_LiliesPink; } set { m_LiliesPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesPlain { get { return m_LiliesPlain; } set { m_LiliesPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesPurple { get { return m_LiliesPurple; } set { m_LiliesPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesRed { get { return m_LiliesRed; } set { m_LiliesRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesWhite { get { return m_LiliesWhite; } set { m_LiliesWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesYellow { get { return m_LiliesYellow; } set { m_LiliesYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBBlue { get { return m_LiliesBBlue; } set { m_LiliesBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBGreen { get { return m_LiliesBGreen; } set { m_LiliesBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBPurple { get { return m_LiliesBPurple; } set { m_LiliesBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBOrange { get { return m_LiliesBOrange; } set { m_LiliesBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBRed { get { return m_LiliesBRed; } set { m_LiliesBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiliesBYellow { get { return m_LiliesBYellow; } set { m_LiliesBYellow = value; InvalidateProperties(); } }
        
        //Page 9
        [CommandProperty(AccessLevel.GameMaster)]
        public int PampAqua { get { return m_PampAqua; } set { m_PampAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBlack { get { return m_PampBlack; } set { m_PampBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBlue { get { return m_PampBlue; } set { m_PampBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampFRed { get { return m_PampFRed; } set { m_PampFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampGreen { get { return m_PampGreen; } set { m_PampGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampMagenta { get { return m_PampMagenta; } set { m_PampMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampOrange { get { return m_PampOrange; } set { m_PampOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampPink { get { return m_PampPink; } set { m_PampPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampPlain { get { return m_PampPlain; } set { m_PampPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampPurple { get { return m_PampPurple; } set { m_PampPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampRed { get { return m_PampRed; } set { m_PampRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampWhite { get { return m_PampWhite; } set { m_PampWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampYellow { get { return m_PampYellow; } set { m_PampYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBBlue { get { return m_PampBBlue; } set { m_PampBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBGreen { get { return m_PampBGreen; } set { m_PampBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBPurple { get { return m_PampBPurple; } set { m_PampBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBOrange { get { return m_PampBOrange; } set { m_PampBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBRed { get { return m_PampBRed; } set { m_PampBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PampBYellow { get { return m_PampBYellow; } set { m_PampBYellow = value; InvalidateProperties(); } }
        
        //Page 10
        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPAqua { get { return m_PTPAqua; } set { m_PTPAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBlack { get { return m_PTPBlack; } set { m_PTPBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBlue { get { return m_PTPBlue; } set { m_PTPBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPFRed { get { return m_PTPFRed; } set { m_PTPFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPGreen { get { return m_PTPGreen; } set { m_PTPGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPMagenta { get { return m_PTPMagenta; } set { m_PTPMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPOrange { get { return m_PTPOrange; } set { m_PTPOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPPink { get { return m_PTPPink; } set { m_PTPPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPPlain { get { return m_PTPPlain; } set { m_PTPPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPPurple { get { return m_PTPPurple; } set { m_PTPPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPRed { get { return m_PTPRed; } set { m_PTPRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPWhite { get { return m_PTPWhite; } set { m_PTPWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPYellow { get { return m_PTPYellow; } set { m_PTPYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBBlue { get { return m_PTPBBlue; } set { m_PTPBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBGreen { get { return m_PTPBGreen; } set { m_PTPBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBPurple { get { return m_PTPBPurple; } set { m_PTPBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBOrange { get { return m_PTPBOrange; } set { m_PTPBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBRed { get { return m_PTPBRed; } set { m_PTPBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PTPBYellow { get { return m_PTPBYellow; } set { m_PTPBYellow = value; InvalidateProperties(); } }
        
        //Page 11
        [CommandProperty(AccessLevel.GameMaster)]
        public int PopAqua { get { return m_PopAqua; } set { m_PopAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBlack { get { return m_PopBlack; } set { m_PopBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBlue { get { return m_PopBlue; } set { m_PopBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopFRed { get { return m_PopFRed; } set { m_PopFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopGreen { get { return m_PopGreen; } set { m_PopGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopMagenta { get { return m_PopMagenta; } set { m_PopMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopOrange { get { return m_PopOrange; } set { m_PopOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopPink { get { return m_PopPink; } set { m_PopPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopPlain { get { return m_PopPlain; } set { m_PopPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopPurple { get { return m_PopPurple; } set { m_PopPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopRed { get { return m_PopRed; } set { m_PopRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopWhite { get { return m_PopWhite; } set { m_PopWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopYellow { get { return m_PopYellow; } set { m_PopYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBBlue { get { return m_PopBBlue; } set { m_PopBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBGreen { get { return m_PopBGreen; } set { m_PopBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBPurple { get { return m_PopBPurple; } set { m_PopBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBOrange { get { return m_PopBOrange; } set { m_PopBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBRed { get { return m_PopBRed; } set { m_PopBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PopBYellow { get { return m_PopBYellow; } set { m_PopBYellow = value; InvalidateProperties(); } }

        //Page 12
        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCAqua { get { return m_PPCAqua; } set { m_PPCAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBlack { get { return m_PPCBlack; } set { m_PPCBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBlue { get { return m_PPCBlue; } set { m_PPCBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCFRed { get { return m_PPCFRed; } set { m_PPCFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCGreen { get { return m_PPCGreen; } set { m_PPCGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCMagenta { get { return m_PPCMagenta; } set { m_PPCMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCOrange { get { return m_PPCOrange; } set { m_PPCOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCPink { get { return m_PPCPink; } set { m_PPCPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCPlain { get { return m_PPCPlain; } set { m_PPCPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCPurple { get { return m_PPCPurple; } set { m_PPCPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCRed { get { return m_PPCRed; } set { m_PPCRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCWhite { get { return m_PPCWhite; } set { m_PPCWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCYellow { get { return m_PPCYellow; } set { m_PPCYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBBlue { get { return m_PPCBBlue; } set { m_PPCBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBGreen { get { return m_PPCBGreen; } set { m_PPCBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBPurple { get { return m_PPCBPurple; } set { m_PPCBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBOrange { get { return m_PPCBOrange; } set { m_PPCBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBRed { get { return m_PPCBRed; } set { m_PPCBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PPCBYellow { get { return m_PPCBYellow; } set { m_PPCBYellow = value; InvalidateProperties(); } }

        //Page 13
        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesAqua { get { return m_RushesAqua; } set { m_RushesAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBlack { get { return m_RushesBlack; } set { m_RushesBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBlue { get { return m_RushesBlue; } set { m_RushesBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesFRed { get { return m_RushesFRed; } set { m_RushesFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesGreen { get { return m_RushesGreen; } set { m_RushesGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesMagenta { get { return m_RushesMagenta; } set { m_RushesMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesOrange { get { return m_RushesOrange; } set { m_RushesOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesPink { get { return m_RushesPink; } set { m_RushesPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesPlain { get { return m_RushesPlain; } set { m_RushesPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesPurple { get { return m_RushesPurple; } set { m_RushesPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesRed { get { return m_RushesRed; } set { m_RushesRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesWhite { get { return m_RushesWhite; } set { m_RushesWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesYellow { get { return m_RushesYellow; } set { m_RushesYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBBlue { get { return m_RushesBBlue; } set { m_RushesBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBGreen { get { return m_RushesBGreen; } set { m_RushesBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBPurple { get { return m_RushesBPurple; } set { m_RushesBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBOrange { get { return m_RushesBOrange; } set { m_RushesBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBRed { get { return m_RushesBRed; } set { m_RushesBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RushesBYellow { get { return m_RushesBYellow; } set { m_RushesBYellow = value; InvalidateProperties(); } }

        //Page 14
        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeAqua { get { return m_SnakeAqua; } set { m_SnakeAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBlack { get { return m_SnakeBlack; } set { m_SnakeBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBlue { get { return m_SnakeBlue; } set { m_SnakeBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeFRed { get { return m_SnakeFRed; } set { m_SnakeFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeGreen { get { return m_SnakeGreen; } set { m_SnakeGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeMagenta { get { return m_SnakeMagenta; } set { m_SnakeMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeOrange { get { return m_SnakeOrange; } set { m_SnakeOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakePink { get { return m_SnakePink; } set { m_SnakePink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakePlain { get { return m_SnakePlain; } set { m_SnakePlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakePurple { get { return m_SnakePurple; } set { m_SnakePurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeRed { get { return m_SnakeRed; } set { m_SnakeRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeWhite { get { return m_SnakeWhite; } set { m_SnakeWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeYellow { get { return m_SnakeYellow; } set { m_SnakeYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBBlue { get { return m_SnakeBBlue; } set { m_SnakeBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBGreen { get { return m_SnakeBGreen; } set { m_SnakeBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBPurple { get { return m_SnakeBPurple; } set { m_SnakeBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBOrange { get { return m_SnakeBOrange; } set { m_SnakeBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBRed { get { return m_SnakeBRed; } set { m_SnakeBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SnakeBYellow { get { return m_SnakeBYellow; } set { m_SnakeBYellow = value; InvalidateProperties(); } }

        //Page 15
        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmAqua { get { return m_SPalmAqua; } set { m_SPalmAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBlack { get { return m_SPalmBlack; } set { m_SPalmBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBlue { get { return m_SPalmBlue; } set { m_SPalmBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmFRed { get { return m_SPalmFRed; } set { m_SPalmFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmGreen { get { return m_SPalmGreen; } set { m_SPalmGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmMagenta { get { return m_SPalmMagenta; } set { m_SPalmMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmOrange { get { return m_SPalmOrange; } set { m_SPalmOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmPink { get { return m_SPalmPink; } set { m_SPalmPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmPlain { get { return m_SPalmPlain; } set { m_SPalmPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmPurple { get { return m_SPalmPurple; } set { m_SPalmPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmRed { get { return m_SPalmRed; } set { m_SPalmRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmWhite { get { return m_SPalmWhite; } set { m_SPalmWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmYellow { get { return m_SPalmYellow; } set { m_SPalmYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBBlue { get { return m_SPalmBBlue; } set { m_SPalmBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBGreen { get { return m_SPalmBGreen; } set { m_SPalmBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBPurple { get { return m_SPalmBPurple; } set { m_SPalmBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBOrange { get { return m_SPalmBOrange; } set { m_SPalmBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBRed { get { return m_SPalmBRed; } set { m_SPalmBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SPalmBYellow { get { return m_SPalmBYellow; } set { m_SPalmBYellow = value; InvalidateProperties(); } }

        //Page 16
        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsAqua { get { return m_SdropsAqua; } set { m_SdropsAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBlack { get { return m_SdropsBlack; } set { m_SdropsBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBlue { get { return m_SdropsBlue; } set { m_SdropsBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsFRed { get { return m_SdropsFRed; } set { m_SdropsFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsGreen { get { return m_SdropsGreen; } set { m_SdropsGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsMagenta { get { return m_SdropsMagenta; } set { m_SdropsMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsOrange { get { return m_SdropsOrange; } set { m_SdropsOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsPink { get { return m_SdropsPink; } set { m_SdropsPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsPlain { get { return m_SdropsPlain; } set { m_SdropsPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsPurple { get { return m_SdropsPurple; } set { m_SdropsPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsRed { get { return m_SdropsRed; } set { m_SdropsRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsWhite { get { return m_SdropsWhite; } set { m_SdropsWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsYellow { get { return m_SdropsYellow; } set { m_SdropsYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBBlue { get { return m_SdropsBBlue; } set { m_SdropsBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBGreen { get { return m_SdropsBGreen; } set { m_SdropsBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBPurple { get { return m_SdropsBPurple; } set { m_SdropsBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBOrange { get { return m_SdropsBOrange; } set { m_SdropsBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBRed { get { return m_SdropsBRed; } set { m_SdropsBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SdropsBYellow { get { return m_SdropsBYellow; } set { m_SdropsBYellow = value; InvalidateProperties(); } }

        //Page 17
        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCAqua { get { return m_TBCAqua; } set { m_TBCAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBlack { get { return m_TBCBlack; } set { m_TBCBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBlue { get { return m_TBCBlue; } set { m_TBCBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCFRed { get { return m_TBCFRed; } set { m_TBCFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCGreen { get { return m_TBCGreen; } set { m_TBCGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCMagenta { get { return m_TBCMagenta; } set { m_TBCMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCOrange { get { return m_TBCOrange; } set { m_TBCOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCPink { get { return m_TBCPink; } set { m_TBCPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCPlain { get { return m_TBCPlain; } set { m_TBCPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCPurple { get { return m_TBCPurple; } set { m_TBCPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCRed { get { return m_TBCRed; } set { m_TBCRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCWhite { get { return m_TBCWhite; } set { m_TBCWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCYellow { get { return m_TBCYellow; } set { m_TBCYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBBlue { get { return m_TBCBBlue; } set { m_TBCBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBGreen { get { return m_TBCBGreen; } set { m_TBCBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBPurple { get { return m_TBCBPurple; } set { m_TBCBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBOrange { get { return m_TBCBOrange; } set { m_TBCBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBRed { get { return m_TBCBRed; } set { m_TBCBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TBCBYellow { get { return m_TBCBYellow; } set { m_TBCBYellow = value; InvalidateProperties(); } }

        //Page 18
        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterAqua { get { return m_WaterAqua; } set { m_WaterAqua = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBlack { get { return m_WaterBlack; } set { m_WaterBlack = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBlue { get { return m_WaterBlue; } set { m_WaterBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterFRed { get { return m_WaterFRed; } set { m_WaterFRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterGreen { get { return m_WaterGreen; } set { m_WaterGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterMagenta { get { return m_WaterMagenta; } set { m_WaterMagenta = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterOrange { get { return m_WaterOrange; } set { m_WaterOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterPink { get { return m_WaterPink; } set { m_WaterPink = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterPlain { get { return m_WaterPlain; } set { m_WaterPlain = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterPurple { get { return m_WaterPurple; } set { m_WaterPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterRed { get { return m_WaterRed; } set { m_WaterRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterWhite { get { return m_WaterWhite; } set { m_WaterWhite = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterYellow { get { return m_WaterYellow; } set { m_WaterYellow = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBBlue { get { return m_WaterBBlue; } set { m_WaterBBlue = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBGreen { get { return m_WaterBGreen; } set { m_WaterBGreen = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBPurple { get { return m_WaterBPurple; } set { m_WaterBPurple = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBOrange { get { return m_WaterBOrange; } set { m_WaterBOrange = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBRed { get { return m_WaterBRed; } set { m_WaterBRed = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WaterBYellow { get { return m_WaterBYellow; } set { m_WaterBYellow = value; InvalidateProperties(); } }
        #endregion
       

		[Constructable]
		public SeedCompanion() : base( 3706 )
		{
			Movable = true;
			Weight = 10.0;
            Hue = 66;
            Name = "Florist's Seed Companion";
		}

		public override void OnDoubleClick( Mobile from )
		{
            int page = 1;
            if (!from.InRange(GetWorldLocation(), 2))
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (from is PlayerMobile)
                from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
		}

		public void BeginCombine( Mobile from )
		{
			from.Target = new SeedCompanionTarget( this );
        }
        #region EndCombine
        public void EndCombine(Mobile from, object obj)
        {
            Seed curSeed = obj as Seed;

            if (!(obj is Seed))
            {
                from.SendMessage("Only flower and plant seeds can be stored here.");
                return;
            }

            if (curSeed.ShowType == false)
            {
                from.SendMessage("Only identified seeds can be stored here. Plant it first.");
                return;
            }

            //Start Page 2 Items
            if (curSeed.PlantType == PlantType.BarrelCactus)
            {
                int page = 2;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (BCAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCAqua += curItem.Amount;
                        curItem.Delete();

                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (BCBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (BCBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (BCFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (BCGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (BCMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (BCOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (BCPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (BCPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (BCPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (BCRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (BCWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (BCYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (BCBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (BCBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (BCBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (BCBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (BCBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (BCBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BCBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 3 Items
            if (curSeed.PlantType == PlantType.Bulrushes)
            {
                int page = 3;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (BullAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (BullBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (BullBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (BullFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (BullGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (BullMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (BullOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (BullPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (BullPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (BullPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (BullRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (BullWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (BullYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (BullBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (BullBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (BullBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (BullBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (BullBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (BullBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        BullBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 4 Items
            if (curSeed.PlantType == PlantType.CampionFlowers)
            {
                int page = 4;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (CampAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (CampBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (CampBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (CampFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (CampGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (CampMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (CampOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (CampPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (CampPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (CampPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (CampRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (CampWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (CampYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (CampBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (CampBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (CampBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (CampBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (CampBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (CampBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CampBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 5 Items
            if (curSeed.PlantType == PlantType.CenturyPlant)
            {
                int page = 5;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (CentAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (CentBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (CentBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (CentFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (CentGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (CentMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (CentOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (CentPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (CentPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (CentPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (CentRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (CentWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (CentYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (CentBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (CentBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (CentBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (CentBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (CentBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (CentBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        CentBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 6 Items
            if (curSeed.PlantType == PlantType.ElephantEarPlant)
            {
                int page = 6;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (EleAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (EleBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (EleBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (EleFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (EleGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (EleMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (EleOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (ElePink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        ElePink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (ElePlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        ElePlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (ElePurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        ElePurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (EleRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (EleWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (EleYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (EleBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (EleBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (EleBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (EleBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (EleBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (EleBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        EleBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 7 Items
            if (curSeed.PlantType == PlantType.Fern)
            {
                int page = 7;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (FernAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (FernBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (FernBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (FernFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (FernGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (FernMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (FernOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (FernPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (FernPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (FernPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (FernRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (FernWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (FernYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (FernBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (FernBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (FernBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (FernBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (FernBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (FernBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        FernBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 8 Items
            if (curSeed.PlantType == PlantType.Lilies)
            {
                int page = 8;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (LiliesAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (LiliesBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (LiliesBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (LiliesFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (LiliesGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (LiliesMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (LiliesOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (LiliesPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (LiliesPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (LiliesPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (LiliesRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (LiliesWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (LiliesYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (LiliesBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (LiliesBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (LiliesBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (LiliesBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (LiliesBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (LiliesBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        LiliesBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 9 Items
            if (curSeed.PlantType == PlantType.PampasGrass)
            {
                int page = 9;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (PampAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (PampBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (PampBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (PampFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (PampGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (PampMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (PampOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (PampPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (PampPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (PampPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (PampRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (PampWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (PampYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (PampBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (PampBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (PampBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (PampBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (PampBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (PampBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PampBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 10 Items
            if (curSeed.PlantType == PlantType.PonytailPalm)
            {
                int page = 10;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (PTPAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (PTPBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (PTPBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (PTPFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (PTPGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (PTPMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (PTPOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (PTPPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (PTPPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (PTPPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (PTPRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (PTPWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (PTPYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (PTPBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (PTPBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (PTPBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (PTPBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (PTPBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (PTPBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PTPBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 11 Items
            if (curSeed.PlantType == PlantType.Poppies)
            {
                int page = 11;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (PopAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (PopBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (PopBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (PopFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (PopGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (PopMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (PopOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (PopPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (PopPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (PopPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (PopRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (PopWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (PopYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (PopBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (PopBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (PopBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (PopBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (PopBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (PopBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PopBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 12 Items
            if (curSeed.PlantType == PlantType.PricklyPearCactus)
            {
                int page = 12;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (PPCAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (PPCBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (PPCBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (PPCFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (PPCGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (PPCMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (PPCOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (PPCPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (PPCPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (PPCPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (PPCRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (PPCWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (PPCYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (PPCBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (PPCBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (PPCBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (PPCBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (PPCBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (PPCBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        PPCBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 13 Items
            if (curSeed.PlantType == PlantType.Rushes)
            {
                int page = 13;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (RushesAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (RushesBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (RushesBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (RushesFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (RushesGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (RushesMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (RushesOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (RushesPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (RushesPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (RushesPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (RushesRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (RushesWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (RushesYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (RushesBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (RushesBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (RushesBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (RushesBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (RushesBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (RushesBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        RushesBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 14 Items
            if (curSeed.PlantType == PlantType.SnakePlant)
            {
                int page = 14;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (SnakeAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (SnakeBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (SnakeBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (SnakeFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (SnakeGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (SnakeMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (SnakeOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (SnakePink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakePink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (SnakePlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakePlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (SnakePurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakePurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (SnakeRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (SnakeWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (SnakeYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (SnakeBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (SnakeBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (SnakeBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (SnakeBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (SnakeBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (SnakeBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SnakeBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 15 Items
            if (curSeed.PlantType == PlantType.SmallPalm)
            {
                int page = 15;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (SPalmAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (SPalmBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (SPalmBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (SPalmFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (SPalmGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (SPalmMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (SPalmOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (SPalmPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (SPalmPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (SPalmPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (SPalmRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (SPalmWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (SPalmYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (SPalmBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (SPalmBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (SPalmBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (SPalmBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (SPalmBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (SPalmBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SPalmBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 16 Items
            if (curSeed.PlantType == PlantType.Snowdrops)
            {
                int page = 16;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (SdropsAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (SdropsBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (SdropsBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (SdropsFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (SdropsGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (SdropsMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (SdropsOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (SdropsPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (SdropsPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (SdropsPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (SdropsRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (SdropsWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (SdropsYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (SdropsBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (SdropsBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (SdropsBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (SdropsBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (SdropsBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (SdropsBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        SdropsBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 17 Items
            if (curSeed.PlantType == PlantType.TribarrelCactus)
            {
                int page = 17;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (TBCAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (TBCBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (TBCBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (TBCFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (TBCGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (TBCMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (TBCOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (TBCPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (TBCPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (TBCPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (TBCRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (TBCWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (TBCYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (TBCBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (TBCBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (TBCBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (TBCBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (TBCBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (TBCBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        TBCBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

            //Start Page 18 Items
            if (curSeed.PlantType == PlantType.WaterPlant)
            {
                int page = 18;
                if (curSeed.PlantHue == PlantHue.Aqua)
                {
                    if (WaterAqua >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterAqua += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Black)
                {
                    if (WaterBlack >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBlack += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Blue)
                {
                    if (WaterBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.FireRed)
                {
                    if (WaterFRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterFRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Green)
                {
                    if (WaterGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Magenta)
                {
                    if (WaterMagenta >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterMagenta += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Orange)
                {
                    if (WaterOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Pink)
                {
                    if (WaterPink >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterPink += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Plain)
                {
                    if (WaterPlain >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterPlain += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Purple)
                {
                    if (WaterPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Red)
                {
                    if (WaterRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.White)
                {
                    if (WaterWhite >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterWhite += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.Yellow)
                {
                    if (WaterYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightBlue)
                {
                    if (WaterBBlue >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBBlue += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightGreen)
                {
                    if (WaterBGreen >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBGreen += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightPurple)
                {
                    if (WaterBPurple >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBPurple += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightOrange)
                {
                    if (WaterBOrange >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBOrange += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightRed)
                {
                    if (WaterBRed >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBRed += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
                if (curSeed.PlantHue == PlantHue.BrightYellow)
                {
                    if (WaterBYellow >= 25)
                        from.SendMessage("You can not add any more of that type.");
                    else
                    {
                        Item curItem = obj as Item;
                        WaterBYellow += curItem.Amount;
                        curItem.Delete();
                        from.SendGump(new SeedCompanionGump((PlayerMobile)from, this, page));
                        BeginCombine(from);
                    }
                }
            }

        }
        #endregion
        public SeedCompanion( Serial serial ) : base( serial )
		{
		}
        #region Serialize

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
            writer.Write( (int) m_BCAqua);
		    writer.Write( (int) m_BCBlack);
		    writer.Write( (int) m_BCBlue);
		    writer.Write( (int) m_BCFRed);
		    writer.Write( (int) m_BCGreen);
		    writer.Write( (int) m_BCMagenta);
		    writer.Write( (int) m_BCOrange);
            writer.Write( (int) m_BCPink);
		    writer.Write( (int) m_BCPlain);
		    writer.Write( (int) m_BCPurple);
            writer.Write( (int) m_BCRed);
            writer.Write( (int) m_BCWhite);
            writer.Write( (int) m_BCYellow);
            writer.Write( (int) m_BCBBlue);
            writer.Write( (int) m_BCBGreen);
            writer.Write( (int) m_BCBPurple);
            writer.Write( (int) m_BCBOrange);
            writer.Write( (int) m_BCBRed);
            writer.Write( (int) m_BCBYellow);
		//Page 3 Variables
            writer.Write( (int) m_BullAqua);
            writer.Write( (int) m_BullBlack);
            writer.Write( (int) m_BullBlue);
            writer.Write( (int) m_BullFRed);
            writer.Write( (int) m_BullGreen);
            writer.Write( (int) m_BullMagenta);
            writer.Write( (int) m_BullOrange);
            writer.Write( (int) m_BullPink);
            writer.Write( (int) m_BullPlain);
            writer.Write( (int) m_BullPurple);
            writer.Write( (int) m_BullRed);
            writer.Write( (int) m_BullWhite);
            writer.Write( (int) m_BullYellow);
            writer.Write( (int) m_BullBBlue);
            writer.Write( (int) m_BullBGreen);
            writer.Write( (int) m_BullBPurple);
            writer.Write( (int) m_BullBOrange);
            writer.Write( (int) m_BullBRed);
            writer.Write( (int) m_BullBYellow);
        //Page 4
            writer.Write( (int) m_CampAqua);
            writer.Write( (int) m_CampBlack);
            writer.Write( (int) m_CampBlue);
            writer.Write( (int) m_CampFRed);
            writer.Write( (int) m_CampGreen);
            writer.Write( (int) m_CampMagenta);
            writer.Write( (int) m_CampOrange);
            writer.Write( (int) m_CampPink);
            writer.Write( (int) m_CampPlain);
            writer.Write( (int) m_CampPurple);
            writer.Write( (int) m_CampRed);
            writer.Write( (int) m_CampWhite);
            writer.Write( (int) m_CampYellow);
            writer.Write( (int) m_CampBBlue);
            writer.Write( (int) m_CampBGreen);
            writer.Write( (int) m_CampBPurple);
            writer.Write( (int) m_CampBOrange);
            writer.Write( (int) m_CampBRed);
            writer.Write( (int) m_CampBYellow);
        //Page 5
            writer.Write( (int) m_CentAqua);
            writer.Write( (int) m_CentBlack);
            writer.Write( (int) m_CentBlue);
            writer.Write( (int) m_CentFRed);
            writer.Write( (int) m_CentGreen);
            writer.Write( (int) m_CentMagenta);
            writer.Write( (int) m_CentOrange);
            writer.Write( (int) m_CentPink);
            writer.Write( (int) m_CentPlain);
            writer.Write( (int) m_CentPurple);
            writer.Write( (int) m_CentRed);
            writer.Write( (int) m_CentWhite);
            writer.Write( (int) m_CentYellow);
            writer.Write( (int) m_CentBBlue);
            writer.Write( (int) m_CentBGreen);
            writer.Write( (int) m_CentBPurple);
            writer.Write( (int) m_CentBOrange);
            writer.Write( (int) m_CentBRed);
            writer.Write( (int) m_CentBYellow);
        //Page 6
            writer.Write( (int) m_EleAqua);
            writer.Write( (int) m_EleBlack);
            writer.Write( (int) m_EleBlue);
            writer.Write( (int) m_EleFRed);
            writer.Write( (int) m_EleGreen);
            writer.Write( (int) m_EleMagenta);
            writer.Write( (int) m_EleOrange);
            writer.Write( (int) m_ElePink);
            writer.Write( (int) m_ElePlain);
            writer.Write( (int) m_ElePurple);
            writer.Write( (int) m_EleRed);
            writer.Write( (int) m_EleWhite);
            writer.Write( (int) m_EleYellow);
            writer.Write( (int) m_EleBBlue);
            writer.Write( (int) m_EleBGreen);
            writer.Write( (int) m_EleBPurple);
            writer.Write( (int) m_EleBOrange);
            writer.Write( (int) m_EleBRed);
            writer.Write( (int) m_EleBYellow);
        //Page 7
            writer.Write( (int) m_FernAqua);
            writer.Write( (int) m_FernBlack);
            writer.Write( (int) m_FernBlue);
            writer.Write( (int) m_FernFRed);
            writer.Write( (int) m_FernGreen);
            writer.Write( (int) m_FernMagenta);
            writer.Write( (int) m_FernOrange);
            writer.Write( (int) m_FernPink);
            writer.Write( (int) m_FernPlain);
            writer.Write( (int) m_FernPurple);
            writer.Write( (int) m_FernRed);
            writer.Write( (int) m_FernWhite);
            writer.Write( (int) m_FernYellow);
            writer.Write( (int) m_FernBBlue);
            writer.Write( (int) m_FernBGreen);
            writer.Write( (int) m_FernBPurple);
            writer.Write( (int) m_FernBOrange);
            writer.Write( (int) m_FernBRed);
            writer.Write( (int) m_FernBYellow);
        //Page 8
            writer.Write( (int) m_LiliesAqua);
            writer.Write( (int) m_LiliesBlack);
            writer.Write( (int) m_LiliesBlue);
            writer.Write( (int) m_LiliesFRed);
            writer.Write( (int) m_LiliesGreen);
            writer.Write( (int) m_LiliesMagenta);
            writer.Write( (int) m_LiliesOrange);
            writer.Write( (int) m_LiliesPink);
            writer.Write( (int) m_LiliesPlain);
            writer.Write( (int) m_LiliesPurple);
            writer.Write( (int) m_LiliesRed);
            writer.Write( (int) m_LiliesWhite);
            writer.Write( (int) m_LiliesYellow);
            writer.Write( (int) m_LiliesBBlue);
            writer.Write( (int) m_LiliesBGreen);
            writer.Write( (int) m_LiliesBPurple);
            writer.Write( (int) m_LiliesBOrange);
            writer.Write( (int) m_LiliesBRed);
            writer.Write( (int) m_LiliesBYellow);
        //Page 9
            writer.Write( (int) m_PampAqua);
            writer.Write( (int) m_PampBlack);
            writer.Write( (int) m_PampBlue);
            writer.Write( (int) m_PampFRed);
            writer.Write( (int) m_PampGreen);
            writer.Write( (int) m_PampMagenta);
            writer.Write( (int) m_PampOrange);
            writer.Write( (int) m_PampPink);
            writer.Write( (int) m_PampPlain);
            writer.Write( (int) m_PampPurple);
            writer.Write( (int) m_PampRed);
            writer.Write( (int) m_PampWhite);
            writer.Write( (int) m_PampYellow);
            writer.Write( (int) m_PampBBlue);
            writer.Write( (int) m_PampBGreen);
            writer.Write( (int) m_PampBPurple);
            writer.Write( (int) m_PampBOrange);
            writer.Write( (int) m_PampBRed);
            writer.Write( (int) m_PampBYellow);
        //Page 10
            writer.Write( (int) m_PTPAqua);
            writer.Write( (int) m_PTPBlack);
            writer.Write( (int) m_PTPBlue);
            writer.Write( (int) m_PTPFRed);
            writer.Write( (int) m_PTPGreen);
            writer.Write( (int) m_PTPMagenta);
            writer.Write( (int) m_PTPOrange);
            writer.Write( (int) m_PTPPink);
            writer.Write( (int) m_PTPPlain);
            writer.Write( (int) m_PTPPurple);
            writer.Write( (int) m_PTPRed);
            writer.Write( (int) m_PTPWhite);
            writer.Write( (int) m_PTPYellow);
            writer.Write( (int) m_PTPBBlue);
            writer.Write( (int) m_PTPBGreen);
            writer.Write( (int) m_PTPBPurple);
            writer.Write( (int) m_PTPBOrange);
            writer.Write( (int) m_PTPBRed);
            writer.Write( (int) m_PTPBYellow);
        //Page 11
            writer.Write( (int) m_PopAqua);
            writer.Write( (int) m_PopBlack);
            writer.Write( (int) m_PopBlue);
            writer.Write( (int) m_PopFRed);
            writer.Write( (int) m_PopGreen);
            writer.Write( (int) m_PopMagenta);
            writer.Write( (int) m_PopOrange);
            writer.Write( (int) m_PopPink);
            writer.Write( (int) m_PopPlain);
            writer.Write( (int) m_PopPurple);
            writer.Write( (int) m_PopRed);
            writer.Write( (int) m_PopWhite);
            writer.Write( (int) m_PopYellow);
            writer.Write( (int) m_PopBBlue);
            writer.Write( (int) m_PopBGreen);
            writer.Write( (int) m_PopBPurple);
            writer.Write( (int) m_PopBOrange);
            writer.Write( (int) m_PopBRed);
            writer.Write( (int) m_PopBYellow);
        //Page 12
            writer.Write( (int) m_PPCAqua);
            writer.Write( (int) m_PPCBlack);
            writer.Write( (int) m_PPCBlue);
            writer.Write( (int) m_PPCFRed);
            writer.Write( (int) m_PPCGreen);
            writer.Write( (int) m_PPCMagenta);
            writer.Write( (int) m_PPCOrange);
            writer.Write( (int) m_PPCPink);
            writer.Write( (int) m_PPCPlain);
            writer.Write( (int) m_PPCPurple);
            writer.Write( (int) m_PPCRed);
            writer.Write( (int) m_PPCWhite);
            writer.Write( (int) m_PPCYellow);
            writer.Write( (int) m_PPCBBlue);
            writer.Write( (int) m_PPCBGreen);
            writer.Write( (int) m_PPCBPurple);
            writer.Write( (int) m_PPCBOrange);
            writer.Write( (int) m_PPCBRed);
            writer.Write( (int) m_PPCBYellow);
        //Page 13
            writer.Write( (int) m_RushesAqua);
            writer.Write( (int) m_RushesBlack);
            writer.Write( (int) m_RushesBlue);
            writer.Write( (int) m_RushesFRed);
            writer.Write( (int) m_RushesGreen);
            writer.Write( (int) m_RushesMagenta);
            writer.Write( (int) m_RushesOrange);
            writer.Write( (int) m_RushesPink);
            writer.Write( (int) m_RushesPlain);
            writer.Write( (int) m_RushesPurple);
            writer.Write( (int) m_RushesRed);
            writer.Write( (int) m_RushesWhite);
            writer.Write( (int) m_RushesYellow);
            writer.Write( (int) m_RushesBBlue);
            writer.Write( (int) m_RushesBGreen);
            writer.Write( (int) m_RushesBPurple);
            writer.Write( (int) m_RushesBOrange);
            writer.Write( (int) m_RushesBRed);
            writer.Write( (int) m_RushesBYellow);
        //Page 14
            writer.Write( (int) m_SnakeAqua);
            writer.Write( (int) m_SnakeBlack);
            writer.Write( (int) m_SnakeBlue);
            writer.Write( (int) m_SnakeFRed);
            writer.Write( (int) m_SnakeGreen);
            writer.Write( (int) m_SnakeMagenta);
            writer.Write( (int) m_SnakeOrange);
            writer.Write( (int) m_SnakePink);
            writer.Write( (int) m_SnakePlain);
            writer.Write( (int) m_SnakePurple);
            writer.Write( (int) m_SnakeRed);
            writer.Write( (int) m_SnakeWhite);
            writer.Write( (int) m_SnakeYellow);
            writer.Write( (int) m_SnakeBBlue);
            writer.Write( (int) m_SnakeBGreen);
            writer.Write( (int) m_SnakeBPurple);
            writer.Write( (int) m_SnakeBOrange);
            writer.Write( (int) m_SnakeBRed);
            writer.Write( (int) m_SnakeBYellow);
        //Page 15
            writer.Write( (int) m_SPalmAqua);
            writer.Write( (int) m_SPalmBlack);
            writer.Write( (int) m_SPalmBlue);
            writer.Write( (int) m_SPalmFRed);
            writer.Write( (int) m_SPalmGreen);
            writer.Write( (int) m_SPalmMagenta);
            writer.Write( (int) m_SPalmOrange);
            writer.Write( (int) m_SPalmPink);
            writer.Write( (int) m_SPalmPlain);
            writer.Write( (int) m_SPalmPurple);
            writer.Write( (int) m_SPalmRed);
            writer.Write( (int) m_SPalmWhite);
            writer.Write( (int) m_SPalmYellow);
            writer.Write( (int) m_SPalmBBlue);
            writer.Write( (int) m_SPalmBGreen);
            writer.Write( (int) m_SPalmBPurple);
            writer.Write( (int) m_SPalmBOrange);
            writer.Write( (int) m_SPalmBRed);
            writer.Write( (int) m_SPalmBYellow);
        //Page 16
            writer.Write( (int) m_SdropsAqua);
            writer.Write( (int) m_SdropsBlack);
            writer.Write( (int) m_SdropsBlue);
            writer.Write( (int) m_SdropsFRed);
            writer.Write( (int) m_SdropsGreen);
            writer.Write( (int) m_SdropsMagenta);
            writer.Write( (int) m_SdropsOrange);
            writer.Write( (int) m_SdropsPink);
            writer.Write( (int) m_SdropsPlain);
            writer.Write( (int) m_SdropsPurple);
            writer.Write( (int) m_SdropsRed);
            writer.Write( (int) m_SdropsWhite);
            writer.Write( (int) m_SdropsYellow);
            writer.Write( (int) m_SdropsBBlue);
            writer.Write( (int) m_SdropsBGreen);
            writer.Write( (int) m_SdropsBPurple);
            writer.Write( (int) m_SdropsBOrange);
            writer.Write( (int) m_SdropsBRed);
            writer.Write( (int) m_SdropsBYellow);
        //Page 17
            writer.Write( (int) m_TBCAqua);
            writer.Write( (int) m_TBCBlack);
            writer.Write( (int) m_TBCBlue);
            writer.Write( (int) m_TBCFRed);
            writer.Write( (int) m_TBCGreen);
            writer.Write( (int) m_TBCMagenta);
            writer.Write( (int) m_TBCOrange);
            writer.Write( (int) m_TBCPink);
            writer.Write( (int) m_TBCPlain);
            writer.Write( (int) m_TBCPurple);
            writer.Write( (int) m_TBCRed);
            writer.Write( (int) m_TBCWhite);
            writer.Write( (int) m_TBCYellow);
            writer.Write( (int) m_TBCBBlue);
            writer.Write( (int) m_TBCBGreen);
            writer.Write( (int) m_TBCBPurple);
            writer.Write( (int) m_TBCBOrange);
            writer.Write( (int) m_TBCBRed);
            writer.Write( (int) m_TBCBYellow);
        //Page 18
            writer.Write( (int) m_WaterAqua);
            writer.Write( (int) m_WaterBlack);
            writer.Write( (int) m_WaterBlue);
            writer.Write( (int) m_WaterFRed);
            writer.Write( (int) m_WaterGreen);
            writer.Write( (int) m_WaterMagenta);
            writer.Write( (int) m_WaterOrange);
            writer.Write( (int) m_WaterPink);
            writer.Write( (int) m_WaterPlain);
            writer.Write( (int) m_WaterPurple);
            writer.Write( (int) m_WaterRed);
            writer.Write( (int) m_WaterWhite);
            writer.Write( (int) m_WaterYellow);
            writer.Write( (int) m_WaterBBlue);
            writer.Write( (int) m_WaterBGreen);
            writer.Write( (int) m_WaterBPurple);
            writer.Write( (int) m_WaterBOrange);
            writer.Write( (int) m_WaterBRed);
            writer.Write( (int) m_WaterBYellow);
		}
#endregion
        #region Deserialize
        public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

            m_BCAqua = reader.ReadInt();
            m_BCBlack = reader.ReadInt();
            m_BCBlue = reader.ReadInt();
            m_BCFRed = reader.ReadInt();
            m_BCGreen = reader.ReadInt();
            m_BCMagenta = reader.ReadInt();
            m_BCOrange = reader.ReadInt();
            m_BCPink = reader.ReadInt();
            m_BCPlain = reader.ReadInt();
            m_BCPurple = reader.ReadInt();
            m_BCRed = reader.ReadInt();
            m_BCWhite = reader.ReadInt();
            m_BCYellow = reader.ReadInt();
            m_BCBBlue = reader.ReadInt();
            m_BCBGreen = reader.ReadInt();
            m_BCBPurple = reader.ReadInt();
            m_BCBOrange = reader.ReadInt();
            m_BCBRed = reader.ReadInt();
            m_BCBYellow = reader.ReadInt();
            //Page 3
            m_BullAqua = reader.ReadInt();
            m_BullBlack = reader.ReadInt();
            m_BullBlue = reader.ReadInt();
            m_BullFRed = reader.ReadInt();
            m_BullGreen = reader.ReadInt();
            m_BullMagenta = reader.ReadInt();
            m_BullOrange = reader.ReadInt();
            m_BullPink = reader.ReadInt();
            m_BullPlain = reader.ReadInt();
            m_BullPurple = reader.ReadInt();
            m_BullRed = reader.ReadInt();
            m_BullWhite = reader.ReadInt();
            m_BullYellow = reader.ReadInt();
            m_BullBBlue = reader.ReadInt();
            m_BullBGreen = reader.ReadInt();
            m_BullBPurple = reader.ReadInt();
            m_BullBOrange = reader.ReadInt();
            m_BullBRed = reader.ReadInt();
            m_BullBYellow = reader.ReadInt();
            //Page 4
            m_CampAqua = reader.ReadInt();
            m_CampBlack = reader.ReadInt();
            m_CampBlue = reader.ReadInt();
            m_CampFRed = reader.ReadInt();
            m_CampGreen = reader.ReadInt();
            m_CampMagenta = reader.ReadInt();
            m_CampOrange = reader.ReadInt();
            m_CampPink = reader.ReadInt();
            m_CampPlain = reader.ReadInt();
            m_CampPurple = reader.ReadInt();
            m_CampRed = reader.ReadInt();
            m_CampWhite = reader.ReadInt();
            m_CampYellow = reader.ReadInt();
            m_CampBBlue = reader.ReadInt();
            m_CampBGreen = reader.ReadInt();
            m_CampBPurple = reader.ReadInt();
            m_CampBOrange = reader.ReadInt();
            m_CampBRed = reader.ReadInt();
            m_CampBYellow = reader.ReadInt();
            //Page 5
            m_CentAqua = reader.ReadInt();
            m_CentBlack = reader.ReadInt();
            m_CentBlue = reader.ReadInt();
            m_CentFRed = reader.ReadInt();
            m_CentGreen = reader.ReadInt();
            m_CentMagenta = reader.ReadInt();
            m_CentOrange = reader.ReadInt();
            m_CentPink = reader.ReadInt();
            m_CentPlain = reader.ReadInt();
            m_CentPurple = reader.ReadInt();
            m_CentRed = reader.ReadInt();
            m_CentWhite = reader.ReadInt();
            m_CentYellow = reader.ReadInt();
            m_CentBBlue = reader.ReadInt();
            m_CentBGreen = reader.ReadInt();
            m_CentBPurple = reader.ReadInt();
            m_CentBOrange = reader.ReadInt();
            m_CentBRed = reader.ReadInt();
            m_CentBYellow = reader.ReadInt();
            //Page 6
            m_EleAqua = reader.ReadInt();
            m_EleBlack = reader.ReadInt();
            m_EleBlue = reader.ReadInt();
            m_EleFRed = reader.ReadInt();
            m_EleGreen = reader.ReadInt();
            m_EleMagenta = reader.ReadInt();
            m_EleOrange = reader.ReadInt();
            m_ElePink = reader.ReadInt();
            m_ElePlain = reader.ReadInt();
            m_ElePurple = reader.ReadInt();
            m_EleRed = reader.ReadInt();
            m_EleWhite = reader.ReadInt();
            m_EleYellow = reader.ReadInt();
            m_EleBBlue = reader.ReadInt();
            m_EleBGreen = reader.ReadInt();
            m_EleBPurple = reader.ReadInt();
            m_EleBOrange = reader.ReadInt();
            m_EleBRed = reader.ReadInt();
            m_EleBYellow = reader.ReadInt();
            //Page 7
            m_FernAqua = reader.ReadInt();
            m_FernBlack = reader.ReadInt();
            m_FernBlue = reader.ReadInt();
            m_FernFRed = reader.ReadInt();
            m_FernGreen = reader.ReadInt();
            m_FernMagenta = reader.ReadInt();
            m_FernOrange = reader.ReadInt();
            m_FernPink = reader.ReadInt();
            m_FernPlain = reader.ReadInt();
            m_FernPurple = reader.ReadInt();
            m_FernRed = reader.ReadInt();
            m_FernWhite = reader.ReadInt();
            m_FernYellow = reader.ReadInt();
            m_FernBBlue = reader.ReadInt();
            m_FernBGreen = reader.ReadInt();
            m_FernBPurple = reader.ReadInt();
            m_FernBOrange = reader.ReadInt();
            m_FernBRed = reader.ReadInt();
            m_FernBYellow = reader.ReadInt();
            //Page 8
            m_LiliesAqua = reader.ReadInt();
            m_LiliesBlack = reader.ReadInt();
            m_LiliesBlue = reader.ReadInt();
            m_LiliesFRed = reader.ReadInt();
            m_LiliesGreen = reader.ReadInt();
            m_LiliesMagenta = reader.ReadInt();
            m_LiliesOrange = reader.ReadInt();
            m_LiliesPink = reader.ReadInt();
            m_LiliesPlain = reader.ReadInt();
            m_LiliesPurple = reader.ReadInt();
            m_LiliesRed = reader.ReadInt();
            m_LiliesWhite = reader.ReadInt();
            m_LiliesYellow = reader.ReadInt();
            m_LiliesBBlue = reader.ReadInt();
            m_LiliesBGreen = reader.ReadInt();
            m_LiliesBPurple = reader.ReadInt();
            m_LiliesBOrange = reader.ReadInt();
            m_LiliesBRed = reader.ReadInt();
            m_LiliesBYellow = reader.ReadInt();
            //Page 9
            m_PampAqua = reader.ReadInt();
            m_PampBlack = reader.ReadInt();
            m_PampBlue = reader.ReadInt();
            m_PampFRed = reader.ReadInt();
            m_PampGreen = reader.ReadInt();
            m_PampMagenta = reader.ReadInt();
            m_PampOrange = reader.ReadInt();
            m_PampPink = reader.ReadInt();
            m_PampPlain = reader.ReadInt();
            m_PampPurple = reader.ReadInt();
            m_PampRed = reader.ReadInt();
            m_PampWhite = reader.ReadInt();
            m_PampYellow = reader.ReadInt();
            m_PampBBlue = reader.ReadInt();
            m_PampBGreen = reader.ReadInt();
            m_PampBPurple = reader.ReadInt();
            m_PampBOrange = reader.ReadInt();
            m_PampBRed = reader.ReadInt();
            m_PampBYellow = reader.ReadInt();
            //Page 10
            m_PTPAqua = reader.ReadInt();
            m_PTPBlack = reader.ReadInt();
            m_PTPBlue = reader.ReadInt();
            m_PTPFRed = reader.ReadInt();
            m_PTPGreen = reader.ReadInt();
            m_PTPMagenta = reader.ReadInt();
            m_PTPOrange = reader.ReadInt();
            m_PTPPink = reader.ReadInt();
            m_PTPPlain = reader.ReadInt();
            m_PTPPurple = reader.ReadInt();
            m_PTPRed = reader.ReadInt();
            m_PTPWhite = reader.ReadInt();
            m_PTPYellow = reader.ReadInt();
            m_PTPBBlue = reader.ReadInt();
            m_PTPBGreen = reader.ReadInt();
            m_PTPBPurple = reader.ReadInt();
            m_PTPBOrange = reader.ReadInt();
            m_PTPBRed = reader.ReadInt();
            m_PTPBYellow = reader.ReadInt();
            //Page 11
            m_PopAqua = reader.ReadInt();
            m_PopBlack = reader.ReadInt();
            m_PopBlue = reader.ReadInt();
            m_PopFRed = reader.ReadInt();
            m_PopGreen = reader.ReadInt();
            m_PopMagenta = reader.ReadInt();
            m_PopOrange = reader.ReadInt();
            m_PopPink = reader.ReadInt();
            m_PopPlain = reader.ReadInt();
            m_PopPurple = reader.ReadInt();
            m_PopRed = reader.ReadInt();
            m_PopWhite = reader.ReadInt();
            m_PopYellow = reader.ReadInt();
            m_PopBBlue = reader.ReadInt();
            m_PopBGreen = reader.ReadInt();
            m_PopBPurple = reader.ReadInt();
            m_PopBOrange = reader.ReadInt();
            m_PopBRed = reader.ReadInt();
            m_PopBYellow = reader.ReadInt();
            //Page 12
            m_PPCAqua = reader.ReadInt();
            m_PPCBlack = reader.ReadInt();
            m_PPCBlue = reader.ReadInt();
            m_PPCFRed = reader.ReadInt();
            m_PPCGreen = reader.ReadInt();
            m_PPCMagenta = reader.ReadInt();
            m_PPCOrange = reader.ReadInt();
            m_PPCPink = reader.ReadInt();
            m_PPCPlain = reader.ReadInt();
            m_PPCPurple = reader.ReadInt();
            m_PPCRed = reader.ReadInt();
            m_PPCWhite = reader.ReadInt();
            m_PPCYellow = reader.ReadInt();
            m_PPCBBlue = reader.ReadInt();
            m_PPCBGreen = reader.ReadInt();
            m_PPCBPurple = reader.ReadInt();
            m_PPCBOrange = reader.ReadInt();
            m_PPCBRed = reader.ReadInt();
            m_PPCBYellow = reader.ReadInt();
            //Page 13
            m_RushesAqua = reader.ReadInt();
            m_RushesBlack = reader.ReadInt();
            m_RushesBlue = reader.ReadInt();
            m_RushesFRed = reader.ReadInt();
            m_RushesGreen = reader.ReadInt();
            m_RushesMagenta = reader.ReadInt();
            m_RushesOrange = reader.ReadInt();
            m_RushesPink = reader.ReadInt();
            m_RushesPlain = reader.ReadInt();
            m_RushesPurple = reader.ReadInt();
            m_RushesRed = reader.ReadInt();
            m_RushesWhite = reader.ReadInt();
            m_RushesYellow = reader.ReadInt();
            m_RushesBBlue = reader.ReadInt();
            m_RushesBGreen = reader.ReadInt();
            m_RushesBPurple = reader.ReadInt();
            m_RushesBOrange = reader.ReadInt();
            m_RushesBRed = reader.ReadInt();
            m_RushesBYellow = reader.ReadInt();
            //Page 14
            m_SnakeAqua = reader.ReadInt();
            m_SnakeBlack = reader.ReadInt();
            m_SnakeBlue = reader.ReadInt();
            m_SnakeFRed = reader.ReadInt();
            m_SnakeGreen = reader.ReadInt();
            m_SnakeMagenta = reader.ReadInt();
            m_SnakeOrange = reader.ReadInt();
            m_SnakePink = reader.ReadInt();
            m_SnakePlain = reader.ReadInt();
            m_SnakePurple = reader.ReadInt();
            m_SnakeRed = reader.ReadInt();
            m_SnakeWhite = reader.ReadInt();
            m_SnakeYellow = reader.ReadInt();
            m_SnakeBBlue = reader.ReadInt();
            m_SnakeBGreen = reader.ReadInt();
            m_SnakeBPurple = reader.ReadInt();
            m_SnakeBOrange = reader.ReadInt();
            m_SnakeBRed = reader.ReadInt();
            m_SnakeBYellow = reader.ReadInt();
            //Page 15
            m_SPalmAqua = reader.ReadInt();
            m_SPalmBlack = reader.ReadInt();
            m_SPalmBlue = reader.ReadInt();
            m_SPalmFRed = reader.ReadInt();
            m_SPalmGreen = reader.ReadInt();
            m_SPalmMagenta = reader.ReadInt();
            m_SPalmOrange = reader.ReadInt();
            m_SPalmPink = reader.ReadInt();
            m_SPalmPlain = reader.ReadInt();
            m_SPalmPurple = reader.ReadInt();
            m_SPalmRed = reader.ReadInt();
            m_SPalmWhite = reader.ReadInt();
            m_SPalmYellow = reader.ReadInt();
            m_SPalmBBlue = reader.ReadInt();
            m_SPalmBGreen = reader.ReadInt();
            m_SPalmBPurple = reader.ReadInt();
            m_SPalmBOrange = reader.ReadInt();
            m_SPalmBRed = reader.ReadInt();
            m_SPalmBYellow = reader.ReadInt();
            //Page 16
            m_SdropsAqua = reader.ReadInt();
            m_SdropsBlack = reader.ReadInt();
            m_SdropsBlue = reader.ReadInt();
            m_SdropsFRed = reader.ReadInt();
            m_SdropsGreen = reader.ReadInt();
            m_SdropsMagenta = reader.ReadInt();
            m_SdropsOrange = reader.ReadInt();
            m_SdropsPink = reader.ReadInt();
            m_SdropsPlain = reader.ReadInt();
            m_SdropsPurple = reader.ReadInt();
            m_SdropsRed = reader.ReadInt();
            m_SdropsWhite = reader.ReadInt();
            m_SdropsYellow = reader.ReadInt();
            m_SdropsBBlue = reader.ReadInt();
            m_SdropsBGreen = reader.ReadInt();
            m_SdropsBPurple = reader.ReadInt();
            m_SdropsBOrange = reader.ReadInt();
            m_SdropsBRed = reader.ReadInt();
            m_SdropsBYellow = reader.ReadInt();
            //Page 17
            m_TBCAqua = reader.ReadInt();
            m_TBCBlack = reader.ReadInt();
            m_TBCBlue = reader.ReadInt();
            m_TBCFRed = reader.ReadInt();
            m_TBCGreen = reader.ReadInt();
            m_TBCMagenta = reader.ReadInt();
            m_TBCOrange = reader.ReadInt();
            m_TBCPink = reader.ReadInt();
            m_TBCPlain = reader.ReadInt();
            m_TBCPurple = reader.ReadInt();
            m_TBCRed = reader.ReadInt();
            m_TBCWhite = reader.ReadInt();
            m_TBCYellow = reader.ReadInt();
            m_TBCBBlue = reader.ReadInt();
            m_TBCBGreen = reader.ReadInt();
            m_TBCBPurple = reader.ReadInt();
            m_TBCBOrange = reader.ReadInt();
            m_TBCBRed = reader.ReadInt();
            m_TBCBYellow = reader.ReadInt();
            //Page 18
            m_WaterAqua = reader.ReadInt();
            m_WaterBlack = reader.ReadInt();
            m_WaterBlue = reader.ReadInt();
            m_WaterFRed = reader.ReadInt();
            m_WaterGreen = reader.ReadInt();
            m_WaterMagenta = reader.ReadInt();
            m_WaterOrange = reader.ReadInt();
            m_WaterPink = reader.ReadInt();
            m_WaterPlain = reader.ReadInt();
            m_WaterPurple = reader.ReadInt();
            m_WaterRed = reader.ReadInt();
            m_WaterWhite = reader.ReadInt();
            m_WaterYellow = reader.ReadInt();
            m_WaterBBlue = reader.ReadInt();
            m_WaterBGreen = reader.ReadInt();
            m_WaterBPurple = reader.ReadInt();
            m_WaterBOrange = reader.ReadInt();
            m_WaterBRed = reader.ReadInt();
            m_WaterBYellow = reader.ReadInt();
        }
        #endregion
    }
}


namespace Server.Items
{
    public class SeedCompanionGump : Gump
	{
		private PlayerMobile m_From;
		private SeedCompanion m_Box;
        private int m_Page;
    
  
        #region Gump
		public SeedCompanionGump( PlayerMobile from, SeedCompanion box, int page ) : base( 0, 0 )
		{
			m_From = from;
			m_Box = box;
            m_Page = page;

			m_From.CloseGump( typeof( SeedCompanionGump ) );

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            AddPage(0);
            AddBackground(0, 16, 495, 243, 9200);
            AddButton(210, 230, 247, 248, (int)Buttons.ButtonOK, GumpButtonType.Reply, 0);
            AddButton(105, 233, 1209, 1210, (int)Buttons.ButtonAdd, GumpButtonType.Reply, 0);
            AddLabel(125, 230, 0, @"Add");
            switch (page)
            {
                case 1:
                    {
                        AddLabel(175, 34, 57, @"Florist's Seed Companion");
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonBC, GumpButtonType.Reply, 0);
                        AddLabel(32, 58, 0, @"Barrel Cactus");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonBull, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 0, @"Bulrushes");
                        AddLabel(32, 98, 0, @"Campion Flowers");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonCamp, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 0, @"Century Plant");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonCent, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 0, @"Elephant Ear Plant");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonEle, GumpButtonType.Reply, 0);
                        AddLabel(32, 158, 0, @"Fern");
                        AddButton(9, 160, 1209, 1210, (int)Buttons.ButtonFern, GumpButtonType.Reply, 0);
                        AddLabel(32, 178, 0, @"Lilies");
                        AddButton(9, 180, 1209, 1210, (int)Buttons.ButtonLilies, GumpButtonType.Reply, 0);
                        AddLabel(32, 198, 0, @"Pampas Grass");
                        AddButton(9, 200, 1209, 1210, (int)Buttons.ButtonPamp, GumpButtonType.Reply, 0);
                        AddImage(220, 90, 9812);
                        AddButton(330, 60, 1209, 1210, (int)Buttons.ButtonPTP, GumpButtonType.Reply, 0);
                        AddLabel(353, 58, 0, @"PonyTail Palm");
                        AddButton(330, 80, 1209, 1210, (int)Buttons.ButtonPop, GumpButtonType.Reply, 0);
                        AddLabel(353, 78, 0, @"Poppies");
                        AddLabel(353, 98, 0, @"Prickly Pear Cactus");
                        AddButton(330, 100, 1209, 1210, (int)Buttons.ButtonPPC, GumpButtonType.Reply, 0);
                        AddLabel(353, 118, 0, @"Rushes");
                        AddButton(330, 120, 1209, 1210, (int)Buttons.ButtonRushes, GumpButtonType.Reply, 0);
                        AddLabel(353, 138, 0, @"Snake Plant");
                        AddButton(330, 140, 1209, 1210, (int)Buttons.ButtonSnake, GumpButtonType.Reply, 0);
                        AddLabel(353, 158, 0, @"Small Palm");
                        AddButton(330, 160, 1209, 1210, (int)Buttons.ButtonSPalm, GumpButtonType.Reply, 0);
                        AddLabel(353, 178, 0, @"Snowdrops");
                        AddButton(330, 180, 1209, 1210, (int)Buttons.ButtonSdrops, GumpButtonType.Reply, 0);
                        AddLabel(353, 198, 0, @"TriBarrel Cactus");
                        AddButton(330, 200, 1209, 1210, (int)Buttons.ButtonTBC, GumpButtonType.Reply, 0);
                        AddLabel(353, 218, 0, @"Water Plant");
                        AddButton(330, 220, 1209, 1210, (int)Buttons.ButtonWater, GumpButtonType.Reply, 0);
                        break;
                    }
                case 2:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonBCAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Barrel Cactus");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonBCBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonBCBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonBCFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonBCGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonBCMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonBCOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonBCPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonBCPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonBCPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonBCRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonBCWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonBCYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonBCBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonBCBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonBCBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonBCBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonBCBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonBCBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.BCGreen.ToString());
                        AddLabel(70, 58, 1152, box.BCAqua.ToString());
                        AddLabel(182, 58, 1152, box.BCBlack.ToString());
                        AddLabel(285, 58, 1152, box.BCBlue.ToString());
                        AddLabel(420, 58, 1152, box.BCFRed.ToString());
                        AddLabel(199, 78, 1152, box.BCMagenta.ToString());
                        AddLabel(301, 78, 1152, box.BCOrange.ToString());
                        AddLabel(394, 78, 1152, box.BCPink.ToString());
                        AddLabel(66, 98, 1152, box.BCPlain.ToString());
                        AddLabel(188, 98, 1152, box.BCPurple.ToString());
                        AddLabel(281, 98, 1152, box.BCRed.ToString());
                        AddLabel(405, 98, 1152, box.BCWhite.ToString());
                        AddLabel(76, 118, 1152, box.BCYellow.ToString());
                        AddLabel(197, 118, 1152, box.BCBBlue.ToString());
                        AddLabel(317, 118, 1152, box.BCBGreen.ToString());
                        AddLabel(432, 118, 1152, box.BCBPurple.ToString());
                        AddLabel(101, 138, 1152, box.BCBOrange.ToString());
                        AddLabel(193, 138, 1152, box.BCBRed.ToString());
                        AddLabel(320, 138, 1152, box.BCBYellow.ToString());
                        AddItem(384, 176, 3366);
                        AddItem(0, 163, 3231);
                        AddItem(80, 176, 3367);
                        AddItem(144, 172, 3332);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(207, 176, 3367);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(268, 177, 3372);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(324, 176, 3367);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(18, 205, 0, @"Fern");
                        AddLabel(55, 160, 0, @"TriBarrel Cactus");
                        AddLabel(132, 205, 0, @"Water Plant");
                        AddLabel(183, 160, 0, @"TriBarrel Cactus");
                        AddLabel(240, 205, 0, @"Prickly Pear Cactus");
                        AddLabel(297, 160, 0, @"TriBarrel Cactus");
                        AddLabel(374, 205, 0, @"Barrel Cactus");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(366, 222, 3906, 66);
                        AddLabel(407, 233, 66, @"Br-Green");
                        break;
                    }
                case 3:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonBullAqua, GumpButtonType.Reply, 0);
                        AddLabel(200, 34, 0, @"Bulrushes");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonBullBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonBullBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonBullFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonBullGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonBullMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonBullOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonBullPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonBullPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonBullPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonBullRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonBullWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonBullYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonBullBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonBullBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonBullBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonBullBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonBullBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonBullBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.BullGreen.ToString());
                        AddLabel(70, 58, 1152, box.BullAqua.ToString());
                        AddLabel(182, 58, 1152, box.BullBlack.ToString());
                        AddLabel(285, 58, 1152, box.BullBlue.ToString());
                        AddLabel(420, 58, 1152, box.BullFRed.ToString());
                        AddLabel(199, 78, 1152, box.BullMagenta.ToString());
                        AddLabel(301, 78, 1152, box.BullOrange.ToString());
                        AddLabel(394, 78, 1152, box.BullPink.ToString());
                        AddLabel(66, 98, 1152, box.BullPlain.ToString());
                        AddLabel(188, 98, 1152, box.BullPurple.ToString());
                        AddLabel(281, 98, 1152, box.BullRed.ToString());
                        AddLabel(405, 98, 1152, box.BullWhite.ToString());
                        AddLabel(76, 118, 1152, box.BullYellow.ToString());
                        AddLabel(197, 118, 1152, box.BullBBlue.ToString());
                        AddLabel(317, 118, 1152, box.BullBGreen.ToString());
                        AddLabel(432, 118, 1152, box.BullBPurple.ToString());
                        AddLabel(101, 138, 1152, box.BullBOrange.ToString());
                        AddLabel(193, 138, 1152, box.BullBRed.ToString());
                        AddLabel(320, 138, 1152, box.BullBYellow.ToString());
                        AddItem(384, 169, 3220);
                        AddItem(15, 163, 3203);
                        AddItem(61, 171, 3231);
                        AddItem(144, 169, 3211);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(207, 163, 3203);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(268, 173, 3208);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(324, 169, 3211);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(0, 205, 0, @"Campion Flowers");
                        AddLabel(89, 161, 0, @"Fern");
                        AddLabel(150, 205, 0, @"Lilies");
                        AddLabel(187, 154, 0, @"Campion Flowers");
                        AddLabel(259, 205, 0, @"Snowdrops");
                        AddLabel(330, 160, 0, @"Lilies");
                        AddLabel(391, 206, 0, @"Bulrushes");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(369, 238, 4129, 42);
                        AddLabel(407, 233, 42, @"Br-Orange");
                        break;
                    }
                case 4:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonCampAqua, GumpButtonType.Reply, 0);
                        AddLabel(200, 34, 0, @"Campion Flowers");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonCampBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonCampBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonCampFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonCampGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonCampMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonCampOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonCampPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonCampPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonCampPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonCampRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonCampWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonCampYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonCampBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonCampBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonCampBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonCampBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonCampBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonCampBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.CampGreen.ToString());
                        AddLabel(70, 58, 1152, box.CampAqua.ToString());
                        AddLabel(182, 58, 1152, box.CampBlack.ToString());
                        AddLabel(285, 58, 1152, box.CampBlue.ToString());
                        AddLabel(420, 58, 1152, box.CampFRed.ToString());
                        AddLabel(199, 78, 1152, box.CampMagenta.ToString());
                        AddLabel(301, 78, 1152, box.CampOrange.ToString());
                        AddLabel(394, 78, 1152, box.CampPink.ToString());
                        AddLabel(66, 98, 1152, box.CampPlain.ToString());
                        AddLabel(188, 98, 1152, box.CampPurple.ToString());
                        AddLabel(281, 98, 1152, box.CampRed.ToString());
                        AddLabel(405, 98, 1152, box.CampWhite.ToString());
                        AddLabel(76, 118, 1152, box.CampYellow.ToString());
                        AddLabel(197, 118, 1152, box.CampBBlue.ToString());
                        AddLabel(317, 118, 1152, box.CampBGreen.ToString());
                        AddLabel(432, 118, 1152, box.CampBPurple.ToString());
                        AddLabel(101, 138, 1152, box.CampBOrange.ToString());
                        AddLabel(193, 138, 1152, box.CampBRed.ToString());
                        AddLabel(320, 138, 1152, box.CampBYellow.ToString());
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddLabel(197, 186, 0, @"Found on Boglings");
                        AddItem(164, 169, 9735, 1817);
                        AddItem(309, 169, 9735, 1817);
                        break;
                    }
                case 5:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonCentAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Century Plant");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonCentBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonCentBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonCentFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonCentGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonCentMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonCentOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonCentPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonCentPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonCentPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonCentRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonCentWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonCentYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonCentBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonCentBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonCentBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonCentBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonCentBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonCentBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.CentGreen.ToString());
                        AddLabel(70, 58, 1152, box.CentAqua.ToString());
                        AddLabel(182, 58, 1152, box.CentBlack.ToString());
                        AddLabel(285, 58, 1152, box.CentBlue.ToString());
                        AddLabel(420, 58, 1152, box.CentFRed.ToString());
                        AddLabel(199, 78, 1152, box.CentMagenta.ToString());
                        AddLabel(301, 78, 1152, box.CentOrange.ToString());
                        AddLabel(394, 78, 1152, box.CentPink.ToString());
                        AddLabel(66, 98, 1152, box.CentPlain.ToString());
                        AddLabel(188, 98, 1152, box.CentPurple.ToString());
                        AddLabel(281, 98, 1152, box.CentRed.ToString());
                        AddLabel(405, 98, 1152, box.CentWhite.ToString());
                        AddLabel(76, 118, 1152, box.CentYellow.ToString());
                        AddLabel(197, 118, 1152, box.CentBBlue.ToString());
                        AddLabel(317, 118, 1152, box.CentBGreen.ToString());
                        AddLabel(432, 118, 1152, box.CentBPurple.ToString());
                        AddLabel(101, 138, 1152, box.CentBOrange.ToString());
                        AddLabel(193, 138, 1152, box.CentBRed.ToString());
                        AddLabel(320, 138, 1152, box.CentBYellow.ToString());
                        AddItem(389, 131, 3377);
                        AddItem(0, 163, 3231);
                        AddItem(80, 176, 3367);
                        AddItem(144, 172, 3332);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(190, 163, 3231);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(266, 151, 3228);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(324, 176, 3332);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(18, 205, 0, @"Fern");
                        AddLabel(55, 160, 0, @"TriBarrel Cactus");
                        AddLabel(131, 205, 0, @"Water Plant");
                        AddLabel(221, 160, 0, @"Fern");
                        AddLabel(266, 205, 0, @"Small Palm");
                        AddLabel(313, 160, 0, @"Water Plant");
                        AddLabel(374, 205, 0, @"Century Plant");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(362, 230, 7813, 32);
                        AddLabel(407, 233, 32, @"Br-Red");
                        break;
                    }
                case 6:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonEleAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Elephant Ear Plant");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonEleBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonEleBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonEleFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonEleGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonEleMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonEleOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonElePink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonElePlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonElePurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonEleRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonEleWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonEleYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonEleBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonEleBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonEleBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonEleBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonEleBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonEleBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.EleGreen.ToString());
                        AddLabel(70, 58, 1152, box.EleAqua.ToString());
                        AddLabel(182, 58, 1152, box.EleBlack.ToString());
                        AddLabel(285, 58, 1152, box.EleBlue.ToString());
                        AddLabel(420, 58, 1152, box.EleFRed.ToString());
                        AddLabel(199, 78, 1152, box.EleMagenta.ToString());
                        AddLabel(301, 78, 1152, box.EleOrange.ToString());
                        AddLabel(394, 78, 1152, box.ElePink.ToString());
                        AddLabel(66, 98, 1152, box.ElePlain.ToString());
                        AddLabel(188, 98, 1152, box.ElePurple.ToString());
                        AddLabel(281, 98, 1152, box.EleRed.ToString());
                        AddLabel(405, 98, 1152, box.EleWhite.ToString());
                        AddLabel(76, 118, 1152, box.EleYellow.ToString());
                        AddLabel(197, 118, 1152, box.EleBBlue.ToString());
                        AddLabel(317, 118, 1152, box.EleBGreen.ToString());
                        AddLabel(432, 118, 1152, box.EleBPurple.ToString());
                        AddLabel(101, 138, 1152, box.EleBOrange.ToString());
                        AddLabel(193, 138, 1152, box.EleBRed.ToString());
                        AddLabel(320, 138, 1152, box.EleBYellow.ToString());
                        AddItem(368, 170, 3223);
                        AddItem(15, 163, 3203);
                        AddItem(61, 171, 3231);
                        AddItem(144, 169, 3211);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(189, 171, 3231);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(261, 164, 3239);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(305, 171, 3231);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(0, 205, 0, @"Campion Flowers");
                        AddLabel(91, 160, 0, @"Fern");
                        AddLabel(153, 208, 0, @"Lilies");
                        AddLabel(216, 160, 0, @"Fern");
                        AddLabel(275, 205, 0, @"Rushes");
                        AddLabel(335, 160, 0, @"Fern");
                        AddLabel(366, 205, 0, @"Elephant Ear Plant");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(362, 230, 7813, 32);
                        AddLabel(407, 233, 32, @"Br-Red");
                        break;
                    }
                case 7:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonFernAqua, GumpButtonType.Reply, 0);
                        AddLabel(220, 34, 0, @"Fern");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonFernBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonFernBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonFernFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonFernGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonFernMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonFernOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonFernPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonFernPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonFernPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonFernRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonFernWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonFernYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonFernBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonFernBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonFernBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonFernBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonFernBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonFernBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.FernGreen.ToString());
                        AddLabel(70, 58, 1152, box.FernAqua.ToString());
                        AddLabel(182, 58, 1152, box.FernBlack.ToString());
                        AddLabel(285, 58, 1152, box.FernBlue.ToString());
                        AddLabel(420, 58, 1152, box.FernFRed.ToString());
                        AddLabel(199, 78, 1152, box.FernMagenta.ToString());
                        AddLabel(301, 78, 1152, box.FernOrange.ToString());
                        AddLabel(394, 78, 1152, box.FernPink.ToString());
                        AddLabel(66, 98, 1152, box.FernPlain.ToString());
                        AddLabel(188, 98, 1152, box.FernPurple.ToString());
                        AddLabel(281, 98, 1152, box.FernRed.ToString());
                        AddLabel(405, 98, 1152, box.FernWhite.ToString());
                        AddLabel(76, 118, 1152, box.FernYellow.ToString());
                        AddLabel(197, 118, 1152, box.FernBBlue.ToString());
                        AddLabel(317, 118, 1152, box.FernBGreen.ToString());
                        AddLabel(432, 118, 1152, box.FernBPurple.ToString());
                        AddLabel(101, 138, 1152, box.FernBOrange.ToString());
                        AddLabel(193, 138, 1152, box.FernBRed.ToString());
                        AddLabel(320, 138, 1152, box.FernBYellow.ToString());
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddLabel(197, 186, 0, @"Found on Boglings");
                        AddItem(164, 169, 9735, 1817);
                        AddItem(309, 169, 9735, 1817);
                        break;
                    }
                case 8:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonLiliesAqua, GumpButtonType.Reply, 0);
                        AddLabel(220, 34, 0, @"Lilies");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonLiliesBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonLiliesBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonLiliesFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonLiliesGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonLiliesMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonLiliesOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonLiliesPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonLiliesPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonLiliesPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonLiliesRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonLiliesWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonLiliesYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonLiliesBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonLiliesBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonLiliesBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonLiliesBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonLiliesBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonLiliesBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.LiliesGreen.ToString());
                        AddLabel(70, 58, 1152, box.LiliesAqua.ToString());
                        AddLabel(182, 58, 1152, box.LiliesBlack.ToString());
                        AddLabel(285, 58, 1152, box.LiliesBlue.ToString());
                        AddLabel(420, 58, 1152, box.LiliesFRed.ToString());
                        AddLabel(199, 78, 1152, box.LiliesMagenta.ToString());
                        AddLabel(301, 78, 1152, box.LiliesOrange.ToString());
                        AddLabel(394, 78, 1152, box.LiliesPink.ToString());
                        AddLabel(66, 98, 1152, box.LiliesPlain.ToString());
                        AddLabel(188, 98, 1152, box.LiliesPurple.ToString());
                        AddLabel(281, 98, 1152, box.LiliesRed.ToString());
                        AddLabel(405, 98, 1152, box.LiliesWhite.ToString());
                        AddLabel(76, 118, 1152, box.LiliesYellow.ToString());
                        AddLabel(197, 118, 1152, box.LiliesBBlue.ToString());
                        AddLabel(317, 118, 1152, box.LiliesBGreen.ToString());
                        AddLabel(432, 118, 1152, box.LiliesBPurple.ToString());
                        AddLabel(101, 138, 1152, box.LiliesBOrange.ToString());
                        AddLabel(193, 138, 1152, box.LiliesBRed.ToString());
                        AddLabel(320, 138, 1152, box.LiliesBYellow.ToString());
                        AddItem(288, 162, 3211);
                        AddLabel(210, 183, 32, @"+");
                        AddItem(204, 170, 3231);
                        AddLabel(274, 183, 32, @"=");
                        AddItem(159, 164, 3203);
                        AddLabel(136, 208, 0, @"Campion Flowers");
                        AddLabel(231, 159, 0, @"Fern");
                        AddLabel(295, 204, 0, @"Lilies");
                        AddLabel(407, 26, 0, @"Max = 25");
                        break;
                    }
                case 9:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonPampAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Pampas Grass");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonPampBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonPampBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonPampFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonPampGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonPampMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonPampOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonPampPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonPampPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonPampPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonPampRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonPampWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonPampYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonPampBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonPampBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonPampBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonPampBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonPampBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonPampBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.PampGreen.ToString());
                        AddLabel(70, 58, 1152, box.PampAqua.ToString());
                        AddLabel(182, 58, 1152, box.PampBlack.ToString());
                        AddLabel(285, 58, 1152, box.PampBlue.ToString());
                        AddLabel(420, 58, 1152, box.PampFRed.ToString());
                        AddLabel(199, 78, 1152, box.PampMagenta.ToString());
                        AddLabel(301, 78, 1152, box.PampOrange.ToString());
                        AddLabel(394, 78, 1152, box.PampPink.ToString());
                        AddLabel(66, 98, 1152, box.PampPlain.ToString());
                        AddLabel(188, 98, 1152, box.PampPurple.ToString());
                        AddLabel(281, 98, 1152, box.PampRed.ToString());
                        AddLabel(405, 98, 1152, box.PampWhite.ToString());
                        AddLabel(76, 118, 1152, box.PampYellow.ToString());
                        AddLabel(197, 118, 1152, box.PampBBlue.ToString());
                        AddLabel(317, 118, 1152, box.PampBGreen.ToString());
                        AddLabel(432, 118, 1152, box.PampBPurple.ToString());
                        AddLabel(101, 138, 1152, box.PampBOrange.ToString());
                        AddLabel(193, 138, 1152, box.PampBRed.ToString());
                        AddLabel(320, 138, 1152, box.PampBYellow.ToString());
                        AddItem(381, 164, 3237);
                        AddItem(18, 166, 3203);
                        AddItem(60, 170, 3231);
                        AddItem(144, 166, 3211);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(187, 170, 3231);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(260, 165, 3239);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(324, 166, 3211);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(0, 205, 0, @"Campion Flowers");
                        AddLabel(90, 160, 0, @"Fern");
                        AddLabel(152, 205, 0, @"Lilies");
                        AddLabel(216, 160, 0, @"Fern");
                        AddLabel(277, 206, 0, @"Rushes");
                        AddLabel(328, 158, 0, @"Lilies");
                        AddLabel(374, 205, 0, @"Pampas Grass");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(369, 238, 4129, 42);
                        AddLabel(407, 233, 42, @"Br-Orange");
                        break;
                    }
                case 10:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonPTPAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Ponytail Palm");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonPTPBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonPTPBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonPTPFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonPTPGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonPTPMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonPTPOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonPTPPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonPTPPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonPTPPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonPTPRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonPTPWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonPTPYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonPTPBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonPTPBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonPTPBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonPTPBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonPTPBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonPTPBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.PTPGreen.ToString());
                        AddLabel(70, 58, 1152, box.PTPAqua.ToString());
                        AddLabel(182, 58, 1152, box.PTPBlack.ToString());
                        AddLabel(285, 58, 1152, box.PTPBlue.ToString());
                        AddLabel(420, 58, 1152, box.PTPFRed.ToString());
                        AddLabel(199, 78, 1152, box.PTPMagenta.ToString());
                        AddLabel(301, 78, 1152, box.PTPOrange.ToString());
                        AddLabel(394, 78, 1152, box.PTPPink.ToString());
                        AddLabel(66, 98, 1152, box.PTPPlain.ToString());
                        AddLabel(188, 98, 1152, box.PTPPurple.ToString());
                        AddLabel(281, 98, 1152, box.PTPRed.ToString());
                        AddLabel(405, 98, 1152, box.PTPWhite.ToString());
                        AddLabel(76, 118, 1152, box.PTPYellow.ToString());
                        AddLabel(197, 118, 1152, box.PTPBBlue.ToString());
                        AddLabel(317, 118, 1152, box.PTPBGreen.ToString());
                        AddLabel(432, 118, 1152, box.PTPBPurple.ToString());
                        AddLabel(101, 138, 1152, box.PTPBOrange.ToString());
                        AddLabel(193, 138, 1152, box.PTPBRed.ToString());
                        AddLabel(320, 138, 1152, box.PTPBYellow.ToString());
                        AddItem(372, 156, 3238);
                        AddItem(0, 165, 3231);
                        AddItem(80, 176, 3367);
                        AddItem(144, 172, 3332);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(185, 169, 3231);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(265, 155, 3228);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(304, 169, 3231);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(18, 205, 0, @"Fern");
                        AddLabel(55, 160, 0, @"TriBarrel Cactus");
                        AddLabel(131, 205, 0, @"Water Plant");
                        AddLabel(214, 160, 0, @"Fern");
                        AddLabel(261, 205, 0, @"Small Palm");
                        AddLabel(335, 160, 0, @"Fern");
                        AddLabel(374, 205, 0, @"Ponytail Palm");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(362, 230, 7813, 32);
                        AddLabel(407, 233, 32, @"Br-Red");
                        break;
                    }
                case 11:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonPopAqua, GumpButtonType.Reply, 0);
                        AddLabel(210, 34, 0, @"Poppies");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonPopBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonPopBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonPopFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonPopGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonPopMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonPopOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonPopPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonPopPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonPopPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonPopRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonPopWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonPopYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonPopBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonPopBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonPopBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonPopBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonPopBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonPopBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.PopGreen.ToString());
                        AddLabel(70, 58, 1152, box.PopAqua.ToString());
                        AddLabel(182, 58, 1152, box.PopBlack.ToString());
                        AddLabel(285, 58, 1152, box.PopBlue.ToString());
                        AddLabel(420, 58, 1152, box.PopFRed.ToString());
                        AddLabel(199, 78, 1152, box.PopMagenta.ToString());
                        AddLabel(301, 78, 1152, box.PopOrange.ToString());
                        AddLabel(394, 78, 1152, box.PopPink.ToString());
                        AddLabel(66, 98, 1152, box.PopPlain.ToString());
                        AddLabel(188, 98, 1152, box.PopPurple.ToString());
                        AddLabel(281, 98, 1152, box.PopRed.ToString());
                        AddLabel(405, 98, 1152, box.PopWhite.ToString());
                        AddLabel(76, 118, 1152, box.PopYellow.ToString());
                        AddLabel(197, 118, 1152, box.PopBBlue.ToString());
                        AddLabel(317, 118, 1152, box.PopBGreen.ToString());
                        AddLabel(432, 118, 1152, box.PopBPurple.ToString());
                        AddLabel(101, 138, 1152, box.PopBOrange.ToString());
                        AddLabel(193, 138, 1152, box.PopBRed.ToString());
                        AddLabel(320, 138, 1152, box.PopBYellow.ToString());
                        AddItem(384, 169, 3263);
                        AddItem(15, 163, 3203);
                        AddItem(61, 171, 3231);
                        AddItem(144, 169, 3211);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(207, 163, 3203);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(268, 173, 3208);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(324, 163, 3203);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(0, 205, 0, @"Campion Flowers");
                        AddLabel(89, 161, 0, @"Fern");
                        AddLabel(150, 205, 0, @"Lilies");
                        AddLabel(187, 154, 0, @"Campion Flowers");
                        AddLabel(259, 205, 0, @"Snowdrops");
                        AddLabel(306, 154, 0, @"Campion Flowers");
                        AddLabel(391, 206, 0, @"Poppies");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(369, 238, 4129, 42);
                        AddLabel(407, 233, 42, @"Br-Orange");
                        break;
                    }
                case 12:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonPPCAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Prickly Pear Cactus");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonPPCBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonPPCBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonPPCFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonPPCGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonPPCMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonPPCOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonPPCPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonPPCPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonPPCPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonPPCRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonPPCWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonPPCYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonPPCBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonPPCBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonPPCBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonPPCBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonPPCBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonPPCBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.PPCGreen.ToString());
                        AddLabel(70, 58, 1152, box.PPCAqua.ToString());
                        AddLabel(182, 58, 1152, box.PPCBlack.ToString());
                        AddLabel(285, 58, 1152, box.PPCBlue.ToString());
                        AddLabel(420, 58, 1152, box.PPCFRed.ToString());
                        AddLabel(199, 78, 1152, box.PPCMagenta.ToString());
                        AddLabel(301, 78, 1152, box.PPCOrange.ToString());
                        AddLabel(394, 78, 1152, box.PPCPink.ToString());
                        AddLabel(66, 98, 1152, box.PPCPlain.ToString());
                        AddLabel(188, 98, 1152, box.PPCPurple.ToString());
                        AddLabel(281, 98, 1152, box.PPCRed.ToString());
                        AddLabel(405, 98, 1152, box.PPCWhite.ToString());
                        AddLabel(76, 118, 1152, box.PPCYellow.ToString());
                        AddLabel(197, 118, 1152, box.PPCBBlue.ToString());
                        AddLabel(317, 118, 1152, box.PPCBGreen.ToString());
                        AddLabel(432, 118, 1152, box.PPCBPurple.ToString());
                        AddLabel(101, 138, 1152, box.PPCBOrange.ToString());
                        AddLabel(193, 138, 1152, box.PPCBRed.ToString());
                        AddLabel(320, 138, 1152, box.PPCBYellow.ToString());
                        AddItem(76, 163, 3231);
                        AddItem(156, 176, 3367);
                        AddItem(220, 172, 3332);
                        AddLabel(148, 184, 32, @"+");
                        AddLabel(206, 184, 32, @"=");
                        AddLabel(271, 184, 32, @"+");
                        AddItem(283, 176, 3367);
                        AddLabel(335, 184, 32, @"=");
                        AddItem(344, 177, 3372);
                        AddLabel(94, 205, 0, @"Fern");
                        AddLabel(131, 160, 0, @"TriBarrel Cactus");
                        AddLabel(207, 205, 0, @"Water Plant");
                        AddLabel(259, 160, 0, @"TriBarrel Cactus");
                        AddLabel(316, 205, 0, @"Prickly Pear Cactus");
                        AddLabel(407, 26, 0, @"Max = 25");
                        break;
                    }
                case 13:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonRushesAqua, GumpButtonType.Reply, 0);
                        AddLabel(220, 34, 0, @"Rushes");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonRushesBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonRushesBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonRushesFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonRushesGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonRushesMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonRushesOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonRushesPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonRushesPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonRushesPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonRushesRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonRushesWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonRushesYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonRushesBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonRushesBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonRushesBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonRushesBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonRushesBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonRushesBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.RushesGreen.ToString());
                        AddLabel(70, 58, 1152, box.RushesAqua.ToString());
                        AddLabel(182, 58, 1152, box.RushesBlack.ToString());
                        AddLabel(285, 58, 1152, box.RushesBlue.ToString());
                        AddLabel(420, 58, 1152, box.RushesFRed.ToString());
                        AddLabel(199, 78, 1152, box.RushesMagenta.ToString());
                        AddLabel(301, 78, 1152, box.RushesOrange.ToString());
                        AddLabel(394, 78, 1152, box.RushesPink.ToString());
                        AddLabel(66, 98, 1152, box.RushesPlain.ToString());
                        AddLabel(188, 98, 1152, box.RushesPurple.ToString());
                        AddLabel(281, 98, 1152, box.RushesRed.ToString());
                        AddLabel(405, 98, 1152, box.RushesWhite.ToString());
                        AddLabel(76, 118, 1152, box.RushesYellow.ToString());
                        AddLabel(197, 118, 1152, box.RushesBBlue.ToString());
                        AddLabel(317, 118, 1152, box.RushesBGreen.ToString());
                        AddLabel(432, 118, 1152, box.RushesBPurple.ToString());
                        AddLabel(101, 138, 1152, box.RushesBOrange.ToString());
                        AddLabel(193, 138, 1152, box.RushesBRed.ToString());
                        AddLabel(320, 138, 1152, box.RushesBYellow.ToString());
                        AddItem(92, 161, 3203);
                        AddItem(138, 169, 3231);
                        AddItem(221, 167, 3211);
                        AddLabel(149, 182, 32, @"+");
                        AddLabel(207, 182, 32, @"=");
                        AddLabel(272, 182, 32, @"+");
                        AddItem(266, 169, 3231);
                        AddLabel(336, 182, 32, @"=");
                        AddItem(338, 162, 3239);
                        AddLabel(60, 205, 0, @"Campion Flowers");
                        AddLabel(168, 158, 0, @"Fern");
                        AddLabel(230, 206, 0, @"Lilies");
                        AddLabel(293, 158, 0, @"Fern");
                        AddLabel(352, 203, 0, @"Rushes");
                        AddLabel(407, 26, 0, @"Max = 25");
                        break;
                    }
                case 14:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonSnakeAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Snake Plant");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonSnakeBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonSnakeBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonSnakeFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonSnakeGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonSnakeMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonSnakeOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonSnakePink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonSnakePlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonSnakePurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonSnakeRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonSnakeWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonSnakeYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonSnakeBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonSnakeBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonSnakeBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonSnakeBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonSnakeBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonSnakeBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.SnakeGreen.ToString());
                        AddLabel(70, 58, 1152, box.SnakeAqua.ToString());
                        AddLabel(182, 58, 1152, box.SnakeBlack.ToString());
                        AddLabel(285, 58, 1152, box.SnakeBlue.ToString());
                        AddLabel(420, 58, 1152, box.SnakeFRed.ToString());
                        AddLabel(199, 78, 1152, box.SnakeMagenta.ToString());
                        AddLabel(301, 78, 1152, box.SnakeOrange.ToString());
                        AddLabel(394, 78, 1152, box.SnakePink.ToString());
                        AddLabel(66, 98, 1152, box.SnakePlain.ToString());
                        AddLabel(188, 98, 1152, box.SnakePurple.ToString());
                        AddLabel(281, 98, 1152, box.SnakeRed.ToString());
                        AddLabel(405, 98, 1152, box.SnakeWhite.ToString());
                        AddLabel(76, 118, 1152, box.SnakeYellow.ToString());
                        AddLabel(197, 118, 1152, box.SnakeBBlue.ToString());
                        AddLabel(317, 118, 1152, box.SnakeBGreen.ToString());
                        AddLabel(432, 118, 1152, box.SnakeBPurple.ToString());
                        AddLabel(101, 138, 1152, box.SnakeBOrange.ToString());
                        AddLabel(193, 138, 1152, box.SnakeBRed.ToString());
                        AddLabel(320, 138, 1152, box.SnakeBYellow.ToString());
                        AddItem(389, 159, 3241);
                        AddItem(0, 163, 3231);
                        AddItem(80, 176, 3367);
                        AddItem(144, 172, 3332);
                        AddLabel(72, 184, 32, @"+");
                        AddLabel(130, 184, 32, @"=");
                        AddLabel(195, 184, 32, @"+");
                        AddItem(208, 176, 3367);
                        AddLabel(259, 184, 32, @"=");
                        AddItem(268, 177, 3372);
                        AddLabel(310, 185, 32, @"+");
                        AddItem(324, 176, 3332);
                        AddLabel(374, 185, 32, @"=");
                        AddLabel(18, 205, 0, @"Fern");
                        AddLabel(55, 160, 0, @"TriBarrel Cactus");
                        AddLabel(131, 205, 0, @"Water Plant");
                        AddLabel(175, 159, 0, @"TriBarrel Cactus");
                        AddLabel(230, 205, 0, @"Prickly Pear Cactus");
                        AddLabel(313, 160, 0, @"Water Plant");
                        AddLabel(374, 205, 0, @"Snake Plant");
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddItem(366, 222, 3906, 66);
                        AddLabel(407, 233, 66, @"Br-Green");
                        break;
                    }
                case 15:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonSPalmAqua, GumpButtonType.Reply, 0);
                        AddLabel(200, 34, 0, @"Small Palm");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonSPalmBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonSPalmBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonSPalmFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonSPalmGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonSPalmMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonSPalmOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonSPalmPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonSPalmPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonSPalmPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonSPalmRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonSPalmWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonSPalmYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonSPalmBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonSPalmBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonSPalmBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonSPalmBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonSPalmBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonSPalmBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.SPalmGreen.ToString());
                        AddLabel(70, 58, 1152, box.SPalmAqua.ToString());
                        AddLabel(182, 58, 1152, box.SPalmBlack.ToString());
                        AddLabel(285, 58, 1152, box.SPalmBlue.ToString());
                        AddLabel(420, 58, 1152, box.SPalmFRed.ToString());
                        AddLabel(199, 78, 1152, box.SPalmMagenta.ToString());
                        AddLabel(301, 78, 1152, box.SPalmOrange.ToString());
                        AddLabel(394, 78, 1152, box.SPalmPink.ToString());
                        AddLabel(66, 98, 1152, box.SPalmPlain.ToString());
                        AddLabel(188, 98, 1152, box.SPalmPurple.ToString());
                        AddLabel(281, 98, 1152, box.SPalmRed.ToString());
                        AddLabel(405, 98, 1152, box.SPalmWhite.ToString());
                        AddLabel(76, 118, 1152, box.SPalmYellow.ToString());
                        AddLabel(197, 118, 1152, box.SPalmBBlue.ToString());
                        AddLabel(317, 118, 1152, box.SPalmBGreen.ToString());
                        AddLabel(432, 118, 1152, box.SPalmBPurple.ToString());
                        AddLabel(101, 138, 1152, box.SPalmBOrange.ToString());
                        AddLabel(193, 138, 1152, box.SPalmBRed.ToString());
                        AddLabel(320, 138, 1152, box.SPalmBYellow.ToString());
                        AddItem(75, 163, 3231);
                        AddItem(156, 176, 3367);
                        AddItem(220, 172, 3332);
                        AddLabel(148, 184, 32, @"+");
                        AddLabel(206, 184, 32, @"=");
                        AddLabel(271, 184, 32, @"+");
                        AddItem(264, 163, 3231);
                        AddLabel(335, 184, 32, @"=");
                        AddItem(348, 157, 3228);
                        AddLabel(94, 205, 0, @"Fern");
                        AddLabel(131, 160, 0, @"TriBarrel Cactus");
                        AddLabel(207, 205, 0, @"Water Plant");
                        AddLabel(296, 156, 0, @"Fern");
                        AddLabel(338, 205, 0, @"Small Palm");
                        AddLabel(407, 26, 0, @"Max = 25");
                        break;
                    }
                case 16:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonSdropsAqua, GumpButtonType.Reply, 0);
                        AddLabel(210, 34, 0, @"Snowdrops");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonSdropsBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonSdropsBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonSdropsFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonSdropsGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonSdropsMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonSdropsOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonSdropsPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonSdropsPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonSdropsPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonSdropsRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonSdropsWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonSdropsYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonSdropsBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonSdropsBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonSdropsBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonSdropsBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonSdropsBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonSdropsBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.SdropsGreen.ToString());
                        AddLabel(70, 58, 1152, box.SdropsAqua.ToString());
                        AddLabel(182, 58, 1152, box.SdropsBlack.ToString());
                        AddLabel(285, 58, 1152, box.SdropsBlue.ToString());
                        AddLabel(420, 58, 1152, box.SdropsFRed.ToString());
                        AddLabel(199, 78, 1152, box.SdropsMagenta.ToString());
                        AddLabel(301, 78, 1152, box.SdropsOrange.ToString());
                        AddLabel(394, 78, 1152, box.SdropsPink.ToString());
                        AddLabel(66, 98, 1152, box.SdropsPlain.ToString());
                        AddLabel(188, 98, 1152, box.SdropsPurple.ToString());
                        AddLabel(281, 98, 1152, box.SdropsRed.ToString());
                        AddLabel(405, 98, 1152, box.SdropsWhite.ToString());
                        AddLabel(76, 118, 1152, box.SdropsYellow.ToString());
                        AddLabel(197, 118, 1152, box.SdropsBBlue.ToString());
                        AddLabel(317, 118, 1152, box.SdropsBGreen.ToString());
                        AddLabel(432, 118, 1152, box.SdropsBPurple.ToString());
                        AddLabel(101, 138, 1152, box.SdropsBOrange.ToString());
                        AddLabel(193, 138, 1152, box.SdropsBRed.ToString());
                        AddLabel(320, 138, 1152, box.SdropsBYellow.ToString());
                        AddItem(92, 161, 3203);
                        AddItem(138, 169, 3231);
                        AddItem(221, 167, 3211);
                        AddLabel(149, 182, 32, @"+");
                        AddLabel(207, 182, 32, @"=");
                        AddLabel(272, 182, 32, @"+");
                        AddItem(285, 161, 3203);
                        AddLabel(336, 182, 32, @"=");
                        AddItem(352, 168, 3208);
                        AddLabel(61, 205, 0, @"Campion Flowers");
                        AddLabel(168, 158, 0, @"Fern");
                        AddLabel(230, 206, 0, @"Lilies");
                        AddLabel(257, 158, 0, @"Campion Flowers");
                        AddLabel(342, 205, 0, @"Snowdrops");
                        AddLabel(407, 26, 0, @"Max = 25");
                        break;
                    }
                case 17:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonTBCAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"TriBarrel Cactus");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonTBCBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonTBCBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonTBCFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonTBCGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonTBCMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonTBCOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonTBCPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonTBCPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonTBCPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonTBCRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonTBCWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonTBCYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonTBCBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonTBCBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonTBCBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonTBCBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonTBCBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonTBCBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.TBCGreen.ToString());
                        AddLabel(70, 58, 1152, box.TBCAqua.ToString());
                        AddLabel(182, 58, 1152, box.TBCBlack.ToString());
                        AddLabel(285, 58, 1152, box.TBCBlue.ToString());
                        AddLabel(420, 58, 1152, box.TBCFRed.ToString());
                        AddLabel(199, 78, 1152, box.TBCMagenta.ToString());
                        AddLabel(301, 78, 1152, box.TBCOrange.ToString());
                        AddLabel(394, 78, 1152, box.TBCPink.ToString());
                        AddLabel(66, 98, 1152, box.TBCPlain.ToString());
                        AddLabel(188, 98, 1152, box.TBCPurple.ToString());
                        AddLabel(281, 98, 1152, box.TBCRed.ToString());
                        AddLabel(405, 98, 1152, box.TBCWhite.ToString());
                        AddLabel(76, 118, 1152, box.TBCYellow.ToString());
                        AddLabel(197, 118, 1152, box.TBCBBlue.ToString());
                        AddLabel(317, 118, 1152, box.TBCBGreen.ToString());
                        AddLabel(432, 118, 1152, box.TBCBPurple.ToString());
                        AddLabel(101, 138, 1152, box.TBCBOrange.ToString());
                        AddLabel(193, 138, 1152, box.TBCBRed.ToString());
                        AddLabel(320, 138, 1152, box.TBCBYellow.ToString());
                        AddLabel(407, 26, 0, @"Max = 25");
                        AddLabel(197, 186, 0, @"Found on Boglings");
                        AddItem(164, 169, 9735, 1817);
                        AddItem(309, 169, 9735, 1817);
                        break;
                    }
                case 18:
                    {
                        AddButton(9, 60, 1209, 1210, (int)Buttons.ButtonWaterAqua, GumpButtonType.Reply, 0);
                        AddLabel(195, 34, 0, @"Water Plant");
                        AddLabel(32, 58, 1172, @"Aqua");
                        AddButton(120, 60, 1209, 1210, (int)Buttons.ButtonWaterBlack, GumpButtonType.Reply, 0);
                        AddLabel(143, 58, 1108, @"Black");
                        AddButton(231, 60, 1209, 1210, (int)Buttons.ButtonWaterBlue, GumpButtonType.Reply, 0);
                        AddLabel(254, 58, 1340, @"Blue");
                        AddButton(342, 60, 1209, 1210, (int)Buttons.ButtonWaterFRed, GumpButtonType.Reply, 0);
                        AddLabel(364, 58, 1160, @"Fire Red");
                        AddButton(9, 80, 1209, 1210, (int)Buttons.ButtonWaterGreen, GumpButtonType.Reply, 0);
                        AddLabel(32, 78, 1434, @"Green");
                        AddButton(120, 80, 1209, 1209, (int)Buttons.ButtonWaterMagenta, GumpButtonType.Reply, 0);
                        AddLabel(143, 78, 1157, @"Magenta");
                        AddButton(231, 80, 1209, 1210, (int)Buttons.ButtonWaterOrange, GumpButtonType.Reply, 0);
                        AddLabel(254, 78, 1134, @"Orange");
                        AddButton(342, 80, 1209, 1210, (int)Buttons.ButtonWaterPink, GumpButtonType.Reply, 0);
                        AddLabel(364, 78, 1165, @"Pink");
                        AddButton(9, 100, 1209, 1210, (int)Buttons.ButtonWaterPlain, GumpButtonType.Reply, 0);
                        AddLabel(32, 98, 0, @"Plain");
                        AddButton(120, 100, 1209, 1209, (int)Buttons.ButtonWaterPurple, GumpButtonType.Reply, 0);
                        AddLabel(143, 98, 12, @"Purple");
                        AddButton(231, 100, 1209, 1210, (int)Buttons.ButtonWaterRed, GumpButtonType.Reply, 0);
                        AddLabel(254, 98, 1644, @"Red");
                        AddButton(342, 100, 1209, 1210, (int)Buttons.ButtonWaterWhite, GumpButtonType.Reply, 0);
                        AddLabel(364, 98, 1152, @"White");
                        AddButton(9, 120, 1209, 1210, (int)Buttons.ButtonWaterYellow, GumpButtonType.Reply, 0);
                        AddLabel(32, 118, 2212, @"Yellow");
                        AddButton(120, 120, 1209, 1209, (int)Buttons.ButtonWaterBBlue, GumpButtonType.Reply, 0);
                        AddLabel(143, 118, 4, @"Br-Blue");
                        AddButton(231, 120, 1209, 1210, (int)Buttons.ButtonWaterBGreen, GumpButtonType.Reply, 0);
                        AddLabel(254, 118, 65, @"Br-Green");
                        AddButton(342, 120, 1209, 1210, (int)Buttons.ButtonWaterBPurple, GumpButtonType.Reply, 0);
                        AddLabel(364, 118, 15, @"Br-Purple");
                        AddButton(9, 140, 1209, 1210, (int)Buttons.ButtonWaterBOrange, GumpButtonType.Reply, 0);
                        AddLabel(32, 138, 42, @"Br-Orange");
                        AddButton(120, 140, 1209, 1209, (int)Buttons.ButtonWaterBRed, GumpButtonType.Reply, 0);
                        AddLabel(143, 138, 32, @"Br-Red");
                        AddButton(231, 140, 1209, 1210, (int)Buttons.ButtonWaterBYellow, GumpButtonType.Reply, 0);
                        AddLabel(254, 138, 55, @"Br-Yellow");
                        AddLabel(72, 78, 1152, box.WaterGreen.ToString());
                        AddLabel(70, 58, 1152, box.WaterAqua.ToString());
                        AddLabel(182, 58, 1152, box.WaterBlack.ToString());
                        AddLabel(285, 58, 1152, box.WaterBlue.ToString());
                        AddLabel(420, 58, 1152, box.WaterFRed.ToString());
                        AddLabel(199, 78, 1152, box.WaterMagenta.ToString());
                        AddLabel(301, 78, 1152, box.WaterOrange.ToString());
                        AddLabel(394, 78, 1152, box.WaterPink.ToString());
                        AddLabel(66, 98, 1152, box.WaterPlain.ToString());
                        AddLabel(188, 98, 1152, box.WaterPurple.ToString());
                        AddLabel(281, 98, 1152, box.WaterRed.ToString());
                        AddLabel(405, 98, 1152, box.WaterWhite.ToString());
                        AddLabel(76, 118, 1152, box.WaterYellow.ToString());
                        AddLabel(197, 118, 1152, box.WaterBBlue.ToString());
                        AddLabel(317, 118, 1152, box.WaterBGreen.ToString());
                        AddLabel(432, 118, 1152, box.WaterBPurple.ToString());
                        AddLabel(101, 138, 1152, box.WaterBOrange.ToString());
                        AddLabel(193, 138, 1152, box.WaterBRed.ToString());
                        AddLabel(320, 138, 1152, box.WaterBYellow.ToString());
                        AddItem(286, 175, 3332);
                        AddLabel(210, 183, 32, @"+");
                        AddItem(204, 170, 3231);
                        AddLabel(274, 183, 32, @"=");
                        AddItem(159, 176, 3367);
                        AddLabel(128, 205, 0, @"TriBarrel Cactus");
                        AddLabel(231, 159, 0, @"Fern");
                        AddLabel(267, 205, 0, @"Water Plant");
                        AddLabel(407, 26, 0, @"Max = 25");
                        break;
                    }
            }

        }
    #endregion

        #region enums
        public enum Buttons
        {
            ButtonExit,
            ButtonOK,
            ButtonAdd,
            ButtonBC,
            ButtonBull,
            ButtonCamp,
            ButtonCent,
            ButtonEle,
            ButtonFern,
            ButtonLilies,
            ButtonPamp,
            ButtonPTP,
            ButtonPop,
            ButtonPPC,
            ButtonRushes,
            ButtonSnake,
            ButtonSPalm,
            ButtonSdrops,
            ButtonTBC,
            ButtonWater,
            ButtonBCAqua,
            ButtonBCBlack,
            ButtonBCBlue,
            ButtonBCFRed,
            ButtonBCGreen,
            ButtonBCMagenta,
            ButtonBCOrange,
            ButtonBCPink,
            ButtonBCPlain,
            ButtonBCPurple,
            ButtonBCRed,
            ButtonBCWhite,
            ButtonBCYellow,
            ButtonBCBBlue,
            ButtonBCBGreen,
            ButtonBCBPurple,
            ButtonBCBOrange,
            ButtonBCBRed,
            ButtonBCBYellow,
            ButtonBullAqua,
            ButtonBullBlack,
            ButtonBullBlue,
            ButtonBullFRed,
            ButtonBullGreen,
            ButtonBullMagenta,
            ButtonBullOrange,
            ButtonBullPink,
            ButtonBullPlain,
            ButtonBullPurple,
            ButtonBullRed,
            ButtonBullWhite,
            ButtonBullYellow,
            ButtonBullBBlue,
            ButtonBullBGreen,
            ButtonBullBPurple,
            ButtonBullBOrange,
            ButtonBullBRed,
            ButtonBullBYellow,
            ButtonCampAqua,
            ButtonCampBlack,
            ButtonCampBlue,
            ButtonCampFRed,
            ButtonCampGreen,
            ButtonCampMagenta,
            ButtonCampOrange,
            ButtonCampPink,
            ButtonCampPlain,
            ButtonCampPurple,
            ButtonCampRed,
            ButtonCampWhite,
            ButtonCampYellow,
            ButtonCampBBlue,
            ButtonCampBGreen,
            ButtonCampBPurple,
            ButtonCampBOrange,
            ButtonCampBRed,
            ButtonCampBYellow,
            ButtonCentAqua,
            ButtonCentBlack,
            ButtonCentBlue,
            ButtonCentFRed,
            ButtonCentGreen,
            ButtonCentMagenta,
            ButtonCentOrange,
            ButtonCentPink,
            ButtonCentPlain,
            ButtonCentPurple,
            ButtonCentRed,
            ButtonCentWhite,
            ButtonCentYellow,
            ButtonCentBBlue,
            ButtonCentBGreen,
            ButtonCentBPurple,
            ButtonCentBOrange,
            ButtonCentBRed,
            ButtonCentBYellow,
            ButtonEleAqua,
            ButtonEleBlack,
            ButtonEleBlue,
            ButtonEleFRed,
            ButtonEleGreen,
            ButtonEleMagenta,
            ButtonEleOrange,
            ButtonElePink,
            ButtonElePlain,
            ButtonElePurple,
            ButtonEleRed,
            ButtonEleWhite,
            ButtonEleYellow,
            ButtonEleBBlue,
            ButtonEleBGreen,
            ButtonEleBPurple,
            ButtonEleBOrange,
            ButtonEleBRed,
            ButtonEleBYellow,
            ButtonFernAqua,
            ButtonFernBlack,
            ButtonFernBlue,
            ButtonFernFRed,
            ButtonFernGreen,
            ButtonFernMagenta,
            ButtonFernOrange,
            ButtonFernPink,
            ButtonFernPlain,
            ButtonFernPurple,
            ButtonFernRed,
            ButtonFernWhite,
            ButtonFernYellow,
            ButtonFernBBlue,
            ButtonFernBGreen,
            ButtonFernBPurple,
            ButtonFernBOrange,
            ButtonFernBRed,
            ButtonFernBYellow,
            ButtonLiliesAqua,
            ButtonLiliesBlack,
            ButtonLiliesBlue,
            ButtonLiliesFRed,
            ButtonLiliesGreen,
            ButtonLiliesMagenta,
            ButtonLiliesOrange,
            ButtonLiliesPink,
            ButtonLiliesPlain,
            ButtonLiliesPurple,
            ButtonLiliesRed,
            ButtonLiliesWhite,
            ButtonLiliesYellow,
            ButtonLiliesBBlue,
            ButtonLiliesBGreen,
            ButtonLiliesBPurple,
            ButtonLiliesBOrange,
            ButtonLiliesBRed,
            ButtonLiliesBYellow,
            ButtonPampAqua,
            ButtonPampBlack,
            ButtonPampBlue,
            ButtonPampFRed,
            ButtonPampGreen,
            ButtonPampMagenta,
            ButtonPampOrange,
            ButtonPampPink,
            ButtonPampPlain,
            ButtonPampPurple,
            ButtonPampRed,
            ButtonPampWhite,
            ButtonPampYellow,
            ButtonPampBBlue,
            ButtonPampBGreen,
            ButtonPampBPurple,
            ButtonPampBOrange,
            ButtonPampBRed,
            ButtonPampBYellow,
            ButtonPTPAqua,
            ButtonPTPBlack,
            ButtonPTPBlue,
            ButtonPTPFRed,
            ButtonPTPGreen,
            ButtonPTPMagenta,
            ButtonPTPOrange,
            ButtonPTPPink,
            ButtonPTPPlain,
            ButtonPTPPurple,
            ButtonPTPRed,
            ButtonPTPWhite,
            ButtonPTPYellow,
            ButtonPTPBBlue,
            ButtonPTPBGreen,
            ButtonPTPBPurple,
            ButtonPTPBOrange,
            ButtonPTPBRed,
            ButtonPTPBYellow,
            ButtonPopAqua,
            ButtonPopBlack,
            ButtonPopBlue,
            ButtonPopFRed,
            ButtonPopGreen,
            ButtonPopMagenta,
            ButtonPopOrange,
            ButtonPopPink,
            ButtonPopPlain,
            ButtonPopPurple,
            ButtonPopRed,
            ButtonPopWhite,
            ButtonPopYellow,
            ButtonPopBBlue,
            ButtonPopBGreen,
            ButtonPopBPurple,
            ButtonPopBOrange,
            ButtonPopBRed,
            ButtonPopBYellow,
            ButtonPPCAqua,
            ButtonPPCBlack,
            ButtonPPCBlue,
            ButtonPPCFRed,
            ButtonPPCGreen,
            ButtonPPCMagenta,
            ButtonPPCOrange,
            ButtonPPCPink,
            ButtonPPCPlain,
            ButtonPPCPurple,
            ButtonPPCRed,
            ButtonPPCWhite,
            ButtonPPCYellow,
            ButtonPPCBBlue,
            ButtonPPCBGreen,
            ButtonPPCBPurple,
            ButtonPPCBOrange,
            ButtonPPCBRed,
            ButtonPPCBYellow,
            ButtonRushesAqua,
            ButtonRushesBlack,
            ButtonRushesBlue,
            ButtonRushesFRed,
            ButtonRushesGreen,
            ButtonRushesMagenta,
            ButtonRushesOrange,
            ButtonRushesPink,
            ButtonRushesPlain,
            ButtonRushesPurple,
            ButtonRushesRed,
            ButtonRushesWhite,
            ButtonRushesYellow,
            ButtonRushesBBlue,
            ButtonRushesBGreen,
            ButtonRushesBPurple,
            ButtonRushesBOrange,
            ButtonRushesBRed,
            ButtonRushesBYellow,
            ButtonSnakeAqua,
            ButtonSnakeBlack,
            ButtonSnakeBlue,
            ButtonSnakeFRed,
            ButtonSnakeGreen,
            ButtonSnakeMagenta,
            ButtonSnakeOrange,
            ButtonSnakePink,
            ButtonSnakePlain,
            ButtonSnakePurple,
            ButtonSnakeRed,
            ButtonSnakeWhite,
            ButtonSnakeYellow,
            ButtonSnakeBBlue,
            ButtonSnakeBGreen,
            ButtonSnakeBPurple,
            ButtonSnakeBOrange,
            ButtonSnakeBRed,
            ButtonSnakeBYellow,
            ButtonSPalmAqua,
            ButtonSPalmBlack,
            ButtonSPalmBlue,
            ButtonSPalmFRed,
            ButtonSPalmGreen,
            ButtonSPalmMagenta,
            ButtonSPalmOrange,
            ButtonSPalmPink,
            ButtonSPalmPlain,
            ButtonSPalmPurple,
            ButtonSPalmRed,
            ButtonSPalmWhite,
            ButtonSPalmYellow,
            ButtonSPalmBBlue,
            ButtonSPalmBGreen,
            ButtonSPalmBPurple,
            ButtonSPalmBOrange,
            ButtonSPalmBRed,
            ButtonSPalmBYellow,
            ButtonSdropsAqua,
            ButtonSdropsBlack,
            ButtonSdropsBlue,
            ButtonSdropsFRed,
            ButtonSdropsGreen,
            ButtonSdropsMagenta,
            ButtonSdropsOrange,
            ButtonSdropsPink,
            ButtonSdropsPlain,
            ButtonSdropsPurple,
            ButtonSdropsRed,
            ButtonSdropsWhite,
            ButtonSdropsYellow,
            ButtonSdropsBBlue,
            ButtonSdropsBGreen,
            ButtonSdropsBPurple,
            ButtonSdropsBOrange,
            ButtonSdropsBRed,
            ButtonSdropsBYellow,
            ButtonTBCAqua,
            ButtonTBCBlack,
            ButtonTBCBlue,
            ButtonTBCFRed,
            ButtonTBCGreen,
            ButtonTBCMagenta,
            ButtonTBCOrange,
            ButtonTBCPink,
            ButtonTBCPlain,
            ButtonTBCPurple,
            ButtonTBCRed,
            ButtonTBCWhite,
            ButtonTBCYellow,
            ButtonTBCBBlue,
            ButtonTBCBGreen,
            ButtonTBCBPurple,
            ButtonTBCBOrange,
            ButtonTBCBRed,
            ButtonTBCBYellow,
            ButtonWaterAqua,
            ButtonWaterBlack,
            ButtonWaterBlue,
            ButtonWaterFRed,
            ButtonWaterGreen,
            ButtonWaterMagenta,
            ButtonWaterOrange,
            ButtonWaterPink,
            ButtonWaterPlain,
            ButtonWaterPurple,
            ButtonWaterRed,
            ButtonWaterWhite,
            ButtonWaterYellow,
            ButtonWaterBBlue,
            ButtonWaterBGreen,
            ButtonWaterBPurple,
            ButtonWaterBOrange,
            ButtonWaterBRed,
            ButtonWaterBYellow,
        }

        #endregion
       
        #region OnResponse
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            
            if (m_Box.Deleted)
                return;

            switch (info.ButtonID)
            {

                case (int)Buttons.ButtonExit:
                    {
                        m_From.SendMessage("Thank you for allowing the Florist's Seed Companion to take care of all your seed storage needs.");
                        break;
                    }
                case (int)Buttons.ButtonOK:
                    {
                        if (m_Page == 1) { goto case 0; }
                        int page = 1;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }

                case (int)Buttons.ButtonAdd:
                    {
                        m_Box.BeginCombine(m_From);
                        break;
                    }
                case (int)Buttons.ButtonBC:
                    {
                        int m_Page = 2;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonBull:
                    {
                        int m_Page = 3;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonCamp:
                    {
                        int m_Page = 4;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonCent:
                    {
                        int m_Page = 5;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonEle:
                    {
                        int m_Page = 6;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonFern:
                    {
                        int m_Page = 7;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonLilies:
                    {
                        int m_Page = 8;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonPamp:
                    {
                        int m_Page = 9;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonPTP:
                    {
                        int m_Page = 10;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonPop:
                    {
                        int m_Page = 11;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonPPC:
                    {
                        int m_Page = 12;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonRushes:
                    {
                        int m_Page = 13;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonSnake:
                    {
                        int m_Page = 14;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonSPalm:
                    {
                        int m_Page = 15;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonSdrops:
                    {
                        int m_Page = 16;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonTBC:
                    {
                        int m_Page = 17;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }
                case (int)Buttons.ButtonWater:
                    {
                        int m_Page = 18;
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, m_Page));
                        break;
                    }

                case (int)Buttons.ButtonBCAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCAqua > 0)
                        {

                            if (m_Box.BCAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCAqua = m_Box.BCAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;

                    }
                case (int)Buttons.ButtonBCBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBlack > 0)
                        {

                            if (m_Box.BCBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBlack = m_Box.BCBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBlue > 0)
                        {

                            if (m_Box.BCBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBlue = m_Box.BCBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCFRed > 0)
                        {

                            if (m_Box.BCFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCFRed = m_Box.BCFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCGreen > 0)
                        {

                            if (m_Box.BCGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCGreen = m_Box.BCGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCMagenta > 0)
                        {

                            if (m_Box.BCMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCMagenta = m_Box.BCMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCOrange > 0)
                        {

                            if (m_Box.BCOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCOrange = m_Box.BCOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCPink > 0)
                        {

                            if (m_Box.BCPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCPink = m_Box.BCPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCPlain > 0)
                        {

                            if (m_Box.BCPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCPlain = m_Box.BCPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Barrel Cactus seed was added to your backpack");

                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCPurple > 0)
                        {

                            if (m_Box.BCPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCPurple = m_Box.BCPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCRed > 0)
                        {

                            if (m_Box.BCRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCRed = m_Box.BCRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCWhite > 0)
                        {

                            if (m_Box.BCWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCWhite = m_Box.BCWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCYellow > 0)
                        {

                            if (m_Box.BCYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCYellow = m_Box.BCYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBBlue > 0)
                        {

                            if (m_Box.BCBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBBlue = m_Box.BCBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBGreen > 0)
                        {

                            if (m_Box.BCBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBGreen = m_Box.BCBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBPurple > 0)
                        {

                            if (m_Box.BCBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBPurple = m_Box.BCBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBOrange > 0)
                        {

                            if (m_Box.BCBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBOrange = m_Box.BCBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBRed > 0)
                        {

                            if (m_Box.BCBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBRed = m_Box.BCBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBCBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.BarrelCactus;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 2;

                        if (m_Box.BCBYellow > 0)
                        {

                            if (m_Box.BCBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBYellow = m_Box.BCBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Barrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BCBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BCBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Barrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Barrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullAqua > 0)
                        {

                            if (m_Box.BullAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullAqua = m_Box.BullAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBlack > 0)
                        {

                            if (m_Box.BullBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBlack = m_Box.BullBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBlue > 0)
                        {

                            if (m_Box.BullBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBlue = m_Box.BullBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullFRed > 0)
                        {

                            if (m_Box.BullFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullFRed = m_Box.BullFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullGreen > 0)
                        {

                            if (m_Box.BullGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullGreen = m_Box.BullGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullMagenta > 0)
                        {

                            if (m_Box.BullMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullMagenta = m_Box.BullMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullOrange > 0)
                        {

                            if (m_Box.BullOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullOrange = m_Box.BullOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullPink > 0)
                        {

                            if (m_Box.BullPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullPink = m_Box.BullPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullPlain > 0)
                        {

                            if (m_Box.BullPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullPlain = m_Box.BullPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullPurple > 0)
                        {

                            if (m_Box.BullPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullPurple = m_Box.BullPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullRed > 0)
                        {

                            if (m_Box.BullRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullRed = m_Box.BullRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullWhite > 0)
                        {

                            if (m_Box.BullWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullWhite = m_Box.BullWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullYellow > 0)
                        {

                            if (m_Box.BullYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullYellow = m_Box.BullYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBBlue > 0)
                        {

                            if (m_Box.BullBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBBlue = m_Box.BullBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBGreen > 0)
                        {

                            if (m_Box.BullBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBGreen = m_Box.BullBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBPurple > 0)
                        {

                            if (m_Box.BullBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBPurple = m_Box.BullBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBOrange > 0)
                        {

                            if (m_Box.BullBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBOrange = m_Box.BullBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBRed > 0)
                        {

                            if (m_Box.BullBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBRed = m_Box.BullBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonBullBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Bulrushes;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 3;

                        if (m_Box.BullBYellow > 0)
                        {

                            if (m_Box.BullBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBYellow = m_Box.BullBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Bulrushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.BullBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.BullBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Bulrushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Bulrushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampAqua > 0)
                        {

                            if (m_Box.CampAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampAqua = m_Box.CampAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBlack > 0)
                        {

                            if (m_Box.CampBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBlack = m_Box.CampBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBlue > 0)
                        {

                            if (m_Box.CampBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBlue = m_Box.CampBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampFRed > 0)
                        {

                            if (m_Box.CampFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampFRed = m_Box.CampFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampGreen > 0)
                        {

                            if (m_Box.CampGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampGreen = m_Box.CampGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampMagenta > 0)
                        {

                            if (m_Box.CampMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampMagenta = m_Box.CampMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampOrange > 0)
                        {

                            if (m_Box.CampOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampOrange = m_Box.CampOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampPink > 0)
                        {

                            if (m_Box.CampPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampPink = m_Box.CampPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampPlain > 0)
                        {

                            if (m_Box.CampPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampPlain = m_Box.CampPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampPurple > 0)
                        {

                            if (m_Box.CampPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampPurple = m_Box.CampPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampRed > 0)
                        {

                            if (m_Box.CampRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampRed = m_Box.CampRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampWhite > 0)
                        {

                            if (m_Box.CampWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampWhite = m_Box.CampWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampYellow > 0)
                        {

                            if (m_Box.CampYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampYellow = m_Box.CampYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBBlue > 0)
                        {

                            if (m_Box.CampBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBBlue = m_Box.CampBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBGreen > 0)
                        {

                            if (m_Box.CampBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBGreen = m_Box.CampBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBPurple > 0)
                        {

                            if (m_Box.CampBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBPurple = m_Box.CampBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBOrange > 0)
                        {

                            if (m_Box.CampBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBOrange = m_Box.CampBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBRed > 0)
                        {

                            if (m_Box.CampBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBRed = m_Box.CampBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCampBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CampionFlowers;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 4;

                        if (m_Box.CampBYellow > 0)
                        {

                            if (m_Box.CampBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBYellow = m_Box.CampBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Campion Flowers seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CampBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CampBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Campion Flowers seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Campion Flowers seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentAqua > 0)
                        {

                            if (m_Box.CentAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentAqua = m_Box.CentAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBlack > 0)
                        {

                            if (m_Box.CentBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBlack = m_Box.CentBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBlue > 0)
                        {

                            if (m_Box.CentBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBlue = m_Box.CentBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentFRed > 0)
                        {

                            if (m_Box.CentFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentFRed = m_Box.CentFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentGreen > 0)
                        {

                            if (m_Box.CentGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentGreen = m_Box.CentGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentMagenta > 0)
                        {

                            if (m_Box.CentMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentMagenta = m_Box.CentMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentOrange > 0)
                        {

                            if (m_Box.CentOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentOrange = m_Box.CentOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentPink > 0)
                        {

                            if (m_Box.CentPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentPink = m_Box.CentPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentPlain > 0)
                        {

                            if (m_Box.CentPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentPlain = m_Box.CentPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentPurple > 0)
                        {

                            if (m_Box.CentPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentPurple = m_Box.CentPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentRed > 0)
                        {

                            if (m_Box.CentRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentRed = m_Box.CentRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentWhite > 0)
                        {

                            if (m_Box.CentWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentWhite = m_Box.CentWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentYellow > 0)
                        {

                            if (m_Box.CentYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentYellow = m_Box.CentYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBBlue > 0)
                        {

                            if (m_Box.CentBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBBlue = m_Box.CentBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBGreen > 0)
                        {

                            if (m_Box.CentBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBGreen = m_Box.CentBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBPurple > 0)
                        {

                            if (m_Box.CentBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBPurple = m_Box.CentBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBOrange > 0)
                        {

                            if (m_Box.CentBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBOrange = m_Box.CentBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBRed > 0)
                        {

                            if (m_Box.CentBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBRed = m_Box.CentBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonCentBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.CenturyPlant;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 5;

                        if (m_Box.CentBYellow > 0)
                        {

                            if (m_Box.CentBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBYellow = m_Box.CentBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Century Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.CentBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.CentBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Century Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Century Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleAqua > 0)
                        {

                            if (m_Box.EleAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleAqua = m_Box.EleAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBlack > 0)
                        {

                            if (m_Box.EleBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBlack = m_Box.EleBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBlue > 0)
                        {

                            if (m_Box.EleBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBlue = m_Box.EleBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleFRed > 0)
                        {

                            if (m_Box.EleFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleFRed = m_Box.EleFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleGreen > 0)
                        {

                            if (m_Box.EleGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleGreen = m_Box.EleGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleMagenta > 0)
                        {

                            if (m_Box.EleMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleMagenta = m_Box.EleMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleOrange > 0)
                        {

                            if (m_Box.EleOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleOrange = m_Box.EleOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonElePink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.ElePink > 0)
                        {

                            if (m_Box.ElePink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.ElePink = m_Box.ElePink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.ElePink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.ElePink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonElePlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.ElePlain > 0)
                        {

                            if (m_Box.ElePlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.ElePlain = m_Box.ElePlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.ElePlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.ElePlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonElePurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.ElePurple > 0)
                        {

                            if (m_Box.ElePurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.ElePurple = m_Box.ElePurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.ElePurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.ElePurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleRed > 0)
                        {

                            if (m_Box.EleRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleRed = m_Box.EleRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleWhite > 0)
                        {

                            if (m_Box.EleWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleWhite = m_Box.EleWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleYellow > 0)
                        {

                            if (m_Box.EleYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleYellow = m_Box.EleYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBBlue > 0)
                        {

                            if (m_Box.EleBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBBlue = m_Box.EleBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBGreen > 0)
                        {

                            if (m_Box.EleBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBGreen = m_Box.EleBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBPurple > 0)
                        {

                            if (m_Box.EleBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBPurple = m_Box.EleBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBOrange > 0)
                        {

                            if (m_Box.EleBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBOrange = m_Box.EleBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBRed > 0)
                        {

                            if (m_Box.EleBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBRed = m_Box.EleBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonEleBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.ElephantEarPlant;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 6;

                        if (m_Box.EleBYellow > 0)
                        {

                            if (m_Box.EleBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBYellow = m_Box.EleBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.EleBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.EleBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Elepant Ear Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Elephant Ear Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernAqua > 0)
                        {

                            if (m_Box.FernAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernAqua = m_Box.FernAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBlack > 0)
                        {

                            if (m_Box.FernBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBlack = m_Box.FernBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBlue > 0)
                        {

                            if (m_Box.FernBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBlue = m_Box.FernBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernFRed > 0)
                        {

                            if (m_Box.FernFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernFRed = m_Box.FernFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernGreen > 0)
                        {

                            if (m_Box.FernGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernGreen = m_Box.FernGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernMagenta > 0)
                        {

                            if (m_Box.FernMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernMagenta = m_Box.FernMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernOrange > 0)
                        {

                            if (m_Box.FernOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernOrange = m_Box.FernOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernPink > 0)
                        {

                            if (m_Box.FernPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernPink = m_Box.FernPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernPlain > 0)
                        {

                            if (m_Box.FernPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernPlain = m_Box.FernPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernPurple > 0)
                        {

                            if (m_Box.FernPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernPurple = m_Box.FernPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernRed > 0)
                        {

                            if (m_Box.FernRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernRed = m_Box.FernRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernWhite > 0)
                        {

                            if (m_Box.FernWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernWhite = m_Box.FernWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernYellow > 0)
                        {

                            if (m_Box.FernYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernYellow = m_Box.FernYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBBlue > 0)
                        {

                            if (m_Box.FernBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBBlue = m_Box.FernBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBGreen > 0)
                        {

                            if (m_Box.FernBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBGreen = m_Box.FernBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBPurple > 0)
                        {

                            if (m_Box.FernBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBPurple = m_Box.FernBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBOrange > 0)
                        {

                            if (m_Box.FernBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBOrange = m_Box.FernBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBRed > 0)
                        {

                            if (m_Box.FernBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBRed = m_Box.FernBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonFernBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Fern;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 7;

                        if (m_Box.FernBYellow > 0)
                        {

                            if (m_Box.FernBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBYellow = m_Box.FernBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Fern seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.FernBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.FernBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Fern seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Fern seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesAqua > 0)
                        {

                            if (m_Box.LiliesAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesAqua = m_Box.LiliesAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBlack > 0)
                        {

                            if (m_Box.LiliesBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBlack = m_Box.LiliesBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBlue > 0)
                        {

                            if (m_Box.LiliesBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBlue = m_Box.LiliesBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesFRed > 0)
                        {

                            if (m_Box.LiliesFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesFRed = m_Box.LiliesFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesGreen > 0)
                        {

                            if (m_Box.LiliesGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesGreen = m_Box.LiliesGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesMagenta > 0)
                        {

                            if (m_Box.LiliesMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesMagenta = m_Box.LiliesMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesOrange > 0)
                        {

                            if (m_Box.LiliesOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesOrange = m_Box.LiliesOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesPink > 0)
                        {

                            if (m_Box.LiliesPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesPink = m_Box.LiliesPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesPlain > 0)
                        {

                            if (m_Box.LiliesPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesPlain = m_Box.LiliesPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesPurple > 0)
                        {

                            if (m_Box.LiliesPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesPurple = m_Box.LiliesPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesRed > 0)
                        {

                            if (m_Box.LiliesRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesRed = m_Box.LiliesRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesWhite > 0)
                        {

                            if (m_Box.LiliesWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesWhite = m_Box.LiliesWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesYellow > 0)
                        {

                            if (m_Box.LiliesYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesYellow = m_Box.LiliesYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBBlue > 0)
                        {

                            if (m_Box.LiliesBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBBlue = m_Box.LiliesBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBGreen > 0)
                        {

                            if (m_Box.LiliesBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBGreen = m_Box.LiliesBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBPurple > 0)
                        {

                            if (m_Box.LiliesBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBPurple = m_Box.LiliesBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBOrange > 0)
                        {

                            if (m_Box.LiliesBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBOrange = m_Box.LiliesBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBRed > 0)
                        {

                            if (m_Box.LiliesBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBRed = m_Box.LiliesBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonLiliesBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Lilies;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 8;

                        if (m_Box.LiliesBYellow > 0)
                        {

                            if (m_Box.LiliesBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBYellow = m_Box.LiliesBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Lilies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.LiliesBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.LiliesBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Lilies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Lilies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampAqua > 0)
                        {

                            if (m_Box.PampAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampAqua = m_Box.PampAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBlack > 0)
                        {

                            if (m_Box.PampBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBlack = m_Box.PampBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBlue > 0)
                        {

                            if (m_Box.PampBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBlue = m_Box.PampBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampFRed > 0)
                        {

                            if (m_Box.PampFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampFRed = m_Box.PampFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampGreen > 0)
                        {

                            if (m_Box.PampGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampGreen = m_Box.PampGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampMagenta > 0)
                        {

                            if (m_Box.PampMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampMagenta = m_Box.PampMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampOrange > 0)
                        {

                            if (m_Box.PampOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampOrange = m_Box.PampOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampPink > 0)
                        {

                            if (m_Box.PampPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampPink = m_Box.PampPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampPlain > 0)
                        {

                            if (m_Box.PampPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampPlain = m_Box.PampPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampPurple > 0)
                        {

                            if (m_Box.PampPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampPurple = m_Box.PampPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampRed > 0)
                        {

                            if (m_Box.PampRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampRed = m_Box.PampRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampWhite > 0)
                        {

                            if (m_Box.PampWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampWhite = m_Box.PampWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampYellow > 0)
                        {

                            if (m_Box.PampYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampYellow = m_Box.PampYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBBlue > 0)
                        {

                            if (m_Box.PampBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBBlue = m_Box.PampBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBGreen > 0)
                        {

                            if (m_Box.PampBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBGreen = m_Box.PampBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBPurple > 0)
                        {

                            if (m_Box.PampBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBPurple = m_Box.PampBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBOrange > 0)
                        {

                            if (m_Box.PampBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBOrange = m_Box.PampBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBRed > 0)
                        {

                            if (m_Box.PampBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBRed = m_Box.PampBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPampBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PampasGrass;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 9;

                        if (m_Box.PampBYellow > 0)
                        {

                            if (m_Box.PampBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBYellow = m_Box.PampBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Pampas Grass seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PampBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PampBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Pampas Grass seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Pampas Grass seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPAqua > 0)
                        {

                            if (m_Box.PTPAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPAqua = m_Box.PTPAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBlack > 0)
                        {

                            if (m_Box.PTPBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBlack = m_Box.PTPBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBlue > 0)
                        {

                            if (m_Box.PTPBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBlue = m_Box.PTPBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPFRed > 0)
                        {

                            if (m_Box.PTPFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPFRed = m_Box.PTPFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPGreen > 0)
                        {

                            if (m_Box.PTPGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPGreen = m_Box.PTPGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPMagenta > 0)
                        {

                            if (m_Box.PTPMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPMagenta = m_Box.PTPMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPOrange > 0)
                        {

                            if (m_Box.PTPOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPOrange = m_Box.PTPOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPPink > 0)
                        {

                            if (m_Box.PTPPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPPink = m_Box.PTPPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPPlain > 0)
                        {

                            if (m_Box.PTPPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPPlain = m_Box.PTPPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPPurple > 0)
                        {

                            if (m_Box.PTPPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPPurple = m_Box.PTPPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPRed > 0)
                        {

                            if (m_Box.PTPRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPRed = m_Box.PTPRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPWhite > 0)
                        {

                            if (m_Box.PTPWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPWhite = m_Box.PTPWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPYellow > 0)
                        {

                            if (m_Box.PTPYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPYellow = m_Box.PTPYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBBlue > 0)
                        {

                            if (m_Box.PTPBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBBlue = m_Box.PTPBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBGreen > 0)
                        {

                            if (m_Box.PTPBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBGreen = m_Box.PTPBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBPurple > 0)
                        {

                            if (m_Box.PTPBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBPurple = m_Box.PTPBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBOrange > 0)
                        {

                            if (m_Box.PTPBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBOrange = m_Box.PTPBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBRed > 0)
                        {

                            if (m_Box.PTPBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBRed = m_Box.PTPBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPTPBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PonytailPalm;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 10;

                        if (m_Box.PTPBYellow > 0)
                        {

                            if (m_Box.PTPBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBYellow = m_Box.PTPBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Ponytail Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PTPBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PTPBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Ponytail Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Ponytail Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopAqua > 0)
                        {

                            if (m_Box.PopAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopAqua = m_Box.PopAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBlack > 0)
                        {

                            if (m_Box.PopBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBlack = m_Box.PopBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBlue > 0)
                        {

                            if (m_Box.PopBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBlue = m_Box.PopBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopFRed > 0)
                        {

                            if (m_Box.PopFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopFRed = m_Box.PopFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopGreen > 0)
                        {

                            if (m_Box.PopGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopGreen = m_Box.PopGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopMagenta > 0)
                        {

                            if (m_Box.PopMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopMagenta = m_Box.PopMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopOrange > 0)
                        {

                            if (m_Box.PopOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopOrange = m_Box.PopOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopPink > 0)
                        {

                            if (m_Box.PopPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopPink = m_Box.PopPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopPlain > 0)
                        {

                            if (m_Box.PopPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopPlain = m_Box.PopPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopPurple > 0)
                        {

                            if (m_Box.PopPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopPurple = m_Box.PopPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopRed > 0)
                        {

                            if (m_Box.PopRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopRed = m_Box.PopRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopWhite > 0)
                        {

                            if (m_Box.PopWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopWhite = m_Box.PopWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopYellow > 0)
                        {

                            if (m_Box.PopYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopYellow = m_Box.PopYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBBlue > 0)
                        {

                            if (m_Box.PopBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBBlue = m_Box.PopBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBGreen > 0)
                        {

                            if (m_Box.PopBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBGreen = m_Box.PopBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBPurple > 0)
                        {

                            if (m_Box.PopBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBPurple = m_Box.PopBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBOrange > 0)
                        {

                            if (m_Box.PopBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBOrange = m_Box.PopBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBRed > 0)
                        {

                            if (m_Box.PopBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBRed = m_Box.PopBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPopBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Poppies;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 11;

                        if (m_Box.PopBYellow > 0)
                        {

                            if (m_Box.PopBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBYellow = m_Box.PopBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Poppies seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PopBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PopBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Poppies seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Poppies seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCAqua > 0)
                        {

                            if (m_Box.PPCAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCAqua = m_Box.PPCAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBlack > 0)
                        {

                            if (m_Box.PPCBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBlack = m_Box.PPCBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBlue > 0)
                        {

                            if (m_Box.PPCBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBlue = m_Box.PPCBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCFRed > 0)
                        {

                            if (m_Box.PPCFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCFRed = m_Box.PPCFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCGreen > 0)
                        {

                            if (m_Box.PPCGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCGreen = m_Box.PPCGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCMagenta > 0)
                        {

                            if (m_Box.PPCMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCMagenta = m_Box.PPCMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCOrange > 0)
                        {

                            if (m_Box.PPCOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCOrange = m_Box.PPCOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCPink > 0)
                        {

                            if (m_Box.PPCPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCPink = m_Box.PPCPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCPlain > 0)
                        {

                            if (m_Box.PPCPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCPlain = m_Box.PPCPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCPurple > 0)
                        {

                            if (m_Box.PPCPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCPurple = m_Box.PPCPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCRed > 0)
                        {

                            if (m_Box.PPCRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCRed = m_Box.PPCRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCWhite > 0)
                        {

                            if (m_Box.PPCWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCWhite = m_Box.PPCWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCYellow > 0)
                        {

                            if (m_Box.PPCYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCYellow = m_Box.PPCYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBBlue > 0)
                        {

                            if (m_Box.PPCBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBBlue = m_Box.PPCBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBGreen > 0)
                        {

                            if (m_Box.PPCBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBGreen = m_Box.PPCBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBPurple > 0)
                        {

                            if (m_Box.PPCBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBPurple = m_Box.PPCBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBOrange > 0)
                        {

                            if (m_Box.PPCBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBOrange = m_Box.PPCBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBRed > 0)
                        {

                            if (m_Box.PPCBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBRed = m_Box.PPCBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonPPCBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.PricklyPearCactus;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 12;

                        if (m_Box.PPCBYellow > 0)
                        {

                            if (m_Box.PPCBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBYellow = m_Box.PPCBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.PPCBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.PPCBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Prickly Pear Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Prickly Pear Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesAqua > 0)
                        {

                            if (m_Box.RushesAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesAqua = m_Box.RushesAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBlack > 0)
                        {

                            if (m_Box.RushesBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBlack = m_Box.RushesBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBlue > 0)
                        {

                            if (m_Box.RushesBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBlue = m_Box.RushesBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesFRed > 0)
                        {

                            if (m_Box.RushesFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesFRed = m_Box.RushesFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesGreen > 0)
                        {

                            if (m_Box.RushesGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesGreen = m_Box.RushesGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesMagenta > 0)
                        {

                            if (m_Box.RushesMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesMagenta = m_Box.RushesMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesOrange > 0)
                        {

                            if (m_Box.RushesOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesOrange = m_Box.RushesOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesPink > 0)
                        {

                            if (m_Box.RushesPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesPink = m_Box.RushesPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesPlain > 0)
                        {

                            if (m_Box.RushesPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesPlain = m_Box.RushesPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesPurple > 0)
                        {

                            if (m_Box.RushesPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesPurple = m_Box.RushesPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesRed > 0)
                        {

                            if (m_Box.RushesRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesRed = m_Box.RushesRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesWhite > 0)
                        {

                            if (m_Box.RushesWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesWhite = m_Box.RushesWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesYellow > 0)
                        {

                            if (m_Box.RushesYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesYellow = m_Box.RushesYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBBlue > 0)
                        {

                            if (m_Box.RushesBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBBlue = m_Box.RushesBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBGreen > 0)
                        {

                            if (m_Box.RushesBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBGreen = m_Box.RushesBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBPurple > 0)
                        {

                            if (m_Box.RushesBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBPurple = m_Box.RushesBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBOrange > 0)
                        {

                            if (m_Box.RushesBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBOrange = m_Box.RushesBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBRed > 0)
                        {

                            if (m_Box.RushesBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBRed = m_Box.RushesBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonRushesBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Rushes;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 13;

                        if (m_Box.RushesBYellow > 0)
                        {

                            if (m_Box.RushesBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBYellow = m_Box.RushesBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Rushes seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.RushesBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.RushesBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Rushes seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Rushes seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeAqua > 0)
                        {

                            if (m_Box.SnakeAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeAqua = m_Box.SnakeAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBlack > 0)
                        {

                            if (m_Box.SnakeBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBlack = m_Box.SnakeBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBlue > 0)
                        {

                            if (m_Box.SnakeBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBlue = m_Box.SnakeBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeFRed > 0)
                        {

                            if (m_Box.SnakeFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeFRed = m_Box.SnakeFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeGreen > 0)
                        {

                            if (m_Box.SnakeGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeGreen = m_Box.SnakeGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeMagenta > 0)
                        {

                            if (m_Box.SnakeMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeMagenta = m_Box.SnakeMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeOrange > 0)
                        {

                            if (m_Box.SnakeOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeOrange = m_Box.SnakeOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakePink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakePink > 0)
                        {

                            if (m_Box.SnakePink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakePink = m_Box.SnakePink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakePink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakePink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakePlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakePlain > 0)
                        {

                            if (m_Box.SnakePlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakePlain = m_Box.SnakePlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakePlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakePlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakePurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakePurple > 0)
                        {

                            if (m_Box.SnakePurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakePurple = m_Box.SnakePurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakePurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakePurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeRed > 0)
                        {

                            if (m_Box.SnakeRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeRed = m_Box.SnakeRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeWhite > 0)
                        {

                            if (m_Box.SnakeWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeWhite = m_Box.SnakeWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeYellow > 0)
                        {

                            if (m_Box.SnakeYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeYellow = m_Box.SnakeYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBBlue > 0)
                        {

                            if (m_Box.SnakeBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBBlue = m_Box.SnakeBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBGreen > 0)
                        {

                            if (m_Box.SnakeBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBGreen = m_Box.SnakeBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBPurple > 0)
                        {

                            if (m_Box.SnakeBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBPurple = m_Box.SnakeBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBOrange > 0)
                        {

                            if (m_Box.SnakeBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBOrange = m_Box.SnakeBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBRed > 0)
                        {

                            if (m_Box.SnakeBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBRed = m_Box.SnakeBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSnakeBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SnakePlant;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 14;

                        if (m_Box.SnakeBYellow > 0)
                        {

                            if (m_Box.SnakeBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBYellow = m_Box.SnakeBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Snake Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SnakeBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SnakeBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Snake Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Snake Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmAqua > 0)
                        {

                            if (m_Box.SPalmAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmAqua = m_Box.SPalmAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBlack > 0)
                        {

                            if (m_Box.SPalmBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBlack = m_Box.SPalmBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBlue > 0)
                        {

                            if (m_Box.SPalmBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBlue = m_Box.SPalmBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmFRed > 0)
                        {

                            if (m_Box.SPalmFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmFRed = m_Box.SPalmFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmGreen > 0)
                        {

                            if (m_Box.SPalmGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmGreen = m_Box.SPalmGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmMagenta > 0)
                        {

                            if (m_Box.SPalmMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmMagenta = m_Box.SPalmMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmOrange > 0)
                        {

                            if (m_Box.SPalmOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmOrange = m_Box.SPalmOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmPink > 0)
                        {

                            if (m_Box.SPalmPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmPink = m_Box.SPalmPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmPlain > 0)
                        {

                            if (m_Box.SPalmPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmPlain = m_Box.SPalmPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmPurple > 0)
                        {

                            if (m_Box.SPalmPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmPurple = m_Box.SPalmPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmRed > 0)
                        {

                            if (m_Box.SPalmRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmRed = m_Box.SPalmRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmWhite > 0)
                        {

                            if (m_Box.SPalmWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmWhite = m_Box.SPalmWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmYellow > 0)
                        {

                            if (m_Box.SPalmYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmYellow = m_Box.SPalmYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBBlue > 0)
                        {

                            if (m_Box.SPalmBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBBlue = m_Box.SPalmBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBGreen > 0)
                        {

                            if (m_Box.SPalmBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBGreen = m_Box.SPalmBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBPurple > 0)
                        {

                            if (m_Box.SPalmBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBPurple = m_Box.SPalmBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBOrange > 0)
                        {

                            if (m_Box.SPalmBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBOrange = m_Box.SPalmBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBRed > 0)
                        {

                            if (m_Box.SPalmBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBRed = m_Box.SPalmBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSPalmBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.SmallPalm;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 15;

                        if (m_Box.SPalmBYellow > 0)
                        {

                            if (m_Box.SPalmBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBYellow = m_Box.SPalmBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Small Palm seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SPalmBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SPalmBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Small Palm seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Small Palm seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsAqua > 0)
                        {

                            if (m_Box.SdropsAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsAqua = m_Box.SdropsAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBlack > 0)
                        {

                            if (m_Box.SdropsBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBlack = m_Box.SdropsBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBlue > 0)
                        {

                            if (m_Box.SdropsBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBlue = m_Box.SdropsBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsFRed > 0)
                        {

                            if (m_Box.SdropsFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsFRed = m_Box.SdropsFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsGreen > 0)
                        {

                            if (m_Box.SdropsGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsGreen = m_Box.SdropsGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsMagenta > 0)
                        {

                            if (m_Box.SdropsMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsMagenta = m_Box.SdropsMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsOrange > 0)
                        {

                            if (m_Box.SdropsOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsOrange = m_Box.SdropsOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsPink > 0)
                        {

                            if (m_Box.SdropsPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsPink = m_Box.SdropsPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsPlain > 0)
                        {

                            if (m_Box.SdropsPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsPlain = m_Box.SdropsPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsPurple > 0)
                        {

                            if (m_Box.SdropsPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsPurple = m_Box.SdropsPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsRed > 0)
                        {

                            if (m_Box.SdropsRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsRed = m_Box.SdropsRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsWhite > 0)
                        {

                            if (m_Box.SdropsWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsWhite = m_Box.SdropsWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsYellow > 0)
                        {

                            if (m_Box.SdropsYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsYellow = m_Box.SdropsYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBBlue > 0)
                        {

                            if (m_Box.SdropsBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBBlue = m_Box.SdropsBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBGreen > 0)
                        {

                            if (m_Box.SdropsBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBGreen = m_Box.SdropsBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBPurple > 0)
                        {

                            if (m_Box.SdropsBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBPurple = m_Box.SdropsBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBOrange > 0)
                        {

                            if (m_Box.SdropsBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBOrange = m_Box.SdropsBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBRed > 0)
                        {

                            if (m_Box.SdropsBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBRed = m_Box.SdropsBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonSdropsBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.Snowdrops;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 16;

                        if (m_Box.SdropsBYellow > 0)
                        {

                            if (m_Box.SdropsBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBYellow = m_Box.SdropsBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Snowdrops seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.SdropsBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.SdropsBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Snowdrops seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Snowdrops seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCAqua > 0)
                        {

                            if (m_Box.TBCAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCAqua = m_Box.TBCAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBlack > 0)
                        {

                            if (m_Box.TBCBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBlack = m_Box.TBCBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBlue > 0)
                        {

                            if (m_Box.TBCBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBlue = m_Box.TBCBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCFRed > 0)
                        {

                            if (m_Box.TBCFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCFRed = m_Box.TBCFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCGreen > 0)
                        {

                            if (m_Box.TBCGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCGreen = m_Box.TBCGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCMagenta > 0)
                        {

                            if (m_Box.TBCMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCMagenta = m_Box.TBCMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCOrange > 0)
                        {

                            if (m_Box.TBCOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCOrange = m_Box.TBCOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCPink > 0)
                        {

                            if (m_Box.TBCPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCPink = m_Box.TBCPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCPlain > 0)
                        {

                            if (m_Box.TBCPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCPlain = m_Box.TBCPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCPurple > 0)
                        {

                            if (m_Box.TBCPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCPurple = m_Box.TBCPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCRed > 0)
                        {

                            if (m_Box.TBCRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCRed = m_Box.TBCRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCWhite > 0)
                        {

                            if (m_Box.TBCWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCWhite = m_Box.TBCWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCYellow > 0)
                        {

                            if (m_Box.TBCYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCYellow = m_Box.TBCYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBBlue > 0)
                        {

                            if (m_Box.TBCBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBBlue = m_Box.TBCBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBGreen > 0)
                        {

                            if (m_Box.TBCBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBGreen = m_Box.TBCBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBPurple > 0)
                        {

                            if (m_Box.TBCBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBPurple = m_Box.TBCBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBOrange > 0)
                        {

                            if (m_Box.TBCBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBOrange = m_Box.TBCBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBRed > 0)
                        {

                            if (m_Box.TBCBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBRed = m_Box.TBCBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonTBCBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.TribarrelCactus;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 17;

                        if (m_Box.TBCBYellow > 0)
                        {

                            if (m_Box.TBCBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBYellow = m_Box.TBCBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.TBCBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.TBCBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Tribarrel Cactus seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Tribarrel Cactus seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterAqua:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Aqua;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterAqua > 0)
                        {

                            if (m_Box.WaterAqua > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterAqua = m_Box.WaterAqua - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterAqua > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterAqua = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1173, "An Aqua Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Aqua Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBlack:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Black;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBlack > 0)
                        {

                            if (m_Box.WaterBlack > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBlack = m_Box.WaterBlack - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBlack > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBlack = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1109, "A Black Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Black Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Blue;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBlue > 0)
                        {

                            if (m_Box.WaterBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBlue = m_Box.WaterBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1341, "A Blue Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Blue Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterFRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.FireRed;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterFRed > 0)
                        {

                            if (m_Box.WaterFRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterFRed = m_Box.WaterFRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterFRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterFRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1161, "A Fire Red Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Fire Red Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Green;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterGreen > 0)
                        {

                            if (m_Box.WaterGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterGreen = m_Box.WaterGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1435, "A Green Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Green Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterMagenta:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Magenta;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterMagenta > 0)
                        {

                            if (m_Box.WaterMagenta > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterMagenta = m_Box.WaterMagenta - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterMagenta > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterMagenta = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1158, "A Magenta Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Magenta Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Orange;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterOrange > 0)
                        {

                            if (m_Box.WaterOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterOrange = m_Box.WaterOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1135, "An Orange Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Orange Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterPink:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Pink;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterPink > 0)
                        {

                            if (m_Box.WaterPink > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterPink = m_Box.WaterPink - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterPink > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterPink = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1166, "A Pink Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Pink Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterPlain:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Plain;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterPlain > 0)
                        {

                            if (m_Box.WaterPlain > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterPlain = m_Box.WaterPlain - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterPlain > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterPlain = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage("A Plain Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Plain Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Purple;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterPurple > 0)
                        {

                            if (m_Box.WaterPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterPurple = m_Box.WaterPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(13, "A Purple Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Purple Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Red;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterRed > 0)
                        {

                            if (m_Box.WaterRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterRed = m_Box.WaterRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1645, "A Red Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Red Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterWhite:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.White;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterWhite > 0)
                        {

                            if (m_Box.WaterWhite > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterWhite = m_Box.WaterWhite - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterWhite > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterWhite = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(1153, "A White Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more White Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.Yellow;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterYellow > 0)
                        {

                            if (m_Box.WaterYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterYellow = m_Box.WaterYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(2213, "A Yellow Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Yellow Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBBlue:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.BrightBlue;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBBlue > 0)
                        {

                            if (m_Box.WaterBBlue > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBBlue = m_Box.WaterBBlue - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBBlue > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBBlue = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(5, "A Bright Blue Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Blue Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBGreen:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.BrightGreen;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBGreen > 0)
                        {

                            if (m_Box.WaterBGreen > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBGreen = m_Box.WaterBGreen - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBGreen > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBGreen = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(66, "A Bright Green Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Green Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBPurple:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.BrightPurple;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBPurple > 0)
                        {

                            if (m_Box.WaterBPurple > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBPurple = m_Box.WaterBPurple - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBPurple > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBPurple = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(16, "A Bright Purple Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Purple Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBOrange:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.BrightOrange;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBOrange > 0)
                        {

                            if (m_Box.WaterBOrange > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBOrange = m_Box.WaterBOrange - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBOrange > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBOrange = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(43, "A Bright Orange Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Orange Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBRed:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.BrightRed;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBRed > 0)
                        {

                            if (m_Box.WaterBRed > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBRed = m_Box.WaterBRed - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBRed > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBRed = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(33, "A Bright Red Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Red Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }
                case (int)Buttons.ButtonWaterBYellow:
                    {
                        Seed seed = new Seed();
                        seed.PlantType = PlantType.WaterPlant;
                        seed.PlantHue = PlantHue.BrightYellow;
                        seed.ShowType = true;
                        int page = 18;

                        if (m_Box.WaterBYellow > 0)
                        {

                            if (m_Box.WaterBYellow > 1)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBYellow = m_Box.WaterBYellow - 1;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Water Plant seed was added to your backpack");
                                break;
                            }
                            else if (m_Box.WaterBYellow > 0)
                            {
                                m_From.AddToBackpack(seed);
                                m_Box.WaterBYellow = 0;
                                m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                                m_From.SendMessage(56, "A Bright Yellow Water Plant seed was added to your backpack");
                                break;
                            }
                        }
                        m_From.SendMessage("You have no more Bright Yellow Water Plant seeds.");
                        m_From.SendGump(new SeedCompanionGump(m_From, m_Box, page));
                        break;
                    }

            }
        }
        #endregion

	}

}

namespace Server.Items
{
	public class SeedCompanionTarget : Target
	{
		private SeedCompanion m_Box;

		public SeedCompanionTarget( SeedCompanion box ) : base( 18, false, TargetFlags.None )
		{
			m_Box = box;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( m_Box.Deleted )
			return;

			m_Box.EndCombine( from, targeted );
		}
	}
}
