// Type: System.Security.Principal.IPrincipal
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Runtime.InteropServices;

namespace System.Security.Principal
{
  [ComVisible(true)]
  public interface IPrincipal
  {
    IIdentity Identity { get; }

    bool IsInRole(string role);
  }
}
