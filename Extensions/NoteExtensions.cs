using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CustomControl.Extensions;

public static class NoteExtensions{
    // takes in a note .txt file and returns a dictionary of parameters and content
    public static Dictionary<string, string> ParseNote(this string file){
            Dictionary<string, string> params_dict = new Dictionary<string, string>();
            int index = 0;
            if (File.Exists(file)) { 
                string[] str = File.ReadAllLines(file); 
                while(str[index] != "---"){
                    string[] s = str[index].Split(":");
                    params_dict.Add(s[0], s[1]);
                    index++;
                }
                string content = string.Join(Environment.NewLine, str.Skip(index+1).ToArray());
                params_dict.Add("content", content);
            }
            return params_dict;
        }
    // takes the name of a notebook and returns the names of all associated notes in a list
    public static string[] Notes(this string notebook_name){
            string file = "NoteBooks/" + notebook_name + ".txt";
            Console.WriteLine(file);
            if (File.Exists(file)) { 
                string[] str = File.ReadAllLines(file); 
                return str;
            }
            return ["not found"];
        }

}