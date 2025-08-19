namespace ASP.NETcoreMVC.Models.Dynamic
{
    public class DbFunctionRequest
    {
        public string FunctionName { get; set; } = "";
        public List<DbParamType> ReqPrms { get; set; } = new();
    }
}
