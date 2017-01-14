using System;
using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
    public class JaacarBox : WoodenBox
    {
        [Constructable]
        public JaacarBox()
            : base()
        {
            this.Movable = true;
            this.Hue = 1266;

            this.DropItem(new RecipeScroll(500));
        }

        public JaacarBox(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Jaacar Reward Box";
            }
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