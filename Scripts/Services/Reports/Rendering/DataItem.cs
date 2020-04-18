using System.Collections;
using System.Drawing;

namespace Server.Engines.Reports
{
    // Modified from MS sample
    //*********************************************************************
    //
    // ChartItem Class
    //
    // This class represents a data point in a chart
    //
    //*********************************************************************
    public class DataItem
    {
        private string _label;
        private string _description;
        private float _value;
        private Color _color;
        private float _startPos;
        private float _sweepSize;
        public DataItem(string label, string desc, float data, float start, float sweep, Color clr)
        {
            _label = label;
            _description = desc;
            _value = data;
            _startPos = start;
            _sweepSize = sweep;
            _color = clr;
        }

        private DataItem()
        {
        }

        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public Color ItemColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        public float StartPos
        {
            get
            {
                return _startPos;
            }
            set
            {
                _startPos = value;
            }
        }
        public float SweepSize
        {
            get
            {
                return _sweepSize;
            }
            set
            {
                _sweepSize = value;
            }
        }
    }

    //*********************************************************************
    //
    // Custom Collection for ChartItems
    //
    //*********************************************************************
    public class ChartItemsCollection : CollectionBase
    {
        public DataItem this[int index]
        {
            get
            {
                return (DataItem)(List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        public int Add(DataItem value)
        {
            return List.Add(value);
        }

        public int IndexOf(DataItem value)
        {
            return List.IndexOf(value);
        }

        public bool Contains(DataItem value)
        {
            return List.Contains(value);
        }

        public void Remove(DataItem value)
        {
            List.Remove(value);
        }
    }
}