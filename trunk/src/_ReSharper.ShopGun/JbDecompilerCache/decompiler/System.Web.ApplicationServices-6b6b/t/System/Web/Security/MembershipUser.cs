// Type: System.Web.Security.MembershipUser
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.ApplicationServices.dll

using System;
using System.Configuration.Provider;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Util;

namespace System.Web.Security
{
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class MembershipUser
  {
    private string _UserName;
    private object _ProviderUserKey;
    private string _Email;
    private string _PasswordQuestion;
    private string _Comment;
    private bool _IsApproved;
    private bool _IsLockedOut;
    private DateTime _LastLockoutDate;
    private DateTime _CreationDate;
    private DateTime _LastLoginDate;
    private DateTime _LastActivityDate;
    private DateTime _LastPasswordChangedDate;
    private string _ProviderName;

    public virtual string UserName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._UserName;
      }
    }

    public virtual object ProviderUserKey
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._ProviderUserKey;
      }
    }

    public virtual string Email
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._Email;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this._Email = value;
      }
    }

    public virtual string PasswordQuestion
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._PasswordQuestion;
      }
    }

    public virtual string Comment
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._Comment;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this._Comment = value;
      }
    }

    public virtual bool IsApproved
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._IsApproved;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this._IsApproved = value;
      }
    }

    public virtual bool IsLockedOut
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._IsLockedOut;
      }
    }

    public virtual DateTime LastLockoutDate
    {
      get
      {
        return this._LastLockoutDate.ToLocalTime();
      }
    }

    public virtual DateTime CreationDate
    {
      get
      {
        return this._CreationDate.ToLocalTime();
      }
    }

    public virtual DateTime LastLoginDate
    {
      get
      {
        return this._LastLoginDate.ToLocalTime();
      }
      set
      {
        this._LastLoginDate = value.ToUniversalTime();
      }
    }

    public virtual DateTime LastActivityDate
    {
      get
      {
        return this._LastActivityDate.ToLocalTime();
      }
      set
      {
        this._LastActivityDate = value.ToUniversalTime();
      }
    }

    public virtual DateTime LastPasswordChangedDate
    {
      get
      {
        return this._LastPasswordChangedDate.ToLocalTime();
      }
    }

    public virtual bool IsOnline
    {
      get
      {
        return this.LastActivityDate.ToUniversalTime() > DateTime.UtcNow.Subtract(new TimeSpan(0, SystemWebProxy.Membership.UserIsOnlineTimeWindow, 0));
      }
    }

    public virtual string ProviderName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._ProviderName;
      }
    }

    public MembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
    {
      if (providerName == null || SystemWebProxy.Membership.Providers[providerName] == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ApplicationServicesStrings.Membership_provider_name_invalid, new object[0]), "providerName");
      if (name != null)
        name = name.Trim();
      if (email != null)
        email = email.Trim();
      if (passwordQuestion != null)
        passwordQuestion = passwordQuestion.Trim();
      this._ProviderName = providerName;
      this._UserName = name;
      this._ProviderUserKey = providerUserKey;
      this._Email = email;
      this._PasswordQuestion = passwordQuestion;
      this._Comment = comment;
      this._IsApproved = isApproved;
      this._IsLockedOut = isLockedOut;
      this._CreationDate = creationDate.ToUniversalTime();
      this._LastLoginDate = lastLoginDate.ToUniversalTime();
      this._LastActivityDate = lastActivityDate.ToUniversalTime();
      this._LastPasswordChangedDate = lastPasswordChangedDate.ToUniversalTime();
      this._LastLockoutDate = lastLockoutDate.ToUniversalTime();
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    protected MembershipUser()
    {
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public override string ToString()
    {
      return this.UserName;
    }

    internal virtual void Update()
    {
      SystemWebProxy.Membership.Providers[this.ProviderName].UpdateUser(this);
      this.UpdateSelf();
    }

    public virtual string GetPassword()
    {
      return SystemWebProxy.Membership.Providers[this.ProviderName].GetPassword(this.UserName, (string) null);
    }

    public virtual string GetPassword(string passwordAnswer)
    {
      return SystemWebProxy.Membership.Providers[this.ProviderName].GetPassword(this.UserName, passwordAnswer);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal string GetPassword(bool throwOnError)
    {
      return this.GetPassword((string) null, false, throwOnError);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal string GetPassword(string answer, bool throwOnError)
    {
      return this.GetPassword(answer, true, throwOnError);
    }

    private string GetPassword(string answer, bool useAnswer, bool throwOnError)
    {
      string str = (string) null;
      try
      {
        str = !useAnswer ? this.GetPassword() : this.GetPassword(answer);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (MembershipPasswordException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (ProviderException ex)
      {
        if (throwOnError)
          throw;
      }
      return str;
    }

    public virtual bool ChangePassword(string oldPassword, string newPassword)
    {
      SecurityServices.CheckPasswordParameter(oldPassword, "oldPassword");
      SecurityServices.CheckPasswordParameter(newPassword, "newPassword");
      if (!SystemWebProxy.Membership.Providers[this.ProviderName].ChangePassword(this.UserName, oldPassword, newPassword))
        return false;
      this.UpdateSelf();
      return true;
    }

    internal bool ChangePassword(string oldPassword, string newPassword, bool throwOnError)
    {
      bool flag = false;
      try
      {
        flag = this.ChangePassword(oldPassword, newPassword);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (MembershipPasswordException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (ProviderException ex)
      {
        if (throwOnError)
          throw;
      }
      return flag;
    }

    public virtual bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      SecurityServices.CheckPasswordParameter(password, "password");
      SecurityServices.CheckForEmptyOrWhiteSpaceParameter(ref newPasswordQuestion, "newPasswordQuestion");
      SecurityServices.CheckForEmptyOrWhiteSpaceParameter(ref newPasswordAnswer, "newPasswordAnswer");
      if (!SystemWebProxy.Membership.Providers[this.ProviderName].ChangePasswordQuestionAndAnswer(this.UserName, password, newPasswordQuestion, newPasswordAnswer))
        return false;
      this.UpdateSelf();
      return true;
    }

    public virtual string ResetPassword(string passwordAnswer)
    {
      string str = SystemWebProxy.Membership.Providers[this.ProviderName].ResetPassword(this.UserName, passwordAnswer);
      if (!string.IsNullOrEmpty(str))
        this.UpdateSelf();
      return str;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public virtual string ResetPassword()
    {
      return this.ResetPassword((string) null);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal string ResetPassword(bool throwOnError)
    {
      return this.ResetPassword((string) null, false, throwOnError);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal string ResetPassword(string passwordAnswer, bool throwOnError)
    {
      return this.ResetPassword(passwordAnswer, true, throwOnError);
    }

    private string ResetPassword(string passwordAnswer, bool useAnswer, bool throwOnError)
    {
      string str = (string) null;
      try
      {
        str = !useAnswer ? this.ResetPassword() : this.ResetPassword(passwordAnswer);
      }
      catch (ArgumentException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (MembershipPasswordException ex)
      {
        if (throwOnError)
          throw;
      }
      catch (ProviderException ex)
      {
        if (throwOnError)
          throw;
      }
      return str;
    }

    public virtual bool UnlockUser()
    {
      if (!SystemWebProxy.Membership.Providers[this.ProviderName].UnlockUser(this.UserName))
        return false;
      this.UpdateSelf();
      return !this.IsLockedOut;
    }

    private void UpdateSelf()
    {
      MembershipUser user = SystemWebProxy.Membership.Providers[this.ProviderName].GetUser(this.UserName, false);
      if (user == null)
        return;
      try
      {
        this._LastPasswordChangedDate = user.LastPasswordChangedDate.ToUniversalTime();
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.LastActivityDate = user.LastActivityDate;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.LastLoginDate = user.LastLoginDate;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._CreationDate = user.CreationDate.ToUniversalTime();
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._LastLockoutDate = user.LastLockoutDate.ToUniversalTime();
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._IsLockedOut = user.IsLockedOut;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.IsApproved = user.IsApproved;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.Comment = user.Comment;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._PasswordQuestion = user.PasswordQuestion;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this.Email = user.Email;
      }
      catch (NotSupportedException ex)
      {
      }
      try
      {
        this._ProviderUserKey = user.ProviderUserKey;
      }
      catch (NotSupportedException ex)
      {
      }
    }
  }
}
