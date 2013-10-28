using System;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public abstract class QuestItem : Item
    {
        public QuestItem(int itemID)
            : base(itemID)
        {
        }

        public QuestItem(Serial serial)
            : base(serial)
        {
        }

        public virtual bool Accepted
        {
            get
            {
                return this.Deleted;
            }
        }
        public abstract bool CanDrop(PlayerMobile pm);

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool ret = base.DropToWorld(from, p);

            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.IsStaff())
                {
                    return true;
                }
                else if (!(from is PlayerMobile) || this.CanDrop((PlayerMobile)from))
                {
                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(1049343); // You can only drop quest items into the top-most level of your backpack while you still need them for your quest.
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            bool ret = base.DropToMobile(from, target, p);

            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.IsStaff())
                {
                    return true;
                }
                else if (!(from is PlayerMobile) || this.CanDrop((PlayerMobile)from))
                {
                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(1049344); // You decide against trading the item.  You still need it for your quest.
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool ret = base.DropToItem(from, target, p);

            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.IsStaff())
                {
                    return true;
                }
                else if (!(from is PlayerMobile) || this.CanDrop((PlayerMobile)from))
                {
                    return true;
                }
                else
                {
                    from.SendLocalizedMessage(1049343); // You can only drop quest items into the top-most level of your backpack while you still need them for your quest.
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            if (parent is PlayerMobile && !this.CanDrop((PlayerMobile)parent))
                return DeathMoveResult.MoveToBackpack;
            else
                return base.OnParentDeath(parent);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}