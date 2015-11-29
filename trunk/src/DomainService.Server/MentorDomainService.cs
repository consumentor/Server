using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Consumentor.ShopGun.DomainService.Server.Interfaces;
using Consumentor.ShopGun.Log;
using Castle.Core.Logging;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.Repository;
using System;

namespace Consumentor.ShopGun.DomainService.Server
{
    [Interceptor(typeof(LogInterceptor))]
     public class MentorDomainService : IMentorDomainService
    {
        private readonly IRepository<Mentor> _mentorRepository;

        public MentorDomainService(IRepository<Mentor> mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }

        public ILogger Log { get; set; }

        #region IMentorDomainService Members

        public IList<Mentor> GetAllMentors()
        {
            return _mentorRepository.Find(m => m != null).ToList();
        }

        public Mentor GetMentorById(int id)
        {
            try
            {
                return _mentorRepository.FindOne(m => m.Id == id);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public Mentor CreateNewMentor(string mentorName)
        {
            if(_mentorRepository.Find(m => m.MentorName == mentorName).FirstOrDefault() != null)
            {
                throw new ApplicationException(String.Format("Mentor with name {0} already exists!", mentorName));
            }

            var mentor = new Mentor {MentorName = mentorName};

            _mentorRepository.Add(mentor);
            _mentorRepository.Persist();

            return mentor;
        }

        public Mentor UpdateMentor(Mentor updatedMentor)
        {
            var mentorToUpdate = _mentorRepository.FindOne(x => x.Id == updatedMentor.Id);
            mentorToUpdate.CopyStringProperties(updatedMentor);
            
            _mentorRepository.Persist();

            return mentorToUpdate;
        }

        public void DeleteMentor(Mentor mentorToDelete)
        {
            _mentorRepository.Delete(mentorToDelete);
            _mentorRepository.MergePersist();
        }

        #endregion
    }
}
