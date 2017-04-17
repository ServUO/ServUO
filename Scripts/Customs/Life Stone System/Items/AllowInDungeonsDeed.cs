using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class AllowInDungeonTarget : Target
    {
        private AllowInDungeonDeed m_Deed;

        public AllowInDungeonTarget(AllowInDungeonDeed deed)
            : base(1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Deed.Deleted || m_Deed.RootParent != from)
                return;

            if (target is LifeStone)
            {
                LifeStone stone = target as LifeStone;

                if (stone.RootParent != from)
                {
                    from.SendMessage("You must be carrying the Life Stone.");
                }
                if (stone.AllowInDungeons)
                {
                    from.SendMessage("That stone already works in dungeons.");
                }
                else
                {
                    stone.AllowInDungeons = true;
                    from.SendMessage("That stone will now work in dungeons!");

                    m_Deed.Delete();
                }
            }
            else
            {
                from.SendMessage("That isn't a Life Stone!");
            }
        }
    }

    public class AllowInDungeonDeed : Item
    {
        public override string DefaultName
        {
            get { return "allow in dungeons deed for a life stone"; }
        }

        [Constructable]
        public AllowInDungeonDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
            Hue = 1461;
            LootType = LootType.Blessed;
        }

        public AllowInDungeonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                from.SendMessage("Target the Life Stone you wish to make work in dungeons.");
                from.Target = new AllowInDungeonTarget(this);
            }
        }
    }
}