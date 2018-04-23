using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

/*********************************
[iamyourmaster
[obeyme
InstaTameDeed

use command [iamyourmaster to instantly tame a creature
or use command [obeyme to instantly tame AND set it so a player can control without any taming or lore

or you can give a player a InstaTameDeed that they can double click use theirselves but will only Instant Tame, 
it will not effect the required taming/lore.

*********************************/
namespace Server.Items
{

    public class InstaTameDeed : Item
    {
        public static void Initialize()
        {
            CommandSystem.Register("IAMYOURMASTER", AccessLevel.GameMaster, new CommandEventHandler(IAMYOURMASTER_OnCommand));
            CommandSystem.Register("OBEYME", AccessLevel.GameMaster, new CommandEventHandler(OBEYME_OnCommand));
        }


        [Usage("IAMYOURMASTER")]
        [Description("Instantly tames pet")]
        public static void IAMYOURMASTER_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the animal or beast you wish to instantly tame.");
            e.Mobile.Target = new InstaTameTarget(false);
        }

        [Usage("OBEYME")]
        [Description("Instantly tames pet AND sets minimum taming skill to 0.00")]
        public static void OBEYME_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the animal or beast you wish to instantly tame.");
            e.Mobile.Target = new InstaTameTarget(true);
        }

        [Constructable]
        public InstaTameDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
            Name = "Instant Taming Deed";
            LootType = LootType.Blessed;
            Hue = 572;
        }

        public InstaTameDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();
        }

        public override bool DisplayLootType { get { return false; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) // Make sure its in their pack 
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
            }
            else
            {
                from.SendMessage("Target the animal or beast you wish to instantly tame.");
                from.Target = new InstaTameTarget(false, this);
            }
        }
    }

    public class InstaTameTarget : Target
    {
        private InstaTameDeed m_Deed;
        bool Obey { get; set; }

        public InstaTameTarget(bool obey, InstaTameDeed deed = null)
            : base(15, false, TargetFlags.None)
        {
            if (deed != null)
            {
                m_Deed = deed;
            }
            Obey = obey;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is BaseCreature)
            {
                BaseCreature t = (BaseCreature)target;

                if (t.ControlMaster != null)
                {
                    from.SendMessage("That pet belongs to someone else!");
                }
                else if (t.Tamable == false)
                {
                    from.SendMessage("That creature cannot be tamed!");
                }
                else
                {
                    t.Controlled = true;
                    t.ControlMaster = from;
                    from.SendMessage("You have instantly tamed your target.");

                    if (m_Deed != null)
                    {
                        m_Deed.Delete(); // Delete the deed 
                    }
                    if (Obey == true)
                    {
                        t.MinTameSkill = 0.00;
                    }
                }

            }
            else
            {
                from.SendMessage("That is not a valid target.");
            }
        }
    }


}
