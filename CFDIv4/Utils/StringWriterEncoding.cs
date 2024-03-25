using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CFDIv4.Utils
{
  class StringWriterEncoding: StringWriter
  {
    public StringWriterEncoding(Encoding encoding)
            : base()
    {
      this.m_Encoding = encoding;
    }
    private readonly Encoding m_Encoding;
    public override Encoding Encoding
    {
      get
      {
        return this.m_Encoding;
      }
    }
  }
}
