using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JiteLang.Main.LangLexer.Token;
using JiteLang.Source;
using JiteLang.Syntax;

namespace JiteLang.Main.LangLexer
{
    internal class Lexer
    {
        private readonly SourceCode _sourceCode;

        private int _lineCount = 1;
        private int _lineChar = 1;
        public Lexer(string sourceText)
        {
            _sourceCode = new SourceCode(sourceText);
        }

        public Lexer(SourceCode sourceCode)
        {
            _sourceCode = sourceCode;
        }

        public List<TokenInfo> Lex()
        {
            var list = new List<TokenInfo>();

            while (_sourceCode.CurrentHasValue())
            {
                var token = ScanToken();
                if (token.Kind != SyntaxKind.None)
                {
                    list.Add(token);
                }
            }

            return list;
        }

        private TokenInfo ScanToken()
        {
            var token = new TokenInfo
            {
                Kind = SyntaxKind.None
            };

            var lastPosition = _sourceCode.Position;

            var syntaxPosition = new SyntaxPosition(_lineCount, _lineChar);

            var letter = _sourceCode.Current;

            switch (letter)
            {
                case >= 'a' and <= 'z' or >= 'A' and <= 'Z':
                    ScanIdentifierOrKeyword(ref token);
                    break;

                case >= '0' and <= '9':
                    ScanNumericLiteral(ref token);
                    break;


                case '\"':
                    _sourceCode.Advance();
                    ScanStringLiteral(ref token);
                    break;
                case '\'':
                    _sourceCode.Advance();
                    ScanCharLiteral(ref token);
                    break;

                case '{':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.OpenBraceToken;
                    break;
                case '}':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.CloseBraceToken;
                    break;
                case '(':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.OpenParenToken;
                    break;
                case ']':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.OpenBracketToken;
                    break;
                case '[':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.CloseBracketToken;
                    break;
                case ')':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.CloseParenToken;
                    break;
                case ';':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.SemiColon;
                    break;

                case '=':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.EqualsEqualsToken : SyntaxKind.EqualsToken;
                    break;

                case '!':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.NotEqualsToken : SyntaxKind.NotToken;
                    break;

                case '>':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.GreaterThanEqualsToken : SyntaxKind.GreaterThanToken;
                    break;
                case '<':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.LessThanEqualsToken : SyntaxKind.LessThanToken;
                    break;

                case '+':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.PlusEqualsToken : SyntaxKind.PlusToken;
                    break;
                case '-':
                    _sourceCode.Advance();
                    if(char.IsDigit(_sourceCode.Current))
                    {
                        ScanNumericLiteral(ref token, true);
                    }
                    else
                    {
                        token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.MinusEqualsToken : SyntaxKind.MinusToken;
                    }
                    break;
                case '*':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.AsteriskEqualsToken : SyntaxKind.AsteriskToken;
                    break;
                case '/':
                    _sourceCode.Advance();
                    if (_sourceCode.AdvanceIf('='))
                    {
                        token.Kind = SyntaxKind.SlashEqualsToken;
                    }
                    else if (_sourceCode.AdvanceIf('/')) // //comments
                    {
                        _sourceCode.AdvanceUntil('\n', out _);
                    }
                    else if (_sourceCode.AdvanceIf('*')) // /* */ comments
                    {
                        while (_sourceCode.Current != '*' && _sourceCode.PeekNext(out var nextValue) && nextValue != '/')
                        {
                            _sourceCode.Advance();
                        }
                        _sourceCode.Advance();
                        _sourceCode.Advance();
                    }
                    else
                    {
                        token.Kind = SyntaxKind.SlashToken;
                    }
                    break;
                case '%':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('=') ? SyntaxKind.PercentEqualsToken : SyntaxKind.PercentToken;
                    break;

                case '&':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('&') ? SyntaxKind.AmpersandAmpersandToken : SyntaxKind.AmpersandToken;
                    break;

                case '|':
                    _sourceCode.Advance();
                    token.Kind = _sourceCode.AdvanceIf('|') ? SyntaxKind.BarBarToken : SyntaxKind.BarToken;
                    break;


                case ',':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.CommaToken;
                    break;

                case '.':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.DotToken;
                    break;

                case '^':
                    _sourceCode.Advance();
                    token.Kind = SyntaxKind.CaretToken;
                    break;      
                
                case '\n':
                    _sourceCode.Advance();
                    _lineCount++;
                    _lineChar = 1;
                    break;

                default:
                    _sourceCode.Advance();
                    break;
            }

            var length = _sourceCode.Position - lastPosition;

            var characters = _sourceCode.Array.AsSpan(lastPosition, length);
            var visibleLength = VisibleCharLength(characters);

            _lineChar += visibleLength;

            syntaxPosition.Length = visibleLength;

            token.Position = syntaxPosition;

            return token;
        }

        private static int VisibleCharLength(in ReadOnlySpan<char> chars)
        {
            var visibleCount = 0;
            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (!char.IsControl(c))
                {
                    visibleCount++;
                }
            }

#if DEBUG
            var debugVisible = new string(chars.ToString().Where(c => !char.IsControl(c)).ToArray());
            Debug.Assert(debugVisible.Length == visibleCount);
#endif

            return visibleCount;
        }

        private void ScanStringLiteral(ref TokenInfo token)
        {
            if (_sourceCode.AdvanceUntil('\"', out var stringVal))
            {
                token.Kind = SyntaxKind.StringLiteralToken;
                token.StringValue = stringVal.ToString();
                token.Text = token.StringValue;
                _sourceCode.Advance();
            }
        }

        private void ScanNumericLiteral(ref TokenInfo token, bool isNegative = false)
        {
            var current = _sourceCode.Position;
            var length = 0;

            while (_sourceCode.Current is >= '0' and <= '9')
            {
                _sourceCode.Advance();
                length++;
            }

            var isLong = _sourceCode.AdvanceIf('L') || _sourceCode.AdvanceIf('l');

            if (length > 0)
            {
                var text = _sourceCode.Slice(current, length);

                token.Text = isNegative ? '-' + text.ToString() : text.ToString();

                if (isLong)
                {
                    token.LongValue = long.Parse(token.Text); // make it faster
                    token.Kind = SyntaxKind.LongLiteralToken;
                }
                else 
                {
                    token.IntValue = int.Parse(token.Text); // make it faster
                    token.Kind = SyntaxKind.IntLiteralToken;
                }
            }
        }

        private void ScanCharLiteral(ref TokenInfo token)
        {
            token.Kind = SyntaxKind.CharLiteralToken;
            _sourceCode.Advance(out var val);

            token.CharValue = val;
            token.Text = val.ToString();

            _sourceCode.AdvanceIf('\'');
        }

        private void ScanIdentifierOrKeyword(ref TokenInfo token)
        {
            var current = _sourceCode.Position;
            var length = 0;

            while (_sourceCode.Current is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9')
            {
                _sourceCode.Advance();
                length++;
            }

            if (length > 0)
            {
                var text = _sourceCode.Slice(current, length).ToString();
                var keyword = SyntaxFacts.GetKeyword(text);

                token.Text = text;
                token.Kind = keyword == SyntaxKind.None ? SyntaxKind.IdentifierToken : keyword;
            }
        }
    }
}
