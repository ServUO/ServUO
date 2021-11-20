using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Network;
using Server.Misc;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.XmlSpawner2;

namespace Server.Gumps
{
    #region " Gump "
    public class ItemExperienceGump : Gump
	{
		private Mobile m_From;
		private Item m_Item;
		private AttributeCategory m_Cat;
		private GumpPage m_Page;

        private const int LabelHue = 0x480;
        private const int TitleHue = 0x12B;

		private enum GumpPage
		{
			None,
			AttributeList
		}

		public ItemExperienceGump( Mobile from, Item item, AttributeCategory cat ) : this( from, item, cat, GumpPage.None )
		{
		}

		private ItemExperienceGump( Mobile from, Item item, AttributeCategory cat, GumpPage page ) : base( 40, 40 )
		{
			m_From = from;
			m_Item = item;
			m_Cat = cat;
			m_Page = page;

			from.CloseGump( typeof( ItemExperienceGump ) );

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;

			AddPage(0);
			AddBackground(50, 35, 540, 382, 9270);
			AddAlphaRegion(66, 91, 219, 170);
			AddAlphaRegion(66, 49, 508, 34);
			AddAlphaRegion(292, 91, 283, 279);
			AddAlphaRegion(66, 269, 219, 101);

			AddLabel(262, 56, TitleHue, @"Item Experience");

            AddLabel(136, 93, TitleHue, @"Categories");
			AddButton(75, 116, 4005, 4007, GetButtonID( 1, 0 ), GumpButtonType.Reply, 0);
            AddLabel(112, 117, LabelHue, @"Melee Attributes");
			AddButton(75, 138, 4005, 4007, GetButtonID( 1, 1 ), GumpButtonType.Reply, 0);
            AddLabel(112, 139, LabelHue, @"Magic Attributes");
			AddButton(75, 160, 4005, 4007, GetButtonID( 1, 2 ), GumpButtonType.Reply, 0);
            AddLabel(112, 161, LabelHue, @"Character Stats");
			AddButton(75, 182, 4005, 4007, GetButtonID( 1, 3 ), GumpButtonType.Reply, 0);
            AddLabel(112, 183, LabelHue, @"Resistances");
            if (m_Item is BaseWeapon)
			    AddButton(75, 204, 4005, 4007, GetButtonID( 1, 4 ), GumpButtonType.Reply, 0);
            AddLabel(112, 205, LabelHue, @"Weapon Hits");
			AddButton(75, 226, 4005, 4007, GetButtonID( 1, 5 ), GumpButtonType.Reply, 0);
            AddLabel(112, 227, LabelHue, @"Misc. Attributes");

			AddLabel(394, 93, TitleHue, @"Attributes");

			AddImage(0, 4, 10440);
			AddImage(554, 4, 10441);

            CreateItemExpList();

			AddButton(280, 379, 241, 243, 0, GumpButtonType.Reply, 0); //Cancel

			if ( page == GumpPage.AttributeList )
				CreateAttributeList( cat );
		}

        public void CreateItemExpList()
        {
            //ILevelable levitem = (ILevelable)m_Item;
            XmlLevelItem levitem = XmlAttach.FindAttachment(m_Item, typeof(XmlLevelItem)) as XmlLevelItem;

            AddLabel(75, 275, LabelHue, @"Max levels on item:");
            AddLabel(198, 275, LabelHue, levitem.MaxLevel.ToString());

            AddLabel(75, 297, LabelHue, @"Experience:");
            AddLabel(154, 297, LabelHue, levitem.Experience.ToString());

            int tolevel = 0;
            for (int i = 0; i < LevelItemManager.ExpTable.Length; ++i)
            {
                if (levitem.Experience < LevelItemManager.ExpTable[i])
                {
                    tolevel = LevelItemManager.ExpTable[i] - levitem.Experience;
                    break;
                }
            }
            AddLabel(75, 319, LabelHue, @"Exp. to next level:");
            AddLabel(191, 319, LabelHue, tolevel.ToString());

            AddLabel(75, 341, LabelHue, @"Spending Points(sp) Avail:");
            AddLabel(230, 341, LabelHue, levitem.Points.ToString());
        }

