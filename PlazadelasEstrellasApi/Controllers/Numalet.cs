using System;
using System.Globalization;
using System.Text;

public sealed class Numalet
{
  private const int UNI = 0;
  private const int DIECI = 1;
  private const int DECENA = 2;
  private const int CENTENA = 3;
  private static string[,] _matriz = new string[4, 10]
  {
    {
      null,
      " uno",
      " dos",
      " tres",
      " cuatro",
      " cinco",
      " seis",
      " siete",
      " ocho",
      " nueve"
    },
    {
      " diez",
      " once",
      " doce",
      " trece",
      " catorce",
      " quince",
      " dieciseis",
      " diecisiete",
      " dieciocho",
      " diecinueve"
    },
    {
      null,
      null,
      null,
      " treinta",
      " cuarenta",
      " cincuenta",
      " sesenta",
      " setenta",
      " ochenta",
      " noventa"
    },
    {
      null,
      null,
      null,
      null,
      null,
      " quinientos",
      null,
      " setecientos",
      null,
      " novecientos"
    }
  };
  private const char sub = '\u001A';
  public const string SeparadorDecimalSalidaDefault = "Pesos";
  public const string MascaraSalidaDecimalDefault = "00'/100. M. N.'";
  public const int DecimalesDefault = 2;
  public const bool LetraCapitalDefault = false;
  public const bool ConvertirDecimalesDefault = false;
  public const bool ApocoparUnoParteEnteraDefault = false;
  public const bool ApocoparUnoParteDecimalDefault = false;
  private int _decimales = 2;
  private CultureInfo _cultureInfo = CultureInfo.CurrentCulture;
  private string _separadorDecimalSalida = "Pesos";
  private int _posiciones = 2;
  private string _mascaraSalidaDecimal;
  private string _mascaraSalidaDecimalInterna = "00'/100. M. N.'";
  private bool _esMascaraNumerica = true;
  private bool _letraCapital = false;
  private bool _convertirDecimales = false;
  private bool _apocoparUnoParteEntera = false;
  private bool _apocoparUnoParteDecimal;

  public int Decimales
  {
    get => this._decimales;
    set => this._decimales = value <= 10 ? value : throw new ArgumentException(value.ToString() + " excede el número máximo de decimales admitidos, solo se admiten hasta 10.");
  }

  public CultureInfo CultureInfo
  {
    get => this._cultureInfo;
    set => this._cultureInfo = value;
  }

  public string SeparadorDecimalSalida
  {
    get => this._separadorDecimalSalida;
    set
    {
      this._separadorDecimalSalida = value;
      this._apocoparUnoParteEntera = value.Trim().IndexOf(" ") > 0;
    }
  }

  public string MascaraSalidaDecimal
  {
    get => !string.IsNullOrEmpty(this._mascaraSalidaDecimal) ? this._mascaraSalidaDecimal : "";
    set
    {
      int index = 0;
      while (index < value.Length && value[index] == '0' | value[index] == '#')
        ++index;
      this._posiciones = index;
      if (index > 0)
      {
        this._decimales = index;
        this._esMascaraNumerica = true;
      }
      else
        this._esMascaraNumerica = false;
      this._mascaraSalidaDecimal = value;
      if (this._esMascaraNumerica)
      {
        string str1 = value.Substring(0, this._posiciones);
        string str2 = value.Substring(this._posiciones);
        char ch = '\u001A';
        string newValue = ch.ToString();
        string str3 = str2.Replace("''", newValue).Replace("'", string.Empty);
        ch = '\u001A';
        string oldValue = ch.ToString();
        string str4 = str3.Replace(oldValue, "'");
        this._mascaraSalidaDecimalInterna = str1 + "'" + str4 + "'";
      }
      else
        this._mascaraSalidaDecimalInterna = value.Replace("''", '\u001A'.ToString()).Replace("'", string.Empty).Replace('\u001A'.ToString(), "'");
    }
  }

  public bool LetraCapital
  {
    get => this._letraCapital;
    set => this._letraCapital = value;
  }

