using System;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseRewardTitleDeed : Item
    {
        public override int LabelNumber { get { return 1155604; } } // A Deed for a Reward Title
        public abstract TextDefinition Title { get; }

        public BaseRewardTitleDeed() : base(5360)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && Title != null && (Title.String != null || Title.Number > 0))
            {
                PlayerMobile pm = from as PlayerMobile;

                if (pm != null)
                {
                    if (Title.Number > 0)
                        pm.AddCollectionTitle(Title.Number);
                    else if (Title.String != null)
                        pm.AddCollectionTitle(Title.String);

                    pm.SendLocalizedMessage(1155605, Title.ToString());  //Thou hath been bestowed the title ~1_TITLE~!

                    Delete();
                }
            }
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