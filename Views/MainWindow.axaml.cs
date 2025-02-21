using System;
using System.Collections.Generic;
using System.IO; 
using System.Threading;
using System.Reactive.Linq;
using System.Text;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

using CustomControl.Controls;
using CustomControl.Extensions;
using CustomControl.Helpers;

namespace CustomControl.Views;

public partial class MainWindow : Window
{
    List<NoteBlock> Notes = new List<NoteBlock>();
    List<NotePreview> Previews = new List<NotePreview>();
    List<NotebookButton> Notebooks = new List<NotebookButton>();
    // List<CheckBlock> checkBlocks = new List<CheckBlock>();

    Dictionary<string, string> params_dict = new Dictionary<string, string>();
    string NotesDir = "NoteBooks";
    string current_notebook = "";
    int NoteBlockID = 0;
    double zoom = 1;
    double[] new_spawn_coords = [10.0, 10.0];
    Avalonia.Point mouse_coords = new Avalonia.Point(0,0);

    public bool dragging = false;
    List<Avalonia.Point> offsets = new List<Avalonia.Point>();

    private Timer saveTimer;

    public MainWindow(){
        InitializeComponent();
        // loadNoteBook(current_notebook);

        saveTimer = new Timer(SaveCallback, null, 0, 1000);

        string[] notebook_files = File.ReadAllText(System.IO.Path.Combine(NotesDir, "Notebooks.txt")).Split(
            new string[] { Environment.NewLine },
            StringSplitOptions.None
        );

        loadNoteBook(notebook_files[0]);
        for(int n = 1; n<notebook_files.Length-1; n++){
            string selected = notebook_files[0]==notebook_files[n]?"t":"f";
            var notebook = new NotebookButton {
                name = notebook_files[n].Substring(0, notebook_files[n].Length-4),
                selected=selected
            };
            notebooks_container.Children.Add(notebook);
            Notebooks.Add(notebook);
            notebook.refresh();

            var selected_note = notebook.GetObservable(NotebookButton.SelectedProperty);
            selected_note.Subscribe(value => {
                if(value=="t"){
                    foreach(NotebookButton b in Notebooks){
                        if(b != notebook)
                            b.selected = "f";
                            b.refresh();
                    }    
                    CleanCanvas();             
                    loadNoteBook(notebook.name+".txt");
                }
            });
        }
    }
    private void SaveCallback(object? state){
        // debugger.Text = "saving: " + current_notebook;
        Dispatcher.UIThread.InvokeAsync(() =>{
            if(current_notebook != ".txt" && current_notebook != "")
                SaveandLoad.SaveNoteBook(Notes, current_notebook, new_spawn_coords[0]+","+new_spawn_coords[1]);
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(NotesDir, "Notebooks.txt"))){
                outputFile.WriteLine(current_notebook);
                foreach(NotebookButton n in Notebooks){
                    outputFile.WriteLine(n.name+".txt");
                }
            }
        });
    }
    
    public void loadNoteBook(string notebook){
        if(notebook == "always_empty.txt"){
            LoadEmpty();
            return;
        }
        current_notebook = notebook;
        string notebook_file = System.IO.Path.Combine(NotesDir, notebook);
        if(!File.Exists(notebook_file)){
            // File.Create(notebook_file);
            using (FileStream fs = File.Create(notebook_file))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(new_spawn_coords[0].ToString() + "," + new_spawn_coords[1].ToString() + "\n");

                // Add some information to the file.
                fs.Write(bytes, 0, bytes.Length);
            }
            return;
        }
        string contents = File.ReadAllText(notebook_file);
        string[] note_strings = contents.Split("<END_OF_NOTE>");
        for(int i = 0; i<note_strings.Length-1; i++){
            loadBlock(SaveandLoad.LoadNote_FromString(note_strings[i]), "note", false);
        }
        
        string[] grid_point = note_strings[note_strings.Length-1].Substring(0,note_strings[note_strings.Length-1].Length-2).Split(",");
        new_spawn_coords = [Int32.Parse(grid_point[0]),Int32.Parse(grid_point[1])];
        new_note_spawn.SetValue(Canvas.LeftProperty, new_spawn_coords[0]);
        new_note_spawn.SetValue(Canvas.TopProperty, new_spawn_coords[1]);
    }
    public void LoadEmpty(){
        Dictionary<string, string> new_note = new Dictionary<string, string>
        {
            { "id", "empty"},
            { "width", "400" },
            { "height", "200" },
            { "left", "0" }, //(new_spawn_coords[0] + 200*(zoom-1)).ToString()
            { "top",  "0" }, //(new_spawn_coords[1] + 200*(zoom-1)).ToString()
            { "fontSize", "20"},
            { "borders", "0,0,0,0" },
            { "bold", "t" },
            { "content", "Empty Notebook... \n\n Please load a notebook or create a new one. \n\n Contents of this notebook will not be saved." }
        };
        // load note, increment note ID, and add name to notebook file
        loadBlock(new_note, "note", true);

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

    // add notebook
    public void addNotebookHandler(object sender, RoutedEventArgs e){

        var notebook = new NotebookButton {
            name = "",
            selected="f"
        };
        notebooks_container.Children.Add(notebook);
        Notebooks.Add(notebook);

        // subscribe to click
        var selected_note = notebook.GetObservable(NotebookButton.SelectedProperty);
        selected_note.Subscribe(value => {
                if(value=="t"){
                    foreach(NotebookButton b in Notebooks){
                        if(b != notebook)
                            b.selected = "f";
                            b.refresh();
                    }    
                    CleanCanvas();             
                    loadNoteBook(notebook.name+".txt");
                }
            });
    }
    // delete notebook
    public void deleteNotebookHandler(object sender, RoutedEventArgs e){
        string to_delete = current_notebook;
        CleanCanvas();
        current_notebook = "always_empty.txt";
        loadNoteBook(current_notebook);
        for(int i = Notebooks.Count-1; i>=0; i--){
            if(Notebooks[i].name+".txt" == to_delete){
                notebooks_container.Children.Remove(Notebooks[i]);
                Notebooks.RemoveAt(i);
                File.Delete(System.IO.Path.Combine(NotesDir, to_delete));
            }
        }
        // if(last_notebook != ""){
        //     current_notebook = last_notebook;
        //     loadNoteBook(current_notebook);
        //     return;
        // }
        // if(Notebooks.Count >1){
        //     current_notebook = Notebooks[1].name;
        //     loadNoteBook(current_notebook);
        //     return;
        // }
    }
    // add a new note
    public void addNoteHandler(object sender, RoutedEventArgs e){
        var object_name = (sender as Button)?.Name;
        if (object_name == null)
            return;
        // create name of note including file extension
        string name = "note" + NoteBlockID.ToString();
        // create dictionary of default values
        // debugger.Text = (this.Width).ToString();
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
        if(mouse_coords.X < 250 || mouse_coords.X > this.Width-250 || mouse_coords.Y < 65)
            return;
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
        if(mouse_coords.X < 250 || mouse_coords.X > this.Width-250 || mouse_coords.Y < 65)
            return;
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
        mouse_coords = args.GetPosition(main_container);
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
    public void CleanCanvas(){
        for(int i = Notes.Count-1; i>=0; i--){
            note_previews_container.Children.Remove(Previews[i]);
            canvas_container.Children.Remove(Notes[i]);
            Previews.RemoveAt(i);
            Notes.RemoveAt(i);
            current_notebook = "";
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