using System;

namespace Server.Items
{
    public class CarvedBoneRelicFromHolmes : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public CarvedBoneRelicFromHolmes()
            : base(0x2F58)
        {
            Name = "Carved Bone Relic from Holmes";
            Hue = 1150;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 20.0);
            Attributes.EnhancePotions = 15;
        }

        public CarvedBoneRelicFromHolmes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
