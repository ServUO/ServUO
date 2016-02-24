using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Targeting;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
    public class MageGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public MageGuildmaster()
            : base("mage")
        {
            this.SetSkill(SkillName.EvalInt, 85.0, 100.0);
            this.SetSkill(SkillName.Inscribe, 65.0, 88.0);
            this.SetSkill(SkillName.MagicResist, 64.0, 100.0);
            this.SetSkill(SkillName.Magery, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 60.0, 83.0);
            this.SetSkill(SkillName.Meditation, 85.0, 100.0);
            this.SetSkill(SkillName.Macing, 36.0, 68.0);
        }

        public MageGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.MagesGuild;
            }
        }
        public override VendorShoeType ShoeType
        {
            get
            {
                return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals;
            }
        }
        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.Robe(Utility.RandomBlueHue()));
            this.AddItem(new Server.Items.GnarledStaff());
        }

		public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.AddCustomContextEntries(from, list);

			//if(from != null)
			//	list.Add(new UpgradeMageArmor(from, this));
		}

		public void PlayerWantsToUpgrade(Mobile from, BaseArmor armor)
		{
			if (Server.SkillHandlers.Imbuing.GetTotalMods(armor) > 4 ||
				(armor is Artifact))
			{
				from.SendLocalizedMessage(1154119); // This action would exceed a stat cap
				return;
			}
			from.SendLocalizedMessage(1154117); // Ah yes, I will convert this piece of armor but it's gonna cost you 250,000 gold coin. Payment is due immediately. Just hand me the armor.
			
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

		protected class PendingConvert
		{
			public Mobile From;
			public BaseArmor Armor;
			public DateTime Expires;

			public PendingConvert(Mobile from, BaseArmor armor)
			{
				From = from;
				Armor = armor;
				Expires = DateTime.UtcNow + TimeSpan.FromMinutes(2.0d);
			}

			public bool Expired { get { return Expires > DateTime.UtcNow; } }
		}

		protected class ConfirmGump : BaseConfirmGump
		{
			Mobile From;
			MageGuildmaster GuildMaster;
			BaseArmor Armor;

			public ConfirmGump(Mobile from, MageGuildmaster gm, BaseArmor armor)
			{

			}
		}

		protected class UpgradeMageArmor : ContextMenuEntry
		{
			Mobile From;
			MageGuildmaster GuildMaster;

			public UpgradeMageArmor(Mobile from, MageGuildmaster gm)
				: base(1154114) // Convert Mage Armor
			{
				From = from;
				GuildMaster = gm;
			}

			public override void OnClick()
			{
				From.Target = new InternalTarget(From, GuildMaster);
				From.SendLocalizedMessage(1154116); // Target a piece of armor to show to the guild master.
			}

			private class InternalTarget : Target
			{
				Mobile From;
				MageGuildmaster GuildMaster;

				public InternalTarget(Mobile from, MageGuildmaster gm)
					: base(1, false, TargetFlags.None)
				{
					From = from;
					GuildMaster = gm;
				}

				protected override void OnTarget(Mobile from, object targeted)
				{
					if (from == null || targeted == null || !(targeted is BaseArmor))
						return;

					BaseArmor armor = (BaseArmor)targeted;
					GuildMaster.PlayerWantsToUpgrade(from, armor);
				}
			}
		}
    }
}