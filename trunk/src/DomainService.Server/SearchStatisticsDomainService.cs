using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Logging;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Log;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server
{
    [Interceptor(typeof(LogInterceptor))]
    public class SearchStatisticsDomainService : ISearchStatisticsDomainService
    {
        private readonly IRepository<SearchStatistic> _searchStatisticsRepository;
        private readonly IRepository<Searchterm> _searchtermRepository;

        public SearchStatisticsDomainService(IRepository<SearchStatistic> searchStatisticsRepository, IRepository<Searchterm> searchtermRepository)
        {
            _searchStatisticsRepository = searchStatisticsRepository;
            _searchtermRepository = searchtermRepository;
        }

        public ILogger Log { get; set; }

        #region ISearchStatisticsDomainService Members

        public void AddAdviceSearch(User user, string searchstring, string userAgent, string imei, string model, bool resultFound, string osVersion)
        {
            var searchterm = _searchtermRepository.Find(
                match => match.Searchstring == searchstring).FirstOrDefault() ??
                             new Searchterm { Searchstring = searchstring };

            var adviceSearch = new SearchStatistic
                                 {
                                     Timestamp = DateTime.Now,
                                     User = user,
                                     Imei = imei,
                                     Model = model,
                                     UserAgent = userAgent,
                                     ResultFound = resultFound,
                                     OsVersion = osVersion
                                 };

            searchterm.SearchStatistics.Add(adviceSearch);
            
            if (searchterm.Id == 0)
            {
                _searchtermRepository.Add(searchterm);
            }
            
            _searchtermRepository.Persist();
        }

        public IList<SearchStatistic> GetSearchStatisticsByUser(User user)
        {
            return GetSearchStatisticsByUser(user, DateTime.MinValue, DateTime.Now);
        }

        public IList<SearchStatistic> GetSearchStatisticsByUser(User user, DateTime from, DateTime until)
        {
            var adviceSearches =
                _searchStatisticsRepository.Find(advicesearch => advicesearch.User == user
                    && advicesearch.Timestamp > from
                    && advicesearch.Timestamp < until);

            return adviceSearches.ToList();
        }

        public int GetNumberSearchesForSearchstring(string searchstring)
        {
            return
                _searchtermRepository.Find(match => match.Searchstring == searchstring).FirstOrDefault().
                    SearchStatistics.Count;
        }

        public IList<Searchterm> GetTopNSearchterms(int n)
        {
            return GetTopNSearchterms(n, DateTime.MinValue, DateTime.MaxValue);
        }

        public IList<Searchterm> GetTopNSearchterms(int n, DateTime fromDate, DateTime untilDate)
        {
            var searchterms = _searchtermRepository.Find(s => s.Id != 0).OrderByDescending(s => s.SearchStatistics.Count).Take(n);

            return searchterms.ToList();
        }

        public IList<User> GetUsersWhoSearched(Searchterm searchterm)
        {
            throw new NotImplementedException();
        }

        public Searchterm GetSearchStatistics(string searchstring)
        {
            return _searchtermRepository.Find(s => s.Searchstring == searchstring).FirstOrDefault();
        }

        public IList<Searchterm> GetSearchterms()
        {
            return _searchtermRepository.Find(s => s.Id != 0).ToList();
        }

        #endregion
    }
}
