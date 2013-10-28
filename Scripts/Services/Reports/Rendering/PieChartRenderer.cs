using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Server.Engines.Reports
{
    // Modified from MS sample
    //*********************************************************************
    //
    // PieChart Class
    //
    // This class uses GDI+ to render Pie Chart.
    //
    //*********************************************************************
    public class PieChartRenderer : ChartRenderer
    {
        private const int	_bufferSpace = 125;
        private readonly ArrayList _chartItems;
        private readonly Color _backgroundColor;
        private readonly Color _borderColor;
        private readonly string _legendFontStyle;
        private readonly float _legendFontSize;
        private int _perimeter;
        private float _total;
        private int _legendWidth;
        private int _legendHeight;
        private int _legendFontHeight;
        private bool _showPercents;
        public PieChartRenderer()
        {
            this._chartItems = new ArrayList();
            this._perimeter = 250;
            this._backgroundColor = Color.White;
            this._borderColor = Color.FromArgb(63, 63, 63);
            this._legendFontSize = 8;
            this._legendFontStyle = "Verdana";
        }

        public PieChartRenderer(Color bgColor)
        {
            this._chartItems = new ArrayList();
            this._perimeter = 250;
            this._backgroundColor = bgColor;
            this._borderColor = Color.FromArgb(63, 63, 63);
            this._legendFontSize = 8;
            this._legendFontStyle = "Verdana";
        }

        public bool ShowPercents
        {
            get
            {
                return this._showPercents;
            }
            set
            {
                this._showPercents = value;
            }
        }
        //*********************************************************************
        //
        // This method collects all data points and calculate all the necessary dimensions 
        // to draw the chart.  It is the first method called before invoking the Draw() method.
        //
        //*********************************************************************
        public void CollectDataPoints(string[] xValues, string[] yValues)
        {
            this._total = 0.0f;
			
            for (int i = 0; i < xValues.Length; i++)
            {
                float ftemp = Convert.ToSingle(yValues[i]);
                this._chartItems.Add(new DataItem(xValues[i], xValues.ToString(), ftemp, 0, 0, Color.AliceBlue));
                this._total += ftemp;
            }
			
            float nextStartPos = 0.0f;
            int counter = 0;
            foreach (DataItem item in this._chartItems)
            {
                item.StartPos = nextStartPos;
                item.SweepSize = item.Value / this._total * 360;
                nextStartPos = item.StartPos + item.SweepSize;
                item.ItemColor = this.GetColor(counter++);
            }

            this.CalculateLegendWidthHeight();
        }

        //*********************************************************************
        //
        // This method returns a bitmap to the calling function.  This is the method
        // that actually draws the pie chart and the legend with it.
        //
        //*********************************************************************
        public override Bitmap Draw()
        {
            int perimeter = this._perimeter;
            Rectangle pieRect = new Rectangle(0, 0, perimeter, perimeter - 1);
            Bitmap bmp = new Bitmap(perimeter + this._legendWidth, perimeter);
            Font fnt = null;
            Pen pen = null;
            Graphics grp = null;
            StringFormat sf = null, sfp = null;
			
            try
            {
                grp = Graphics.FromImage(bmp);
                grp.CompositingQuality = CompositingQuality.HighQuality;
                grp.SmoothingMode = SmoothingMode.AntiAlias;
                sf = new StringFormat();

                //Paint Back ground
                using (SolidBrush brsh = new SolidBrush(this._backgroundColor))
                    grp.FillRectangle(brsh, -1, -1, perimeter + this._legendWidth + 1, perimeter + 1);

                //Align text to the right
                sf.Alignment = StringAlignment.Far; 
			
                //Draw all wedges and legends
                for (int i = 0; i < this._chartItems.Count; i++)
                {
                    DataItem item = (DataItem)this._chartItems[i];
                    SolidBrush brs = null;
                    try
                    {
                        brs = new SolidBrush(item.ItemColor);
                        grp.FillPie(brs, pieRect, item.StartPos, item.SweepSize);

                        //grp.DrawPie(new Pen(_borderColor,1.2f),pieRect,item.StartPos,item.SweepSize);

                        if (fnt == null)
                            fnt = new Font(this._legendFontStyle, this._legendFontSize);

                        if (this._showPercents && item.SweepSize > 10)
                        {
                            if (sfp == null)
                            {
                                sfp = new StringFormat();
                                sfp.Alignment = StringAlignment.Center;
                                sfp.LineAlignment = StringAlignment.Center;
                            }

                            float perc = (item.SweepSize * 100.0f) / 360.0f;
                            string percString = String.Format("{0:F0}%", perc);

                            float px = pieRect.X + (pieRect.Width / 2);
                            float py = pieRect.Y + (pieRect.Height / 2);

                            double angle = item.StartPos + (item.SweepSize / 2);
                            double rads = (angle / 180.0) * Math.PI;

                            px += (float)(Math.Cos(rads) * perimeter / 3);
                            py += (float)(Math.Sin(rads) * perimeter / 3);

                            grp.DrawString(percString, fnt, Brushes.Gray,
                                new RectangleF(px - 30 - 1, py - 20, 60, 40), sfp);

                            grp.DrawString(percString, fnt, Brushes.Gray,
                                new RectangleF(px - 30 + 1, py - 20, 60, 40), sfp);

                            grp.DrawString(percString, fnt, Brushes.Gray,
                                new RectangleF(px - 30, py - 20 - 1, 60, 40), sfp);

                            grp.DrawString(percString, fnt, Brushes.Gray,
                                new RectangleF(px - 30, py - 20 + 1, 60, 40), sfp);

                            grp.DrawString(percString, fnt, Brushes.White,
                                new RectangleF(px - 30, py - 20, 60, 40), sfp);
                        }

                        if (pen == null)
                            pen = new Pen(this._borderColor, 0.5f);

                        grp.FillRectangle(brs, perimeter + _bufferSpace, i * this._legendFontHeight + 15, 10, 10);
                        grp.DrawRectangle(pen, perimeter + _bufferSpace, i * this._legendFontHeight + 15, 10, 10);
					
                        grp.DrawString(item.Label, fnt,
                            Brushes.Black, perimeter + _bufferSpace + 20, i * this._legendFontHeight + 13);

                        grp.DrawString(item.Value.ToString("#,###.##"), fnt,
                            Brushes.Black, perimeter + _bufferSpace + 200, i * this._legendFontHeight + 13, sf);
                    }
                    finally
                    {
                        if (brs != null)
                            brs.Dispose();
                    }
                }

                for (int i = 0; i < this._chartItems.Count; i++)
                {
                    DataItem item = (DataItem)this._chartItems[i];
                    SolidBrush brs = null;
                    try
                    {
                        grp.DrawPie(new Pen(this._borderColor,0.5f), pieRect, item.StartPos, item.SweepSize);
                    }
                    finally
                    {
                        if (brs != null)
                            brs.Dispose();
                    }
                }
			
                //draws the border around Pie
                using (Pen pen2 = new Pen(this._borderColor, 2))
                    grp.DrawEllipse(pen2, pieRect);  

                //draw border around legend
                using (Pen pen1 = new Pen(this._borderColor, 1))
                    grp.DrawRectangle(pen1, perimeter + _bufferSpace - 10, 10, 220, this._chartItems.Count * this._legendFontHeight + 25);

                //Draw Total under legend
                using (Font fntb = new Font(this._legendFontStyle, this._legendFontSize, FontStyle.Bold))
                {
                    grp.DrawString("Total", fntb,
                        Brushes.Black, perimeter + _bufferSpace + 30, (this._chartItems.Count + 1) * this._legendFontHeight, sf);
                    grp.DrawString(this._total.ToString("#,###.##"), fntb,
                        Brushes.Black, perimeter + _bufferSpace + 200, (this._chartItems.Count + 1) * this._legendFontHeight, sf);
                }
			
                grp.SmoothingMode = SmoothingMode.AntiAlias;
            }
            finally
            {
                if (sf != null)
                    sf.Dispose();
                if (grp != null)
                    grp.Dispose();
                if (sfp != null)
                    sfp.Dispose();
                if (fnt != null)
                    fnt.Dispose();
                if (pen != null)
                    pen.Dispose();
            }
            return bmp;
        }

        //*********************************************************************
        //
        //	This method calculates the space required to draw the chart legend.
        //
        //*********************************************************************
        private void CalculateLegendWidthHeight()
        {
            Font fontLegend = new Font(this._legendFontStyle, this._legendFontSize);
            this._legendFontHeight = fontLegend.Height + 3;
            this._legendHeight = fontLegend.Height * (this._chartItems.Count + 1);
            if (this._legendHeight > this._perimeter)
                this._perimeter = this._legendHeight;

            this._legendWidth = this._perimeter + _bufferSpace;
            fontLegend.Dispose();
        }
    }
}