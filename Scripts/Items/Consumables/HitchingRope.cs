using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class HitchingRope : Item
    {
        [Constructable]
        public HitchingRope()
            : base(0x14F8)
        {
            Hue = 0x41F; // guessed

            Weight = 5;
        }

        public HitchingRope(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1071124;//  hitching rope
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || (from.InRange(GetWorldLocation(), 2) && Movable))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1071159); // Select the hitching post you want to supply hitching rope.
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly HitchingRope m_Rope;
            public InternalTarget(HitchingRope rope)
                : base(-1, false, TargetFlags.None)
            {
                m_Rope = rope;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Rope.Deleted)
                    return;

                if (!from.InRange(m_Rope.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
                else if (targeted is HitchingPost)
                {
                    HitchingPost postItem = (HitchingPost)targeted;

                    if (postItem.UsesRemaining >= 1)
                    {
                        from.SendMessage("Hitching Rope cannot be applied at this time.", 0x59);
                    }
                    else if (postItem.Replica && postItem.Charges <= 0 && postItem.UsesRemaining == 0)
                    {
                        from.SendLocalizedMessage(1071157); // This hitching post is damaged. You can't use it any longer.
                    }
                    else
                    {
                        postItem.Charges -= 1;
                        postItem.UsesRemaining += postItem.Replica ? 15 : 30;

                        m_Rope.Delete();

                        if (postItem is Item)
                        {
                            from.SendLocalizedMessage(1071158); // Supplied hitching rope.
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1062020); // That has no effect.
                }
            }
        }
    }
}