using System;
using Server.Gumps;
using Server.Items;

namespace Server.ContextMenus
{
    public class LevelInfoEntry : ContextMenuEntry
    {
        private readonly Item m_Item;
        private readonly Mobile m_From;
        private readonly AttributeCategory m_Cat;
        public LevelInfoEntry(Mobile from, Item item, AttributeCategory cat)
            : base(98, 3)
        {
            this.m_From = from;
            this.m_Item = item;
            this.m_Cat = cat;
        }

        public override void OnClick()
        {
            this.Owner.From.CloseGump(typeof(ItemExperienceGump));
            this.Owner.From.SendGump(new ItemExperienceGump(this.m_From, this.m_Item, this.m_Cat)); 
        }
    }
}