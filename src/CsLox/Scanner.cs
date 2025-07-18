namespace CsLox
{
    public class Scanner
    {
        private readony Dictionary<> _myVar;
        private readonly string _source;
        private readonly List<Token> _tokens = [];

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private bool IsAtEnd() => _current >= _source.Length;

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespaces
                    break;
                case '\n':
                    _line++;
                    break;

                case '"': LoxString(); break;
                // case 'o':
                //     if (Match('r'))
                //         AddToken(TokenType.OR);
                //     break;
                default:
                    if (IsValidLuxDigit(c))
                        LoxNumber();
                    else if (IsAlpha(c))
                        Identifier();
                    else
                        Program.Error(_line, "Unexpected character.");
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }
            AddToken(TokenType.IDENTIFIER);
        }

        private static bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || c == '_';
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsValidLuxDigit(c);
        }

        private void LoxString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Program.Error(_line, "Unterminated string.");
                return;
            }

            // The closing "
            Advance();

            // trim the surrounding quotes
            string value = _source.Substring(_start + 1, _current - 1);
            AddToken(TokenType.STRING, value);
        }

        private void LoxNumber()
        {
            while (IsValidLuxDigit(Peek()))
            {
                Advance();
            }
            // Look for a fractional part.
            if (Peek() == '.' && IsValidLuxDigit(PeekNext()))
            {
                // consume the '.'
                Advance();
                while (IsValidLuxDigit(Peek()))
                {
                    Advance();
                }
            }
            AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current)));
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private static bool IsValidLuxDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _source.Substring(_start, _current);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
                return false;
            if (_source[_current] != expected) return false;
            _current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }
    }
}

