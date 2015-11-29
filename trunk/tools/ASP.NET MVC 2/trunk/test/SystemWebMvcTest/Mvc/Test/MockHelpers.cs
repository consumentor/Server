namespace System.Web.Mvc.Test {
    using Moq;
    using Moq.Language.Flow;

    [CLSCompliant(false)]
    public static class MockHelpers {

        public static IExpect ExpectMvcVersionResponseHeader(this Mock<HttpContextBase> mock) {
            return mock.Expect(r => r.Response.AppendHeader(MvcHandler.MvcVersionHeaderName, "2.0"));
        }

    }
}
