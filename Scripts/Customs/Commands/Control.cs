/*********************************************
Version: 2.0.2

Changes:
2.0.2 -- by fcondon aka Exale
  - Fixed errors with modern servers.
2.0.1:
  - Skills will be saved correct
2.0.0:
  - Compatible with RunUO 2.0 RC1
1.0.1:
	- Added "private static object Construct(...)" to "class ControlCommand"
	- Following command arguments added: NoStats, NoSkills, NoItems

-- Please do not edit this Header --

Scripted by Quick_silver
for www.Welt-von-Midgrd.ch
and everyone who want to use it.

Please note me if you make Changes in the Scripts.

|*****Instalation*****>
Notice strings in engish are commented in the code, so you can replace the germans.

To uncontrol at death add the following lines to the PlayerMobbile.cs or edit the OnBeforeDeath() function.

	public override bool OnBeforeDeath()
	{
		if (Server.Commands.ControlCommand.UncontrolDeath( (Mobile)this )) 
		{
		  return base.OnBeforeDeath();
		}
		else {
		  return false; 
		}
	}
	
And now: Have fun...

*********************************************/

using System;
using System.Collections;
using System.Reflection;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Commands;

namespace Server.Commands
{
	public class ControlCommand
	{
		public static AccessLevel accessLevel = AccessLevel.Counselor;

		private static Layer[] m_DesiredLayerOrder = new Layer[]
		{
			Layer.Cloak,
			Layer.Bracelet,
			Layer.Ring,
			Layer.Shirt,
			Layer.Pants,
			Layer.InnerLegs,
			Layer.Shoes,
			Layer.Arms,
			Layer.InnerTorso,
			Layer.MiddleTorso,
			Layer.OuterLegs,
			Layer.Neck,
			Layer.Waist,
			Layer.Gloves,
			Layer.OuterTorso,
			Layer.OneHanded,
			Layer.TwoHanded,
			Layer.FacialHair,
			Layer.Hair,
			Layer.Helm
		};

		public static void Initialize()
		{
			CommandSystem.Register( "Control", accessLevel, new CommandEventHandler( Control_OnCommand ) );
		}
	
		[Usage( "Control [target]" )]
		[Description( "Let you control a NPC." )]
		private static void Control_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			e.GetInt32( 0 );
			
			if ( from != null ) 
			{
				from.SendMessage( "Choose the target to control..." );
				
				from.Target = new InternalTarget( e.Arguments );
			}
		}

		private class InternalTarget : Target
		{
			string[] m_parameter;

			public InternalTarget( params string[] parameter ) : base( -1, true, TargetFlags.None )
			{
				m_parameter = parameter;
			}
			
			protected override void OnTarget( Mobile from, object targeted )
			{
				bool stats = true;
				bool skills = true;
				bool items = true;
				
				for (int i=0; i<m_parameter.Length; i++)
					if (string.Compare(m_parameter[i], "NoStats", true) == 0)
						stats = false;
						
				for (int i=0; i<m_parameter.Length; i++)
					if (string.Compare(m_parameter[i], "NoSkills", true) == 0)
						skills = false;
						
				for (int i=0; i<m_parameter.Length; i++)
					if (string.Compare(m_parameter[i], "NoItems", true) == 0)
						items = false;
					
				
				DoControl( from, targeted, stats, skills, items );
			}
		}
		
		/*Find the Control item of the Mobile from*/
		public static ControlItem GetControlItem( Mobile from )
		{
			Item result = SearchItemInCont( typeof(ControlItem), from.Backpack);
				
			if ( result != null && result is ControlItem )
				return (ControlItem)result;
			else 
				return null;
		}
		
