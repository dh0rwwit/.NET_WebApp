using ASP.NETcoreMVC.Models.Dynamic;

namespace ASP.NETcoreMVC.Services.Interface
{
    public interface IPgDynamicService
    {
        Task<Dictionary<string, List<DbParamType>>> ExecuteIPgDynamic(string functionName, List<DbParamType> prms);


    }
}
