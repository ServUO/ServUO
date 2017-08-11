using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class FountainOfFortune : BaseAddon
	{
		private Dictionary<Mobile, DateTime> m_ResCooldown;
		private Dictionary<Mobile, DateTime> m_RewardCooldown;

        public Dictionary<Mobile, DateTime> ResCooldown { get { return m_ResCooldown; } }
        public Dictionary<Mobile, DateTime> RewardCooldown { get { return m_RewardCooldown; } }

        private static readonly int LuckBonus = 400;

        private static List<FountainOfFortune> m_Fountains = new List<FountainOfFortune>();
        private static Dictionary<Mobile, DateTime> m_LuckTable = new Dictionary<Mobile, DateTime>();

        private static Dictionary<Mobile, DateTime> m_SpecialProtection = new Dictionary<Mobile, DateTime>();
        private static Dictionary<Mobile, DateTime> m_BalmBoost = new Dictionary<Mobile, DateTime>();

        public static Dictionary<Mobile, DateTime> SpecialProtection { get { return m_SpecialProtection; } }
        public static Dictionary<Mobile, DateTime> BalmBoost { get { return m_BalmBoost; } }

        private static Timer m_Timer;

        [Constructable]
		public FountainOfFortune()
		{
            int itemID = 0x1731;

            AddComponent(new AddonComponent(itemID++), -2, +1, 0);
            AddComponent(new AddonComponent(itemID++), -1, +1, 0);
            AddComponent(new AddonComponent(itemID++), +0, +1, 0);
            AddComponent(new AddonComponent(itemID++), +1, +1, 0);

            AddComponent(new AddonComponent(itemID++), +1, +0, 0);
            AddComponent(new AddonComponent(itemID++), +1, -1, 0);
            AddComponent(new AddonComponent(itemID++), +1, -2, 0);

            AddComponent(new AddonComponent(itemID++), +0, -2, 0);
            AddComponent(new AddonComponent(itemID++), +0, -1, 0);
            AddComponent(new AddonComponent(itemID++), +0, +0, 0);

            AddComponent(new AddonComponent(itemID++), -1, +0, 0);
            AddComponent(new AddonComponent(itemID++), -2, +0, 0);

            AddComponent(new AddonComponent(itemID++), -2, -1, 0);
            AddComponent(new AddonComponent(itemID++), -1, -1, 0);

            AddComponent(new AddonComponent(itemID++), -1, -2, 0);
            AddComponent(new AddonComponent(++itemID), -2, -2, 0);

			m_ResCooldown = new Dictionary<Mobile, DateTime>();
			m_RewardCooldown = new Dictionary<Mobile, DateTime>();

            Movable = false;

            AddFountain(this);
		}
		
		public bool OnTarget( Mobile from, Item coin )
		{
			DefragTables();
			
			if(IsCoolingDown(from))
			{
                from.SendLocalizedMessage(1113368); // You already made a wish today. Try again tomorrow!
				return false;
			}
			
			if(.20 >= Utility.RandomDouble())
			{
				Item item = null;
				switch(Utility.Random(4))
				{
					case 0: item = new SolesOfProvidence(); break;
					case 1: item = new GemologistsSatchel(); break;
					case 2: item = new RelicFragment(5); break;
					case 3: item = new EnchantedEssence(5); break;
				}
				
				if(from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
					item.MoveToWorld(from.Location, from.Map);
			}
			else
			{
				switch(Utility.Random(4))
				{
					case 0:
						from.AddStatMod(new StatMod(StatType.Str, "FoF_Str", 10, TimeSpan.FromMinutes(60)));
                        from.SendLocalizedMessage(1113373); // You suddenly feel stronger!
						break;
					case 1:
                        from.AddStatMod(new StatMod(StatType.Dex, "FoF_Dex", 10, TimeSpan.FromMinutes(60)));
                        from.SendLocalizedMessage(1113374); // You suddenly feel more agile!
						break;
					case 2:
                        from.AddStatMod(new StatMod(StatType.Int, "FoF_Int", 10, TimeSpan.FromMinutes(60)));
						from.SendLocalizedMessage(1113371); // You suddenly feel wiser!
						break;
					case 3:
                        m_LuckTable[from] = DateTime.UtcNow + TimeSpan.FromMinutes(60);
                        from.SendLocalizedMessage(1079551); // Your luck just improved!
						break;
                    case 4:
                        m_SpecialProtection[from] = DateTime.UtcNow + TimeSpan.FromMinutes(60);
                        from.SendLocalizedMessage(1113375); // You suddenly feel less vulnerable!
                        break;
                    case 5:
                        m_BalmBoost[from] = DateTime.UtcNow + TimeSpan.FromMinutes(60);
                        from.SendLocalizedMessage(1113372); // The duration of your balm has been increased by an hour!
                        break;
				}

                from.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
			}

            from.PlaySound(0x22);

			m_RewardCooldown[from] = DateTime.UtcNow + TimeSpan.FromHours(24);

            if (coin.Amount <= 1)
                coin.Delete();
            else
			    coin.Amount--;

			return false;
		}

        public bool IsCoolingDown(Mobile from)
        {
            foreach (FountainOfFortune fountain in m_Fountains)
            {
                if (fountain.RewardCooldown != null && fountain.RewardCooldown.ContainsKey(from))
                    return true;
            }

            return false;
        }

        public static int GetLuckBonus(Mobile from)
        {
            if (m_LuckTable.ContainsKey(from))
                return LuckBonus;

            return 0;
        }

        public static bool UnderProtection(Mobile m)
        {
            if (m_SpecialProtection.ContainsKey(m))
                return true;

            return false;
        }
		
		public bool CanRes(Mobile m)
		{
			if(!m_ResCooldown.ContainsKey(m))
				return true;
				
			if(m_ResCooldown[m] < DateTime.UtcNow)
			{
				m_ResCooldown.Remove(m);
				return true;
			}
				
			return false;
		}
		
		public override bool HandlesOnMovement { get { return true; } }
		
		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if(m.Player && CanRes(m) && !m.Alive && m.InRange(this.Location, 5))
                m.SendGump(new ResurrectGump(m, m, ResurrectMessage.Generic, false, 0.0, Resurrect_Callback));
		}
		
		public void Resurrect_Callback(Mobile m)
		{
			m_ResCooldown[m] = DateTime.UtcNow + TimeSpan.FromMinutes(10);
		}
		
		public static void DefragTables()
		{
            foreach (FountainOfFortune fountain in m_Fountains)
            {
                List<Mobile> list = new List<Mobile>(fountain.ResCooldown.Keys);
                List<Mobile> list2 = new List<Mobile>(fountain.RewardCooldown.Keys);

                foreach (Mobile m in list)
                {
                    if (fountain.ResCooldown.ContainsKey(m) && fountain.ResCooldown[m] < DateTime.UtcNow)
                        fountain.ResCooldown.Remove(m);
                }

                foreach (Mobile m in list2)
                {
                    if (fountain.RewardCooldown.ContainsKey(m) && fountain.RewardCooldown[m] < DateTime.UtcNow)
                        fountain.RewardCooldown.Remove(m);
                }

                list.Clear();
                list2.Clear();
            }

            List<Mobile> remove = new List<Mobile>();
            foreach (KeyValuePair<Mobile, DateTime> kvp in m_LuckTable)
            {
                if (kvp.Value < DateTime.UtcNow)
                    remove.Add(kvp.Key);
            }

            remove.ForEach(m =>
                {
                    m_LuckTable.Remove(m);

                    if (m.NetState != null)
                        m.SendLocalizedMessage(1079552); //Your luck just ran out.
                });

            remove.Clear();

            foreach (KeyValuePair<Mobile, DateTime> kvp in m_SpecialProtection)
            {
                if (kvp.Value < DateTime.UtcNow)
                    remove.Add(kvp.Key);
            }

            remove.ForEach(m =>
            {
                m_SpecialProtection.Remove(m);
            });

            remove.Clear();

            foreach (KeyValuePair<Mobile, DateTime> kvp in m_BalmBoost)
            {
                if (kvp.Value < DateTime.UtcNow)
                    remove.Add(kvp.Key);
            }

            remove.ForEach(m =>
            {
                m_BalmBoost.Remove(m);
            });

            remove.Clear();
		}

        public override void Delete()
        {
            base.Delete();

            RemoveFountain(this);

            if (m_ResCooldown != null)
                m_ResCooldown.Clear();

            if (m_RewardCooldown != null)
                m_RewardCooldown.Clear();
        }

        public static void AddFountain(FountainOfFortune fountain)
        {
            if (!m_Fountains.Contains(fountain))
            {
                m_Fountains.Add(fountain);
                StartTimer();
            }
        }

        public static void RemoveFountain(FountainOfFortune fountain)
        {
            if (m_Fountains.Contains(fountain))
                m_Fountains.Remove(fountain);

            if (m_Fountains.Count == 0 && m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public static void StartTimer()
        {
            if (m_Timer != null && m_Timer.Running)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(DefragTables));
            m_Timer.Start();
        }
		
		public FountainOfFortune(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			m_ResCooldown = new Dictionary<Mobile, DateTime>();
			m_RewardCooldown = new Dictionary<Mobile, DateTime>();

            AddFountain(this);
		}
	}
}