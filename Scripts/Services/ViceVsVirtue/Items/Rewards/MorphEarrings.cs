using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Factions;
using Server.Engines.VvV;

namespace Server.Items
{
    [TypeAlias("Server.Engines.VvV.MorphEarrings")]
    public class MorphEarrings : GoldEarrings
	{
        public override int LabelNumber
        {
            get
            {
                return 1094746; // Morph Earrings
            }
        }

        [Constructable]
        public MorphEarrings()
        {
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
            {
                ValidateEquipment((Mobile)parent);
            }
        }

        private void ValidateEquipment(Mobile m)
        {
            if (m == null)
                return;

            Race race = m.Race;
            bool didDrop = false;

            List<Item> list = new List<Item>(m.Items);

            foreach (Item item in list)
            {
                bool drop = false;

                if (item is BaseArmor && ((BaseArmor)item).RequiredRace != null && ((BaseArmor)item).RequiredRace != race)
                    drop = true;
                else if (item is BaseWeapon && ((BaseWeapon)item).RequiredRace != null && ((BaseWeapon)item).RequiredRace != race)
                    drop = true;
                else if (item is BaseJewel && ((BaseJewel)item).RequiredRace != null && ((BaseJewel)item).RequiredRace != race)
                    drop = true;
                else if (item is BaseClothing && ((BaseClothing)item).RequiredRace != null && ((BaseClothing)item).RequiredRace != race)
                    drop = true;

                if (drop)
                {
                    if (!didDrop)
                        didDrop = true;

                    if (m.Backpack == null || !m.Backpack.TryDropItem(m, item, false))
                    {
                        m.BankBox.DropItem(item);
                    }
                }
            }

            ColUtility.Free(list);

            if (didDrop)
                m.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
        }

        public MorphEarrings(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(1);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

            if (version == 0 && ViceVsVirtueSystem.Enabled)
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
		}
	}
}