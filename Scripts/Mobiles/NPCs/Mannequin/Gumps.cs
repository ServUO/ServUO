using System;
using Server.Mobiles;
using System.Linq;
using System.Collections.Generic;
using Server.Network;

namespace Server.Gumps
{
    public class MannequinCompareGump : Gump
    {
        private readonly Item _SameItem;

        public MannequinCompareGump(Mannequin mann, Item item)
            : base(100, 100)
        {
            _SameItem = mann.Items.Where(x => mann.LayerValidation(x, item)).FirstOrDefault();

            if (_SameItem == null)
                return;

            AddPage(0);

            AddBackground(0, 0, 560, 720, 0x6DB);
            AddImageTiled(270, 5, 6, 710, 0x6DE);
            AddHtmlLocalized(5, 5, 270, 18, 1114513, "#1159291", 0x42FF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>            
            AddAlphaRegion(5, 5, 550, 710);

            AddHtmlLocalized(0, 23, 270, 18, 1114513, string.Format("#{0}", _SameItem.LabelNumber), 0x7FFF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddImageTiled(4, 80, 550, 5, 0x6DC);

            List<ValuedProperty> EquipmentItem = Mannequin.FindItemProperty(_SameItem);

            int si = 0;

            for (int i = 0; i < EquipmentItem.Count; i++)
            {
                if (EquipmentItem[i].LabelNumber == 1159280) // Medable Armor - not appear
                    continue;

                if (EquipmentItem[i].IsSpriteGraph)
                {
                    AddSpriteImage(5 + (35 * si), 41, 0x9D3B, EquipmentItem[i].SpriteW, EquipmentItem[i].SpriteH, 30, 30);
                    AddTooltip(EquipmentItem[i].LabelNumber);

                    si++;
                }
                else
                {
                    AddHtmlLocalized(45, 94 + (18 * i), 140, 18, EquipmentItem[i].LabelNumber, EquipmentItem[i].Hue, false, false);
                    AddTooltip(EquipmentItem[i].Description);

                    if (!EquipmentItem[i].IsBoolen)
                        AddHtml(190, 94 + (18 * i), 100, 18, Color(EquipmentItem[i].Value, EquipmentItem[i].Cap), false, false);
                }
            }

            AddHtmlLocalized(270, 23, 270, 18, 1114513, string.Format("#{0}", item.LabelNumber), 0x7FFF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            List<ValuedProperty> SelectItem = Mannequin.FindItemProperty(item);

            si = 0;

            for (int i = 0; i < SelectItem.Count; i++)
            {
                if (SelectItem[i].LabelNumber == 1159280) // Medable Armor - not appear
                    continue;

                if (SelectItem[i].IsSpriteGraph)
                {
                    AddSpriteImage(275 + (35 * si), 41, 0x9D3B, SelectItem[i].SpriteW, SelectItem[i].SpriteH, 30, 30);
                    AddTooltip(SelectItem[i].LabelNumber);

                    si++;
                }
                else
                {
                    AddHtmlLocalized(315, 94 + (18 * i), 140, 18, SelectItem[i].LabelNumber, SelectItem[i].Hue, false, false);
                    AddTooltip(SelectItem[i].Description);

                    if (!SelectItem[i].IsBoolen)
                    {
                        double ev = 0;

                        if (EquipmentItem.Any(a => a.LabelNumber == SelectItem[i].LabelNumber))
                        {
                            ev = EquipmentItem.Find(a => a.LabelNumber == SelectItem[i].LabelNumber).Value;
                        }

                        AddHtml(460, 94 + (18 * i), 100, 18, Color(SelectItem[i].Value, SelectItem[i].Cap, ev), false, false);
                    }
                }
            }
        }

        public string Color(double v1, double v2, double ev = 0)
        {
            string name;

            double diffv = v1 - ev;

            if (v2 == 0)
            {
                if (v1 == diffv || diffv == 0)
                {
                    name = string.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", v1);
                }
                else if (v1 > ev)
                {
                    name = string.Format("<BASEFONT COLOR=#008000>{0} (+{1})</BASEFONT>", v1, diffv);
                }
                else
                {
                    name = string.Format("<BASEFONT COLOR=#800000>{0} ({1})</BASEFONT>", v1, diffv);
                }
            }
            else
            {
                if (v1 == diffv || diffv == 0)
                {
                    name = string.Format("<BASEFONT COLOR=#FFFFFF>{0}/{1}</BASEFONT>", v1, v2);
                }
                else if (v1 > ev)
                {
                    name = string.Format("<BASEFONT COLOR=#008000>{0}/{1} (+{2})</BASEFONT>", v1, v2, diffv);
                }
                else
                {
                    name = string.Format("<BASEFONT COLOR=#800000>{0}/{1} ({2})</BASEFONT>", v1, v2, diffv);
                }
            }

            return name;
        }
    }

    public class MannequinStatsGump : Gump
    {
        private readonly Mannequin _Mannequin;
        private readonly Item _SameItem;
        private readonly Item _Item;

        public LabelDefinition[] Page1 = new LabelDefinition[]
        {
            new LabelDefinition(1049593, Catalog.Attributes, 5),    // Attributes
            new LabelDefinition(1061645, Catalog.Resistances, 5),   // Resistances
            new LabelDefinition(1077417, Catalog.Combat1, 8),       // Combat
        };

        public LabelDefinition[] Page2 = new LabelDefinition[]
        {
            new LabelDefinition(1077417, Catalog.Combat2, 3),       // Combat
            new LabelDefinition(1076209, Catalog.Casting),          // Casting
            new LabelDefinition(1044050, Catalog.Misc),             // Miscellaneous
            new LabelDefinition(1114251, Catalog.HitEffects),       // Hit Effects
            new LabelDefinition(1155065, Catalog.SkillBonusGear),   // Skill Bonus Gear
        };

        public class ItemPropDefinition
        {
            public List<ValuedProperty> SelectItem { get; set; }
            public List<ValuedProperty> EquipmentItem { get; set; }

            public ItemPropDefinition(Item tl, Item ctlg)
            {
                SelectItem = Mannequin.GetProperty(tl);
                EquipmentItem = Mannequin.GetProperty(ctlg);
            }
        }

        public MannequinStatsGump(Mannequin mann)
            : this(mann, null)
        { }

        public MannequinStatsGump(Mannequin mann, Item item)
            : this(mann, item, 0)
        { }

        public MannequinStatsGump(Mannequin mann, Item item, int page)
            : base(100, 100)
        {
            _Mannequin = mann;
            _Item = item;

            AddPage(0);

            AddBackground(0, 0, 604, 820, 0x6DB);
            AddAlphaRegion(5, 5, 594, 810);
            AddHtmlLocalized(10, 10, 584, 18, 1114513, "#1159279", 0x43F7, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            List<ValuedProperty> lm = Mannequin.GetProperty(_Mannequin.Items);
            List<ItemPropDefinition> CompareItem = null;

            if (_Item != null)
            {
                _SameItem = _Mannequin.Items.Where(x => _Mannequin.LayerValidation(x, item)).FirstOrDefault();

                if (_SameItem != null)
                {
                    CompareItem = new List<ItemPropDefinition>
                    {
                        new ItemPropDefinition(_Item, _SameItem)
                    };

                    List<Item> list = _Mannequin.Items.Where(x => x != _SameItem).ToList();
                    list.Add(_Item);

                    lm = Mannequin.GetProperty(list);
                }
            }

            AddHtmlLocalized(479, 10, 75, 18, 1114514, "#1158215", 0x42FF, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>

            if (page > 0)
                AddButton(554, 10, 0x15E3, 0x15E7, 1000, GumpButtonType.Reply, 0); // Next Page Button
            else
                AddButton(554, 10, 0x15E1, 0x15E5, 1001, GumpButtonType.Reply, 0); // Previous Page Button

            int y = 0;

            LabelDefinition[] pages = page > 0 ? Page2 : Page1;

            pages.ToList().ForEach(x =>
            {
                y += 28;

                if (lm.Any(l => l.Catalog == x.Catalog))
                {
                    AddHtmlLocalized(10, y, 150, 18, x.TitleLabel, 0x6A05, false, false);
                    AddImageTiled(10, y + 18, 584, 5, 0x6DC);

                    y += 32;

                    List<ValuedProperty> cataloglist = lm.Where(i => i.Catalog == x.Catalog).OrderBy(ii => ii.Order).ToList();

                    int columncount = x.ColumnLeftCount == 0 ? (int)Math.Ceiling(cataloglist.Count / (decimal)2) : x.ColumnLeftCount;

                    for (int i = 0; i < cataloglist.Count; i++)
                    {
                        int label = cataloglist[i].LabelNumber;

                        if (i < columncount)
                        {
                            AddHtmlLocalized(45, y, 200, 18, label, 0x560A, false, false);
                            AddSpriteImage(10, y - 5, 0x9D3B, cataloglist[i].SpriteW, cataloglist[i].SpriteH, 30, 30);
                            AddTooltip(cataloglist[i].Description);

                            if (cataloglist[i].IsBoolen)
                            {
                                if (cataloglist[i].BoolenValue)
                                {
                                    if (cataloglist[i].Value > 0)
                                        AddHtmlLocalized(195, y, 100, 18, 1046362, 0x42FF, false, false); // Yes
                                    else
                                        AddHtmlLocalized(195, y, 100, 18, 1046363, 0x42FF, false, false); // No
                                }
                            }
                            else
                            {
                                AddHtml(195, y, 100, 18, Color(cataloglist[i].Value, GetItemValue(CompareItem, label), cataloglist[i].Cap), false, false);
                            }
                        }
                        else
                        {
                            if (i == columncount)
                                y -= i * 30;

                            AddHtmlLocalized(342, y, 200, 18, label, 0x560A, false, false);
                            AddSpriteImage(307, y - 5, 0x9D3B, cataloglist[i].SpriteW, cataloglist[i].SpriteH, 30, 30);
                            AddTooltip(cataloglist[i].Description);

                            if (!cataloglist[i].IsBoolen)
                                AddHtml(492, y, 100, 18, Color(cataloglist[i].Value, GetItemValue(CompareItem, label), cataloglist[i].Cap), false, false);
                        }

                        y += 30;
                    }

                    y += 3;
                }
            });
        }

        public double GetItemValue(List<ItemPropDefinition> list, int label)
        {
            if (list == null)
                return 0;

            var l = list.FirstOrDefault();

            double v1 = 0;
            double v2 = 0;

            if (l.EquipmentItem.Any(r => r.LabelNumber == label))
                v1 = l.EquipmentItem.Where(r => r.LabelNumber == label).FirstOrDefault().Value;

            if (l.SelectItem.Any(r => r.LabelNumber == label))
                v2 = l.SelectItem.Where(r => r.LabelNumber == label).FirstOrDefault().Value;

            return v2 - v1;
        }

        public string Color(double ev, double diff, int parmv)
        {
            string name;

            if (_SameItem == null || diff == 0)
            {
                if (parmv == 0)
                {
                    name = string.Format("<BASEFONT COLOR=#80BFFF>{0}</BASEFONT>", ev);
                }
                else
                {
                    name = string.Format("<BASEFONT COLOR=#80BFFF>{0}/{1}</BASEFONT>", ev, parmv);
                }
            }
            else if (diff < 0)
            {
                if (parmv == 0)
                {
                    name = string.Format("<BASEFONT COLOR=#800000>{0} (-{1})</BASEFONT>", ev, Math.Abs(diff));
                }
                else
                {
                    name = string.Format("<BASEFONT COLOR=#800000>{0}/{1} (-{2})</BASEFONT>", ev, parmv, Math.Abs(diff));
                }
            }
            else
            {
                if (parmv == 0)
                {
                    name = string.Format("<BASEFONT COLOR=#008000>{0} (+{1})</BASEFONT>", ev, diff);
                }
                else
                {
                    name = string.Format("<BASEFONT COLOR=#008000>{0}/{1} (+{2})</BASEFONT>", ev, parmv, diff);
                }
            }

            return name;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (_Mannequin == null)
                return;

            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: { break; }
                case 1000:
                    {
                        from.SendGump(new MannequinStatsGump(_Mannequin, _Item, 0));
                        break;
                    }
                case 1001:
                    {
                        from.SendGump(new MannequinStatsGump(_Mannequin, _Item, 1));
                        break;
                    }
            }
        }
    }
}
