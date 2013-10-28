using System;
using Server.Items;

namespace Server.Mobiles
{
    public class PresetMapBuyInfo : GenericBuyInfo
    {
        private readonly PresetMapEntry m_Entry;
        public PresetMapBuyInfo(PresetMapEntry entry, int price, int amount)
            : base(entry.Name.ToString(), null, price, amount, 0x14EC, 0)
        {
            this.m_Entry = entry;
        }

        public override bool CanCacheDisplay
        {
            get
            {
                return false;
            }
        }
        public override IEntity GetEntity()
        {
            return new PresetMap(this.m_Entry);
        }
    }
}