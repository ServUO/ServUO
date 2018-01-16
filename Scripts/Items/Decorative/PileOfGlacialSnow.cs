using System;
using Server.Targeting;

namespace Server.Items
{
    public class PileOfGlacialSnow : Item
    {
        [Constructable]
        public PileOfGlacialSnow()
            : base(0x913)
        {
            this.Hue = 0x480;
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public PileOfGlacialSnow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070874;
            }
        }// a Pile of Glacial Snow
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                this.Weight = 1.0;
                this.LootType = LootType.Blessed;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            this.LabelTo(from, 1070880); // Winter 2004
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070880); // Winter 2004
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
            else if (from.Mounted)
                from.SendLocalizedMessage(1010097); // You cannot use this while mounted.

            else if (from.CanBeginAction(typeof(SnowPile)))
            {
                from.SendLocalizedMessage(1005575); // You carefully pack the snow into a ball...
                from.Target = new SnowTarget(from, this);
            }
            else
            {
                from.SendLocalizedMessage(1005574); // The snow is not ready to be packed yet.  Keep trying.
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(5.0))
            {
                this.m_From = from;
            }

            protected override void OnTick()
            {
                this.m_From.EndAction(typeof(SnowPile));
            }
        }

        private class SnowTarget : Target
        {
            private readonly Mobile m_Thrower;
            private readonly Item m_Snow;
            public SnowTarget(Mobile thrower, Item snow)
                : base(10, false, TargetFlags.None)
            {
                this.m_Thrower = thrower;
                this.m_Snow = snow;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target == from)
                {
                    from.SendLocalizedMessage(1005576); // You can't throw this at yourself.
                }
                else if (target is Mobile)
                {
                    Mobile targ = (Mobile)target;
                    Container pack = targ.Backpack;

                    if (pack != null && pack.FindItemByType(new Type[] { typeof(SnowPile), typeof(PileOfGlacialSnow) }) != null)
                    {
                        if (from.BeginAction(typeof(SnowPile)))
                        {
                            new InternalTimer(from).Start();

                            from.PlaySound(0x145);

                            from.Animate(9, 1, 1, true, false, 0);

                            targ.SendLocalizedMessage(1010572); // You have just been hit by a snowball!
                            from.SendLocalizedMessage(1010573); // You throw the snowball and hit the target!

                            Effects.SendMovingEffect(from, targ, 0x36E4, 7, 0, false, true, 0x47F, 0);
                        }
                        else
                        {
                            from.SendLocalizedMessage(1005574); // The snow is not ready to be packed yet.  Keep trying.
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1005577); // You can only throw a snowball at something that can throw one back.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1005577); // You can only throw a snowball at something that can throw one back.
                }
            }
        }
    }
}
