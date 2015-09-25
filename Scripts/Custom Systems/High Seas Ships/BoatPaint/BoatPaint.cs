using System;
using Server;

namespace Server.Items
{
    public class BoatPaint : PaintCan, Engines.VeteranRewards.IRewardItem
    {
        public override bool AllowRepaint { get { return true; } }
        public override bool AllowHouse { get { return true; } }

        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.SpecialDyeTub; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        public BoatPaint()
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsRewardItem && !Engines.VeteranRewards.RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            base.OnDoubleClick(from);
        }

        public BoatPaint(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((bool)IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }
    }
}