		private static Item SearchItemInCont( Type targetType, Container cont )
		{
			Item item;
			
			if( cont != null && !cont.Deleted )
			{
				for (int i = 0; i < cont.Items.Count;i++)
				{
					item = (Item)cont.Items[i];
					// recursively search containers
					if( item != null && !item.Deleted)
					{
						if ( item.GetType() == targetType )
							return item;
						else if ( item is Container )
							item = SearchItemInCont(targetType, (Container)item);
							
						if ( item != null && item.GetType() == targetType )
							return item;
					}
				}
			}
				
			return null;
		}
		
		public static void DoControl( Mobile from, object targeted, bool stats, bool skills, bool items )
		{
			Mobile target;

			if ( from is PlayerMobile && targeted is Mobile)
			{
				if ( targeted is PlayerMobile && ((PlayerMobile)targeted).Player )
				{
					from.SendMessage("You cant control players");
					return;
				}
				
				target = (Mobile)targeted;	
				ControlItem controlItem = GetControlItem(from);

				if ( controlItem == null )
				{	
					from.SendMessage("Stats: {0} Skills: {1} Items: {2}", stats, skills, items);
					StartControl(from, target, stats, skills, items);
				}
				else
				{	
					from.SendMessage("Stats: {0} Skills: {1} Items: {2}", controlItem.Stats, controlItem.Skills, controlItem.Items);
					ChangeControl(target, controlItem, controlItem.Stats, controlItem.Skills, controlItem.Items);
				}
			}
			else if ( from is PlayerMobile && targeted is ControlItem )
				((Item)targeted).Delete();
		}
		
		
		private static void StartControl( Mobile from, Mobile target, bool stats, bool skills, bool items )
		{
			from.SendMessage("You leave your Body an control {0}, {1}", target.Name, target.Title);
			
			//Clone Player
			PlayerMobile playerClone = (PlayerMobile)DupeMobile(from);
								
			//Create ControlItem
			ControlItem controlItem = new ControlItem(from, playerClone, target, stats, skills, items);
			from.Backpack.DropItem(controlItem);
					
			//Props target -> player
			CopyProps(from, target, stats, skills);
			
			//Backup Equip
			MoveEquip(from, playerClone, items);
			//Equip from target to player
			MoveEquip(target, from, items);
			
			
					
			target.Internalize();
			playerClone.Internalize();

		}
		

		private static void ChangeControl( Mobile target, ControlItem controlItem, bool stats, bool skills, bool items )
		{
			Mobile from							= controlItem.Owner;
			PlayerMobile oldPlayer 	= controlItem.Player;
			Mobile oldNPC 				 	= controlItem.NPC;
			
			if ( oldNPC != null )
			{
				//NPC Wiederherstellen
				if ( !oldNPC.Deleted )
				{
					//Props immer übernehmen bei der Rückverwandlung?
					//ja, weil sich hits etc ändern
					//Props from -> oldNPC
					CopyProps( oldNPC, from, stats, skills );
					//nicht nur zurück holen
					//if ( oldNPC.Map == Map.Internal )
					//	oldNPC.MoveToWorld(from.Location, from.Map);
					
					//Equip: from -> oldNPC
					MoveEquip(from, oldNPC, items);
				}
				else
				{
					from.SendMessage("Der originale NPC wurde gelöscht und wird nicht wiederhergestellt. Grund könnte ein manueller Respawn gewesen sein");
					oldNPC.Delete();
				}
			}
			
			if ( target != oldPlayer && target != null && !target.Deleted )
			{
				from.SendMessage("You Control  {0}, {1}", target.Name, target.Title);

				//Update ControlItem
				controlItem.NPC = target;
				controlItem.Stats = stats;
				controlItem.Skills = skills;
				controlItem.Items = items;
				
				//Props: target -> player
				CopyProps( from, target, stats, skills );
				//Equip: target -> player
				MoveEquip(target, from, items);
				
				target.Internalize();
			}
			else if ( target == oldPlayer && !target.Deleted )
			{
				controlItem.Delete();
			}
		}
		
