using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.WrathGrapes")]
    public class GrapesOfWrath : BaseMagicalFood, ICommodity
    {
        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        [Constructable]
        public GrapesOfWrath()
            : base(0x2FD7)
        {
            Weight = 1.0;
            Hue = 0x482;
            Stackable = true;
        }

        public GrapesOfWrath(Serial serial)
            : base(serial)
        {
        }

        public override MagicalFood FoodID => MagicalFood.GrapesOfWrath;
        public override TimeSpan Cooldown => TimeSpan.FromMinutes(2);
        public override TimeSpan Duration => TimeSpan.FromSeconds(20);
        public override int EatMessage => 1074847;// The grapes of wrath invigorate you for a short time, allowing you to deal extra damage.

        public override bool Eat(Mobile from)
        {
            if (base.Eat(from))
            {
                BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.GrapesOfWrath, 1032247, 1153762, Duration, from, "15\t35"));
                return true;
            }

            return false;
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
    }
}