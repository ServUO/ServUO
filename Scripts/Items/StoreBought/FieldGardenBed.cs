using Server.Gumps;

namespace Server.Items
{
    public class FieldGardenBedAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FieldGardenBedDeed();

        [Constructable]
        public FieldGardenBedAddon(GardenBedDirection direction)
        {
            switch (direction)
            {
                case GardenBedDirection.Large:
                    {
                        AddComponent(new GardenAddonComponent(41746), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(41744), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(41745), -1, 1, 0);
                        AddComponent(new GardenAddonComponent(41743), -1, 0, 0);
                        AddComponent(new GardenAddonComponent(41749), 0, 1, 0);
                        AddComponent(new GardenAddonComponent(41748), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(41742), 1, -1, 0);
                        AddComponent(new GardenAddonComponent(41741), -1, -1, 0);
                        AddComponent(new GardenAddonComponent(41747), 0, -1, 0);

                        break;
                    }
                case GardenBedDirection.East:
                    {
                        AddComponent(new GardenAddonComponent(41746), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(41744), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(41745), 0, 1, 0);
                        AddComponent(new GardenAddonComponent(41743), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(41742), 1, -1, 0);
                        AddComponent(new GardenAddonComponent(41741), 0, -1, 0);

                        break;
                    }
                case GardenBedDirection.South:
                    {
                        AddComponent(new GardenAddonComponent(41746), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(41742), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(41749), 0, 1, 0);
                        AddComponent(new GardenAddonComponent(41747), 0, 0, 0);
                        AddComponent(new GardenAddonComponent(41745), -1, 1, 0);
                        AddComponent(new GardenAddonComponent(41741), -1, 0, 0);

                        break;
                    }
                case GardenBedDirection.Small:
                    {
                        AddComponent(new GardenAddonComponent(41746), 1, 1, 0);
                        AddComponent(new GardenAddonComponent(41745), 0, 1, 0);
                        AddComponent(new GardenAddonComponent(41742), 1, 0, 0);
                        AddComponent(new GardenAddonComponent(41741), 0, 0, 0);

                        break;
                    }
            }
        }

        public override void OnChop(Mobile from)
        {
            foreach (AddonComponent comp in Components)
            {
                if (comp is GardenAddonComponent && ((GardenAddonComponent)comp).Plant != null)
                {
                    from.SendLocalizedMessage(1150383); // You need to remove all plants through the plant menu before destroying this.
                    return;
                }
            }

            base.OnChop(from);
        }

        public FieldGardenBedAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FieldGardenBedDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1159056; // Field Garden Bed

        public override BaseAddon Addon => new FieldGardenBedAddon(m_Direction);
        public GardenBedDirection m_Direction;

        [Constructable]
        public FieldGardenBedDeed()
        {
            LootType = LootType.Blessed;
        }

        public FieldGardenBedDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, 1150381); // Garden Bed (South) 
            list.Add(2, 1150382); // Garden Bed (East)
            list.Add(3, 1150620); // Garden Bed (Large)
            list.Add(4, 1150621); // Garden Bed (Small)
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_Direction = (GardenBedDirection)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076170)); // Choose Direction
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
