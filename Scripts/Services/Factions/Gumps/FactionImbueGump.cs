using System;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Factions
{
    public class FactionImbueGump : FactionGump
    {
        private readonly Item m_Item;
        private readonly Mobile m_Mobile;
        private readonly Faction m_Faction;
        private readonly CraftSystem m_CraftSystem;
        private readonly ITool m_Tool;
        private readonly object m_Notice;
        private readonly int m_Quality;
        private readonly FactionItemDefinition m_Definition;
        public FactionImbueGump(int quality, Item item, Mobile from, CraftSystem craftSystem, ITool tool, object notice, int availableSilver, Faction faction, FactionItemDefinition def)
            : base(100, 200)
        { 
            this.m_Item = item;
            this.m_Mobile = from;
            this.m_Faction = faction;
            this.m_CraftSystem = craftSystem;
            this.m_Tool = tool;
            this.m_Notice = notice;
            this.m_Quality = quality;
            this.m_Definition = def;

            this.AddPage(0);

            this.AddBackground(0, 0, 320, 270, 5054);
            this.AddBackground(10, 10, 300, 250, 3000);

            this.AddHtmlLocalized(20, 20, 210, 25, 1011569, false, false); // Imbue with Faction properties?

            this.AddHtmlLocalized(20, 60, 170, 25, 1018302, false, false); // Item quality: 
            this.AddHtmlLocalized(175, 60, 100, 25, 1018305 - this.m_Quality, false, false); //	Exceptional, Average, Low

            this.AddHtmlLocalized(20, 80, 170, 25, 1011572, false, false); // Item Cost : 
            this.AddLabel(175, 80, 0x34, def.SilverCost.ToString("N0")); // NOTE: Added 'N0'

            this.AddHtmlLocalized(20, 100, 170, 25, 1011573, false, false); // Your Silver : 
            this.AddLabel(175, 100, 0x34, availableSilver.ToString("N0")); // NOTE: Added 'N0'

            this.AddRadio(20, 140, 210, 211, true, 1);
            this.AddLabel(55, 140, this.m_Faction.Definition.HuePrimary - 1, "*****");
            this.AddHtmlLocalized(150, 140, 150, 25, 1011570, false, false); // Primary Color

            this.AddRadio(20, 160, 210, 211, false, 2);
            this.AddLabel(55, 160, this.m_Faction.Definition.HueSecondary - 1, "*****");
            this.AddHtmlLocalized(150, 160, 150, 25, 1011571, false, false); // Secondary Color

            this.AddHtmlLocalized(55, 200, 200, 25, 1011011, false, false); // CONTINUE
            this.AddButton(20, 200, 4005, 4007, 1, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(55, 230, 200, 25, 1011012, false, false); // CANCEL
            this.AddButton(20, 230, 4005, 4007, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                Container pack = this.m_Mobile.Backpack;

                if (pack != null && this.m_Item.IsChildOf(pack))
                {
                    if (pack.ConsumeTotal(typeof(Silver), this.m_Definition.SilverCost))
                    {
                        int hue;

                        if (this.m_Item is SpellScroll)
                            hue = 0;
                        else if (info.IsSwitched(1))
                            hue = this.m_Faction.Definition.HuePrimary;
                        else
                            hue = this.m_Faction.Definition.HueSecondary;

                        FactionItem.Imbue(this.m_Item, this.m_Faction, true, hue);
                    }
                    else
                    {
                        this.m_Mobile.SendLocalizedMessage(1042204); // You do not have enough silver.
                    }
                }
            }

            if (this.m_Tool != null && !this.m_Tool.Deleted && this.m_Tool.UsesRemaining > 0)
                this.m_Mobile.SendGump(new CraftGump(this.m_Mobile, this.m_CraftSystem, this.m_Tool, this.m_Notice));
            else if (this.m_Notice is string)
                this.m_Mobile.SendMessage((string)this.m_Notice);
            else if (this.m_Notice is int && ((int)this.m_Notice) > 0)
                this.m_Mobile.SendLocalizedMessage((int)this.m_Notice);
        }
    }
}