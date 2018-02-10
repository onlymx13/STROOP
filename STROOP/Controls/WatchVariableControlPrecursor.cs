﻿using STROOP.Extensions;
using STROOP.Managers;
using STROOP.Structs;
using STROOP.Structs.Configurations;
using STROOP.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

namespace STROOP.Controls
{
    public class WatchVariableControlPrecursor
    {
        private readonly string _name;
        private readonly WatchVariable _watchVar;
        private readonly WatchVariableSubclass _subclass;
        private readonly Color? _backgroundColor;
        private readonly bool? _useHex;
        private readonly bool? _invertBool;
        private readonly WatchVariableCoordinate? _coordinate;
        private readonly List<VariableGroup> _groupList;

        public WatchVariableControlPrecursor(
            string name,
            WatchVariable watchVar,
            WatchVariableSubclass subclass,
            Color? backgroundColor,
            bool? useHex,
            bool? invertBool,
            WatchVariableCoordinate? coordinate,
            List<VariableGroup> groupList)
        {
            _name = name;
            _watchVar = watchVar;
            _subclass = subclass;
            _backgroundColor = backgroundColor;
            _useHex = useHex;
            _invertBool = invertBool;
            _coordinate = coordinate;
            _groupList = groupList;
        }

        public WatchVariableControlPrecursor(XElement element)
        {

            /// Watchvariable params
            string typeName = (element.Attribute(XName.Get("type"))?.Value);
            string specialType = element.Attribute(XName.Get("specialType"))?.Value;
            BaseAddressTypeEnum baseAddressType = WatchVariableUtilities.GetBaseAddressType(element.Attribute(XName.Get("base")).Value);
            uint? offsetUS = ParsingUtilities.ParseHexNullable(element.Attribute(XName.Get("offsetUS"))?.Value);
            uint? offsetJP = ParsingUtilities.ParseHexNullable(element.Attribute(XName.Get("offsetJP"))?.Value);
            uint? offsetPAL = ParsingUtilities.ParseHexNullable(element.Attribute(XName.Get("offsetPAL"))?.Value);
            uint? offsetDefault = ParsingUtilities.ParseHexNullable(element.Attribute(XName.Get("offset"))?.Value);
            uint? mask = element.Attribute(XName.Get("mask")) != null ?
                (uint?)ParsingUtilities.ParseHex(element.Attribute(XName.Get("mask")).Value) : null;

            if (offsetDefault.HasValue && (offsetUS.HasValue || offsetJP.HasValue || offsetPAL.HasValue))
            {
                throw new ArgumentOutOfRangeException("Can't have both a default offset value and a rom-specific offset value");
            }

            if (specialType != null)
            {
                if (baseAddressType != BaseAddressTypeEnum.None &&
                    baseAddressType != BaseAddressTypeEnum.Object &&
                    baseAddressType != BaseAddressTypeEnum.Triangle)
                {
                    throw new ArgumentOutOfRangeException("Special var cannot have base address type " + baseAddressType);
                }

                if (offsetDefault.HasValue || offsetUS.HasValue || offsetJP.HasValue || offsetPAL.HasValue)
                {
                    throw new ArgumentOutOfRangeException("Special var cannot have any type of offset");
                }

                if (mask != null)
                {
                    throw new ArgumentOutOfRangeException("Special var cannot have mask");
                }
            }

            _watchVar = 
                new WatchVariable(
                    typeName,
                    specialType,
                    baseAddressType,
                    offsetUS,
                    offsetJP,
                    offsetPAL,
                    offsetDefault,
                    mask);

            _name = element.Value;
            _subclass = WatchVariableUtilities.GetSubclass(element.Attribute(XName.Get("subclass"))?.Value);
            _groupList = WatchVariableUtilities.ParseVariableGroupList(element.Attribute(XName.Get("groupList"))?.Value);
            _backgroundColor = (element.Attribute(XName.Get("color")) != null) ?
                ColorTranslator.FromHtml(element.Attribute(XName.Get("color")).Value) : (Color?)null;
            _useHex = (element.Attribute(XName.Get("useHex")) != null) ?
                bool.Parse(element.Attribute(XName.Get("useHex")).Value) : (bool?)null;
            _invertBool = element.Attribute(XName.Get("invertBool")) != null ?
                bool.Parse(element.Attribute(XName.Get("invertBool")).Value) : (bool?)null;
            _coordinate = element.Attribute(XName.Get("coord")) != null ?
                WatchVariableUtilities.GetCoordinate(element.Attribute(XName.Get("coord")).Value) : (WatchVariableCoordinate?)null;

            if (_subclass == WatchVariableSubclass.Angle && specialType != null)
            {
                if (typeName != "ushort" && typeName != "short" && typeName != "uint" && typeName != "int")
                {
                    throw new ArgumentOutOfRangeException("Special angle vars must have a good type");
                }
            }

            if (_useHex.HasValue && (_subclass == WatchVariableSubclass.String))
            {
                throw new ArgumentOutOfRangeException("useHex cannot be used with var subclass String");
            }

            if ((_useHex == true) && (_subclass == WatchVariableSubclass.Object))
            {
                throw new ArgumentOutOfRangeException("useHex as true is redundant with var subclass Object");
            }

            if (_invertBool.HasValue && (_subclass != WatchVariableSubclass.Boolean))
            {
                throw new ArgumentOutOfRangeException("invertBool must be used with var subclass Boolean");
            }

            if (_coordinate.HasValue && (_subclass == WatchVariableSubclass.String))
            {
                throw new ArgumentOutOfRangeException("coordinate cannot be used with var subclass String");
            }
        }

