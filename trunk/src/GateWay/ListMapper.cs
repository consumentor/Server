using System.Collections.Generic;

namespace Consumentor.ShopGun.Gateway
{
    public class ListMapper<TDomain, TGateway> : IListMapper<TDomain, TGateway>
        where TDomain : class
    {
        private readonly IMapper<TDomain, TGateway> _mapper;

        public ListMapper(IMapper<TDomain, TGateway> mapper)
        {
            _mapper = mapper;
        }

        IEnumerable<TGateway> IMapper<IEnumerable<TDomain>, IEnumerable<TGateway>>.Map(IEnumerable<TDomain> source)
        {
            var dest = new List<TGateway>();
            foreach (var item in source)
            {
                dest.Add(_mapper.Map(item));
            }
            return dest;
        }

        IEnumerable<TDomain> IMapper<IEnumerable<TDomain>, IEnumerable<TGateway>>.Map(IEnumerable<TGateway> source)
        {
            var dest = new List<TDomain>();
            foreach (var item in source)
            {
                dest.Add(_mapper.Map(item));
            }
            return dest;
        }

        IEnumerable<TGateway> IListMapper<TDomain, TGateway>.Map<TDomainBase>(IEnumerable<TDomainBase> source)
        {
            var dest = new List<TGateway>();
            foreach (var item in source)
            {
                dest.Add(_mapper.Map(item as TDomain));
            }
            return dest;
        }

        IEnumerable<TDomainBase> IListMapper<TDomain, TGateway>.Map<TDomainBase>(IEnumerable<TGateway> source)
        {
            var dest = new List<TDomainBase>();
            foreach (TGateway item in source)
            {
                TDomain mappedObject = _mapper.Map(item);
                dest.Add(mappedObject as TDomainBase);
            }
            return dest;
        }
    }
}