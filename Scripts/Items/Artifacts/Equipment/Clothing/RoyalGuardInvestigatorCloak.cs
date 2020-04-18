namespace Server.Items
{
    public class RoyalGuardInvestigatorsCloak : Cloak
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalGuardInvestigatorsCloak()
            : base()
        {
            Hue = 1163;
            SkillBonuses.SetValues(0, SkillName.Stealth, 20.0);
        }

        public RoyalGuardInvestigatorsCloak(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override int LabelNumber => 1112409;// Royal Guard Investigator's Cloak [Replica]
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);//version
        }
    }
}