namespace Server.Items
{
    public class EpauletteBearingTheCrestOfBlackthorn3 : Cloak
    {
        public override bool IsArtifact => true;

        public override int LabelNumber => 1123325;  // Epaulette

        [Constructable]
        public EpauletteBearingTheCrestOfBlackthorn3()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            ItemID = 0x9985;
            SkillBonuses.SetValues(0, SkillName.Stealth, 10.0);
            Hue = 2130;

            Layer = Layer.OuterTorso;
        }

        public EpauletteBearingTheCrestOfBlackthorn3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                MaxHitPoints = 0;
                HitPoints = 0;

                if (Layer != Layer.OuterTorso)
                {
                    if (Parent is Mobile)
                    {
                        ((Mobile)Parent).AddToBackpack(this);
                    }

                    Layer = Layer.OuterTorso;
                }
            }
        }
    }
}