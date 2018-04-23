using System;
using Server.Items;

namespace Server.Kiasta.Settings
{
    #region Miscellaneous Items Settings
    /*******************************************
     * **********Miscellaneous Items********** *
     *******************************************/

    public static partial class Misc
    {
        public static Container BlackDragoonBag = new Kiasta.Dragoon.BagOfBlackDragoonEquipment(1);
        public static Container BlueDragoonBag = new Kiasta.Dragoon.BagOfBlueDragoonEquipment(1);
        public static Container GreenDragoonBag = new Kiasta.Dragoon.BagOfGreenDragoonEquipment(1);
        public static Container RandomDragoonColorBag = new Kiasta.Dragoon.BagOfRandomColorDragoonEquipment(1);
        public static Container RedDragoonBag = new Kiasta.Dragoon.BagOfRedDragoonEquipment(1);
        public static Container WhiteDragoonBag = new Kiasta.Dragoon.BagOfWhiteDragoonEquipment(1);
        public static Container YellowDragoonBag = new Kiasta.Dragoon.BagOfYellowDragoonEquipment(1);
        public static Container ItemStatDeedsBag = new Kiasta.Deeds.BagOfItemStatDeeds(1);
        public static Container SlayerDeedsBag = new Kiasta.Deeds.BagOfSlayerDeeds(1);
        public static LootType BagLootType = LootType.Cursed;
    }
    #endregion
}
