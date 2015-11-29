using System;
using System.Collections.Generic;
using Consumentor.ShopGun.Domain;

namespace Consumentor.ShopGun.DomainService.Server.Interfaces
{
    public interface ISearchStatisticsDomainService
    {
        void AddAdviceSearch(User user, string searchstring, string userAgent, string imei, string model, bool resultFound, string osVersion);
        Searchterm GetSearchStatistics(string searchstring);
        IList<SearchStatistic> GetSearchStatisticsByUser(User user);
        IList<SearchStatistic> GetSearchStatisticsByUser(User user, DateTime from, DateTime until);
        IList<Searchterm> GetSearchterms();
        int GetNumberSearchesForSearchstring(string searchstring);
        IList<Searchterm> GetTopNSearchterms(int n);
        IList<User> GetUsersWhoSearched(Searchterm searchterm);
    }
}
