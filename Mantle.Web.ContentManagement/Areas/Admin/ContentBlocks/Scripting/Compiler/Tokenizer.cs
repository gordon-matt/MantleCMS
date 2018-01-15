using System;
using System.Text;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class Tokenizer
    {
        private readonly string expression;
        private readonly StringBuilder stringBuilder;
        private int index;
        private int startTokenIndex;

        public Tokenizer(string expression)
        {
            this.expression = expression;
            stringBuilder = new StringBuilder();
        }

        public Token NextToken()
        {
            while (true)
            {
                if (Eof())
                    return CreateToken(TokenKind.Eof);

                startTokenIndex = index;
                char ch = Character();
                switch (ch)
                {
                    case '(':
                        NextCharacter();
                        return CreateToken(TokenKind.OpenParen);

                    case ')':
                        NextCharacter();
                        return CreateToken(TokenKind.CloseParen);

                    case ',':
                        NextCharacter();
                        return CreateToken(TokenKind.Comma);

                    case '+':
                        NextCharacter();
                        return CreateToken(TokenKind.Plus);

                    case '-':
                        NextCharacter();
                        return CreateToken(TokenKind.Minus);

                    case '*':
                        NextCharacter();
                        return CreateToken(TokenKind.Mul);

                    case '/':
                        NextCharacter();
                        return CreateToken(TokenKind.Div);

                    case '"':
                        return LexStringLiteral();

                    case '\'':
                        return LexSingleQuotedStringLiteral();

                    case '!':
                        return LexNotSign();

                    case '|':
                        return LexOrSign();

                    case '&':
                        return LexAndSign();

                    case '=':
                        return LexEqual();

                    case '<':
                        return LexLessThan();

                    case '>':
                        return LexGreaterThan();
                }

                if (IsDigitCharacter(ch))
                {
                    return LexInteger();
                }
                if (IsIdentifierCharacter(ch))
                {
                    return LexIdentifierOrKeyword();
                }
                if (IsWhitespaceCharacter(ch))
                {
                    NextCharacter();
                    continue;
                }

                return InvalidToken();
            }
        }

        private Token InvalidToken()
        {
            return CreateToken(TokenKind.Invalid, "Unrecognized character");
        }

        private Token LexNotSign()
        {
            NextCharacter();
            char ch = Character();
            if (ch == '=')
            {
                NextCharacter();
                return CreateToken(TokenKind.NotEqual);
            }
            return CreateToken(TokenKind.NotSign);
        }

        private Token LexOrSign()
        {
            NextCharacter();
            char ch = Character();
            if (ch == '|')
            {
                NextCharacter();
                return CreateToken(TokenKind.OrSign);
            }
            return InvalidToken();
        }

        private Token LexAndSign()
        {
            NextCharacter();
            char ch = Character();
            if (ch == '&')
            {
                NextCharacter();
                return CreateToken(TokenKind.AndSign);
            }
            return InvalidToken();
        }

        private Token LexGreaterThan()
        {
            NextCharacter();
            char ch = Character();
            if (ch == '=')
            {
                NextCharacter();
                return CreateToken(TokenKind.GreaterThanEqual);
            }
            return CreateToken(TokenKind.GreaterThan);
        }

        private Token LexLessThan()
        {
            NextCharacter();
            char ch = Character();
            if (ch == '=')
            {
                NextCharacter();
                return CreateToken(TokenKind.LessThanEqual);
            }
            return CreateToken(TokenKind.LessThan);
        }

        private Token LexEqual()
        {
            NextCharacter();
            char ch = Character();
            if (ch == '=')
            {
                NextCharacter();
                return CreateToken(TokenKind.EqualEqual);
            }
            return CreateToken(TokenKind.Equal);
        }

        private Token LexIdentifierOrKeyword()
        {
            stringBuilder.Clear();

            stringBuilder.Append(Character());
            while (true)
            {
                NextCharacter();

                if (!Eof() && (IsIdentifierCharacter(Character()) || IsDigitCharacter(Character())))
                {
                    stringBuilder.Append(Character());
                }
                else
                {
                    return CreateIdentiferOrKeyword(stringBuilder.ToString());
                }
            }
        }

        private Token LexInteger()
        {
            stringBuilder.Clear();

            stringBuilder.Append(Character());
            while (true)
            {
                NextCharacter();

                if (!Eof() && IsDigitCharacter(Character()))
                {
                    stringBuilder.Append(Character());
                }
                else
                {
                    return CreateToken(TokenKind.Integer, Int32.Parse(stringBuilder.ToString()));
                }
            }
        }

        private Token CreateIdentiferOrKeyword(string identifier)
        {
            switch (identifier)
            {
                case "true":
                    return CreateToken(TokenKind.True, true);

                case "false":
                    return CreateToken(TokenKind.False, false);

                case "or":
                    return CreateToken(TokenKind.Or);

                case "and":
                    return CreateToken(TokenKind.And);

                case "not":
                    return CreateToken(TokenKind.Not);

                case "null":
                case "nil":
                    return CreateToken(TokenKind.NullLiteral);

                default:
                    return CreateToken(TokenKind.Identifier, identifier);
            }
        }

        private static bool IsWhitespaceCharacter(char character)
        {
            return char.IsWhiteSpace(character);
        }

        private static bool IsIdentifierCharacter(char ch)
        {
            return
                (ch >= 'a' && ch <= 'z') ||
                (ch >= 'A' && ch <= 'Z') ||
                (ch == '_');
        }

        private static bool IsDigitCharacter(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private Token LexSingleQuotedStringLiteral()
        {
            stringBuilder.Clear();

            while (true)
            {
                NextCharacter();

                if (Eof())
                    return CreateToken(TokenKind.Invalid, "Unterminated string literal");

                // Termination
                if (Character() == '\'')
                {
                    NextCharacter();
                    return CreateToken(TokenKind.SingleQuotedStringLiteral, stringBuilder.ToString());
                }

                // backslash notation
                if (Character() == '\\')
                {
                    NextCharacter();

                    if (Eof())
                        return CreateToken(TokenKind.Invalid, "Unterminated string literal");

                    switch (Character())
                    {
                        case '\\':
                            stringBuilder.Append('\\');
                            break;

                        case '\'':
                            stringBuilder.Append('\'');
                            break;

                        default:
                            stringBuilder.Append('\\');
                            stringBuilder.Append(Character());
                            break;
                    }
                }

                // Regular character in string
                else
                {
                    stringBuilder.Append(Character());
                }
            }
        }

        private Token LexStringLiteral()
        {
            stringBuilder.Clear();

            while (true)
            {
                NextCharacter();

                if (Eof())
                    return CreateToken(TokenKind.Invalid, "Unterminated string literal");

                // Termination
                if (Character() == '"')
                {
                    NextCharacter();
                    return CreateToken(TokenKind.StringLiteral, stringBuilder.ToString());
                }

                // backslash notation
                if (Character() == '\\')
                {
                    NextCharacter();

                    if (Eof())
                        return CreateToken(TokenKind.Invalid, "Unterminated string literal");

                    stringBuilder.Append(Character());
                }

                // Regular character in string
                else
                {
                    stringBuilder.Append(Character());
                }
            }
        }

        private void NextCharacter()
        {
            index++;
        }

        private char Character()
        {
            return expression[index];
        }

        private Token CreateToken(TokenKind kind, object value = null)
        {
            return new Token
            {
                Kind = kind,
                Position = startTokenIndex,
                Value = value
            };
        }

        private bool Eof()
        {
            return (index >= expression.Length);
        }
    }
}