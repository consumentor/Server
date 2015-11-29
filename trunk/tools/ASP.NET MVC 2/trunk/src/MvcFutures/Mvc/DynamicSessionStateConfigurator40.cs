namespace Microsoft.Web.Mvc {
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web;

    internal sealed class DynamicSessionStateConfigurator40 : IDynamicSessionStateConfigurator {

        // lazy evaluation so that the Executor constructor isn't called prematurely
        private static readonly Lazy<Executor> _executorFunc = new Lazy<Executor>(() => new Executor());
        private readonly HttpContextBase _httpContext;

        public DynamicSessionStateConfigurator40(HttpContextBase httpContext) {
            _httpContext = httpContext;
        }

        public void ConfigureSessionState(ControllerSessionState mode) {
            Executor executor = _executorFunc.Eval();
            executor.ConfigureSessionState(_httpContext, mode);
        }

        private sealed class Executor {
            // Need to accept an Int32 rather than the actual enumeration type since the method that calls
            // this delegate is statically compiled against .NET 3.5 SP1, and the actual enumeration type
            // didn't exist until .NET 4.
            private readonly Action<HttpContextBase, int> _setter;

            public Executor() {
                Type sessionStateBehaviorType = typeof(HttpContext).Assembly.GetType("System.Web.SessionState.SessionStateBehavior");
                MethodInfo setSessionStateBehaviorMethod = typeof(HttpContextBase).GetMethod("SetSessionStateBehavior", new Type[] { sessionStateBehaviorType });
                ParameterExpression httpContextBaseParam = Expression.Parameter(typeof(HttpContextBase), "contextBase");
                ParameterExpression sessionStateBehaviorParam = Expression.Parameter(typeof(int), "sessionStateBehavior");

                MethodCallExpression callExpr = Expression.Call(httpContextBaseParam, setSessionStateBehaviorMethod, Expression.Convert(sessionStateBehaviorParam, sessionStateBehaviorType));
                var lambdaExpr = Expression.Lambda<Action<HttpContextBase, int>>(callExpr, httpContextBaseParam, sessionStateBehaviorParam);
                _setter = lambdaExpr.Compile();
            }

            public void ConfigureSessionState(HttpContextBase httpContext, ControllerSessionState mode) {
                _setter(httpContext, (int)mode);
            }
        }

    }
}
