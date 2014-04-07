using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace uNhAddIns.Adapters.CommonTests
{
    [Serializable]
    public class Other
    {
        private readonly Guid _id = Guid.NewGuid();
        public virtual Guid Id
        {
            get { return _id; }
        }

        private readonly int _concurrencyId = -1;
        public virtual int ConcurrencyId
        {
            get { return _concurrencyId; }
        }

        public virtual string Name { get; set; }
    }


    [Serializable]
    public class Silly
    {
        public Silly() {}

        public Silly(Guid id)
        {
            _id = id;
        }

        private readonly Guid _id = Guid.NewGuid();
        public virtual Guid Id
        {
            get { return _id; }
        }

        private readonly int _concurrencyId = -1;
        public virtual int ConcurrencyId
        {
            get { return _concurrencyId; }
        }

        public virtual string Name { get; set; }

        public virtual Other Other { get; set; }
    }


	public interface IDaoFactory
	{
		TDao GetDao<TDao>();
	}

	public interface ISillyDao
	{
		Silly Get(Guid id);
		IList<Silly> GetAll();
		IQueryable<Silly> Retrieve(Expression<Func<Silly, bool>>  predicate);
		Silly MakePersistent(Silly entity);
		void MakeTransient(Silly entity);
	}

	public interface ISillyCrudModel
	{
		IList<Silly> GetEntirelyList();
		Silly GetIfAvailable(Guid id);
		Silly Save(Silly entity);
		void Delete(Silly entity);
		void ImmediateDelete(Silly entity);
		void AcceptAll();
		void Abort();
	}

	public interface ISillyReportModel
	{
		IQueryable<Silly> GetSillies();

		//this method is suppossed to not work properly.
		void End();
	}
}