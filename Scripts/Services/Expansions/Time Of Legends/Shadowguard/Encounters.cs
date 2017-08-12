using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Engines.PartySystem;
using System.Linq;

namespace Server.Engines.Shadowguard	
{
	public class BarEncounter : ShadowguardEncounter
	{
        public const int LiquorCount = 10;

        public override Type AddonType { get { return typeof(BarAddon); } }

		public List<Mobile> Pirates { get; set; }
		public int Wave { get; set; }
	
		public List<ShadowguardBottleOfLiquor> Bottles { get; set; }

        public BarEncounter()
            : base(EncounterType.Bar)
        {
        }

        public BarEncounter(ShadowguardInstance instance)
            : base(EncounterType.Bar, instance)
		{
		}
		
		public override void Setup()
		{
			Pirates = new List<Mobile>();
            Bottles = new List<ShadowguardBottleOfLiquor>();
            Wave = 0;

            int toSpawn = Math.Max(3, PartySize() * 3);

            for (int i = 0; i < toSpawn; i++)
                SpawnRandomPirate();

            for (int i = 0; i < LiquorCount; i++)
				SpawnRandomLiquor();
		}
		
		public override void CheckEncounter()
		{
			if(Wave >= 4 || Bottles == null)
				return;

            int liquorCount = Bottles.Where(b => b != null && !b.Deleted).Count();

			if(liquorCount < LiquorCount)
			{
				SpawnRandomLiquor();
			}
		}

        private void SpawnRandomLiquor()
        {
            int row = Utility.Random(4);
            Rectangle2D rec = SpawnRecs[Utility.Random(SpawnRecs.Length)];

            ConvertOffset(ref rec);

            int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
			int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
			int z = -14;

            x = x + (9 * row);
			
			ShadowguardBottleOfLiquor bottle = new ShadowguardBottleOfLiquor(this);
			bottle.MoveToWorld(new Point3D(x, y, z), Map.TerMur);
			
			Bottles.Add(bottle);
        }

        private void SpawnRandomPirate()
        {
            if (Pirates == null)
                return;

            int row = Utility.Random(8);
            Point3D ranPnt = SpawnPoints[Utility.Random(SpawnPoints.Length)];

            ConvertOffset(ref ranPnt);

            int a = row % 2 == 0 ? 0 : 3;
            int startX = ranPnt.X + a;
            int x = startX + ((row / 2) * 9);

            ShadowguardPirate pirate = new ShadowguardPirate();
			pirate.MoveToWorld(new Point3D(x, ranPnt.Y, ranPnt.Z), Map.TerMur);
			Pirates.Add(pirate);
        }
		
		public override void OnCreatureKilled(BaseCreature bc)
		{
            if (!(bc is ShadowguardPirate) || Pirates == null)
                return;

			if(Pirates.Contains(bc))
				Pirates.Remove(bc);

			if(Pirates.Count <= 0)
			{
				Wave++;
				Pirates.Clear();

                int toSpawn = Math.Max(3, PartySize() * 3);
				
				if(Wave < 4)
				{
                    for (int i = 0; i < toSpawn; i++)
					{
                        SpawnRandomPirate();
					}
				}
				else if (Wave == 4)
				{
                    var pirate = new ShantyThePirate();
                    Point3D p = SpawnPoints[Utility.Random(SpawnPoints.Length)];
                    ConvertOffset(ref p);
                    pirate.MoveToWorld(p, Map.TerMur);
					Pirates.Add(pirate);
				}
				else
					CompleteEncounter();
			}
		}
		
		public override void ClearItems()
		{
            if (Bottles != null)
            {
                List<ShadowguardBottleOfLiquor> list = new List<ShadowguardBottleOfLiquor>(Bottles.Where(b => b != null && !b.Deleted));

                foreach (var bottle in list)
                    bottle.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Bottles);
                Bottles = null;
            }

            if (Pirates != null)
            {
                ColUtility.Free(Pirates);
                Pirates = null;
            }
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Wave);

            writer.Write(Pirates == null ? 0 : Pirates.Count);
            if (Pirates != null) Pirates.ForEach(p => writer.Write(p));

            writer.Write(Bottles == null ? 0 : Bottles.Count);
            if (Bottles != null) Bottles.ForEach(b => writer.Write(b));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Pirates = new List<Mobile>();
            Bottles = new List<ShadowguardBottleOfLiquor>();

