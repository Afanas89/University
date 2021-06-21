namespace University.ARTQ
{
    /// <summary>
    /// Токен sql-выражения
    /// </summary>
    public sealed class SqlToken : Token
    {
        #region Fields and Properties

        /// <summary>
        /// Тип токена
        /// </summary>
        public SqlTokenType Type { get; set; }

        #endregion

        #region Constructors

        public SqlToken()
        {
            Type = SqlTokenType.Unknown;
            Text = string.Empty;
        }
        
        public SqlToken(SqlTokenType type, string text)
        {
            Type = type;
            Text = text;
        }

        #endregion
    }
}