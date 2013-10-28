using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;

namespace Server.Items
{
    public class FishBowl : BaseContainer
    {
        [Constructable]
        public FishBowl()
            : base(0x241C)
        {
            this.Hue = 0x47E;
            this.MaxItems = 1;
        }

        public FishBowl(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074499;
            }
        }// A fish bowl
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Empty
        {
            get
            {
                return (this.Items.Count == 0);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseFish Fish
        {
            get
            {
                if (this.Empty)
                    return null;

                if (this.Items[0] is BaseFish)
                    return (BaseFish)this.Items[0];

                return null;
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 2.0;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (!this.CheckHold(from, dropped, sendFullMessage, true))
                return false;

            this.DropItem(dropped);
            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!this.IsAccessibleTo(from))
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
                this.InvalidateProperties();

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

            if (!this.Empty)
            {
                BaseFish fish = this.Fish;

                if (fish != null)
                    list.Add(1074494, "#{0}", fish.LabelNumber); // Contains: ~1_CREATURE~
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (!this.Empty && this.IsAccessibleTo(from))
                list.Add(new RemoveCreature(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                this.Weight = this.DefaultWeight;
        }

        private class RemoveCreature : ContextMenuEntry
        {
            private readonly FishBowl m_Bowl;
            public RemoveCreature(FishBowl bowl)
                : base(6242, 3)// Remove creature
            {
                this.m_Bowl = bowl;
            }

            public override void OnClick()
            {
                if (this.m_Bowl == null || this.m_Bowl.Deleted || !this.m_Bowl.IsAccessibleTo(this.Owner.From))
                    return;

                BaseFish fish = this.m_Bowl.Fish;

                if (fish != null)
                {
                    if (fish.IsLockedDown) // for legacy fish bowls
                    {
                        this.Owner.From.SendLocalizedMessage(1010449); // You may not use this object while it is locked down.
                    }
                    else if (!this.Owner.From.PlaceInBackpack(fish))
                    {
                        this.Owner.From.SendLocalizedMessage(1074496); // There is no room in your pack for the creature.
                    }
                    else
                    {
                        this.Owner.From.SendLocalizedMessage(1074495); // The creature has been removed from the fish bowl.
                        fish.StartTimer();
                        this.m_Bowl.InvalidateProperties();
                    }
                }
            }
        }
    }
}