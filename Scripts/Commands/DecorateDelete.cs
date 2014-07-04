/****************************************************
* Scripted by Callandor2k                           *
* in his "RunUO with Mondain's Legacy" package r133 *
* Updated by Nerun in Engine r114                   *
*****************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Engines.Quests.Haven;
using Server.Engines.Quests.Necro;
using Server.Items;

namespace Server.Commands
{
	public class DecorateDelete
	{
		public static void Initialize()
		{
			CommandSystem.Register( "DecorateDelete", AccessLevel.Administrator, new CommandEventHandler( DecorateDelete_OnCommand ) );
			CommandSystem.Register( "DecoDel", AccessLevel.Administrator, new CommandEventHandler( DecorateDelete_OnCommand ));
		}

		[Usage( "DecorateDelete" )]
		[Aliases( "DecoDel" )]
		[Description( "Deletes world decoration." )]
		private static void DecorateDelete_OnCommand( CommandEventArgs e )
		{
			m_Mobile = e.Mobile;
			m_Count = 0;
			
			DateTime aTime = DateTime.Now;

			m_Mobile.SendMessage( "Deleting world decoration, please wait." );

			Remove( "Data/Decoration/Britannia", Map.Trammel, Map.Felucca );
			Remove( "Data/Decoration/Trammel", Map.Trammel );
			Remove( "Data/Decoration/Felucca", Map.Felucca );
			Remove( "Data/Decoration/Ilshenar", Map.Ilshenar );
			Remove( "Data/Decoration/Malas", Map.Malas );
			Remove( "Data/Decoration/Tokuno", Map.Tokuno );
			
			DateTime bTime = DateTime.Now;

			m_Mobile.SendMessage( "Deleting complete. {0} items were deleted in {1} seconds.", m_Count, (bTime - aTime).TotalSeconds );
		}

		public static void Remove( string folder, params Map[] maps )
		{
			if ( !Directory.Exists( folder ) )
				return;
				
			string[] files = Directory.GetFiles( folder, "*.cfg" );

			RemoveGeneric( files, maps );
		}

		public static void RemoveOne( string folder, string filename, params Map[] maps )
		{
			if ( !Directory.Exists( folder ) )
				return;
				
			string[] files = Directory.GetFiles( folder, filename );
			
			RemoveGeneric( files, maps );
		}
	
		private static void RemoveGeneric( string[] files, params Map[] maps )
		{
			for ( int i = 0; i < files.Length; ++i )
			{
				ArrayList list = DecorationListDelete.ReadAll( files[i] );

				#region Mondain's Legacy
				m_List = list;
				#endregion

				for ( int j = 0; j < list.Count; ++j )
					m_Count += ((DecorationListDelete)list[j]).Remove( maps );
			}
		}

		#region Mondain's Legacy
		public static Item FindByID(int id)
		{
			if (m_List == null)
				return null;

			for (int j = 0; j < m_List.Count; ++j)
			{
				DecorationListDelete list = (DecorationListDelete)m_List[j];

				if (list.ID == id)
					return list.Constructed;
			}

			return null;
		}

		private static ArrayList m_List;
		#endregion

		private static Mobile m_Mobile;
		private static int m_Count;
	}

	public class DecorationListDelete
	{
		private Type m_Type;
		private int m_ItemID;
		private string[] m_Params;
		private ArrayList m_Entries;

		#region Mondain's Legacy
		private Item m_Constructed;

		public Item Constructed { get { return m_Constructed; } }

		public int ID
		{
			get
			{
				for (int i = 0; i < m_Params.Length; ++i)
				{
					if (m_Params[i].StartsWith("ID"))
					{
						int indexOf = m_Params[i].IndexOf('=');

						if (indexOf >= 0)
							return Utility.ToInt32(m_Params[i].Substring(++indexOf));
					}
				}

				return 0;
			}
		}
		#endregion

		public DecorationListDelete()
		{
		}

		private static Type typeofStatic = typeof( Static );
		private static Type typeofLocalizedStatic = typeof( LocalizedStatic );
		private static Type typeofBaseDoor = typeof( BaseDoor );
		private static Type typeofAnkhWest = typeof( AnkhWest );
		private static Type typeofAnkhNorth = typeof( AnkhNorth );
		private static Type typeofBeverage = typeof( BaseBeverage );
		private static Type typeofLocalizedSign = typeof( LocalizedSign );
		private static Type typeofMarkContainer = typeof( MarkContainer );
		private static Type typeofWarningItem = typeof( WarningItem );
		private static Type typeofHintItem = typeof( HintItem );
		private static Type typeofCannon = typeof( Cannon );
		private static Type typeofSerpentPillar = typeof( SerpentPillar );

		public Item Construct()
		{
			Item item;

			try
			{
				if ( m_Type == typeofStatic )
				{
					item = new Static( m_ItemID );
				}
				else if ( m_Type == typeofLocalizedStatic )
				{
					int labelNumber = 0;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "LabelNumber" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
							{
								labelNumber = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
								break;
							}
						}
					}

					item = new LocalizedStatic( m_ItemID, labelNumber );
				}
				else if ( m_Type == typeofLocalizedSign )
				{
					int labelNumber = 0;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "LabelNumber" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
							{
								labelNumber = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
								break;
							}
						}
					}

					item = new LocalizedSign( m_ItemID, labelNumber );
				}
				else if ( m_Type == typeofAnkhWest || m_Type == typeofAnkhNorth )
				{
					bool bloodied = false;

					for ( int i = 0; !bloodied && i < m_Params.Length; ++i )
						bloodied = ( m_Params[i] == "Bloodied" );

					if ( m_Type == typeofAnkhWest )
						item = new AnkhWest( bloodied );
					else
						item = new AnkhNorth( bloodied );
				}
				else if ( m_Type == typeofMarkContainer )
				{
					bool bone = false;
					bool locked = false;
					Map map = Map.Malas;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i] == "Bone" )
						{
							bone = true;
						}
						else if ( m_Params[i] == "Locked" )
						{
							locked = true;
						}
						else if ( m_Params[i].StartsWith( "TargetMap" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								map = Map.Parse( m_Params[i].Substring( ++indexOf ) );
						}
					}

					MarkContainer mc = new MarkContainer( bone, locked );

					mc.TargetMap = map;
					mc.Description = "strange location";

					item = mc;
				}
				else if ( m_Type == typeofHintItem )
				{
					int range = 0;
					int messageNumber = 0;
					string messageString = null;
					int hintNumber = 0;
					string hintString = null;
					TimeSpan resetDelay = TimeSpan.Zero;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "Range" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								range = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
						}
						else if ( m_Params[i].StartsWith( "WarningString" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								messageString = m_Params[i].Substring( ++indexOf );
						}
						else if ( m_Params[i].StartsWith( "WarningNumber" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								messageNumber = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
						}
						else if ( m_Params[i].StartsWith( "HintString" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								hintString = m_Params[i].Substring( ++indexOf );
						}
						else if ( m_Params[i].StartsWith( "HintNumber" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								hintNumber = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
						}
						else if ( m_Params[i].StartsWith( "ResetDelay" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								resetDelay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
						}
					}

					HintItem hi = new HintItem( m_ItemID, range, messageNumber, hintNumber );

					hi.WarningString = messageString;
					hi.HintString = hintString;
					hi.ResetDelay = resetDelay;

					item = hi;
				}
				else if ( m_Type == typeofWarningItem )
				{
					int range = 0;
					int messageNumber = 0;
					string messageString = null;
					TimeSpan resetDelay = TimeSpan.Zero;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "Range" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								range = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
						}
						else if ( m_Params[i].StartsWith( "WarningString" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								messageString = m_Params[i].Substring( ++indexOf );
						}
						else if ( m_Params[i].StartsWith( "WarningNumber" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								messageNumber = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
						}
						else if ( m_Params[i].StartsWith( "ResetDelay" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								resetDelay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
						}
					}

					WarningItem wi = new WarningItem( m_ItemID, range, messageNumber );

					wi.WarningString = messageString;
					wi.ResetDelay = resetDelay;

					item = wi;
				}
				else if ( m_Type == typeofCannon )
				{
					CannonDirection direction = CannonDirection.North;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "CannonDirection" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								direction = (CannonDirection)Enum.Parse( typeof( CannonDirection ), m_Params[i].Substring( ++indexOf ), true );
						}
					}

					item = new Cannon( direction );
				}
				else if ( m_Type == typeofSerpentPillar )
				{
					string word = null;
					Rectangle2D destination = new Rectangle2D();

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "Word" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								word = m_Params[i].Substring( ++indexOf );
						}
						else if ( m_Params[i].StartsWith( "DestStart" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								destination.Start = Point2D.Parse( m_Params[i].Substring( ++indexOf ) );
						}
						else if ( m_Params[i].StartsWith( "DestEnd" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								destination.End = Point2D.Parse( m_Params[i].Substring( ++indexOf ) );
						}
					}

					item = new SerpentPillar( word, destination );
				}
				else if ( m_Type.IsSubclassOf( typeofBeverage ) )
				{
					BeverageType content = BeverageType.Liquor;
					bool fill = false;

					for ( int i = 0; !fill && i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "Content" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
							{
								content = (BeverageType)Enum.Parse( typeof( BeverageType ), m_Params[i].Substring( ++indexOf ), true );
								fill = true;
							}
						}
					}

					if ( fill )
						item = (Item)Activator.CreateInstance( m_Type, new object[]{ content } );
					else
						item = (Item)Activator.CreateInstance( m_Type );
				}
				else if ( m_Type.IsSubclassOf( typeofBaseDoor ) )
				{
					DoorFacing facing = DoorFacing.WestCW;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "Facing" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
							{
								facing = (DoorFacing)Enum.Parse( typeof( DoorFacing ), m_Params[i].Substring( ++indexOf ), true );
								break;
							}
						}
					}

					item = (Item)Activator.CreateInstance( m_Type, new object[]{ facing } );
				}
				else
				{
					item = (Item)Activator.CreateInstance( m_Type );
				}
			}
			catch ( Exception e )
			{
				throw new Exception( String.Format( "Bad type: {0}", m_Type ), e );
			}

			if ( item is BaseAddon )
			{
				if ( item is MaabusCoffin )
				{
					MaabusCoffin coffin = (MaabusCoffin)item;

					for ( int i = 0; i < m_Params.Length; ++i )
					{
						if ( m_Params[i].StartsWith( "SpawnLocation" ) )
						{
							int indexOf = m_Params[i].IndexOf( '=' );

							if ( indexOf >= 0 )
								coffin.SpawnLocation = Point3D.Parse( m_Params[i].Substring( ++indexOf ) );
						}
					}
				}
				else if ( m_ItemID > 0 )
				{
					List<AddonComponent> comps = ((BaseAddon)item).Components;

					for ( int i = 0; i < comps.Count; ++i )
					{
						AddonComponent comp = (AddonComponent)comps[i];

						if ( comp.Offset == Point3D.Zero )
							comp.ItemID = m_ItemID;
					}
				}
			}
			else if ( item is BaseLight )
			{
				bool unlit = false, unprotected = false;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( !unlit && m_Params[i] == "Unlit" )
						unlit = true;
					else if ( !unprotected && m_Params[i] == "Unprotected" )
						unprotected = true;
					
					if ( unlit && unprotected )
						break;
				}

				if ( !unlit )
					((BaseLight)item).Ignite();
				if ( !unprotected )
					((BaseLight)item).Protected = true;

				if ( m_ItemID > 0 )
					item.ItemID = m_ItemID;
			}
			else if ( item is Server.Mobiles.Spawner )
			{
				Server.Mobiles.Spawner sp = (Server.Mobiles.Spawner)item;

				sp.NextSpawn = TimeSpan.Zero;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( m_Params[i].StartsWith( "Spawn" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.SpawnNames.Add( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "MinDelay" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.MinDelay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "MaxDelay" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.MaxDelay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "NextSpawn" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.NextSpawn = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Count" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.Count = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Team" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.Team = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "HomeRange" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.HomeRange = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Running" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.Running = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Group" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							sp.Group = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
				}
			}
			else if ( item is RecallRune )
			{
				RecallRune rune = (RecallRune)item;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( m_Params[i].StartsWith( "Description" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							rune.Description = m_Params[i].Substring( ++indexOf );
					}
					else if ( m_Params[i].StartsWith( "Marked" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							rune.Marked = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "TargetMap" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							rune.TargetMap = Map.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Target" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							rune.Target = Point3D.Parse( m_Params[i].Substring( ++indexOf ) );
					}
				}
			}
			else if ( item is SkillTeleporter )
			{
				SkillTeleporter tp = (SkillTeleporter)item;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( m_Params[i].StartsWith( "Skill" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Skill = (SkillName)Enum.Parse( typeof( SkillName ), m_Params[i].Substring( ++indexOf ), true );
					}
					else if ( m_Params[i].StartsWith( "RequiredFixedPoint" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Required = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) ) * 0.01;
					}
					else if ( m_Params[i].StartsWith( "Required" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Required = Utility.ToDouble( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "MessageString" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.MessageString = m_Params[i].Substring( ++indexOf );
					}
					else if ( m_Params[i].StartsWith( "MessageNumber" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.MessageNumber = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "PointDest" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.PointDest = Point3D.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "MapDest" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.MapDest = Map.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Creatures" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Creatures = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "SourceEffect" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.SourceEffect = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "DestEffect" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.DestEffect = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "SoundID" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.SoundID = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Delay" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Delay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
					}
				}

				if ( m_ItemID > 0 )
					item.ItemID = m_ItemID;
			}
			else if ( item is KeywordTeleporter )
			{
				KeywordTeleporter tp = (KeywordTeleporter)item;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( m_Params[i].StartsWith( "Substring" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Substring = m_Params[i].Substring( ++indexOf );
					}
					else if ( m_Params[i].StartsWith( "Keyword" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Keyword = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Range" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Range = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "PointDest" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.PointDest = Point3D.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "MapDest" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.MapDest = Map.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Creatures" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Creatures = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "SourceEffect" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.SourceEffect = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "DestEffect" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.DestEffect = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "SoundID" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.SoundID = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Delay" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Delay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
					}
				}

				if ( m_ItemID > 0 )
					item.ItemID = m_ItemID;
			}
			else if ( item is Teleporter )
			{
				Teleporter tp = (Teleporter)item;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( m_Params[i].StartsWith( "PointDest" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.PointDest = Point3D.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "MapDest" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.MapDest = Map.Parse( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Creatures" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Creatures = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "SourceEffect" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.SourceEffect = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "DestEffect" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.DestEffect = Utility.ToBoolean( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "SoundID" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.SoundID = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
					}
					else if ( m_Params[i].StartsWith( "Delay" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							tp.Delay = TimeSpan.Parse( m_Params[i].Substring( ++indexOf ) );
					}
				}

				if ( m_ItemID > 0 )
					item.ItemID = m_ItemID;
			}
			else if ( item is FillableContainer )
			{
				FillableContainer cont = (FillableContainer) item;

				for ( int i = 0; i < m_Params.Length; ++i )
				{
					if ( m_Params[i].StartsWith( "ContentType" ) )
					{
						int indexOf = m_Params[i].IndexOf( '=' );

						if ( indexOf >= 0 )
							cont.ContentType = (FillableContentType)Enum.Parse( typeof( FillableContentType ), m_Params[i].Substring( ++indexOf ), true );
					}
				}

				if ( m_ItemID > 0 )
					item.ItemID = m_ItemID;
			}
			else if ( m_ItemID > 0 )
			{
				item.ItemID = m_ItemID;
			}

			item.Movable = false;

			for ( int i = 0; i < m_Params.Length; ++i )
			{
				if ( m_Params[i].StartsWith( "Light" ) )
				{
					int indexOf = m_Params[i].IndexOf( '=' );

					if ( indexOf >= 0 )
						item.Light = (LightType)Enum.Parse( typeof( LightType ), m_Params[i].Substring( ++indexOf ), true );
				}
				else if ( m_Params[i].StartsWith( "Hue" ) )
				{
					int indexOf = m_Params[i].IndexOf( '=' );

					if ( indexOf >= 0 )
					{
						int hue = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );

						if ( item is DyeTub )
							((DyeTub)item).DyedHue = hue;
						else
							item.Hue = hue;
					}
				}
				else if ( m_Params[i].StartsWith( "Name" ) )
				{
					int indexOf = m_Params[i].IndexOf( '=' );

					if ( indexOf >= 0 )
						item.Name = m_Params[i].Substring( ++indexOf );
				}
				else if ( m_Params[i].StartsWith( "Amount" ) )
				{
					int indexOf = m_Params[i].IndexOf( '=' );

					if ( indexOf >= 0 )
					{
						// Must supress stackable warnings

						bool wasStackable = item.Stackable;

						item.Stackable = true;
						item.Amount = Utility.ToInt32( m_Params[i].Substring( ++indexOf ) );
						item.Stackable = wasStackable;
					}
				}
			}

			return item;
		}

		private static Queue<Item> m_DeleteQueue = new Queue<Item>();

		private static bool FindItemDelete(int x, int y, int z, Map map, Item srcItem)
		{
			int itemID = srcItem.ItemID;

			bool res = false;

			IPooledEnumerable eable;

			if ( srcItem is BaseDoor )
			{
				eable = map.GetItemsInRange( new Point3D(x, y, z), 1 );

				foreach ( Item item in eable )
				{
					if ( !(item is BaseDoor) )
						continue;

					BaseDoor bd = (BaseDoor)item;
					Point3D p;
					int bdItemID;

					if ( bd.Open )
					{
						p = new Point3D( bd.X - bd.Offset.X, bd.Y - bd.Offset.Y, bd.Z - bd.Offset.Z );
						bdItemID = bd.ClosedID;
					}
					else
					{
						p = bd.Location;
						bdItemID = bd.ItemID;
					}

					if ( p.X != x || p.Y != y )
						continue;

					if (item.Z == z && bdItemID == itemID)
					{
						m_DeleteQueue.Enqueue(item);
						res = true;
					}
					/*else if (Math.Abs(item.Z - z) < 8)
						m_DeleteQueue.Enqueue(item);*/
				}
			}
			else if ( (TileData.ItemTable[itemID & 0x7FFF].Flags & TileFlag.LightSource) != 0 )
			{
				eable = map.GetItemsInRange( new Point3D( x, y, z ), 0 );

				LightType lt = srcItem.Light;
				string srcName = srcItem.ItemData.Name;

				foreach ( Item item in eable )
				{
					if ( item.Z == z )
					{
						if ( item.ItemID == itemID )
						{
							/*if ( item.Light != lt )
								m_DeleteQueue.Enqueue( item );
							else*/
							res = true;
							m_DeleteQueue.Enqueue(item);
						}
						else if ( (item.ItemData.Flags & TileFlag.LightSource) != 0 && item.ItemData.Name == srcName )
						{
							//m_DeleteQueue.Enqueue( item );
						}
					}
				}
			}
			else if ( srcItem is Teleporter || srcItem is FillableContainer || srcItem is BaseBook )
			{
				eable = map.GetItemsInRange( new Point3D( x, y, z ), 0 );

				Type type = srcItem.GetType();

				foreach ( Item item in eable )
				{
					if ( item.Z == z && item.ItemID == itemID )
					{
						if (item.GetType() != type)
						{
							//m_DeleteQueue.Enqueue(item);
						}
						else
						{
							m_DeleteQueue.Enqueue(item);
							res = true;
						}
					}
				}
			}
			else
			{
				eable = map.GetItemsInRange( new Point3D( x, y, z ), 0 );

				foreach ( Item item in eable )
				{
					if ( item.Z == z && item.ItemID == itemID )
					{
						m_DeleteQueue.Enqueue(item);
						res = true;
						//eable.Free();
						//return true;
					}
				}
			}

			eable.Free();

			while ( m_DeleteQueue.Count > 0 )
				m_DeleteQueue.Dequeue().Delete();

			return res;
		}

		public int Remove( Map[] maps )
		{
			int count = 0;

			Item item = null;

			for ( int i = 0; i < m_Entries.Count; ++i )
			{
				DecorationEntry entry = (DecorationEntry)m_Entries[i];
				Point3D loc = entry.Location;
				string extra = entry.Extra;

				for ( int j = 0; j < maps.Length; ++j )
				{
					if ( item == null )
						item = Construct();

					#region Mondain's Legacy
					m_Constructed = item;
					#endregion

					if ( item == null )
						continue;

					if (FindItemDelete( loc.X, loc.Y, loc.Z, maps[j], item ))
					{
						++count;
					}
				}
			}

			if ( item != null )
				item.Delete();

			return count;
		}

		public static ArrayList ReadAll( string path )
		{
			using ( StreamReader ip = new StreamReader( path ) )
			{
				ArrayList list = new ArrayList();

				for ( DecorationListDelete v = Read( ip ); v != null; v = Read( ip ) )
					list.Add( v );

				return list;
			}
		}

		private static string[] m_EmptyParams = new string[0];

		public static DecorationListDelete Read( StreamReader ip )
		{
			string line;

			while ( (line = ip.ReadLine()) != null )
			{
				line = line.Trim();

				if ( line.Length > 0 && !line.StartsWith( "#" ) )
					break;
			}

			if ( string.IsNullOrEmpty( line ) )
				return null;

			DecorationListDelete list = new DecorationListDelete();

			int indexOf = line.IndexOf( ' ' );

			list.m_Type = ScriptCompiler.FindTypeByName( line.Substring( 0, indexOf++ ), true );

			if ( list.m_Type == null )
				throw new ArgumentException( String.Format( "Type not found for header: '{0}'", line ) );

			line = line.Substring( indexOf );
			indexOf = line.IndexOf( '(' );
			if ( indexOf >= 0 )
			{
				list.m_ItemID = Utility.ToInt32( line.Substring( 0, indexOf - 1 ) );

				string parms = line.Substring( ++indexOf );

				if ( line.EndsWith( ")" ) )
					parms = parms.Substring( 0, parms.Length - 1 );

				list.m_Params = parms.Split( ';' );

				for ( int i = 0; i < list.m_Params.Length; ++i )
					list.m_Params[i] = list.m_Params[i].Trim();
			}
			else
			{
				list.m_ItemID = Utility.ToInt32( line );
				list.m_Params = m_EmptyParams;
			}

			list.m_Entries = new ArrayList();

			while ( (line = ip.ReadLine()) != null )
			{
				line = line.Trim();

				if ( line.Length == 0 )
					break;

				if ( line.StartsWith( "#" ) )
					continue;

				list.m_Entries.Add( new DecorationEntry( line ) );
			}

			return list;
		}
	}
}