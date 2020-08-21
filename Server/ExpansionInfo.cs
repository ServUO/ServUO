#region References
using System;
#endregion

namespace Server
{
	public enum Expansion
	{
		None = 0,
		T2A,
		UOR,
		UOTD,
		LBR,
		AOS,
		SE,
		ML,
		SA,
		HS,
		TOL,
		EJ
	}

	[Flags]
	public enum ClientFlags
	{
		None = 0x00000000,
		Felucca = 0x00000001,
		Trammel = 0x00000002,
		Ilshenar = 0x00000004,
		Malas = 0x00000008,
		Tokuno = 0x00000010,
		TerMur = 0x00000020,
		Unk1 = 0x00000040,
		Unk2 = 0x00000080,
		UOTD = 0x00000100
	}

	[Flags]
	public enum FeatureFlags
	{
		None = 0x00000000,
		T2A = 0x00000001,
		UOR = 0x00000002,
		UOTD = 0x00000004,
		LBR = 0x00000008,
		AOS = 0x00000010,
		SixthCharacterSlot = 0x00000020,
		SE = 0x00000040,
		ML = 0x00000080,
		EigthAge = 0x00000100,
		NinthAge = 0x00000200, /* Crystal/Shadow Custom House Tiles */
		TenthAge = 0x00000400,
		IncreasedStorage = 0x00000800, /* Increased Housing/Bank Storage */
		SeventhCharacterSlot = 0x00001000,
		RoleplayFaces = 0x00002000,
		TrialAccount = 0x00004000,
		LiveAccount = 0x00008000,
		SA = 0x00010000,
		HS = 0x00020000,
		Gothic = 0x00040000,
		Rustic = 0x00080000,
		Jungle = 0x00100000,
		Shadowguard = 0x00200000,
		TOL = 0x00400000,
		EJ = 0x00800000, // TODO: Verify value

		ExpansionNone = None,
		ExpansionT2A = T2A,
		ExpansionUOR = ExpansionT2A | UOR,
		ExpansionUOTD = ExpansionUOR | UOTD,
		ExpansionLBR = ExpansionUOTD | LBR,
		ExpansionAOS = ExpansionLBR | AOS | LiveAccount,
		ExpansionSE = ExpansionAOS | SE,
		ExpansionML = ExpansionSE | ML | NinthAge,
		ExpansionSA = ExpansionML | SA | Gothic | Rustic,
		ExpansionHS = ExpansionSA | HS,
		ExpansionTOL = ExpansionHS | TOL | Jungle | Shadowguard,
		ExpansionEJ = ExpansionTOL | EJ
	}

	[Flags]
	public enum CharacterListFlags
	{
		None = 0x00000000,
		Unk1 = 0x00000001,
		OverwriteConfigButton = 0x00000002,
		OneCharacterSlot = 0x00000004,
		ContextMenus = 0x00000008,
		SlotLimit = 0x00000010,
		AOS = 0x00000020,
		SixthCharacterSlot = 0x00000040,
		SE = 0x00000080,
		ML = 0x00000100,
		Unk2 = 0x00000200,
		UO3DClientType = 0x00000400,
		KR = 0x00000600, // uo:kr support flags
		Unk3 = 0x00000800,
		SeventhCharacterSlot = 0x00001000,
		Unk4 = 0x00002000,
		NewMovementSystem = 0x00004000,
		NewFeluccaAreas = 0x00008000,

		ExpansionNone = ContextMenus, //
		ExpansionT2A = ContextMenus, //
		ExpansionUOR = ContextMenus, // None
		ExpansionUOTD = ContextMenus, //
		ExpansionLBR = ContextMenus, //
		ExpansionAOS = ContextMenus | AOS,
		ExpansionSE = ExpansionAOS | SE,
		ExpansionML = ExpansionSE | ML,
		ExpansionSA = ExpansionML,
		ExpansionHS = ExpansionSA,
		ExpansionTOL = ExpansionHS,
		ExpansionEJ = ExpansionTOL
	}

	[Flags]
	public enum HousingFlags
	{
		None = 0x0,
		AOS = 0x10,
		SE = 0x40,
		ML = 0x80,
		Crystal = 0x200,
		SA = 0x10000,
		HS = 0x20000,
		Gothic = 0x40000,
		Rustic = 0x80000,
		Jungle = 0x100000,
		Shadowguard = 0x200000,
		TOL = 0x400000,
		EJ = 0x800000, // TODO: Verify value

		HousingAOS = AOS,
		HousingSE = HousingAOS | SE,
		HousingML = HousingSE | ML | Crystal,
		HousingSA = HousingML | SA | Gothic | Rustic,
		HousingHS = HousingSA | HS,
		HousingTOL = HousingHS | TOL | Jungle | Shadowguard,
		HousingEJ = HousingTOL | EJ
	}

	public class ExpansionInfo
	{
		public static ExpansionInfo CoreExpansion => GetInfo(Core.Expansion);

		public static ExpansionInfo[] Table { get; private set; }

		static ExpansionInfo()
		{
			Table = new[]
			{
				new ExpansionInfo(
					0,
					"Endless Journey",
					new ClientVersion("7.0.61.0"),
					FeatureFlags.ExpansionEJ,
					CharacterListFlags.ExpansionEJ,
					HousingFlags.HousingEJ)
			};
		}

		public static FeatureFlags GetFeatures(Expansion ex)
		{
			ExpansionInfo info = GetInfo(ex);

			if (info != null)
			{
				return info.SupportedFeatures;
			}

			return FeatureFlags.ExpansionEJ;
		}

		public static ExpansionInfo GetInfo(Expansion ex)
		{
			return GetInfo((int)ex);
		}

		public static ExpansionInfo GetInfo(int ex)
		{
			int v = ex;

			if (v < 0 || v >= Table.Length)
			{
				v = 0;
			}

			return Table[v];
		}

		public int ID { get; private set; }
		public string Name { get; set; }

		public ClientFlags ClientFlags { get; set; }
		public FeatureFlags SupportedFeatures { get; set; }
		public CharacterListFlags CharacterListFlags { get; set; }
		public ClientVersion RequiredClient { get; set; }
		public HousingFlags CustomHousingFlag { get; set; }

		public ExpansionInfo(
			int id,
			string name,
			ClientFlags clientFlags,
			FeatureFlags supportedFeatures,
			CharacterListFlags charListFlags,
			HousingFlags customHousingFlag)
			: this(id, name, supportedFeatures, charListFlags, customHousingFlag)
		{
			ClientFlags = clientFlags;
		}

		public ExpansionInfo(
			int id,
			string name,
			ClientVersion requiredClient,
			FeatureFlags supportedFeatures,
			CharacterListFlags charListFlags,
			HousingFlags customHousingFlag)
			: this(id, name, supportedFeatures, charListFlags, customHousingFlag)
		{
			RequiredClient = requiredClient;
		}

		private ExpansionInfo(
			int id,
			string name,
			FeatureFlags supportedFeatures,
			CharacterListFlags charListFlags,
			HousingFlags customHousingFlag)
		{
			ID = id;
			Name = name;

			SupportedFeatures = supportedFeatures;
			CharacterListFlags = charListFlags;
			CustomHousingFlag = customHousingFlag;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