		public void CreateAttributeList( AttributeCategory cat )
		{
			int index = 0;
			int pageindex;
			int attrvalue;

			for ( int i = 0; i < LevelAttributes.m_Attributes.Length; ++i )
			{
				if (LevelAttributes.m_Attributes[i].m_Category == cat)
				{
					pageindex = index % 10;

					if ( pageindex == 0 )
					{
						if ( index > 0 )
						{
							AddButton(536, 343, 4005, 4007, 0, GumpButtonType.Page, (index / 10) + 1);
                            AddLabel(497, 344, LabelHue, @"Next");
						}

						AddPage( (index / 10) + 1 );

						if ( index > 0 )
						{
							AddButton(301, 343, 4014, 4016, 0, GumpButtonType.Page, index / 10);
                            AddLabel(337, 344, LabelHue, @"Previous");
						}
					}

                    if (m_Item is BaseWeapon)
                        attrvalue = ((BaseWeapon)m_Item).Attributes[LevelAttributes.m_Attributes[i].m_Attribute];
                    else if (m_Item is BaseArmor)
                        attrvalue = ((BaseArmor)m_Item).Attributes[LevelAttributes.m_Attributes[i].m_Attribute];
                    else if (m_Item is BaseJewel)
                        attrvalue = ((BaseJewel)m_Item).Attributes[LevelAttributes.m_Attributes[i].m_Attribute];
                    else if (m_Item is BaseClothing)
                        attrvalue = ((BaseClothing)m_Item).Attributes[LevelAttributes.m_Attributes[i].m_Attribute];
                    else
                        return;

                    if (attrvalue < LevelAttributes.m_Attributes[i].m_MaxValue)
					    AddButton(301, 116 + (pageindex * 20), 4005, 4007, GetButtonID( 2, i ), GumpButtonType.Reply, 0);
                    AddLabel(337, 117 + (pageindex * 20), LabelHue, LevelAttributes.m_Attributes[i].m_Name+" ("+GetPointCost(m_Item, LevelAttributes.m_Attributes[i].m_XP)+"sp)");
                    AddLabel(540, 117 + (pageindex * 20), LabelHue, attrvalue.ToString());

					++index;
				}
			}

            if (m_Item is BaseWeapon)
            {
                for (int i = 0; i < LevelAttributes.m_WeaponAttributes.Length; ++i)
                {
                    if (LevelAttributes.m_WeaponAttributes[i].m_Category == cat)
                    {
                        pageindex = index % 10;

                        if (pageindex == 0)
                        {
                            if (index > 0)
                            {
                                AddButton(536, 343, 4005, 4007, 0, GumpButtonType.Page, (index / 10) + 1);
                                AddLabel(497, 344, LabelHue, @"Next");
                            }

                            AddPage((index / 10) + 1);

                            if (index > 0)
                            {
                                AddButton(301, 343, 4014, 4016, 0, GumpButtonType.Page, index / 10);
                                AddLabel(337, 344, LabelHue, @"Previous");
                            }
                        }

						if (LevelAttributes.m_WeaponAttributes[i].m_Attribute == AosWeaponAttribute.DurabilityBonus)
						{
							attrvalue = ((BaseWeapon)m_Item).MaxHitPoints;
						}
						else
						{
                        	attrvalue = ((BaseWeapon)m_Item).WeaponAttributes[LevelAttributes.m_WeaponAttributes[i].m_Attribute];
						}
                        if (attrvalue < LevelAttributes.m_WeaponAttributes[i].m_MaxValue)
                            AddButton(301, 116 + (pageindex * 20), 4005, 4007, GetButtonID(3, i), GumpButtonType.Reply, 0);

                        AddLabel(337, 117 + (pageindex * 20), LabelHue, LevelAttributes.m_WeaponAttributes[i].m_Name+" ("+GetPointCost(m_Item, LevelAttributes.m_WeaponAttributes[i].m_XP)+"sp)");
                        AddLabel(540, 117 + (pageindex * 20), LabelHue, attrvalue.ToString());

                        ++index;
                    }
                }
            }
            else if (m_Item is BaseArmor)
            {
                if (cat == AttributeCategory.Resists)
                {
					for (int i = 0; i < LevelAttributes.m_ResistanceTypes.Length; ++i)
					{
						pageindex = index % 10;

						if (pageindex == 0)
						{
							if (index > 0)
							{
								AddButton(536, 343, 4005, 4007, 0, GumpButtonType.Page, (index / 10) + 1);
								AddLabel(497, 344, LabelHue, @"Next");
							}

							AddPage((index / 10) + 1);

							if (index > 0)
							{
								AddButton(301, 343, 4014, 4016, 0, GumpButtonType.Page, index / 10);
								AddLabel(337, 344, LabelHue, @"Previous");
							}
						}

						if (LevelAttributes.m_ResistanceTypes[i].m_Attribute == ResistanceType.Physical)
							attrvalue = ((BaseArmor)m_Item).PhysicalBonus;
						else if (LevelAttributes.m_ResistanceTypes[i].m_Attribute == ResistanceType.Fire)
							attrvalue = ((BaseArmor)m_Item).FireBonus;
						else if (LevelAttributes.m_ResistanceTypes[i].m_Attribute == ResistanceType.Cold)
							attrvalue = ((BaseArmor)m_Item).ColdBonus;
						else if (LevelAttributes.m_ResistanceTypes[i].m_Attribute == ResistanceType.Poison)
							attrvalue = ((BaseArmor)m_Item).PoisonBonus;
						else if (LevelAttributes.m_ResistanceTypes[i].m_Attribute == ResistanceType.Energy)
							attrvalue = ((BaseArmor)m_Item).EnergyBonus;
						else
							attrvalue = 0;

						if (attrvalue < LevelAttributes.m_ResistanceTypes[i].m_MaxValue)
							AddButton(301, 116 + (pageindex * 20), 4005, 4007, GetButtonID(5, i), GumpButtonType.Reply, 0);
						AddLabel(337, 117 + (pageindex * 20), LabelHue, LevelAttributes.m_ResistanceTypes[i].m_Name+" ("+GetPointCost(m_Item, LevelAttributes.m_ResistanceTypes[i].m_XP)+"sp)");
						AddLabel(540, 117 + (pageindex * 20), LabelHue, attrvalue.ToString());

						++index;
					}

                }
                else
                {
					for (int i = 0; i < LevelAttributes.m_ArmorAttributes.Length; ++i)
					{
						if (LevelAttributes.m_ArmorAttributes[i].m_Category == cat)
						{
							pageindex = index % 10;

							if (pageindex == 0)
							{
								if (index > 0)
								{
									AddButton(536, 343, 4005, 4007, 0, GumpButtonType.Page, (index / 10) + 1);
									AddLabel(497, 344, LabelHue, @"Next");
								}

								AddPage((index / 10) + 1);

								if (index > 0)
								{
									AddButton(301, 343, 4014, 4016, 0, GumpButtonType.Page, index / 10);
									AddLabel(337, 344, LabelHue, @"Previous");
								}
							}

							if (LevelAttributes.m_ArmorAttributes[i].m_Attribute == AosArmorAttribute.DurabilityBonus)
							{
								attrvalue = ((BaseArmor)m_Item).MaxHitPoints;
							}
							else
							{
								attrvalue = ((BaseArmor)m_Item).ArmorAttributes[LevelAttributes.m_ArmorAttributes[i].m_Attribute];
							}
							if (attrvalue < LevelAttributes.m_ArmorAttributes[i].m_MaxValue)
								AddButton(301, 116 + (pageindex * 20), 4005, 4007, GetButtonID(4, i), GumpButtonType.Reply, 0);

							AddLabel(337, 117 + (pageindex * 20), LabelHue, LevelAttributes.m_ArmorAttributes[i].m_Name+" ("+GetPointCost(m_Item, LevelAttributes.m_ArmorAttributes[i].m_XP)+"sp)");
							AddLabel(540, 117 + (pageindex * 20), LabelHue, attrvalue.ToString());

							++index;
						}
					}
				}
			}
            else if (m_Item is BaseJewel || m_Item is BaseClothing)
			{
                if (cat == AttributeCategory.Resists)
                {
					for (int i = 0; i < LevelAttributes.m_ElementAttributes.Length; ++i)
					{
						pageindex = index % 10;

						if (pageindex == 0)
						{
							if (index > 0)
							{
								AddButton(536, 343, 4005, 4007, 0, GumpButtonType.Page, (index / 10) + 1);
								AddLabel(497, 344, LabelHue, @"Next");
							}

							AddPage((index / 10) + 1);

							if (index > 0)
							{
								AddButton(301, 343, 4014, 4016, 0, GumpButtonType.Page, index / 10);
								AddLabel(337, 344, LabelHue, @"Previous");
							}
						}

						if (m_Item is BaseJewel)
							attrvalue = ((BaseJewel)m_Item).Resistances[LevelAttributes.m_ElementAttributes[i].m_Attribute];
						else
							attrvalue = ((BaseClothing)m_Item).Resistances[LevelAttributes.m_ElementAttributes[i].m_Attribute];

						if (attrvalue < LevelAttributes.m_ElementAttributes[i].m_MaxValue)
							AddButton(301, 116 + (pageindex * 20), 4005, 4007, GetButtonID(6, i), GumpButtonType.Reply, 0);
						AddLabel(337, 117 + (pageindex * 20), LabelHue, LevelAttributes.m_ElementAttributes[i].m_Name+" ("+GetPointCost(m_Item, LevelAttributes.m_ElementAttributes[i].m_XP)+"sp)");
						AddLabel(540, 117 + (pageindex * 20), LabelHue, attrvalue.ToString());

						++index;
					}
				}
            }
            else
            {
				return;
			}
		}

