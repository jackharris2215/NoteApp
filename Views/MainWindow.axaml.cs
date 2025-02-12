using System;
using System.Collections.Generic;
using System.IO; 
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;


using CustomControl.Controls;
using CustomControl.Extensions;

namespace CustomControl.Views;

public partial class MainWindow : Window
{
    List<NoteBlock> notes = new List<NoteBlock>();
    Dictionary<string, string> params_dict = new Dictionary<string, string>();
    int NoteBlockID = 0;

    bool dragging = false;
    List<Avalonia.Point> offsets = new List<Avalonia.Point>();

    public MainWindow()
    {
        InitializeComponent();
        Dispatcher.UIThread.Post(async () => await LongRunningTask(), DispatcherPriority.Background);
        Canvas main = canvas_container;

        string NoteBook = "NoteBooks/NoteBook.txt"; 
        string[] note_names = File.ReadAllLines(NoteBook);
        foreach(string n in note_names){
            loadNote(("NoteBooks/"+n).ParseNote());
        }
        
    }
    public void loadNote(Dictionary<string, string> parameters){
        int width = Int32.Parse(parameters["width"]);
        int height = Int32.Parse(parameters["height"]);
        int left = Int32.Parse(parameters["left"]);
        int top = Int32.Parse(parameters["top"]);

        var noteBlock = new NoteBlock {
            id = parameters["id"],
            window = this,
            size = new int[4] {width, height, height-20, width-10},
            noteContent = parameters["content"]
        };
        Canvas main = canvas_container;
        Canvas.SetLeft(noteBlock, left);
        Canvas.SetTop(noteBlock, top);
        main.Children.Add(noteBlock);
        notes.Add(noteBlock);
    } 
    public void saveNote(NoteBlock note){
        string docPath = "NoteBooks";
        string noteId = note.id.Substring(0, note.id.Length-1);
        string noteTitle = noteId + ".txt";
        using (StreamWriter o = new StreamWriter(Path.Combine(docPath, noteTitle)))
        {
            o.WriteLine("---");
            o.WriteLine("id:"+noteId);
            o.WriteLine("top:"+note.GetValue(Canvas.TopProperty));
            o.WriteLine("left:"+note.GetValue(Canvas.LeftProperty));
            o.WriteLine("width:"+note.size[0]);
            o.WriteLine("height:"+note.size[1]);
            o.WriteLine("---");
            o.WriteLine(note.noteContent);
        }
        debugger.Text = "hi";
    }
    async Task LongRunningTask(){
        while(true){
            await Task.Delay(3000);
            
            foreach(NoteBlock n in notes)
                saveNote(n);
        }
    }

    public void addNoteHandler(object sender, RoutedEventArgs e)
    {
        Dictionary<string, string> new_note = new Dictionary<string, string>
        {
            { "id", "note_" + NoteBlockID.ToString()},
            { "width", "300" },
            { "height", "200" },
            { "left", "20" },
            { "top", "20" },
            { "content", "Note" }
        };
        loadNote(new_note);
        NoteBlockID++;
        
    }
    public void PointerPressedHandler(object sender, PointerPressedEventArgs args){
        if (!args.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
            return;
        dragging = true;
        
        foreach (NoteBlock n in notes){
            offsets.Add(args.GetCurrentPoint(n).Position);
        }
    }
    public void PointerReleasedHandler(object sender, PointerReleasedEventArgs args){
        offsets.Clear();
        dragging = false;
    }
    protected override void OnPointerMoved(PointerEventArgs args){
        if(!dragging)
            return;

        for(int i = 0; i < notes.Count; i++){
            Avalonia.Point pos = args.GetPosition(canvas_container) - offsets[i];
            notes[i].SetValue(Canvas.LeftProperty, pos.X);
            notes[i].SetValue(Canvas.TopProperty, pos.Y);
        }
    }
}