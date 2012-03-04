// Type: System.Security.Principal.IIdentity
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Runtime.InteropServices;

namespace System.Security.Principal
{
  [ComVisible(true)]
  public interface IIdentity
  {
    string Name { get; }

    string AuthenticationType { get; }

    bool IsAuthenticated { get; }
  }
}
