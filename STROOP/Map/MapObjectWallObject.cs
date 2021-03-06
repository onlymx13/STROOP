﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using STROOP.Utilities;
using STROOP.Structs.Configurations;
using STROOP.Structs;
using OpenTK;
using System.Drawing.Imaging;
using STROOP.Models;
using System.Windows.Forms;

namespace STROOP.Map
{
    public class MapObjectWallObject : MapWallObject
    {
        private readonly uint _objAddress;

        public MapObjectWallObject(uint objAddress)
            : base()
        {
            _objAddress = objAddress;
        }

        protected override List<TriangleDataModel> GetTriangles()
        {
            return TriangleUtilities.GetObjectTrianglesForObject(_objAddress)
                .FindAll(tri => tri.IsWall());
        }

        public override string GetName()
        {
            return "Wall Tris for " + PositionAngle.GetMapNameForObject(_objAddress);
        }

        public override Image GetInternalImage()
        {
            return Config.ObjectAssociations.TriangleWallImage;
        }

        public override ContextMenuStrip GetContextMenuStrip()
        {
            if (_contextMenuStrip == null)
            {
                _contextMenuStrip = CreateWallContextMenuStrip();
            }

            return _contextMenuStrip;
        }
    }
}