  public bool ConvertirDecimales
  {
    get => this._convertirDecimales;
    set
    {
      this._convertirDecimales = value;
      this._apocoparUnoParteDecimal = value;
      if (value)
      {
        if (!(this._mascaraSalidaDecimal == "00'/100. M. N.'"))
          return;
        this.MascaraSalidaDecimal = "";
      }
      else
      {
        if (!string.IsNullOrEmpty(this._mascaraSalidaDecimal))
          return;
        this.MascaraSalidaDecimal = "00'/100. M. N.'";
      }
    }
  }

  public bool ApocoparUnoParteEntera
  {
    get => this._apocoparUnoParteEntera;
    set => this._apocoparUnoParteEntera = value;
  }

  public bool ApocoparUnoParteDecimal
  {
    get => this._apocoparUnoParteDecimal;
    set => this._apocoparUnoParteDecimal = value;
  }

  public Numalet()
  {
    this.MascaraSalidaDecimal = "00'/100. M. N.'";
    this.SeparadorDecimalSalida = "Pesos";
    this.LetraCapital = false;
    this.ConvertirDecimales = this._convertirDecimales;
  }

  public Numalet(
    bool ConvertirDecimales,
    string MascaraSalidaDecimal,
    string SeparadorDecimalSalida,
    bool LetraCapital)
  {
    if (!string.IsNullOrEmpty(MascaraSalidaDecimal))
      this.MascaraSalidaDecimal = MascaraSalidaDecimal;
    if (!string.IsNullOrEmpty(SeparadorDecimalSalida))
      this._separadorDecimalSalida = SeparadorDecimalSalida;
    this._letraCapital = LetraCapital;
    this._convertirDecimales = ConvertirDecimales;
  }

  public string ToCustomString(double Numero) => Numalet.Convertir((Decimal)Numero, this._decimales, this._separadorDecimalSalida, this._mascaraSalidaDecimalInterna, this._esMascaraNumerica, this._letraCapital, this._convertirDecimales, this._apocoparUnoParteEntera, this._apocoparUnoParteDecimal);

  public string ToCustomString(string Numero)
  {
    double result;
    if (double.TryParse(Numero, NumberStyles.Float, (IFormatProvider)this._cultureInfo, out result))
      return this.ToCustomString(result);
    throw new ArgumentException("'" + Numero + "' no es un número válido.");
  }

  public string ToCustomString(Decimal Numero) => Numalet.ToString(Convert.ToDouble(Numero));

  public string ToCustomString(int Numero) => Numalet.Convertir((Decimal)Numero, 0, this._separadorDecimalSalida, this._mascaraSalidaDecimalInterna, this._esMascaraNumerica, this._letraCapital, this._convertirDecimales, this._apocoparUnoParteEntera, false);

  public static string ToString(int Numero) => Numalet.Convertir((Decimal)Numero, 0, (string)null, (string)null, true, false, false, false, false);

  public static string ToString(double Numero) => Numalet.Convertir((Decimal)Numero, 2, "Pesos", "00'/100. M. N.'", true, false, false, false, false);

  public static string ToString(string Numero, CultureInfo ReferenciaCultural)
  {
    double result;
    if (double.TryParse(Numero, NumberStyles.Float, (IFormatProvider)ReferenciaCultural, out result))
      return Numalet.ToString(result);
    throw new ArgumentException("'" + Numero + "' no es un número válido.");
  }

  public static string ToString(string Numero) => Numalet.ToString(Numero, CultureInfo.CurrentCulture);

  public static string ToString(Decimal Numero) => Numalet.ToString(Convert.ToDouble(Numero));

