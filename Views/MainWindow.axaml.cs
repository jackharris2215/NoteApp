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
    string notes_dir = "NoteBooks";
    string current_notebook = "LameNoteBook";
    int NoteBlockID = 0;

    bool dragging = false;
    // bool dragged = false;
    List<Avalonia.Point> offsets = new List<Avalonia.Point>();
    Avalonia.Point grid_center_point = new Avalonia.Point(0,0);
    // public Avalonia.Point grid_center = new Avalonia.Point(0.0, 0.0);

    public MainWindow()
    {
        // setup
        // Initialize window
        InitializeComponent();
        // begin thread for saving notes
        // Dispatcher.UIThread.Post(async () => await LongRunningTask(), DispatcherPriority.Background);

        // find and read from notebook to be opened upon launch
        string LaunchNoteBook = File.ReadAllLines(
            Path.Combine(notes_dir, "launch_notebook.txt")
        )[0];
        
        // loadNoteBook(LaunchNoteBook);
        

    }
    // load each note from notebook and set ID to next avaliable number
    // public void loadNoteBook(string LaunchNoteBook){
    //     // find each note in a notebook
    //     current_notebook = LaunchNoteBook;
    //     string[] Notes = File.ReadAllLines(
    //         Path.Combine(notes_dir, LaunchNoteBook)
    //     );
        
    //     NoteBlockID = 0;
    //     for(int i = 1; i<Notes.Length; i++){
    //         string n = Notes[i];
    //         loadNote(("NoteBooks/"+n).ParseNote());
    //         NoteBlockID = Math.Max(NoteBlockID, n[n.Length-5]-'0');
    //     }
    //     NoteBlockID++;
    // }
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
    // public void saveNote(NoteBlock note){
    //     using (StreamWriter o = new StreamWriter(Path.Combine(notes_dir, note.id)))
    //     {
    //         o.WriteLine("id:"+note.id);
    //         o.WriteLine("top:"+note.GetValue(Canvas.TopProperty));
    //         o.WriteLine("left:"+note.GetValue(Canvas.LeftProperty));
    //         o.WriteLine("width:"+note.size[0]);
    //         o.WriteLine("height:"+note.size[1]);
    //         o.WriteLine("---");
    //         o.WriteLine(note.noteContent);
    //     }
    // }
    // public void updateNotes(){
    //     List<NoteBlock> temp = new List<NoteBlock>();
    //     foreach(NoteBlock n in notes){
    //         if(n.remove){
    //             deleteNote(n);
    //         }
    //         else if(n.edited | dragged){
    //             saveNote(n);
    //             temp.Add(n);
    //             n.edited = false;
    //         }
    //     }
    //     notes = temp;
    //     dragged = false;
    // }
    // public void deleteNote(NoteBlock note){
    //     Canvas main = canvas_container;
    //     main.Children.Remove(note);
    //     File.Delete(Path.Combine(notes_dir, note.id));
    //     updateNotebook();
    // }
    // public void updateNotebook(){
    //     string coords = grid_center_object.GetValue(Canvas.LeftProperty).ToString()
    //                         +","+
    //                         grid_center_object.GetValue(Canvas.TopProperty).ToString();
    //     using (StreamWriter o = new StreamWriter(Path.Combine(notes_dir, current_notebook))){
    //         o.WriteLine(coords);
    //         foreach(NoteBlock n in notes){
    //             o.WriteLine(n.id);
    //         }
    //     }
    // }
    // async Task LongRunningTask(){
    //     while(true){
    //         await Task.Delay(1000);
    //         updateNotes();
    //     }
    // }

    // add a new note
    public void addNoteHandler(object sender, RoutedEventArgs e){
        // create name of note including file extension
        string name = current_notebook.Substring(0,current_notebook.Length-4).ToLower() + 
                          "note" + 
                          NoteBlockID.ToString() + 
                          ".txt";
        // create dictionary of default values
        Dictionary<string, string> new_note = new Dictionary<string, string>
        {
            { "id", name},
            { "width", "300" },
            { "height", "200" },
            { "left", "20" },
            { "top", "20" },
            { "content", "Note" }
        };
        // load note, increment note ID, and add name to notebook file
        loadNote(new_note);
        NoteBlockID++;
        // File.AppendAllText(Path.Combine(notes_dir, current_notebook), name + Environment.NewLine);
        
    }
    // call whenever pointer is pressed and object has pressed event call
    public void PointerPressedHandler(object sender, PointerPressedEventArgs args){
        if (!args.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
            return;
        dragging = true;

        offsets.Add(args.GetCurrentPoint(grid_center_object).Position);
        foreach (NoteBlock n in notes){
            offsets.Add(args.GetCurrentPoint(n).Position);
        }
    }
    // call whenever pointer is released and object has released event call
    public void PointerReleasedHandler(object sender, PointerReleasedEventArgs args){
        offsets.Clear();
        dragging = false;
    }
    // call whenever pointer is moved
    protected override void OnPointerMoved(PointerEventArgs args){
        if(!dragging)
            return;

        Avalonia.Point pos = args.GetPosition(canvas_container) - offsets[0];
        grid_center_object.SetValue(Canvas.LeftProperty, pos.X);
        grid_center_object.SetValue(Canvas.TopProperty, pos.Y);
        for(int i = 0; i < notes.Count; i++){
            pos = args.GetPosition(canvas_container) - offsets[i+1];
            notes[i].SetValue(Canvas.LeftProperty, pos.X);
            notes[i].SetValue(Canvas.TopProperty, pos.Y);
        }
    }
    // built in for windows drag window
    public void BeginWindowDrag(object sender, PointerPressedEventArgs e){
        BeginMoveDrag(e);
    }
}