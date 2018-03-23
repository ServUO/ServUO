using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.VvV
{
	public enum SteedType 
	{
		Ostard, 
		WarHorse
	}

    public class VvVSteedStatuette : BaseImprisonedMobile
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public SteedType SteedType { get; set; }

        public override BaseCreature Summon
        {
            get
            {
                switch (this.SteedType)
                {
                    default:
                    case SteedType.Ostard: return new VvVMount("a war ostard", 0xDA, 0x3EA4, this.Hue);
                    case SteedType.WarHorse: return new VvVMount("a war horse", 0xE2, 0x3EA0, this.Hue);
                }
            }
        }

		[Constructable]
		public VvVSteedStatuette(SteedType mounttype, int hue) 
            : base(mounttype == SteedType.Ostard ? 8501 : 8484)
		{
            Hue = hue;
            SteedType = mounttype;
		}

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!ViceVsVirtueSystem.IsVvV(m))
            {
                m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                return;
            }

            base.OnDoubleClick(m);
        }
		
		public VvVSteedStatuette(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write((int)SteedType);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			SteedType = (SteedType)reader.ReadInt();
		}
	}
	
	public class VvVMount : BaseMount
	{
		private int _Readiness;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int BattleReadiness
		{
			get { return _Readiness; }
			set
			{
				int old = _Readiness;
				_Readiness = value;
				
				if(_Readiness > 20)
					_Readiness = 20;
				
				if(old != value && ControlMaster != null && ControlMaster.NetState != null)
				{
					int cliloc;
					
					if(old > value)
					{
						if(_Readiness < 5)
							cliloc = 1155551; // *Your steed's battle readiness is dangerously low!*
						else
							cliloc = 1155549; // *Your steed's battle readiness is fading...*
					}
					else
					{
						if(_Readiness == 20)
							cliloc = 1155553; // *Your steed is at maximum battle readiness!*
						else
							cliloc = 1155552;// *Your steed's battle readiness has increased!*
					}
					
					Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
					{
                        if(!Deleted && ControlMaster != null)
						    ControlMaster.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, cliloc, ControlMaster.NetState);
					});
				}
				
				if(_Readiness <= 0)
					GoPoof();
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextReadinessAtrophy { get; set; }

        public override bool DeleteOnRelease { get { return true; } }

		public VvVMount(string name, int id, int itemid, int hue) 
			: base(name, id, itemid, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.4, .2)
		{
			Hue = hue;
			
            if(id == 0xDA)
                BaseSoundID = 0x275;
            else
                BaseSoundID = 0xA8;

			this.InitStats(Utility.Random(300, 100), 125, 60);

            SetStr(400);
            SetDex(125);
            SetInt(51, 55);

            SetHits(240);
            SetMana(0);

            SetDamage(5, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 25.1, 30.0);
            SetSkill(SkillName.Tactics, 29.3, 44.0);
            SetSkill(SkillName.Wrestling, 29.3, 44.0);

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
			
			_Readiness = 8;
			NextReadinessAtrophy = DateTime.UtcNow + TimeSpan.FromHours(24);
			
			Steeds.Add(this);
		}
		
		public void GoPoof()
		{
			if(Rider != null)
			{
				Rider = null;
			}
			
			if(ControlMaster != null && ControlMaster.NetState != null)
				ControlMaster.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1155550, ControlMaster.NetState); // *Your steed has depleted it's battle readiness!*
			
			Delete();
		}
		
		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

            if (BattleReadiness > 1)
                BattleReadiness--;
		}
		
		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if(from == ControlMaster && dropped is EssenceOfCourage)
			{
				EssenceOfCourage ec = dropped as EssenceOfCourage;
                BattleReadiness += dropped.Amount;

                dropped.Delete();

                if (Body.IsAnimal)
                {
                    Animate(3, 5, 1, true, false, 0);
                }
                else if (Body.IsMonster)
                {
                    Animate(17, 5, 1, true, false, 0);
                }

				return true;
			}
			
			return base.OnDragDrop(from, dropped);
		}
		
		public override void Delete()
		{
			base.Delete();
			
			Steeds.Remove(this);
		}

        public override bool CanTransfer(Mobile m)
        {
            if (ControlMaster != null && ControlMaster.NetState != null)
                ControlMaster.SendLocalizedMessage(1155547); // Pets obtained from VvV are non-transferable.

            return false;
        }

        public override bool CanFriend(Mobile m)
        {
            if (ControlMaster != null && ControlMaster.NetState != null)
                ControlMaster.SendLocalizedMessage(1155548); // You may not add friends to a VvV War Steed.

            return false;
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }

        public override int Hides
        {
            get
            {
                return 10;
            }
        }

        public override FoodType FavoriteFood
        {
            get
            {
                if (Body == 0xDA)
                {
                    return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.FruitsAndVegies;
                }
                else
                {
                    return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
                }
            }
        }
		
		public VvVMount(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write(_Readiness);
			writer.Write(NextReadinessAtrophy);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			_Readiness = reader.ReadInt();
			NextReadinessAtrophy = reader.ReadDateTime();
			
			Steeds.Add(this);
		}
		
		public static List<VvVMount> Steeds { get; set; }
		
		public static void Configure()
		{
			Steeds = new List<VvVMount>();
		}
		
		public static void Initialize()
		{
			Timer.DelayCall(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10), () =>
			{
                List<VvVMount> steeds = new List<VvVMount>(Steeds);

				steeds.ForEach(s =>
				{
					if((s.Map != Map.Internal || (s.Rider != null && s.Rider.Map != Map.Internal)) && s.NextReadinessAtrophy < DateTime.UtcNow)
					{
						s.BattleReadiness--;
						
						if(!s.Deleted)
							s.NextReadinessAtrophy = DateTime.UtcNow + TimeSpan.FromHours(24);
					}
				});
				
				steeds.Clear();
				steeds.TrimExcess();
			});
		}
	}
}
