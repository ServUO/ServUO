using System;

namespace Server.Items
{
    public class RoyalGuardSurvivalKnife : SkinningKnife
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RoyalGuardSurvivalKnife()
        {
            this.Attributes.SpellChanneling = 1;
            this.Attributes.Luck = 140;
            this.Attributes.EnhancePotions = 25;

            this.WeaponAttributes.UseBestSkill = 1;
            this.WeaponAttributes.LowerStatReq = 50;
        }

        public RoyalGuardSurvivalKnife(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094918;
            }
        }// Royal Guard Survival Knife [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
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