using System;

namespace Server.Items
{
    public class TinctureOfSilver : Item
    {
        public override int LabelNumber => 1155619;  // Tincture of Silver

        [Constructable]
        public TinctureOfSilver()
            : base(0x183B)
        {
            Hue = 1900;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1155613); // Target the weapon, spellbook, or instrument you wish to apply this to...
                m.BeginTarget(-1, false, Targeting.TargetFlags.None, (from, targeted) =>
                {
                    if (targeted is Item && targeted is ISlayer)
                    {
                        Item item = (Item)targeted;
                        ISlayer slayer = (ISlayer)targeted;

                        SlayerSocket socket = item.GetSocket<SlayerSocket>();

                        if (socket == null || socket.Slayer != SlayerName.Silver)
                        {
                            if (slayer.Slayer != SlayerName.None && slayer.Slayer2 != SlayerName.None)
                            {
                                from.SendLocalizedMessage(1155680); // You cannot apply Tincture of Silver to items that are already slayers!
                            }
                            else
                            {
                                item.AttachSocket(new SlayerSocket(SlayerName.Silver, TimeSpan.FromHours(1)));

                                Delete();

                                from.SendLocalizedMessage(1155616); // You carefully apply Tincture of Silver to the item.  The effects will fade in one hour.
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(1155614); // This item is already treated with Tincture of Silver!
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1155615); // You cannot apply Tincture of Silver to this item.
                    }
                });
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1155620); // When Applied:
            list.Add(1155621, "#1155622"); // 200% Damage Increase towards Monsters of ~1_NAME~ : Undead Vulnerability
        }

        public TinctureOfSilver(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}