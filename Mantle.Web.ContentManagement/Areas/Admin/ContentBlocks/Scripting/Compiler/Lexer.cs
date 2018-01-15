using System.Collections.Generic;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class Lexer
    {
        private readonly Tokenizer tokenizer;
        private readonly List<Token> tokens = new List<Token>();
        private int tokenIndex;

        public Lexer(Tokenizer tokenizer)
        {
            this.tokenizer = tokenizer;
        }

        public Token Token()
        {
            if (tokenIndex == tokens.Count)
            {
                tokens.Add(tokenizer.NextToken());
            }
            return tokens[tokenIndex];
        }

        public void NextToken()
        {
            tokenIndex++;
        }

        public Marker Mark()
        {
            return new Marker(tokens.Count);
        }

        public void Mark(Marker marker)
        {
            tokenIndex = marker.TokenIndex;
        }

        public struct Marker
        {
            private readonly int _tokenIndex;

            public Marker(int tokenIndex)
            {
                _tokenIndex = tokenIndex;
            }

            public int TokenIndex
            {
                get { return _tokenIndex; }
            }
        }
    }
}