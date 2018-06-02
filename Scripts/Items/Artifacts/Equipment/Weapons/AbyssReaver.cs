using System;

namespace Server.Items
{
    public class AbyssReaver : BaseThrown
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1112694; } } // Abyss Reaver

        [Constructable]
        public AbyssReaver()
            : base(0x901)
        {
            Weight = 6.0;
            Layer = Layer.OneHanded;

            SkillBonuses.SetValues(0, SkillName.Throwing, 10);
            Attributes.WeaponDamage = 35;
            Slayer = SlayerName.DaemonDismissal;
        }

        public AbyssReaver(Serial serial)
            : base(serial)
        {
        }        

        public override int MinThrowRange { get { return 6; } }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.InfusedThrow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.MovingShot;
            }
        }

        public override int AosStrengthReq
        {
            get
            {
                return 40;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 17;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 25;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.25f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 20;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 9;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 41;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 20;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 35;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 70;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Attributes.AttackChance = 0;
                Attributes.WeaponSpeed = 0;
            }
        }
    }
}
