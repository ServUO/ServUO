namespace Server.Items
{
    [Flipable(0x13B2, 0x13B1)]
    public class JukaBow : Bow
    {
        [Constructable]
        public JukaBow()
        { 
		}

        public JukaBow(Serial serial)
            : base(serial)
        { 
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsModified => Slayer != SlayerName.None;

        public override bool CanEquip(Mobile from)
        {
            if (IsModified)
            {
                return base.CanEquip(from);
            }

            from.SendMessage("You cannot equip this bow until a bowyer modifies it.");
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsModified)
            {
                from.SendMessage("That has already been modified.");
            }
            else if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This must be in your backpack to modify it.");
            }
            else if (from.Skills[SkillName.Fletching].Base < 100.0)
            {
                from.SendMessage("Only a grandmaster bowcrafter can modify this weapon.");
            }
            else
            {
                from.BeginTarget(2, false, Targeting.TargetFlags.None, OnTargetGears);
                from.SendMessage("Select the gears you wish to use.");
            }
        }

        public void OnTargetGears(Mobile from, object targ)
        {
            Gears g = targ as Gears;

            if (g == null || !g.IsChildOf(from.Backpack))
            {
                from.SendMessage("Those are not gears."); 
            }
            else if (IsModified)
            {
                from.SendMessage("That has already been modified.");
            }
            else if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This must be in your backpack to modify it.");
            }
            else if (from.Skills[SkillName.Fletching].Base < 100.0)
            {
                from.SendMessage("Only a grandmaster bowcrafter can modify this weapon.");
            }
            else
            {
                g.Consume();

                Hue = 0x453;
                Slayer = (SlayerName)Utility.Random(2, 25);

                from.SendMessage("You modify it.");
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