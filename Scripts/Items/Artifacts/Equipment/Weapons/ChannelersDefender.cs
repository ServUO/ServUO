namespace Server.Items
{
    public class ChannelersDefender : GlassSword
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113518;  // Channeler's Defender

        [Constructable]
        public ChannelersDefender()
        {
            Hue = 95;
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 5;
            Attributes.LowerManaCost = 5;
            Attributes.WeaponSpeed = 20;
            Attributes.CastRecovery = 1;
            Attributes.SpellChanneling = 1;
            WeaponAttributes.HitLowerAttack = 60;
            AosElementDamages.Energy = 100;
        }

        public ChannelersDefender(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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