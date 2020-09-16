namespace Server.Items
{
    [Flipable(0x48B0, 0x48B1)]
    public class GargishBattleAxe : BaseAxe
    {
        [Constructable]
        public GargishBattleAxe()
            : base(0x48B0)
        {
            Weight = 4.0;
            Layer = Layer.TwoHanded;
        }

        public GargishBattleAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;
        public override int StrengthReq => 35;
        public override int MinDamage => 16;
        public override int MaxDamage => 19;
        public override float Speed => 3.50f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

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
