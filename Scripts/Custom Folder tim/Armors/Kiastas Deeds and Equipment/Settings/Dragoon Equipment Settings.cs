using System;
using Server.Items;

namespace Server.Kiasta.Settings
{
    #region Dragoon Equipment Settings
    /*******************************************
     * **********Dragoon Equipment********** *
     *******************************************/

    public static class BaseEquipmentAttribute
    {
        public static ArmorMaterialType ArmorMaterialType = ArmorMaterialType.Dragon;
        public static CraftResource[] ResourceType = new CraftResource[]
			{
				CraftResource.BlackScales,
                CraftResource.BlueScales,
				CraftResource.GreenScales,
                CraftResource.RedScales,
				CraftResource.WhiteScales,
                CraftResource.YellowScales
			};
        public static CraftResource ClothingDefaultResource = CraftResource.BarbedLeather;
        public static int EquipmentArtifactRarity = 15;
    }

    public static class DragoonEquipmentName
    {
        public static string Suffix = "of the Dragoon";
    }

    public static partial class Misc
    {
        public static string ModifyAttributeMethodName = "ModifyAttribute"; // Modify this only if you know what you are doing
    }
    #endregion
}
