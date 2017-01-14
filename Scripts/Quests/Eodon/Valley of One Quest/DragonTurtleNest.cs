using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using System.Linq;
using Server.Engines.Quests;

namespace Server.Items
{
	public class NestWithEgg : BaseAddon
    {
        public TimeSpan ResetPeriod { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(120, 360)); } }

		public List<BaseCreature> Poachers { get; set; }
        public List<Mobile> FocusList { get; set; }
		public BaseCreature Hatchling { get; set; }
		public Mobile Focus { get; set; }
		public bool IsHatching { get; set; }
		public DateTime CooldownEnds { get; set; }
		
		public Timer DeadlineTimer { get; set; }
		
		public AddonComponent Egg 
		{
            get
            {
                return Components.FirstOrDefault(c => c.ItemID == 16831);
            }
		}
		
		public bool IsInCooldown { get { return CooldownEnds != DateTime.MinValue && DateTime.UtcNow < CooldownEnds; } }
		
        [Constructable]
        public NestWithEgg()
        {
            AddonComponent comp = new LocalizedAddonComponent(3518, 1026869);
            comp.Hue = 449;
            AddComponent(comp, 0, 0, 0);
 
            comp = new LocalizedAddonComponent(3270, 1026869);
            comp.Hue = 551;
            AddComponent(comp, 0, 0, 2);
 
            comp = new LocalizedAddonComponent(3374, 1026869);
            comp.Hue = 551;
            AddComponent(comp, 0, 0, 2);
 
            comp = new LocalizedAddonComponent(16831, 1112469);
            AddComponent(comp, 1, 1, 15);

            FocusList = new List<Mobile>();
        }
		
		public override bool HandlesOnMovement { get { return !IsInCooldown; } }
		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if(m is PlayerMobile && m.Location != oldLocation && m.InRange(this.Location, 3) && (!FocusList.Contains(m) || 0.015 > Utility.RandomDouble()))
			{
				EmptyNestQuest quest = QuestHelper.GetQuest((PlayerMobile)m, typeof(EmptyNestQuest)) as EmptyNestQuest;
				
				if(quest != null && !quest.Completed)
				{
					if(Focus == null)
					{
						Focus = m;
                        m.RevealingAction();
					    SpawnPoachers(m);
						
						DeadlineTimer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), () =>
						{
							DeadlineTimer.Stop();
							
							if(Poachers != null && Poachers.Count > 0)
								Delete();
							else
							{
								Focus = null;
								Hatchling = null;
								CooldownEnds = DateTime.MinValue;
							}
						});
						
						DeadlineTimer.Start();
					}
					else if(IsInCooldown)
					{
						CooldownEnds = DateTime.UtcNow + TimeSpan.FromMinutes(2);
					}
				}
			}
		}
 
		public void SpawnPoachers(Mobile m)
		{
			Map map = m.Map;
			
			if(map == null || map == Map.Internal)
				return;
			
			if(Poachers == null)
				Poachers = new List<BaseCreature>();
			
			for(int i = 0; i < 3; i++)
			{
				Poacher p = new Poacher(this);
				Point3D pnt = m.Location;
				
				for(int j = 0; j < 10; j++)
				{
					int x = Utility.RandomMinMax(m.X - 2, m.X + 2);
					int y = Utility.RandomMinMax(m.Y - 2, m.Y + 2);
					int z = map.GetAverageZ(x, y);
					
					if(map.CanSpawnMobile(x, y, z))
					{
						pnt = new Point3D(x, y, z);
						break;
					}
				}
				
				p.MoveToWorld(pnt, map);
				Poachers.Add(p);
			}
			
			Poachers.ForEach(poacher => poacher.Combatant = m);
		}
		
		public void OnPoacherKilled(BaseCreature bc)
		{
			if(Poachers != null && Poachers.Contains(bc))
			{
				Poachers.Remove(bc);
				
				if(Poachers.Count == 0)
				{
					if(!IsHatching)
					{
						IsHatching = true;
                        ColUtility.Free(Poachers);
						
						Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30)), () =>
						{
							Hatchling = new DragonTurtleHatchling();
							Hatchling.MoveToWorld(this.Location, this.Map);
                            Hatchling.Tamable = false;

							SpawnPoachers(Hatchling);
							
							if(Egg != null)
								Egg.Visible = false;
						});
					}
					else
					{
						if(Hatchling.Alive)
						{
                            CooldownEnds = DateTime.UtcNow + ResetPeriod;
							Timer.DelayCall(TimeSpan.FromSeconds(1), OnComplete, new object[] { Hatchling, Focus } );

                            if(!FocusList.Contains(Focus))
                                FocusList.Add(Focus);
						}
						else
							Delete();
						
						if(DeadlineTimer != null)
						{
							DeadlineTimer.Stop();
							DeadlineTimer = null;
						}
						
						Hatchling = null;
						Focus = null;

                        ColUtility.Free(Poachers);
						Poachers = null;
					}
				}
			}
		}
		
		private void OnComplete(object o)
		{
			object[] objs = o as object[];
			BaseCreature hatchling = objs[0] as BaseCreature;
			Mobile focus = objs[1] as Mobile;
			
			if(hatchling != null)
			{
				focus.PublicOverheadMessage(Server.Network.MessageType.Regular, 0x35, 1156496); // *The Hatchling safely burrows into the sand*
				Timer.DelayCall(TimeSpan.FromSeconds(1), hatchling.Delete);
			}
			
			if(focus != null && focus is PlayerMobile)
			{
				EmptyNestQuest quest = QuestHelper.GetQuest((PlayerMobile)focus, typeof(EmptyNestQuest)) as EmptyNestQuest;
				
				if(quest != null)
				{
					quest.Update(hatchling);
					// Quest Complete and crap can be handled in update
				}
			}
			
			Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
			{
				if(Egg != null && !Egg.Visible)
					Egg.Visible = true;
			});

			IsHatching = false;
		}

        public override void Delete()
        {
            base.Delete();

            if (DeadlineTimer != null)
            {
                DeadlineTimer.Stop();
                DeadlineTimer = null;
            }

            if (Poachers != null)
                ColUtility.Free(Poachers);
        }

        public NestWithEgg(Serial serial)
            : base(serial)
        {
        }
 
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
 
            writer.Write((int)0);
			writer.Write(Hatchling);
        }
 
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
 
            int version = reader.ReadInt();
			Hatchling = reader.ReadMobile() as BaseCreature;
			
			if(Hatchling != null)
				Hatchling.Delete();
			
			if(Egg != null && !Egg.Visible)
				Egg.Visible = true;

            FocusList = new List<Mobile>();
        }
    }
}