	public static int GetPointCost(Item m_Item, int xp)
        {
            int cost = xp;

            if (LevelItems.DoubleArtifactCost)
            {
                if (m_Item is BaseWeapon)
                {
                    if (((BaseWeapon)m_Item).ArtifactRarity >= 10)
                        cost *= 2;
                }
                else if (m_Item is BaseArmor)
                {
                    if (((BaseArmor)m_Item).ArtifactRarity >= 10)
                        cost *= 2;
                }
                else if (m_Item is BaseJewel)
                {
                    if (((BaseJewel)m_Item).ArtifactRarity >= 10)
                        cost *= 2;
                }
                else
                {
                    if (((BaseClothing)m_Item).ArtifactRarity >= 10)
                        cost *= 2;
                }
            }

            return cost;
        }

		public static int GetButtonID( int type, int index )
		{
			return 1 + type + (index * 7);
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID <= 0 )
				return; // Canceled


            //ILevelable levitem = (ILevelable)m_Item;
            XmlLevelItem levitem = XmlAttach.FindAttachment(m_Item, typeof(XmlLevelItem)) as XmlLevelItem;

			int buttonID = info.ButtonID - 1;
			int type = buttonID % 7;
			int index = buttonID / 7;

			int cost = 0;
			int attrvalue = 0;

