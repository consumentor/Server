#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace System.Web.Mvc.ExpressionUtil {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;

    // MemberExpression fingerprint class
    // Expression of form xxx.FieldOrProperty
    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals",
        Justification = "Overrides AddToHashCodeCombiner() instead.")]
    internal sealed class MemberExpressionFingerprint : ExpressionFingerprint {

        private MemberExpressionFingerprint(MemberExpression expression)
            : base(expression) {

            Member = expression.Member;
        }

        public MemberInfo Member {
            get;
            private set;
        }

        public ExpressionFingerprint Target {
            get;
            private set;
        }

        internal override void AddToHashCodeCombiner(HashCodeCombiner combiner) {
            base.AddToHashCodeCombiner(combiner);

            combiner.AddObject(Member);
            combiner.AddFingerprint(Target);
        }

        public static MemberExpressionFingerprint Create(MemberExpression expression, ParserContext parserContext) {
            ExpressionFingerprint target = Create(expression.Expression, parserContext);
            if (target == null && expression.Expression != null) {
                return null;
            }

            return new MemberExpressionFingerprint(expression) {
                Target = target
            };
        }

        public override bool Equals(object obj) {
            MemberExpressionFingerprint other = obj as MemberExpressionFingerprint;
            if (other == null) {
                return false;
            }

            return (this.Member == other.Member
                && Object.Equals(this.Target, other.Target)
                && base.Equals(other));
        }

        public override Expression ToExpression(ParserContext parserContext) {
            Expression targetExpr = ToExpression(Target, parserContext);
            return Expression.MakeMemberAccess(targetExpr, Member);
        }

    }
}
