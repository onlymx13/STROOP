﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using STROOP.Controls.Map;
using OpenTK.Graphics.OpenGL;
using STROOP.Utilities;
using STROOP.Structs.Configurations;
using STROOP.Structs;
using OpenTK;

namespace STROOP.Map3
{
    public abstract class Map3Object : IDisposable
    {
        public Color Color = SystemColors.Control;
        public double Opacity = 1;

        public Map3Object()
        {
        }

        public abstract void DrawOnControl();

        public abstract void Dispose();
    }
}
