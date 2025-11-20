namespace ASP.NETcoreMVC.DynSelect
{
    public record DynSelect
    {
        public string Table { get; init; } = "";
        public string Schema { get; init; } = "pb";
        public int Skip { get; init; }
        public int Take { get; init; } = 100;
        public string? WhereJson { get; init; } // {"col":{"op":"=","value":123}} , {"col":{"op":"ilike","value":"%abc%"}}
        public string[]? Columns { get; init; }  // null이면 전체
        public string? OrderBy { get; init; }    // 안전을 위해 아래에서 검증
    }
}
