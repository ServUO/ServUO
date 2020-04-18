using Server.Mobiles;
using System;

namespace Server.Items
{
    public class ClockworkAssembly : Item, ICommodity
    {
        public override int LabelNumber => 1073426;  // Clockwork Assembly

        [Constructable]
        public ClockworkAssembly()
            : base(0x1EA8)
        {
            Weight = 5.0;
            Hue = 1102;
        }

        public ClockworkAssembly(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (from.Skills[SkillName.Tinkering].Value < 60.0)
            {
                from.SendLocalizedMessage(1071943); // You must be a Journeyman or higher Tinker to construct a golem.
                return;
            }
            else if ((from.Followers + 4) > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                return;
            }

            Container pack = from.Backpack;

            if (pack == null)
                return;

            int res = pack.ConsumeTotal(new Type[] { typeof(PowerCrystal), typeof(IronIngot), typeof(BronzeIngot), typeof(Gears) }, new int[] { 1, 50, 50, 5 });

            switch (res)
            {
                case 0:
                    {
                        from.SendLocalizedMessage(1071945); // You need a power crystal to construct a golem.
                        break;
                    }
                case 1:
                    {
                        from.SendLocalizedMessage(1071948); // You need more iron ingots to construct a golem.
                        break;
                    }
                case 2:
                    {
                        from.SendLocalizedMessage(1071947); // You need more bronze ingots to construct a golem.
                        break;
                    }
                case 3:
                    {
                        from.SendLocalizedMessage(1071946); // You need more gears to construct a golem.
                        break;
                    }
                default:
                    {
                        Golem g = new Golem(true, Scalar(from));

                        if (g.SetControlMaster(from))
                        {
                            Delete();

                            g.MoveToWorld(from.Location, from.Map);
                            from.PlaySound(0x241);
                        }

                        break;
                    }
            }
        }

        public double Scalar(Mobile m)
        {
            double scalar;

            double skill = m.Skills[SkillName.Tinkering].Value;

            if (skill >= 100.0)
                scalar = 1.0;
            else if (skill >= 90.0)
                scalar = 0.9;
            else if (skill >= 80.0)
                scalar = 0.8;
            else if (skill >= 70.0)
                scalar = 0.7;
            else
                scalar = 0.6;

            return scalar;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
