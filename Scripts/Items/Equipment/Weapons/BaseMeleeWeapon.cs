namespace Server.Items
{
    public abstract class BaseMeleeWeapon : BaseWeapon
    {
        public BaseMeleeWeapon(int itemID)
            : base(itemID)
        {
        }

        public BaseMeleeWeapon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}