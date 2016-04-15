﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace X.Viewer.SketchFlow.Controls.Stamps
{
    interface IStamp
    {
        event EventHandler PerformAction;
        string GenerateXAML(double scaleX, double scaleY, double left, double top);
        void PopulateFromUIElement(UIElement element);
        void UpdateRotation(double angle);
    }
}
