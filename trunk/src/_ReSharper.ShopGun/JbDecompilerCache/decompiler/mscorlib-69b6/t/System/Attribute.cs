// Type: System.Attribute
// Assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll

using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.None)]
  [ComDefaultInterface(typeof (_Attribute))]
  [Serializable]
  public abstract class Attribute : _Attribute
  {
    public virtual object TypeId
    {
      get
      {
        return (object) this.GetType();
      }
    }

    private static Attribute[] InternalGetCustomAttributes(PropertyInfo element, Type type, bool inherit)
    {
      Attribute[] attributes = (Attribute[]) element.GetCustomAttributes(type, inherit);
      if (!inherit)
        return attributes;
      Hashtable types = new Hashtable(11);
      ArrayList attributeList = new ArrayList();
      Attribute.CopyToArrayList(attributeList, attributes, types);
      for (PropertyInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
      {
        Attribute[] customAttributes = Attribute.GetCustomAttributes((MemberInfo) parentDefinition, type, false);
        Attribute.AddAttributesToList(attributeList, customAttributes, types);
      }
      return (Attribute[]) attributeList.ToArray(type);
    }

    private static bool InternalIsDefined(PropertyInfo element, Type attributeType, bool inherit)
    {
      if (element.IsDefined(attributeType, inherit))
        return true;
      if (inherit && Attribute.InternalGetAttributeUsage(attributeType).Inherited)
      {
        for (PropertyInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
        {
          if (parentDefinition.IsDefined(attributeType, false))
            return true;
        }
      }
      return false;
    }

    private static PropertyInfo GetParentDefinition(PropertyInfo property)
    {
      MethodInfo methodInfo = property.GetGetMethod(true) ?? property.GetSetMethod(true);
      if (methodInfo != null)
      {
        MethodInfo parentDefinition = methodInfo.GetParentDefinition();
        if (parentDefinition != null)
          return parentDefinition.DeclaringType.GetProperty(property.Name, property.PropertyType);
      }
      return (PropertyInfo) null;
    }

    private static Attribute[] InternalGetCustomAttributes(EventInfo element, Type type, bool inherit)
    {
      Attribute[] attributes = (Attribute[]) element.GetCustomAttributes(type, inherit);
      if (!inherit)
        return attributes;
      Hashtable types = new Hashtable(11);
      ArrayList attributeList = new ArrayList();
      Attribute.CopyToArrayList(attributeList, attributes, types);
      for (EventInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
      {
        Attribute[] customAttributes = Attribute.GetCustomAttributes((MemberInfo) parentDefinition, type, false);
        Attribute.AddAttributesToList(attributeList, customAttributes, types);
      }
      return (Attribute[]) attributeList.ToArray(type);
    }

    private static EventInfo GetParentDefinition(EventInfo ev)
    {
      MethodInfo addMethod = ev.GetAddMethod(true);
      if (addMethod != null)
      {
        MethodInfo parentDefinition = addMethod.GetParentDefinition();
        if (parentDefinition != null)
          return parentDefinition.DeclaringType.GetEvent(ev.Name);
      }
      return (EventInfo) null;
    }

    private static bool InternalIsDefined(EventInfo element, Type attributeType, bool inherit)
    {
      if (element.IsDefined(attributeType, inherit))
        return true;
      if (inherit && Attribute.InternalGetAttributeUsage(attributeType).Inherited)
      {
        for (EventInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
        {
          if (parentDefinition.IsDefined(attributeType, false))
            return true;
        }
      }
      return false;
    }

    private static Attribute[] InternalParamGetCustomAttributes(MethodInfo method, ParameterInfo param, Type type, bool inherit)
    {
      ArrayList arrayList = new ArrayList();
      if (type == null)
        type = typeof (Attribute);
      object[] customAttributes1 = param.GetCustomAttributes(type, false);
      for (int index = 0; index < customAttributes1.Length; ++index)
      {
        Type type1 = customAttributes1[index].GetType();
        if (!Attribute.InternalGetAttributeUsage(type1).AllowMultiple)
          arrayList.Add((object) type1);
      }
      Attribute[] attributeArray1 = customAttributes1.Length != 0 ? (Attribute[]) customAttributes1 : (Attribute[]) Array.CreateInstance(type, 0);
      if (method.DeclaringType == null || !inherit)
        return attributeArray1;
      int position = param.Position;
      for (method = method.GetParentDefinition(); method != null; method = method.GetParentDefinition())
      {
        param = method.GetParameters()[position];
        object[] customAttributes2 = param.GetCustomAttributes(type, false);
        int length1 = 0;
        for (int index = 0; index < customAttributes2.Length; ++index)
        {
          Type type1 = customAttributes2[index].GetType();
          AttributeUsageAttribute attributeUsage = Attribute.InternalGetAttributeUsage(type1);
          if (attributeUsage.Inherited && !arrayList.Contains((object) type1))
          {
            if (!attributeUsage.AllowMultiple)
              arrayList.Add((object) type1);
            ++length1;
          }
          else
            customAttributes2[index] = (object) null;
        }
        Attribute[] attributeArray2 = (Attribute[]) Array.CreateInstance(type, length1);
        int index1 = 0;
        for (int index2 = 0; index2 < customAttributes2.Length; ++index2)
        {
          if (customAttributes2[index2] != null)
          {
            attributeArray2[index1] = (Attribute) customAttributes2[index2];
            ++index1;
          }
        }
        Attribute[] attributeArray3 = attributeArray1;
        attributeArray1 = (Attribute[]) Array.CreateInstance(type, attributeArray3.Length + index1);
        Array.Copy((Array) attributeArray3, (Array) attributeArray1, attributeArray3.Length);
        int length2 = attributeArray3.Length;
        for (int index2 = 0; index2 < attributeArray2.Length; ++index2)
          attributeArray1[length2 + index2] = attributeArray2[index2];
      }
      return attributeArray1;
    }

    private static bool InternalParamIsDefined(MethodInfo method, ParameterInfo param, Type type, bool inherit)
    {
      if (param.IsDefined(type, false))
        return true;
      if (method.DeclaringType == null || !inherit)
        return false;
      int position = param.Position;
      for (method = method.GetParentDefinition(); method != null; method = method.GetParentDefinition())
      {
        param = method.GetParameters()[position];
        object[] customAttributes = param.GetCustomAttributes(type, false);
        for (int index = 0; index < customAttributes.Length; ++index)
        {
          AttributeUsageAttribute attributeUsage = Attribute.InternalGetAttributeUsage(customAttributes[index].GetType());
          if (customAttributes[index] is Attribute && attributeUsage.Inherited)
            return true;
        }
      }
      return false;
    }

    private static void CopyToArrayList(ArrayList attributeList, Attribute[] attributes, Hashtable types)
    {
      for (int index = 0; index < attributes.Length; ++index)
      {
        attributeList.Add((object) attributes[index]);
        Type type = attributes[index].GetType();
        if (!types.Contains((object) type))
          types[(object) type] = (object) Attribute.InternalGetAttributeUsage(type);
      }
    }

    private static void AddAttributesToList(ArrayList attributeList, Attribute[] attributes, Hashtable types)
    {
      for (int index = 0; index < attributes.Length; ++index)
      {
        Type type = attributes[index].GetType();
        AttributeUsageAttribute attributeUsageAttribute = (AttributeUsageAttribute) types[(object) type];
        if (attributeUsageAttribute == null)
        {
          AttributeUsageAttribute attributeUsage = Attribute.InternalGetAttributeUsage(type);
          types[(object) type] = (object) attributeUsage;
          if (attributeUsage.Inherited)
            attributeList.Add((object) attributes[index]);
        }
        else if (attributeUsageAttribute.Inherited && attributeUsageAttribute.AllowMultiple)
          attributeList.Add((object) attributes[index]);
      }
    }

    private static AttributeUsageAttribute InternalGetAttributeUsage(Type type)
    {
      object[] customAttributes = type.GetCustomAttributes(typeof (AttributeUsageAttribute), false);
      if (customAttributes.Length == 1)
        return (AttributeUsageAttribute) customAttributes[0];
      if (customAttributes.Length == 0)
        return AttributeUsageAttribute.Default;
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Environment.GetResourceString("Format_AttributeUsage"), new object[1]
      {
        (object) type
      }));
    }

    public static Attribute[] GetCustomAttributes(MemberInfo element, Type type)
    {
      return Attribute.GetCustomAttributes(element, type, true);
    }

    public static Attribute[] GetCustomAttributes(MemberInfo element, Type type, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (type == null)
        throw new ArgumentNullException("type");
      if (!type.IsSubclassOf(typeof (Attribute)) && type != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      switch (element.MemberType)
      {
        case MemberTypes.Event:
          return Attribute.InternalGetCustomAttributes((EventInfo) element, type, inherit);
        case MemberTypes.Property:
          return Attribute.InternalGetCustomAttributes((PropertyInfo) element, type, inherit);
        default:
          return element.GetCustomAttributes(type, inherit) as Attribute[];
      }
    }

    public static Attribute[] GetCustomAttributes(MemberInfo element)
    {
      return Attribute.GetCustomAttributes(element, true);
    }

    public static Attribute[] GetCustomAttributes(MemberInfo element, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      switch (element.MemberType)
      {
        case MemberTypes.Event:
          return Attribute.InternalGetCustomAttributes((EventInfo) element, typeof (Attribute), inherit);
        case MemberTypes.Property:
          return Attribute.InternalGetCustomAttributes((PropertyInfo) element, typeof (Attribute), inherit);
        default:
          return element.GetCustomAttributes(typeof (Attribute), inherit) as Attribute[];
      }
    }

    public static bool IsDefined(MemberInfo element, Type attributeType)
    {
      return Attribute.IsDefined(element, attributeType, true);
    }

    public static bool IsDefined(MemberInfo element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      switch (element.MemberType)
      {
        case MemberTypes.Event:
          return Attribute.InternalIsDefined((EventInfo) element, attributeType, inherit);
        case MemberTypes.Property:
          return Attribute.InternalIsDefined((PropertyInfo) element, attributeType, inherit);
        default:
          return element.IsDefined(attributeType, inherit);
      }
    }

    public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType)
    {
      return Attribute.GetCustomAttribute(element, attributeType, true);
    }

    public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType, bool inherit)
    {
      Attribute[] customAttributes = Attribute.GetCustomAttributes(element, attributeType, inherit);
      if (customAttributes == null || customAttributes.Length == 0)
        return (Attribute) null;
      if (customAttributes.Length == 1)
        return customAttributes[0];
      else
        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
    }

    public static Attribute[] GetCustomAttributes(ParameterInfo element)
    {
      return Attribute.GetCustomAttributes(element, true);
    }

    public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType)
    {
      return Attribute.GetCustomAttributes(element, attributeType, true);
    }

    public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      MemberInfo member = element.Member;
      if (member.MemberType == MemberTypes.Method && inherit)
        return Attribute.InternalParamGetCustomAttributes((MethodInfo) member, element, attributeType, inherit);
      else
        return element.GetCustomAttributes(attributeType, inherit) as Attribute[];
    }

    public static Attribute[] GetCustomAttributes(ParameterInfo element, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      MemberInfo member = element.Member;
      if (member.MemberType == MemberTypes.Method && inherit)
        return Attribute.InternalParamGetCustomAttributes((MethodInfo) member, element, (Type) null, inherit);
      else
        return element.GetCustomAttributes(typeof (Attribute), inherit) as Attribute[];
    }

    public static bool IsDefined(ParameterInfo element, Type attributeType)
    {
      return Attribute.IsDefined(element, attributeType, true);
    }

    public static bool IsDefined(ParameterInfo element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      MemberInfo member = element.Member;
      switch (member.MemberType)
      {
        case MemberTypes.Constructor:
          return element.IsDefined(attributeType, false);
        case MemberTypes.Method:
          return Attribute.InternalParamIsDefined((MethodInfo) member, element, attributeType, inherit);
        case MemberTypes.Property:
          return element.IsDefined(attributeType, false);
        default:
          throw new ArgumentException(Environment.GetResourceString("Argument_InvalidParamInfo"));
      }
    }

    public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType)
    {
      return Attribute.GetCustomAttribute(element, attributeType, true);
    }

    public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType, bool inherit)
    {
      Attribute[] customAttributes = Attribute.GetCustomAttributes(element, attributeType, inherit);
      if (customAttributes == null || customAttributes.Length == 0)
        return (Attribute) null;
      if (customAttributes.Length == 0)
        return (Attribute) null;
      if (customAttributes.Length == 1)
        return customAttributes[0];
      else
        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
    }

    public static Attribute[] GetCustomAttributes(Module element, Type attributeType)
    {
      return Attribute.GetCustomAttributes(element, attributeType, true);
    }

    public static Attribute[] GetCustomAttributes(Module element)
    {
      return Attribute.GetCustomAttributes(element, true);
    }

    public static Attribute[] GetCustomAttributes(Module element, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      else
        return (Attribute[]) element.GetCustomAttributes(typeof (Attribute), inherit);
    }

    public static Attribute[] GetCustomAttributes(Module element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      else
        return (Attribute[]) element.GetCustomAttributes(attributeType, inherit);
    }

    public static bool IsDefined(Module element, Type attributeType)
    {
      return Attribute.IsDefined(element, attributeType, false);
    }

    public static bool IsDefined(Module element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      else
        return element.IsDefined(attributeType, false);
    }

    public static Attribute GetCustomAttribute(Module element, Type attributeType)
    {
      return Attribute.GetCustomAttribute(element, attributeType, true);
    }

    public static Attribute GetCustomAttribute(Module element, Type attributeType, bool inherit)
    {
      Attribute[] customAttributes = Attribute.GetCustomAttributes(element, attributeType, inherit);
      if (customAttributes == null || customAttributes.Length == 0)
        return (Attribute) null;
      if (customAttributes.Length == 1)
        return customAttributes[0];
      else
        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
    }

    public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType)
    {
      return Attribute.GetCustomAttributes(element, attributeType, true);
    }

    public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      else
        return (Attribute[]) element.GetCustomAttributes(attributeType, inherit);
    }

    public static Attribute[] GetCustomAttributes(Assembly element)
    {
      return Attribute.GetCustomAttributes(element, true);
    }

    public static Attribute[] GetCustomAttributes(Assembly element, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      else
        return (Attribute[]) element.GetCustomAttributes(typeof (Attribute), inherit);
    }

    public static bool IsDefined(Assembly element, Type attributeType)
    {
      return Attribute.IsDefined(element, attributeType, true);
    }

    public static bool IsDefined(Assembly element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      else
        return element.IsDefined(attributeType, false);
    }

    public static Attribute GetCustomAttribute(Assembly element, Type attributeType)
    {
      return Attribute.GetCustomAttribute(element, attributeType, true);
    }

    public static Attribute GetCustomAttribute(Assembly element, Type attributeType, bool inherit)
    {
      Attribute[] customAttributes = Attribute.GetCustomAttributes(element, attributeType, inherit);
      if (customAttributes == null || customAttributes.Length == 0)
        return (Attribute) null;
      if (customAttributes.Length == 1)
        return customAttributes[0];
      else
        throw new AmbiguousMatchException(Environment.GetResourceString("RFLCT.AmbigCust"));
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      RuntimeType runtimeType = (RuntimeType) this.GetType();
      if ((RuntimeType) obj.GetType() != runtimeType)
        return false;
      object obj1 = (object) this;
      FieldInfo[] fields = runtimeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      for (int index = 0; index < fields.Length; ++index)
      {
        object obj2 = fields[index].GetValue(obj1);
        object obj3 = fields[index].GetValue(obj);
        if (obj2 == null)
        {
          if (obj3 != null)
            return false;
        }
        else if (!obj2.Equals(obj3))
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      Type type = this.GetType();
      FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      object obj = (object) null;
      for (int index = 0; index < fields.Length; ++index)
      {
        obj = fields[index].GetValue((object) this);
        if (obj != null)
          break;
      }
      if (obj != null)
        return obj.GetHashCode();
      else
        return type.GetHashCode();
    }

    public virtual bool Match(object obj)
    {
      return this.Equals(obj);
    }

    public virtual bool IsDefaultAttribute()
    {
      return false;
    }

    void _Attribute.GetTypeInfoCount(out uint pcTInfo)
    {
      throw new NotImplementedException();
    }

    void _Attribute.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
    {
      throw new NotImplementedException();
    }

    void _Attribute.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
    {
      throw new NotImplementedException();
    }

    void _Attribute.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
    {
      throw new NotImplementedException();
    }
  }
}