		public static void EndControl( ControlItem controlItem, bool stats, bool skills, bool items )
		{
			Mobile from							= controlItem.Owner;
			PlayerMobile oldPlayer 	= controlItem.Player;
			Mobile oldNPC 				 	= controlItem.NPC;
			
			if ( from == null )
				return;
			
			from.SendMessage("You are in your original Body");
			
			if ( oldNPC != null && !oldNPC.Deleted )
			{
				//Props from -> oldNPC
				CopyProps( oldNPC, from, stats, skills );
				//if ( oldNPC.Map == Map.Internal )
				//	oldNPC.MoveToWorld(from.Location, from.Map);
				
				//Equip from -> oldNPC
				MoveEquip( from, oldNPC, items );
			}
			else
			{
				from.SendMessage("The original NPC was deleted. Maybe because a manual respawn");	
				oldNPC.Delete();
			}
			
			if ( oldPlayer != null && !oldPlayer.Deleted )
			{
				//Spieler Wiederherstellen (100%)
				//Props: oldPlayer -> player
				CopyProps( from, oldPlayer, true, true );
				//Equip: oldPlayer -> player
				MoveEquip( oldPlayer, from, true );
					
				oldPlayer.Delete();
			}
		}

    //Return true if the base.OnBeforeDeath should be executed and false if not.
		public static bool UncontrolDeath( Mobile from )
		{
			if ( from.AccessLevel < accessLevel )
				return true;
		
			ControlItem controlItem = GetControlItem(from);
			
			if ( controlItem != null )
			{
				//Backup NPC
				Mobile NPC = (Mobile)controlItem.NPC;
				
				//Release GM
				controlItem.Delete();
				from.Hits = from.HitsMax;
				from.Stam = from.StamMax;
				from.Mana = from.StamMax;
				
				//Kill NPC as normal
				NPC.Kill();
				
				return false; //GM stirbt nicht ;)
			}
			
			return true;
		}
		
		private static void MoveEquip( Mobile from, Mobile to, bool fromBackpack)
		{
			Item item;
			
			for ( int i = 0; i < m_DesiredLayerOrder.Length; ++i )
			{
				item = to.FindItemOnLayer( m_DesiredLayerOrder[i] );
				if ( item != null ) 
					item.Bounce(to);
	
				item = from.FindItemOnLayer( m_DesiredLayerOrder[i] );
				if( item != null )
				{
					to.EquipItem(item);
				}
			}
			
			//Backpack
			if ( from.Backpack != null && !(from.Backpack is VendorBackpack) && fromBackpack )
			{
				
				if ( to.Backpack == null )
					to.EquipItem( new Backpack() );
				
				ArrayList itemsToMove = new ArrayList();
				
				for (int i = 0; i < from.Backpack.Items.Count;++i)
				{
					item = (Item)from.Backpack.Items[i];

					if( item != null && !item.Deleted && item.LootType != LootType.Newbied && item.LootType != LootType.Blessed )
					{
						itemsToMove.Add( item );
					}
				}
				
				for ( int i = 0; i < itemsToMove.Count; ++i)
				{
					to.Backpack.DropItem((Item)itemsToMove[i]);
				}
				itemsToMove.Clear();
				
			}
		}
		
		//With items for DupeCommand?
		public static Mobile DupeMobile( object mobile )
		{
			Type t = mobile.GetType();
			object o = Construct(t);
			
			if (o == null)
			{
				Console.WriteLine("Unable to dupe {0}. Mobile must have a 0 parameter constructor.",t.Name);
				return null;
			}

			if (o is Mobile)
			{
				Mobile newMobile = (Mobile)o;
				Mobile srcMobile = (Mobile)mobile;
				//CopyProperties( o, mobile, t, "Parent", "NetState" );
				
				CopyProps(newMobile, srcMobile, true, true);
				
				//CopyProps didn't copy the AccessLevel, but we need it for some items the GM is wearing.
				newMobile.AccessLevel = srcMobile.AccessLevel;
				
				newMobile.Player = false;
				newMobile.UpdateTotals();
				return newMobile;
			}

			return null;
		}
		
