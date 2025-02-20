using System;
using System.Collections.Generic;
using System.IO; 
using System.Threading;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Controls.ApplicationLifetimes;

using CustomControl.Controls;
using CustomControl.Extensions;
using CustomControl.Helpers;

namespace CustomControl.Views;

public partial class MainWindow : Window
{
    List<NoteBlock> Notes = new List<NoteBlock>();
    List<NotePreview> Previews = new List<NotePreview>();
    // List<CheckBlock> checkBlocks = new List<CheckBlock>();

    Dictionary<string, string> params_dict = new Dictionary<string, string>();
    string NotesDir = "NoteBooks";
    string current_notebook = "LameNoteBook.txt";
    int NoteBlockID = 0;
    double zoom = 1;
    double[] new_spawn_coords = [10.0, 10.0];

    public bool dragging = false;
    List<Avalonia.Point> offsets = new List<Avalonia.Point>();
    Avalonia.Point grid_center_point = new Avalonia.Point(0,0);

    private Timer saveTimer;

    public MainWindow(){
        InitializeComponent();
        loadNoteBook(current_notebook);

        saveTimer = new Timer(SaveCallback, null, 0, 1000);
    }
    private void SaveCallback(object? state)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                SaveandLoad.SaveNoteBook(Notes, current_notebook, new_spawn_coords[0]+","+new_spawn_coords[1]);
            });
        }
    public void loadNoteBook(string notebook){
        current_notebook = notebook;
        string contents = File.ReadAllText(System.IO.Path.Combine(NotesDir, notebook));
        string[] note_strings = contents.Split("<END_OF_NOTE>");

        // string[] lines = note_strings[0].Split(
        //     new string[] { Environment.NewLine },
        //     StringSplitOptions.None
        // );

        // Dictionary<string, string> new_note = new Dictionary<string, string>
        // {
        //     { "id", "testnote"},
        //     { "width", "400" },
        //     { "height", "200" },
        //     { "left", "30" },
        //     { "top",  "30" },
        //     { "fontSize", "20"},
        //     { "borders", "0,0,0,0" },
        //     { "bold", "f" },
        //     { "content",  lines[0]}
        // };
        // loadBlock(new_note, "note");
        // Dictionary<string, string> new_note = SaveandLoad.LoadNote_FromString(note_strings[2]);

        // loadBlock(new_note, "note");
        
        // string full = "";
        // foreach (KeyValuePair<string, string> kvp in new_note)
        // {
        //     //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        //     full += string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
        //     full += Environment.NewLine;
        // }
        // debugger.Text = full;
        for(int i = 0; i<note_strings.Length-1; i++){
            loadBlock(SaveandLoad.LoadNote_FromString(note_strings[i]), "note", false);
        }
        // set new note spawn offset from file
        debugger.Text = note_strings[note_strings.Length-1].Substring(0,note_strings[note_strings.Length-1].Length-2);
        string[] grid_point = note_strings[note_strings.Length-1].Substring(0,note_strings[note_strings.Length-1].Length-2).Split(",");
        new_spawn_coords = [Int32.Parse(grid_point[0]),Int32.Parse(grid_point[1])];
        new_note_spawn.SetValue(Canvas.LeftProperty, new_spawn_coords[0]);
        new_note_spawn.SetValue(Canvas.TopProperty, new_spawn_coords[1]);
    }
    public void loadBlock(Dictionary<string, string> parameters, string blockType, bool focused){
        int width = Int32.Parse(parameters["width"]);
        int height = Int32.Parse(parameters["height"]);
        double left = double.Parse(parameters["left"]);
        double top = double.Parse(parameters["top"]);
        int font_size = Int32.Parse(parameters["fontSize"]);
        string[] borders = parameters["borders"].Split(",");
        string bold = parameters["bold"];
        string content = parameters["content"];

        var preview = new NotePreview {
            content = content.Length>10?content.Substring(0, 10)+"...":content
        };

        StackPanel prevs = note_previews_container;
        prevs.Children.Insert(0, preview);
        Previews.Add(preview);
        preview.refresh();

        var block = new NoteBlock {
                    id = parameters["id"],
                    size = new int[2] {width, height},
                    fontSize = font_size,
                    noteContent = content,
                    borders = new int[4] {Int32.Parse(borders[0]),
                                            Int32.Parse(borders[1]),
                                            Int32.Parse(borders[2]),
                                            Int32.Parse(borders[3])},
                    bold = bold=="t"?true:false,
                    window = this,
                    preview = preview,
        };

        Canvas main = canvas_container;
        Canvas.SetLeft(block, left);
        Canvas.SetTop(block, top);
        main.Children.Insert(0, block);
        Notes.Add(block);
        if(focused)
            (block as dynamic).OnFocus();
        else
            (block as dynamic).OffFocus();
            (block as dynamic).refresh();
            
    } 

    // add a new note
    public void addNoteHandler(object sender, RoutedEventArgs e){
        var object_name = (sender as Button)?.Name;
        if (object_name == null)
            return;
        // create name of note including file extension
        string name = "note" + NoteBlockID.ToString();
        // create dictionary of default values
        debugger.Text = (this.Width).ToString();
        double origin_x = ((Math.Round(this.Width/20)*10)-400)+new_spawn_coords[0];
        double origin_y = ((Math.Round(this.Height/20)*10)-200)+new_spawn_coords[1];
        Dictionary<string, string> new_note = new Dictionary<string, string>
        {
            { "id", name},
            { "width", "400" },
            { "height", "200" },
            { "left", origin_x.ToString() }, //(new_spawn_coords[0] + 200*(zoom-1)).ToString()
            { "top",  origin_y.ToString() }, //(new_spawn_coords[1] + 200*(zoom-1)).ToString()
            { "fontSize", "20"},
            { "borders", "0,0,0,0" },
            { "bold", "f" },
            { "content", "" }
        };
        // load note, increment note ID, and add name to notebook file
        switch(object_name){
            case "add_check":
                loadBlock(new_note, "check", true);
                break;
            case "add_note":
                loadBlock(new_note, "note", true);
                break;
        }
        NoteBlockID++;
    }
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e){
        switch(e.Delta.Y){
            case(-1): zoom = Math.Max(0.5, Math.Round(zoom-0.1, 1)); break;
            case(1): zoom = Math.Min(2, Math.Round(zoom+0.1, 1)); break;
        }
        canvas_container.RenderTransformOrigin = new RelativePoint((this.Width/2)-400, (this.Height/2)-200, RelativeUnit.Absolute);
        canvas_container.RenderTransform = new ScaleTransform(){
            ScaleX = zoom,
            ScaleY = zoom
        };
        // debugger.Text = zoom.ToString();
    }
    // call whenever pointer is pressed and object has pressed event call
    public void PointerPressedHandler(object sender, PointerPressedEventArgs args){
        if(args.GetCurrentPoint(this).Properties.IsRightButtonPressed){
            debugger.Text = canvas_container.Children.ToString();
        }
        if (!args.GetCurrentPoint(this).Properties.IsMiddleButtonPressed)
            return;
        dragging = true;

        offsets.Add(args.GetCurrentPoint(new_note_spawn).Position);
        foreach (UserControl n in Notes){
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
        new_spawn_coords = [Math.Round((pos.X)%10), Math.Round((pos.Y)%10)];
        new_note_spawn.SetValue(Canvas.LeftProperty, new_spawn_coords[0]+(300*(zoom-1)));
        new_note_spawn.SetValue(Canvas.TopProperty, new_spawn_coords[1]+(300*(zoom-1)));
        // debugger.Text = new_spawn_coords[0].ToString() + ", " + new_spawn_coords[1].ToString();

        for(int i = 0; i < Notes.Count; i++){
            pos = args.GetPosition(canvas_container) - offsets[i+1];
            Notes[i].SetValue(Canvas.LeftProperty, Math.Round(pos.X));
            Notes[i].SetValue(Canvas.TopProperty, Math.Round(pos.Y));
        }
    }
    // built in for windows drag window
    public void BeginWindowDrag(object sender, PointerPressedEventArgs e){
        BeginMoveDrag(e);
    }
    public void ZoomIn(object sender, RoutedEventArgs args){
        zoom = Math.Min(2, Math.Round(zoom+0.1, 1));

        canvas_container.RenderTransformOrigin = new RelativePoint(this.Width/2, this.Height/2, RelativeUnit.Absolute);
        canvas_container.RenderTransform = new ScaleTransform(){
            ScaleX = zoom,
            ScaleY = zoom
        };
    }
    public void ZoomOut(object sender, RoutedEventArgs args){
        zoom = Math.Max(0.5, Math.Round(zoom-0.1, 1));

        canvas_container.RenderTransformOrigin = new RelativePoint(this.Width/2, this.Height/2, RelativeUnit.Absolute);
        canvas_container.RenderTransform = new ScaleTransform(){
            ScaleX = zoom,
            ScaleY = zoom
        };
    }
    public void DeleteNotes(object sender, PointerReleasedEventArgs e){
        for(int i = Notes.Count-1; i>=0; i--){
            if(Previews[i].selected == true){
                note_previews_container.Children.Remove(Previews[i]);
                canvas_container.Children.Remove(Notes[i]);
                Previews.RemoveAt(i);
                Notes.RemoveAt(i);
            }
        }
    }
    public void SelectAll(object sender, PointerReleasedEventArgs e){
        foreach(NotePreview n in Previews){
            n.focus();
        }
    }
    public void DeselectAll(object sender, PointerReleasedEventArgs e){
        foreach(NotePreview n in Previews){
            n.unfocus();
        }
    }
}