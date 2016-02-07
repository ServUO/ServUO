using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using Server.Gumps;
using System.Text;
using Server.Targeting;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Engines.XmlSpawner2
{

	public class XmlSockets : XmlAttachment
	{
		// if CanSocketByDefault is set to true, then any object can be socketed using the default settings.  If the XmlSocketable attachment is present, it
		// will override these defaults regardless of the CanSocketByDefault setting.  
		// If this is set to false, then ONLY objects with the XmlSocketable attachment can be socketed.
		public static bool CanSocketByDefault = false;

		// The following default settings will be applied when CanSocketByDefault is true
		//
		// DefaultMaxSockets determines the default maximum number of sockets and item can have.
		// A value of -1 means that any item can have an unlimited number of sockets.  
		// To set it up so that items by default cannot have sockets added to them set this to zero.  
		// That way, only items that have the XmlSocketLimit attachment will be allowed to be socketed.  This is the same as setting CanSocketByDefault to false.
		// You can also assign any other value here to set the default number of sockets allowed when no XmlSocketLimit attachment is present.
		public static int DefaultMaxSockets = -1;

		// DefaultSocketDifficulty is the default minimum skill required to socket.
		// This can be overridden with the XmlSocketLimit attachment on specific items (100 by default)
		public static double DefaultSocketDifficulty = 100.0;
        
		public static SkillName DefaultSocketSkill = SkillName.Blacksmith;

		public static Type DefaultSocketResource = typeof(ValoriteIngot);

		public static int DefaultSocketResourceQuantity = 50;

		// DefaultDestructionProbability is the percent chance that failure to socket will result in destruction of the the item (10% by default)
		public static double DefaultDestructionProbability = 0.1;
        
		public static int MaxAugmentDistance = 2;   // if socketed object isnt in pack, then this is the max distance away from it you can be and still augment
        
		public static int MaxSocketDistance = 2;   // if socketable object isnt in pack, then this is the max distance away from it you can be and still socket

		private ArrayList m_SocketOccupants;
        
		private bool m_MustAugmentInPack = true;         // determines whether the socketed object must be in the players pack in order to augment

		public class SocketOccupant
		{
			public Type OccupantType;
			public int OccupantID;
			public int AugmentationVersion;
			public string Description;
			public int IconXOffset;
			public int IconYOffset;
			public int IconHue;
			public bool UseGumpArt;
            
			public SocketOccupant(Type t, int version, int id, int xoffset, int yoffset, int hue, bool usegumpart, string description)
			{
				OccupantType = t;
				AugmentationVersion = version;
				OccupantID = id;
				IconXOffset = xoffset;
				IconYOffset = yoffset;
				IconHue = hue;
				UseGumpArt = usegumpart;
				Description = description;
			}
            
			public SocketOccupant(Type t, int nsockets, int version, int id)
			{
				OccupantType = t;
				AugmentationVersion = version;
				OccupantID = id;
			}
		}

		public ArrayList SocketOccupants { get{ return m_SocketOccupants; } set { m_SocketOccupants = value;} }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool MustAugmentInPack 
		{
			get
			{ 
				return m_MustAugmentInPack;
			}
			set 
			{
				m_MustAugmentInPack = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int NSockets 
		{ 
			get
			{ 
				if(m_SocketOccupants == null)
				{
					return 0;
				}
				else
				{
					return  m_SocketOccupants.Count;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int NFree 
		{ 
			get
			{
				if(m_SocketOccupants == null)
				{
					return 0;
				}
				else
				{
					int count = 0;
					for(int i = 0; i < m_SocketOccupants.Count; i++)
					{
						if(m_SocketOccupants[i] != null) count++;
					}
					return  m_SocketOccupants.Count - count;
				}
			}
		}
        
		public static new void Initialize()
		{
			CommandSystem.Register( "AddSocket", AccessLevel.Player, new CommandEventHandler( AddSocket_OnCommand ) );
			TargetCommands.Register( new UpgradeAugmentCommand() );
			TargetCommands.Register( new LimitSocketsCommand() );
		}

		public class LimitSocketsCommand : BaseCommand
		{
			int m_maxsockets;

			public LimitSocketsCommand()
			{
				AccessLevel = AccessLevel.Administrator;
				Supports = CommandSupport.Area | CommandSupport.Region | CommandSupport.Global | CommandSupport.Multi | CommandSupport.Single;
				Commands = new string[]{ "LimitSockets" };
				ObjectTypes = ObjectTypes.All;
				Usage = "LimitSockets <maxsockets>";
				Description = "Finds XmlSocket attachments with more than maxsockets, recovers their augmentations and reduces the socket number";
				ListOptimized = true;
			}
			
			public override void ExecuteList( CommandEventArgs e, ArrayList list )
			{
				
			
				int nobjects = 0;

				for ( int i = 0; i < list.Count; ++i )
				{

					object targetobject = list[i];

					// get any socket attachments
					// first see if the target has any existing sockets
					XmlSockets s = XmlAttach.FindAttachment(targetobject,typeof(XmlSockets)) as XmlSockets;

					bool found = true;

					if(s != null && s.SocketOccupants != null && s.SocketOccupants.Count > m_maxsockets)
					{
						int maxsock = s.NSockets;
						int nchange = 0;
						while(found && nchange <= maxsock)
						{
							found = false;
						
							// recover all of the augments
							for(int j = 0; j < s.SocketOccupants.Count; j++)
							{
								SocketOccupant so =s.SocketOccupants[j] as SocketOccupant;
								if(so != null )
								{
									nchange++;

									// recover the old version of the augment
									BaseSocketAugmentation augment = s.RecoverAugmentation(null, targetobject, j);

									if(augment != null)
									{

										// give it back to the owner
										if(targetobject is Item)
										{
											// is the parent a container?
											if(((Item)targetobject).Parent is Container)
											{
												// add the augment to the container
												((Container)((Item)targetobject).Parent).DropItem(augment);

											} 
											else
												if(((Item)targetobject).Parent is Mobile)
											{
												// drop it on the ground
												augment.MoveToWorld(((Mobile)((Item)targetobject).Parent).Location);
											} 
											else
												if(((Item)targetobject).Parent == null)
											{
												// drop it on the ground
												augment.MoveToWorld(((Item)targetobject).Location);
											}
										} 
										else
											if(targetobject is Mobile)
										{
											// drop it on the ground
											augment.MoveToWorld(((Mobile)targetobject).Location);
										}
										
										found = true;

										break;
									}
								}
							}
						}
						if(nchange > 0)
							nobjects ++;

						e.Mobile.SendMessage("Modified object {0}",targetobject);

						// limit the sockets
						s.SocketOccupants = new ArrayList(m_maxsockets);

						for(int k =0;k<m_maxsockets;k++) 
						{
							s.SocketOccupants.Add(null);
						}
					}
				}
				
				AddResponse( String.Format( "{0} objects adjusted.",nobjects) );

			}
			
			public override bool ValidateArgs( BaseCommandImplementor impl, CommandEventArgs e )
			{
				if ( e.Arguments.Length >= 1 )
				{
					try
					{
						m_maxsockets = int.Parse(e.GetString( 0 ));
					} 
					catch { e.Mobile.SendMessage( "Usage: " + Usage ); return false; }

					return true;

					
				}
				
				e.Mobile.SendMessage( "Usage: " + Usage );
				return false;
			}
		}

		public class UpgradeAugmentCommand : BaseCommand
		{
			int m_oldversion;
			Type m_augmenttype;

			public UpgradeAugmentCommand()
			{
				AccessLevel = AccessLevel.Administrator;
				Supports = CommandSupport.Area | CommandSupport.Region | CommandSupport.Global | CommandSupport.Multi | CommandSupport.Single;
				Commands = new string[]{ "UpgradeAugment" };
				ObjectTypes = ObjectTypes.All;
				Usage = "UpgradeAugment <augmenttype> <oldversion>";
				Description = "Upgrades the specified augmentation on objects from the oldversion number to the newversion number.";
				ListOptimized = true;
			}
			
			public override void ExecuteList( CommandEventArgs e, ArrayList list )
			{
				
			
				int naugments = 0;
				int nobjects = 0;
				int nfailures = 0;

				for ( int i = 0; i < list.Count; ++i )
				{

					object targetobject = list[i];

					// get any socket attachments
					// first see if the target has any existing sockets
					XmlSockets s = XmlAttach.FindAttachment(targetobject,typeof(XmlSockets)) as XmlSockets;

					bool found = true;

					if(s != null && s.SocketOccupants != null)
					{
						int maxsock = s.NSockets;
						int nchange = 0;
						while(found && nchange <= maxsock)
						{
							found = false;
						
							// find the specified augment type
							for(int j = 0; j < s.SocketOccupants.Count; j++)
							{
								SocketOccupant so =s.SocketOccupants[j] as SocketOccupant;
								if(so != null && so.OccupantType == m_augmenttype && so.AugmentationVersion == m_oldversion)
								{

									// recover the old version of the augment
									BaseSocketAugmentation augment = s.RecoverAugmentation(null, targetobject, j);

									if(augment != null)
									{

										// augment with the new version
										if(!Augment( null, targetobject, s, j, augment))
										{
											e.Mobile.SendMessage("failed to replace augment on {0}",targetobject);
											nfailures++;
										} 
										else 
										{

											naugments++;
										}

										nchange++;

										found = true;

										break;
									}
								}
							}
						}
						if(nchange > 0)
							nobjects ++;
					}
				}
				
				AddResponse( String.Format( "{0} {1} augmentations upgraded on {2} objects. {3} failures.", naugments, m_augmenttype.Name, nobjects, nfailures ) );

			}
			
			public override bool ValidateArgs( BaseCommandImplementor impl, CommandEventArgs e )
			{
				if ( e.Arguments.Length >= 2 )
				{
					m_augmenttype = SpawnerType.GetType(e.GetString( 0 ));

					if(m_augmenttype == null)
					{
						e.Mobile.SendMessage( "Unknown augment type: " + e.GetString( 0 ));
						return false;
					}

					m_oldversion = -1;
					try
					{
						m_oldversion = int.Parse(e.GetString( 1 ));
					} 
					catch{}

					if(m_oldversion != -1)
					{
						return true;
					}
				}
				
				e.Mobile.SendMessage( "Usage: " + Usage );
				return false;
			}
		}

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlSockets(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlSockets(int nsockets)
		{
			m_SocketOccupants = new ArrayList(nsockets);

			for(int i =0;i<nsockets;i++) 
			{
				m_SocketOccupants.Add(null);
			}

			InvalidateParentProperties();
		}
        
		[Attachable]
		public XmlSockets(int nsockets, bool mustaugmentinpack)
		{
			m_SocketOccupants = new ArrayList(nsockets);

			for(int i =0;i<nsockets;i++) 
			{
				m_SocketOccupants.Add(null);
			}
            
			MustAugmentInPack = mustaugmentinpack;

			InvalidateParentProperties();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 2 );

			// version 1
			writer.Write(m_MustAugmentInPack);

			// version 0
			if(m_SocketOccupants == null)
			{
				writer.Write((int)0);
			}
			else
			{
				writer.Write(m_SocketOccupants.Count);

				for(int i = 0;i< m_SocketOccupants.Count;i++)
				{
					SocketOccupant s = (SocketOccupant)m_SocketOccupants[i];
					if(s != null)
					{
						string typename = s.OccupantType.ToString();

						writer.Write(typename);

						if( typename != null)
						{
							writer.Write(s.AugmentationVersion);  // added in version 2
							writer.Write(s.OccupantID);
							writer.Write(s.Description);
							writer.Write(s.IconXOffset);
							writer.Write(s.IconYOffset);
							writer.Write(s.IconHue);
							writer.Write(s.UseGumpArt);
						}

					} 
					else
					{
						writer.Write((string)null);
					}
				}
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if(version > 0)
			{
				// version 1
				m_MustAugmentInPack = reader.ReadBool();
			}

			// version 0
			int count = reader.ReadInt();

			m_SocketOccupants = new ArrayList(count);

			for(int i = 0;i< count;i++)
			{
				Type t = null;

				string typename = reader.ReadString();

				if(typename != null)
				{
					try
					{
						t = Type.GetType(typename);
					} 
					catch{}

					int augmentversion = 0;
					if(version > 1)
					{
						// version 2
						augmentversion = reader.ReadInt();
					}
					int id = reader.ReadInt();
					string desc = reader.ReadString();
					int xoff = reader.ReadInt();
					int yoff = reader.ReadInt();
					int hue = reader.ReadInt();
					bool use = reader.ReadBool();

					if(t != null)
						m_SocketOccupants.Add(new SocketOccupant(t, augmentversion, id, xoff, yoff, hue, use, desc));
					else
						m_SocketOccupants.Add(null);
				} 
				else
				{
					m_SocketOccupants.Add(null);
				}
			}
		}
		
		public override void OnAttach()
		{
			base.OnAttach();
		
			InvalidateParentProperties();
		}

		public override string OnIdentify(Mobile from)
		{

			// open up the socket gump
			if(from != null)
			{
				from.SendGump(new SocketsGump(from, this));
			} 
			else
			{
				return String.Format("{0} Sockets. {1} available.",NSockets, NFree);
			}

			return null;
		}
		
		
		// this is a utility method that will set up a target item with random socket configurations
		public static void ConfigureRandom(object target, double dropchance, double chance5, double chance4, double chance3, double chance2, double chance1)
		{
			ConfigureRandom( target, dropchance, chance5, chance4, chance3, chance2, chance1, 
				DefaultSocketSkill, DefaultSocketDifficulty, DefaultSocketResource, DefaultSocketResourceQuantity);
		}

		public static void ConfigureRandom(object target, double dropchance, double chance5, double chance4, double chance3,
			double chance2, double chance1, SkillName skillname, double minskill, Type resource, int quantity)
		{

			if(Utility.Random(1000) < (int)(dropchance*10))
			{
				// compute chance of having sockets
				int level = Utility.Random(1000);

				if(level < (int)(chance5*10))
					XmlAttach.AttachTo(target, new XmlSockets(5));
				else
					if(level < (int)(chance4 + chance5)*10)
					XmlAttach.AttachTo(target, new XmlSockets(4));
				else
					if(level < (int)(chance3 + chance4)*10)
					XmlAttach.AttachTo(target, new XmlSockets(3));
				else
					if(level < (int)(chance2 + chance3)*10)
					XmlAttach.AttachTo(target, new XmlSockets(2));
				else
					if(level < (int)(chance1 + chance2)*10)
					XmlAttach.AttachTo(target, new XmlSockets(1));
                    

				// compute chance of being socketable
				int socklevel = Utility.Random(1000);
                
				if(socklevel < (int)(chance5*10))
					XmlAttach.AttachTo(target, new XmlSocketable(5, skillname, minskill, resource, quantity));
				else
					if(socklevel < (int)(chance4 + chance5)*10)
					XmlAttach.AttachTo(target, new XmlSocketable(4, skillname, minskill, resource, quantity));
				else
					if(socklevel < (int)(chance3 + chance4)*10)
					XmlAttach.AttachTo(target, new XmlSocketable(3, skillname, minskill, resource, quantity));
				else
					if(socklevel < (int)(chance2 + chance3)*10)
					XmlAttach.AttachTo(target, new XmlSocketable(2, skillname, minskill, resource, quantity));
				else
					XmlAttach.AttachTo(target, new XmlSocketable(1, skillname, minskill, resource, quantity));
			}
		}
        
		public static void ConfigureRandom(object target, double dropchance, double chance5, double chance4, double chance3,
			double chance2, double chance1, SkillName skillname, double minskill, SkillName skillname2, double minskill2, Type resource, int quantity)
		{

			if(Utility.Random(1000) < (int)(dropchance*10))
			{
				// compute chance of having sockets
				int level = Utility.Random(1000);

				if(level < (int)(chance5*10))
					XmlAttach.AttachTo(target, new XmlSockets(5));
				else
					if(level < (int)(chance4 + chance5)*10)
					XmlAttach.AttachTo(target, new XmlSockets(4));
				else
					if(level < (int)(chance3 + chance4)*10)
					XmlAttach.AttachTo(target, new XmlSockets(3));
				else
					if(level < (int)(chance2 + chance3)*10)
					XmlAttach.AttachTo(target, new XmlSockets(2));
				else
					if(level < (int)(chance1 + chance2)*10)
					XmlAttach.AttachTo(target, new XmlSockets(1));
                    

				// compute chance of being socketable
				int socklevel = Utility.Random(1000);
                
				if(socklevel < (int)(chance5*10))
					XmlAttach.AttachTo(target, new XmlSocketable(5, skillname, minskill, skillname2, minskill2, resource, quantity));
				else
					if(socklevel < (int)(chance4 + chance5)*10)
					XmlAttach.AttachTo(target, new XmlSocketable(4, skillname, minskill, skillname2, minskill2, resource, quantity));
				else
					if(socklevel < (int)(chance3 + chance4)*10)
					XmlAttach.AttachTo(target, new XmlSocketable(3, skillname, minskill, skillname2, minskill2, resource, quantity));
				else
					if(socklevel < (int)(chance2 + chance3)*10)
					XmlAttach.AttachTo(target, new XmlSocketable(2, skillname, minskill, skillname2, minskill2, resource, quantity));
				else
					XmlAttach.AttachTo(target, new XmlSocketable(1, skillname, minskill, skillname2, minskill2, resource, quantity));
			}
		}

		public static int ComputeSuccessChance(Mobile from, int nsockets, double difficulty, SkillName requiredSkill)
		{
			if(from == null) return 0;

			double skill = 0;

			// with a difficulty of 120
			// at 120, chance of adding 1 socket is 15%, 2 sockets is 2%, 3 is 0%
			//
			// with a difficulty of 100
			// at skill level of 100, adding 1 socket has a probability of success of 15%, 2 sockets is 0%
			// at 120, chance of adding 1 socket is 55%, 2 sockets is 42%, 3 is 29%, 4 is 16% , 5 is 3%, 6 is 0%
			//
			// with a difficulty of 70
			// at skill level of 100, adding 1 socket has a probability of success of 75%, 2 sockets is 60%, 3 sockets is 45%, etc.
			// at 120, chance of adding 1 socket is 100%, 2 sockets is 100%, 3 sockets is 85%, etc.
			//
			try
			{
				skill = from.Skills[requiredSkill].Value;
			} 
			catch{}

			if((int)(skill - difficulty) < 0) return 0;

			int successchance = (int)(2*(skill - difficulty) + 15 - (nsockets-1)*13);

			if(successchance < 0 ) successchance = 0;

			return successchance;
		}

		public static int ComputeSuccessChance(Mobile from, int nsockets, double difficulty, SkillName requiredSkill, double difficulty2, SkillName requiredSkill2)
		{
			if(from == null) return 0;

			double skill = 0;

			// with a difficulty of 120
			// at 120, chance of adding 1 socket is 15%, 2 sockets is 2%, 3 is 0%
			//
			// with a difficulty of 100
			// at skill level of 100, adding 1 socket has a probability of success of 15%, 2 sockets is 0%
			// at 120, chance of adding 1 socket is 55%, 2 sockets is 42%, 3 is 29%, 4 is 16% , 5 is 3%, 6 is 0%
			//
			// with a difficulty of 70
			// at skill level of 100, adding 1 socket has a probability of success of 75%, 2 sockets is 60%, 3 sockets is 45%, etc.
			// at 120, chance of adding 1 socket is 100%, 2 sockets is 100%, 3 sockets is 85%, etc.
			//
			try
			{
				skill = from.Skills[requiredSkill].Value;
			} 
			catch{}

			if((int)(skill - difficulty) < 0) return 0;

			int successchance = (int)(2*(skill - difficulty) + 15 - (nsockets-1)*13);

			// if a second skill check has been specified then test it
			if(difficulty2 > 0)
			{
				double skill2 = 0;
				try
				{
					skill2 = from.Skills[requiredSkill2].Value;
				} 
				catch{}

				if((int)(skill2 - difficulty2) < 0) return 0;

				successchance = (int)((skill - difficulty) + (skill2 - difficulty2) + 15 - (nsockets-1)*13);
			}

			if(successchance < 0 ) successchance = 0;

			return successchance;
		}

		public static bool AddSocket(Mobile from, object target, int maxSockets, double difficulty, SkillName requiredSkill, Type resource, int quantity)
		{
			return AddSocket(from, target, maxSockets, difficulty, requiredSkill, 0, requiredSkill, resource, quantity);
		}
		
		public static bool AddSocket(Mobile from, object target, int maxSockets, double difficulty, SkillName requiredSkill, 
			double difficulty2, SkillName requiredSkill2, Type resource, int quantity)
		{

			if(maxSockets == 0 || target == null || from == null) return false;


			// is it in the pack
			if ( target is Item && !((Item)target).IsChildOf( from.Backpack ))
			{
				from.SendMessage("Must be in your backpack.");
				return false;
			} 
			else
				if ( target is BaseCreature && ((BaseCreature)target).ControlMaster != from)
			{
				from.SendMessage("Must be under your control.");
				return false;
			}
            
			// check the target range
			if ( target is Mobile && !Utility.InRange( ((Mobile)target).Location, from.Location, MaxSocketDistance ))
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
				return false;
			}


			int nSockets = 0;

			// first see if the target has any existing sockets

			XmlSockets s = XmlAttach.FindAttachment(target,typeof(XmlSockets)) as XmlSockets;

			if(s != null)
			{
				// find out how many sockets it has
				nSockets = s.NSockets;
			}

			// already full, no more sockets allowed
			if(nSockets >= maxSockets && maxSockets >= 0)
			{ 
				from.SendMessage("This cannot be socketed any further.");
				return false;
			}

			// try to add a socket to the target

			// determine the difficulty based upon the required skill and the current number of sockets

			int successchance = ComputeSuccessChance( from, nSockets, difficulty, requiredSkill, difficulty2, requiredSkill2);
            
			if(successchance <= 0)
			{
				from.SendMessage("You have no chance of socketing this.");
				return false;
			}

			// consume the resources

			if(from.Backpack != null && resource != null && quantity > 0)
			{
				if(!from.Backpack.ConsumeTotal( resource, quantity, true))
				{
					from.SendMessage("Socketing this requires {0} {1}",quantity, resource.Name);
					return false;
				}
			}

			from.PlaySound( 0x2A ); // play anvil sound

			if(Utility.Random(1000) < successchance*10)
			{
				from.SendMessage("You have successfully added a socket the target!");
				if(s != null)
				{
					// add an empty socket
					s.SocketOccupants.Add(null );

				} 
				else
				{
					// add an xmlsockets attachment with the new socket
					s = new XmlSockets(1);
					XmlAttach.AttachTo(target, s);
				}

				if(s != null)
					s.InvalidateParentProperties();

				from.SendGump(new SocketsGump(from, s));

			} 
			else
			{
				from.SendMessage("Your attempt to add a socket to the target was unsuccessful.");
				// if it fails then there is also a chance of destroying it
				if(DefaultDestructionProbability > 0 && Utility.RandomDouble() < DefaultDestructionProbability)
				{
                
					from.SendMessage("You destroy the target while attempting to add a socket to it.");

					if(target is Item)
					{
						((Item)target).Delete();

					} 
					else
						if(target is BaseCreature)
					{
						((BaseCreature)target).Delete();
					}

					from.CloseGump(typeof(SocketsGump));

					return false;
				}
			}

			return true;
		}
		
		public static bool Augment( Mobile from, object parent, XmlSockets sock, int socketnum, IXmlSocketAugmentation a)
		{
			if(parent == null || a == null || sock == null || socketnum < 0)
			{
				if(a != null && from == null) a.Delete();

				return false;
			}

			if(sock.SocketOccupants != null && sock.NFree >= a.SocketsRequired && sock.SocketOccupants.Count > socketnum && a.OnAugment(from, parent))
			{
				SocketOccupant occ = new SocketOccupant(a.GetType(), a.Version, a.Icon, a.IconXOffset, a.IconYOffset, a.IconHue, a.UseGumpArt,
					String.Format("{0}\n{1}\n{2} Socket(s)",a.Name,a.OnIdentify(null),a.SocketsRequired));
                
				int remaining = a.SocketsRequired;

				// make sure the requested socket is unoccupied
				if(sock.SocketOccupants[socketnum] == null)
				{
					sock.SocketOccupants[socketnum] = occ;
					remaining--;
				}

				// if this augmentation requires more than 1 socket, then fill the rest
				if(remaining > 0)
				{
					// find a free socket
					for(int i = 0;i<sock.SocketOccupants.Count;i++)
					{
						if(sock.SocketOccupants[i] == null)
						{
							sock.SocketOccupants[i] = occ;
							remaining--;
						}
                        
						// filled all of the required sockets
						if(remaining == 0) break;
					}
				}
                
				// destroy after augmenting if needed
				if(a.DestroyAfterUse || from == null)
					a.Delete();
                    
				return true;
			} 
			else
			{
				if(from == null) a.Delete();
				return false;
			}
		}
        
		public BaseSocketAugmentation RecoverAugmentation(Mobile from, object o, int index)
		{
			if(SocketOccupants == null || SocketOccupants.Count < index) return null;

			BaseSocketAugmentation augment = null;

			SocketOccupant s = SocketOccupants[index] as SocketOccupant;
            
			if(s == null) return null;

			Type t = s.OccupantType;
			if(t != null)
			{
				try
				{
					augment = Activator.CreateInstance( t ) as BaseSocketAugmentation;
				} 
				catch{}
			}
			if(augment != null)
			{
				if(augment.CanRecover(from, o, s.AugmentationVersion) && augment.RecoverableSockets(s.AugmentationVersion) > 0)
				{

					// recover the augment
					augment.OnRecover(from, o, s.AugmentationVersion);

					// clear all of the sockets that it occupied
					int count = augment.RecoverableSockets(s.AugmentationVersion);
					for(int i = 0; i < SocketOccupants.Count; i++)
					{
						if(SocketOccupants[i] != null && ((SocketOccupant)SocketOccupants[i]).OccupantType == t && ((SocketOccupant)SocketOccupants[i]).AugmentationVersion == s.AugmentationVersion)
						{
							SocketOccupants[i] = null;
							count--;
							if(count <= 0) break;
						}
					}

				} 
				else
				{
					augment.Delete();
					return null;
				}
			}
            
			return augment;
		}
        
		public BaseSocketAugmentation RecoverRandomAugmentation(Mobile from, object o)
		{
			int nfilled = NSockets - NFree;

			if(nfilled > 0)
			{
				// pick a random augmentation from a filled socket
				int rindex = Utility.Random(nfilled);
				int count = 0;
				for(int i = 0; i < SocketOccupants.Count; i++)
				{
					if(SocketOccupants[i] != null)
					{
						if(count == rindex)
						{
							// reconstruct the augment
							BaseSocketAugmentation augment = RecoverAugmentation(from, o, i);

							return augment;
						}
						count++;
					}
				}
			}

			return null;
		}

		
		private class AddAugmentationToSocket : Target
		{
			private object m_parent;
			private XmlSockets m_s;
			private int m_socketnum;

			public AddAugmentationToSocket(object parent, XmlSockets s, int socketnum) :  base ( 30, false, TargetFlags.None )
			{
				m_parent = parent;
				m_socketnum = socketnum;
				m_s = s;
			}
			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null || targeted == null) return;

				// is this a valid augmentation
				if(targeted is IXmlSocketAugmentation)
				{
					IXmlSocketAugmentation a = targeted as IXmlSocketAugmentation;

					// check the augment
					if ( a is Item && !((Item)a).IsChildOf( from.Backpack ))
					{
						from.SendMessage("Augmentation must be in your backpack.");
						return;
					} 
					else
						if ( a is BaseCreature && ((BaseCreature)a).ControlMaster != from)
					{
						from.SendMessage("Augmentation must be under your control.");
						return;
					}
                    
					// check the parent
					if ( m_parent is Item && !((Item)m_parent).IsChildOf( from.Backpack ) && m_s != null && m_s.MustAugmentInPack)
					{
						from.SendMessage("Socketable item be in your backpack.");
						return;
					} 
					else
						if ( m_parent is BaseCreature && ((BaseCreature)m_parent).ControlMaster != from)
					{
						from.SendMessage("Socketable creature must be under your control.");
						return;
					}
                    
					// check the parent range
					if ( ( m_parent is Item && ((Item)m_parent).Parent == null) || m_parent is Mobile)
					{
						Point3D loc = from.Location;

						if(m_parent is Item)
						{
							loc = ((Item)m_parent).Location;
						} 
						else
							if(m_parent is Mobile)
						{
							loc = ((Mobile)m_parent).Location;
						}


						if ( !Utility.InRange( loc, from.Location, MaxAugmentDistance ) )
						{
							from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
							return;
						}
					}

					if (a.CanAugment(from, m_parent, m_socketnum))
					{

						// check for sufficient available sockets
						if( m_s == null || m_s.NFree < a.SocketsRequired)
						{
							from.SendMessage("Not enough available sockets for that augmentation.");
							return;
						}

						if(!a.ConsumeOnAugment(from))
						{
							from.SendMessage("You are lacking the resources needed to use that augmentation.");
							return;
						}

						from.PlaySound( 0x2A ); // play anvil sound

						// add the augmentation to the socket
						if(Augment(from, m_parent, m_s, m_socketnum, a))
						{
							// refresh the gump
							from.CloseGump(typeof(SocketsGump));
							from.SendGump(new SocketsGump(from, m_s));
							from.SendMessage("You successfully augment the target.");
                            
							m_s.InvalidateParentProperties();



						} 
						else
						{
							from.SendMessage("The augmentation fails.");
						}
					} 
					else
					{
						from.SendMessage("That cannot be used in this socket.");

					}
				} 
				else
				{
					from.SendMessage("That is not a valid augmentation.");
				}
			}
		}

		public class AddSocketToTarget : Target
		{
			//private CommandEventArgs m_e;
			private int m_extrasockets;

			public AddSocketToTarget( ) :  this ( 0 )
			{
			}

			public AddSocketToTarget( int extrasockets) :  base ( 30, false, TargetFlags.None )
			{
				m_extrasockets = extrasockets;
			}
			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null || targeted == null) return;


				int maxSockets = DefaultMaxSockets;
				double difficulty = DefaultSocketDifficulty;
				SkillName skillname = DefaultSocketSkill;
				double difficulty2 = DefaultSocketDifficulty;
				SkillName skillname2 = DefaultSocketSkill;
				Type resource = DefaultSocketResource;
				int quantity = DefaultSocketResourceQuantity;

				// see if this has an XmlSocketable attachment that might impose socketing restrictions
				XmlSocketable s = XmlAttach.FindAttachment(targeted, typeof(XmlSocketable)) as XmlSocketable;

				if(s != null)
				{
					// get the socketing restrictions
					maxSockets = s.MaxSockets;
					// and any difficulty restrictions
					difficulty = s.MinSkillLevel;
					skillname = s.RequiredSkill;
					difficulty2 = s.MinSkillLevel2;
					skillname2 = s.RequiredSkill2;
					resource = s.RequiredResource;
					quantity = s.ResourceQuantity;
				} 
				else if(!CanSocketByDefault && m_extrasockets <= 0)
				{
					from.SendMessage("This cannot be socketed.");
					return;
				}

				if(maxSockets < 0 && m_extrasockets > 0)
				{
					maxSockets = 0;
				}

				// override maximum socket restrictions
				maxSockets += m_extrasockets;

				AddSocket(from, targeted, maxSockets, difficulty, skillname, difficulty2, skillname2, resource, quantity);
			}
		}
        
		public class RecoverAugmentationFromTarget : Target
		{
			//private CommandEventArgs m_e;

			public RecoverAugmentationFromTarget( /*CommandEventArgs e*/) :  base ( 30, false, TargetFlags.None )
			{
				//m_e = e;
			}
			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null || targeted == null) return;

				// see if the target has an XmlSockets attachment
				XmlSockets s = XmlAttach.FindAttachment(targeted,typeof(XmlSockets)) as XmlSockets;

				if(s == null)
				{
					from.SendMessage("This is not socketed.");
					return;
				}

				BaseSocketAugmentation augment = s.RecoverRandomAugmentation(from, targeted);

				if(augment != null)
				{
					from.SendMessage("Recovered a {0} augmentation.", augment.Name);
					from.AddToBackpack(augment);
                    
					// update the sockets gump
					s.OnIdentify(from);
				} 
				else
				{
					from.SendMessage("Failed to recover augmentation.");
				}
			}
		}

		[Usage( "AddSocket" )]
		[Description( "Attempts to add a socket to the targeted item." )]
		public static void AddSocket_OnCommand( CommandEventArgs e )
		{
			e.Mobile.Target = new AddSocketToTarget();
		}

		private class SocketsGump : Gump
		{
			private XmlSockets m_attachment;
			private const int vertspacing = 65;

			public SocketsGump( Mobile from, XmlSockets a) : base( 0,0)
			{
				if(a == null)
				{
					return;
				}
				if(from != null)
					from.CloseGump(typeof( SocketsGump));

				m_attachment = a;

				int socketcount = 0;

				if(a.SocketOccupants != null)
				{
					socketcount = a.SocketOccupants.Count;
				}

				// prepare the page
				AddPage( 0 );

				AddBackground( 0, 0, 185, 137 + socketcount*vertspacing, 5054 );

				AddImage( 2 , 2 , 0x28d4);  // garg top center

				AddImageTiled( 2 , 52 , 30,  50 + socketcount*vertspacing, 0x28e0);  // left edge

				AddImageTiled( 151 , 52 , 30,  50 + socketcount*vertspacing, 0x28e0);  // right edge
                
				AddImageTiled( 40 , 104 + socketcount*vertspacing , 121, 30, 0x28de);  // bottom edge
				AddImage( 143 , 84 + socketcount*vertspacing , 0x28d2);  // garg bottom right
				AddImage( 2 , 84 + socketcount*vertspacing , 0x28d2);  // garg bottom left

				string label = null;

				if(m_attachment.AttachedTo is Item)
				{
					Item item =  m_attachment.AttachedTo as Item;

					label = item.Name;
                    
					if(item.Name == null)
					{
						label = item.ItemData.Name;
					}

				} 
				else
					if(m_attachment.AttachedTo is Mobile)
				{
					Mobile m = m_attachment.AttachedTo as Mobile;
                    
					label = m.Name;
				}

				int xadjust = 50;
				if(label != null)
					xadjust -= label.Length*5/2;

				if(xadjust < 0) xadjust = 0;

				AddLabelCropped( 34 + xadjust, 60, 110-xadjust, 20, 55, label );

				// go through the list of sockets and add buttons for them
				int xoffset = 62;
				int y = 98;
				for(int i = 0;i<socketcount;i++)
				{
					SocketOccupant s = (SocketOccupant)m_attachment.SocketOccupants[i];

					// add the socket icon
					if(s != null && s.OccupantID != 0)
					{
						// add the describe socket button
						AddButton( xoffset, y, 0x00ea, 0x00ea, 2000+i, GumpButtonType.Reply, 0 );

						// put in the filled socket background
						AddImageTiled( xoffset , y+1 , 57, 59, 0x1404);  // dark
						AddImageTiled( xoffset+2 , y , 56, 58, 0xbbc);  // light
						AddImageTiled( xoffset+2, y+2 , 54, 56, 0x13f0);   // neutral

						if(s.UseGumpArt)
						{
							AddImage( xoffset + s.IconXOffset, y + s.IconYOffset, s.OccupantID, s.IconHue );
						} 
						else
						{
							AddItem( xoffset + s.IconXOffset, y + s.IconYOffset, s.OccupantID, s.IconHue );
						}
					} 
					else
					{
						// display the empty socket button
						AddButton( xoffset+2, y, 0x00ea, 0x00ea, 1000+i, GumpButtonType.Reply, 0 );

					}

					y += vertspacing;
				}

			}
			public override void OnResponse( NetState state, RelayInfo info )
			{

				if(m_attachment == null || state == null || state.Mobile == null || info == null || m_attachment.SocketOccupants == null) return;

				// make sure the parent of the socket is still valid
				if(m_attachment.AttachedTo == null ||  (m_attachment.AttachedTo is Item && ((Item)m_attachment.AttachedTo).Deleted) || 
					(m_attachment.AttachedTo is Mobile && ((Mobile)m_attachment.AttachedTo).Deleted))
				{
					return;
				}

				// go through all of the possible specials and find the matching button
				for(int i = 0;i<m_attachment.SocketOccupants.Count;i++)
				{
					SocketOccupant s = (SocketOccupant)m_attachment.SocketOccupants[i];

					if(info.ButtonID == i + 1000)
					{
						// bring up the targeting to fill the socket
						state.Mobile.Target = new AddAugmentationToSocket(m_attachment.AttachedTo, m_attachment, i);

						state.Mobile.SendGump(new SocketsGump(state.Mobile, m_attachment));

						break;
					}
					if(info.ButtonID == i + 2000)
					{
						if(s != null)
						{
							// bring up the info on the socket
							// check the augmentation to see if it can be recovered.  Optimize this later with table look up of prototype instances
							bool canrecover = false;
							BaseSocketAugmentation augment = null;
							Type t = s.OccupantType;
							if(t != null)
							{
								try
								{
									augment = Activator.CreateInstance( t ) as BaseSocketAugmentation;
								} 
								catch{}
							}
							if(augment != null)
							{
								canrecover = augment.CanRecover(state.Mobile, m_attachment.AttachedTo, s.AugmentationVersion);
								augment.Delete();
							}

							state.Mobile.SendMessage(s.Description + (canrecover ? "\nCan be recovered" : ""));
						}

						state.Mobile.SendGump(new SocketsGump(state.Mobile, m_attachment));

						break;
					}
				}
			}
		}
	}
}
