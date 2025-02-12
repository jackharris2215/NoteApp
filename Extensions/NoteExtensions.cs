using System.IO;
using System.Collections.Generic;

namespace CustomControl.Extensions;

public static class NoteExtensions{
    public static Dictionary<string, string> ParseNote(this string file){
        Dictionary<string, string> params_dict = new Dictionary<string, string>();
        if (File.Exists(file)) { 
            string[] str = File.ReadAllText(file).Split("---"); 
            string[] parameters = str[1].Split("\n");
            params_dict.Add("content", str[2].Substring(2));
            for(int i = 1; i<parameters.Length-1; i++){
                string[] p = parameters[i].Split(":");
                params_dict.Add(p[0], p[1]);
            }

        } 
        return params_dict;
    }
}