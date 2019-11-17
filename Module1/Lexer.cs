using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexer
{
    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }
    }

    public class Lexer
    {
        protected int position;
        protected char currentCh; // очередной считанный символ
        protected int currentCharValue; // целое значение очередного считанного символа
        protected System.IO.StringReader inputReader;
        protected string inputString;

        public Lexer(string input)
        {
            inputReader = new System.IO.StringReader(input);
            inputString = input;
        }

        public void Error()
        {
            System.Text.StringBuilder o = new System.Text.StringBuilder();
            o.Append(inputString + '\n');
            o.Append(new System.String(' ', 22+position - 1) + "^\n");
            o.AppendFormat("Error in symbol {0}", currentCh);
            throw new LexerException(o.ToString());
        }

        protected void NextCh()
        {
            this.currentCharValue = this.inputReader.Read();
            this.currentCh = (char) currentCharValue;
            this.position += 1;
        }

        public virtual bool Parse()
        {
            return true;
        }
    }

    public class IntLexer : Lexer
    {
        protected System.Text.StringBuilder intString;
        public int parseResult = 0;

        public IntLexer(string input)
            : base(input)
        {
            intString = new System.Text.StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                this.intString.Append(this.currentCh);
                NextCh();
            }

            if (char.IsDigit(currentCh))
            {
                this.intString.Append(this.currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                this.intString.Append(this.currentCh);
                NextCh();
            }

            if (currentCharValue != -1)
            {
                Error();
            }
            
            this.parseResult = int.Parse(this.intString.ToString());
            return true;
        }
    }

    public class IdentLexer : Lexer
    {
        private string parseResult;
        protected StringBuilder builder;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public IdentLexer(string input) : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsLetter(currentCh))
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsLetter(currentCh) || char.IsDigit(currentCh) || currentCh == '_')
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }

            if (currentCharValue != -1)
            {
                Error();
            }
            
            this.parseResult = this.builder.ToString();
            return true;
        }
    }

    public class IntNoZeroLexer : IntLexer
    {
        public IntNoZeroLexer(string input)
            : base(input)
        {
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                this.intString.Append(this.currentCh);
                NextCh();
            }

            if (char.IsDigit(currentCh) && currentCh != '0')
            {
                this.intString.Append(this.currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                this.intString.Append(this.currentCh);
                NextCh();
            }

            if (currentCharValue != -1)
            {
                Error();
            }
            
            this.parseResult = int.Parse(this.intString.ToString());
            return true;
        }
    }

    public class LetterDigitLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();

            if (!char.IsLetter(currentCh))
            {
                Error();
            }

            builder.Append(currentCh);
            NextCh();

            var nextDigit = true;
            while (currentCharValue != -1)
            {
                if ((nextDigit && !char.IsDigit(currentCh)) || (!nextDigit && !char.IsLetter(currentCh)))
                {
                    Error();
                }
                builder.Append(currentCh);
                NextCh();
                nextDigit = !nextDigit;
            }

            return true;
        }
    }

    public class LetterListLexer : Lexer
    {
        protected List<char> parseResult;

        public List<char> ParseResult
        {
            get { return parseResult; }
        }

        public LetterListLexer(string input)
            : base(input)
        {
            parseResult = new List<char>();
        }

        public override bool Parse()
        {
            NextCh();

            if (!char.IsLetter(currentCh))
            {
                Error();
            }
            
            parseResult.Add(currentCh);
            NextCh();
            
            while (currentCharValue != -1)
            {
                if (currentCh != ',' && currentCh != ';')
                {
                    Error();
                }
                NextCh();

                if (!char.IsLetter(currentCh))
                {
                    Error();
                }
                
                parseResult.Add(currentCh);
                NextCh();
            }

            return true;
        }
    }

    public class DigitListLexer : Lexer
    {
        protected List<int> parseResult;

        public List<int> ParseResult
        {
            get { return parseResult; }
        }

        public DigitListLexer(string input)
            : base(input)
        {
            parseResult = new List<int>();
        }

        public override bool Parse()
        {
            NextCh();
            
            if (!char.IsDigit(currentCh))
            {
                Error();
            }
            parseResult.Add(int.Parse(currentCh.ToString()));
            NextCh();
            
            while (this.currentCharValue != -1)
            {
                if (!char.IsWhiteSpace(currentCh))
                {
                    Error();
                }
                
                while (char.IsWhiteSpace(currentCh))
                {
                    NextCh();
                }
                
                if (!char.IsDigit(currentCh))
                {
                    Error();
                }

                parseResult.Add(int.Parse(currentCh.ToString()));
                NextCh();
            }

            return true;
        }

        private void ParseInt()
        {
            var builder = new StringBuilder();
            if (!char.IsDigit(currentCh))
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            Console.WriteLine(builder.ToString());
            var val = int.Parse(builder.ToString());
            parseResult.Add(val);
        }
    }

    public class LetterDigitGroupLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitGroupLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();

            if (!char.IsLetter(currentCh))
            {
                Error();
            }
            
            while (currentCharValue != -1)
            {
                if (!char.IsLetter(currentCh))
                {
                    Error();
                }
                
                while (char.IsLetter(currentCh))
                {
                    this.builder.Append(this.currentCh);
                    NextCh();
                }

                if (currentCharValue == -1)
                {
                    break;
                }

                if (!char.IsDigit(currentCh))
                {
                    Error();
                }
                
                while (char.IsDigit(currentCh))
                {
                    this.builder.Append(this.currentCh);
                    NextCh();
                }
            }

            parseResult = builder.ToString();

            return true;
        }
    }

    public class DoubleLexer : Lexer
    {
        private StringBuilder builder;
        private double parseResult;

        public double ParseResult
        {
            get { return parseResult; }
        }

        public DoubleLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }

            if (char.IsDigit(currentCh))
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }

            if (currentCh == '.')
            {
                this.builder.Append(this.currentCh);
                NextCh();

                if (char.IsDigit(currentCh))
                {
                    this.builder.Append(this.currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }

                while (char.IsDigit(currentCh))
                {
                    this.builder.Append(this.currentCh);
                    NextCh();
                }
            }

            if (currentCharValue != -1)
            {
                Error();
            }
            
            this.parseResult = double.Parse(this.builder.ToString());
            return true;
        }
    }

    public class StringLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public StringLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();

            if (currentCh != '\'')
            {
                Error();
            }

            builder.Append(currentCh);
            NextCh();

            while (currentCh != '\'' && currentCharValue != -1)
            {
                builder.Append(currentCh);
                NextCh();
            }
            
            if (currentCh != '\'')
            {
                Error();
            }
            
            builder.Append(currentCh);
            NextCh();
            
            if (currentCharValue != -1)
            {
                Error();
            }

            return true;
        }
    }

    public class CommentLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public CommentLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '/')
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }

            if (currentCh == '*')
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (true)
            {
                if (currentCharValue == -1)
                {
                    Error();
                }
                
                this.builder.Append(this.currentCh);
                if (currentCh == '*')
                {
                    NextCh();
                    if (currentCh != '/') continue;
                    this.builder.Append(this.currentCh);
                    NextCh();
                    if (currentCharValue != -1)
                    {
                        Error();
                    }
                    break;
                }

                NextCh();
            }

            if (currentCharValue != -1)
            {
                Error();
            }

            this.parseResult = this.builder.ToString();
            return true;
        }
    }

    public class IdentChainLexer : Lexer
    {
        private StringBuilder builder;
        private List<string> parseResult;

        public List<string> ParseResult
        {
            get { return parseResult; }
        }

        public IdentChainLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            parseResult = new List<string>();
        }

        public override bool Parse()
        {
            NextCh();
            ParseIdent();
            
            while (currentCharValue != -1)
            {            
                if (currentCh != '.')
                {
                    Error();
                }
                
                NextCh();
                ParseIdent();
            }

            return true;
        }

        private void ParseIdent()
        {
            if (char.IsLetter(currentCh))
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsLetter(currentCh) || char.IsDigit(currentCh) || currentCh == '_')
            {
                this.builder.Append(this.currentCh);
                NextCh();
            }

            this.parseResult.Add(this.builder.ToString());
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "154216";
            Lexer L = new IntLexer(input);
            try
            {
                L.Parse();
            }
            catch (LexerException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}