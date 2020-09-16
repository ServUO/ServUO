using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x1950, 0x1953)]
    public class MongbatDartboard : Item
    {
        public override int LabelNumber => 1075498;  // Mongbat Dartboard

        [Constructable]
        public MongbatDartboard()
            : base(0x1950)
        {
            LootType = LootType.Blessed;
            Weight = 8.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (!IsLockedDown)
            {
                from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.
                return;
            }

            BaseKnife knife = from.Weapon as BaseKnife;

            if (knife == null)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500751); // Try holding a knife...
                return;
            }

            from.Animate(AnimationType.Attack, 4);
            Point3D loc = new Point3D(from.X + 1, from.Y, from.Z + 103);
            Effects.SendPacket(from.Location, from.Map, new GraphicalEffect(EffectType.Moving, from.Serial, Serial.Zero, knife.ItemID, from.Location, loc, 10, 0, true, true));
            
            double rand = Utility.RandomDouble();

            int message;
            if (rand < 0.05)
                message = 500752; // BULLSEYE! 50 Points!
            else if (rand < 0.20)
                message = 500753; // Just missed the center! 20 points.
            else if (rand < 0.45)
                message = 500754; // 10 point shot.
            else if (rand < 0.70)
                message = 500755; // 5 pointer.
            else if (rand < 0.85)
                message = 500756; // 1 point.  Bad throw.
            else
                message = 500757; // Missed.

            PublicOverheadMessage(MessageType.Regular, 0x3B2, message);

            if (message != 500757)
            {
                AnimateMongbat();
            }
        }

        private void AnimateMongbat()
        {
            if (Deleted)
                return;

            if (ItemID == 0x1950 || ItemID == 0x1953)
            {
                ++ItemID;
                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(AnimateMongbat));
            }
            else if (ItemID == 0x1951 || ItemID == 0x1954)
            {
                --ItemID;
            }
        }

        public MongbatDartboard(Serial serial)
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
            reader.ReadInt();
        }
    }
}
