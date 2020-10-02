using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Misc
{
    public class Weather
    {
        private static readonly Dictionary<Map, List<Weather>> m_WeatherByFacet = new Dictionary<Map, List<Weather>>();
        private static readonly string m_TimerID = "WeatherTimer";

        private static Map[] m_Facets;
        private readonly Map m_Facet;
        private Rectangle2D[] m_Area;
        private int m_Temperature;
        private int m_ChanceOfPercipitation;
        private int m_ChanceOfExtremeTemperature;
        // For dynamic weather:
        private Rectangle2D m_Bounds;
        private int m_MoveSpeed;
        private int m_MoveAngleX, m_MoveAngleY;
        private int m_Stage;
        private bool m_Active;
        private bool m_ExtremeTemperature;
        public Weather(Map facet, Rectangle2D[] area, int temperature, int chanceOfPercipitation, int chanceOfExtremeTemperature, TimeSpan interval)
        {
            m_Facet = facet;
            m_Area = area;
            m_Temperature = temperature;
            m_ChanceOfPercipitation = chanceOfPercipitation;
            m_ChanceOfExtremeTemperature = chanceOfExtremeTemperature;

            List<Weather> list = GetWeatherList(facet);

            if (list != null)
                list.Add(this);

            TimerRegistry.Register(m_TimerID, this, interval, TimeSpan.FromSeconds((0.2 + (Utility.RandomDouble() * 0.8)) * interval.TotalSeconds), false, weather => weather.OnTick());
        }

        public Map Facet => m_Facet;

        public Rectangle2D[] Area
        {
            get
            {
                return m_Area;
            }
            set
            {
                m_Area = value;
            }
        }
        public int Temperature
        {
            get
            {
                return m_Temperature;
            }
            set
            {
                m_Temperature = value;
            }
        }
        public int ChanceOfPercipitation
        {
            get
            {
                return m_ChanceOfPercipitation;
            }
            set
            {
                m_ChanceOfPercipitation = value;
            }
        }
        public int ChanceOfExtremeTemperature
        {
            get
            {
                return m_ChanceOfExtremeTemperature;
            }
            set
            {
                m_ChanceOfExtremeTemperature = value;
            }
        }
        public Rectangle2D Bounds
        {
            get
            {
                return m_Bounds;
            }
            set
            {
                m_Bounds = value;
            }
        }
        public int MoveSpeed
        {
            get
            {
                return m_MoveSpeed;
            }
            set
            {
                m_MoveSpeed = value;
            }
        }
        public int MoveAngleX
        {
            get
            {
                return m_MoveAngleX;
            }
            set
            {
                m_MoveAngleX = value;
            }
        }
        public int MoveAngleY
        {
            get
            {
                return m_MoveAngleY;
            }
            set
            {
                m_MoveAngleY = value;
            }
        }
        public static void Initialize()
        {
            m_Facets = new Map[] { Map.Felucca, Map.Trammel };

            /* Static weather:
            * 
            * Format:
            *   AddWeather( temperature, chanceOfPercipitation, chanceOfExtremeTemperature, <area ...> );
            */

            // ice island
            AddWeather(-15, 100, 5, new Rectangle2D(3850, 160, 390, 320), new Rectangle2D(3900, 480, 380, 180), new Rectangle2D(4160, 660, 150, 110));

            // covetous entrance, around vesper and minoc
            AddWeather(+15, 50, 5, new Rectangle2D(2425, 725, 250, 250));

            // despise entrance, north of britain
            AddWeather(+15, 50, 5, new Rectangle2D(1245, 1045, 250, 250));

            /* Dynamic weather:
            * 
            * Format:
            *   AddDynamicWeather( temperature, chanceOfPercipitation, chanceOfExtremeTemperature, moveSpeed, width, height, bounds );
            */

            for (int i = 0; i < 15; ++i)
                AddDynamicWeather(+15, 100, 5, 8, 400, 400, new Rectangle2D(0, 0, 5120, 4096));
        }

        public static List<Weather> GetWeatherList(Map facet)
        {
            if (facet == null)
                return null;

            List<Weather> list = null;
            m_WeatherByFacet.TryGetValue(facet, out list);

            if (list == null)
                m_WeatherByFacet[facet] = list = new List<Weather>();

            return list;
        }

        public static void AddDynamicWeather(int temperature, int chanceOfPercipitation, int chanceOfExtremeTemperature, int moveSpeed, int width, int height, Rectangle2D bounds)
        {
            for (int i = 0; i < m_Facets.Length; ++i)
            {
                Rectangle2D area = new Rectangle2D();
                bool isValid = false;

                for (int j = 0; j < 10; ++j)
                {
                    area = new Rectangle2D(bounds.X + Utility.Random(bounds.Width - width), bounds.Y + Utility.Random(bounds.Height - height), width, height);

                    if (!CheckWeatherConflict(m_Facets[i], null, area))
                        isValid = true;

                    if (isValid)
                        break;
                }

                if (!isValid)
                    continue;

                _ = new Weather(m_Facets[i], new Rectangle2D[] { area }, temperature, chanceOfPercipitation, chanceOfExtremeTemperature, TimeSpan.FromSeconds(30.0))
                {
                    m_Bounds = bounds,
                    m_MoveSpeed = moveSpeed
                };
            }
        }

        public static void AddWeather(int temperature, int chanceOfPercipitation, int chanceOfExtremeTemperature, params Rectangle2D[] area)
        {
            for (int i = 0; i < m_Facets.Length; ++i)
                new Weather(m_Facets[i], area, temperature, chanceOfPercipitation, chanceOfExtremeTemperature, TimeSpan.FromSeconds(30.0));
        }

        public static bool CheckWeatherConflict(Map facet, Weather exclude, Rectangle2D area)
        {
            List<Weather> list = GetWeatherList(facet);

            if (list == null)
                return false;

            for (int i = 0; i < list.Count; ++i)
            {
                Weather w = list[i];

                if (w != exclude && w.IntersectsWith(area))
                    return true;
            }

            return false;
        }

        public static bool CheckIntersection(Rectangle2D r1, Rectangle2D r2)
        {
            if (r1.X >= (r2.X + r2.Width))
                return false;

            if (r2.X >= (r1.X + r1.Width))
                return false;

            if (r1.Y >= (r2.Y + r2.Height))
                return false;

            if (r2.Y >= (r1.Y + r1.Height))
                return false;

            return true;
        }

        public static bool CheckContains(Rectangle2D big, Rectangle2D small)
        {
            if (small.X < big.X)
                return false;

            if (small.Y < big.Y)
                return false;

            if ((small.X + small.Width) > (big.X + big.Width))
                return false;

            if ((small.Y + small.Height) > (big.Y + big.Height))
                return false;

            return true;
        }

        public virtual bool IntersectsWith(Rectangle2D area)
        {
            for (int i = 0; i < m_Area.Length; ++i)
            {
                if (CheckIntersection(area, m_Area[i]))
                    return true;
            }

            return false;
        }

        public virtual void Reposition()
        {
            if (m_Area.Length == 0)
                return;

            int width = m_Area[0].Width;
            int height = m_Area[0].Height;

            Rectangle2D area = new Rectangle2D();
            bool isValid = false;

            for (int j = 0; j < 10; ++j)
            {
                area = new Rectangle2D(m_Bounds.X + Utility.Random(m_Bounds.Width - width), m_Bounds.Y + Utility.Random(m_Bounds.Height - height), width, height);

                if (!CheckWeatherConflict(m_Facet, this, area))
                    isValid = true;

                if (isValid)
                    break;
            }

            if (!isValid)
                return;

            m_Area[0] = area;
        }

        public virtual void RecalculateMovementAngle()
        {
            double angle = Utility.RandomDouble() * Math.PI * 2.0;

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            m_MoveAngleX = (int)(100 * cos);
            m_MoveAngleY = (int)(100 * sin);
        }

        public virtual void MoveForward()
        {
            if (m_Area.Length == 0)
                return;

            for (int i = 0; i < 5; ++i) // try 5 times to find a valid spot
            {
                int xOffset = (m_MoveSpeed * m_MoveAngleX) / 100;
                int yOffset = (m_MoveSpeed * m_MoveAngleY) / 100;

                Rectangle2D oldArea = m_Area[0];
                Rectangle2D newArea = new Rectangle2D(oldArea.X + xOffset, oldArea.Y + yOffset, oldArea.Width, oldArea.Height);

                if (!CheckWeatherConflict(m_Facet, this, newArea) && CheckContains(m_Bounds, newArea))
                {
                    m_Area[0] = newArea;
                    break;
                }

                RecalculateMovementAngle();
            }
        }

        public virtual void OnTick()
        {
            if (m_Stage == 0)
            {
                m_Active = (m_ChanceOfPercipitation > Utility.Random(100));
                m_ExtremeTemperature = (m_ChanceOfExtremeTemperature > Utility.Random(100));

                if (m_MoveSpeed > 0)
                {
                    Reposition();
                    RecalculateMovementAngle();
                }
            }

            if (m_Active)
            {
                if (m_Stage > 0 && m_MoveSpeed > 0)
                    MoveForward();

                int type, density, temperature;

                temperature = m_Temperature;

                if (m_ExtremeTemperature)
                    temperature *= -1;

                if (m_Stage < 15)
                {
                    density = m_Stage * 5;
                }
                else
                {
                    density = 150 - (m_Stage * 5);

                    if (density < 10)
                        density = 10;
                    else if (density > 70)
                        density = 70;
                }

                if (density == 0)
                    type = 0xFE;
                else if (temperature > 0)
                    type = 0;
                else
                    type = 2;

                List<NetState> states = NetState.Instances;

                Packet weatherPacket = null;

                for (int i = 0; i < states.Count; ++i)
                {
                    NetState ns = states[i];
                    Mobile mob = ns.Mobile;

                    if (mob == null || mob.Map != m_Facet)
                        continue;

                    bool contains = (m_Area.Length == 0);

                    for (int j = 0; !contains && j < m_Area.Length; ++j)
                        contains = m_Area[j].Contains(mob.Location);

                    if (!contains)
                        continue;

                    if (weatherPacket == null)
                        weatherPacket = Packet.Acquire(new Network.Weather(type, density, temperature));

                    ns.Send(weatherPacket);
                }

                Packet.Release(weatherPacket);
            }

            m_Stage++;
            m_Stage %= 30;
        }
    }

    public class WeatherMap : MapItem
    {
        [Constructable]
        public WeatherMap()
        {
            SetDisplay(0, 0, 5119, 4095, 400, 400);
        }

        public WeatherMap(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName => "weather map";

        public override void OnDoubleClick(Mobile from)
        {
            Map facet = from.Map;

            if (facet == null)
                return;

            List<Weather> list = Weather.GetWeatherList(facet);

            ClearPins();

            for (int i = 0; i < list.Count; ++i)
            {
                Weather w = list[i];

                for (int j = 0; j < w.Area.Length; ++j)
                    AddWorldPin(w.Area[j].X + (w.Area[j].Width / 2), w.Area[j].Y + (w.Area[j].Height / 2));
            }

            base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
