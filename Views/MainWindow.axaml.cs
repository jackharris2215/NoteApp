using System;
using System.Collections.Generic;
using System.IO; 
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;


using CustomControl.Controls;
using CustomControl.Extensions;
using CustomControl.Helpers;

namespace CustomControl.Views;

public partial class MainWindow : Window
{
    List<UserControl> Blocks = new List<UserControl>();
    // List<CheckBlock> checkBlocks = new List<CheckBlock>();

    Dictionary<string, string> params_dict = new Dictionary<string, string>();
    // string notes_dir = "NoteBooks";
    string current_notebook = "LameNoteBook";
    int NoteBlockID = 0;
    double zoom = 1;
    double[] new_spawn_coords = [10.0, 10.0];

    public bool dragging = false;
    // bool dragged = false;
    List<Avalonia.Point> offsets = new List<Avalonia.Point>();
    Avalonia.Point grid_center_point = new Avalonia.Point(0,0);
    // public Avalonia.Point grid_center = new Avalonia.Point(0.0, 0.0);

    public MainWindow()
    {
        // setup
        // Initialize window
        InitializeComponent();
        this.AttachDevTools();

        Dictionary<string, string> new_note = new Dictionary<string, string>
        {
            { "id", "checkBlockTest"},
            { "width", "300" },
            { "height", "200" },
            { "left", "20" }, //(new_spawn_coords[0] + 200*(zoom-1)).ToString()
            { "top",  "20" }, //(new_spawn_coords[1] + 200*(zoom-1)).ToString()
            { "fontSize", "20"},
            { "content", "" }
        };
        loadBlock(new_note, "check");

    }
    public void loadBlock(Dictionary<string, string> parameters, string blockType){
        int width = Int32.Parse(parameters["width"]);
        int height = Int32.Parse(parameters["height"]);
        double left = double.Parse(parameters["left"]);
        double top = double.Parse(parameters["top"]);
        int font_size = Int32.Parse(parameters["fontSize"]);
        string content = parameters["content"];

        UserControl? block = null;

        switch(blockType){
            case "note":
                block = new NoteBlock {
                    id = parameters["id"],
                    size = new int[2] {width, height},
                    fontSize = font_size,
                    noteContent = content,
                    window = this
                };
                break;
            case "check":
                block = new CheckBlock {
                    id = parameters["id"],
                    size = new int[2] {width, height},
                    fontSize = font_size,
                    blockContent = content,
                    window = this
                };
                break;
        }

        if(block == null)
            return;

        Canvas main = canvas_container;
        Canvas.SetLeft(block, left);
        Canvas.SetTop(block, top);
        main.Children.Insert(0, block);
        Blocks.Add(block);
        (block as dynamic).OnFocus();
    } 

    // add a new note
    public void addNoteHandler(object sender, RoutedEventArgs e){
        var object_name = (sender as Button)?.Name;
        if (object_name == null)
            return;
        // create name of note including file extension
        string name = current_notebook.Substring(0,current_notebook.Length-4).ToLower() + 
                          "note" + 
                          NoteBlockID.ToString() + 
                          ".txt";
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
            { "content", "Note" }
        };
        // load note, increment note ID, and add name to notebook file
        switch(object_name){
            case "add_check":
                loadBlock(new_note, "check");
                break;
            case "add_note":
                loadBlock(new_note, "note");
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
        foreach (UserControl n in Blocks){
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

        for(int i = 0; i < Blocks.Count; i++){
            pos = args.GetPosition(canvas_container) - offsets[i+1];
            Blocks[i].SetValue(Canvas.LeftProperty, Math.Round(pos.X));
            Blocks[i].SetValue(Canvas.TopProperty, Math.Round(pos.Y));
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
}