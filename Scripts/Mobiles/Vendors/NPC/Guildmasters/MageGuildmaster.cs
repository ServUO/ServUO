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
        private List<PendingConvert> m_PendingConverts = new List<PendingConvert>();

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
            PendingConvert.CreateExpireTimer(m_PendingConverts);
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

			if(from != null)
				list.Add(new UpgradeMageArmor(from, this));
		}

        public static bool CanConvertArmor(BaseArmor armor)
        {
            if (armor.ArtifactRarity != 0)
                return false;
            if (armor.ArmorAttributes.MageArmor == 0 &&
                Server.SkillHandlers.Imbuing.GetTotalMods(armor) > 4)
                return false;
            return true;
        }

        public void PlayerWantsToUpgrade(Mobile from, BaseArmor armor)
		{
            if(!CanConvertArmor(armor))
			{
				from.SendLocalizedMessage(1154119); // This action would exceed a stat cap
				return;
			}

			from.SendLocalizedMessage(1154117); // Ah yes, I will convert this piece of armor but it's gonna cost you 250,000 gold coin. Payment is due immediately. Just hand me the armor.
            m_PendingConverts.Add(new PendingConvert(from, armor));
		}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PendingConvert convert = null;

            foreach(PendingConvert c in m_PendingConverts)
            {
                if(c.From == from && c.Armor == dropped)
                {
                    convert = c;
                    break;
                }
            }

            if(convert == null)
                return base.OnDragDrop(from, dropped);

            m_PendingConverts.Remove(convert);
            from.CloseGump(typeof(ConfirmGump));
            from.SendGump(new ConfirmGump(convert.From, convert.Armor));
            return false; // Want the item to stay in the player's pack
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
			private DateTime Expires;

			public PendingConvert(Mobile from, BaseArmor armor)
			{
				From = from;
				Armor = armor;
				Expires = DateTime.UtcNow + TimeSpan.FromMinutes(2.0d);
			}

			protected bool Expired { get { return Expires > DateTime.UtcNow; } }

            public static void CreateExpireTimer(List<PendingConvert> list)
            {
                new ExpireTimer(list);
            }

            private class ExpireTimer : Timer
            {
                List<PendingConvert> List;

                public ExpireTimer(List<PendingConvert> list)
                    : base(TimeSpan.FromSeconds(15.0), TimeSpan.FromSeconds(15.0))
                {
                    List = list;
                }

                protected override void OnTick()
                {
                    List<PendingConvert> toDelete = new List<PendingConvert>();

                    foreach(PendingConvert convert in List)
                    {
                        if (convert.Expired)
                            toDelete.Add(convert);
                    }

                    foreach(PendingConvert convert in toDelete)
                    {
                        List.Remove(convert);
                    }
                }
            }
		}

		protected class ConfirmGump : BaseConfirmGump
		{
			Mobile From;
			BaseArmor Armor;

			public ConfirmGump(Mobile from, BaseArmor armor)
			{
                From = from;
                Armor = armor;
			}

            public override int TitleNumber
            {
                get
                {
                    return 1049004; // Confirm
                }
            }

            public override int LabelNumber
            {
                get
                {
                    return 1154115; // So you would like to add or remove mage armor from your armor at the cost of 250,000 gold?
                }
            }

            public override void Confirm(Mobile from)
            {
                if (From == null)
                    return;

                if (!CanConvertArmor(Armor))
                {
                    from.SendLocalizedMessage(1154119); // This action would exceed a stat cap
                    return;
                }
                   
                if (From.Account == null || !From.Account.WithdrawGold(250000))
                {
                    From.SendLocalizedMessage(1019022); // You do not have enough gold.
                    return;
                }

				if (!Armor.IsChildOf(From.Backpack))
					return;

                if (Armor.ArmorAttributes.MageArmor > 0)
                    Armor.ArmorAttributes.MageArmor = 0;
                else
                    Armor.ArmorAttributes.MageArmor = 1;
                Armor.InvalidateProperties();

                From.SendLocalizedMessage(1154118); // Your armor has been converted.
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