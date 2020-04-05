using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishBattleAxe))]
    [FlipableAttribute(0xF47, 0xF48)]
    public class BattleAxe : BaseAxe
    {
        [Constructable]
        public BattleAxe()
            : base(0xF47)
        {
            Weight = 4.0;
            Layer = Layer.TwoHanded;
        }

        public BattleAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ConcussionBlow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 16;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 19;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.50f;
            }
        }
        
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 70;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
