using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x13E3, 0x13E4)]
    public class SmithHammer : BaseTool
    {
        [Constructable]
        public SmithHammer()
            : base(0x13E3)
        {
            Weight = 8.0;
            Layer = Layer.OneHanded;
        }

        [Constructable]
        public SmithHammer(int uses)
            : base(uses, 0x13E3)
        {
            Weight = 8.0;
            Layer = Layer.OneHanded;
        }

        public SmithHammer(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefBlacksmithy.CraftSystem;
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

    public class SmithyHammer : BaseBashing, ITool
    {
        [Constructable]
        public SmithyHammer()
            : base(0x13E3)
        {
            Weight = 8.0;
            ShowUsesRemaining = true;
        }

        #region ITool Members
        public CraftSystem CraftSystem => DefBlacksmithy.CraftSystem;
        public bool BreakOnDepletion => true;

        public bool CheckAccessible(Mobile m, ref int num)
        {
            if (!IsChildOf(m) && Parent != m)
            {
                num = 1044263;
                return false;
            }

            return true;
        }
        #endregion

        public SmithyHammer(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ParalyzingBlow;
        public override int StrengthReq => 5;
        public override int MinDamage => 13;
        public override int MaxDamage => 17;
        public override float Speed => 3.25f;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        public override void OnDoubleClick(Mobile from)
        {
            if (CraftSystem != null && (IsChildOf(from.Backpack) || Parent == from))
            {
                int num = CraftSystem.CanCraft(from, this, null);

                if (num > 0 && num != 1044267) // Blacksmithing shows the gump regardless of proximity of an anvil and forge after SE
                {
                    from.SendLocalizedMessage(num);
                }
                else
                {
                    CraftContext context = CraftSystem.GetContext(from);

                    from.SendGump(new CraftGump(from, CraftSystem, this, null));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

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
