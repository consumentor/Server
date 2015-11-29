using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Repository;

namespace Consumentor.ShopGun.DomainService.Server
{
    [Obsolete]
    public class AdviceDomainService<TEntity, TAdvice> : IAdviceDomainService<TEntity, TAdvice>
        where TEntity : class, IAdviceable<TAdvice>
        where TAdvice : AdviceBase, new()
    {
        private readonly DataContext _dataContext;
        protected readonly IRepository<TEntity> EntityRepository;
        protected readonly IRepository<TAdvice> AdviceRepository;

        public AdviceDomainService(DataContext dataContext, IRepository<TEntity> entityRepository, IRepository<TAdvice> adviceRepository)
        {
            _dataContext = dataContext;
            EntityRepository = entityRepository;
            AdviceRepository = adviceRepository;
        }

        public TAdvice AddAdvice(Mentor mentor, TEntity entity, Semaphore semaphore, string label, string introduction, string advice, string keyWords, bool publish)
        {
            //entity = _adviceRepository.AttachItem(entity);
            //mentor = _adviceRepository.AttachItem(mentor);
            //semaphore = _adviceRepository.AttachItem(semaphore);
            //var adviceToAdd = new TAdvice
            //{
            //    Mentor = mentor,
            //    Label = label,
            //    Introduction = introduction,
            //    Advice = advice,
            //    KeyWords = keyWords,
            //    Semaphore = semaphore,
            //    Published = publish
            //};

            //entity.AddAdvice(adviceToAdd);
            //_adviceRepository.Add(adviceToAdd);
            //_adviceRepository.Persist();

            //if (publish)
            //{
            //    Publish(adviceToAdd);
            //}

            //return adviceToAdd;

            //var entityFromDc = _entityRepository.FindOne(x => x == entity);
            //mentor = _entityRepository.AttachItem(mentor);
            //semaphore = _entityRepository.AttachItem(semaphore);
            //var adviceToAdd = new TAdvice
            //{
            //    Mentor = mentor,
            //    Label = label,
            //    Introduction = introduction,
            //    Advice = advice,
            //    KeyWords = keyWords,
            //    Semaphore = semaphore,
            //    Published = publish
            //};

            //entityFromDc.AddAdvice(adviceToAdd);
            //_entityRepository.MergePersist();

            //if (publish)
            //{
            //    Publish(adviceToAdd);
            //}

            //return adviceToAdd;

            var entityFromDc = _dataContext.GetTable<TEntity>().Where(x => x == entity).FirstOrDefault();
            var mentorFromDc = _dataContext.GetTable<Mentor>().Where(m => m == mentor).FirstOrDefault();
            var semaphoreFromDc = _dataContext.GetTable<Semaphore>().Where(s => s == semaphore).FirstOrDefault();
            var adviceToAdd = new TAdvice
            {
                Mentor = mentorFromDc,
                Label = label,
                Introduction = introduction,
                Advice = advice,
                KeyWords = keyWords,
                Semaphore = semaphoreFromDc,
                Published = publish
            };

            entityFromDc.AddAdvice(adviceToAdd);
            try
            {
                _dataContext.SubmitChanges();
            }
            catch (SqlException ex)
            {
                entityFromDc.RemoveAdvice(adviceToAdd);
                _dataContext.ChangeConflicts.Clear();
            }

            if (publish)
            {
                Publish(adviceToAdd);
            }

            return adviceToAdd;
        }

        public TAdvice UpdateAdvice(TAdvice adviceToUpdate, Semaphore semaphore, string label, string introduction, string advice, string keyWords, bool publish)
        {
            var adviceFromRepository = AdviceRepository.FindOne(a => a == adviceToUpdate);

            //TODO code below handles versioning of advices - disabled for the moment
            // If the previous version has been published or once published and then unpublished, a new version has to be created and added))
            //if (adviceFromRepository.Published || adviceFromRepository.UnpublishDate != null)
            //{
            //    var newAdviceVersion = adviceFromRepository.CreateNewVersion<TAdvice>();
            //    _adviceRepository.Add(newAdviceVersion);
            //    _adviceRepository.Persist();

            //    adviceFromRepository = newAdviceVersion;
            //}

            adviceFromRepository = CopyUpdatedValues(adviceFromRepository, semaphore, label, introduction, advice, keyWords, publish);
            AdviceRepository.MergePersist();

            if (publish)
            {
                Publish(adviceFromRepository);
            }

            return adviceFromRepository;
        }

        public TAdvice GetAdviceById(int id)
        {
            return AdviceRepository.FindOne(a => a.Id == id);
        }

        public void Publish(TAdvice adviceToPublish)
        {
            var adviceFromRepository = AdviceRepository.FindOne(a => a == adviceToPublish);

            //Todo versioning disabled for the moment...
            //if (adviceFromRepository.PreviousVersion != null)
            //{
            //    Unpublish(adviceFromRepository.PreviousVersion as TAdvice);
            //}

            //// If adviceToPublish already has been published and unpublished, a new version has to be created
            //if (adviceFromRepository.UnpublishDate != null)
            //{
            //    var newAdviceVersion = adviceFromRepository.CreateNewVersion<TAdvice>();
            //    _adviceRepository.Add(newAdviceVersion);
            //    adviceFromRepository = newAdviceVersion;
            //}

            adviceFromRepository.Published = true;
            adviceFromRepository.PublishDate = DateTime.Now;
            AdviceRepository.Persist();
        }

        public void Unpublish(TAdvice adviceToUnpublish)
        {
            var adviceFromRepository = AdviceRepository.FindOne(a => a == adviceToUnpublish);
            adviceFromRepository.Published = false;
            adviceFromRepository.UnpublishDate = DateTime.Now;
            AdviceRepository.Persist();
        }

        public void Delete(TAdvice adviceToDelete)
        {
            AdviceRepository.Delete(adviceToDelete);
            AdviceRepository.Persist();
        }

        #region Helper methods

        protected TAdvice CopyUpdatedValues(TAdvice adviceFromRepository, Semaphore semaphore, string label, string introduction, string advice, string keyWords, bool publish)
        {
            adviceFromRepository.Semaphore = semaphore.Id == adviceFromRepository.Semaphore.Id ? adviceFromRepository.Semaphore : AdviceRepository.AttachItem(semaphore);
            adviceFromRepository.Label = label;
            adviceFromRepository.Introduction = introduction;
            adviceFromRepository.Advice = advice;
            adviceFromRepository.KeyWords = keyWords;
            adviceFromRepository.Published = publish;

            return adviceFromRepository;
        }

        public IList<TAdvice> GetAdvicesByMentor(Mentor mentor)
        {
            var advices = AdviceRepository.Find(a => a.Mentor == mentor).ToList();
            return advices;
        }

        #endregion
    }
}
