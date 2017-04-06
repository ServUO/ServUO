using System;
using Server.Mobiles;

namespace Server.Items
{
    public class BODRewardTitleDeed : BaseRewardTitleDeed
    {
        public override int LabelNumber { get { return 1155604; } } // A Deed for a Reward Title
        public override TextDefinition Title { get { return _Title; } }

        private TextDefinition _Title;

        [Constructable]
        public BODRewardTitleDeed(int title)
        {
            _Title = new TextDefinition(1157181 + title);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1114057, Title.ToString()); // ~1_NOTHING~
        }

        public BODRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            TextDefinition.Serialize(writer, _Title);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            _Title = TextDefinition.Deserialize(reader);
        }
    }
}