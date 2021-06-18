namespace University.ARTQ
{
    /// <summary>
    /// Тип токена sql-выражения
    /// </summary>
    public enum SqlTokenType
    {
        Select,
        From,
        Where,
        Operator,
        SeparatorOpen,
        SeparatorClose,
        Unknown
    }
}