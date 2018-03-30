#region Header
// **********
// ServUO - IronWorker.cs
// **********
#endregion

#region References
using System.Collections.Generic;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class IronWorker : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

		[Constructable]
		public IronWorker()
			: base("the iron worker")
		{
			SetSkill(SkillName.ArmsLore, 36.0, 68.0);
			SetSkill(SkillName.Blacksmith, 65.0, 88.0);
			SetSkill(SkillName.Fencing, 60.0, 83.0);
			SetSkill(SkillName.Macing, 61.0, 93.0);
			SetSkill(SkillName.Swords, 60.0, 83.0);
			SetSkill(SkillName.Tactics, 60.0, 83.0);
			SetSkill(SkillName.Parry, 61.0, 93.0);
		}

		public IronWorker(Serial serial)
			: base(serial)
		{ }

		public override VendorShoeType ShoeType { get { return VendorShoeType.None; } }
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBAxeWeapon());
			m_SBInfos.Add(new SBKnifeWeapon());
			m_SBInfos.Add(new SBMaceWeapon());
			m_SBInfos.Add(new SBSmithTools());
			m_SBInfos.Add(new SBPoleArmWeapon());
			m_SBInfos.Add(new SBSpearForkWeapon());
			m_SBInfos.Add(new SBSwordWeapon());

			m_SBInfos.Add(new SBMetalShields());

			m_SBInfos.Add(new SBHelmetArmor());
			m_SBInfos.Add(new SBPlateArmor());
			m_SBInfos.Add(new SBChainmailArmor());
			m_SBInfos.Add(new SBRingmailArmor());
			m_SBInfos.Add(new SBStuddedArmor());
			m_SBInfos.Add(new SBLeatherArmor());
		}

		public override void InitOutfit()
		{
			Item item = (Utility.RandomBool() ? null : new RingmailChest());

			if (item != null && !EquipItem(item))
			{
				item.Delete();
				item = null;
			}

			switch (Utility.Random(4))
			{
				case 0:
				case 1:
					AddItem(new JesterHat(Utility.RandomBrightHue()));
					break;
				case 2:
					AddItem(new Bandana(Utility.RandomBrightHue()));
					break;
                case 3:
                    AddItem(new Bascinet());
                    break;
			}

			if (item == null)
			{
				AddItem(new FullApron(Utility.RandomBrightHue()));
			}

			AddItem(new SmithHammer());

            base.InitOutfit();

			item = FindItemOnLayer(Layer.Pants);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.OuterLegs);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.InnerLegs);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.OuterTorso);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.InnerTorso);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.Shirt);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
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