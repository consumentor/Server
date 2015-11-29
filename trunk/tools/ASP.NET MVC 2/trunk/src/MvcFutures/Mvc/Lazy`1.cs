namespace Microsoft.Web.Mvc {
    using System;

    internal sealed class Lazy<T> {

        private readonly Func<T> _creator;
        private bool _hasExecuted;
        private readonly object _lockObj = new object();
        private T _result;

        public Lazy(Func<T> creator) {
            _creator = creator;
        }

        public T Eval() {
            if (!_hasExecuted) {
                lock (_lockObj) {
                    if (!_hasExecuted) {
                        _result = _creator();
                        _hasExecuted = true;
                    }
                }
            }

            return _result;
        }

    }
}