			switch ( type )
			{
				case 0: // Cancel
				{
					break;
				}
				case 1: // Select Attribute Type
				{
					switch ( index )
					{
						case 0: // Melee
						{
							m_From.SendGump( new ItemExperienceGump( m_From, m_Item, AttributeCategory.Melee, GumpPage.AttributeList ) );

							break;
						}
						case 1: // Magic
						{
							m_From.SendGump( new ItemExperienceGump( m_From, m_Item, AttributeCategory.Magic, GumpPage.AttributeList ) );

							break;
						}
						case 2: // Char Stats
						{
							m_From.SendGump( new ItemExperienceGump( m_From, m_Item, AttributeCategory.Stats, GumpPage.AttributeList ) );

							break;
						}
						case 3: // Resistances
						{
							m_From.SendGump( new ItemExperienceGump( m_From, m_Item, AttributeCategory.Resists, GumpPage.AttributeList ) );

							break;
						}
						case 4: // Weapon Hits
						{
							m_From.SendGump( new ItemExperienceGump( m_From, m_Item, AttributeCategory.Hits, GumpPage.AttributeList ) );

							break;
						}
						case 5: // Misc.
						{
							m_From.SendGump( new ItemExperienceGump( m_From, m_Item, AttributeCategory.Misc, GumpPage.AttributeList ) );

							break;
						}
					}

					break;
				}

				case 2: // Attribute selected
				{
					cost = GetPointCost(m_Item, LevelAttributes.m_Attributes[index].m_XP);

                    if ((levitem.Points - cost) >= 0)
                    {
                        //add point to selected attribute
                        if (index >= 0 && index < LevelAttributes.m_Attributes.Length)
                        {
							if (m_Item is BaseWeapon)
							{
								attrvalue = ((BaseWeapon)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute];
								if (attrvalue < LevelAttributes.m_Attributes[index].m_MaxValue)
								{
									((BaseWeapon)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute] += 1;
									levitem.Points -= cost;
								}
							}
							else if (m_Item is BaseArmor)
							{
								attrvalue = ((BaseArmor)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute];
								if (attrvalue < LevelAttributes.m_Attributes[index].m_MaxValue)
								{
									((BaseArmor)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute] += 1;
									levitem.Points -= cost;
								}
							}
							else if (m_Item is BaseJewel)
							{
								attrvalue = ((BaseJewel)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute];
								if (attrvalue < LevelAttributes.m_Attributes[index].m_MaxValue)
								{
									((BaseJewel)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute] += 1;
									levitem.Points -= cost;
								}
							}
							else if (m_Item is BaseClothing)
							{
								attrvalue = ((BaseClothing)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute];
								if (attrvalue < LevelAttributes.m_Attributes[index].m_MaxValue)
								{
									((BaseClothing)m_Item).Attributes[LevelAttributes.m_Attributes[index].m_Attribute] += 1;
									levitem.Points -= cost;
								}
							}
							else
								return;
    					}
                    }
                    else
                    {
                        m_From.SendMessage("You don't have enough points available!  This attribute costs "+cost+" points.");
                    }

                    m_From.SendGump(new ItemExperienceGump(m_From, m_Item, LevelAttributes.m_Attributes[index].m_Category, GumpPage.AttributeList));

                    break;
				}
                case 3: // WeaponAttribute selected
                {
					cost = GetPointCost(m_Item, LevelAttributes.m_WeaponAttributes[index].m_XP);

                    if ((levitem.Points - cost) >= 0)
                    {
                        //add point to selected weapon attribute
                        if (index >= 0 && index < LevelAttributes.m_WeaponAttributes.Length)
                        {
							if (LevelAttributes.m_WeaponAttributes[index].m_Attribute == AosWeaponAttribute.DurabilityBonus)
							{
								attrvalue = ((BaseWeapon)m_Item).MaxHitPoints;
							}
							else
							{
                        		attrvalue = ((BaseWeapon)m_Item).WeaponAttributes[LevelAttributes.m_WeaponAttributes[index].m_Attribute];
							}

							if (attrvalue < LevelAttributes.m_WeaponAttributes[index].m_MaxValue)
							{
                            	((BaseWeapon)m_Item).WeaponAttributes[LevelAttributes.m_WeaponAttributes[index].m_Attribute] += 1;
                            	levitem.Points -= cost;
							}
                        }
                    }
                    else
                    {
                        m_From.SendMessage("You don't have enough points available!  This attribute costs "+cost+" points.");
                    }

                    m_From.SendGump(new ItemExperienceGump(m_From, m_Item, LevelAttributes.m_WeaponAttributes[index].m_Category, GumpPage.AttributeList));

                    break;
                }
                case 4: // Armor Attributes Selected
                {
					cost = GetPointCost(m_Item, LevelAttributes.m_ArmorAttributes[index].m_XP);
                    if ((levitem.Points - cost) >= 0)
                    {
                        //add point to selected weapon attribute
                        if (index >= 0 && index < LevelAttributes.m_ArmorAttributes.Length)
                        {
							if (LevelAttributes.m_ArmorAttributes[index].m_Attribute == AosArmorAttribute.DurabilityBonus)
							{
								attrvalue = ((BaseArmor)m_Item).MaxHitPoints;
							}
							else
							{
								attrvalue = ((BaseArmor)m_Item).ArmorAttributes[LevelAttributes.m_ArmorAttributes[index].m_Attribute];
							}

							if (attrvalue < LevelAttributes.m_ArmorAttributes[index].m_MaxValue)
							{
                            	((BaseArmor)m_Item).ArmorAttributes[LevelAttributes.m_ArmorAttributes[index].m_Attribute] += 1;
                            	levitem.Points -= cost;
							}
                        }
                    }
                    else
                    {
                        m_From.SendMessage("You don't have enough points available!  This attribute costs "+cost+" points.");
                    }

                    m_From.SendGump(new ItemExperienceGump(m_From, m_Item, LevelAttributes.m_ArmorAttributes[index].m_Category, GumpPage.AttributeList));

                    break;
                }
                case 5: // Armor Resists Selected
                {
					cost = GetPointCost(m_Item, LevelAttributes.m_ResistanceTypes[index].m_XP);
                    if ((levitem.Points - cost) >= 0)
                    {
                        //add point to selected weapon attribute
                        if (index >= 0 && index < LevelAttributes.m_ResistanceTypes.Length)
                        {
							if (LevelAttributes.m_ResistanceTypes[index].m_Attribute == ResistanceType.Physical)
							{
								attrvalue = ((BaseArmor)m_Item).PhysicalBonus;
								if (attrvalue < LevelAttributes.m_ResistanceTypes[index].m_MaxValue)
								{
									((BaseArmor)m_Item).PhysicalBonus += 1;
									levitem.Points -= cost;
								}
							}
							else if (LevelAttributes.m_ResistanceTypes[index].m_Attribute == ResistanceType.Fire)
							{
								attrvalue = ((BaseArmor)m_Item).FireBonus;
								if (attrvalue < LevelAttributes.m_ResistanceTypes[index].m_MaxValue)
								{
									((BaseArmor)m_Item).FireBonus += 1;
									levitem.Points -= cost;
								}
							}
							else if (LevelAttributes.m_ResistanceTypes[index].m_Attribute == ResistanceType.Cold)
							{
								attrvalue = ((BaseArmor)m_Item).ColdBonus;
								if (attrvalue < LevelAttributes.m_ResistanceTypes[index].m_MaxValue)
								{
									((BaseArmor)m_Item).ColdBonus += 1;
									levitem.Points -= cost;
								}
							}
							else if (LevelAttributes.m_ResistanceTypes[index].m_Attribute == ResistanceType.Poison)
							{
								attrvalue = ((BaseArmor)m_Item).PoisonBonus;
								if (attrvalue < LevelAttributes.m_ResistanceTypes[index].m_MaxValue)
								{
									((BaseArmor)m_Item).PoisonBonus += 1;
									levitem.Points -= cost;
								}
							}
							else if (LevelAttributes.m_ResistanceTypes[index].m_Attribute == ResistanceType.Energy)
							{
								attrvalue = ((BaseArmor)m_Item).EnergyBonus;
								if (attrvalue < LevelAttributes.m_ResistanceTypes[index].m_MaxValue)
								{
									((BaseArmor)m_Item).EnergyBonus += 1;
									levitem.Points -= cost;
								}
							}
							else
								return;
                        }
                    }
                    else
                    {
                        m_From.SendMessage("You don't have enough points available!  This attribute costs "+cost+" points.");
                    }

                    m_From.SendGump(new ItemExperienceGump(m_From, m_Item, LevelAttributes.m_ResistanceTypes[index].m_Category, GumpPage.AttributeList));

                    break;
                }
                case 6: // Jewelry & Clothing Resists Selected
                {
					cost = GetPointCost(m_Item, LevelAttributes.m_ElementAttributes[index].m_XP);
                    if ((levitem.Points - cost) >= 0)
                    {
                        //add point to selected weapon attribute
                        if (index >= 0 && index < LevelAttributes.m_ElementAttributes.Length)
                        {
							if (m_Item is BaseJewel)
							{
								attrvalue = ((BaseJewel)m_Item).Resistances[LevelAttributes.m_ElementAttributes[index].m_Attribute];
								if (attrvalue < LevelAttributes.m_ElementAttributes[index].m_MaxValue)
								{
                            		((BaseJewel)m_Item).Resistances[LevelAttributes.m_ElementAttributes[index].m_Attribute] += 1;
                            		levitem.Points -= cost;
								}
							}
							else
							{
								attrvalue = ((BaseClothing)m_Item).Resistances[LevelAttributes.m_ElementAttributes[index].m_Attribute];
								if (attrvalue < LevelAttributes.m_ElementAttributes[index].m_MaxValue)
								{
                            		((BaseClothing)m_Item).Resistances[LevelAttributes.m_ElementAttributes[index].m_Attribute] += 1;
                            		levitem.Points -= cost;
								}
							}
                        }
                    }
                    else
                    {
                        m_From.SendMessage("You don't have enough points available!  This attribute costs "+cost+" points.");
                    }

                    m_From.SendGump(new ItemExperienceGump(m_From, m_Item, LevelAttributes.m_ElementAttributes[index].m_Category, GumpPage.AttributeList));

                    break;
                }
			}
		}
    }
    #endregion
}