            Wave = reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile p = reader.ReadMobile();
                if (p != null)
                    Pirates.Add(p);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ShadowguardBottleOfLiquor b = reader.ReadItem() as ShadowguardBottleOfLiquor;
                if (b != null)
                    Bottles.Add(b);
            }

            if (Pirates == null || Pirates.Count < 6)
            {
                int toSpawn = Pirates == null ? 6 : 6 - Pirates.Count;

                for (int i = 0; i < toSpawn; i++)
                {
                    SpawnRandomPirate();
                }
            }
        }
	}
	
	public class OrchardEncounter : ShadowguardEncounter
	{
		public List<ShadowguardCypress> Trees { get; set; }
        public List<BaseCreature> Spawn { get; set; }

        public Item Bones { get; set; }

        public override Type AddonType { get { return typeof(OrchardAddon); } }

        public OrchardEncounter()
            : base(EncounterType.Orchard)
        {
        }

        public OrchardEncounter(ShadowguardInstance instance)
            : base(EncounterType.Orchard, instance)
		{
		}
		
		public override void Setup()
		{
			Trees = new List<ShadowguardCypress>();
            Spawn = new List<BaseCreature>();
			
			List<Point3D> points = new List<Point3D>();
            for(int i = 0; i < SpawnPoints.Length; i++)
            {
                Point3D p = SpawnPoints[i];
                ConvertOffset(ref p);
                points.Add(p);
            }
					
			foreach(int i in Enum.GetValues(typeof(VirtueType)))
			{
				if(i > 7)
					break;
					
				ShadowguardCypress tree = new ShadowguardCypress(this, (VirtueType)i);
				Point3D p = points[Utility.Random(points.Count)];

				tree.MoveToWorld(p, Map.TerMur);
				points.Remove(p);
				Trees.Add(tree);
				
				tree = new ShadowguardCypress(this, (VirtueType)i + 8);
				p = points[Utility.Random(points.Count)];

				tree.MoveToWorld(p, Map.TerMur);
				points.Remove(p);
				Trees.Add(tree);
			}

            Item bones = new WitheringBones();
            Point3D pnt = new Point3D(-15, -11, 0);
            ConvertOffset(ref pnt);
            bones.MoveToWorld(pnt, Map.TerMur);
            Bones = bones;

            ColUtility.Free(points);
		}
		
		public override void CheckEncounter()
		{
            if (Trees == null)
                return;

			int treeCount = Trees.Count;
			
			foreach(ShadowguardCypress tree in Trees)
			{
				if(tree == null || tree.Deleted)
					treeCount--;
			}
			
			if(treeCount <= 0)
				CompleteEncounter();
		}
		
		public void AddSpawn(BaseCreature bc)
		{
            if(Spawn != null)
			    Spawn.Add(bc);
		}
		
		public override void ClearItems()
		{
            if (Spawn != null)
            {
                List<BaseCreature> list = new List<BaseCreature>(Spawn.Where(e => e != null && e.Alive));

                foreach (var spawn in list)
                    spawn.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Spawn);
                Spawn = null;
            }

            if (Trees != null)
            {
                List<ShadowguardCypress> list = new List<ShadowguardCypress>(Trees.Where(t => t != null && !t.Deleted));

                foreach (var tree in list)
                    tree.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Trees);
                Trees = null;
            }

            if (Bones != null)
            {
                Bones.Delete();
                Bones = null;
            }
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Trees == null ? 0 : Trees.Count);
            if (Trees != null) Trees.ForEach(t => writer.Write(t));

            writer.Write(Spawn == null ? 0 : Spawn.Count);
            if (Spawn != null) Spawn.ForEach(s => writer.Write(s));

            writer.Write(Bones);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Trees = new List<ShadowguardCypress>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ShadowguardCypress tree = reader.ReadItem() as ShadowguardCypress;

                if (tree != null)
                    Trees.Add(tree);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (Spawn == null)
                    Spawn = new List<BaseCreature>();

                BaseCreature bc = reader.ReadMobile() as BaseCreature;

                if (bc != null)
                    Spawn.Add(bc);
            }

            Bones = reader.ReadItem();
        }
	}
	
	public class ArmoryEncounter : ShadowguardEncounter
	{
		public List<Item> Armor { get; set; }
		public List<Item> DestroyedArmor { get; set; }
        public List<Item> Items { get; set; }
		public List<BaseCreature> Spawn { get; set; }

        public override Type AddonType { get { return typeof(ArmoryAddon); } }

        public ArmoryEncounter()
            : base(EncounterType.Armory)
        {
        }

        public ArmoryEncounter(ShadowguardInstance instance)
            : base(EncounterType.Armory, instance)
		{
		}
		
		public override void Setup()
		{
            Armor = new List<Item>();
			DestroyedArmor = new List<Item>();
			Spawn = new List<BaseCreature>();
            Items = new List<Item>();

            int toSpawn = 1 + (PartySize() * 2);

            ColUtility.For(SpawnPoints, (i, p) =>
			{
                ConvertOffset(ref p);

				var armor = new CursedSuitOfArmor(this);
				armor.MoveToWorld(p, Map.TerMur);
				Armor.Add(armor);

                if(i > 13)
                    armor.ItemID = 0x1512;
			});
			
			for(int i = 0; i < toSpawn; i++)
			{
				SpawnRandom();
			}

            Item item = new Static(3633);
            Point3D pnt = new Point3D(-4, 2, 0);
            ConvertOffset(ref pnt);
            item.MoveToWorld(pnt, Map.TerMur);
            Items.Add(item);

            item = new Static(3633);
            pnt = new Point3D(-4, -4, 0);
            ConvertOffset(ref pnt);
            item.MoveToWorld(pnt, Map.TerMur);
            Items.Add(item);

            item = new Static(3633);
            pnt = new Point3D(2, -4, 0);
            ConvertOffset(ref pnt);
            item.MoveToWorld(pnt, Map.TerMur);
            Items.Add(item);

            item = new PurifyingFlames();
            pnt = new Point3D(-4, 2, 8);
            ConvertOffset(ref pnt);
            item.MoveToWorld(pnt, Map.TerMur);
            Items.Add(item);

            item = new PurifyingFlames();
            pnt = new Point3D(-4, -4, 8);
            ConvertOffset(ref pnt);
            item.MoveToWorld(pnt, Map.TerMur);
            Items.Add(item);

            item = new PurifyingFlames();
            pnt = new Point3D(2, -4, 8);
            ConvertOffset(ref pnt);
            item.MoveToWorld(pnt, Map.TerMur);
            Items.Add(item);
		}
		
		public override void CheckEncounter()
		{
			if(Armor != null && Armor.Where(a => a != null && !a.Deleted).Count() == 0)
				CompleteEncounter();
		}
		
		public override void OnCreatureKilled(BaseCreature bc)
		{
            if (Spawn != null && Spawn.Contains(bc))
			{
				Spawn.Remove(bc);
				Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(5, 15)), SpawnRandom);
			}
		}
		
		public void AddDestroyedArmor(Item item)
		{
            if(DestroyedArmor != null)
			    DestroyedArmor.Add(item);
		}
		
		public override void ClearItems()
		{
            if (Armor != null)
            {
                List<Item> list = new List<Item>(Armor.Where(i => i != null && !i.Deleted));

                foreach (Item armor in list)
                    armor.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Armor);
                Armor = null;
            }

            if (DestroyedArmor != null)
            {
                List<Item> list = new List<Item>(DestroyedArmor.Where(i => i != null && !i.Deleted));

                foreach (Item dest in list)
                    dest.Delete();

                ColUtility.Free(list);

                ColUtility.Free(DestroyedArmor);
                DestroyedArmor = null;
            }

            if (Spawn != null)
            {
                List<BaseCreature> list = new List<BaseCreature>(Spawn.Where(s => s != null && !s.Deleted));

                foreach (BaseCreature spawn in list)
                    spawn.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Spawn);
                Spawn = null;
            }

            if (Items != null)
            {
                List<Item> list = new List<Item>(Items.Where(i => i != null && !i.Deleted));

                foreach (Item item in list)
                    item.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Items);
                Items = null;
            }
		}
		
		private void SpawnRandom()
		{
            if (Spawn == null)
                return;

			Rectangle2D rec = SpawnRecs[Utility.Random(SpawnRecs.Length)];
            ConvertOffset(ref rec);

			while(true)
			{
				int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
				int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
				int z = Map.TerMur.GetAverageZ(x, y);
				
				if(Map.TerMur.CanSpawnMobile(x, y, z))
				{
					var armor = new EnsorcelledArmor(this);
					armor.MoveToWorld(new Point3D(x, y, z), Map.TerMur);
					Spawn.Add(armor);
					break;
				}
			}
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(Armor == null ? 0 : Armor.Count);
            if (Armor != null) Armor.ForEach(a => writer.Write(a));

            writer.Write(DestroyedArmor == null ? 0 : DestroyedArmor.Count);
            if (DestroyedArmor != null) DestroyedArmor.ForEach(a => writer.Write(a));

            writer.Write(Spawn == null ? 0 : Spawn.Count);
            if (Spawn != null) Spawn.ForEach(a => writer.Write(a));

            writer.Write(Items == null ? 0 : Items.Count);
            if (Items != null) Items.ForEach(i => writer.Write(i));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Armor = new List<Item>();
            DestroyedArmor = new List<Item>();
            Items = new List<Item>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item it = reader.ReadItem();

                if (it != null)
                {
                    if(it is CursedSuitOfArmor)
                        ((CursedSuitOfArmor)it).Encounter = this;

                    Armor.Add(it);
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item it = reader.ReadItem();

                if (it != null)
                    DestroyedArmor.Add(it);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (Spawn == null)
                    Spawn = new List<BaseCreature>();

                BaseCreature bc = reader.ReadMobile() as BaseCreature;
                if (bc != null)
                {
                    if (bc is EnsorcelledArmor)
                        ((EnsorcelledArmor)bc).Encounter = this;

                    Spawn.Add(bc);
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (Items == null)
                    Items = new List<Item>();

                Item item = reader.ReadItem();
                if (item != null)
                {
                    Items.Add(item);
                }
            }

            if (Spawn == null || Spawn.Count < 4)
            {
                int toSpawn = Spawn == null ? 4 : 4 - Spawn.Count;
                for (int i = 0; i < toSpawn; i++)
                {
                    if(Spawn == null)
                        Spawn = new List<BaseCreature>();

                    SpawnRandom();
                }
            }
        }
	}
	
	public class FountainEncounter : ShadowguardEncounter
	{
        public static readonly int MaxElementals = 30;

		public List<BaseCreature> Elementals { get; set; }
		public List<Item> ShadowguardCanals { get; set; }
	
		public List<FlowChecker> FlowCheckers { get; set; }

        public DateTime _NextSpawn;

        public override Type AddonType { get { return typeof(ShadowguardFountainAddon); } }

        public FountainEncounter()
            : base(EncounterType.Fountain)
        {
        }

        public FountainEncounter(ShadowguardInstance instance)
            : base(EncounterType.Fountain, instance)
		{
		}
		
		public void UseSpigot(ShadowguardSpigot spigot, Mobile m)
		{
            if (FlowCheckers == null)
                return;

			foreach(FlowChecker checker in FlowCheckers)
			{
				if(checker.CheckUse(spigot, m))
					return;
			}
		}

        public override void CheckEncounter()
        {
            if (FlowCheckers != null && FlowCheckers.Where(c => c.Complete).Count() == 4)
                CompleteEncounter();
        }
		
		public override void OnCreatureKilled(BaseCreature bc)
		{
            if (Elementals != null && Elementals.Contains(bc))
			{
				Elementals.Remove(bc);
				Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(1, 5)), SpawnRandomElemental);
			}
		}

        public override void OnTick()
        {
            if (_NextSpawn < DateTime.UtcNow && (Elementals == null || Elementals.Count < MaxElementals))
            {
                SpawnRandomElemental();
                _NextSpawn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45));
            }
        }
		
		public override void Setup()
		{
			ShadowguardCanals = new List<Item>();
			Elementals = new List<BaseCreature>();
			FlowCheckers = new List<FlowChecker>();

            int toSpawn = 3 + (PartySize() * 2);

			Timer.DelayCall(ShadowguardController.ReadyDuration + TimeSpan.FromSeconds(30), () =>
			{
				for(int i = 0; i < toSpawn; i++)
					SpawnRandomElemental();
			});
			
			for(int i = 0; i < 4; i++)
			{
                ShadowguardSpigot spigot = new ShadowguardSpigot(i < 2 ? 0x9BF2 : 0x9BE5);
                Point3D p = SpawnPoints[i];

                ConvertOffset(ref p);

				spigot.MoveToWorld(p, Map.TerMur);
                AddShadowguardCanal(spigot);
				FlowCheckers.Add(new FlowChecker(spigot, this));
			}

            Rectangle2D rec1 = SpawnRecs[4];
            Rectangle2D rec2 = SpawnRecs[5];

            ConvertOffset(ref rec1);
            ConvertOffset(ref rec2);

            SpawnDrain(rec1);
            SpawnDrain(rec2);
		}
		
		public override void ClearItems()
		{
            if (ShadowguardCanals != null)
            {
                List<Item> list = new List<Item>(ShadowguardCanals.Where(i => i != null && !i.Deleted));

                foreach (var canal in list)
                    canal.Delete();

                ColUtility.Free(list);

                ColUtility.Free(ShadowguardCanals);
                ShadowguardCanals = null;
            }

            if (Elementals != null)
            {
                List<BaseCreature> list = new List<BaseCreature>(Elementals.Where(t => t != null && !t.Deleted));

                foreach (var elemental in list)
                    elemental.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Elementals);
                Elementals = null;
            }

            if (FlowCheckers != null)
            {
                ColUtility.ForEach(FlowCheckers.Where(f => f != null), f => f.EndEncounter());
            }
		}
		
		private void SpawnRandomElemental()
		{
            if (Elementals == null)
                return;

			Rectangle2D rec = SpawnRecs[Utility.RandomMinMax(0, 3)];
            ConvertOffset(ref rec);

			while(true)
			{
				int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
				int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
				int z = Map.TerMur.GetAverageZ(x, y);
				
				if(Map.TerMur.CanSpawnMobile(x, y, z))
				{
					BaseCreature elemental = new VileWaterElemental();

					elemental.MoveToWorld(new Point3D(x, y, z), Map.TerMur);
					Elementals.Add(elemental);
					break;
				}
			}
		}

        private void SpawnDrain(Rectangle2D rec)
        {
            int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
            int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
            int z = Map.TerMur.GetAverageZ(x, y);

            ShadowguardDrain drain = new ShadowguardDrain();

            drain.MoveToWorld(new Point3D(x, y, z), Map.TerMur);
            AddShadowguardCanal(drain);
        }

        public void SpawnBaddie(Mobile m)
        {
            if (Elementals == null)
                return;

            Point3D p = m.Location;

            for (int i = 0; i < 10; i++)
            {
                int x = Utility.RandomMinMax(p.X - 1, p.X + 1);
                int y = Utility.RandomMinMax(p.Y - 1, p.Y + 1);

                if (Map.TerMur.CanSpawnMobile(x, y, -20))
                {
                    p = new Point3D(x, y, -20);
                    break;
                }
            }

            BaseCreature creature = new HurricaneElemental();

            creature.MoveToWorld(p, Map.TerMur);
            Elementals.Add(creature);

            creature.Combatant = m;
        }

        public void AddShadowguardCanal(Item canal)
        {
            if(ShadowguardCanals != null && !ShadowguardCanals.Contains(canal))
                ShadowguardCanals.Add(canal);
        }
		
		public class FlowChecker
		{
			private FountainEncounter _Encounter;
			private List<ShadowguardCanal> _Checked;
			
			private ShadowguardSpigot _Spigot;
			private ShadowguardDrain _Drain;
			
			public bool Complete { get { return _Spigot != null && _Drain != null; } }
			
			public FlowChecker(ShadowguardSpigot start, FountainEncounter encounter)
			{
				_Spigot = start;
				_Encounter = encounter;
			}
			
			public void EndEncounter()
			{
				if(_Spigot != null)
					_Spigot.Delete();
					
				if(_Checked != null)
				{
                    ColUtility.Free(_Checked);
					_Checked = null;
				}
				
				_Encounter = null;
			}
			
			public bool CheckUse(ShadowguardSpigot spigot, Mobile m)
			{
                if (spigot != _Spigot)
					return false;
					
				Check(m);
				return true;
			}
			
			public void Check(Mobile m)
			{
				Point3D p;
				Map map = Map.TerMur;
                bool southFacing = _Spigot.ItemID == 39922;

                if(_Checked != null)
                    _Checked.Clear();

                if (southFacing)
                    p = new Point3D(_Spigot.X, _Spigot.Y + 1, -20);
				else
                    p = new Point3D(_Spigot.X + 1, _Spigot.Y, -20);
					
				Item item = FindItem(p);

                if (item is ShadowguardCanal)
                {
                    ShadowguardCanal canal = item as ShadowguardCanal;

                    if ((southFacing && (canal.Flow == Flow.NorthSouth || canal.Flow == Flow.SouthEastCorner || canal.Flow == Flow.SouthWestCorner)) ||
                       (!southFacing && (canal.Flow == Flow.EastWest || canal.Flow == Flow.SouthEastCorner || canal.Flow == Flow.NorthEastCorner)))
                    {
                        if (_Checked == null)
                            _Checked = new List<ShadowguardCanal>();

                        _Checked.Add(canal);

                        RecursiveCheck(item, null);
                    }
                }
				
				if(Complete)
				{
					Fill();
					Timer.DelayCall(TimeSpan.FromSeconds(2), () => _Encounter.CheckEncounter());
				}
				else
					_Encounter.SpawnBaddie(m);
			}
			
			public void RecursiveCheck(Item item, Item last)
			{
				Item next = null;
				
				for(int i = 0; i < _Offsets.Length; i += 2)
				{
					Point3D p = new Point3D(item.X + _Offsets[i], item.Y + _Offsets[i+1], item.Z);
					next = FindItem(p);
					
					if(next != null && next != last)
					{
						if(Connects(item, next))
						{
							if(next is ShadowguardDrain)
							{
								_Drain = (ShadowguardDrain)next;
								return;
							}
                            else if (next is ShadowguardCanal)
                            {
                                if (_Checked == null)
                                    _Checked = new List<ShadowguardCanal>();

                                _Checked.Add((ShadowguardCanal)next);

                                RecursiveCheck(next, item);
                            }
						}
					}
				}
			}
			
			public bool Connects(Item one, Item two)
			{
				if(one is ShadowguardCanal && two is ShadowguardDrain)
				{
					Flow flow = ((ShadowguardCanal)one).Flow;
					Direction d = Utility.GetDirection(one, two);
                    bool canConnect = false;

					switch(d)
					{
                        case Direction.North: canConnect = flow == Flow.NorthSouth || flow == Flow.SouthEastCorner || flow == Flow.SouthWestCorner; break;
                        case Direction.East: canConnect = flow == Flow.EastWest || flow == Flow.NorthWestCorner || flow == Flow.SouthWestCorner; break;
                        case Direction.South: canConnect = flow == Flow.NorthSouth || flow == Flow.NorthEastCorner || flow == Flow.NorthWestCorner; break;
                        case Direction.West: canConnect = flow == Flow.EastWest || flow == Flow.NorthEastCorner || flow == Flow.SouthEastCorner; break;
					}

                    return canConnect;
				}
				else if (two is ShadowguardCanal)
				{
					return ((ShadowguardCanal)one).Connects((ShadowguardCanal)two);
				}
				
				return false;
			}
			
			public Item FindItem(Point3D p)
			{
				IPooledEnumerable eable = Map.TerMur.GetItemsInRange(p, 0);
				
				foreach(Item i in eable)
				{
					if(i.Z == p.Z && (i is ShadowguardCanal || i is ShadowguardDrain))
					{
						eable.Free();
						return i;
					}
				}
				
				eable.Free();
				return null;
			}
			
			private void Fill()
			{
                int time = 200;

                if (_Spigot.ItemID == 39922)
                    _Spigot.ItemID = 17294;
                else if (_Spigot.ItemID == 39909)
                    _Spigot.ItemID = 17278;

				_Checked.ForEach(i => i.Movable = false);
                ColUtility.For(_Checked, (i, item) =>
				{
                    Timer.DelayCall(TimeSpan.FromMilliseconds(time), Fill, item);
                    time += 200;
				});
			}

            private void Fill(object o)
            {
                if (o is ShadowguardCanal)
                {
                    ((ShadowguardCanal)o).Fill();

                    if (_Checked.IndexOf((ShadowguardCanal)o) == _Checked.Count - 1)
                        _Drain.Hue = 0;
                }
            }
			
			private int[] _Offsets = new int[]
			{
				0, -1,
				1, 0,
				0, 1,
				-1, 0
			};
            
            public FlowChecker(GenericReader reader, FountainEncounter encounter)
            {
                int version = reader.ReadInt();
                _Encounter = encounter;

                _Spigot = reader.ReadItem() as ShadowguardSpigot;
                _Drain = reader.ReadItem() as ShadowguardDrain;

                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    if (_Checked == null)
                        _Checked = new List<ShadowguardCanal>();

                    ShadowguardCanal c = reader.ReadItem() as ShadowguardCanal;

                    if (c != null)
                        _Checked.Add(c);
                }
            }

            public void Serialize(GenericWriter writer)
            {
                writer.Write(0);

                writer.Write(_Spigot);
                writer.Write(_Drain);

                writer.Write(_Checked == null ? 0 : _Checked.Count);
                if (_Checked != null) _Checked.ForEach(c => writer.Write(c));
            }
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Elementals == null ? 0 : Elementals.Count);
            if (Elementals != null) Elementals.ForEach(e => writer.Write(e));

            writer.Write(ShadowguardCanals == null ? 0 : ShadowguardCanals.Count);
            if (ShadowguardCanals != null) ShadowguardCanals.ForEach(c => writer.Write(c));

            writer.Write(FlowCheckers == null ? 0 : FlowCheckers.Count);
            if (FlowCheckers != null) FlowCheckers.ForEach(f => f.Serialize(writer));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Elementals = new List<BaseCreature>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                BaseCreature bc = reader.ReadMobile() as BaseCreature;

                if (bc != null)
                    Elementals.Add(bc);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (ShadowguardCanals == null)
                    ShadowguardCanals = new List<Item>();

                Item canal = reader.ReadItem();

                if (canal != null)
                    ShadowguardCanals.Add(canal);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (FlowCheckers == null)
                    FlowCheckers = new List<FlowChecker>();

                FlowCheckers.Add(new FlowChecker(reader, this));
            }

            if (Elementals == null || Elementals.Count < 8)
            {
                int toSpawn = Elementals == null ? 8 : 8 - Elementals.Count;

                for (int i = 0; i < toSpawn; i++)
                {
                    SpawnRandomElemental();
                }
            }
        }
	}
	
	public class BelfryEncounter : ShadowguardEncounter
	{
		public List<VileDrake> Drakes { get; set; }
		public ShadowguardGreaterDragon Dragon { get; set; }
        public List<Item> Bells { get; set; }

        public override Type AddonType { get { return typeof(BelfryAddon); } }

        public BelfryEncounter()
            : base(EncounterType.Belfry)
        {
        }

        public BelfryEncounter(ShadowguardInstance instance)
            : base(EncounterType.Belfry, instance)
		{
		}
				
		public override void Setup()
		{
			Drakes = new List<VileDrake>();
            Bells = new List<Item>();

            Point3D p = SpawnPoints[0];
            ConvertOffset(ref p);

			Dragon = new ShadowguardGreaterDragon();
			Dragon.MoveToWorld(p, Map.TerMur);

            FeedingBell bell = new FeedingBell();
            p = new Point3D(16, 6, 0);
            ConvertOffset(ref p);
            bell.MoveToWorld(p, Map.TerMur);
            Bells.Add(bell);

            bell = new FeedingBell();
            p = new Point3D(16, -7, 0);
            ConvertOffset(ref p);
            bell.MoveToWorld(p, Map.TerMur);
            Bells.Add(bell);

            bell = new FeedingBell();
            p = new Point3D(-20, -7, 0);
            ConvertOffset(ref p);
            bell.MoveToWorld(p, Map.TerMur);
            Bells.Add(bell);

            bell = new FeedingBell();
            p = new Point3D(-20, 6, 0);
            ConvertOffset(ref p);
            bell.MoveToWorld(p, Map.TerMur);
            Bells.Add(bell);
		}
		
		public void SpawnDrake(Point3D p, Mobile from)
		{
            if (Drakes == null)
                return;

            Rectangle2D rec = SpawnRecs[0];
            ConvertOffset(ref rec);

            foreach (Rectangle2D r in SpawnRecs)
            {
                Rectangle2D copy = r;
                ConvertOffset(ref copy);

                if (copy.Contains(p))
                {
                    rec = copy;
                    break;
                }
            }
			
			while(true)
			{
				int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
				int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
				int z = Map.TerMur.GetAverageZ(x, y);
				
				if(Map.TerMur.CanSpawnMobile(x, y, z))
				{
					var drake = new VileDrake();
					drake.MoveToWorld(new Point3D(x, y, z), Map.TerMur);

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () => drake.Combatant = from);

					Drakes.Add(drake);
					break;
				}
			}
		}
		
		public override void CheckEncounter()
		{
		}
		
		public override void OnCreatureKilled(BaseCreature bc)
		{
			if(bc is VileDrake && Drakes != null && Drakes.Contains((VileDrake)bc))
				Drakes.Remove((VileDrake)bc);
				
			if(bc == Dragon)
			{
				CompleteEncounter();
			}
		}
		
		public override void ClearItems()
		{
            if (Drakes != null)
            {
                List<BaseCreature> list = new List<BaseCreature>(Drakes.Where(d => d != null && !d.Deleted));

                foreach (var drake in list)
                    drake.Delete();

                ColUtility.Free(list);

                ColUtility.Free(Drakes);
                Drakes = null;
            }

            if (Bells != null)
            {
                List<Item> list = new List<Item>(Bells.Where(b => b != null && !b.Deleted));

                foreach (var bell in list)
                    bell.Delete();

                ColUtility.Free(list);
                ColUtility.Free(Bells);

                Bells = null;
            }

			if(Dragon != null && Dragon.Alive)
				Dragon.Delete();
				
			Dragon = null;
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Dragon);
            writer.Write(Drakes == null ? 0 : Drakes.Count);

            if (Drakes != null) Drakes.ForEach(d => writer.Write(d));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Dragon = reader.ReadMobile() as ShadowguardGreaterDragon;
            int count = reader.ReadInt();

            Drakes = new List<VileDrake>();

            for (int i = 0; i < count; i++)
            {
                VileDrake d = reader.ReadMobile() as VileDrake;

                if (d != null)
                    Drakes.Add(d);
            }

            if (Dragon == null || Dragon.Deleted)
                Expire();
        }
	}
	
	public class RoofEncounter : ShadowguardEncounter
	{
		public ShadowguardBoss CurrentBoss { get; set; }
        public LadyMinax Minax { get; set; }
		
		public List<Type> Bosses { get; set; }
		
		private Type[] _Bosses = new Type[] { typeof(Anon), typeof(Virtuebane), typeof(Ozymandias), typeof(Juonar) };

        public override TimeSpan EncounterDuration { get { return TimeSpan.MaxValue; } }
        public override TimeSpan ResetDuration { get { return TimeSpan.FromMinutes(5); } }

        public override Type AddonType { get { return null; } }

		public override void Setup()
		{
			Bosses = new List<Type>(_Bosses);
			
			for(int i = 0; i < 15; i++)
			{
				Type t = Bosses[Utility.Random(Bosses.Count)];
				Bosses.Remove(t);
				Bosses.Insert(0, t);
			}

            Point3D p = SpawnPoints[0];
            ConvertOffset(ref p);

            Minax = new LadyMinax();
            Minax.MoveToWorld(p, Map.TerMur);
            Minax.Home = p;
            Minax.RangeHome = 5;

            Timer.DelayCall(TimeSpan.FromSeconds(35), () =>
                {
                    SendPartyMessage(1156255); // Minax Says: Well Well! You've managed to get through my fortress but alas you shall never foil my plans! Perhaps you need some friends to play with? Muwahhahah!
                });

            Timer.DelayCall(TimeSpan.FromSeconds(40), SpawnBoss);
		}

        public RoofEncounter()
            : base(EncounterType.Roof)
        {
        }

        public RoofEncounter(ShadowguardInstance instance)
            : base(EncounterType.Roof, instance)
		{
		}
		
		public override void CheckEncounter()
		{
		}

        public override void CompleteEncounter()
        {
            base.CompleteEncounter();

            Controller.CompleteRoof(PartyLeader);
        }

		public override void OnCreatureKilled(BaseCreature bc)
		{
			if(Bosses != null && bc is ShadowguardBoss && bc == CurrentBoss)
			{
                if (Bosses.Count > 0)
                    SpawnBoss();
                else
                {
                    if (Minax != null && Minax.Alive)
                        Minax.Say(1156257); // How...How could this happen! I shall nay be bested by mere mortals! We shall meet again vile heroes!

                    GiveRewardTitle();

                    CompleteEncounter();
                }
			}
		}

        private void GiveRewardTitle()
        {
            Mobile m = PartyLeader;

            if (m == null)
                return;

            Party p = Party.Get(m);

            if (p != null)
            {
                foreach (PartyMemberInfo info in p.Members)
                {
                    if (info.Mobile is PlayerMobile && info.Mobile.Region.IsPartOf<ShadowguardRegion>())
                        ((PlayerMobile)info.Mobile).AddRewardTitle(1156318); // Destroyer of the Time Rift
                }
            }
            else if (m is PlayerMobile)
                ((PlayerMobile)m).AddRewardTitle(1156318); // Destroyer of the Time Rift
        }
		
		public override void ClearItems()
		{
            if (Minax != null)
                Minax.Delete();

            if (Bosses != null)
            {
                ColUtility.Free(Bosses);
                Bosses = null;
            }

            if (CurrentBoss != null)
            {
                if(!CurrentBoss.Deleted)
                    CurrentBoss.Delete();

                CurrentBoss = null;
            }
		}
		
		private void SpawnBoss()
		{
            if (Bosses == null)
                return;

            Point3D p = SpawnPoints[0];
            ConvertOffset(ref p);

			CurrentBoss = Activator.CreateInstance(Bosses[0]) as ShadowguardBoss;
			Bosses.Remove(Bosses[0]);
			
			if(CurrentBoss != null)
			{
				CurrentBoss.MoveToWorld(p, Map.TerMur);
			}

            if (Bosses.Count == 0)
                CurrentBoss.IsLastBoss = true;

            if (Minax != null && Minax.Alive)
            {
                if (CurrentBoss is Juonar)
                    Minax.Say(1156258); // You shall burn as Trinsic burned at the hands of the Vile Lich Juo'nar!
                else if (CurrentBoss is Anon)
                    Minax.Say(1156259); // Oh Anon my dear! Deal with these pesky intruders will you? Burn them to ASH!
                else if (CurrentBoss is Virtuebane)
                    Minax.Say(1156260); // You didn't think that ridiculous pie trick would work twice in a row? Virtuebane I command thee destroy these vile creatures!
                else
                    Minax.Say(1156261); // And now you shall bow to the King of Kings! Suffer at the hands of the Feudal Lord Ozymandias!
            }
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(CurrentBoss);
            writer.Write(Minax);
            writer.Write(Bosses == null ? 0 : Bosses.Count);

            if (Bosses != null) Bosses.ForEach(b => writer.Write(b.Name));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Bosses = new List<Type>();

            CurrentBoss = reader.ReadMobile() as ShadowguardBoss;
            Minax = reader.ReadMobile() as LadyMinax;

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Type boss = ScriptCompiler.FindTypeByName(reader.ReadString());

                if (boss != null)
                    Bosses.Add(boss);
            }

            if (CurrentBoss == null)
                Reset();
        }
	}
}
