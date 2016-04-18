using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ClockworkAssembly : Item
    {
        [Constructable]
        public ClockworkAssembly()
            : base(0x1EA8)
        {
            this.Weight = 5.0;
            this.Hue = 1102;
        }

        public ClockworkAssembly(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "clockwork assembly";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            double tinkerSkill = from.Skills[SkillName.Tinkering].Value;

            if (tinkerSkill < 60.0)
            {
                from.SendMessage("You must have at least 60.0 skill in tinkering to construct a golem.");
                return;
            }
            else if ((from.Followers + 4) > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                return;
            }

            double scalar;

            if (tinkerSkill >= 100.0)
                scalar = 1.0;
            else if (tinkerSkill >= 90.0)
                scalar = 0.9;
            else if (tinkerSkill >= 80.0)
                scalar = 0.8;
            else if (tinkerSkill >= 70.0)
                scalar = 0.7;
            else
                scalar = 0.6;

            Container pack = from.Backpack;

            if (pack == null)
                return;

            int res = pack.ConsumeTotal(
                new Type[]
                {
                    typeof(PowerCrystal),
                    typeof(IronIngot),
                    typeof(BronzeIngot),
                    typeof(Gears)
                },
                new int[]
                {
                    1,
                    50,
                    50,
                    5
                });

            switch ( res )
            {
                case 0:
                    {
                        from.SendMessage("You must have a power crystal to construct the golem.");
                        break;
                    }
                case 1:
                    {
                        from.SendMessage("You must have 50 iron ingots to construct the golem.");
                        break;
                    }
                case 2:
                    {
                        from.SendMessage("You must have 50 bronze ingots to construct the golem.");
                        break;
                    }
                case 3:
                    {
                        from.SendMessage("You must have 5 gears to construct the golem.");
                        break;
                    }
                default:
                    {
                        Golem g = new Golem(true, scalar);

                        if (g.SetControlMaster(from))
                        {
                            this.Delete();

                            g.MoveToWorld(from.Location, from.Map);
                            from.PlaySound(0x241);
                        }

                        break;
                    }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}