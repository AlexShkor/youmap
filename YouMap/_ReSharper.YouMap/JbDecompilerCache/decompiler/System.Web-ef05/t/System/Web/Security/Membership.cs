// Type: System.Web.Security.Membership
// Assembly: System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Runtime;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Util;

namespace System.Web.Security
{
  public static class Membership
  {
    private static char[] punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
    private static int s_UserIsOnlineTimeWindow = 15;
    private static object s_lock = new object();
    private static bool s_Initialized = false;
    private static Exception s_InitializeException = (Exception) null;
    private static MembershipProviderCollection s_Providers;
    private static MembershipProvider s_Provider;
    private static bool s_InitializedDefaultProvider;
    private static string s_HashAlgorithmType;
    private static bool s_HashAlgorithmFromConfig;

    public static bool EnablePasswordRetrieval
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.EnablePasswordRetrieval;
      }
    }

    public static bool EnablePasswordReset
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.EnablePasswordReset;
      }
    }

    public static bool RequiresQuestionAndAnswer
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.RequiresQuestionAndAnswer;
      }
    }

    public static int UserIsOnlineTimeWindow
    {
      get
      {
        Membership.Initialize();
        return Membership.s_UserIsOnlineTimeWindow;
      }
    }

    public static MembershipProviderCollection Providers
    {
      get
      {
        Membership.Initialize();
        return Membership.s_Providers;
      }
    }

    public static MembershipProvider Provider
    {
      get
      {
        Membership.Initialize();
        if (Membership.s_Provider == null)
          throw new InvalidOperationException(System.Web.SR.GetString("Def_membership_provider_not_found"));
        else
          return Membership.s_Provider;
      }
    }

    public static string HashAlgorithmType
    {
      get
      {
        Membership.Initialize();
        return Membership.s_HashAlgorithmType;
      }
    }

    internal static bool IsHashAlgorithmFromMembershipConfig
    {
      get
      {
        Membership.Initialize();
        return Membership.s_HashAlgorithmFromConfig;
      }
    }

    public static int MaxInvalidPasswordAttempts
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.MaxInvalidPasswordAttempts;
      }
    }

    public static int PasswordAttemptWindow
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.PasswordAttemptWindow;
      }
    }

    public static int MinRequiredPasswordLength
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.MinRequiredPasswordLength;
      }
    }

    public static int MinRequiredNonAlphanumericCharacters
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.MinRequiredNonAlphanumericCharacters;
      }
    }

    public static string PasswordStrengthRegularExpression
    {
      get
      {
        Membership.Initialize();
        return Membership.Provider.PasswordStrengthRegularExpression;
      }
    }

    public static string ApplicationName
    {
      get
      {
        return Membership.Provider.ApplicationName;
      }
      set
      {
        Membership.Provider.ApplicationName = value;
      }
    }

    public static event MembershipValidatePasswordEventHandler ValidatingPassword
    {
      add
      {
        Membership.Provider.ValidatingPassword += value;
      }
      remove
      {
        Membership.Provider.ValidatingPassword -= value;
      }
    }

    static Membership()
    {
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static MembershipUser CreateUser(string username, string password)
    {
      return Membership.CreateUser(username, password, (string) null);
    }

    public static MembershipUser CreateUser(string username, string password, string email)
    {
      MembershipCreateStatus status;
      MembershipUser user = Membership.CreateUser(username, password, email, (string) null, (string) null, true, out status);
      if (user == null)
        throw new MembershipCreateUserException(status);
      else
        return user;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, out MembershipCreateStatus status)
    {
      return Membership.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, (object) null, out status);
    }

    public static MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      if (!SecUtility.ValidateParameter(ref username, true, true, true, 0))
      {
        status = MembershipCreateStatus.InvalidUserName;
        return (MembershipUser) null;
      }
      else if (!SecUtility.ValidatePasswordParameter(ref password, 0))
      {
        status = MembershipCreateStatus.InvalidPassword;
        return (MembershipUser) null;
      }
      else if (!SecUtility.ValidateParameter(ref email, false, false, false, 0))
      {
        status = MembershipCreateStatus.InvalidEmail;
        return (MembershipUser) null;
      }
      else if (!SecUtility.ValidateParameter(ref passwordQuestion, false, true, false, 0))
      {
        status = MembershipCreateStatus.InvalidQuestion;
        return (MembershipUser) null;
      }
      else
      {
        if (SecUtility.ValidateParameter(ref passwordAnswer, false, true, false, 0))
          return Membership.Provider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        status = MembershipCreateStatus.InvalidAnswer;
        return (MembershipUser) null;
      }
    }

    public static bool ValidateUser(string username, string password)
    {
      return Membership.Provider.ValidateUser(username, password);
    }

    public static MembershipUser GetUser()
    {
      return Membership.GetUser(Membership.GetCurrentUserName(), true);
    }

    public static MembershipUser GetUser(bool userIsOnline)
    {
      return Membership.GetUser(Membership.GetCurrentUserName(), userIsOnline);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static MembershipUser GetUser(string username)
    {
      return Membership.GetUser(username, false);
    }

    public static MembershipUser GetUser(string username, bool userIsOnline)
    {
      SecUtility.CheckParameter(ref username, true, false, true, 0, "username");
      return Membership.Provider.GetUser(username, userIsOnline);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static MembershipUser GetUser(object providerUserKey)
    {
      return Membership.GetUser(providerUserKey, false);
    }

    public static MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      if (providerUserKey == null)
        throw new ArgumentNullException("providerUserKey");
      else
        return Membership.Provider.GetUser(providerUserKey, userIsOnline);
    }

    public static string GetUserNameByEmail(string emailToMatch)
    {
      SecUtility.CheckParameter(ref emailToMatch, false, false, false, 0, "emailToMatch");
      return Membership.Provider.GetUserNameByEmail(emailToMatch);
    }

    public static bool DeleteUser(string username)
    {
      SecUtility.CheckParameter(ref username, true, true, true, 0, "username");
      return Membership.Provider.DeleteUser(username, true);
    }

    public static bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      SecUtility.CheckParameter(ref username, true, true, true, 0, "username");
      return Membership.Provider.DeleteUser(username, deleteAllRelatedData);
    }

    public static void UpdateUser(MembershipUser user)
    {
      if (user == null)
        throw new ArgumentNullException("user");
      user.Update();
    }

    public static MembershipUserCollection GetAllUsers()
    {
      int totalRecords = 0;
      return Membership.GetAllUsers(0, int.MaxValue, out totalRecords);
    }

    public static MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      if (pageIndex < 0)
        throw new ArgumentException(System.Web.SR.GetString("PageIndex_bad"), "pageIndex");
      if (pageSize < 1)
        throw new ArgumentException(System.Web.SR.GetString("PageSize_bad"), "pageSize");
      else
        return Membership.Provider.GetAllUsers(pageIndex, pageSize, out totalRecords);
    }

    public static int GetNumberOfUsersOnline()
    {
      return Membership.Provider.GetNumberOfUsersOnline();
    }

    public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
    {
      if (length < 1 || length > 128)
        throw new ArgumentException(System.Web.SR.GetString("Membership_password_length_incorrect"));
      if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
      {
        throw new ArgumentException(System.Web.SR.GetString("Membership_min_required_non_alphanumeric_characters_incorrect", new object[1]
        {
          (object) "numberOfNonAlphanumericCharacters"
        }));
      }
      else
      {
        string s;
        int matchIndex;
        do
        {
          byte[] data = new byte[length];
          char[] chArray = new char[length];
          int num1 = 0;
          new RNGCryptoServiceProvider().GetBytes(data);
          for (int index = 0; index < length; ++index)
          {
            int num2 = (int) data[index] % 87;
            if (num2 < 10)
              chArray[index] = (char) (48 + num2);
            else if (num2 < 36)
              chArray[index] = (char) (65 + num2 - 10);
            else if (num2 < 62)
            {
              chArray[index] = (char) (97 + num2 - 36);
            }
            else
            {
              chArray[index] = Membership.punctuations[num2 - 62];
              ++num1;
            }
          }
          if (num1 < numberOfNonAlphanumericCharacters)
          {
            Random random = new Random();
            for (int index1 = 0; index1 < numberOfNonAlphanumericCharacters - num1; ++index1)
            {
              int index2;
              do
              {
                index2 = random.Next(0, length);
              }
              while (!char.IsLetterOrDigit(chArray[index2]));
              chArray[index2] = Membership.punctuations[random.Next(0, Membership.punctuations.Length)];
            }
          }
          s = new string(chArray);
        }
        while (CrossSiteScriptingValidation.IsDangerousString(s, out matchIndex));
        return s;
      }
    }

    public static MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 0, "usernameToMatch");
      if (pageIndex < 0)
        throw new ArgumentException(System.Web.SR.GetString("PageIndex_bad"), "pageIndex");
      if (pageSize < 1)
        throw new ArgumentException(System.Web.SR.GetString("PageSize_bad"), "pageSize");
      else
        return Membership.Provider.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
    }

    public static MembershipUserCollection FindUsersByName(string usernameToMatch)
    {
      SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 0, "usernameToMatch");
      int totalRecords = 0;
      return Membership.Provider.FindUsersByName(usernameToMatch, 0, int.MaxValue, out totalRecords);
    }

    public static MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      SecUtility.CheckParameter(ref emailToMatch, false, false, false, 0, "emailToMatch");
      if (pageIndex < 0)
        throw new ArgumentException(System.Web.SR.GetString("PageIndex_bad"), "pageIndex");
      if (pageSize < 1)
        throw new ArgumentException(System.Web.SR.GetString("PageSize_bad"), "pageSize");
      else
        return Membership.Provider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
    }

    public static MembershipUserCollection FindUsersByEmail(string emailToMatch)
    {
      SecUtility.CheckParameter(ref emailToMatch, false, false, false, 0, "emailToMatch");
      int totalRecords = 0;
      return Membership.FindUsersByEmail(emailToMatch, 0, int.MaxValue, out totalRecords);
    }

    private static void Initialize()
    {
      if (Membership.s_Initialized && Membership.s_InitializedDefaultProvider)
        return;
      if (Membership.s_InitializeException != null)
        throw Membership.s_InitializeException;
      if (HostingEnvironment.IsHosted)
        HttpRuntime.CheckAspNetHostingPermission(AspNetHostingPermissionLevel.Low, "Feature_not_supported_at_this_level");
      lock (Membership.s_lock)
      {
        if (Membership.s_Initialized && Membership.s_InitializedDefaultProvider)
          return;
        if (Membership.s_InitializeException != null)
          throw Membership.s_InitializeException;
        bool local_0 = !Membership.s_Initialized;
        bool local_1 = !Membership.s_InitializedDefaultProvider && (!HostingEnvironment.IsHosted || BuildManager.PreStartInitStage == PreStartInitStage.AfterPreStartInit);
        if (!local_1 && !local_0)
          return;
        bool local_2;
        bool local_3_1;
        try
        {
          RuntimeConfig local_4 = RuntimeConfig.GetAppConfig();
          MembershipSection local_5 = local_4.Membership;
          local_2 = Membership.InitializeSettings(local_0, local_4, local_5);
          local_3_1 = Membership.InitializeDefaultProvider(local_1, local_5);
        }
        catch (Exception exception_0)
        {
          Membership.s_InitializeException = exception_0;
          throw;
        }
        if (local_2)
          Membership.s_Initialized = true;
        if (!local_3_1)
          return;
        Membership.s_InitializedDefaultProvider = true;
      }
    }

    private static bool InitializeSettings(bool initializeGeneralSettings, RuntimeConfig appConfig, MembershipSection settings)
    {
      if (!initializeGeneralSettings)
        return false;
      Membership.s_HashAlgorithmType = settings.HashAlgorithmType;
      Membership.s_HashAlgorithmFromConfig = !string.IsNullOrEmpty(Membership.s_HashAlgorithmType);
      if (!Membership.s_HashAlgorithmFromConfig)
      {
        switch (appConfig.MachineKey.Validation)
        {
          case MachineKeyValidation.AES:
          case MachineKeyValidation.TripleDES:
            Membership.s_HashAlgorithmType = "SHA1";
            break;
          default:
            Membership.s_HashAlgorithmType = appConfig.MachineKey.ValidationAlgorithm;
            break;
        }
      }
      Membership.s_Providers = new MembershipProviderCollection();
      if (HostingEnvironment.IsHosted)
      {
        ProvidersHelper.InstantiateProviders(settings.Providers, (ProviderCollection) Membership.s_Providers, typeof (MembershipProvider));
      }
      else
      {
        foreach (ProviderSettings providerSettings in (ConfigurationElementCollection) settings.Providers)
        {
          Type type = Type.GetType(providerSettings.Type, true, true);
          if (!typeof (MembershipProvider).IsAssignableFrom(type))
          {
            throw new ArgumentException(System.Web.SR.GetString("Provider_must_implement_type", new object[1]
            {
              (object) typeof (MembershipProvider).ToString()
            }));
          }
          else
          {
            MembershipProvider membershipProvider = (MembershipProvider) Activator.CreateInstance(type);
            NameValueCollection parameters = providerSettings.Parameters;
            NameValueCollection config = new NameValueCollection(parameters.Count, (IEqualityComparer) StringComparer.Ordinal);
            foreach (string index in (NameObjectCollectionBase) parameters)
              config[index] = parameters[index];
            membershipProvider.Initialize(providerSettings.Name, config);
            Membership.s_Providers.Add((ProviderBase) membershipProvider);
          }
        }
      }
      Membership.s_UserIsOnlineTimeWindow = (int) settings.UserIsOnlineTimeWindow.TotalMinutes;
      return true;
    }

    private static bool InitializeDefaultProvider(bool initializeDefaultProvider, MembershipSection settings)
    {
      if (!initializeDefaultProvider)
        return false;
      Membership.s_Providers.SetReadOnly();
      if (settings.DefaultProvider == null || Membership.s_Providers.Count < 1)
        throw new ProviderException(System.Web.SR.GetString("Def_membership_provider_not_specified"));
      Membership.s_Provider = Membership.s_Providers[settings.DefaultProvider];
      if (Membership.s_Provider == null)
        throw new ConfigurationErrorsException(System.Web.SR.GetString("Def_membership_provider_not_found"), settings.ElementInformation.Properties["defaultProvider"].Source, settings.ElementInformation.Properties["defaultProvider"].LineNumber);
      else
        return true;
    }

    private static string GetCurrentUserName()
    {
      if (HostingEnvironment.IsHosted)
      {
        HttpContext current = HttpContext.Current;
        if (current != null)
          return current.User.Identity.Name;
      }
      IPrincipal currentPrincipal = Thread.CurrentPrincipal;
      if (currentPrincipal == null || currentPrincipal.Identity == null)
        return string.Empty;
      else
        return currentPrincipal.Identity.Name;
    }
  }
}
