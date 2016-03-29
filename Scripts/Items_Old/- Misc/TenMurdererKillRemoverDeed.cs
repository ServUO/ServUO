using System;

namespace Server.Items
{
    public class TenMurdererKillRemoverDeed : Item
    {
        [Constructable]
        public TenMurdererKillRemoverDeed()
            : base(0x14F0)
        {
            this.Name = "10 Murderer Kill Remover Deed";
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
            this.Hue = 2067;
        }

        public TenMurdererKillRemoverDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                if (from.Kills >= 10)
                {
                    from.Kills -= 10;
                    this.Delete();
                    from.SendMessage("You remove 10 Kills from your Character !");
                }
                else
                {
                    from.SendMessage("You can use this deed only if you have 10 Kills on your Character !!");
                }
            }
        }
    }
}