		/*copy the poropertys from one Mobile to another*/
		private static void CopyProps(Mobile target, Mobile from, bool stats, bool skills)
		{
			try
			{
				if ( from.Map == Map.Internal )
					from.MoveToWorld(target.Location, target.Map);
				
				if ( stats )
					CopyMobileProps( target, from, "Parent", "NetState", "Player", "AccessLevel" );
				else
					CopyMobileProps( target, from, "Parent", "NetState", "Player", "AccessLevel", "RawStr", "Str", "RawDex", "Dex", "RawInt", "Int", "Hits", "Mana", "Stam" );
			  
				if ( skills )
				  //Console.WriteLine("Copy {2} Skills from {0} to {1}", from, target, target.Skills.Length);
					for ( int i = 0; i < target.Skills.Length; ++i )
					{
					  //Console.WriteLine("Skill {0} old Value = {1} new Value = {2}", i, target.Skills[i].Base, from.Skills[i].Base);
						target.Skills[i].Base = from.Skills[i].Base;
						
					}
			}
			catch
			{
				Console.WriteLine("Error in Control.cs -> CopyProps(Mobile from, Mobile target, bool stats, bool skills)");
				return;
			}
		}
		
		private static void CopyMobileProps( Mobile dest, Mobile src, params string[] omitProps )
		{
			//Type type = src.GetType(); didn't work correct
			
			Type type = typeof(Mobile);
			
			PropertyInfo[] props = type.GetProperties();

			bool omit = false;
			//Console.WriteLine("----- COPPY PROPS ------");
			//Console.WriteLine("From: {0} to {1}", src.Name, dest.Name);
			for ( int i = 0; i < props.Length; i++ )
			{
				try
				{
					for (int j=0; j<omitProps.Length; j++)
					{
						if (string.Compare(omitProps[j], props[i].Name, true) == 0)
						{
							omit = true;
							//Console.WriteLine("Skip Value {0} @ {1} = {2}", props[i].Name, dest.Name, props[i].GetValue( src, null )); 
							break;
						}
					}

					if ( props[i].CanRead && props[i].CanWrite && !omit)
					{
						//Setzte am Ziel 
						//Console.WriteLine("SetValue {0} @ {1} = {2}", props[i].Name, dest.Name, props[i].GetValue( src, null )); 
						//dest.SendMessage("SetValue {0}", props[i].Name); 
						props[i].SetValue( dest, props[i].GetValue( src, null ), null );
						//Console.WriteLine("-> {0}", props[i].GetValue( dest, null ));
					}
					
					omit = false; //Weiter kopieren
				}
				catch
				{
					Console.WriteLine( "Can't copy property: Control.cs" );
				}
			}
		}
		
		private static bool CompareType(object o, Type type)
		{
			if (o.GetType() == type || o.GetType().IsSubclassOf(type))
				return true;
			else
				return false;
		}
		
		/*Unused now*/
		private static void CopyProperties ( object dest, object src, Type type , params string[] omitProps )
		{
			if (!CompareType(dest,type) || !CompareType(src,type) || (dest.GetType() != src.GetType()) )
				return;

			PropertyInfo[] props = type.GetProperties();
			
			bool omit = false;
			for ( int i = 0; i < props.Length; i++ )
			{
				try
				{
					
					for (int j=0; j<omitProps.Length; j++)
						if (string.Compare(omitProps[j], props[i].Name, true) == 0)
						{
							omit = true;
							break;
						}

					if ( props[i].CanRead && props[i].CanWrite && !omit)
					{
						//Console.WriteLine( "Setting {0} = {1}", props[i].Name, props[i].GetValue( src, null ) );
						props[i].SetValue( dest, props[i].GetValue( src, null ), null );
					}
					omit = false;
				}
				catch
				{
					//Console.WriteLine( "Denied" );
				}
			}
		}
		
