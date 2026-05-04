namespace ConstructionSaaS.Api.Repositories
{
    public interface ICompanyRepository
    {
        Task<int> CreateCompanyAsync(string name);
    }
}
