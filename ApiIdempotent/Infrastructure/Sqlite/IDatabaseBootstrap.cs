namespace ApiIdempotent.Infrastructure.Sqlite
{
    public interface IDatabaseBootstrap
    {
        void Setup();

        void EnsureDeleted();
    }
}