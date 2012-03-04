// Type: System.Web.Security.FormsAuthentication
// Assembly: System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.dll

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Management;
using System.Web.Util;

namespace System.Web.Security
{
  public sealed class FormsAuthentication
  {
    private static object _lockObject = new object();
    private static string _CookieDomain = (string) null;
    internal const string RETURN_URL = "ReturnUrl";
    private static bool _Initialized;
    private static string _FormsName;
    private static FormsProtectionEnum _Protection;
    private static int _Timeout;
    private static string _FormsCookiePath;
    private static bool _RequireSSL;
    private static bool _SlidingExpiration;
    private static string _LoginUrl;
    private static string _DefaultUrl;
    private static HttpCookieMode _CookieMode;
    private static bool _EnableCrossAppRedirects;
    private static TicketCompatibilityMode _TicketCompatibilityMode;
    private const int MAX_TICKET_LENGTH = 4096;
    private const string CONFIG_DEFAULT_COOKIE = ".ASPXAUTH";

    public static bool IsEnabled
    {
      get
      {
        return AuthenticationConfig.Mode == AuthenticationMode.Forms;
      }
    }

    public static string FormsCookieName
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._FormsName;
      }
    }

    public static string FormsCookiePath
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._FormsCookiePath;
      }
    }

    public static bool RequireSSL
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._RequireSSL;
      }
    }

    public static TimeSpan Timeout
    {
      get
      {
        FormsAuthentication.Initialize();
        return new TimeSpan(0, FormsAuthentication._Timeout, 0);
      }
    }

    public static bool SlidingExpiration
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._SlidingExpiration;
      }
    }

    public static HttpCookieMode CookieMode
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._CookieMode;
      }
    }

    public static string CookieDomain
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._CookieDomain;
      }
    }

    public static bool EnableCrossAppRedirects
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._EnableCrossAppRedirects;
      }
    }

    public static TicketCompatibilityMode TicketCompatibilityMode
    {
      get
      {
        FormsAuthentication.Initialize();
        return FormsAuthentication._TicketCompatibilityMode;
      }
    }

    public static bool CookiesSupported
    {
      get
      {
        HttpContext current = HttpContext.Current;
        if (current != null)
          return !CookielessHelperClass.UseCookieless(current, false, FormsAuthentication.CookieMode);
        else
          return true;
      }
    }

    public static string LoginUrl
    {
      get
      {
        FormsAuthentication.Initialize();
        HttpContext current = HttpContext.Current;
        if (current != null)
          return AuthenticationConfig.GetCompleteLoginUrl(current, FormsAuthentication._LoginUrl);
        if (FormsAuthentication._LoginUrl.Length == 0 || (int) FormsAuthentication._LoginUrl[0] != 47 && FormsAuthentication._LoginUrl.IndexOf("//", StringComparison.Ordinal) < 0)
          return "/" + FormsAuthentication._LoginUrl;
        else
          return FormsAuthentication._LoginUrl;
      }
    }

    public static string DefaultUrl
    {
      get
      {
        FormsAuthentication.Initialize();
        HttpContext current = HttpContext.Current;
        if (current != null)
          return AuthenticationConfig.GetCompleteLoginUrl(current, FormsAuthentication._DefaultUrl);
        if (FormsAuthentication._DefaultUrl.Length == 0 || (int) FormsAuthentication._DefaultUrl[0] != 47 && FormsAuthentication._DefaultUrl.IndexOf("//", StringComparison.Ordinal) < 0)
          return "/" + FormsAuthentication._DefaultUrl;
        else
          return FormsAuthentication._DefaultUrl;
      }
    }

    static FormsAuthentication()
    {
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public FormsAuthentication()
    {
    }

    public static string HashPasswordForStoringInConfigFile(string password, string passwordFormat)
    {
      if (password == null)
        throw new ArgumentNullException("password");
      if (passwordFormat == null)
        throw new ArgumentNullException("passwordFormat");
      HashAlgorithm hashAlgorithm;
      if (StringUtil.EqualsIgnoreCase(passwordFormat, "sha1"))
        hashAlgorithm = (HashAlgorithm) SHA1.Create();
      else if (StringUtil.EqualsIgnoreCase(passwordFormat, "md5"))
        hashAlgorithm = (HashAlgorithm) MD5.Create();
      else
        throw new ArgumentException(System.Web.SR.GetString("InvalidArgumentValue", new object[1]
        {
          (object) "passwordFormat"
        }));
      return MachineKeySection.ByteArrayToHexString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password)), 0);
    }

    public static void Initialize()
    {
      if (FormsAuthentication._Initialized)
        return;
      lock (FormsAuthentication._lockObject)
      {
        if (FormsAuthentication._Initialized)
          return;
        AuthenticationSection local_0 = RuntimeConfig.GetAppConfig().Authentication;
        local_0.ValidateAuthenticationMode();
        FormsAuthentication._FormsName = local_0.Forms.Name;
        FormsAuthentication._RequireSSL = local_0.Forms.RequireSSL;
        FormsAuthentication._SlidingExpiration = local_0.Forms.SlidingExpiration;
        if (FormsAuthentication._FormsName == null)
          FormsAuthentication._FormsName = ".ASPXAUTH";
        FormsAuthentication._Protection = local_0.Forms.Protection;
        FormsAuthentication._Timeout = (int) local_0.Forms.Timeout.TotalMinutes;
        FormsAuthentication._FormsCookiePath = local_0.Forms.Path;
        FormsAuthentication._LoginUrl = local_0.Forms.LoginUrl;
        if (FormsAuthentication._LoginUrl == null)
          FormsAuthentication._LoginUrl = "login.aspx";
        FormsAuthentication._DefaultUrl = local_0.Forms.DefaultUrl;
        if (FormsAuthentication._DefaultUrl == null)
          FormsAuthentication._DefaultUrl = "default.aspx";
        FormsAuthentication._CookieMode = local_0.Forms.Cookieless;
        FormsAuthentication._CookieDomain = local_0.Forms.Domain;
        FormsAuthentication._EnableCrossAppRedirects = local_0.Forms.EnableCrossAppRedirects;
        FormsAuthentication._TicketCompatibilityMode = local_0.Forms.TicketCompatibilityMode;
        FormsAuthentication._Initialized = true;
      }
    }

    public static FormsAuthenticationTicket Decrypt(string encryptedTicket)
    {
      if (string.IsNullOrEmpty(encryptedTicket) || encryptedTicket.Length > 4096)
      {
        throw new ArgumentException(System.Web.SR.GetString("InvalidArgumentValue", new object[1]
        {
          (object) "encryptedTicket"
        }));
      }
      else
      {
        FormsAuthentication.Initialize();
        byte[] numArray1 = (byte[]) null;
        if (encryptedTicket.Length % 2 == 0)
        {
          try
          {
            numArray1 = MachineKeySection.HexStringToByteArray(encryptedTicket);
          }
          catch
          {
          }
        }
        if (numArray1 == null)
          numArray1 = HttpServerUtility.UrlTokenDecode(encryptedTicket);
        if (numArray1 == null || numArray1.Length < 1)
        {
          throw new ArgumentException(System.Web.SR.GetString("InvalidArgumentValue", new object[1]
          {
            (object) "encryptedTicket"
          }));
        }
        else
        {
          if (FormsAuthentication._Protection == FormsProtectionEnum.All || FormsAuthentication._Protection == FormsProtectionEnum.Encryption)
          {
            numArray1 = MachineKeySection.EncryptOrDecryptData(false, numArray1, (byte[]) null, 0, numArray1.Length, false, false, IVType.Random);
            if (numArray1 == null)
              return (FormsAuthenticationTicket) null;
          }
          int length = numArray1.Length;
          if (FormsAuthentication._Protection == FormsProtectionEnum.All || FormsAuthentication._Protection == FormsProtectionEnum.Validation)
          {
            if (!MachineKeySection.VerifyHashedData(numArray1))
              return (FormsAuthenticationTicket) null;
            length -= MachineKeySection.HashSize;
          }
          int num1 = length > 4096 ? 4096 : length;
          StringBuilder szName = new StringBuilder(num1);
          StringBuilder szData = new StringBuilder(num1);
          StringBuilder szPath = new StringBuilder(num1);
          byte[] numArray2 = (byte[]) null;
          byte[] pBytes = new byte[4];
          long[] pDates = new long[2];
          int num2;
          if (FormsAuthentication.TicketCompatibilityMode == TicketCompatibilityMode.Framework20)
          {
            num2 = System.Web.UnsafeNativeMethods.CookieAuthParseTicket(numArray1, length, szName, num1, szData, num1, szPath, num1, pBytes, pDates);
          }
          else
          {
            int index1 = 8;
            pBytes[0] = numArray1[index1];
            int index2 = index1 + 1;
            if (index2 + 3 > length)
              return (FormsAuthenticationTicket) null;
            if ((int) numArray1[index2] != 0 || (int) numArray1[index2 + 1] != 0 || (int) numArray1[index2 + 2] != 0)
              return (FormsAuthenticationTicket) null;
            int startIndex1 = index2 + 3;
            if (startIndex1 + 4 > length)
              return (FormsAuthenticationTicket) null;
            int count1 = BitConverter.ToInt32(numArray1, startIndex1);
            int index3 = startIndex1 + 4;
            if (index3 + count1 > length)
              return (FormsAuthenticationTicket) null;
            szName.Append(Encoding.Unicode.GetChars(numArray1, index3, count1));
            int startIndex2 = index3 + count1;
            if (startIndex2 + 8 > length)
              return (FormsAuthenticationTicket) null;
            pDates[0] = BitConverter.ToInt64(numArray1, startIndex2);
            int index4 = startIndex2 + 8;
            if (index4 + 1 > length)
              return (FormsAuthenticationTicket) null;
            pBytes[1] = numArray1[index4];
            int startIndex3 = index4 + 1;
            if (startIndex3 + 8 > length)
              return (FormsAuthenticationTicket) null;
            pDates[1] = BitConverter.ToInt64(numArray1, startIndex3);
            int index5 = startIndex3 + 8;
            if (index5 + 1 > length)
              return (FormsAuthenticationTicket) null;
            pBytes[2] = numArray1[index5];
            int startIndex4 = index5 + 1;
            if (startIndex4 + 4 > length)
              return (FormsAuthenticationTicket) null;
            int count2 = BitConverter.ToInt32(numArray1, startIndex4);
            int srcOffset = startIndex4 + 4;
            if (count2 > 0)
            {
              if (srcOffset + count2 > length)
                return (FormsAuthenticationTicket) null;
              numArray2 = new byte[count2];
              Buffer.BlockCopy((Array) numArray1, srcOffset, (Array) numArray2, 0, count2);
            }
            int startIndex5 = srcOffset + count2;
            if (startIndex5 + 4 > length)
              return (FormsAuthenticationTicket) null;
            int count3 = BitConverter.ToInt32(numArray1, startIndex5);
            int index6 = startIndex5 + 4;
            if (index6 + count3 > length)
              return (FormsAuthenticationTicket) null;
            szData.Append(Encoding.Unicode.GetChars(numArray1, index6, count3));
            int startIndex6 = index6 + count3;
            if (startIndex6 + 4 > length)
              return (FormsAuthenticationTicket) null;
            int count4 = BitConverter.ToInt32(numArray1, startIndex6);
            int index7 = startIndex6 + 4;
            if (index7 + count4 > length)
              return (FormsAuthenticationTicket) null;
            szPath.Append(Encoding.Unicode.GetChars(numArray1, index7, count4));
            int num3 = index7 + count4;
            num2 = 0;
          }
          if (num2 != 0)
            return (FormsAuthenticationTicket) null;
          DateTime issueDate;
          DateTime expiration;
          if (FormsAuthentication.TicketCompatibilityMode == TicketCompatibilityMode.Framework20)
          {
            issueDate = DateTime.FromFileTime(pDates[0]);
            expiration = DateTime.FromFileTime(pDates[1]);
          }
          else
          {
            issueDate = DateTime.FromFileTimeUtc(pDates[0]).ToLocalTime();
            expiration = DateTime.FromFileTimeUtc(pDates[1]).ToLocalTime();
          }
          FormsAuthenticationTicket authenticationTicket = new FormsAuthenticationTicket((int) pBytes[0], ((object) szName).ToString(), issueDate, expiration, (int) pBytes[1] != 0, ((object) szData).ToString(), ((object) szPath).ToString());
          if (FormsAuthentication.TicketCompatibilityMode > TicketCompatibilityMode.Framework20)
          {
            authenticationTicket.InternalVersion = (int) pBytes[2];
            authenticationTicket.InternalData = numArray2;
          }
          return authenticationTicket;
        }
      }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static string Encrypt(FormsAuthenticationTicket ticket)
    {
      return FormsAuthentication.Encrypt(ticket, true);
    }

    internal static string Encrypt(FormsAuthenticationTicket ticket, bool hexEncodedTicket)
    {
      if (ticket == null)
        throw new ArgumentNullException("ticket");
      FormsAuthentication.Initialize();
      byte[] numArray1 = FormsAuthentication.MakeTicketIntoBinaryBlob(ticket);
      if (numArray1 == null)
        return (string) null;
      if (FormsAuthentication._Protection == FormsProtectionEnum.All || FormsAuthentication._Protection == FormsProtectionEnum.Validation)
      {
        byte[] numArray2 = MachineKeySection.HashData(numArray1, (byte[]) null, 0, numArray1.Length);
        if (numArray2 == null)
          return (string) null;
        byte[] numArray3 = new byte[numArray2.Length + numArray1.Length];
        Buffer.BlockCopy((Array) numArray1, 0, (Array) numArray3, 0, numArray1.Length);
        Buffer.BlockCopy((Array) numArray2, 0, (Array) numArray3, numArray1.Length, numArray2.Length);
        numArray1 = numArray3;
      }
      if (FormsAuthentication._Protection == FormsProtectionEnum.All || FormsAuthentication._Protection == FormsProtectionEnum.Encryption)
        numArray1 = MachineKeySection.EncryptOrDecryptData(true, numArray1, (byte[]) null, 0, numArray1.Length, false, false, IVType.Random);
      if (!hexEncodedTicket)
        return HttpServerUtility.UrlTokenEncode(numArray1);
      else
        return MachineKeySection.ByteArrayToHexString(numArray1, 0);
    }

    public static bool Authenticate(string name, string password)
    {
      bool flag = FormsAuthentication.InternalAuthenticate(name, password);
      if (flag)
      {
        PerfCounters.IncrementCounter(AppPerfCounter.FORMS_AUTH_SUCCESS);
        WebBaseEvent.RaiseSystemEvent((object) null, 4001, name);
      }
      else
      {
        PerfCounters.IncrementCounter(AppPerfCounter.FORMS_AUTH_FAIL);
        WebBaseEvent.RaiseSystemEvent((object) null, 4005, name);
      }
      return flag;
    }

    public static void SignOut()
    {
      FormsAuthentication.Initialize();
      HttpContext current = HttpContext.Current;
      bool flag = current.CookielessHelper.DoesCookieValueExistInOriginal('F');
      current.CookielessHelper.SetCookieValue('F', (string) null);
      if (!CookielessHelperClass.UseCookieless(current, false, FormsAuthentication.CookieMode) || current.Request.Browser.Cookies)
      {
        string str = string.Empty;
        if (current.Request.Browser["supportsEmptyStringInCookieValue"] == "false")
          str = "NoCookie";
        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, str);
        cookie.HttpOnly = true;
        cookie.Path = FormsAuthentication._FormsCookiePath;
        cookie.Expires = new DateTime(1999, 10, 12);
        cookie.Secure = FormsAuthentication._RequireSSL;
        if (FormsAuthentication._CookieDomain != null)
          cookie.Domain = FormsAuthentication._CookieDomain;
        current.Response.Cookies.RemoveCookie(FormsAuthentication.FormsCookieName);
        current.Response.Cookies.Add(cookie);
      }
      if (!flag)
        return;
      current.Response.Redirect(FormsAuthentication.GetLoginPage((string) null), false);
    }

    public static void SetAuthCookie(string userName, bool createPersistentCookie)
    {
      FormsAuthentication.Initialize();
      FormsAuthentication.SetAuthCookie(userName, createPersistentCookie, FormsAuthentication.FormsCookiePath);
    }

    public static void SetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath)
    {
      FormsAuthentication.Initialize();
      HttpContext current = HttpContext.Current;
      if (!current.Request.IsSecureConnection && FormsAuthentication.RequireSSL)
        throw new HttpException(System.Web.SR.GetString("Connection_not_secure_creating_secure_cookie"));
      bool flag = CookielessHelperClass.UseCookieless(current, false, FormsAuthentication.CookieMode);
      HttpCookie authCookie = FormsAuthentication.GetAuthCookie(userName, createPersistentCookie, flag ? "/" : strCookiePath, !flag);
      if (!flag)
      {
        HttpContext.Current.Response.Cookies.Add(authCookie);
        current.CookielessHelper.SetCookieValue('F', (string) null);
      }
      else
        current.CookielessHelper.SetCookieValue('F', authCookie.Value);
    }

    public static HttpCookie GetAuthCookie(string userName, bool createPersistentCookie)
    {
      FormsAuthentication.Initialize();
      return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie, FormsAuthentication.FormsCookiePath);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath)
    {
      return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie, strCookiePath, true);
    }

    private static HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath, bool hexEncodedTicket)
    {
      FormsAuthentication.Initialize();
      if (userName == null)
        userName = string.Empty;
      if (strCookiePath == null || strCookiePath.Length < 1)
        strCookiePath = FormsAuthentication.FormsCookiePath;
      FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, userName, DateTime.Now, DateTime.Now.AddMinutes((double) FormsAuthentication._Timeout), createPersistentCookie, string.Empty, strCookiePath);
      if (FormsAuthentication.TicketCompatibilityMode > TicketCompatibilityMode.Framework20)
      {
        ticket.InternalVersion = (int) FormsAuthentication.TicketCompatibilityMode;
        ticket.InternalData = (byte[]) null;
      }
      string str = FormsAuthentication.Encrypt(ticket, hexEncodedTicket);
      if (str == null || str.Length < 1)
        throw new HttpException(System.Web.SR.GetString("Unable_to_encrypt_cookie_ticket"));
      HttpCookie httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, str);
      httpCookie.HttpOnly = true;
      httpCookie.Path = strCookiePath;
      httpCookie.Secure = FormsAuthentication._RequireSSL;
      if (FormsAuthentication._CookieDomain != null)
        httpCookie.Domain = FormsAuthentication._CookieDomain;
      if (ticket.IsPersistent)
        httpCookie.Expires = ticket.Expiration;
      return httpCookie;
    }

    internal static string GetReturnUrl(bool useDefaultIfAbsent)
    {
      FormsAuthentication.Initialize();
      HttpContext current = HttpContext.Current;
      string str = current.Request.QueryString["ReturnUrl"];
      if (str == null)
      {
        str = current.Request.Form["ReturnUrl"];
        if (!string.IsNullOrEmpty(str) && !str.Contains("/") && str.Contains("%"))
          str = HttpUtility.UrlDecode(str);
      }
      if (!string.IsNullOrEmpty(str) && !FormsAuthentication.EnableCrossAppRedirects && !UrlPath.IsPathOnSameServer(str, current.Request.Url))
        str = (string) null;
      if (!string.IsNullOrEmpty(str) && CrossSiteScriptingValidation.IsDangerousUrl(str))
        throw new HttpException(System.Web.SR.GetString("Invalid_redirect_return_url"));
      if (str != null || !useDefaultIfAbsent)
        return str;
      else
        return FormsAuthentication.DefaultUrl;
    }

    public static string GetRedirectUrl(string userName, bool createPersistentCookie)
    {
      if (userName == null)
        return (string) null;
      else
        return FormsAuthentication.GetReturnUrl(true);
    }

    public static void RedirectFromLoginPage(string userName, bool createPersistentCookie)
    {
      FormsAuthentication.Initialize();
      FormsAuthentication.RedirectFromLoginPage(userName, createPersistentCookie, FormsAuthentication.FormsCookiePath);
    }

    public static void RedirectFromLoginPage(string userName, bool createPersistentCookie, string strCookiePath)
    {
      FormsAuthentication.Initialize();
      if (userName == null)
        return;
      HttpContext current = HttpContext.Current;
      string returnUrl = FormsAuthentication.GetReturnUrl(true);
      string url;
      if (FormsAuthentication.CookiesSupported || FormsAuthentication.IsPathWithinAppRoot(current, returnUrl))
      {
        FormsAuthentication.SetAuthCookie(userName, createPersistentCookie, strCookiePath);
        url = FormsAuthentication.RemoveQueryStringVariableFromUrl(returnUrl, FormsAuthentication.FormsCookieName);
        if (!FormsAuthentication.CookiesSupported)
        {
          int num = url.IndexOf("://", StringComparison.Ordinal);
          if (num > 0)
          {
            int startIndex = url.IndexOf('/', num + 3);
            if (startIndex > 0)
              url = url.Substring(startIndex);
          }
        }
      }
      else
      {
        if (!FormsAuthentication.EnableCrossAppRedirects)
          throw new HttpException(System.Web.SR.GetString("Can_not_issue_cookie_or_redirect"));
        HttpCookie authCookie = FormsAuthentication.GetAuthCookie(userName, createPersistentCookie, strCookiePath);
        string str = FormsAuthentication.RemoveQueryStringVariableFromUrl(returnUrl, authCookie.Name);
        if (str.IndexOf('?') > 0)
          url = str + "&" + authCookie.Name + "=" + authCookie.Value;
        else
          url = str + "?" + authCookie.Name + "=" + authCookie.Value;
      }
      current.Response.Redirect(url, false);
    }

    public static FormsAuthenticationTicket RenewTicketIfOld(FormsAuthenticationTicket tOld)
    {
      if (tOld == null)
        return (FormsAuthenticationTicket) null;
      DateTime now = DateTime.Now;
      TimeSpan timeSpan = now - tOld.IssueDate;
      if (tOld.Expiration - now > timeSpan)
        return tOld;
      FormsAuthenticationTicket authenticationTicket = new FormsAuthenticationTicket(tOld.Version, tOld.Name, now, now + tOld.Expiration - tOld.IssueDate, tOld.IsPersistent, tOld.UserData, tOld.CookiePath);
      if (FormsAuthentication.TicketCompatibilityMode > TicketCompatibilityMode.Framework20)
      {
        authenticationTicket.InternalVersion = tOld.InternalVersion;
        authenticationTicket.InternalData = tOld.InternalData;
      }
      return authenticationTicket;
    }

    public static void EnableFormsAuthentication(NameValueCollection configurationData)
    {
      BuildManager.ThrowIfPreAppStartNotRunning();
      configurationData = configurationData ?? new NameValueCollection();
      AuthenticationConfig.Mode = AuthenticationMode.Forms;
      FormsAuthentication.Initialize();
      string str1 = configurationData["defaultUrl"];
      if (!string.IsNullOrEmpty(str1))
        FormsAuthentication._DefaultUrl = str1;
      string str2 = configurationData["loginUrl"];
      if (string.IsNullOrEmpty(str2))
        return;
      FormsAuthentication._LoginUrl = str2;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal static string GetLoginPage(string extraQueryString)
    {
      return FormsAuthentication.GetLoginPage(extraQueryString, false);
    }

    internal static string GetLoginPage(string extraQueryString, bool reuseReturnUrl)
    {
      HttpContext current = HttpContext.Current;
      string strUrl = FormsAuthentication.LoginUrl;
      if (strUrl.IndexOf('?') >= 0)
        strUrl = FormsAuthentication.RemoveQueryStringVariableFromUrl(strUrl, "ReturnUrl");
      int num = strUrl.IndexOf('?');
      if (num < 0)
        strUrl = strUrl + "?";
      else if (num < strUrl.Length - 1)
        strUrl = strUrl + "&";
      string str1 = (string) null;
      if (reuseReturnUrl)
        str1 = HttpUtility.UrlEncode(FormsAuthentication.GetReturnUrl(false), current.Request.QueryStringEncoding);
      if (str1 == null)
        str1 = HttpUtility.UrlEncode(current.Request.RawUrl, current.Request.ContentEncoding);
      string str2 = strUrl + "ReturnUrl=" + str1;
      if (!string.IsNullOrEmpty(extraQueryString))
        str2 = str2 + "&" + extraQueryString;
      return str2;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static void RedirectToLoginPage()
    {
      FormsAuthentication.RedirectToLoginPage((string) null);
    }

    public static void RedirectToLoginPage(string extraQueryString)
    {
      HttpContext.Current.Response.Redirect(FormsAuthentication.GetLoginPage(extraQueryString), false);
    }

    internal static string RemoveQueryStringVariableFromUrl(string strUrl, string QSVar)
    {
      int posQ = strUrl.IndexOf('?');
      if (posQ < 0)
        return strUrl;
      string sep1 = "&";
      string str1 = "?";
      string token1 = sep1 + QSVar + "=";
      FormsAuthentication.RemoveQSVar(ref strUrl, posQ, token1, sep1, sep1.Length);
      string token2 = str1 + QSVar + "=";
      FormsAuthentication.RemoveQSVar(ref strUrl, posQ, token2, sep1, str1.Length);
      string sep2 = HttpUtility.UrlEncode("&");
      string str2 = HttpUtility.UrlEncode("?");
      string token3 = sep2 + HttpUtility.UrlEncode(QSVar + "=");
      FormsAuthentication.RemoveQSVar(ref strUrl, posQ, token3, sep2, sep2.Length);
      string token4 = str2 + HttpUtility.UrlEncode(QSVar + "=");
      FormsAuthentication.RemoveQSVar(ref strUrl, posQ, token4, sep2, str2.Length);
      return strUrl;
    }

    private static bool InternalAuthenticate(string name, string password)
    {
      if (name == null || password == null)
        return false;
      FormsAuthentication.Initialize();
      AuthenticationSection authentication = RuntimeConfig.GetAppConfig().Authentication;
      authentication.ValidateAuthenticationMode();
      FormsAuthenticationUserCollection users = authentication.Forms.Credentials.Users;
      if (users == null)
        return false;
      FormsAuthenticationUser authenticationUser = users[name.ToLower(CultureInfo.InvariantCulture)];
      if (authenticationUser == null)
        return false;
      string password1 = authenticationUser.Password;
      if (password1 == null)
        return false;
      string strA;
      switch (authentication.Forms.Credentials.PasswordFormat)
      {
        case FormsAuthPasswordFormat.Clear:
          strA = password;
          break;
        case FormsAuthPasswordFormat.SHA1:
          strA = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
          break;
        case FormsAuthPasswordFormat.MD5:
          strA = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
          break;
        default:
          return false;
      }
      return string.Compare(strA, password1, authentication.Forms.Credentials.PasswordFormat != FormsAuthPasswordFormat.Clear ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == 0;
    }

    private static byte[] MakeTicketIntoBinaryBlob(FormsAuthenticationTicket ticket)
    {
      byte[] pData = new byte[4096];
      byte[] pBytes = new byte[4];
      long[] pDates = new long[2];
      byte[] numArray1 = new byte[3];
      if (FormsAuthentication._Protection != FormsProtectionEnum.All && FormsAuthentication._Protection != FormsProtectionEnum.Encryption || MachineKeySection.CompatMode == MachineKeyCompatibilityMode.Framework20SP1)
      {
        byte[] data = new byte[8];
        new RNGCryptoServiceProvider().GetBytes(data);
        Buffer.BlockCopy((Array) data, 0, (Array) pData, 0, 8);
      }
      pBytes[0] = (byte) ticket.Version;
      pBytes[1] = ticket.IsPersistent ? (byte) 1 : (byte) 0;
      if (FormsAuthentication.TicketCompatibilityMode == TicketCompatibilityMode.Framework20)
      {
        pDates[0] = ticket.IssueDate.ToFileTime();
        pDates[1] = ticket.Expiration.ToFileTime();
      }
      else
      {
        pDates[0] = ticket.IssueDate.ToUniversalTime().ToFileTimeUtc();
        pDates[1] = ticket.Expiration.ToUniversalTime().ToFileTimeUtc();
        pBytes[2] = (byte) ticket.InternalVersion;
      }
      int count1;
      if (FormsAuthentication.TicketCompatibilityMode == TicketCompatibilityMode.Framework20)
      {
        count1 = System.Web.UnsafeNativeMethods.CookieAuthConstructTicket(pData, pData.Length, ticket.Name, ticket.UserData, ticket.CookiePath, pBytes, pDates);
      }
      else
      {
        int index1 = 8;
        int length = pData.Length;
        if (ticket.Name == null || ticket.UserData == null || ticket.CookiePath == null)
          return (byte[]) null;
        pData[index1] = pBytes[0];
        int dstOffset1 = index1 + 1;
        Buffer.BlockCopy((Array) numArray1, 0, (Array) pData, dstOffset1, 3);
        int dstOffset2 = dstOffset1 + 3;
        int byteCount1 = Encoding.Unicode.GetByteCount(ticket.Name);
        if (dstOffset2 + 4 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) BitConverter.GetBytes(byteCount1), 0, (Array) pData, dstOffset2, 4);
        int dstOffset3 = dstOffset2 + 4;
        if (dstOffset3 + byteCount1 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) Encoding.Unicode.GetBytes(ticket.Name), 0, (Array) pData, dstOffset3, byteCount1);
        int dstOffset4 = dstOffset3 + byteCount1;
        if (dstOffset4 + 8 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) BitConverter.GetBytes(pDates[0]), 0, (Array) pData, dstOffset4, 8);
        int index2 = dstOffset4 + 8;
        if (index2 + 1 > length)
          return (byte[]) null;
        pData[index2] = pBytes[1];
        int dstOffset5 = index2 + 1;
        if (dstOffset5 + 8 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) BitConverter.GetBytes(pDates[1]), 0, (Array) pData, dstOffset5, 8);
        int index3 = dstOffset5 + 8;
        if (index3 + 1 > length)
          return (byte[]) null;
        pData[index3] = pBytes[2];
        int dstOffset6 = index3 + 1;
        if (dstOffset6 + 4 > length)
          return (byte[]) null;
        int count2 = ticket.InternalData != null ? ticket.InternalData.Length : 0;
        Buffer.BlockCopy((Array) BitConverter.GetBytes(count2), 0, (Array) pData, dstOffset6, 4);
        int dstOffset7 = dstOffset6 + 4;
        if (dstOffset7 + count2 > length)
          return (byte[]) null;
        if (count2 != 0)
        {
          Buffer.BlockCopy((Array) ticket.InternalData, 0, (Array) pData, dstOffset7, count2);
          dstOffset7 += count2;
        }
        int byteCount2 = Encoding.Unicode.GetByteCount(ticket.UserData);
        if (dstOffset7 + 4 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) BitConverter.GetBytes(byteCount2), 0, (Array) pData, dstOffset7, 4);
        int dstOffset8 = dstOffset7 + 4;
        if (dstOffset8 + byteCount2 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) Encoding.Unicode.GetBytes(ticket.UserData), 0, (Array) pData, dstOffset8, byteCount2);
        int dstOffset9 = dstOffset8 + byteCount2;
        int byteCount3 = Encoding.Unicode.GetByteCount(ticket.CookiePath);
        if (dstOffset9 + 4 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) BitConverter.GetBytes(byteCount3), 0, (Array) pData, dstOffset9, 4);
        int dstOffset10 = dstOffset9 + 4;
        if (dstOffset10 + byteCount3 > length)
          return (byte[]) null;
        Buffer.BlockCopy((Array) Encoding.Unicode.GetBytes(ticket.CookiePath), 0, (Array) pData, dstOffset10, byteCount3);
        count1 = dstOffset10 + byteCount3;
      }
      if (count1 < 0)
        return (byte[]) null;
      byte[] numArray2 = new byte[count1];
      Buffer.BlockCopy((Array) pData, 0, (Array) numArray2, 0, count1);
      return numArray2;
    }

    private static void RemoveQSVar(ref string strUrl, int posQ, string token, string sep, int lenAtStartToLeave)
    {
      for (int length = strUrl.LastIndexOf(token, StringComparison.Ordinal); length >= posQ; length = strUrl.LastIndexOf(token, StringComparison.Ordinal))
      {
        int startIndex = strUrl.IndexOf(sep, length + token.Length, StringComparison.Ordinal) + sep.Length;
        strUrl = startIndex < sep.Length || startIndex >= strUrl.Length ? strUrl.Substring(0, length) : strUrl.Substring(0, length + lenAtStartToLeave) + strUrl.Substring(startIndex);
      }
    }

    private static bool IsPathWithinAppRoot(HttpContext context, string path)
    {
      Uri result;
      if (!Uri.TryCreate(path, UriKind.Absolute, out result) || result.IsLoopback || string.Equals(context.Request.Url.Host, result.Host, StringComparison.OrdinalIgnoreCase))
        return HttpRuntime.IsPathWithinAppRoot(path);
      else
        return false;
    }
  }
}
