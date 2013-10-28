using System;
using Server.Targeting;

namespace Server.Items
{
    [FlipableAttribute(0xF52, 0xF51)]
    public class ThrowingDagger : Item
    {
        [Constructable]
        public ThrowingDagger()
            : base(0xF52)
        {
            this.Weight = 1.0;
            this.Layer = Layer.OneHanded;
        }

        public ThrowingDagger(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a throwing dagger";
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

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Items.Contains(this))
            {
                InternalTarget t = new InternalTarget(this);
                from.Target = t;
            }
            else
            {
                from.SendMessage("You must be holding that weapon to use it.");
            }
        }

        private class InternalTarget : Target
        {
            private readonly ThrowingDagger m_Dagger;
            public InternalTarget(ThrowingDagger dagger)
                : base(10, false, TargetFlags.Harmful)
            {
                this.m_Dagger = dagger;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Dagger.Deleted)
                {
                    return;
                }
                else if (!from.Items.Contains(this.m_Dagger))
                {
                    from.SendMessage("You must be holding that weapon to use it.");
                }
                else if (targeted is Mobile)
                {
                    Mobile m = (Mobile)targeted;

                    if (m != from && from.HarmfulCheck(m))
                    {
                        Direction to = from.GetDirectionTo(m);

                        from.Direction = to;

                        from.Animate(from.Mounted ? 26 : 9, 7, 1, true, false, 0);

                        if (Utility.RandomDouble() >= (Math.Sqrt(m.Dex / 100.0) * 0.8))
                        {
                            from.MovingEffect(m, 0x1BFE, 7, 1, false, false, 0x481, 0);

                            AOS.Damage(m, from, Utility.Random(5, from.Str / 10), 100, 0, 0, 0, 0);

                            this.m_Dagger.MoveToWorld(m.Location, m.Map);
                        }
                        else
                        {
                            int x = 0, y = 0;

                            switch ( to & Direction.Mask )
                            {
                                case Direction.North:
                                    --y;
                                    break;
                                case Direction.South:
                                    ++y;
                                    break;
                                case Direction.West:
                                    --x;
                                    break;
                                case Direction.East:
                                    ++x;
                                    break;
                                case Direction.Up:
                                    --x;
                                    --y;
                                    break;
                                case Direction.Down:
                                    ++x;
                                    ++y;
                                    break;
                                case Direction.Left:
                                    --x;
                                    ++y;
                                    break;
                                case Direction.Right:
                                    ++x;
                                    --y;
                                    break;
                            }

                            x += Utility.Random(-1, 3);
                            y += Utility.Random(-1, 3);

                            x += m.X;
                            y += m.Y;

                            this.m_Dagger.MoveToWorld(new Point3D(x, y, m.Z), m.Map);

                            from.MovingEffect(this.m_Dagger, 0x1BFE, 7, 1, false, false, 0x481, 0);

                            from.SendMessage("You miss.");
                        }
                    }
                }
            }
        }
    }
}