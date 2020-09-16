using Server.Commands;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public class ItemPropertiesGump : BaseGump
    {
        public static void Initialize()
        {
            CommandSystem.Register("ItemProps", AccessLevel.GameMaster, e =>
            {
                SendGump(new ItemPropertiesGump((PlayerMobile)e.Mobile));
            });
        }

        public enum PropFilter
        {
            All,
            AosAttributes,
            AosWeaponAttributes,
            AosArmorAttributes,
            Slayer,
            AosElemental,
            SkillName,
            SAAbsorption,
            ExtendedWeapon,
            Other,
        }

        public PropFilter Filter { get; set; }
        public ItemType TypeFilter { get; set; }
        public List<ItemPropertyInfo> Infos { get; set; }

        public ItemPropertiesGump(PlayerMobile pm)
            : base(pm, 0, 0)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 900, 600, 0x2454);
            AddPage(0);

            AddHtml(0, 5, 900, 20, string.Format("Item Properties: {0}", Filter.ToString()), false, false);
            AddHtml(275, 15, 625, 20, Center("Item Description: (Imbuing/Runic Cap) - (Loot Cap) [Scale]"), false, false);

            AddButton(5, 550, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtml(40, 550, 200, 20, string.Format("Filter: {0}", Filter.ToString()), false, false);

            //
            AddButton(105, 575, TypeFilter == ItemType.Melee ? 4006 : 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtml(140, 575, 200, 20, "Melee", false, false);

            AddButton(205, 575, TypeFilter == ItemType.Ranged ? 4006 : 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtml(240, 575, 200, 20, "Ranged", false, false);

            AddButton(305, 575, TypeFilter == ItemType.Armor ? 4006 : 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtml(340, 575, 200, 20, "Armor", false, false);

            AddButton(405, 575, TypeFilter == ItemType.Shield ? 4006 : 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddHtml(440, 575, 200, 20, "Shields", false, false);

            AddButton(505, 575, TypeFilter == ItemType.Hat ? 4006 : 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddHtml(540, 575, 200, 20, "Hats", false, false);

            AddButton(605, 575, TypeFilter == ItemType.Jewel ? 4006 : 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtml(640, 575, 200, 20, "Jewels", false, false);

            AddLabel(5, 30, 0, "Property");
            AddLabel(125, 30, 0, "Max");
            AddLabel(175, 30, 0, "Start");
            AddLabel(225, 30, 0, "Scale");
            AddLabel(275, 30, 0, "Melee");
            AddLabel(375, 30, 0, "Ranged");
            AddLabel(475, 30, 0, "Armor");
            AddLabel(575, 30, 0, "Shields");
            AddLabel(675, 30, 0, "Hats");
            AddLabel(775, 30, 0, "Jewels");

            Infos = CompileList().ToList();
            int index = 0;
            int page = 1;
            int y = 50;
            int perPage = 25;

            AddPage(page);

            for (int i = 0; i < Infos.Count; i++)
            {
                ItemPropertyInfo info = Infos[i];
                int scale = info.Scale;

                //AddLabel(5, y, 0, info.Attribute.ToString());
                AddHtmlLocalized(5, y, 120, 20, 1114057, info.AttributeName.ToString(), 0x1, false, false);
                AddLabel(125, y, 0, info.MaxIntensity.ToString());
                AddLabel(175, y, 0, info.Start.ToString());
                AddLabel(225, y, 0, scale.ToString());

                LoadTypeInfo(ItemType.Melee, info, scale, 275, y);
                LoadTypeInfo(ItemType.Ranged, info, scale, 375, y);
                LoadTypeInfo(ItemType.Armor, info, scale, 475, y);
                LoadTypeInfo(ItemType.Shield, info, scale, 575, y);
                LoadTypeInfo(ItemType.Hat, info, scale, 675, y);
                LoadTypeInfo(ItemType.Jewel, info, scale, 775, y);

                AddButton(868, y, 4011, 4012, 10 + i, GumpButtonType.Reply, 0);

                if (++index % perPage == 0)
                {
                    y = 50;
                    AddButton(868, 0, 4005, 4007, 0, GumpButtonType.Page, page + 1);
                    AddPage(++page);
                    AddButton(838, 0, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                }
                else
                {
                    y += 20;
                }
            }
        }

        private void LoadTypeInfo(ItemType type, ItemPropertyInfo info, int scale, int x, int y)
        {
            PropInfo typeInfo = info.GetItemTypeInfo(type);

            if (typeInfo != null)
            {
                AddLabel(x, y, TypeFilter == type ? 0x9E : 0, string.Format("{0}-{1}[{2}]", typeInfo.StandardMax, typeInfo.LootMax, typeInfo.Scale > 1 ? typeInfo.Scale.ToString() : scale.ToString()));
            }
            else
            {
                AddLabel(x, y, 0, "N/A");
            }
        }

        private IEnumerable<ItemPropertyInfo> CompileList()
        {
            if (TypeFilter > ItemType.Invalid)
            {
                foreach (int i in ItemPropertyInfo.LootTable[TypeFilter])
                {
                    yield return ItemPropertyInfo.GetInfo(i);
                }

                yield break;
            }

            foreach (ItemPropertyInfo info in ItemPropertyInfo.Table.Values)
            {
                switch (Filter)
                {
                    case PropFilter.All:
                        yield return info;
                        break;
                    case PropFilter.AosAttributes:
                        if (info.Attribute is AosAttribute)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.AosWeaponAttributes:
                        if (info.Attribute is AosWeaponAttribute)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.AosArmorAttributes:
                        if (info.Attribute is AosArmorAttribute)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.Slayer:
                        if (info.Attribute is SlayerName)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.AosElemental:
                        if (info.Attribute is AosElementAttribute)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.SkillName:
                        if (info.Attribute is SkillName)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.SAAbsorption:
                        if (info.Attribute is SAAbsorptionAttribute)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.ExtendedWeapon:
                        if (info.Attribute is ExtendedWeaponAttribute)
                        {
                            yield return info;
                        }
                        break;
                    case PropFilter.Other:
                        if (info.Attribute is string)
                        {
                            yield return info;
                        }
                        break;
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                return;
            }
            else if (info.ButtonID == 1)
            {
                if (Filter == PropFilter.Other)
                {
                    Filter = PropFilter.All;
                }
                else
                {
                    Filter++;
                }

                TypeFilter = ItemType.Invalid;
                Refresh();
            }
            else if (info.ButtonID < 10)
            {
                switch (info.ButtonID)
                {
                    case 2:
                        TypeFilter = ItemType.Melee;
                        break;
                    case 3:
                        TypeFilter = ItemType.Ranged;
                        break;
                    case 4:
                        TypeFilter = ItemType.Armor;
                        break;
                    case 5:
                        TypeFilter = ItemType.Shield;
                        break;
                    case 6:
                        TypeFilter = ItemType.Hat;
                        break;
                    case 7:
                        TypeFilter = ItemType.Jewel;
                        break;
                }

                Refresh();
            }
            else
            {
                Refresh();

                int id = info.ButtonID - 10;

                if (id >= 0 && id < Infos.Count)
                {
                    ItemPropertyInfo propInfo = Infos[id];

                    SendGump(new InfoSpecificGump(User, propInfo, TypeFilter));
                }
            }
        }
    }

    public class InfoSpecificGump : BaseGump
    {
        public ItemPropertyInfo Info { get; set; }
        public ItemType ItemType { get; set; }

        public InfoSpecificGump(PlayerMobile pm, ItemPropertyInfo info, ItemType type)
            : base(pm, 100, 100)
        {
            Info = info;
            ItemType = type;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 600, 400, 0x2454);
            AddHtmlLocalized(0, 10, 600, 20, CenterLoc, Info.AttributeName.ToString(), 0x1, false, false);

            AddLabel(5, 30, 0, "Weight:");
            AddLabel(5, 50, 0, "Primary Resource:");
            AddLabel(5, 70, 0, "Gem Resource:");
            AddLabel(5, 90, 0, "Special Resource:");
            AddLabel(5, 110, 0, "Default Scale:");
            AddLabel(5, 130, 0, "Start Intensity:");
            AddLabel(5, 150, 0, "Default Max Intensity:");

            AddLabel(225, 30, 0, Info.Weight.ToString());
            AddHtmlLocalized(225, 50, 250, 20, 1114057, Info.PrimaryName != null ? Info.PrimaryName.ToString() : "N/A", 0x1, false, false);
            AddHtmlLocalized(225, 70, 250, 20, 1114057, Info.GemName != null ? Info.GemName.ToString() : "N/A", 0x1, false, false);
            AddHtmlLocalized(225, 90, 250, 20, 1114057, Info.SpecialName != null ? Info.SpecialName.ToString() : "N/A", 0x1, false, false);
            AddLabel(225, 110, 0, Info.Scale.ToString());
            AddLabel(225, 130, 0, Info.Start.ToString());
            AddLabel(225, 150, 0, Info.MaxIntensity.ToString());

            AddLabel(5, 190, 0, "Item Type");
            AddLabel(125, 190, 0, "Scale");
            AddLabel(225, 190, 0, "Standard Max");
            AddLabel(325, 190, 0, "Loot Max");
            AddLabel(425, 190, 0, "Over Cap");

            int y = 210;

            foreach (object i in Enum.GetValues(typeof(ItemType)))
            {
                ItemType type = (ItemType)i;

                if (type == ItemType.Invalid || (ItemType != ItemType.Invalid && type != ItemType))
                {
                    continue;
                }

                AddLabel(5, y, 0, type.ToString());
                PropInfo typeInfo = Info.GetItemTypeInfo(type);

                if (typeInfo != null)
                {
                    AddLabel(125, y, 0, typeInfo.Scale.ToString());
                    AddLabel(225, y, 0, typeInfo.StandardMax.ToString());
                    AddLabel(325, y, 0, typeInfo.LootMax.ToString());

                    if (typeInfo.PowerfulLootRange != null)
                    {
                        string str = string.Empty;

                        for (int j = 0; j < typeInfo.PowerfulLootRange.Length; j++)
                        {
                            int v = typeInfo.PowerfulLootRange[j];

                            if (j == typeInfo.PowerfulLootRange.Length - 1)
                            {
                                str += string.Format(" {0}", v.ToString());
                            }
                            else
                            {
                                str += string.Format(" {0},", v.ToString());
                            }
                        }

                        AddLabel(425, y, 0, str);
                    }
                    else
                    {
                        AddLabel(425, y, 0, "N/A");
                    }
                }
                else
                {
                    AddLabel(125, y, 0, "No Info");
                }

                y += 20;
            }
        }
    }
}
