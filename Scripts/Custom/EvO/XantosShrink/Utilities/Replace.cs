#region AuthorHeader
//
//	Replace version 1.0 - utilities version 2.0, by Xanthos
//
//
#endregion AuthorHeader                                                                                                                                                                                                                                                               
using System;
using System.Collections;
using System.Reflection;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.IO;
using System.Xml;
using Server.Commands;
using Server.Commands.Generic;

namespace Xanthos.Utilities
{
	public class PickReplacementTarget : Target
	{
		private Item m_ItemToReplace;
		private bool m_CopyDeep;

		public static void Initialize()
		{
			CommandHandlers.Register( "ReplaceItem", AccessLevel.GameMaster, new CommandEventHandler( ReplaceItem_OnCommand ) );
//			CommandHandlers.Register( "UpgradeShrinkSystem", AccessLevel.GameMaster, new CommandEventHandler( UpgradeShrinkSystem ) );
		}

		public PickReplacementTarget( object o, bool copyDeep ) : base( -1, false, TargetFlags.None )
		{
			m_ItemToReplace = o as Item;
			m_CopyDeep = copyDeep;
		}

		[Usage( "ReplaceItem [-d]" )]
		[Description( "Replaces an item with another copying properties of the replaced to the replacement." )]
		private static void ReplaceItem_OnCommand( CommandEventArgs e )
		{
			bool copyDeep = false;

			if ( e.Length == 1 && e.GetString( 0 ).Equals( "-d" ))
				copyDeep = true;

			e.Mobile.Target = new ReplaceItemTarget( copyDeep );
			e.Mobile.SendMessage( "Choose an Item to replace." );
		}

		[Usage( "UpdateShrinkSystem" )]
		[Description( "Updates the Items in the shrink system." )]
		public static void UpgradeShrinkSystem( CommandEventArgs e )
		{
//			ReplaceType( "Xanthos.ShrinkSystem.PetLeash", "Xanthos.Evo.PetLeash", false );
//			ReplaceType( "Xanthos.ShrinkSystem.HitchingPost", "Xanthos.Evo.HitchingPost", false );
//			ReplaceType( "Xanthos.ShrinkSystem.ShrinkItem", "Xanthos.Evo.ShrinkItem", true );
		}

		protected override void OnTarget( Mobile from, object replacement )
		{
			if ( !( replacement is Item ))
				from.SendMessage( "You cannot use that as a replacement!" );
			else
				ReplaceItem( replacement as Item, m_ItemToReplace, m_CopyDeep );
		}

		private static void CopyPropertiesShallow( Item dest, Item src ) 
		{
			dest.IsLockedDown = src.IsLockedDown;
			dest.IsSecure = src.IsSecure;
			dest.Movable = src.Movable;
			dest.Insured = src.Insured;
			dest.LootType = src.LootType;
			dest.BlessedFor = src.BlessedFor;
			dest.Name = src.Name;
		}

		private static void CopyProperties( Item dest, Item src ) 
		{
			PropertyInfo[] props = src.GetType().GetProperties();

			//Console.WriteLine( "Copying properties..." );
			for ( int i = 0; i < props.Length; i++ ) 
			{ 
				try
				{
					if ( props[i].CanRead && props[i].CanWrite )
					{
						//Console.WriteLine( "Setting {0} = {1}", props[i].Name, props[i].GetValue( src, null ) );
						props[i].SetValue( dest, props[i].GetValue( src, null ), null ); 
					}
				}
				catch
				{
					Console.WriteLine( "Exception copying properties for {0}", props[i].Name );
				}
			}
			// Must do this due to a bug in impl of Newbied, which comes after loottype
			// in the props list, overwriting loottype.
			dest.LootType = src.LootType;
		}

		//
		// Search through all of the Items for those with Type oldTypeName
		// and create Items of newTypeName passing the two Items to ReplaceItem()
		//
		private static void ReplaceType( string newTypeName, string oldTypeName, bool copyDeep )
		{
			Type oldType = null;
			Type newType = null;

			try
			{
				oldType = Type.GetType( oldTypeName );
				newType = Type.GetType( newTypeName );
			}
			catch ( Exception e )
			{
				Console.WriteLine( "Exception getting types {0}", e.Message );
			}

			if ( oldType == null )
				Console.WriteLine( "No class {0} installed", oldTypeName );
			else if ( newType == null )
				Console.WriteLine( "No class {0} installed", newTypeName );
			else
			{
				Type[] types = new Type[0];
				ConstructorInfo ci = newType.GetConstructor( types );

				if ( ci != null )
				{
					ArrayList array = new ArrayList();

					Console.WriteLine( "Looking for {0}s", oldTypeName );
					foreach( Item item in World.Items.Values )
					{
						if ( item.GetType() == oldType )
							array.Add( item );
					}

					Console.WriteLine( "Found {0} {1}s - replacing with {2}s", array.Count, oldTypeName, newTypeName );

					for ( int j = 0; j < array.Count; j++ )
					{
						Item item = array[j] as Item;

						if ( item != null && item.GetType() == oldType )
						{
							Object obj = ci.Invoke( null );

							if ( obj == null )
								continue;
							
							ReplaceItem( (Item)obj, item, copyDeep );	// This does the actual work of replacement
						}
					}
					Console.WriteLine( "Replaced {0} {1} with {2}", array.Count, oldTypeName, newTypeName );
				}
			}
		}

		//
		// Given two Items, copies the properties of the first to the second
		// then places the second one where the first is and deletes the first.
		//
		public static void ReplaceItem( Item replacement, Item itemToReplace, bool copyDeep )
		{
			Container pack;

			if ( null != itemToReplace && !itemToReplace.Deleted )
			{
				if ( copyDeep )
					CopyProperties( replacement, itemToReplace );
				else
					CopyPropertiesShallow( replacement, itemToReplace );

				if ( itemToReplace.Parent is Container )
					pack = (Container)itemToReplace.Parent;
				else if ( itemToReplace.Parent is Mobile )
					pack = ((Mobile)itemToReplace.Parent).Backpack;
				else
					pack = null;

				replacement.Parent = null;

				if ( pack != null )
					pack.DropItem( replacement );
				else
					replacement.MoveToWorld( itemToReplace.Location, itemToReplace.Map );

				itemToReplace.Delete();
			}
		}
	}

	public class ReplaceItemTarget : Target
	{
		bool m_CopyDeep;

		public ReplaceItemTarget( bool copyDeep ) : base( -1, false, TargetFlags.None )
		{
			m_CopyDeep = copyDeep;
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( o is Item )
			{
				from.Target = new PickReplacementTarget( o, m_CopyDeep );
				from.SendMessage( "Choose a replacement." );
			}
			else
				from.SendMessage( "You cannot replace that!" );
		}
	}
}



