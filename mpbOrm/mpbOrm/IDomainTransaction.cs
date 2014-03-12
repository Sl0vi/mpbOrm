namespace mpbOrm
{
    using System;

    public interface IDomainTransaction : IDisposable
    {
        void Commit();
    }
}
