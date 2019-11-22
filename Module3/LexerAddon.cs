using System;
using System.IO;
using SimpleScanner;
using ScannerHelper;
using System.Collections.Generic;

namespace GeneratedLexer
{
    public class LexerAddon
    {
        public Scanner myScanner;
        private byte[] inputText = new byte[255];

        public int idCount = 0;
        public int minIdLength = Int32.MaxValue;
        public double avgIdLength = 0;
        public int maxIdLength = 0;
        public int sumInt = 0;
        public double sumDouble = 0.0;
        public List<string> idsInComment = new List<string>();


        public LexerAddon(string programText)
        {
            using (StreamWriter writer = new StreamWriter(new MemoryStream(inputText)))
            {
                writer.Write(programText);
                writer.Flush();
            }

            MemoryStream inputStream = new MemoryStream(inputText);

            myScanner = new Scanner(inputStream);
        }

        public void Lex()
        {
            // Чтобы вещественные числа распознавались и отображались в формате 3.14 (а не 3,14 как в русской Culture)
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            int tok = 0;
            do
            {
                tok = myScanner.yylex();

                switch ((Tok) tok)
                {
                    case Tok.ID:
                        if (minIdLength > myScanner.yyleng)
                        {
                            minIdLength = myScanner.yyleng;
                        }

                        if (maxIdLength < myScanner.yyleng)
                        {
                            maxIdLength = myScanner.yyleng;
                        }

                        avgIdLength = (avgIdLength * idCount + myScanner.yyleng) / (idCount + 1);

                        idCount++;
                        break;

                    case Tok.INUM:
                        sumInt += int.Parse(myScanner.yytext);
                        break;

                    case Tok.RNUM:
                        sumDouble += double.Parse(myScanner.yytext);
                        break;

                    case Tok.COMMENT:
                        Console.Out.WriteLine("DEBUG::{0}", myScanner.yytext);
//                        var curLine = myScanner.;
//                        do
//                        {
//                            tok = myScanner.yylex();
//                        } while (myScanner.yyleng);

                        break;
                    
                    case Tok.COMMENTED_ID:
                        idsInComment.Add(myScanner.yytext);
                        break;

                    case Tok.EOF:
                        return;
                }
            } while (true);
        }
    }
}