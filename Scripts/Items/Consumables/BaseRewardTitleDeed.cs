using System;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseRewardTitleDeed : Item
    {
        public override int LabelNumber { get { return 1155604; } } // A Deed for a Reward Title
        public abstract TextDefinition Title { get; }

        public BaseRewardTitleDeed()
            : base(5360)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (Title != null && (Title.String != null || Title.Number > 0))
                {
                    PlayerMobile pm = from as PlayerMobile;

                    if (pm != null)
                    {
                        if ((Title.Number > 0 && pm.AddRewardTitle(Title.Number)) ||
                             Title.String != null && pm.AddRewardTitle(Title.String))
                        {

                            pm.SendLocalizedMessage(1155605, Title.ToString());  //Thou hath been bestowed the title ~1_TITLE~!
                            Delete();
                        }
                        else
                            pm.SendLocalizedMessage(1073626); // You already have that title!
                    }
                }
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1114057, Title.ToString()); // ~1_NOTHING~
        }

        public BaseRewardTitleDeed(Serial serial)
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
            int v = reader.ReadInt();
        }
    }
}