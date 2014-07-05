////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace System
{
    using System;
    using System.Threading;
    using System.Collections;
    using System.Runtime.CompilerServices;
    /**
     * <p>The <code>String</code> class represents a static string of characters.  Many of
     * the <code>String</code> methods perform some type of transformation on the current
     * instance and return the result as a new <code>String</code>. All comparison methods are
     * implemented as a part of <code>String</code>.</p>  As with arrays, character positions
     * (indices) are zero-based.
     *
     * <p>When passing a null string into a constructor in VJ and VC, the null should be
     * explicitly type cast to a <code>String</code>.</p>
     * <p>For Example:<br>
     * <pre>String s = new String((String)null);
     * Text.Out.WriteLine(s);</pre></p>
     *
     * @author Jay Roxe (jroxe)
     * @version
     */
    [Serializable]
    public sealed class String : IComparable
    {
        public static readonly String Empty = "";

        private char[] chars;

        public override bool Equals(object obj)
        {
            String s = obj as String;
            if (s != null)
            {
                return String.Equals(this, s);
            }

            return false;
        }
       
        public static bool Equals(String a, String b)
        {
            var charsA = a.ToCharArray();
            var charsB = b.ToCharArray();

            if (charsA == null && charsB == null)
            {
                return true;
            }

            if (charsA == null || charsB == null)
            {
                return false;
            }

            var index = 0;
            while (true)
            {
                var cA = charsA[index];
                var cB = charsB[index];

                if (cA == cB)
                {
                    index++;
                }

                var cmpA = cA == '\0';
                var cmpB = cB == '\0';
                if (cmpA && cmpB)
                {
                    return true;
                }

                if (cmpA || cmpB)
                {
                    return false;
                }
            }
        }
        
        public static bool operator ==(String a, String b)
        {
            return a.Equals(b);
        }

        
        public static bool operator !=(String a, String b)
        {
            return !a.Equals(b);
        }

        [System.Runtime.CompilerServices.IndexerName("Chars")]
        public char this[int index]
        {
            
            get
            {
                throw new NotImplementedException();
            }
        }

        
        public char[] ToCharArray()
        {
            return this.chars;
        }

        
        public char[] ToCharArray(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public int Length
        {
            get
            {
                if (chars == null)
                {
                    return 0;
                }

                ////var size = 0;
                ////while (chars[size++] != '\0');

                ////return size + 1;

                return chars.Length;
            }
        }

        public String[] Split(params char[] separator)
        {
            throw new NotImplementedException();
        }
        
        public String[] Split(char[] separator, int count)
        {
            throw new NotImplementedException();
        }

        
        public String Substring(int startIndex)
        {
            throw new NotImplementedException();
        }

        
        public String Substring(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        
        public String Trim(params char[] trimChars)
        {
            throw new NotImplementedException();
        }

        
        public String TrimStart(params char[] trimChars)
        {
            throw new NotImplementedException();
        }

        public String TrimEnd(params char[] trimChars)
        {
            throw new NotImplementedException();
        }

        
        public String(char[] value, int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public String(char[] value)
        {
            this.chars = value;
        }
        
        public String(char c, int count)
        {
            throw new NotImplementedException();
        }
        
        public static int Compare(String strA, String strB)
        {
            throw new NotImplementedException();
        }
        
        public int CompareTo(Object value)
        {
            throw new NotImplementedException();
        }
        
        public int CompareTo(String strB)
        {
            throw new NotImplementedException();
        }
        
        public int IndexOf(char value)
        {
            throw new NotImplementedException();
        }

        
        public int IndexOf(char value, int startIndex)
        {
            throw new NotImplementedException();
        }

        
        public int IndexOf(char value, int startIndex, int count)
        {
            throw new NotImplementedException();
        }

        
        public extern int IndexOfAny(char[] anyOf);

        
        public extern int IndexOfAny(char[] anyOf, int startIndex);

        
        public extern int IndexOfAny(char[] anyOf, int startIndex, int count);

        
        public int IndexOf(String value)
        {
            throw new NotImplementedException();
        }

        
        public extern int IndexOf(String value, int startIndex);

        
        public extern int IndexOf(String value, int startIndex, int count);

        
        public int LastIndexOf(char value)
        {
            throw new NotImplementedException();
        }

        
        public extern int LastIndexOf(char value, int startIndex);

        
        public extern int LastIndexOf(char value, int startIndex, int count);

        
        public extern int LastIndexOfAny(char[] anyOf);

        
        public extern int LastIndexOfAny(char[] anyOf, int startIndex);

        
        public extern int LastIndexOfAny(char[] anyOf, int startIndex, int count);

        
        public extern int LastIndexOf(String value);

        
        public extern int LastIndexOf(String value, int startIndex);

        
        public extern int LastIndexOf(String value, int startIndex, int count);

        
        public String ToLower()
        {
            throw new NotImplementedException();
        }

        
        public String ToUpper()
        {
            throw new NotImplementedException();
        }

        public override String ToString()
        {
            return this;
        }

        
        public String Trim()
        {
            throw new NotImplementedException();
        }

        ////// This method contains the same functionality as StringBuilder Replace. The only difference is that
        ////// a new String has to be allocated since Strings are immutable
        public static String Concat(Object arg0)
        {
            if (arg0 == null)
            {
                return String.Empty;
            }

            return arg0.ToString();
        }

        public static String Concat(Object arg0, Object arg1)
        {
            if (arg0 == null)
            {
                arg0 = String.Empty;
            }

            if (arg1 == null)
            {
                arg1 = String.Empty;
            }

            return Concat(arg0.ToString(), arg1.ToString());
        }

        public static String Concat(Object arg0, Object arg1, Object arg2)
        {
            if (arg0 == null)
            {
                arg0 = String.Empty;
            }

            if (arg1 == null)
            {
                arg1 = String.Empty;
            }

            if (arg2 == null)
            {
                arg2 = String.Empty;
            }

            return Concat(arg0.ToString(), arg1.ToString(), arg2.ToString());
        }

        public static String Concat(params Object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            int length = args.Length;
            String[] sArgs = new String[length];
            int totalLength = 0;

            for (int i = 0; i < length; i++)
            {
                sArgs[i] = ((args[i] == null) ? (String.Empty) : (args[i].ToString()));
                totalLength += sArgs[i].Length;
            }

            return String.Concat(sArgs);
        }

        
        public static String Concat(String str0, String str1)
        {
            var sb = new Text.StringBuilder();
            sb.Append(str0);
            sb.Append(str1);
            return sb.ToString();
        }

        
        public static String Concat(String str0, String str1, String str2)
        {
            var sb = new Text.StringBuilder();
            sb.Append(str0);
            sb.Append(str1);
            sb.Append(str2);
            return sb.ToString();
        }

        
        public static String Concat(String str0, String str1, String str2, String str3)
        {
            var sb = new Text.StringBuilder();
            sb.Append(str0);
            sb.Append(str1);
            sb.Append(str2);
            sb.Append(str3);
            return sb.ToString();
        }

        
        public static String Concat(params String[] values)
        {
            var sb = new Text.StringBuilder();
            foreach (var value in values)
            {
                sb.Append(value);
            }

            return sb.ToString();
        }

        public static String Intern(String str)
        {
            // We don't support "interning" of strings. So simply return the string.
            return str;
        }

        public static String IsInterned(String str)
        {
            // We don't support "interning" of strings. So simply return the string.
            return str;
        }

    }
}

