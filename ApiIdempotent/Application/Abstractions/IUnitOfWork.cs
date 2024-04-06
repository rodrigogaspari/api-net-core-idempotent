using System;

namespace ApiIdempotent.Application.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
