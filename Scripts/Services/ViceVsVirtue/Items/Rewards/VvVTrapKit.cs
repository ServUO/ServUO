using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Targeting;
using Server.ContextMenus;
using System.Linq;

namespace Server.Engines.VvV
{
	public class VvVTrapKit : Item
	{
        [CommandProperty(AccessLevel.GameMaster)]
		public DeploymentType DeploymentType { get; set;}

        [CommandProperty(AccessLevel.GameMaster)]
		public TrapType TrapType { get; set; }
		
		public override int LabelNumber { get { return 1154944; } } // Trap Kit
		
		private static Dictionary<Mobile, DateTime> _Cooldown = new Dictionary<Mobile, DateTime>();
		
		[Constructable]
		public VvVTrapKit(TrapType type) : base(7866)
		{
			TrapType = type;
            DeploymentType = DeploymentType.Proximaty;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			CheckCooldown();
			
			if(IsChildOf(from.Backpack))
			{
				ViceVsVirtueSystem sys = ViceVsVirtueSystem.Instance;
				
				if(sys != null)
				{
					if(!ViceVsVirtueSystem.IsVvV(from))
					{
						from.SendLocalizedMessage(1155415); // Only participants in Vice vs Virtue may use this item.
					}
					else if(!sys.Battle.OnGoing || !from.Region.IsPartOf(sys.Battle.Region))
					{
						from.SendLocalizedMessage(1155406); // This item can only be used in an active VvV battle region!
					}
					else if (sys.Battle.TrapCount >= VvVBattle.MaxTraps)
					{
						from.SendLocalizedMessage(1155407); // The trap limit for this battle has been reached!
					}
					else if (_Cooldown != null && _Cooldown.ContainsKey(from))
					{
						from.SendLocalizedMessage(1155408); // You must wait a few moments before attempting to place another trap.
					}
					else
					{
						from.SendLocalizedMessage(1155409); // Where do you want to place the trap?
						from.BeginTarget(2, true, Server.Targeting.TargetFlags.None, (m, targeted) =>
						{
							IPoint3D p = targeted as IPoint3D;
							
							if(p != null)
							{
                                if (!sys.Battle.OnGoing || !m.Region.IsPartOf(sys.Battle.Region))
								{
									m.SendLocalizedMessage(1155406); // This item can only be used in an active VvV battle region!
								}
								else if (sys.Battle.Traps.Count >= VvVBattle.MaxTraps)
								{
									m.SendLocalizedMessage(1155407); // The trap limit for this battle has been reached!
								}
								else if (!from.InLOS(p))
								{
									m.SendLocalizedMessage(1042261); // You cannot place the trap there.
								}
								else
								{
									TryDeployTrap(m, new Point3D(p));
								}
							}
							else
								m.SendLocalizedMessage(1042261); // You cannot place the trap there.
						});
					}
				}
			}
			else
			{
				from.SendLocalizedMessage(1042004); // That must be in your pack for you to use it
			}
		}
		
		public void TryDeployTrap(Mobile m, Point3D trapLocation)
		{
			VvVTrap trap = null;
			
			if(this.DeploymentType == DeploymentType.Tripwire)
			{
				m.SendLocalizedMessage(1155410); // Target the location to run the tripwire...
				m.BeginTarget(5, true, TargetFlags.None, (from, targeted) =>
				{
					IPoint3D p = targeted as IPoint3D;
					
					if(p != null)
					{
						Point3D point = new Point3D(p);
						
						//TODO: Rules?  For now, must be within 3 tiles of trap
                        if (!Utility.InRange(point, trapLocation, 3) || point == trapLocation)
						{
							m.SendLocalizedMessage(1011577); // This is an invalid location.
						}
						else
						{
							trap = ConstructTrap(m);

                            if (!trap.SetTripwire(this, trapLocation, point, m.Map))
                            {
                                trap.Delete();
                                m.SendLocalizedMessage(1042261); // You cannot place the trap there.
                                return;
                            }
                            else
                            {
                                m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1155411, m.NetState); // *You successfully lay the tripwire*
                            }
						}
					}
					else
					{
						m.SendLocalizedMessage(1042261); // You cannot place the trap there.
					}
				});
			}
			else
			{
				m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1155412, m.NetState); // *You successfully set the trap*
				trap = ConstructTrap(m);
			}
			
			if(trap != null)
			{
				trap.MoveToWorld(trapLocation, m.Map);
				Delete();

                ViceVsVirtueSystem.Instance.Battle.Traps.Add(trap);

				AddToCooldown(m);
			}
		}
		
		private void AddToCooldown(Mobile m)
		{
			_Cooldown[m] = DateTime.UtcNow + TimeSpan.FromSeconds(30);
		}
		
		private void CheckCooldown()
		{
			if(_Cooldown.Count == 0)
				return;
			
			List<Mobile> mobs = new List<Mobile>(_Cooldown.Keys);
			
			foreach(Mobile m in mobs.Where(mob => _Cooldown[mob] < DateTime.UtcNow))
			{
				_Cooldown.Remove(m);
			}

            mobs.Clear();
            mobs.TrimExcess();
		}
		
		public VvVTrap ConstructTrap(Mobile m)
		{
			switch(this.TrapType)
			{
				case TrapType.Explosion: return new VvVExplosionTrap(m, this.DeploymentType);
				case TrapType.Poison: return new VvVPoisonTrap(m, this.DeploymentType);
				case TrapType.Cold: return new VvVColdTrap(m, this.DeploymentType);
				case TrapType.Energy: return new VvVEnergyTrap(m, this.DeploymentType);
				case TrapType.Blade: return new VvVBladeTrap(m, this.DeploymentType);
			}
			
			return null;
		}
		
		public override void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list)
		{
			list.Add(new InternalEntry(this, m));
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			
			list.Add(1154938, String.Format("#{0}", ((int)this.DeploymentType).ToString())); // Deployment Type: ~1_DEPLOYTYPE~
			list.Add(1154941, String.Format("#{0}", ((int)this.TrapType).ToString())); // Damage Type: ~1_DMGTYPE~
            list.Add(1154937); // VvV Item
		}

        private class InternalEntry : ContextMenuEntry
        {
            public VvVTrapKit Deed { get; set; }
            public Mobile Clicker { get; set; }

            public InternalEntry(VvVTrapKit deed, Mobile m)
                : base(1155514, -1)
            {
                Deed = deed;
                Clicker = m;

                if (!Deed.IsChildOf(m.Backpack))
                    Enabled = false;
            }

            public override void OnClick()
            {
                if (Deed.DeploymentType == DeploymentType.Proximaty)
                    Deed.DeploymentType = DeploymentType.Tripwire;
                else
                    Deed.DeploymentType = DeploymentType.Proximaty;

                Deed.InvalidateProperties();

                Clicker.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1155515, Clicker.NetState); // *You adjust the deployment mechanism*
            }
        }
		
		public VvVTrapKit(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);

            writer.Write((int)DeploymentType);
            writer.Write((int)TrapType);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

            DeploymentType = (DeploymentType)reader.ReadInt();
            TrapType = (TrapType)reader.ReadInt();
		}
	}
}