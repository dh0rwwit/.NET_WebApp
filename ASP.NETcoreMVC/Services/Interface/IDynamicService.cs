using ASP.NETcoreMVC.Models.Dynamic;

namespace ASP.NETcoreMVC.Services.Interface
{
    public interface IDynamicService
    {
        Task<Dictionary<string, List<DbParamType>>> ExecutePgDynamic(string functionName, List<DbParamType> prms);


    }
}
