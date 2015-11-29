using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.DomainService.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Consumentor.ShopGun.Repository;
using Consumentor.ShopGun.Domain;
using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using Consumentor.ShopGun.Configuration;

namespace DomainServiceTest
{
    
    
    /// <summary>
    ///This is a test class for AdviceSearchStatisticsDomainServiceTest and is intended
    ///to contain all AdviceSearchStatisticsDomainServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdviceSearchStatisticsDomainServiceTest
    {
        private readonly ISearchStatisticsDomainService _adviceSearchStatisticsDomainService;

        public AdviceSearchStatisticsDomainServiceTest()
        {
            try
            {
                var container = ContainerFactory.CreateContainer();
                _adviceSearchStatisticsDomainService = container.Resolve(typeof(ISearchStatisticsDomainService)) as ISearchStatisticsDomainService;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void GetUsersWhoSearchedTest()
        {
        }

        [TestMethod()]
        public void GetNumberAdviceSearchesTest()
        {
        }

        [TestMethod()]
        public void GetAdviceSearchesForUserTest1()
        {
        }

        [TestMethod()]
        public void GetAdviceSearchesForUserTest()
        {
        }

        [TestMethod()]
        public void AddAdviceSearchTest1()
        {
        }

        [TestMethod()]
        public void AddAdviceSearchTest()
        {
            _adviceSearchStatisticsDomainService.AddAdviceSearch(null, Platforms.Unknown, "moep");
            
        }

        [TestMethod()]
        public void AdviceSearchStatisticsDomainServiceConstructorTest()
        {
        }
    }
}
