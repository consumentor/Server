namespace System.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicTypeGeneratorTest {

        [TestMethod]
        public void GenerateType() {
            // Arrange
            Type baseType = typeof(MyBaseType);
            Type[] interfaceTypes = new Type[] { typeof(SomeInterfaceA), typeof(SomeInterfaceB) };

            // Act
            Type newType = DynamicTypeGenerator.GenerateType("SomeTypeName", baseType, interfaceTypes);

            // Assert
            Assert.AreEqual("System.Web.Mvc.{Dynamic}.SomeTypeName", newType.FullName, "New type name is incorrect.");
            Assert.IsTrue(newType.IsPublic, "New type should have public visibility.");
            Assert.IsTrue(newType.IsSubclassOf(baseType), "New type does not have proper base class.");
            Assert.IsTrue(typeof(SomeInterfaceA).IsAssignableFrom(newType), "New type does not implement SomeInterfaceA.");
            Assert.IsTrue(typeof(SomeInterfaceB).IsAssignableFrom(newType), "New type does not implement SomeInterfaceB.");

            ConstructorInfo[] constructors = newType.GetConstructors();
            Assert.AreEqual(3, constructors.Length, "Incorrect number of public constructors.");

            Assert.AreEqual(
                baseType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Length,
                newType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Length,
                "Number of public instance methods should not have increased.");

            MyBaseType o1 = (MyBaseType)Activator.CreateInstance(newType, new object[] { 1, 10, 100, 1000 });
            Assert.AreEqual(1, o1.ParamA);
            Assert.AreEqual(10, o1.ParamB);
            Assert.AreEqual(100, o1.ParamC);
            Assert.AreEqual("public", o1.ConstructorCalled);

            MyBaseType o2 = (MyBaseType)Activator.CreateInstance(newType, new object[] { 0, 0, 0, (short)0 });
            Assert.AreEqual("protected", o2.ConstructorCalled);

            MyBaseType o3 = (MyBaseType)Activator.CreateInstance(newType, new object[] { 0, 0, 0, (long)0 });
            Assert.AreEqual("protected internal", o3.ConstructorCalled);

            SomeInterfaceA oA = (SomeInterfaceA)o1;
            Assert.AreEqual("Hello from sealed A!", oA.SomeMethodA(), "SomeInterfaceA was not implemented correctly.");

            SomeInterfaceB oB = (SomeInterfaceB)o1;
            Assert.AreEqual("Hello from virtual B: 42!", oB.SomeMethodB(42), "SomeInterfaceB was not implemented correctly.");
        }

        public class MyBaseType {
            public readonly int ParamA;
            public readonly int ParamB;
            public readonly int ParamC;
            public readonly string ConstructorCalled;

            public MyBaseType(int paramA, int paramB, int paramC, int dummy) : this(paramA, paramB, paramC, "public") { }
            protected MyBaseType(int paramA, int paramB, int paramC, short dummy) : this(paramA, paramB, paramC, "protected") { }
            protected internal MyBaseType(int paramA, int paramB, int paramC, long dummy) : this(paramA, paramB, paramC, "protected internal") { }

            private MyBaseType(int paramA, int paramB, int paramC, string constructorCalled) {
                ParamA = paramA;
                ParamB = paramB;
                ParamC = paramC;
                ConstructorCalled = constructorCalled;
            }

            public string SomeMethodA() {
                return "Hello from sealed A!";
            }

            public virtual string SomeMethodB(int value) {
                return String.Format(CultureInfo.InvariantCulture, "Hello from virtual B: {0}!", value);
            }
        }

        public interface SomeInterfaceA {
            string SomeMethodA();
        }

        public interface SomeInterfaceB {
            string SomeMethodB(int value);
        }

    }
}
