#region References
using System;
using System.Collections.Generic;
using System.Globalization;

using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Plants;
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace Server.Items
{
	[Flipable(0x0DFC, 0x0DFD)]
	public class Clippers : Item, IUsesRemaining, ICraftable
	{
		private Mobile _Crafter;
		private ToolQuality _Quality;

		private int _UsesRemaining;
		private bool _ShowUsesRemaining;

		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Crafter
		{
			get { return _Crafter; }
			set
			{
				_Crafter = value;

				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public ToolQuality Quality
		{
			get { return _Quality; }
			set
			{
				UnscaleUses();

				_Quality = value;

				ScaleUses();
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int UsesRemaining
		{
			get { return _UsesRemaining; }
			set
			{
				_UsesRemaining = value;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ShowUsesRemaining
		{
			get { return _ShowUsesRemaining; }
			set
			{
				_ShowUsesRemaining = value;

				InvalidateProperties();
			}
		}

		public override int LabelNumber { get { return 1112117; } } // clippers
		
		[Constructable]
		public Clippers()
			: base(0x0DFC)
		{
			Weight = 1.0;
			Hue = 1168;
		}

		public void ScaleUses()
		{
			_UsesRemaining = (_UsesRemaining * GetUsesScalar()) / 100;
			InvalidateProperties();
		}

		public void UnscaleUses()
		{
			_UsesRemaining = (_UsesRemaining * 100) / GetUsesScalar();
		}

		public int GetUsesScalar()
		{
			return _Quality == ToolQuality.Exceptional ? 200 : 100;
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			//Makers mark not displayed on OSI
			if (_Crafter != null)
			{
				list.Add(1050043, _Crafter.TitleName); // crafted by ~1_NAME~
			}

			if (_Quality == ToolQuality.Exceptional)
			{
				list.Add(1060636); // exceptional
			}

			list.Add(1060584, _UsesRemaining.ToString(CultureInfo.InvariantCulture)); // uses remaining: ~1_val~
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (_Crafter != null)
			{
				LabelTo(from, 1050043, _Crafter.TitleName); // crafted by ~1_NAME~
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			AddContextMenuEntries(from, this, list);
		}

		public static void AddContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
		{
			if (!item.IsChildOf(from.Backpack) && item.Parent != from)
			{
				return;
			}

			PlayerMobile pm = from as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			/*
			int typeEntry = 0;

			if (pm.ToggleCutClippings)
			{
				typeEntry = 1112282;
			}

			if (pm.ToggleCutReeds)
			{
				typeEntry = 1112283;
			}

			list.Add( new ContextMenuEntry(typeEntry) { Color = 0x421F } );
			*/

			list.Add(new ToggleClippings(pm, true, false, 1112282)); //set to clip plants
			list.Add(new ToggleClippings(pm, false, true, 1112283)); //Set to cut reeds
		}

		private class ToggleClippings : ContextMenuEntry
		{
			private readonly PlayerMobile m_Mobile;
			private readonly bool m_Valueclips;
			private readonly bool m_Valuereeds;

			public ToggleClippings(PlayerMobile mobile, bool valueclips, bool valuereeds, int number)
				: base(number)
			{
				m_Mobile = mobile;
				m_Valueclips = valueclips;
				m_Valuereeds = valuereeds;
			}

			public override void OnClick()
			{
				bool oldValueclips = m_Mobile.ToggleCutClippings;
				bool oldValuereeds = m_Mobile.ToggleCutReeds;

				if (m_Valueclips)
				{
					if (oldValueclips)
					{
						m_Mobile.ToggleCutClippings = true;
						m_Mobile.ToggleCutReeds = false;
						m_Mobile.SendLocalizedMessage(1112284); // You are already set to make plant clippings 
					}
					else
					{
						m_Mobile.ToggleCutClippings = true;
						m_Mobile.ToggleCutReeds = false;
						m_Mobile.SendLocalizedMessage(1112285); // You are now set to make plant clippings
					}
				}
				else if (m_Valuereeds)
				{
					if (oldValuereeds)
					{
						m_Mobile.ToggleCutReeds = true;
						m_Mobile.ToggleCutClippings = false;
						m_Mobile.SendLocalizedMessage(1112287); // You are already set to cut reeds. 
					}
					else
					{
						m_Mobile.ToggleCutReeds = true;
						m_Mobile.ToggleCutClippings = false;
						m_Mobile.SendLocalizedMessage(1112286); // You are now set to cut reeds.
					}
				}
			}
		}

		public Clippers(Serial serial)
			: base(serial)
		{ }

		public virtual PlantHue PlantHue { get { return PlantHue.None; } }

		public override void OnDoubleClick(Mobile from)
		{
			from.SendLocalizedMessage(1112118); // What plant do you wish to use these clippers on?

			from.Target = new InternalTarget(this);
		}

		private class InternalTarget : Target
		{
			private readonly Clippers m_Item;

			public InternalTarget(Clippers item)
				: base(2, false, TargetFlags.None)
			{
				m_Item = item;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				PlayerMobile pm = from as PlayerMobile;

				if (pm == null || m_Item == null || m_Item.Deleted)
				{
					return;
				}

				PlantItem plant = targeted as PlantItem;

				if (null == plant || PlantStatus.DecorativePlant != plant.PlantStatus)
				{
					from.SendLocalizedMessage(1112119); // You may only use these clippers on decorative plants.
					return;
				}

				if (pm.ToggleCutClippings)
				{
					/*
					PlantClippings clippings = new PlantClippings();
                    
					clippings.PlantHue = plant.PlantHue;
                    clippings.MoveToWorld(plant.Location, plant.Map);
                    plant.Delete();
					*/
					from.PlaySound(0x248);
					from.AddToBackpack(
						new PlantClippings
						{
							Hue = ((PlantItem)targeted).Hue,
							PlantHue = plant.PlantHue
						});
					plant.Delete();
				}
				else if (pm.ToggleCutReeds)
				{
					/*
					DryReeds reeds = new DryReeds();
                    
					reeds.PlantHue = plant.PlantHue;
                    reeds.MoveToWorld(plant.Location, plant.Map);
                    plant.Delete();
                    from.PlaySound(0x248);
					*/
					from.PlaySound(0x248);
					from.AddToBackpack(
						new DryReeds
						{
							Hue = ((PlantItem)targeted).Hue,
							PlantHue = plant.PlantHue
						});
					plant.Delete();
				}

				// TODO: Add in clipping hedges (short and tall) and juniperbushes for topiaries
			}
		}

		#region ICraftable Members
		public int OnCraft(
			int quality,
			bool makersMark,
			Mobile from,
			CraftSystem craftSystem,
			Type typeRes,
			BaseTool tool,
			CraftItem craftItem,
			int resHue)
		{
			Quality = (ToolQuality)quality;

			if (makersMark)
			{
				Crafter = from;
			}

			return quality;
		}
		#endregion

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			const int ver = 2;

			writer.Write(ver); // version

			switch (ver)
			{
				case 2:
					writer.Write(_ShowUsesRemaining);
					goto case 1;
				case 1:
					writer.Write(_UsesRemaining);
					goto case 0;
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					_ShowUsesRemaining = reader.ReadBool();
					goto case 1;
				case 1:
					_UsesRemaining = reader.ReadInt();
					goto case 0;
				case 0:
					{
						if (version < 2)
						{
							ShowUsesRemaining = true;
						}
					}
					break;
			}
		}
	}
}