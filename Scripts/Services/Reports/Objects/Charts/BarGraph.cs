using System;
using System.Collections;

namespace Server.Engines.Reports
{
    public enum BarGraphRenderMode
    {
        Bars,
        Lines
    }

    public class BarGraph : Chart
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("bg", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new BarGraph();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private int m_Ticks;
        private BarGraphRenderMode m_RenderMode;

        private string m_xTitle;
        private string m_yTitle;

        private int m_FontSize = 7;
        private int m_Interval = 1;

        private BarRegion[] m_Regions;

        public int Ticks
        {
            get
            {
                return this.m_Ticks;
            }
            set
            {
                this.m_Ticks = value;
            }
        }
        public BarGraphRenderMode RenderMode
        {
            get
            {
                return this.m_RenderMode;
            }
            set
            {
                this.m_RenderMode = value;
            }
        }

        public string xTitle
        {
            get
            {
                return this.m_xTitle;
            }
            set
            {
                this.m_xTitle = value;
            }
        }
        public string yTitle
        {
            get
            {
                return this.m_yTitle;
            }
            set
            {
                this.m_yTitle = value;
            }
        }

        public int FontSize
        {
            get
            {
                return this.m_FontSize;
            }
            set
            {
                this.m_FontSize = value;
            }
        }
        public int Interval
        {
            get
            {
                return this.m_Interval;
            }
            set
            {
                this.m_Interval = value;
            }
        }

        public BarRegion[] Regions
        {
            get
            {
                return this.m_Regions;
            }
            set
            {
                this.m_Regions = value;
            }
        }

        public BarGraph(string name, string fileName, int ticks, string xTitle, string yTitle, BarGraphRenderMode rm)
        {
            this.m_Name = name;
            this.m_FileName = fileName;
            this.m_Ticks = ticks;
            this.m_xTitle = xTitle;
            this.m_yTitle = yTitle;
            this.m_RenderMode = rm;
        }

        private BarGraph()
        {
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            base.SerializeAttributes(op);

            op.SetInt32("t", this.m_Ticks);
            op.SetInt32("r", (int)this.m_RenderMode);

            op.SetString("x", this.m_xTitle);
            op.SetString("y", this.m_yTitle);

            op.SetInt32("s", this.m_FontSize);
            op.SetInt32("i", this.m_Interval);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            base.DeserializeAttributes(ip);

            this.m_Ticks = ip.GetInt32("t");
            this.m_RenderMode = (BarGraphRenderMode)ip.GetInt32("r");

            this.m_xTitle = Utility.Intern(ip.GetString("x"));
            this.m_yTitle = Utility.Intern(ip.GetString("y"));

            this.m_FontSize = ip.GetInt32("s");
            this.m_Interval = ip.GetInt32("i");
        }

        public static int LookupReportValue(Snapshot ss, string reportName, string valueName)
        {
            for (int j = 0; j < ss.Children.Count; ++j)
            {
                Report report = ss.Children[j] as Report;

                if (report == null || report.Name != reportName)
                    continue;

                for (int k = 0; k < report.Items.Count; ++k)
                {
                    ReportItem item = report.Items[k];

                    if (item.Values[0].Value == valueName)
                        return Utility.ToInt32(item.Values[1].Value);
                }

                break;
            }

            return -1;
        }

        public static BarGraph DailyAverage(SnapshotHistory history, string reportName, string valueName)
        {
            int[] totals = new int[24];
            int[] counts = new int[24];

            int min = history.Snapshots.Count - (7 * 24); // averages over one week

            if (min < 0)
                min = 0;

            for (int i = min; i < history.Snapshots.Count; ++i)
            {
                Snapshot ss = history.Snapshots[i];

                int val = LookupReportValue(ss, reportName, valueName);

                if (val == -1)
                    continue;

                int hour = ss.TimeStamp.TimeOfDay.Hours;

                totals[hour] += val;
                counts[hour]++;
            }

            BarGraph barGraph = new BarGraph("Hourly average " + valueName, "graphs_" + valueName.ToLower() + "_avg", 10, "Time", valueName, BarGraphRenderMode.Lines);

            barGraph.m_FontSize = 6;

            for (int i = 7; i <= totals.Length + 7; ++i)
            {
                int val;

                if (counts[i % totals.Length] == 0)
                    val = 0;
                else
                    val = (totals[i % totals.Length] + (counts[i % totals.Length] / 2)) / counts[i % totals.Length];

                int realHours = i % totals.Length;
                int hours;

                if (realHours == 0)
                    hours = 12;
                else if (realHours > 12)
                    hours = realHours - 12;
                else
                    hours = realHours;

                barGraph.Items.Add(hours + (realHours >= 12 ? " PM" : " AM"), val);
            }

            return barGraph;
        }

        public static BarGraph Growth(SnapshotHistory history, string reportName, string valueName)
        {
            BarGraph barGraph = new BarGraph("Growth of " + valueName + " over time", "graphs_" + valueName.ToLower() + "_growth", 10, "Time", valueName, BarGraphRenderMode.Lines);

            barGraph.FontSize = 6;
            barGraph.Interval = 7;

            DateTime startPeriod = history.Snapshots[0].TimeStamp.Date + TimeSpan.FromDays(1.0);
            DateTime endPeriod = history.Snapshots[history.Snapshots.Count - 1].TimeStamp.Date;

            ArrayList regions = new ArrayList();

            DateTime curDate = DateTime.MinValue;
            int curPeak = -1;
            int curLow = 1000;
            int curTotl = 0;
            int curCont = 0;
            int curValu = 0;

            for (int i = 0; i < history.Snapshots.Count; ++i)
            {
                Snapshot ss = history.Snapshots[i];
                DateTime timeStamp = ss.TimeStamp;

                if (timeStamp < startPeriod || timeStamp >= endPeriod)
                    continue;

                int val = LookupReportValue(ss, reportName, valueName);

                if (val == -1)
                    continue;

                DateTime thisDate = timeStamp.Date;

                if (curDate == DateTime.MinValue)
                    curDate = thisDate;

                curCont++;
                curTotl += val;
                curValu = curTotl / curCont;

                if (curDate != thisDate && curValu >= 0)
                {
                    string mnthName = thisDate.ToString("MMMM");

                    if (regions.Count == 0)
                    {
                        regions.Add(new BarRegion(barGraph.Items.Count, barGraph.Items.Count, mnthName));
                    }
                    else
                    {
                        BarRegion region = (BarRegion)regions[regions.Count - 1];

                        if (region.m_Name == mnthName)
                            region.m_RangeTo = barGraph.Items.Count;
                        else
                            regions.Add(new BarRegion(barGraph.Items.Count, barGraph.Items.Count, mnthName));
                    }

                    barGraph.Items.Add(thisDate.Day.ToString(), curValu);

                    curPeak = val;
                    curLow = val;
                }
                else
                {
                    if (val > curPeak)
                        curPeak = val;

                    if (val > 0 && val < curLow)
                        curLow = val;
                }

                curDate = thisDate;
            }

            barGraph.Regions = (BarRegion[])regions.ToArray(typeof(BarRegion));

            return barGraph;
        }

        public static BarGraph OverTime(SnapshotHistory history, string reportName, string valueName, int step, int max, int ival)
        {
            BarGraph barGraph = new BarGraph(valueName + " over time", "graphs_" + valueName.ToLower() + "_ot", 10, "Time", valueName, BarGraphRenderMode.Lines);

            TimeSpan ts = TimeSpan.FromHours((max * step) - 0.5);

            DateTime mostRecent = history.Snapshots[history.Snapshots.Count - 1].TimeStamp;
            DateTime minTime = mostRecent - ts;

            barGraph.FontSize = 6;
            barGraph.Interval = ival;

            ArrayList regions = new ArrayList();

            for (int i = 0; i < history.Snapshots.Count; ++i)
            {
                Snapshot ss = history.Snapshots[i];
                DateTime timeStamp = ss.TimeStamp;

                if (timeStamp < minTime)
                    continue;

                if ((i % step) != 0)
                    continue;

                int val = LookupReportValue(ss, reportName, valueName);

                if (val == -1)
                    continue;

                int realHours = timeStamp.TimeOfDay.Hours;
                int hours;

                if (realHours == 0)
                    hours = 12;
                else if (realHours > 12)
                    hours = realHours - 12;
                else
                    hours = realHours;

                string dayName = timeStamp.DayOfWeek.ToString();

                if (regions.Count == 0)
                {
                    regions.Add(new BarRegion(barGraph.Items.Count, barGraph.Items.Count, dayName));
                }
                else
                {
                    BarRegion region = (BarRegion)regions[regions.Count - 1];

                    if (region.m_Name == dayName)
                        region.m_RangeTo = barGraph.Items.Count;
                    else
                        regions.Add(new BarRegion(barGraph.Items.Count, barGraph.Items.Count, dayName));
                }

                barGraph.Items.Add(hours + (realHours >= 12 ? " PM" : " AM"), val);
            }

            barGraph.Regions = (BarRegion[])regions.ToArray(typeof(BarRegion));

            return barGraph;
        }
    }
}