  private static string Convertir(
    Decimal Numero,
    int Decimales,
    string SeparadorDecimalSalida,
    string MascaraSalidaDecimal,
    bool EsMascaraNumerica,
    bool LetraCapital,
    bool ConvertirDecimales,
    bool ApocoparUnoParteEntera,
    bool ApocoparUnoParteDecimal)
  {
    StringBuilder stringBuilder = new StringBuilder();
    long num1 = (long)Math.Abs(Numero);
    if (num1 >= 1000000000000L || num1 < 0L)
      throw new ArgumentException("El número '" + Numero.ToString() + "' excedió los límites del conversor: [0;1.000.000.000.000)");
    if (num1 == 0L)
    {
      stringBuilder.Append(" cero");
    }
    else
    {
      string str1 = num1.ToString();
      int num2 = 0;
      int length = str1.Length;
      do
      {
        ++num2;
        string str2 = string.Empty;
        int num3 = int.Parse(length >= 3 ? str1.Substring(length - 3, 3) : str1.Substring(0, length));
        int num4 = num3 / 100;
        int num5 = num3 - num4 * 100;
        int index = num5 - num5 / 10 * 10;
        if (num5 > 0 && num5 < 10)
          str2 = Numalet._matriz[0, index] + str2;
        else if (num5 >= 10 && num5 < 20)
          str2 += Numalet._matriz[1, num5 - num5 / 10 * 10];
        else if (num5 == 20)
          str2 += " veinte";
        else if (num5 > 20 && num5 < 30)
          str2 = " veinti" + Numalet._matriz[0, index].Substring(1, Numalet._matriz[0, index].Length - 1);
        else if (num5 >= 30 && num5 < 100)
          str2 = (uint)index <= 0U ? str2 + Numalet._matriz[2, num5 / 10] : Numalet._matriz[2, num5 / 10] + " y" + Numalet._matriz[0, index] + str2;
        switch (num4)
        {
          case 1:
            str2 = num5 <= 0 ? " cien" + str2 : " ciento" + str2;
            break;
          case 5:
          case 7:
          case 9:
            str2 = Numalet._matriz[3, num3 / 100] + str2;
            break;
          default:
            if (num3 / 100 > 1)
            {
              str2 = Numalet._matriz[0, num3 / 100] + "cientos" + str2;
              break;
            }
            break;
        }
        if (num2 > 1 | ApocoparUnoParteEntera && num5 == 21)
          str2 = str2.Replace("veintiuno", "veintiun");
        else if (num2 > 1 | ApocoparUnoParteEntera && index == 1 && num5 != 11)
        {
          str2 = str2.Substring(0, str2.Length - 1);
        }
        else
        {
          switch (num5)
          {
            case 16:
              str2 = str2.Replace("dieciseis", "dieciseis");
              break;
            case 22:
              str2 = str2.Replace("veintidos", "veintidos");
              break;
            case 23:
              str2 = str2.Replace("veintitres", "veintitres");
              break;
            case 26:
              str2 = str2.Replace("veintiseis", "veintiseis");
              break;
          }
        }
        switch (num2)
        {
          case 2:
          case 4:
            if (num3 > 0)
            {
              str2 += " mil";
              break;
            }
            break;
          case 3:
            str2 = num1 >= 2000000L ? str2 + " millones" : str2 + " millón";
            break;
        }
        stringBuilder.Insert(0, str2);
        length -= 3;
      }
      while (length > 0);
    }
    if (Decimales > 0)
    {
      stringBuilder.Append(" " + SeparadorDecimalSalida + " ");
      int num6 = (int)Math.Round((double)(Numero - (Decimal)(long)Numero) * Math.Pow(10.0, (double)Decimales), 0);
      if (ConvertirDecimales)
      {
        bool flag = MascaraSalidaDecimal == "00'/100. M. N.'";
        stringBuilder.Append(Numalet.Convertir((Decimal)num6, 0, (string)null, (string)null, EsMascaraNumerica, false, false, ApocoparUnoParteDecimal && !EsMascaraNumerica, false) + " " + (EsMascaraNumerica ? "" : MascaraSalidaDecimal));
      }
      else if (EsMascaraNumerica)
        stringBuilder.Append(num6.ToString(MascaraSalidaDecimal));
      else
        stringBuilder.Append(num6.ToString() + " " + MascaraSalidaDecimal);
    }
    return LetraCapital ? stringBuilder[1].ToString().ToUpper() + stringBuilder.ToString(2, stringBuilder.Length - 2) : stringBuilder.ToString().Substring(1);
  }
}
