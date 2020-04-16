using Server.Targets;

namespace Server.Items
{
    public abstract class BaseKnife : BaseMeleeWeapon
    {
        public BaseKnife(int itemID)
            : base(itemID)
        {
        }

        public BaseKnife(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x238;

        public override SkillName DefSkill => SkillName.Swords;

        public override WeaponType DefType => WeaponType.Slashing;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1010018); // What do you want to use this item on?
            from.Target = new BladedItemTarget(this);
        }
    }
}