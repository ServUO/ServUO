using Server.ContextMenus;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
    public class FishBowl : BaseContainer
    {
        [Constructable]
        public FishBowl()
            : base(0x241C)
        {
            Hue = 0x47E;
            MaxItems = 1;
        }

        public FishBowl(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074499;// A fish bowl
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Empty => (Items.Count == 0);
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseFish Fish
        {
            get
            {
                if (Empty)
                    return null;

                if (Items[0] is BaseFish)
                    return (BaseFish)Items[0];

                return null;
            }
        }
        public override double DefaultWeight => 2.0;
        public override void OnDoubleClick(Mobile from)
        {
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (!CheckHold(from, dropped, sendFullMessage, true))
                return false;

            DropItem(dropped);
            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!IsAccessibleTo(from))
            {
                from.SendLocalizedMessage(502436); // That is not accessible.
                return false;
            }

            if (!(dropped is BaseFish))
            {
                from.SendLocalizedMessage(1074836); // The container can not hold that type of object.
                return false;
            }

            if (base.OnDragDrop(from, dropped))
            {
                ((BaseFish)dropped).StopTimer();
                InvalidateProperties();

                return true;
            }

            return false;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this)
                return false;

            return base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (item != this)
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (!Empty)
            {
                BaseFish fish = Fish;

                if (fish != null)
                    list.Add(1074494, "#{0}", fish.LabelNumber); // Contains: ~1_CREATURE~
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (!Empty && IsAccessibleTo(from))
                list.Add(new RemoveCreature(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                Weight = DefaultWeight;
        }

        private class RemoveCreature : ContextMenuEntry
        {
            private readonly FishBowl m_Bowl;
            public RemoveCreature(FishBowl bowl)
                : base(6242, 3)// Remove creature
            {
                m_Bowl = bowl;
            }

            public override void OnClick()
            {
                if (m_Bowl == null || m_Bowl.Deleted || !m_Bowl.IsAccessibleTo(Owner.From))
                    return;

                BaseFish fish = m_Bowl.Fish;

                if (fish != null)
                {
                    if (fish.IsLockedDown) // for legacy fish bowls
                    {
                        Owner.From.SendLocalizedMessage(1010449); // You may not use this object while it is locked down.
                    }
                    else if (!Owner.From.PlaceInBackpack(fish))
                    {
                        Owner.From.SendLocalizedMessage(1074496); // There is no room in your pack for the creature.
                    }
                    else
                    {
                        Owner.From.SendLocalizedMessage(1074495); // The creature has been removed from the fish bowl.
                        fish.StartTimer();
                        m_Bowl.InvalidateProperties();
                    }
                }
            }
        }
    }
}