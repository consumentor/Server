// Type: System.Attribute
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System
{
  [ClassInterface(ClassInterfaceType.None)]
  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
  [ComDefaultInterface(typeof (_Attribute))]
  [ComVisible(true)]
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
      Dictionary<Type, AttributeUsageAttribute> types = new Dictionary<Type, AttributeUsageAttribute>(11);
      List<Attribute> attributeList = new List<Attribute>();
      Attribute.CopyToArrayList(attributeList, attributes, types);
      for (PropertyInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != (PropertyInfo) null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
      {
        Attribute[] customAttributes = Attribute.GetCustomAttributes((MemberInfo) parentDefinition, type, false);
        Attribute.AddAttributesToList(attributeList, customAttributes, types);
      }
      Array destinationArray = (Array) Attribute.CreateAttributeArrayHelper(type, attributeList.Count);
      Array.Copy((Array) attributeList.ToArray(), 0, destinationArray, 0, attributeList.Count);
      return (Attribute[]) destinationArray;
    }

    private static bool InternalIsDefined(PropertyInfo element, Type attributeType, bool inherit)
    {
      if (element.IsDefined(attributeType, inherit))
        return true;
      if (inherit && Attribute.InternalGetAttributeUsage(attributeType).Inherited)
      {
        for (PropertyInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != (PropertyInfo) null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
        {
          if (parentDefinition.IsDefined(attributeType, false))
            return true;
        }
      }
      return false;
    }

    private static PropertyInfo GetParentDefinition(PropertyInfo property)
    {
      MethodInfo methodInfo = property.GetGetMethod(true);
      if (methodInfo == (MethodInfo) null)
        methodInfo = property.GetSetMethod(true);
      RuntimeMethodInfo runtimeMethodInfo = methodInfo as RuntimeMethodInfo;
      if ((MethodInfo) runtimeMethodInfo != (MethodInfo) null)
      {
        RuntimeMethodInfo parentDefinition = runtimeMethodInfo.GetParentDefinition();
        if ((MethodInfo) parentDefinition != (MethodInfo) null)
          return parentDefinition.DeclaringType.GetProperty(property.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, property.PropertyType);
      }
      return (PropertyInfo) null;
    }

    private static Attribute[] InternalGetCustomAttributes(EventInfo element, Type type, bool inherit)
    {
      Attribute[] attributes = (Attribute[]) element.GetCustomAttributes(type, inherit);
      if (!inherit)
        return attributes;
      Dictionary<Type, AttributeUsageAttribute> types = new Dictionary<Type, AttributeUsageAttribute>(11);
      List<Attribute> attributeList = new List<Attribute>();
      Attribute.CopyToArrayList(attributeList, attributes, types);
      for (EventInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != (EventInfo) null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
      {
        Attribute[] customAttributes = Attribute.GetCustomAttributes((MemberInfo) parentDefinition, type, false);
        Attribute.AddAttributesToList(attributeList, customAttributes, types);
      }
      Array destinationArray = (Array) Attribute.CreateAttributeArrayHelper(type, attributeList.Count);
      Array.Copy((Array) attributeList.ToArray(), 0, destinationArray, 0, attributeList.Count);
      return (Attribute[]) destinationArray;
    }

    private static EventInfo GetParentDefinition(EventInfo ev)
    {
      RuntimeMethodInfo runtimeMethodInfo = ev.GetAddMethod(true) as RuntimeMethodInfo;
      if ((MethodInfo) runtimeMethodInfo != (MethodInfo) null)
      {
        RuntimeMethodInfo parentDefinition = runtimeMethodInfo.GetParentDefinition();
        if ((MethodInfo) parentDefinition != (MethodInfo) null)
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
        for (EventInfo parentDefinition = Attribute.GetParentDefinition(element); parentDefinition != (EventInfo) null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
        {
          if (parentDefinition.IsDefined(attributeType, false))
            return true;
        }
      }
      return false;
    }

    private static ParameterInfo GetParentDefinition(ParameterInfo param)
    {
      RuntimeMethodInfo runtimeMethodInfo = param.Member as RuntimeMethodInfo;
      if ((MethodInfo) runtimeMethodInfo != (MethodInfo) null)
      {
        RuntimeMethodInfo parentDefinition = runtimeMethodInfo.GetParentDefinition();
        if ((MethodInfo) parentDefinition != (MethodInfo) null)
          return parentDefinition.GetParameters()[param.Position];
      }
      return (ParameterInfo) null;
    }

    private static Attribute[] InternalParamGetCustomAttributes(ParameterInfo param, Type type, bool inherit)
    {
      List<Type> list = new List<Type>();
      if (type == (Type) null)
        type = typeof (Attribute);
      object[] customAttributes1 = param.GetCustomAttributes(type, false);
      for (int index = 0; index < customAttributes1.Length; ++index)
      {
        Type type1 = customAttributes1[index].GetType();
        if (!Attribute.InternalGetAttributeUsage(type1).AllowMultiple)
          list.Add(type1);
      }
      Attribute[] attributeArray1 = customAttributes1.Length != 0 ? (Attribute[]) customAttributes1 : Attribute.CreateAttributeArrayHelper(type, 0);
      if (param.Member.DeclaringType == (Type) null || !inherit)
        return attributeArray1;
      for (ParameterInfo parentDefinition = Attribute.GetParentDefinition(param); parentDefinition != null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
      {
        object[] customAttributes2 = parentDefinition.GetCustomAttributes(type, false);
        int elementCount = 0;
        for (int index = 0; index < customAttributes2.Length; ++index)
        {
          Type type1 = customAttributes2[index].GetType();
          AttributeUsageAttribute attributeUsage = Attribute.InternalGetAttributeUsage(type1);
          if (attributeUsage.Inherited && !list.Contains(type1))
          {
            if (!attributeUsage.AllowMultiple)
              list.Add(type1);
            ++elementCount;
          }
          else
            customAttributes2[index] = (object) null;
        }
        Attribute[] attributeArrayHelper = Attribute.CreateAttributeArrayHelper(type, elementCount);
        int index1 = 0;
        for (int index2 = 0; index2 < customAttributes2.Length; ++index2)
        {
          if (customAttributes2[index2] != null)
          {
            attributeArrayHelper[index1] = (Attribute) customAttributes2[index2];
            ++index1;
          }
        }
        Attribute[] attributeArray2 = attributeArray1;
        attributeArray1 = Attribute.CreateAttributeArrayHelper(type, attributeArray2.Length + index1);
        Array.Copy((Array) attributeArray2, (Array) attributeArray1, attributeArray2.Length);
        int length = attributeArray2.Length;
        for (int index2 = 0; index2 < attributeArrayHelper.Length; ++index2)
          attributeArray1[length + index2] = attributeArrayHelper[index2];
      }
      return attributeArray1;
    }

    private static bool InternalParamIsDefined(ParameterInfo param, Type type, bool inherit)
    {
      if (param.IsDefined(type, false))
        return true;
      if (param.Member.DeclaringType == (Type) null || !inherit)
        return false;
      for (ParameterInfo parentDefinition = Attribute.GetParentDefinition(param); parentDefinition != null; parentDefinition = Attribute.GetParentDefinition(parentDefinition))
      {
        object[] customAttributes = parentDefinition.GetCustomAttributes(type, false);
        for (int index = 0; index < customAttributes.Length; ++index)
        {
          AttributeUsageAttribute attributeUsage = Attribute.InternalGetAttributeUsage(customAttributes[index].GetType());
          if (customAttributes[index] is Attribute && attributeUsage.Inherited)
            return true;
        }
      }
      return false;
    }

    private static void CopyToArrayList(List<Attribute> attributeList, Attribute[] attributes, Dictionary<Type, AttributeUsageAttribute> types)
    {
      for (int index = 0; index < attributes.Length; ++index)
      {
        attributeList.Add(attributes[index]);
        Type type = attributes[index].GetType();
        if (!types.ContainsKey(type))
          types[type] = Attribute.InternalGetAttributeUsage(type);
      }
    }

    private static void AddAttributesToList(List<Attribute> attributeList, Attribute[] attributes, Dictionary<Type, AttributeUsageAttribute> types)
    {
      for (int index = 0; index < attributes.Length; ++index)
      {
        Type type = attributes[index].GetType();
        AttributeUsageAttribute attributeUsageAttribute = (AttributeUsageAttribute) null;
        types.TryGetValue(type, out attributeUsageAttribute);
        if (attributeUsageAttribute == null)
        {
          attributeUsageAttribute = Attribute.InternalGetAttributeUsage(type);
          types[type] = attributeUsageAttribute;
          if (attributeUsageAttribute.Inherited)
            attributeList.Add(attributes[index]);
        }
        else if (attributeUsageAttribute.Inherited && attributeUsageAttribute.AllowMultiple)
          attributeList.Add(attributes[index]);
      }
    }

    private static AttributeUsageAttribute InternalGetAttributeUsage(Type type)
    {
      object[] customAttributes = type.GetCustomAttributes(typeof (AttributeUsageAttribute), false);
      if (customAttributes.Length == 1)
        return (AttributeUsageAttribute) customAttributes[0];
      if (customAttributes.Length == 0)
        return AttributeUsageAttribute.Default;
      throw new FormatException(Environment.GetResourceString("Format_AttributeUsage", new object[1]
      {
        (object) type
      }));
    }

    [SecuritySafeCritical]
    private static Attribute[] CreateAttributeArrayHelper(Type elementType, int elementCount)
    {
      return (Attribute[]) Array.UnsafeCreateInstance(elementType, elementCount);
    }

    public static Attribute[] GetCustomAttributes(MemberInfo element, Type type)
    {
      return Attribute.GetCustomAttributes(element, type, true);
    }

    public static Attribute[] GetCustomAttributes(MemberInfo element, Type type, bool inherit)
    {
      if (element == (MemberInfo) null)
        throw new ArgumentNullException("element");
      if (type == (Type) null)
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
      if (element == (MemberInfo) null)
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
      if (element == (MemberInfo) null)
        throw new ArgumentNullException("element");
      if (attributeType == (Type) null)
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

    [SecuritySafeCritical]
    public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (attributeType == (Type) null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      if (element.Member == (MemberInfo) null)
        throw new ArgumentException(Environment.GetResourceString("Argument_InvalidParameterInfo"), "element");
      if (element.Member.MemberType == MemberTypes.Method && inherit)
        return Attribute.InternalParamGetCustomAttributes(element, attributeType, inherit);
      else
        return element.GetCustomAttributes(attributeType, inherit) as Attribute[];
    }

    [SecuritySafeCritical]
    public static Attribute[] GetCustomAttributes(ParameterInfo element, bool inherit)
    {
      if (element == null)
        throw new ArgumentNullException("element");
      if (element.Member == (MemberInfo) null)
        throw new ArgumentException(Environment.GetResourceString("Argument_InvalidParameterInfo"), "element");
      if (element.Member.MemberType == MemberTypes.Method && inherit)
        return Attribute.InternalParamGetCustomAttributes(element, (Type) null, inherit);
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
      if (attributeType == (Type) null)
        throw new ArgumentNullException("attributeType");
      if (!attributeType.IsSubclassOf(typeof (Attribute)) && attributeType != typeof (Attribute))
        throw new ArgumentException(Environment.GetResourceString("Argument_MustHaveAttributeBaseClass"));
      switch (element.Member.MemberType)
      {
        case MemberTypes.Constructor:
          return element.IsDefined(attributeType, false);
        case MemberTypes.Method:
          return Attribute.InternalParamIsDefined(element, attributeType, inherit);
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
      if (element == (Module) null)
        throw new ArgumentNullException("element");
      else
        return (Attribute[]) element.GetCustomAttributes(typeof (Attribute), inherit);
    }

    public static Attribute[] GetCustomAttributes(Module element, Type attributeType, bool inherit)
    {
      if (element == (Module) null)
        throw new ArgumentNullException("element");
      if (attributeType == (Type) null)
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

    [SecuritySafeCritical]
    public static bool IsDefined(Module element, Type attributeType, bool inherit)
    {
      if (element == (Module) null)
        throw new ArgumentNullException("element");
      if (attributeType == (Type) null)
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
      if (element == (Assembly) null)
        throw new ArgumentNullException("element");
      if (attributeType == (Type) null)
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
      if (element == (Assembly) null)
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
      if (element == (Assembly) null)
        throw new ArgumentNullException("element");
      if (attributeType == (Type) null)
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

    [SecuritySafeCritical]
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
        if (!Attribute.AreFieldValuesEqual(((RtFieldInfo) fields[index]).InternalGetValue(obj1, false, false), ((RtFieldInfo) fields[index]).InternalGetValue(obj, false, false)))
          return false;
      }
      return true;
    }

    private static bool AreFieldValuesEqual(object thisValue, object thatValue)
    {
      if (thisValue == null && thatValue == null)
        return true;
      if (thisValue == null || thatValue == null)
        return false;
      if (thisValue.GetType().IsArray)
      {
        if (!thisValue.GetType().Equals(thatValue.GetType()))
          return false;
        Array array1 = thisValue as Array;
        Array array2 = thatValue as Array;
        if (array1.Length != array2.Length)
          return false;
        for (int index = 0; index < array1.Length; ++index)
        {
          if (!Attribute.AreFieldValuesEqual(array1.GetValue(index), array2.GetValue(index)))
            return false;
        }
      }
      else if (!thisValue.Equals(thatValue))
        return false;
      return true;
    }

    [SecuritySafeCritical]
    public override int GetHashCode()
    {
      Type type = this.GetType();
      FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      object obj1 = (object) null;
      for (int index = 0; index < fields.Length; ++index)
      {
        object obj2 = ((RtFieldInfo) fields[index]).InternalGetValue((object) this, false, false);
        if (obj2 != null && !obj2.GetType().IsArray)
          obj1 = obj2;
        if (obj1 != null)
          break;
      }
      if (obj1 != null)
        return obj1.GetHashCode();
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
