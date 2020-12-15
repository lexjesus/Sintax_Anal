using System;
using lex_analyzer;

namespace Sintax_my_anal
{
    class Program
    {
        class ParseException : Exception
        {
            public int Value { get; }
            public ParseException(string message, int val)
                : base(message)
            {
                Value = val;
            }
        }
        public class Parser
        {
            public Lexems.LexTokList lex;
            public Lexems.LexemToken curr;

            public Parser(Lexems.LexTokList lex)
            {
                this.lex = lex;
                curr = lex.head;
            }

            public void ClassDecl()
            {

                Header();
                if (String.Equals(curr.lexem, "class"))
                {
                   curr = curr.Next;
                }
                else 
                {
                   throw new ParseException("Error of compilation, ClassDecl(1)!", curr.cLine);
                }

                if (curr.token == Token.ID) 
                {
                   curr = curr.Next;
                }
                else
                {
                    throw new ParseException("Error of compilation, ClassDecl(2)!", curr.cLine);
                }

                ClassBaseOpt();

                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, "{"))
                    {
                        curr = curr.Next;
                    }
                    else
                    {
                        throw new ParseException("Error of compilation, ClassDecl(3)!", curr.cLine);
                    }
                }

                ClassItemDecsOpt();

                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, "}"))
                    {
                        curr = curr.Next;
                    }
                    else
                    {
                        throw new ParseException("Error of compilation, ClassDecl(4)!", curr.cLine);
                    }
                }
                if (curr != null)
                {
                    curr = curr.Next;
                    ClassDecl();
                }
            }

            public void Header()
            {
                AccessOpt();
                Modifier();  
            }

            public void AccessOpt()
            {

                if (curr.token == Token.KeyWord )
                {
                   if (String.Equals(curr.lexem, "private") || String.Equals(curr.lexem, "protected") || String.Equals(curr.lexem, "public"))
                   {
                       curr = curr.Next;
                   }
                }
            }

            public void Modifier()
            {
                if (curr.token == Token.KeyWord)
                { 
                    if (String.Equals(curr.lexem, "abstract") || String.Equals(curr.lexem, "override") ||
                        String.Equals(curr.lexem, "static") || String.Equals(curr.lexem, "extern") ||
                        String.Equals(curr.lexem, "virtual") || String.Equals(curr.lexem, "volatile"))
                    {
                        curr = curr.Next;
                    } 
                }
            }

            public void ClassBaseOpt()
            {
                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, ":"))
                    {
                        curr = curr.Next;
                        if (curr.token == Token.ID)
                        {
                            curr = curr.Next;
                        }
                        else
                        {
                            throw new ParseException("Error of compilation, ClassBaseOpt()!", curr.cLine);
                        }
                    }
                }
            }

            public void ClassItemDecsOpt()
            {
                if (curr.lexem != "}")
                {
                    ClassItem();
                }
            }

            public void ClassItem()
            {
                HTID();
                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, ";"))
                    {
                        curr = curr.Next;
                        ClassItemDecsOpt();
                    }
                    else if (String.Equals(curr.lexem, "("))
                    {
                        curr = curr.Next;
                        MethodDec();
                    }
                    else
                    {
                        throw new ParseException("Error of compilation, ClassItem()!", curr.cLine);
                    }
                }
            }
            
            public void HTID()
            {
                Header();

                IdType();

                if (curr.token == Token.ID)
                {
                    curr = curr.Next;
                }
                else
                {
                    throw new ParseException("Error of compilation, HTID()!", curr.cLine);
                }
            }

            public void MethodDec()
            {

                FormalParamListOpt();
                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, ")"))
                    {
                        curr = curr.Next;
                    }
                }
                else
                {
                    throw new ParseException("Error of compilation, MethodDec(1)!", curr.cLine);
                }

                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, ";"))
                    {
                        curr = curr.Next;
                    }
                }
                else
                {
                    throw new ParseException("Error of compilation, MethodDec(2)!", curr.cLine);
                }
            }

            public void IdType()
            {
                if (curr.token == Token.ID)
                {
                    curr = curr.Next;
                }
                else if (curr.token == Token.KeyWord)
                {
                    if (String.Equals(curr.lexem, "int") || String.Equals(curr.lexem, "double") ||
                       String.Equals(curr.lexem, "string") || String.Equals(curr.lexem, "bool"))
                    {
                        curr = curr.Next;
                    }
                }
                else
                {
                    throw new ParseException("Error of compilation, IdType()!", curr.cLine);
                }
            }
            public void FormalParamListOpt()
            {
                ParamList();
            }

            public void ParamList()
            {
                IdType();
                if (curr.token == Token.ID)
                {
                    curr = curr.Next;
                }
                if (curr.token == Token.Separator)
                {
                    if (String.Equals(curr.lexem, ","))
                    {
                        curr = curr.Next;
                        ParamList();
                    }
                }
            } 
        }

        static void Main()
        {
            Lexems.LexTokList lexMas = new Lexems.LexTokList();

            lexMas = Lexems.GetLexem();

            lexMas.Display();
            Console.WriteLine();
            Parser p = new Parser(lexMas);

            try
            {
                p.ClassDecl();
                Console.WriteLine("Compilation complete success.");
            }
            catch (ParseException e)
            {
                Console.WriteLine(e.Message + " CountLine: " + e.Value);
            }

            Console.ReadKey();
        }
    }
}