namespace University.ARTQ
{
    /// <summary>
    /// Токен выражения реляционной алгебры
    /// </summary>
    public sealed class RelationalAlgebraToken : Token
    {
        #region Fields and Properties
        /// <summary>
        /// Тип токена
        /// </summary>
        public RelationalAlgebraTokenType Type { get; set; }
        #endregion

        #region Constructors
        public RelationalAlgebraToken()
        {
            Type = RelationalAlgebraTokenType.Unknown;
            Text = string.Empty;
        }
        
        public RelationalAlgebraToken(RelationalAlgebraTokenType type = RelationalAlgebraTokenType.Unknown
            , string text = null)
        {
            Type = type;
            Text = text;
        }
        #endregion
    }
}