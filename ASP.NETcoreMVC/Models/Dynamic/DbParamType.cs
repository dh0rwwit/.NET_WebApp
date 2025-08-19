using NpgsqlTypes;
using System.Data;

namespace ASP.NETcoreMVC.Models.Dynamic
{
    public class DbParamType
    {
        public string Name { get; set; } = "";
        public object? Value { get; set; }
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;
        public NpgsqlDbType? Dbtype { get; set; }
    }
}
