using System;
using System.Collections.Generic;
using System.IO; 

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;

using CustomControl.Controls;
using CustomControl.Extensions;

namespace CustomControl.Helpers;

public static class SaveandLoad{
    static string NotesDir = "NoteBooks";
    public static void SaveNoteBook(List<NoteBlock> Notes, string Notebook, string grid_center){
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(NotesDir, Notebook)))
        {
            foreach(NoteBlock b in Notes){
                string id = b.id;
                string width = b.size[0].ToString();
                string height = b.size[1].ToString();
                string left = b.GetValue(Canvas.LeftProperty).ToString();
                string top = b.GetValue(Canvas.TopProperty).ToString();
                string fontSize = b.fontSize.ToString();
                string borders = b.borders[0].ToString() + "," + 
                                 b.borders[1].ToString() + "," + 
                                 b.borders[2].ToString() + "," + 
                                 b.borders[3].ToString();
                string bold = b.bold?"t":"f";
                string content = b.noteContent;

                outputFile.WriteLine("id:"+id);
                outputFile.WriteLine("width:"+width);
                outputFile.WriteLine("height:"+height);
                outputFile.WriteLine("left:"+left);
                outputFile.WriteLine("top:"+top);
                outputFile.WriteLine("fontSize:"+fontSize);
                outputFile.WriteLine("borders:"+borders);
                outputFile.WriteLine("bold:"+bold);
                outputFile.WriteLine("---");
                outputFile.WriteLine(content);
                outputFile.WriteLine("<END_OF_NOTE>");
            }
            outputFile.WriteLine(grid_center);
        }
    }
    public static Dictionary<string, string> LoadNote_FromString(string note){
        // there is definitely a better way to do this
        Dictionary<string, string> params_dict = new Dictionary<string, string>();
        string[] split = note.Split("---");
        string[] lines = split[0].Split(
            new string[] { Environment.NewLine },
            StringSplitOptions.None
        );
        // clear new lines that get added at beginning and end for some reason
        params_dict["content"] = split[1].Substring(2, split[1].Length-4);

        foreach(string parameter in lines){
            if(parameter.Contains(":")){
                string[] p_split = parameter.Split(":");
                params_dict[p_split[0]] = p_split[1];
            }
        }

        return params_dict;
    }
}