        public WatchVariableControl CreateWatchVariableControl(Color? newColor = null)
        {
            return new WatchVariableControl(
                this,
                _name,
                _watchVar,
                _subclass,
                newColor ?? _backgroundColor,
                _useHex,
                _invertBool,
                _coordinate,
                _groupList);
        }

        public XElement ToXML()
        {
            XElement root = new XElement("Data", _name);

            if (_groupList.Count > 0)
                root.Add(new XAttribute("groupList", String.Join(",", _groupList)));

            root.Add(new XAttribute("base", _watchVar.BaseAddressType.ToString()));

            if (_watchVar.OffsetDefault != null)
                root.Add(new XAttribute(
                    "offset",
                    String.Format("0x{0:X}", _watchVar.OffsetDefault.Value)));

            if (_watchVar.OffsetUS != null)
                root.Add(new XAttribute(
                    "offsetUS",
                    String.Format("0x{0:X}", _watchVar.OffsetUS.Value)));

            if (_watchVar.OffsetJP != null)
                root.Add(new XAttribute(
                    "offsetJP",
                    String.Format("0x{0:X}", _watchVar.OffsetJP.Value)));

            if (_watchVar.OffsetPAL != null)
                root.Add(new XAttribute(
                    "offsetPAL",
                    String.Format("0x{0:X}", _watchVar.OffsetPAL.Value)));

            if (_watchVar.MemoryTypeName != null)
                root.Add(new XAttribute("type", _watchVar.MemoryTypeName));

            if (_watchVar.SpecialType != null)
                root.Add(new XAttribute("specialType", _watchVar.SpecialType));

            if (_watchVar.Mask != null)
                root.Add(new XAttribute(
                    "mask",
                    String.Format("0x{0:X" + _watchVar.NibbleCount + "}", _watchVar.Mask.Value)));

            if (_subclass != WatchVariableSubclass.Number)
                root.Add(new XAttribute("subclass", _subclass.ToString()));

            if (_invertBool.HasValue)
                root.Add(new XAttribute("invertBool", _invertBool.Value.ToString().ToLower()));

            if (_useHex.HasValue)
                root.Add(new XAttribute("useHex", _useHex.Value.ToString().ToLower()));

            if (_coordinate.HasValue)
                root.Add(new XAttribute("coord", _coordinate.Value.ToString()));

            if (_backgroundColor.HasValue)
                root.Add(new XAttribute(
                    "color",
                    "#" + ColorUtilities.ToString(_backgroundColor.Value)));

            return root;
        }

        public override string ToString()
        {
            return ToXML().ToString();
        }
    }
}