using System;
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
            this._label = label;
            this._description = desc;
            this._value = data;
            this._startPos = start;
            this._sweepSize = sweep;
            this._color = clr;
        }

        private DataItem()
        {
        }

        public string Label 
        {
            get
            {
                return this._label;
            }
            set
            {
                this._label = value;
            }
        }
        public string Description 
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }
        public float Value 
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
        public Color ItemColor 
        {
            get
            {
                return this._color;
            }
            set
            {
                this._color = value;
            }
        }
        public float StartPos
        {
            get
            {
                return this._startPos;
            }
            set
            {
                this._startPos = value;
            }
        }
        public float SweepSize
        {
            get
            {
                return this._sweepSize;
            }
            set
            {
                this._sweepSize = value;
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
                return (DataItem)(this.List[index]);
            }
            set
            {
                this.List[index] = value;
            }
        }
        public int Add(DataItem value) 
        {
            return this.List.Add(value);
        }

        public int IndexOf(DataItem value) 
        {
            return this.List.IndexOf(value);
        }

        public bool Contains(DataItem value) 
        {
            return this.List.Contains(value);
        }

        public void Remove(DataItem value) 
        {
            this.List.Remove(value);
        }
    }
}