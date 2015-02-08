namespace ltl2ba
{
    class lex
    {
        static Symbol[]	symtab = new Symbol[util.Nhash+1];
        static char[] yytext = new char[2048];
        static int Token(int y)
        {
            parse.tl_yylval = cache.tl_nn(y,util.ZN,util.ZN);
            return y;
        }
        private static bool StartString = false;

        internal static int isalnum_(int c)
        {
            if(((char)c) == '"')
            {
                StartString = !StartString;

                //return true for the second '"'
                if (!StartString)
                {
                    return 1;
                }
                else
                {
                     //there is char before the ", then we need to stop
                     return 0;                    
                }
            }

            if(StartString)
            {
                return 1;                
            }
            else
            {
                return (char.IsLetterOrDigit((char)c) || c == '_' || c == '.' || c == '?' || c == '!') ? 1 : 0;    
            }            
        }

        internal static int isFirstCharAlnum_(int c)
        {
            if (StartString)
            {
                return 1;
            }
            else
            {
                return (char.IsLetterOrDigit((char)c) || c == '_') ? 1 : 0;
            }
        }

        internal static int hash(char[] s)
        {
            int h = 0;

            for (int i = 0; s[i] != '\0'; i++)
            {
                h += s[i];
                h <<= 1;
                if ((h & (util.Nhash + 1)) != 0)
                    h |= 1;
            }
            return h & util.Nhash;
        }
        internal static void getword(int first) //, TST tst
        {
            int i = 0;
            char c;

            //if the word starts with ", then set the start string to be true;
            if (((char) first) == '"')
            {
                StartString = true;
            }


            yytext[i++] = (char)first;

            if (isFirstCharAlnum_(c = (char)first) != 0)
            {
                while (isalnum_(c = (char)main.tl_Getchar()) != 0)
                    yytext[i++] = c;
            }
            
            yytext[i] = '\0';
            main.tl_UnGetchar();
        }

        internal static int follow(int tok, int ifyes, int ifno)
        {	int c;
	        string buf = "";
	        //extern int tl_yychar;

	        if ((c = main.tl_Getchar()) == tok)
		        return ifyes;
	        main.tl_UnGetchar();
	        parse.tl_yychar = c;
	        buf = string.Format("expected '%c'", tok);
	        main.tl_yyerror(buf);	/* no return from here */
	        return ifno;
        }
        
        internal static int tl_yylex()
        {	
            int c = tl_lex();
	        return c;
        }

        internal static int tl_lex()
        {
            int c;

            do
            {
                c = main.tl_Getchar();
                yytext[0] = (char)c;
                yytext[1] = '\0';

                if (c <= 0)
                {
                    return Token(';');
                }

            } while (c == ' ');	/* '\t' is removed in tl_main.c */

            if (char.IsLower((char)c) || char.IsUpper((char)c) || (char)c == '"')  
            {
                getword(c); //, isalnum_
                int len = 0;
                for (int i = 0; yytext[i] != '\0'; i++)
                {
                    len++;
                }
                string yytextstr = new string(yytext, 0, len);
                
                //special case for the the three operators
                if (yytextstr != "U" && yytextstr != "V" && yytextstr != "X" && yytextstr != "R" && yytextstr != "G" && yytextstr != "F")
                {
                    if ("true".CompareTo(yytextstr) == 0)
                    {
                        return Token((int)Operator.TRUE);
                    }

                    if ("false".CompareTo(yytextstr) == 0)
                    {
                        return Token((int)Operator.FALSE);
                    }

                    parse.tl_yylval = cache.tl_nn((int)Operator.PREDICATE, util.ZN, util.ZN);
                    parse.tl_yylval.sym = tl_lookup(yytext, len);
                    return (int)Operator.PREDICATE;    
                }
            }
            if (c == '<')
            {
                c = main.tl_Getchar();
                if (c == '>')
                {
                    return Token((int)Operator.EVENTUALLY);
                }

                if (c != '-')
                {
                    main.tl_UnGetchar();
                    main.tl_yyerror("expected '<>' or '<->'");
                }

                c = main.tl_Getchar();
                if (c == '>')
                {
                    return Token((int)Operator.EQUIV);
                }
                main.tl_UnGetchar();
                main.tl_yyerror("expected '<->'");
            }

            switch (c)
            {
                case '/': c = follow('\\', (int)Operator.AND, '/'); break;
                case '\\': c = follow('/', (int)Operator.OR, '\\'); break;
                case '&': c = follow('&', (int)Operator.AND, '&'); break;
                case '|': c = follow('|', (int)Operator.OR, '|'); break;
                case '[': c = follow(']', (int)Operator.ALWAYS, '['); break;
                case '-': c = follow('>', (int)Operator.IMPLIES, '-'); break;
                case '!': c = (int)Operator.NOT; break;
                case 'U': c = (int)Operator.U_OPER; break;
                case 'R': c = (int)Operator.V_OPER; break;
                case 'V': c = (int)Operator.V_OPER; break;
                case 'X': c = (int)Operator.NEXT; break;
                case 'G': c = (int)Operator.ALWAYS; break;
                case 'F': c = (int)Operator.EVENTUALLY; break;

                default: break;
            }

            return Token(c);
        }

        internal static Symbol tl_lookup(string s)
        {
            Symbol sp;
            
            char[] ss = new char[s.Length + 1];
            for (int i = 0; i < s.Length; i++)
                ss[i] = s[i];
            //ss[s.Length] = '\0';

            int h = hash(ss);

            for (sp = symtab[h]; sp != null; sp = sp.next)
            {
                if (sp.name.CompareTo(s) == 0)
                {
                    return sp;
                }
            }

            sp = new Symbol();
            
            sp.name = s;
            //sp.name = s;
            sp.next = symtab[h];
            symtab[h] = sp;

            return sp;
        }

        internal static Symbol tl_lookup(char[] s, int len)
        {
            Symbol sp;
            int h = hash(s);

            /*int len = 0;
            for (int i = 0; i < s.Length; i++)
                len++;
            */

            string ss = new string(s, 0, len);
            for (sp = symtab[h]; sp != null; sp = sp.next)
                if (sp.name.CompareTo(ss) == 0)
                    return sp;

            sp = new Symbol();

            sp.name = ss;
            //sp.name = s;
            sp.next = symtab[h];
            symtab[h] = sp;

            return sp;
        }
        internal static Symbol getsym(Symbol s)
        {
            Symbol n = new Symbol();

            n.name = s.name;
            return n;
        }
    }
    delegate int TST(int n, bool isStartSymbol);
}
