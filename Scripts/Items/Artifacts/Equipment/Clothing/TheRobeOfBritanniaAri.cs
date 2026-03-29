using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class TheRobeOfBritanniaAri : BaseOuterTorso, IRepairable
    {
        public CraftSystem RepairSystem { get { return DefTailoring.CraftSystem; } }

		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TheRobeOfBritanniaAri()
            : base(0x2684)
        {
            Hue = 0x48b;
            SkillBonuses.SetValues(0, SkillName.EvalInt, 15.0);
            Attributes.SpellDamage = 30;
            Attributes.RegenMana = 2;
        }

        public TheRobeOfBritanniaAri(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094931;
            }
        }// The Robe of Britannia "Ari" [Replica]
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
            }
        }
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