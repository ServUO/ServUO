using System.IO;
using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using System.Linq;

namespace Server.Engines.VoidPool
{
	public static class VoidPoolStats
	{
		public static string FilePath = Path.Combine("Saves/VoidPoolStats", "VoidPoolStats.bin");
		
		public static Dictionary<Mobile, long> BestSingle { get; set; }
		public static Dictionary<Mobile, long> OverallTotal { get; set; }
		public static List<Dictionary<Mobile, long>> Top20 { get; set; }
        public static BestWave BestWave { get; set; }
		
		public static void Configure()
		{
			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;
		}
		
		public static void OnSave(WorldSaveEventArgs e)
		{
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)0);

                    if (BestWave != null)
                    {
                        writer.Write(1);
                        BestWave.Serialize(writer);
                    }
                    else
                        writer.Write(0);

                    writer.Write(BestSingle.Count);
                    foreach (KeyValuePair<Mobile, long> kvp in BestSingle)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }

                    writer.Write(OverallTotal.Count);
                    foreach (KeyValuePair<Mobile, long> kvp in OverallTotal)
                    {
                        writer.Write(kvp.Key);
                        writer.Write(kvp.Value);
                    }

                    writer.Write(Top20.Count);
                    foreach(Dictionary<Mobile, long> dic in Top20)
                    {
                        writer.Write(dic.Count);
                        foreach (KeyValuePair<Mobile, long> kvp in dic)
                        {
                            writer.Write(kvp.Key);
                            writer.Write(kvp.Value);
                        }
                    };
                });
		}

        public static void OnLoad()
        {
            BestSingle = new Dictionary<Mobile, long>();
            OverallTotal = new Dictionary<Mobile, long>();
            Top20 = new List<Dictionary<Mobile, long>>();

            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    if (reader.ReadInt() == 1)
                        BestWave = new BestWave(reader);

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        long l = reader.ReadLong();

                        if (m != null)
                            BestSingle[m] = l;
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        long l = reader.ReadLong();

                        if (m != null)
                            OverallTotal[m] = l;
                    }

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        int c = reader.ReadInt();
                        Dictionary<Mobile, long> dic = new Dictionary<Mobile, long>();
                        for (int j = 0; j < c; j++)
                        {
                            Mobile m = reader.ReadMobile();
                            long l = reader.ReadLong();

                            if (m != null)
                                dic[m] = l;
                        }

                        if (dic.Count > 0)
                            Top20.Add(dic);
                    }
                });
        }
		
		public static bool CheckBestSingle(Dictionary<Mobile, long> score)
		{
			foreach(KeyValuePair<Mobile, long> kvp in score)
			{
				if(!BestSingle.ContainsKey(kvp.Key) || kvp.Value > BestSingle[kvp.Key])
				{
					BestSingle[kvp.Key] = kvp.Value;
					return true;
				}
			}

            return false;
		}
		
		public static void AddToOverallTotal(Dictionary<Mobile, long> score)
		{
			foreach(KeyValuePair<Mobile, long> kvp in score)
			{
				if(!OverallTotal.ContainsKey(kvp.Key))
					OverallTotal[kvp.Key] = kvp.Value;
				else
					OverallTotal[kvp.Key] += kvp.Value;
			}
		}
		
		public static bool CheckAddTop20(Dictionary<Mobile, long> score)
		{
			long total = GetCollectiveScore(score);
			
			List<Dictionary<Mobile, long>> copy = new List<Dictionary<Mobile, long>>(Top20);
			
			foreach(Dictionary<Mobile, long> s in copy.OrderBy(dic => -GetCollectiveScore(dic)))
			{
				if(total > GetCollectiveScore(s))
				{
					Top20.Remove(copy[copy.Count - 1]);
					Top20.Add(s);
					return true;
				}
			}
			
			return false;
		}
		
		public static long GetCollectiveScore(Dictionary<Mobile, long> score)
		{
			if(score == null)
				return 0;
				
			long s = 0;
				
			foreach(long i in score.Values)
			{
				s += i;
			}
			
			return s;
		}

        public static long GetCollectiveScore(Mobile m)
        {
            if (OverallTotal.ContainsKey(m))
                return OverallTotal[m];

            return 0;
        }

        public static void CheckBestWave(Dictionary<Mobile, long> score, int wave)
        {
            if (BestWave == null || wave > BestWave.Waves)
            {
                BestWave = new BestWave(score, wave);
                Timer.DelayCall(TimeSpan.FromSeconds(1.5), () => World.Broadcast(2072, false, String.Format("A new Void Pool Invasion record has been made: {0}!", wave.ToString())));
            }
        }

        public static void OnInvasionEnd(Dictionary<Mobile, long> score, int wave)
        {
            CheckAddTop20(score);
            CheckBestSingle(score);
            AddToOverallTotal(score);
            CheckBestWave(score, wave);
        }
	}

    public class BestWave
    {
        public Dictionary<Mobile, long> Score { get; private set; }
        public int Waves { get; private set; }
        public long TotalScore { get; private set; }
        public string Date { get; private set; }

        public BestWave(Dictionary<Mobile, long> score, int waves)
        {
            Score = score;

            TotalScore = VoidPoolStats.GetCollectiveScore(score);
            Waves = waves;
            Date = DateTime.Now.ToShortDateString();
        }

        public BestWave(GenericReader reader)
        {
            int version = reader.ReadInt();
            Waves = reader.ReadInt();
            TotalScore = reader.ReadLong();
            Date = reader.ReadString();

            Score = new Dictionary<Mobile, long>();

            int cnt = reader.ReadInt();
            for (int i = 0; i < cnt; i++)
            {
                Mobile m = reader.ReadMobile();
                long score = reader.ReadLong();
                if (m != null)
                    Score[m] = score;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);
            writer.Write(Waves);
            writer.Write(TotalScore);
            writer.Write(Date);

            writer.Write(Score.Count);
            foreach (KeyValuePair<Mobile, long> kvp in Score)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }
    }
}