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
            m_RangeFrom = rangeFrom;
            m_RangeTo = rangeTo;
            m_Name = name;
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
        private const float _labelFontSize = 7f;
        private const int _legendFontSize = 9;
        private const float _legendRectangleSize = 10F;
        private const float _spacer = 5F;
        private BarGraphRenderMode _renderMode;
        // Overall related members
        private Color _backColor;
        private string _fontFamily;
        private string _longestTickValue = string.Empty;// Used to calculate max value width
        private float _maxTickValueWidth;// Used to calculate left offset of bar graph
        private float _totalHeight;
        private float _totalWidth;
        // Graph related members
        private float _barWidth;
        private float _bottomBuffer;// Space from bottom to x axis
        private bool _displayBarData;
        private Color _fontColor;
        private float _graphHeight;
        private float _graphWidth;
        private float _maxValue = 0.0f;// = final tick value * tick count
        private float _scaleFactor;// = _maxValue / _graphHeight
        private float _spaceBtwBars;// For now same as _barWidth
        private float _topBuffer;// Space from top to the top of y axis
        private float _xOrigin;// x position where graph starts drawing
        private float _yOrigin;// y position where graph starts drawing
        private string _yLabel;
        private int _yTickCount;
        private float _yTickValue;// Value for each tick = _maxValue/_yTickCount

        // Legend related members
        private bool _displayLegend;
        private float _legendWidth;
        private string _longestLabel = string.Empty;// Used to calculate legend width
        private float _maxLabelWidth = 0.0f;
        private string _xTitle, _yTitle;
        public BarGraphRenderer()
        {
            AssignDefaultSettings();
        }

        public BarGraphRenderer(Color bgColor)
        {
            AssignDefaultSettings();
            BackgroundColor = bgColor;
        }

        public string FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                _fontFamily = value;
            }
        }
        public BarGraphRenderMode RenderMode
        {
            get
            {
                return _renderMode;
            }
            set
            {
                _renderMode = value;
            }
        }
        public Color BackgroundColor
        {
            set
            {
                _backColor = value;
            }
        }
        public int BottomBuffer
        {
            set
            {
                _bottomBuffer = Convert.ToSingle(value);
            }
        }
        public Color FontColor
        {
            set
            {
                _fontColor = value;
            }
        }
        public int Height
        {
            get
            {
                return Convert.ToInt32(_totalHeight);
            }
            set
            {
                _totalHeight = Convert.ToSingle(value);
            }
        }
        public int Width
        {
            get
            {
                return Convert.ToInt32(_totalWidth);
            }
            set
            {
                _totalWidth = Convert.ToSingle(value);
            }
        }
        public bool ShowLegend
        {
            get
            {
                return _displayLegend;
            }
            set
            {
                _displayLegend = value;
            }
        }
        public bool ShowData
        {
            get
            {
                return _displayBarData;
            }
            set
            {
                _displayBarData = value;
            }
        }
        public int TopBuffer
        {
            set
            {
                _topBuffer = Convert.ToSingle(value);
            }
        }
        public string VerticalLabel
        {
            get
            {
                return _yLabel;
            }
            set
            {
                _yLabel = value;
            }
        }
        public int VerticalTickCount
        {
            get
            {
                return _yTickCount;
            }
            set
            {
                _yTickCount = value;
            }
        }
        public void SetTitles(string xTitle, string yTitle)
        {
            _xTitle = xTitle;
            _yTitle = yTitle;
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
                    string shortLbl = MakeShortLabel(labels[i]);

                    // For now put 0.0 for start position and sweep size
                    DataPoints.Add(new DataItem(shortLbl, labels[i], temp, 0.0f, 0.0f, GetColor(i)));

                    // Find max value from data; this is only temporary _maxValue
                    if (_maxValue < temp)
                        _maxValue = temp;

                    // Find the longest description
                    if (_displayLegend)
                    {
                        string currentLbl = labels[i] + " (" + shortLbl + ")";
                        float currentWidth = CalculateImgFontWidth(currentLbl, _legendFontSize, FontFamily);
                        if (_maxLabelWidth < currentWidth)
                        {
                            _longestLabel = currentLbl;
                            _maxLabelWidth = currentWidth;
                        }
                    }
                }

                CalculateTickAndMax();
                CalculateGraphDimension();
                CalculateBarWidth(DataPoints.Count, _graphWidth);
                CalculateSweepValues();
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
            CollectDataPoints(labels, values);
        }

        public void DrawRegions(Graphics gfx)
        {
            if (_regions == null)
                return;

            using (StringFormat textFormat = new StringFormat())
            {
                textFormat.Alignment = StringAlignment.Center;
                textFormat.LineAlignment = StringAlignment.Center;

                using (Font font = new Font(_fontFamily, _labelFontSize))
                {
                    using (Brush textBrush = new SolidBrush(_fontColor))
                    {
                        using (Pen solidPen = new Pen(_fontColor))
                        {
                            using (Pen lightPen = new Pen(Color.FromArgb(128, _fontColor)))
                            {
                                float labelWidth = _barWidth + _spaceBtwBars;

                                for (int i = 0; i < _regions.Length; ++i)
                                {
                                    BarRegion reg = _regions[i];

                                    RectangleF rc = new RectangleF(_xOrigin + (reg.m_RangeFrom * labelWidth), _yOrigin, (reg.m_RangeTo - reg.m_RangeFrom + 1) * labelWidth, _graphHeight);

                                    if (rc.X + rc.Width > _xOrigin + _graphWidth)
                                        rc.Width = _xOrigin + _graphWidth - rc.X;

                                    using (SolidBrush brsh = new SolidBrush(Color.FromArgb(48, GetColor(i))))
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
            int height = Convert.ToInt32(_totalHeight);
            int width = Convert.ToInt32(_totalWidth);

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics graph = Graphics.FromImage(bmp))
            {
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush brsh = new SolidBrush(_backColor))
                    graph.FillRectangle(brsh, -1, -1, bmp.Width + 1, bmp.Height + 1);

                DrawRegions(graph);
                DrawVerticalLabelArea(graph);
                DrawXLabelBack(graph);
                DrawBars(graph);
                DrawXLabelArea(graph);

                if (_displayLegend)
                    DrawLegend(graph);
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
                brsFont = new SolidBrush(_fontColor);
                valFont = new Font(_fontFamily, _labelFontSize);
                sfFormat = new StringFormat();
                sfFormat.Alignment = StringAlignment.Center;
                int i = 0;

                PointF[] linePoints = null;

                if (_renderMode == BarGraphRenderMode.Lines)
                    linePoints = new PointF[DataPoints.Count];

                int pointIndex = 0;

                // Draw bars and the value above each bar
                using (Pen pen = new Pen(_fontColor, 0.15f))
                {
                    using (SolidBrush whiteBrsh = new SolidBrush(Color.FromArgb(128, Color.White)))
                    {
                        foreach (DataItem item in DataPoints)
                        {
                            using (SolidBrush barBrush = new SolidBrush(item.ItemColor))
                            {
                                float itemY = _yOrigin + _graphHeight - item.SweepSize;

                                if (_renderMode == BarGraphRenderMode.Lines)
                                {
                                    linePoints[pointIndex++] = new PointF(_xOrigin + item.StartPos + (_barWidth / 2), itemY);
                                }
                                else if (_renderMode == BarGraphRenderMode.Bars)
                                {
                                    float ox = _xOrigin + item.StartPos;
                                    float oy = itemY;
                                    float ow = _barWidth;
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
                                    if (_displayBarData && (i % _interval) == 0)
                                    {
                                        float sectionWidth = (_barWidth + _spaceBtwBars);
                                        float startX = _xOrigin + (i * sectionWidth) + (sectionWidth / 2);  // This draws the value on center of the bar
                                        float startY = itemY - 2f - valFont.Height;					  // Positioned on top of each bar by 2 pixels
                                        RectangleF recVal = new RectangleF(startX - ((sectionWidth * _interval) / 2), startY, sectionWidth * _interval, valFont.Height);
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

                        if (_renderMode == BarGraphRenderMode.Lines)
                        {
                            if (linePoints.Length >= 2)
                            {
                                using (Pen linePen = new Pen(Color.FromArgb(220, Color.Red), 2.5f))
                                    graph.DrawCurve(linePen, linePoints, 0.5f);
                            }

                            using (Pen linePen = new Pen(Color.FromArgb(40, _fontColor), 0.8f))
                            {
                                for (int j = 0; j < linePoints.Length; ++j)
                                {
                                    graph.DrawLine(linePen, linePoints[j], new PointF(linePoints[j].X, _yOrigin + _graphHeight));

                                    DataItem item = DataPoints[j];
                                    float itemY = _yOrigin + _graphHeight - item.SweepSize;

                                    // Draw data value
                                    if (_displayBarData && (j % _interval) == 0)
                                    {
                                        graph.FillEllipse(brsFont, new RectangleF(linePoints[j].X - 2.0f, linePoints[j].Y - 2.0f, 4.0f, 4.0f));

                                        float sectionWidth = (_barWidth + _spaceBtwBars);
                                        float startX = _xOrigin + (j * sectionWidth) + (sectionWidth / 2);  // This draws the value on center of the bar
                                        float startY = itemY - 2f - valFont.Height;					  // Positioned on top of each bar by 2 pixels
                                        RectangleF recVal = new RectangleF(startX - ((sectionWidth * _interval) / 2), startY, sectionWidth * _interval, valFont.Height);
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

            float fo = (_yTitle == null ? 0.0f : 20.0f);

            try
            {
                brs = new SolidBrush(_fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(_fontColor);

                if (_yTitle != null)
                {
                    sfVLabel = new StringFormat();
                    sfVLabel.Alignment = StringAlignment.Center;
                    sfVLabel.LineAlignment = StringAlignment.Center;
                    sfVLabel.FormatFlags = StringFormatFlags.DirectionVertical;

                    lblFont = new Font(_fontFamily, _labelFontSize + 4.0f);
                    graph.DrawString(_yTitle, lblFont, brs, new RectangleF(0.0f, _yOrigin, 20.0f, _graphHeight), sfVLabel);
                    lblFont.Dispose();
                }

                sfVLabel = new StringFormat();
                lblFormat.Alignment = StringAlignment.Far;
                lblFormat.FormatFlags |= StringFormatFlags.NoClip;

                // Draw vertical label at the top of y-axis and place it in the middle top of y-axis
                lblFont = new Font(_fontFamily, _labelFontSize + 2.0f, FontStyle.Bold);
                RectangleF recVLabel = new RectangleF(0, _yOrigin - 2 * _spacer - lblFont.Height, _xOrigin * 2, lblFont.Height);
                sfVLabel.Alignment = StringAlignment.Center;
                sfVLabel.FormatFlags |= StringFormatFlags.NoClip;
                //graph.DrawRectangle(Pens.Black,Rectangle.Truncate(recVLabel));
                graph.DrawString(_yLabel, lblFont, brs, recVLabel, sfVLabel);
                lblFont.Dispose();

                lblFont = new Font(_fontFamily, _labelFontSize);
                // Draw all tick values and tick marks
                using (Pen smallPen = new Pen(Color.FromArgb(96, _fontColor), 0.8f))
                {
                    for (int i = 0; i < _yTickCount; i++)
                    {
                        float currentY = _topBuffer + (i * _yTickValue / _scaleFactor);	// Position for tick mark
                        float labelY = currentY - lblFont.Height / 2;						// Place label in the middle of tick
                        RectangleF lblRec = new RectangleF(_spacer + fo - 6, labelY, _maxTickValueWidth, lblFont.Height);

                        float currentTick = _maxValue - i * _yTickValue;					// Calculate tick value from top to bottom
                        graph.DrawString(currentTick.ToString("#,###.##"), lblFont, brs, lblRec, lblFormat);	// Draw tick value  
                        graph.DrawLine(pen, _xOrigin, currentY, _xOrigin - 4.0f, currentY);						// Draw tick mark

                        graph.DrawLine(smallPen, _xOrigin, currentY, _xOrigin + _graphWidth, currentY);
                    }
                }

                // Draw y axis
                graph.DrawLine(pen, _xOrigin, _yOrigin, _xOrigin, _yOrigin + _graphHeight);
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
                lblFont = new Font(_fontFamily, _labelFontSize);
                brs = new SolidBrush(_fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(_fontColor);

                lblFormat.Alignment = StringAlignment.Center;

                // Draw x axis
                graph.DrawLine(pen, _xOrigin, _yOrigin + _graphHeight, _xOrigin + _graphWidth, _yOrigin + _graphHeight);
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
                brs = new SolidBrush(_fontColor);
                pen = new Pen(_fontColor);

                if (_xTitle != null)
                {
                    lblFormat = new StringFormat();
                    lblFormat.Alignment = StringAlignment.Center;
                    lblFormat.LineAlignment = StringAlignment.Center;
                    //					sfVLabel.FormatFlags=StringFormatFlags.DirectionVertical;

                    lblFont = new Font(_fontFamily, _labelFontSize + 2.0f, FontStyle.Bold);
                    graph.DrawString(_xTitle, lblFont, brs, new RectangleF(_xOrigin, _yOrigin + _graphHeight + 14.0f + (_renderMode == BarGraphRenderMode.Bars ? 10.0f : 0.0f) + ((DataPoints.Count / _interval) > 24 ? 16.0f : 0.0f), _graphWidth, 20.0f), lblFormat);
                }

                lblFont = new Font(_fontFamily, _labelFontSize);
                lblFormat = new StringFormat();
                lblFormat.Alignment = StringAlignment.Center;
                lblFormat.FormatFlags |= StringFormatFlags.NoClip;
                lblFormat.Trimming = StringTrimming.None;
                //lblFormat.FormatFlags |= StringFormatFlags.NoWrap;

                float of = 0.0f;

                if (_renderMode == BarGraphRenderMode.Bars)
                {
                    of = 10.0f;

                    // Draw x axis
                    graph.DrawLine(pen, _xOrigin + of, _yOrigin + _graphHeight + of, _xOrigin + _graphWidth + of, _yOrigin + _graphHeight + of);

                    graph.DrawLine(pen, _xOrigin, _yOrigin + _graphHeight, _xOrigin + of, _yOrigin + _graphHeight + of);
                    graph.DrawLine(pen, _xOrigin + _graphWidth, _yOrigin + _graphHeight, _xOrigin + of + _graphWidth, _yOrigin + _graphHeight + of);
                }

                float currentX;
                float currentY = _yOrigin + _graphHeight + 2.0f;	// All x labels are drawn 2 pixels below x-axis
                float labelWidth = _barWidth + _spaceBtwBars;		// Fits exactly below the bar
                int i = 0;

                // Draw x labels
                foreach (DataItem item in DataPoints)
                {
                    if ((i % _interval) == 0)
                    {
                        currentX = _xOrigin + (i * labelWidth) + of + (labelWidth / 2);
                        RectangleF recLbl = new RectangleF(currentX - ((labelWidth * _interval) / 2), currentY + of, labelWidth * _interval, lblFont.Height * 2);
                        string lblString = _displayLegend ? item.Label : item.Description;	// Decide what to show: short or long

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
                lblFont = new Font(_fontFamily, _legendFontSize);
                brs = new SolidBrush(_fontColor);
                lblFormat = new StringFormat();
                pen = new Pen(_fontColor);
                lblFormat.Alignment = StringAlignment.Near;

                // Calculate Legend drawing start point
                float startX = _xOrigin + _graphWidth + _graphLegendSpacer;
                float startY = _yOrigin;

                float xColorCode = startX + _spacer;
                float xLegendText = xColorCode + _legendRectangleSize + _spacer;
                float legendHeight = 0.0f;
                for (int i = 0; i < DataPoints.Count; i++)
                {
                    DataItem point = DataPoints[i];
                    string text = point.Description + " (" + point.Label + ")";
                    float currentY = startY + _spacer + (i * (lblFont.Height + _spacer));
                    legendHeight += lblFont.Height + _spacer;

                    // Draw legend description
                    graph.DrawString(text, lblFont, brs, xLegendText, currentY, lblFormat);

                    // Draw color code
                    using (SolidBrush brsh = new SolidBrush(DataPoints[i].ItemColor))
                        graph.FillRectangle(brsh, xColorCode, currentY + 3f, _legendRectangleSize, _legendRectangleSize);
                }

                // Draw legend border
                graph.DrawRectangle(pen, startX, startY, _legendWidth, legendHeight + _spacer);
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
            FindLongestTickValue();

            // Need to add another character for spacing; this is not used for drawing, just for calculation
            _longestTickValue += "0";
            //_maxTickValueWidth = CalculateImgFontWidth(_longestTickValue, _labelFontSize, FontFamily);
            _maxTickValueWidth = 0.0f;

            float currentTick;
            string tickString;
            for (int i = 0; i < _yTickCount; i++)
            {
                currentTick = _maxValue - i * _yTickValue;
                tickString = currentTick.ToString("#,###.##");

                float measured = CalculateImgFontWidth(tickString, _labelFontSize, FontFamily);

                if (measured > _maxTickValueWidth)
                    _maxTickValueWidth = measured;
            }

            float leftOffset = _spacer + _maxTickValueWidth + (_yTitle == null ? 0.0f : 20.0f);
            float rtOffset = 0.0f;

            if (_displayLegend)
            {
                _legendWidth = _spacer + _legendRectangleSize + _spacer + _maxLabelWidth + _spacer;
                rtOffset = _graphLegendSpacer + _legendWidth + _spacer;
            }
            else
                rtOffset = _spacer;		// Make graph in the middle

            if (_renderMode == BarGraphRenderMode.Bars)
                rtOffset += 10.0f;

            rtOffset += 10.0f;

            _graphHeight = _totalHeight - _topBuffer - _bottomBuffer - (_xTitle == null ? 0.0f : 20.0f);	// Buffer spaces are used to print labels
            _graphWidth = _totalWidth - leftOffset - rtOffset;
            _xOrigin = leftOffset;
            _yOrigin = _topBuffer;

            // Once the correct _maxValue is determined, then calculate _scaleFactor
            _scaleFactor = _maxValue / _graphHeight;
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
            for (int i = 0; i < _yTickCount; i++)
            {
                currentTick = _maxValue - i * _yTickValue;
                tickString = currentTick.ToString("#,###.##");
                if (_longestTickValue.Length < tickString.Length)
                    _longestTickValue = tickString;
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
                bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
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
            _maxValue *= 1.1f;

            if (_maxValue != 0.0f)
            {
                // Find a rounded value nearest to the current max value
                // Calculate this max first to give enough space to draw value on each bar
                double exp = Convert.ToDouble(Math.Floor(Math.Log10(_maxValue)));
                tempMax = Convert.ToSingle(Math.Ceiling(_maxValue / Math.Pow(10, exp)) * Math.Pow(10, exp));
            }
            else
                tempMax = 1.0f;

            // Once max value is calculated, tick value can be determined; tick value should be a whole number
            _yTickValue = tempMax / _yTickCount;
            double expTick = Convert.ToDouble(Math.Floor(Math.Log10(_yTickValue)));
            _yTickValue = Convert.ToSingle(Math.Ceiling(_yTickValue / Math.Pow(10, expTick)) * Math.Pow(10, expTick));

            // Re-calculate the max value with the new tick value
            _maxValue = _yTickValue * _yTickCount;
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
            foreach (DataItem item in DataPoints)
            {
                // This implementation does not support negative value
                if (item.Value >= 0)
                    item.SweepSize = item.Value / _scaleFactor;

                // (_spaceBtwBars/2) makes half white space for the first bar
                item.StartPos = (_spaceBtwBars / 2) + i * (_barWidth + _spaceBtwBars);
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
            _barWidth = barGraphWidth / (dataCount * 2);  // Each bar has 1 white space 
            //_barWidth =/* (float)Math.Floor(*/_barWidth/*)*/;
            _spaceBtwBars = _barWidth;
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
            _totalWidth = 680f;
            _totalHeight = 450f;
            _fontFamily = "Verdana";
            _backColor = Color.White;
            _fontColor = Color.Black;
            _topBuffer = 30f;
            _bottomBuffer = 30f;
            _yTickCount = 2;
            _displayLegend = false;
            _displayBarData = false;
        }
    }
}