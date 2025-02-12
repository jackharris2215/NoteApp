using System;
using System.Collections.Generic;

namespace CustomControl.Extensions;

public static class StringExtensions{
    public static int LocationOf(this string str, char chr){
        char[] x = str.ToCharArray();
        return Array.IndexOf(x, chr);
    }
    // removes new lines and spaces
    public static string Clean(this string str){
            List<char> c_list = new List<char>();
            char[] x = str.ToCharArray();
            foreach(char c in x){
                switch(c){
                    case ' ':
                        break;
                    case '\n':
                        break;
                    default:
                        c_list.Add(c);
                        break;
                }
            }
            return new string(c_list.ToArray());
        }
}