		private static object Construct( Type type, params object[] constructParams)
		{
			bool constructed=false;
			object toReturn=null;
			ConstructorInfo[] info = type.GetConstructors();

			foreach ( ConstructorInfo c in info )
			{
				if (constructed) break;
				ParameterInfo[] paramInfo = c.GetParameters();

				if ( paramInfo.Length == constructParams.Length )
				{
					try
					{
						object o = c.Invoke( constructParams );

						if ( o != null )
						{
							constructed = true;
							toReturn = o;
						}
					}
					catch
					{
						toReturn = null;
					}
				}
			}
			return toReturn;
		}
		
		
	}
	
}


namespace Server.Items
{
	public class ControlItem : Item
	{
		private Mobile m_Owner;
		private Mobile m_Player;
		private Mobile m_NPC;
		
		private bool m_Stats;
		private bool m_Skills;
		private bool m_Items;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public PlayerMobile Owner
		{
			get
			{ 
				if ( m_Player is PlayerMobile )
					return (PlayerMobile)m_Owner; 
				else return null;
			}
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public PlayerMobile Player
		{
			get
			{ 
				if ( m_Player is PlayerMobile )
					return (PlayerMobile)m_Player; 
				else return null;
			}
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile NPC
		{
			get{ return m_NPC; }
			set{ m_NPC = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Stats
		{
			get{ return m_Stats; }
			set{ m_Stats = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Skills
		{
			get{ return m_Skills; }
			set{ m_Skills = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		new public bool Items
		{
			get{ return m_Items; }
			set{ m_Items = value; }
		}
	
		
		public ControlItem( Mobile owner, Mobile player, Mobile npc, bool stats, bool skills, bool items ) : base( 0x2106 )
		{
			m_Owner = owner;
			m_Player = player;
			m_NPC = npc;
			
			m_Stats = stats;
			m_Skills = skills;
			m_Items = items;
			
			Name = "Control Item";
			LootType = LootType.Blessed;
		}
		
		public ControlItem( Mobile owner, Mobile player, Mobile npc ) : base( 0x2106 )
		{
			m_Owner = owner;
			m_Player = player;
			m_NPC = npc;
			
			m_Stats = true;
			m_Skills = true;
			m_Items = true;
			
			Name = "Control Item";
			LootType = LootType.Blessed;
		}
		
		
		public ControlItem( Serial serial ) : base( serial )
		{
		}
		
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from == m_Owner )
				Delete();
			
			base.OnDoubleClick( from );
		}

#if NEWPARENT
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
		{
			base.OnAdded( parent );
			
			if ( RootParent != m_Owner )
				Delete();
		}
		
		public override bool DropToWorld( Mobile from, Point3D p )
		{
			Delete();
			
			
			return false; 
			//return base.DropToWorld( from, p );
		}
		

		public override void OnDelete()
		{
			ControlCommand.EndControl( this, m_Stats, m_Skills, m_Items );
			
			base.OnDelete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			
			//Version 1
			writer.Write( (bool)m_Stats );
			writer.Write( (bool)m_Skills );
			writer.Write( (bool)m_Items );
			
			//Version 0
			writer.Write( (Mobile)m_Owner );
			writer.Write( (Mobile)m_Player );
			writer.Write( (Mobile)m_NPC );
		
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			switch ( version )
			{
				case 1:
				{
					m_Stats = reader.ReadBool();
					m_Skills = reader.ReadBool();
					m_Items = reader.ReadBool();
					goto case 0;
				}
				case 0:
				{
					m_Owner = reader.ReadMobile();
					m_Player = reader.ReadMobile();
					m_NPC = reader.ReadMobile();
					break;
				}
			}
			
		}
		
		
		
		
	}
}