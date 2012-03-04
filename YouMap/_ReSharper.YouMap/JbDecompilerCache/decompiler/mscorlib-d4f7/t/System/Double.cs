// Type: System.Double
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace System
{
  [ComVisible(true)]
  [Serializable]
  public struct Double : IComparable, IFormattable, IConvertible, IComparable<double>, IEquatable<double>
  {
    internal static double NegativeZero = BitConverter.Int64BitsToDouble(long.MinValue);
    public const double MinValue = -1.79769313486232E+308;
    public const double MaxValue = 1.79769313486232E+308;
    public const double Epsilon = 4.94065645841247E-324;
    public const double NegativeInfinity = double.NegativeInfinity;
    public const double PositiveInfinity = double.PositiveInfinity;
    public const double NaN = double.NaN;
    internal double m_value;

    static Double()
    {
    }

    public static bool operator ==(double left, double right)
    {
      return left == right;
    }

    public static bool operator !=(double left, double right)
    {
      return left != right;
    }

    public static bool operator <(double left, double right)
    {
      return left < right;
    }

    public static bool operator >(double left, double right)
    {
      return left > right;
    }

    public static bool operator <=(double left, double right)
    {
      return left <= right;
    }

    public static bool operator >=(double left, double right)
    {
      return left >= right;
    }

    [SecuritySafeCritical]
    public static unsafe bool IsInfinity(double d)
    {
      return (*(long*) &d & long.MaxValue) == 9218868437227405312L;
    }

    public static bool IsPositiveInfinity(double d)
    {
      if (d == double.PositiveInfinity)
        return true;
      else
        return false;
    }

    public static bool IsNegativeInfinity(double d)
    {
      if (d == double.NegativeInfinity)
        return true;
      else
        return false;
    }

    [SecuritySafeCritical]
    internal static unsafe bool IsNegative(double d)
    {
      return (*(long*) &d & long.MinValue) == long.MinValue;
    }

    [SecuritySafeCritical]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    public static unsafe bool IsNaN(double d)
    {
      return (ulong) (*(long*) &d & long.MaxValue) > 9218868437227405312UL;
    }

    public int CompareTo(object value)
    {
      if (value == null)
        return 1;
      if (!(value is double))
        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeDouble"));
      double d = (double) value;
      if (this < d)
        return -1;
      if (this > d)
        return 1;
      if (this == d)
        return 0;
      if (!double.IsNaN(this))
        return 1;
      if (!double.IsNaN(d))
        return -1;
      else
        return 0;
    }

    public int CompareTo(double value)
    {
      if (this < value)
        return -1;
      if (this > value)
        return 1;
      if (this == value)
        return 0;
      if (!double.IsNaN(this))
        return 1;
      if (!double.IsNaN(value))
        return -1;
      else
        return 0;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is double))
        return false;
      double d = (double) obj;
      if (d == this)
        return true;
      if (double.IsNaN(d))
        return double.IsNaN(this);
      else
        return false;
    }

    public bool Equals(double obj)
    {
      if (obj == this)
        return true;
      if (double.IsNaN(obj))
        return double.IsNaN(this);
      else
        return false;
    }

    [SecuritySafeCritical]
    public override unsafe int GetHashCode()
    {
      double num1 = this;
      if (num1 == 0.0)
        return 0;
      long num2 = *(long*) &num1;
      return (int) num2 ^ (int) (num2 >> 32);
    }

    [SecuritySafeCritical]
    public override string ToString()
    {
      return Number.FormatDouble(this, (string) null, NumberFormatInfo.CurrentInfo);
    }

    [SecuritySafeCritical]
    public string ToString(string format)
    {
      return Number.FormatDouble(this, format, NumberFormatInfo.CurrentInfo);
    }

    [SecuritySafeCritical]
    public string ToString(IFormatProvider provider)
    {
      return Number.FormatDouble(this, (string) null, NumberFormatInfo.GetInstance(provider));
    }

    [SecuritySafeCritical]
    public string ToString(string format, IFormatProvider provider)
    {
      return Number.FormatDouble(this, format, NumberFormatInfo.GetInstance(provider));
    }

    public static double Parse(string s)
    {
      return double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.CurrentInfo);
    }

    public static double Parse(string s, NumberStyles style)
    {
      NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
      return double.Parse(s, style, NumberFormatInfo.CurrentInfo);
    }

    public static double Parse(string s, IFormatProvider provider)
    {
      return double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.GetInstance(provider));
    }

    public static double Parse(string s, NumberStyles style, IFormatProvider provider)
    {
      NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
      return double.Parse(s, style, NumberFormatInfo.GetInstance(provider));
    }

    public static bool TryParse(string s, out double result)
    {
      return double.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, NumberFormatInfo.CurrentInfo, out result);
    }

    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out double result)
    {
      NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
      return double.TryParse(s, style, NumberFormatInfo.GetInstance(provider), out result);
    }

    public TypeCode GetTypeCode()
    {
      return TypeCode.Double;
    }

    bool IConvertible.ToBoolean(IFormatProvider provider)
    {
      return Convert.ToBoolean(this);
    }

    char IConvertible.ToChar(IFormatProvider provider)
    {
      throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", (object) "Double", (object) "Char"));
    }

    sbyte IConvertible.ToSByte(IFormatProvider provider)
    {
      return Convert.ToSByte(this);
    }

    byte IConvertible.ToByte(IFormatProvider provider)
    {
      return Convert.ToByte(this);
    }

    short IConvertible.ToInt16(IFormatProvider provider)
    {
      return Convert.ToInt16(this);
    }

    ushort IConvertible.ToUInt16(IFormatProvider provider)
    {
      return Convert.ToUInt16(this);
    }

    int IConvertible.ToInt32(IFormatProvider provider)
    {
      return Convert.ToInt32(this);
    }

    uint IConvertible.ToUInt32(IFormatProvider provider)
    {
      return Convert.ToUInt32(this);
    }

    long IConvertible.ToInt64(IFormatProvider provider)
    {
      return Convert.ToInt64(this);
    }

    ulong IConvertible.ToUInt64(IFormatProvider provider)
    {
      return Convert.ToUInt64(this);
    }

    float IConvertible.ToSingle(IFormatProvider provider)
    {
      return Convert.ToSingle(this);
    }

    double IConvertible.ToDouble(IFormatProvider provider)
    {
      return this;
    }

    Decimal IConvertible.ToDecimal(IFormatProvider provider)
    {
      return Convert.ToDecimal(this);
    }

    DateTime IConvertible.ToDateTime(IFormatProvider provider)
    {
      throw new InvalidCastException(Environment.GetResourceString("InvalidCast_FromTo", (object) "Double", (object) "DateTime"));
    }

    object IConvertible.ToType(Type type, IFormatProvider provider)
    {
      return Convert.DefaultToType((IConvertible) this, type, provider);
    }

    private static double Parse(string s, NumberStyles style, NumberFormatInfo info)
    {
      return Number.ParseDouble(s, style, info);
    }

    private static bool TryParse(string s, NumberStyles style, NumberFormatInfo info, out double result)
    {
      if (s == null)
      {
        result = 0.0;
        return false;
      }
      else
      {
        if (!Number.TryParseDouble(s, style, info, out result))
        {
          string str = s.Trim();
          if (str.Equals(info.PositiveInfinitySymbol))
            result = double.PositiveInfinity;
          else if (str.Equals(info.NegativeInfinitySymbol))
          {
            result = double.NegativeInfinity;
          }
          else
          {
            if (!str.Equals(info.NaNSymbol))
              return false;
            result = double.NaN;
          }
        }
        return true;
      }
    }
  }
}
