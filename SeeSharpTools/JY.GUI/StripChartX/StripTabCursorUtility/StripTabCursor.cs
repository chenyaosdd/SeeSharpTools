﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using SeeSharpTools.JY.GUI.StripTabCursorUtility;
using SeeSharpTools.JY.GUI.StripChartXUtility;

namespace SeeSharpTools.JY.GUI
{
    //DynamicCursor类是public权限，为了避免在ToolBox显示DynamicCursor的控件，将DynamicCursor的控件功能迁移到内部类DynamicCursorControl中实现
    /// <summary>
    /// Class of data cursor
    /// </summary>
    public class StripTabCursor
    {
        private StripTabCursorCollection _collection;
        internal StripTabCursorControl Control { get; }

        /// <summary>
        /// Create a tab cursor instance
        /// </summary>
        public StripTabCursor()
        {
            this.Control = new StripTabCursorControl();
            this._name = "";
            this._xValue = 0;
            this.Color = Color.Red;
            this._enabled = true;

            this.SeriesIndex = -1;
        }

        internal void Initialize(StripTabCursorCollection collection)
        {
            this._collection = collection;
            this.Control.RefreshAndShowView = new Action(() =>
            {
                _collection.RefreshCursorValue(this);
                _collection.ShowCursorValue(this, true);
            });
            this.Control.MouseEnter += (sender, args) => { _collection.ShowCursorValue(this, true);};
            this.Control.MouseLeave += (sender, args) => { _collection.ShowCursorValue(this, false);};
        }

        const int MaxNameLength = 50;
        private string _name;
        /// <summary>
        /// Set or get cursor name
        /// </summary>
        [
            Browsable(true),
            Category("Design"),
            Description("Set or get the name of cursor.")
        ]
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || _name.Equals(value) || value.Length > MaxNameLength || 
                    null == _collection || _collection.Any(cursor => cursor.Name.Equals(value)))
                {
                    return;
                }
                _name = value;
            }
        }
        // TabCursor位置对应的坐标轴原始值
        private int _xValue;
        /// <summary>
        /// Set or get cursor value
        /// </summary>
        [
            Browsable(true),
            Category("Data"),
            Description("Get the X value of cursor.")
        ]
        public string XValue => _collection.GetXValue(_xValue);

        public int XIndex
        {
            get { return _xValue; }
            set
            {
                _xValue = value;
                _collection.RefreshCursorPosition(this);
            }
        }

        /// <summary>
        /// Set or get the Y value of cursor.
        /// </summary>
        [
            Browsable(true),
            Category("Data"),
            Description("Set or get the Y value of cursor.")
        ]
        public double YValue
        {
            get
            {
                double xAfterAlign = _xValue;
                double yValue = _collection.GetYValue(ref xAfterAlign, SeriesIndex);
                return yValue;
            }
            
        }

        /// <summary>
        /// Specify the index of series which the tabcursor will be attached to.
        /// </summary>
        [
            Browsable(true),
            Category("Behavior"),
            Description("Specify the index of series which the tabcursor will be attached to.")
        ]
        public int SeriesIndex { get; set; }

        /// <summary>
        /// Get the formated value string
        /// </summary>
        internal string ValueString => string.IsNullOrEmpty(_collection?.CursorValueFormat)
            ? _xValue.ToString()
            : _xValue.ToString(_collection.CursorValueFormat);

        /// <summary>
        /// Specify or get cursor color
        /// </summary>
        [
            Browsable(true),
            Category("Apperance"),
            Description("Specify or get the color of cursor.")
        ]
        public Color Color
        {
            get { return Control.CursorColor; }
            set { Control.CursorColor = value; }
        }

        private bool _enabled;

        /// <summary>
        /// Specify or get whether cursor enabled.
        /// </summary>
        [
            Browsable(true),
            Category("Design"),
            Description("Set or get the name of cursor.")
        ]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                _collection?.RefreshCursorPosition(this);
                _collection?.AttachOrDetachPaintEvent();
            }
        }

        public void ShowValue()
        {
            _collection.ShowCursorValue(this, true);
        }

        public void HideValue()
        {
            _collection.ShowCursorValue(this, false);
        }
    }

    
}