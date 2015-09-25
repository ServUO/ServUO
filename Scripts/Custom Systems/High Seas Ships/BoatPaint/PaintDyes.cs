using System;
using Server.Targeting;
using Server.HuePickers;

namespace Server.Items
{
    public class PaintDyes : Item
    {
        [Constructable]
        public PaintDyes()
            : base(0xFA9)
        {
            Weight = 3.0;
        }

        public PaintDyes(Serial serial)
            : base(serial)
        {
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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Select the paint can to use the dyes on.");
            from.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(1, false, TargetFlags.None)
            {
            }

            private class InternalPicker : HuePicker
            {
                private PaintCan _can;

                public InternalPicker(PaintCan can)
                    : base(can.ItemID)
                {
                    _can = can;
                }

                public override void OnResponse(int hue)
                {
                    _can.DyedHue = hue;
                }
            }

            private static void SetCanHue(Mobile from, object state, int hue)
            {
                ((PaintCan)state).DyedHue = hue;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PaintCan)
                {
                    PaintCan can = (PaintCan)targeted;

                    if (can.Redyable)
                    {
                        if (can.CustomHuePicker == null)
                            from.SendHuePicker(new InternalPicker(can));
                        else
                            from.SendGump(new CustomHuePickerGump(from, can.CustomHuePicker, new CustomHuePickerCallback(SetCanHue), can));
                    }
                    else
                    {
                        from.SendMessage("That paint can may not be redyed.");
                    }
                }
                else
                {
                    from.SendMessage("Use this on a paint can.");
                }
            }
        }
    }
}