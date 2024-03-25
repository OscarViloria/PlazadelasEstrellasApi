using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CFDIv4.Utils
{
   public class CustomWriter : XmlWriter
   {
      XmlWriter _writer;
      bool _docElement = true;

      public string SchemaLocation { get; set; }
      public string NoNamespaceSchemaLocation { get; set; }

      public override WriteState WriteState => _writer.WriteState;

      public CustomWriter( XmlWriter writer )
      {
         _writer = writer;
      }

      public override void WriteStartElement( string prefix, string localName, string ns )
      {
         _writer.WriteStartElement(prefix, localName, ns);
         if ( _docElement )
         {
            
            if ( !string.IsNullOrEmpty(SchemaLocation) )
            {
               _writer.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", SchemaLocation);
            }
            _docElement = false;
         }
      }

      public override void Flush() => _writer.Flush();
      public override string LookupPrefix( string ns ) => _writer.LookupPrefix(ns);
      public override void WriteBase64( byte[] buffer, int index, int count ) => _writer.WriteBase64(buffer, index, count);
      public override void WriteCData( string text ) => _writer.WriteCData(text);
      public override void WriteCharEntity( char ch ) => _writer.WriteCharEntity(ch);
      public override void WriteChars( char[] buffer, int index, int count ) => _writer.WriteChars(buffer, index, count);
      public override void WriteComment( string text ) => _writer.WriteComment(text);
      public override void WriteDocType( string name, string pubid, string sysid, string subset ) => _writer.WriteDocType(name, pubid, sysid, subset);
      public override void WriteEndAttribute() => _writer.WriteEndAttribute();
      public override void WriteEndDocument() => _writer.WriteEndDocument();
      public override void WriteEndElement() => _writer.WriteEndElement();
      public override void WriteEntityRef( string name ) => _writer.WriteEntityRef(name);
      public override void WriteFullEndElement() => _writer.WriteFullEndElement();
      public override void WriteProcessingInstruction( string name, string text ) => _writer.WriteProcessingInstruction(name, text);
      public override void WriteRaw( char[] buffer, int index, int count ) => _writer.WriteRaw(buffer, index, count);
      public override void WriteRaw( string data ) => _writer.WriteRaw(data);
      public override void WriteStartAttribute( string prefix, string localName, string ns ) => _writer.WriteStartAttribute(prefix, localName, ns);
      public override void WriteStartDocument() => _writer.WriteStartDocument();
      public override void WriteStartDocument( bool standalone ) => _writer.WriteStartDocument(standalone);
      public override void WriteString( string text ) => _writer.WriteString(text);
      public override void WriteSurrogateCharEntity( char lowChar, char highChar ) => _writer.WriteSurrogateCharEntity(lowChar, highChar);
      public override void WriteWhitespace( string ws ) => _writer.WriteWhitespace(ws);
   }
}
