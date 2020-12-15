using System.Collections.Generic;
using System.IO;
using System;


namespace lex_analyzer
{

    enum States { Start, ID, ERROR };
    public enum Token { KeyWord, Separator, ID, ERROR };
    public class Lexems
    {
       

        public class LexemToken
        {
            public string lexem;
            public Token token;
            public int cLine;
            public LexemToken Next;

            public LexemToken(string lexem, Token token, int cLine)
            {
                this.lexem = lexem;
                this.token = token;
                this.cLine = cLine;

            }
        }

        public class LexTokList
        {
            public LexemToken head;
            public LexemToken tail;
            public void Add(string lexem, Token token, int cLine)
            {
                LexemToken node = new LexemToken(lexem, token, cLine);

                if (head == null)
                {
                    head = node;
                }
                else
                {
                    tail.Next = node;
                }
                tail = node;
            }

            public void Display()
            {
                LexemToken curr = head;
                while(curr != null)
                {
                    Console.Write(curr.lexem + ", ");
                    Console.Write(curr.token + ", ");
                    Console.WriteLine(curr.cLine);
                    Console.WriteLine("---------------------------------------------------");
                    curr = curr.Next;
                }
            }

        }
        public static LexTokList GetLexem()
        {


            StreamReader f1 = new StreamReader(@"1.txt");

            string text = f1.ReadToEnd();

            States state = States.Start;
            Token tokn;

            List<string> KeyWords = new List<string>() { "int", "double", "string", "bool", "class",
                "private", "protected", "public", "abstract", "extern" , "override", "static", "virtual", "volatile"};
            List<char> Separator = new List<char>() { ';', '{', '}', '(', ')', ',', ':' };

            List<string> listID = new List<string>();

            int countLine = 1;

            bool dotExist = false;

            string buf = "";

            LexTokList mainList = new LexTokList();

            foreach (char cur_char in text)
            {
                switch (state)
                {
                    case States.Start:

                        if (cur_char == '\n' || cur_char == '\0' || cur_char == ' ' || cur_char == '\t' || cur_char == '\r')
                        {
                            if (cur_char == '\n')
                            {
                                countLine++;
                            }
                            continue;
                        }

                        else if (char.IsLetter(cur_char))
                        {
                            buf += cur_char;
                            state = States.ID;
                        }

                        else if (Separator.Contains(cur_char))
                        {
                            tokn = Token.Separator;
                            mainList.Add(cur_char.ToString(), tokn, countLine);
                        }

                        else
                        {
                            buf += cur_char;
                            state = States.ERROR;
                        }
                        break;

                    case States.ID:

                        if (char.IsLetterOrDigit(cur_char) || cur_char == '_' || cur_char == '.')
                        {
                            if (cur_char == '.' && !dotExist)
                            {
                                dotExist = true;
                            }
                            else if (cur_char == '.' && dotExist)
                            {
                                state = States.ERROR;
                            }
                            buf += cur_char;
                        }
                        else
                        {
                            if (cur_char == ' ' || cur_char == '\t' || cur_char == '\0' || cur_char == '\r' || cur_char == '\n' || Separator.Contains(cur_char))
                            {
                                if (KeyWords.Contains(buf))
                                {
                                    tokn = Token.KeyWord;
                                    mainList.Add(buf, tokn, countLine);
                                    buf = "";
                                }
                                else
                                {
                                    if (!listID.Contains(buf))
                                    {
                                        listID.Add(buf);
                                    }
                                    tokn = Token.ID;
                                    mainList.Add(buf, tokn, countLine);
                                    buf = "";
                                }
                                if (cur_char == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(cur_char))
                                {
                                    tokn = Token.Separator;
                                    mainList.Add(cur_char.ToString(), tokn, countLine);
                                }
                                state = States.Start;
                            }
                            else
                            {
                                buf += cur_char;
                                state = States.ERROR;
                            }
                        }
                        break;

                    case States.ERROR:
                        if (cur_char != ' ' && cur_char != '\t' && cur_char != '\0' && cur_char != '\r' && cur_char != '\n' && !Separator.Contains(cur_char))
                        {
                            buf += cur_char;
                        }
                        else
                        {
                            tokn = Token.ERROR;
                            mainList.Add(buf, tokn, countLine);
                            if (cur_char == '\n')
                            {
                                countLine++;
                            }
                            if (Separator.Contains(cur_char))
                            {
                                tokn = Token.Separator;
                                mainList.Add(cur_char.ToString(), tokn, countLine);
                            }
                            buf = "";
                            state = States.Start;
                        }
                        break;
                }
            }
            f1.Close();
            return mainList;
        }
    }
}
