using System;
using Server.Gumps;

namespace Server.Items
{
    public enum RugType
    {
        EastLarge,
        SouthLarge,
        EastSmall,
        SouthSmall
    }

    public abstract class BaseRugAddonDeed : BaseAddonDeed
    {
        public abstract override BaseAddon Addon { get; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUse { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public RugType RugType { get; set; }

        [Constructable]
        public BaseRugAddonDeed(DateTime nextuse, RugType type)
        {
            NextUse = nextuse;
            RugType = type;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RugGump));
                from.SendGump(new RugGump(from, this));
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public BaseRugAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1080457); // 10th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(NextUse);
            writer.Write((int)RugType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    NextUse = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 0:
                    NextUse = reader.ReadDateTime();
                    break;
            }
        }
    }
}
