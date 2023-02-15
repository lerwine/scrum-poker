using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ScrumPokerServer
{
    public class XhtmlElementBuilder
    {
        private readonly XElement _element;
        public XElement Element { get { return _element; } }

        private XmlDateTimeSerializationMode _dateTimeOption = XmlDateTimeSerializationMode.RoundtripKind;
        public XmlDateTimeSerializationMode DateTimeOption
        {
            get { return _dateTimeOption; }
            set { _dateTimeOption = value; }
        }
        

        public XhtmlElementBuilder(string localName, params object[] content)
        {
            _element = new XElement(XNamespace.None.GetName(localName));
            Add(content);
        }

        public XhtmlElementBuilder WithDateTimeOption(XmlDateTimeSerializationMode dateTimeOption)
        {
            _dateTimeOption = dateTimeOption;
            return this;
        }

        private string ToStringValue(object obj)
        {
            if (obj == null)
                return "";
            if (obj is string)
                return obj as string;
            if (obj is char)
                return XmlConvert.ToString((char)obj);
            if (obj is bool)
                return XmlConvert.ToString((bool)obj);
            if (obj is byte)
                return XmlConvert.ToString((byte)obj);
            if (obj is sbyte)
                return XmlConvert.ToString((sbyte)obj);
            if (obj is short)
                return XmlConvert.ToString((short)obj);
            if (obj is ushort)
                return XmlConvert.ToString((ushort)obj);
            if (obj is int)
                return XmlConvert.ToString((int)obj);
            if (obj is uint)
                return XmlConvert.ToString((uint)obj);
            if (obj is long)
                return XmlConvert.ToString((long)obj);
            if (obj is ulong)
                return XmlConvert.ToString((ulong)obj);
            if (obj is float)
                return XmlConvert.ToString((float)obj);
            if (obj is double)
                return XmlConvert.ToString((double)obj);
            if (obj is decimal)
                return XmlConvert.ToString((decimal)obj);
            if (obj is TimeSpan)
                return XmlConvert.ToString((TimeSpan)obj);
            if (obj is Guid)
                return XmlConvert.ToString((Guid)obj);
            if (obj is Uri)
            {
                Uri uri = obj as Uri;
                return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
            }
            if (obj is byte[])
                return string.Join("", ((byte[])obj).Select(b => b.ToString("x2")));
            if (obj is char[])
                return new string((char[])obj);
            if (obj is DateTime)
                return XmlConvert.ToString((DateTime)obj, _dateTimeOption);
            Type t = obj.GetType();
            if (t.IsEnum)
                return Enum.GetName(t, obj);
            return Convert.ToString(obj) ?? "";
        }

        public XhtmlElementBuilder AddComment(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                XNode lastNode = _element.LastNode;
                if (lastNode is XComment)
                    ((XComment)lastNode).Value += text;
                else
                    _element.Add(new XComment(text));
            }
            return this;
        }

        public XhtmlElementBuilder SetAttribute(string localName, object value)
        {
            XName name = XNamespace.None.GetName(localName);
            XAttribute attribute = _element.Attribute(name);
            if (attribute == null)
            {
                if (value != null)
                    _element.Add(new XAttribute(name, ToStringValue(value)));
            }
            else if (value == null)
                attribute.Remove();
            else
                attribute.Value = ToStringValue(value);
            return this;
        }

        public XhtmlElementBuilder Add(params object[] content)
        {
            if (content != null)
                foreach (object obj in content)
                {
                    if (obj == null)
                        continue;
                    if (obj is XAttribute || obj is XNode)
                        _element.Add(obj);
                    else if (obj is XhtmlElementBuilder)
                        _element.Add(((XhtmlElementBuilder)obj)._element);
                    else
                    {
                        string value = ToStringValue(obj);
                        XNode lastNode = _element.LastNode;
                        if (lastNode is XText)
                        {
                            if (value.Length > 0)
                                ((XText)lastNode).Value += value;
                        }
                        else
                            _element.Add(new XText(value));
                    }
                }
            return this;
        }
    }
}