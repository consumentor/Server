namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AsyncActionMethodSelectorTest {

        [TestMethod]
        public void AliasedMethodsProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);

            // Act
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Assert
            Assert.AreEqual(3, selector.AliasedMethods.Length);

            List<MethodInfo> sortedAliasedMethods = selector.AliasedMethods.OrderBy(methodInfo => methodInfo.Name).ToList();
            Assert.AreEqual("Bar", sortedAliasedMethods[0].Name);
            Assert.AreEqual("FooRenamed", sortedAliasedMethods[1].Name);
            Assert.AreEqual("Renamed", sortedAliasedMethods[2].Name);
        }

        [TestMethod]
        public void ControllerTypeProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & Assert
            Assert.AreSame(controllerType, selector.ControllerType);
        }

        [TestMethod]
        public void FindAction_DoesNotMatchAsyncMethod() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindAction(null, "EventPatternAsync");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindAction_DoesNotMatchCompletedMethod() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindAction(null, "EventPatternCompleted");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindAction_ReturnsMatchingMethodIfOneMethodMatches() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindAction(null, "OneMatch");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedActionDescriptor));
            ReflectedActionDescriptor castActionDescriptor = (ReflectedActionDescriptor)actionDescriptor;

            Assert.AreEqual("OneMatch", castActionDescriptor.MethodInfo.Name, "Method named OneMatch() should have matched.");
            Assert.AreEqual(typeof(string), castActionDescriptor.MethodInfo.GetParameters()[0].ParameterType, "Method overload OneMatch(string) should have matched.");
        }

        [TestMethod]
        public void FindAction_ReturnsMethodWithActionSelectionAttributeIfMultipleMethodsMatchRequest() {
            // DevDiv Bugs 212062: If multiple action methods match a request, we should match only the methods with an
            // [ActionMethod] attribute since we assume those methods are more specific.

            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindAction(null, "ShouldMatchMethodWithSelectionAttribute");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedActionDescriptor));
            ReflectedActionDescriptor castActionDescriptor = (ReflectedActionDescriptor)actionDescriptor;

            Assert.AreEqual("MethodHasSelectionAttribute1", castActionDescriptor.MethodInfo.Name);
        }

        [TestMethod]
        public void FindAction_ReturnsNullIfNoMethodMatches() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindAction(null, "ZeroMatch");

            // Assert
            Assert.IsNull(creator, "No method should have matched.");
        }

        [TestMethod]
        public void FindAction_ThrowsIfMultipleMethodsMatch() {
            // Arrange
            Type controllerType = typeof(SelectionAttributeController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & veriy
            ExceptionHelper.ExpectException<AmbiguousMatchException>(
                delegate {
                    selector.FindAction(null, "TwoMatch");
                },
                @"The current request for action 'TwoMatch' on controller type 'SelectionAttributeController' is ambiguous between the following action methods:
Void TwoMatch2() on type System.Web.Mvc.Async.Test.AsyncActionMethodSelectorTest+SelectionAttributeController
Void TwoMatch() on type System.Web.Mvc.Async.Test.AsyncActionMethodSelectorTest+SelectionAttributeController");
        }

        [TestMethod]
        public void FindActionMethod_Asynchronous() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act
            ActionDescriptorCreator creator = selector.FindAction(null, "EventPattern");
            ActionDescriptor actionDescriptor = creator("someName", new Mock<ControllerDescriptor>().Object);

            // Assert
            Assert.IsInstanceOfType(actionDescriptor, typeof(ReflectedAsyncActionDescriptor));
            ReflectedAsyncActionDescriptor castActionDescriptor = (ReflectedAsyncActionDescriptor)actionDescriptor;

            Assert.AreEqual("EventPatternAsync", castActionDescriptor.AsyncMethodInfo.Name);
            Assert.AreEqual("EventPatternCompleted", castActionDescriptor.CompletedMethodInfo.Name);
        }

        [TestMethod]
        public void FindActionMethod_Asynchronous_ThrowsIfCompletionMethodNotFound() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ActionDescriptorCreator creator = selector.FindAction(null, "EventPatternWithoutCompletionMethod");
                },
                @"Could not locate a method named 'EventPatternWithoutCompletionMethodCompleted' on controller type System.Web.Mvc.Async.Test.AsyncActionMethodSelectorTest+MethodLocatorController.");
        }

        [TestMethod]
        public void FindActionMethod_Asynchronous_ThrowsIfMultipleCompletedMethodsMatched() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Act & assert
            ExceptionHelper.ExpectException<AmbiguousMatchException>(
                delegate {
                    ActionDescriptorCreator creator = selector.FindAction(null, "EventPatternAmbiguous");
                },
                @"Lookup for method 'EventPatternAmbiguousCompleted' on controller type 'MethodLocatorController' failed because of an ambiguity between the following methods:
Void EventPatternAmbiguousCompleted(Int32) on type System.Web.Mvc.Async.Test.AsyncActionMethodSelectorTest+MethodLocatorController
Void EventPatternAmbiguousCompleted(System.String) on type System.Web.Mvc.Async.Test.AsyncActionMethodSelectorTest+MethodLocatorController");
        }

        [TestMethod]
        public void NonAliasedMethodsProperty() {
            // Arrange
            Type controllerType = typeof(MethodLocatorController);

            // Act
            AsyncActionMethodSelector selector = new AsyncActionMethodSelector(controllerType);

            // Assert
            Assert.AreEqual(4, selector.NonAliasedMethods.Count);

            List<MethodInfo> sortedMethods = selector.NonAliasedMethods["foo"].OrderBy(methodInfo => methodInfo.GetParameters().Length).ToList();
            Assert.AreEqual("Foo", sortedMethods[0].Name);
            Assert.AreEqual(0, sortedMethods[0].GetParameters().Length);
            Assert.AreEqual("Foo", sortedMethods[1].Name);
            Assert.AreEqual(typeof(string), sortedMethods[1].GetParameters()[0].ParameterType);

            Assert.AreEqual(1, selector.NonAliasedMethods["EventPattern"].Count());
            Assert.AreEqual("EventPatternAsync", selector.NonAliasedMethods["EventPattern"].First().Name);
            Assert.AreEqual(1, selector.NonAliasedMethods["EventPatternAmbiguous"].Count());
            Assert.AreEqual("EventPatternAmbiguousAsync", selector.NonAliasedMethods["EventPatternAmbiguous"].First().Name);
            Assert.AreEqual(1, selector.NonAliasedMethods["EventPatternWithoutCompletionMethod"].Count());
            Assert.AreEqual("EventPatternWithoutCompletionMethodAsync", selector.NonAliasedMethods["EventPatternWithoutCompletionMethod"].First().Name);
        }

        private class MethodLocatorController : Controller {

            public void Foo() {
            }

            public void Foo(string s) {
            }

            [ActionName("Foo")]
            public void FooRenamed() {
            }

            [ActionName("Bar")]
            public void Bar() {
            }

            [ActionName("PrivateVoid")]
            private void PrivateVoid() {
            }

            protected void ProtectedVoidAction() {
            }

            public static void StaticMethod() {
            }

            public void EventPatternAsync() {
            }

            public void EventPatternCompleted() {
            }

            public void EventPatternWithoutCompletionMethodAsync() {
            }

            public void EventPatternAmbiguousAsync() {
            }

            public void EventPatternAmbiguousCompleted(int i) {
            }

            public void EventPatternAmbiguousCompleted(string s) {
            }

            [ActionName("RenamedCompleted")]
            public void Renamed() {
            }

            // ensure that methods inheriting from Controller or a base class are not matched
            [ActionName("Blah")]
            protected override void ExecuteCore() {
                throw new NotImplementedException();
            }

            public string StringProperty {
                get;
                set;
            }

#pragma warning disable 0067
            public event EventHandler<EventArgs> SomeEvent;
#pragma warning restore 0067

        }

        private class SelectionAttributeController : Controller {

            [Match(false)]
            public void OneMatch() {
            }

            public void OneMatch(string s) {
            }

            public void TwoMatch() {
            }

            [ActionName("TwoMatch")]
            public void TwoMatch2() {
            }

            [Match(true), ActionName("ShouldMatchMethodWithSelectionAttribute")]
            public void MethodHasSelectionAttribute1() {
            }

            [ActionName("ShouldMatchMethodWithSelectionAttribute")]
            public void MethodDoesNotHaveSelectionAttribute1() {
            }

            private class MatchAttribute : ActionMethodSelectorAttribute {
                private bool _match;
                public MatchAttribute(bool match) {
                    _match = match;
                }
                public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo) {
                    return _match;
                }
            }
        }

    }
}
