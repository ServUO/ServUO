using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Server.Engines.Reports
{
    // Modified from MS sample
    //*********************************************************************
    //
    // BarGraph Class
    //
    // This class uses GDI+ to render Bar Chart.
    //
    //*********************************************************************
    public class BarRegion
    {
        public int m_RangeFrom, m_RangeTo;
        public string m_Name;
        public BarRegion(int rangeFrom, int rangeTo, string name)
        {
            this.m_RangeFrom = rangeFrom;
            this.m_RangeTo = rangeTo;
            this.m_Name = name;
        }
    }

    public class BarGraphRenderer : ChartRenderer
    {
        public BarRegion[] _regions;
        //*********************************************************************
        //
        // This method draws all the bars for the graph.
        //
        //*********************************************************************
        public int _interval;
        private const float _graphLegendSpacer = 15F;
        private const float	_labelFontSize = 7f;
        private const int	_legendFontSize = 9;
        private const float _legendRectangleSize = 10F;
        private const float _spacer = 5F;
        private BarGraphRenderMode _renderMode;
        // Overall related members
        private Color	_backColor;
        private string	_fontFamily;
        private string	_longestTickValue = string.Empty;// Used to calculate max value width
        private float	_maxTickValueWidth;// Used to calculate left offset of bar graph
        private float	_totalHeight;
        private float	_totalWidth;
        // Graph related members
        private float	_barWidth;
        private float	_bottomBuffer;// Space from bottom to x axis
        private bool	_displayBarData;
        private Color	_fontColor;
        private float	_graphHeight;
        private float	_graphWidth;
        private float	_maxValue = 0.0f;// = final tick value * tick count
        private float	_scaleFactor;// = _maxValue / _graphHeight
        private float	_spaceBtwBars;// For now same as _barWidth
        private float	_topBuffer;// Space from top to the top of y axis
        private float	_xOrigin;// x position where graph starts drawing
        private float	_yOrigin;// y position where graph starts drawing
        private string	_yLabel;
        private int _yTickCount;
        private float	_yTickValue;// Value for each tick = _maxValue/_yTickCount

        // Legend related members
        private bool	_displayLegend;
        private float	_legendWidth;
        private string	_longestLabel = string.Empty;// Used to calculate legend width
        private float	_maxLabelWidth = 0.0f;
        private string _xTitle, _yTitle;
        public BarGraphRenderer()
        {
            this.AssignDefaultSettings();
        }

        public BarGraphRenderer(Color bgColor)
        {
            this.AssignDefaultSettings();
            this.BackgroundColor = bgColor;
        }

        public string FontFamily 
        {
            get
            {
                return this._fontFamily;
            }
            set
            {
                this._fontFamily = value;
            }
        }
        public BarGraphRenderMode RenderMode
        {
            get
            {
                return this._renderMode;
            }
            set
            {
                this._renderMode = value;
            }
        }
        public Color BackgroundColor 
        {
            set
            {
                this._backColor = value;
            }
        }
        public int BottomBuffer 
        {
            set
            {
                this._bottomBuffer = Convert.ToSingle(value);
            }
        }
        public Color FontColor 
        {
            set
            {
                this._fontColor = value;
            }
        }
        public int Height 
        {
            get
            {
                return Convert.ToInt32(this._totalHeight);
            }
            set
            {
                this._totalHeight = Convert.ToSingle(value);
            }
        }
        public int Width 
        {
            get
            {
                return Convert.ToInt32(this._totalWidth);
            }
            set
            {
                this._totalWidth = Convert.ToSingle(value);
            }
        }
        public bool ShowLegend 
        {
            get
            {
                return this._displayLegend;
            }
            set
            {
                this._displayLegend = value;
            }
        }
        public bool ShowData 
        {
            get
            {
                return this._displayBarData;
            }
            set
            {
                this._displayBarData = value;
            }
        }
        public int TopBuffer 
        {
            set
            {
                this._topBuffer = Convert.ToSingle(value);
            }
        }
        public string VerticalLabel 
        {
            get
            {
                return this._yLabel;
            }
            set
            {
                this._yLabel = value;
            }
        }
        public int VerticalTickCount 
        {
            get
            {
                return this._yTickCount;
            }
            set
            {
                this._yTickCount = value;
            }
        }
        public void SetTitles(string xTitle, string yTitle)
        {
            this._xTitle = xTitle;
            this._yTitle = yTitle;
        }

        //*********************************************************************
        //
        // This method collects all data points and calculate all the necessary dimensions 
        // to draw the bar graph.  It is the method called before invoking the Draw() method.
        // labels is the x values.
        // values is the y values.
        //
        //*********************************************************************
        public void CollectDataPoints(string[] labels, string[] values)
        {
            if (labels.Length == values.Length) 
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    float temp = Convert.ToSingle(values[i]);
                    string shortLbl = this.MakeShortLabel(labels[i]);

                    // For now put 0.0 for start position and sweep size
                    this.DataPoints.Add(new DataItem(shortLbl, labels[i], temp, 0.0f, 0.0f, this.GetColor(i)));
				
                    // Find max value from data; this is only temporary _maxValue
                    if (this._maxValue < temp)
                        this._maxValue = temp;

                    // Find the longest description
                    if (this._displayLegend) 
                    {
                        string currentLbl = labels[i] + " (" + shortLbl + ")";
                        float currentWidth = this.CalculateImgFontWidth(currentLbl, _legendFontSize, this.FontFamily);
                        if (this._maxLabelWidth < currentWidth)
                        {
                            this._longestLabel = currentLbl;
                            this._maxLabelWidth = currentWidth;
                        }
                    }
                }

                this.CalculateTickAndMax();
                this.CalculateGraphDimension();
                this.CalculateBarWidth(this.DataPoints.Count, this._graphWidth);
                this.CalculateSweepValues();
            }
            else
                throw new Exception("X data count is different from Y data count");
        }

        //*********************************************************************
        //
        // Same as above; called when user doesn't care about the x values
        //
        //*********************************************************************
        public void CollectDataPoints(string[] values)
        {
            string[] labels = values;
            this.CollectDataPoints(labels, values);
        }

        public void DrawRegions(Graphics gfx)
        {
            if (this._regions == null)
                return;

            using (StringFormat textFormat = new StringFormat())
            {
                textFormat.Alignment = StringAlignment.Center;
                textFormat.LineAlignment = StringAlignment.Center;

                using (Font font = new Font(this._fontFamily, _labelFontSize))
                {
                    using (Brush textBrush = new SolidBrush(this._fontColor))
                    {
                        using (Pen solidPen = new Pen(this._fontColor))
                        {
                            using (Pen lightPen = new Pen(Color.FromArgb(128, this._fontColor)))
                            {
                                float labelWidth = this._barWidth + this._spaceBtwBars;

                                for (int i = 0; i < this._regions.Length; ++i)
                                {
                                    BarRegion reg = this._regions[i];

                                    RectangleF rc = new RectangleF(this._xOrigin + (reg.m_RangeFrom * labelWidth), this._yOrigin, (reg.m_RangeTo - reg.m_RangeFrom + 1) * labelWidth, this._graphHeight);

                                    if (rc.X + rc.Width > this._xOrigin + this._graphWidth)
                                        rc.Width = this._xOrigin + this._graphWidth - rc.X;

                                    using (SolidBrush brsh = new SolidBrush(Color.FromArgb(48, this.GetColor(i))))
                                        gfx.FillRectangle(brsh, rc);

                                    rc.Offset((rc.Width - 200.0f) * 0.5f, -16.0f);
                                    rc.Width = 200.0f;
                                    rc.Height = 20.0f;

                                    gfx.DrawString(reg.m_Name, font, textBrush, rc, textFormat);
                                }
                            }
                        }
                    }
                }
            }
        }

        //*********************************************************************
        //
        // This method returns a bar graph bitmap to the calling function.  It is called after 
        // all dimensions and data points are calculated.
        //
        //*********************************************************************
        public override Bitmap Draw()
        {
            int height = Convert.ToInt32(this._totalHeight);
            int width = Convert.ToInt32(this._totalWidth);

            Bitmap bmp = new Bitmap(width, height);
			
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush brsh = new SolidBrush(this._backColor))
                    graph.FillRectangle(brsh, -1, -1, bmp.Width + 1, bmp.Height + 1);

                this.DrawRegions(graph);
                this.DrawVerticalLabelArea(graph);
                this.DrawXLabelBack(graph);
                this.DrawBars(graph);
                this.DrawXLabelArea(graph);

                if (this._displayLegend)
                    this.DrawLegend(graph);
            }

            return bmp;
        }

        private void DrawBars(Graphics graph)
        {
            SolidBrush brsFont = null;
            Font valFont = null;
            StringFormat sfFormat = null;

            try 
            {
                brsFont = new SolidBrush(this._fontColor);
                valFont = new Font(this._fontFamily, _labelFontSize);
                sfFormat = new StringFormat();
                sfFormat.Alignment = StringAlignment.Center;
                int i = 0;

                PointF[] linePoints = null;

                if (this._renderMode == BarGraphRenderMode.Lines)
                    linePoints = new PointF[this.DataPoints.Count];

                int pointIndex = 0;

                // Draw bars and the value above each bar
                using (Pen pen = new Pen(this._fontColor,0.15f))
                {
                    using (SolidBrush whiteBrsh = new SolidBrush(Color.FromArgb(128, Color.White)))
                    {
                        foreach (DataItem item in this.DataPoints)
                        {
                            using (SolidBrush barBrush = new SolidBrush(item.ItemColor))
                            {
                                float itemY = this._yOrigin + this._graphHeight - item.SweepSize;

                                if (this._renderMode == BarGraphRenderMode.Lines)
                                {
                                    linePoints[pointIndex++] = new PointF(this._xOrigin + item.StartPos + (this._barWidth / 2), itemY);
                                }
                                else if (this._renderMode == BarGraphRenderMode.Bars)
                                {
                                    float ox = this._xOrigin + item.StartPos;
                                    float oy = itemY;
                                    float ow = this._barWidth;
                                    float oh = item.SweepSize;
                                    float of = 9.5f;

                                    PointF[] pts = new PointF[]
                                    {
                                        new PointF(ox, oy),
                                        new PointF(ox + ow, oy),
                                        new PointF(ox + of, oy + of),
                                        new PointF(ox + of + ow, oy + of),
                                        new PointF(ox, oy + oh),
                                        new PointF(ox + of, oy + of + oh),
                                        new PointF(ox + of + ow, oy + of + oh)
                                    };

                                    graph.FillPolygon(barBrush, new PointF[] { pts[2], pts[3], pts[6], pts[5] });

                                    using (SolidBrush ltBrsh = new SolidBrush(System.Windows.Forms.ControlPaint.Light(item.ItemColor, 0.1f)))
                                        graph.FillPolygon(ltBrsh, new PointF[] { pts[0], pts[2], pts[5], pts[4] });

                                    using (SolidBrush drkBrush = new SolidBrush(System.Windows.Forms.ControlPaint.Dark(item.ItemColor, 0.05f)))
                                        graph.FillPolygon(drkBrush, new PointF[] { pts[0], pts[1], pts[3], pts[2] });

                                    graph.DrawLine(pen, pts[0], pts[1]);
                                    graph.DrawLine(pen, pts[0], pts[2]);
                                    graph.DrawLine(pen, pts[1], pts[3]);
                                    graph.DrawLine(pen, pts[2], pts[3]);
                                    graph.DrawLine(pen, pts[2], pts[5]);
                                    graph.DrawLine(pen, pts[0], pts[4]);
                                    graph.DrawLine(pen, pts[4], pts[5]);
                                    graph.DrawLine(pen, pts[5], pts[6]);
                                    graph.DrawLine(pen, pts[3], pts[6]);

                                    // Draw data value
                                    if (this._displayBarData && (i % this._interval) == 0)
                                    {
                                        float sectionWidth = (this._barWidth + this._spaceBtwBars);
                                        float startX = this._xOrigin + (i * sectionWidth) + (sectionWidth / 2);  // This draws the value on center of the bar
                                        float startY = itemY - 2f - valFont.Height;					  // Positioned on top of each bar by 2 pixels
                                        RectangleF recVal = new RectangleF(startX - ((sectionWidth * this._interval) / 2), startY, sectionWidth * this._interval, valFont.Height);
                                        SizeF sz = graph.MeasureString(item.Value.ToString("#,###.##"), valFont, recVal.Size, sfFormat);
                                        //using ( SolidBrush brsh = new SolidBrush( Color.FromArgb( 180, 255, 255, 255 ) ) )
                                        //	graph.FillRectangle( brsh, new RectangleF(recVal.X+((recVal.Width-sz.Width)/2),recVal.Y+((recVal.Height-sz.Height)/2),sz.Width+4,sz.Height) );

                                        //graph.DrawString(item.Value.ToString("#,###.##"), valFont, brsFont, recVal, sfFormat);

                                        for (int box = -1; box <= 1; ++box)
                                        {
                                            for (int boy = -1; boy <= 1; ++boy)
                                            {
                                                if (box == 0 && boy == 0)
                                                    continue;

                                                RectangleF rco = new RectangleF(recVal.X + box, recVal.Y + boy, recVal.Width, recVal.Height);
                                                graph.DrawString(item.Value.ToString("#,###.##"), valFont, whiteBrsh, rco, sfFormat);
                                            }
                                        }

                                        graph.DrawString(item.Value.ToString("#,###.##"), valFont, brsFont, recVal, sfFormat);	
                                    }
                                }

                                i++;
                            }
                        }

                        if (this._renderMode == BarGraphRenderMode.Lines)
                        {
                            if (linePoints.Length >= 2)
                            {
                                using (Pen linePen = new Pen(Color.FromArgb(220, Color.Red), 2.5f))
                                    graph.DrawCurve(linePen, linePoints, 0.5f);
                            }

                            using (Pen linePen = new Pen(Color.FromArgb(40, this._fontColor), 0.8f))
                            {
                                for (int j = 0; j < linePoints.Length; ++j)
                                {
                                    graph.DrawLine(linePen, linePoints[j], new PointF(linePoints[j].X, this._yOrigin + this._graphHeight));

                                    DataItem item = this.DataPoints[j];
                                    float itemY = this._yOrigin + this._graphHeight - item.SweepSize;

                                    // Draw data value
                                    if (this._displayBarData && (j % this._interval) == 0)
                                    {
                                        graph.FillEllipse(brsFont, new RectangleF(linePoints[j].X - 2.0f, linePoints[j].Y - 2.0f, 4.0f, 4.0f));

                                        float sectionWidth = (this._barWidth + this._spaceBtwBars);
                                        float startX = this._xOrigin + (j * sectionWidth) + (sectionWidth / 2);  // This draws the value on center of the bar
                                        float startY = itemY - 2f - valFont.Height;					  // Positioned on top of each bar by 2 pixels
                                        RectangleF recVal = new RectangleF(startX - ((sectionWidth * this._interval) / 2), startY, sectionWidth * this._interval, valFont.Height);
                                        SizeF sz = graph.MeasureString(item.Value.ToString("#,###.##"), valFont, recVal.Size, sfFormat);
                                        //using ( SolidBrush brsh = new SolidBrush( Color.FromArgb( 48, 255, 255, 255 ) ) )
                                        //	graph.FillRectangle( brsh, new RectangleF(recVal.X+((recVal.Width-sz.Width)/2),recVal.Y+((recVal.Height-sz.Height)/2),sz.Width+4,sz.Height) );

                                        for (int box = -1; box <= 1; ++box)
                                        {
                                            for (int boy = -1; boy <= 1; ++boy)
                                            {
                                                if (box == 0 && boy == 0)
                                                    continue;

                                                RectangleF rco = new RectangleF(recVal.X + box, recVal.Y + boy, recVal.Width, recVal.Height);
                                                graph.DrawString(item.Value.ToString("#,###.##"), valFont, whiteBrsh, rco, sfFormat);
                                            }
                                        }

                                        graph.DrawString(item.Value.ToString("#,###.##"), valFont, brsFont, recVal, sfFormat);	
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally 
            {
                if (brsFont != null)
                    brsFont.Dispose();
                if (valFont != null)
                    valFont.Dispose();
                if (sfFormat != null)
                    sfFormat.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method draws the y label, tick marks, tick values, and the y axis.
        //
        //*********************************************************************
        private void DrawVerticalLabelArea(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;
            StringFormat sfVLabel = null;

            float fo = (this._yTitle == null ? 0.0f : 20.0f);
			
            try
            {
                brs = new SolidBrush(this._fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(this._fontColor);

                if (this._yTitle != null)
                {
                    sfVLabel = new StringFormat();
                    sfVLabel.Alignment = StringAlignment.Center;
                    sfVLabel.LineAlignment = StringAlignment.Center;
                    sfVLabel.FormatFlags = StringFormatFlags.DirectionVertical;

                    lblFont = new Font(this._fontFamily, _labelFontSize + 4.0f);
                    graph.DrawString(this._yTitle, lblFont, brs, new RectangleF(0.0f, this._yOrigin, 20.0f, this._graphHeight), sfVLabel);
                    lblFont.Dispose();
                }

                sfVLabel = new StringFormat();
                lblFormat.Alignment = StringAlignment.Far;
                lblFormat.FormatFlags |= StringFormatFlags.NoClip;

                // Draw vertical label at the top of y-axis and place it in the middle top of y-axis
                lblFont = new Font(this._fontFamily, _labelFontSize + 2.0f,FontStyle.Bold);
                RectangleF recVLabel = new RectangleF(0, this._yOrigin - 2 * _spacer - lblFont.Height, this._xOrigin * 2, lblFont.Height);
                sfVLabel.Alignment = StringAlignment.Center;
                sfVLabel.FormatFlags |= StringFormatFlags.NoClip;
                //graph.DrawRectangle(Pens.Black,Rectangle.Truncate(recVLabel));
                graph.DrawString(this._yLabel, lblFont, brs, recVLabel, sfVLabel);
                lblFont.Dispose();

                lblFont = new Font(this._fontFamily, _labelFontSize);
                // Draw all tick values and tick marks
                using (Pen smallPen = new Pen(Color.FromArgb(96, this._fontColor),0.8f))
                {
                    for (int i = 0; i < this._yTickCount; i++)
                    {
                        float currentY = this._topBuffer + (i * this._yTickValue / this._scaleFactor);	// Position for tick mark
                        float labelY = currentY - lblFont.Height / 2;						// Place label in the middle of tick
                        RectangleF lblRec = new RectangleF(_spacer + fo - 6, labelY, this._maxTickValueWidth, lblFont.Height);
				
                        float currentTick = this._maxValue - i * this._yTickValue;					// Calculate tick value from top to bottom
                        graph.DrawString(currentTick.ToString("#,###.##"), lblFont, brs, lblRec, lblFormat);	// Draw tick value  
                        graph.DrawLine(pen, this._xOrigin, currentY, this._xOrigin - 4.0f, currentY);						// Draw tick mark

                        graph.DrawLine(smallPen, this._xOrigin, currentY, this._xOrigin + this._graphWidth, currentY);
                    }
                }

                // Draw y axis
                graph.DrawLine(pen, this._xOrigin, this._yOrigin, this._xOrigin, this._yOrigin + this._graphHeight);
            }
            finally
            {
                if (lblFont != null)
                    lblFont.Dispose();
                if (brs != null)
                    brs.Dispose();
                if (lblFormat != null)
                    lblFormat.Dispose();
                if (pen != null)
                    pen.Dispose();
                if (sfVLabel != null)
                    sfVLabel.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method draws x axis and all x labels
        //
        //*********************************************************************
        private void DrawXLabelBack(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;

            try
            {
                lblFont = new Font(this._fontFamily, _labelFontSize);
                brs = new SolidBrush(this._fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(this._fontColor);

                lblFormat.Alignment = StringAlignment.Center;

                // Draw x axis
                graph.DrawLine(pen, this._xOrigin, this._yOrigin + this._graphHeight, this._xOrigin + this._graphWidth, this._yOrigin + this._graphHeight);
            }
            finally
            {
                if (lblFont != null)
                    lblFont.Dispose();
                if (brs != null)
                    brs.Dispose();
                if (lblFormat != null)
                    lblFormat.Dispose();
                if (pen != null)
                    pen.Dispose();
            }
        }

        private void DrawXLabelArea(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;

            try
            {
                brs = new SolidBrush(this._fontColor);
                pen = new Pen(this._fontColor);

                if (this._xTitle != null)
                {
                    lblFormat = new StringFormat();
                    lblFormat.Alignment = StringAlignment.Center;
                    lblFormat.LineAlignment = StringAlignment.Center;
                    //					sfVLabel.FormatFlags=StringFormatFlags.DirectionVertical;

                    lblFont = new Font(this._fontFamily, _labelFontSize + 2.0f, FontStyle.Bold);
                    graph.DrawString(this._xTitle, lblFont, brs, new RectangleF(this._xOrigin, this._yOrigin + this._graphHeight + 14.0f + (this._renderMode == BarGraphRenderMode.Bars ? 10.0f : 0.0f) + ((this.DataPoints.Count / this._interval) > 24 ? 16.0f : 0.0f), this._graphWidth, 20.0f), lblFormat);
                }

                lblFont = new Font(this._fontFamily, _labelFontSize);
                lblFormat = new StringFormat();
                lblFormat.Alignment = StringAlignment.Center;
                lblFormat.FormatFlags |= StringFormatFlags.NoClip;
                lblFormat.Trimming = StringTrimming.None;
                //lblFormat.FormatFlags |= StringFormatFlags.NoWrap;

                float of = 0.0f;

                if (this._renderMode == BarGraphRenderMode.Bars)
                {
                    of = 10.0f;

                    // Draw x axis
                    graph.DrawLine(pen, this._xOrigin + of, this._yOrigin + this._graphHeight + of, this._xOrigin + this._graphWidth + of, this._yOrigin + this._graphHeight + of);

                    graph.DrawLine(pen, this._xOrigin, this._yOrigin + this._graphHeight, this._xOrigin + of, this._yOrigin + this._graphHeight + of);
                    graph.DrawLine(pen, this._xOrigin + this._graphWidth, this._yOrigin + this._graphHeight, this._xOrigin + of + this._graphWidth, this._yOrigin + this._graphHeight + of);
                }

                float currentX;
                float currentY = this._yOrigin + this._graphHeight + 2.0f;	// All x labels are drawn 2 pixels below x-axis
                float labelWidth = this._barWidth + this._spaceBtwBars;		// Fits exactly below the bar
                int i = 0;

                // Draw x labels
                foreach (DataItem item in this.DataPoints)
                {
                    if ((i % this._interval) == 0)
                    {
                        currentX = this._xOrigin + (i * labelWidth) + of + (labelWidth / 2);
                        RectangleF recLbl = new RectangleF(currentX - ((labelWidth * this._interval) / 2), currentY + of, labelWidth * this._interval, lblFont.Height * 2);
                        string lblString = this._displayLegend ? item.Label : item.Description;	// Decide what to show: short or long

                        graph.DrawString(lblString, lblFont, brs, recLbl, lblFormat);
                    }
                    i++;
                }
            }
            finally
            {
                if (lblFont != null)
                    lblFont.Dispose();
                if (brs != null)
                    brs.Dispose();
                if (lblFormat != null)
                    lblFormat.Dispose();
                if (pen != null)
                    pen.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method determines where to place the legend box.
        // It draws the legend border, legend description, and legend color code.
        //
        //*********************************************************************
        private void DrawLegend(Graphics graph)
        {
            Font lblFont = null;
            SolidBrush brs = null;
            StringFormat lblFormat = null;
            Pen pen = null;

            try
            {
                lblFont = new Font(this._fontFamily, _legendFontSize);
                brs = new SolidBrush(this._fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(this._fontColor);
                lblFormat.Alignment = StringAlignment.Near;

                // Calculate Legend drawing start point
                float startX = this._xOrigin + this._graphWidth + _graphLegendSpacer;
                float startY = this._yOrigin;

                float xColorCode = startX + _spacer;
                float xLegendText = xColorCode + _legendRectangleSize + _spacer;
                float legendHeight = 0.0f;
                for (int i = 0; i < this.DataPoints.Count; i++)
                {
                    DataItem point = this.DataPoints[i];
                    string text = point.Description + " (" + point.Label + ")";
                    float currentY = startY + _spacer + (i * (lblFont.Height + _spacer));
                    legendHeight += lblFont.Height + _spacer;

                    // Draw legend description
                    graph.DrawString(text, lblFont, brs, xLegendText, currentY, lblFormat);

                    // Draw color code
                    using (SolidBrush brsh = new SolidBrush(this.DataPoints[i].ItemColor))
                        graph.FillRectangle(brsh, xColorCode, currentY + 3f, _legendRectangleSize, _legendRectangleSize);
                }

                // Draw legend border
                graph.DrawRectangle(pen, startX, startY, this._legendWidth, legendHeight + _spacer);
            }
            finally
            {
                if (lblFont != null)
                    lblFont.Dispose();
                if (brs != null)
                    brs.Dispose();
                if (lblFormat != null)
                    lblFormat.Dispose();
                if (pen != null)
                    pen.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method calculates all measurement aspects of the bar graph from the given data points
        //
        //*********************************************************************
        private void CalculateGraphDimension() 
        {
            this.FindLongestTickValue();
			
            // Need to add another character for spacing; this is not used for drawing, just for calculation
            this._longestTickValue += "0";		
            //_maxTickValueWidth = CalculateImgFontWidth(_longestTickValue, _labelFontSize, FontFamily);
            this._maxTickValueWidth = 0.0f;

            float currentTick;
            string tickString;
            for (int i = 0; i < this._yTickCount; i++)
            {
                currentTick = this._maxValue - i * this._yTickValue;	
                tickString = currentTick.ToString("#,###.##");

                float measured = this.CalculateImgFontWidth(tickString, _labelFontSize, this.FontFamily);

                if (measured > this._maxTickValueWidth)
                    this._maxTickValueWidth = measured;
            }

            float leftOffset = _spacer + this._maxTickValueWidth + (this._yTitle == null ? 0.0f : 20.0f);
            float rtOffset = 0.0f;

            if (this._displayLegend) 
            {
                this._legendWidth = _spacer + _legendRectangleSize + _spacer + this._maxLabelWidth + _spacer;
                rtOffset = _graphLegendSpacer + this._legendWidth + _spacer;
            }
            else
                rtOffset = _spacer;		// Make graph in the middle

            if (this._renderMode == BarGraphRenderMode.Bars)
                rtOffset += 10.0f;

            rtOffset += 10.0f;

            this._graphHeight = this._totalHeight - this._topBuffer - this._bottomBuffer - (this._xTitle == null ? 0.0f : 20.0f);	// Buffer spaces are used to print labels
            this._graphWidth = this._totalWidth - leftOffset - rtOffset;
            this._xOrigin = leftOffset;
            this._yOrigin = this._topBuffer;

            // Once the correct _maxValue is determined, then calculate _scaleFactor
            this._scaleFactor = this._maxValue / this._graphHeight;
        }

        //*********************************************************************
        //
        // This method determines the longest tick value from the given data points.
        // The result is needed to calculate the correct graph dimension.
        //
        //*********************************************************************
        private void FindLongestTickValue()
        {
            float currentTick;
            string tickString;
            for (int i = 0; i < this._yTickCount; i++)
            {
                currentTick = this._maxValue - i * this._yTickValue;	
                tickString = currentTick.ToString("#,###.##");
                if (this._longestTickValue.Length < tickString.Length)
                    this._longestTickValue = tickString;
            }
        }

        //*********************************************************************
        //
        // This method calculates the image width in pixel for a given text
        //
        //*********************************************************************
        private float CalculateImgFontWidth(string text, float size, string family)
        {
            Bitmap bmp = null;
            Graphics graph = null;
            Font font = null;

            try
            {
                font = new Font(family, size);

                // Calculate the size of the string.
                bmp = new Bitmap(1,1,PixelFormat.Format32bppArgb);
                graph = Graphics.FromImage(bmp);
                SizeF oSize = graph.MeasureString(text, font);
                oSize.Width = 4 + (float)Math.Ceiling(oSize.Width);
			
                return oSize.Width;
            }
            finally
            {
                if (graph != null)
                    graph.Dispose();
                if (bmp != null)
                    bmp.Dispose();
                if (font != null)
                    font.Dispose();
            }
        }

        //*********************************************************************
        //
        // This method creates abbreviation from long description; used for making legend
        //
        //*********************************************************************
        private string MakeShortLabel(string text)
        {
            string label = text;
            if (text.Length > 2) 
            {
                int midPostition = Convert.ToInt32(Math.Floor(text.Length / 2.0));
                label = text.Substring(0, 1) + text.Substring(midPostition, 1) + text.Substring(text.Length - 1, 1);
            }
            return label;
        }

        //*********************************************************************
        //
        // This method calculates the max value and each tick mark value for the bar graph.
        //
        //*********************************************************************
        private void CalculateTickAndMax()
        {
            float tempMax = 0.0f;

            // Give graph some head room first about 10% of current max
            this._maxValue *= 1.1f;

            if (this._maxValue != 0.0f)
            {
                // Find a rounded value nearest to the current max value
                // Calculate this max first to give enough space to draw value on each bar
                double exp = Convert.ToDouble(Math.Floor(Math.Log10(this._maxValue)));
                tempMax = Convert.ToSingle(Math.Ceiling(this._maxValue / Math.Pow(10, exp)) * Math.Pow(10, exp));
            }
            else
                tempMax = 1.0f;

            // Once max value is calculated, tick value can be determined; tick value should be a whole number
            this._yTickValue = tempMax / this._yTickCount;
            double expTick = Convert.ToDouble(Math.Floor(Math.Log10(this._yTickValue)));
            this._yTickValue = Convert.ToSingle(Math.Ceiling(this._yTickValue / Math.Pow(10, expTick)) * Math.Pow(10, expTick));

            // Re-calculate the max value with the new tick value
            this._maxValue = this._yTickValue * this._yTickCount;
        }

        //*********************************************************************
        //
        // This method calculates the height for each bar in the graph
        //
        //*********************************************************************
        private void CalculateSweepValues()
        {
            // Called when all values and scale factor are known
            // All values calculated here are relative from (_xOrigin, _yOrigin)
            int i = 0;
            foreach (DataItem item in this.DataPoints)
            {
                // This implementation does not support negative value
                if (item.Value >= 0)
                    item.SweepSize = item.Value / this._scaleFactor;
				
                // (_spaceBtwBars/2) makes half white space for the first bar
                item.StartPos = (this._spaceBtwBars / 2) + i * (this._barWidth + this._spaceBtwBars);
                i++;
            }
        }

        //*********************************************************************
        //
        // This method calculates the width for each bar in the graph
        //
        //*********************************************************************
        private void CalculateBarWidth(int dataCount, float barGraphWidth)
        {
            // White space between each bar is the same as bar width itself
            this._barWidth = barGraphWidth / (dataCount * 2);  // Each bar has 1 white space 
            //_barWidth =/* (float)Math.Floor(*/_barWidth/*)*/;
            this._spaceBtwBars = this._barWidth;
        }

        //*********************************************************************
        //
        // This method assigns default value to the bar graph properties and is only 
        // called from BarGraph constructors
        //
        //*********************************************************************
        private void AssignDefaultSettings()
        {
            // default values
            this._totalWidth = 680f;
            this._totalHeight = 450f;
            this._fontFamily = "Verdana";
            this._backColor = Color.White;
            this._fontColor = Color.Black;
            this._topBuffer = 30f;
            this._bottomBuffer = 30f;
            this._yTickCount = 2;
            this._displayLegend = false;
            this._displayBarData = false;
        }
    }
}