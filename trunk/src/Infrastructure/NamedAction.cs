using System;

namespace Consumentor.ShopGun
{
    public class NamedAction : INamedAction
    {
        private readonly Action _action;

        public NamedAction(Action action, string actionName)
            : this(actionName)
        {
            this._action = action;
        }

        protected NamedAction(string actionName)
        {
            ((INamedAction)this).ActionName = actionName;
        }

        string INamedAction.ActionName { get; set; }

        Action INamedAction.Action
        {
            get { return _action; }
        }

        bool INamedAction.HasActionName
        {
            get { return string.IsNullOrEmpty(((INamedAction)this).ActionName) == false; }
        }
    }
}