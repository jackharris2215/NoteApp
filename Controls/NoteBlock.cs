using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;

namespace CustomControl.Controls;

public partial class NoteBlock : UserControl
{
    public double x_offset = 0;
    public double y_offset = 0;
    public double grid_center_x = 0;
    public double grid_center_y = 0;

    public List<Rectangle> rects = new List<Rectangle>();
    public int[] thicknesses = [0, 0, 0, 0];
    
    public bool edited = true;
    public bool isPressedDrag = false;
    public bool isPressedResize = false;
    
    public static readonly StyledProperty<string> NoteNameProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(id));

    public static readonly StyledProperty<string> NoteContentProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(noteContent));

    public static readonly StyledProperty<int[]> SizeProperty =
        AvaloniaProperty.Register<NoteBlock, int[]>(nameof(size));

    public static readonly StyledProperty<Window> WindowProperty =
        AvaloniaProperty.Register<NoteBlock, Window>(nameof(window));

    public static readonly StyledProperty<int> CustomFontSizeProperty =
        AvaloniaProperty.Register<NoteBlock, int>(nameof(fontSize));

    // id is also file name
    public string id{
        get => GetValue(NoteNameProperty);
        set => SetValue(NoteNameProperty, value);
    }
    public string noteContent{
        get => GetValue(NoteContentProperty);
        set => SetValue(NoteContentProperty, value);
    }
    public int[] size{
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    public Window window{
        get => GetValue(WindowProperty);
        set => SetValue(WindowProperty, value);
    }
    public int fontSize{
        get => GetValue(CustomFontSizeProperty);
        set => SetValue(CustomFontSizeProperty, value);
    }
    
    public NoteBlock(){
        InitializeComponent();
        DataContext=this;

        // Add all rectangles to list
        // font buttons
        rects.Add(bold_rect);
        rects.Add(font_up); rects.Add(font_down); 
        // border buttons
        rects.Add(border_rect_l); rects.Add(border_rect_t); rects.Add(border_rect_r); rects.Add(border_rect_b);
        // drag, resize, delete
        rects.Add(drag_rect_top); rects.Add(delete_rect); rects.Add(drag_rect_bottom); rects.Add(resize_rect);

    }
    

    public void OnFocusHandler(object sender, GotFocusEventArgs args){
        OnFocus();
    }
    public void OnFocus(){
        foreach(Rectangle r in rects) r.Height = 10;
        this.ZIndex = 2;
    }
    public void OffFocusHandler(object sender, RoutedEventArgs args){
        foreach(Rectangle r in rects) r.Height = 0;
        this.ZIndex = 1;
    }

    public void KeyStrokeHandler(object sender, KeyEventArgs args){
        if(!edited) edited=true;
    }

    public void OnPointerPressed(object sender, PointerPressedEventArgs args){
        
        if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        if (Parent == null)
            return;
        if (sender == null)
            return;

        
        var object_name = (sender as Rectangle)?.Name;
        if (object_name == null)
            return;

        // if border rect -> do border things
        if(object_name.StartsWith("border_rect")){
            char[] sides = ['l', 't', 'r', 'b'];
            int index = Array.IndexOf(sides, object_name[object_name.Length-1]);
            bool on = thicknesses[index]==2;
            thicknesses[index] = on?0:2;
            var rectangle = sender as Rectangle;
            if (rectangle != null)
                rectangle.Fill = on ? Brushes.Orange : Brushes.Yellow;
            note_box.BorderThickness = new Thickness(thicknesses[0], thicknesses[1], thicknesses[2], thicknesses[3]);
        }

        // get relative grid center for dragging in grid
        var grid_center_object = window?.FindControl<Rectangle>("grid_center_object");
        if (grid_center_object != null){
            grid_center_x = (double)grid_center_object.GetValue(Canvas.LeftProperty);
            grid_center_y = (double)grid_center_object.GetValue(Canvas.TopProperty);
        }

        Avalonia.Point position = args.GetCurrentPoint(this).Position;
        x_offset = position.X;
        y_offset = position.Y;

        // drag, resize
        switch(object_name){
            case "drag_rect_top":
                isPressedDrag = true;
                edited = true;
                break;
            case "resize_rect":
                isPressedResize = true;
                edited = true;
                break;
            case "font_up":
                fontSize = Math.Min(75, fontSize+5);
                break;
            case "font_down":
                fontSize = Math.Max(5, fontSize-5);
                break;
            case "bold_rect":
                var fontWeight = note_box.FontWeight == FontWeight.Bold ? FontWeight.Normal : FontWeight.Bold;
                note_box.FontWeight = fontWeight;
                break;
        }
    }
    public void OnPointerReleased(object sender, PointerReleasedEventArgs args){
        if (Parent == null)
            return;
        
        edited = true;
        isPressedDrag = false;
        isPressedResize = false;
    }

    protected override void OnPointerMoved(PointerEventArgs args){
        if (Parent == null)
            return;
        if (isPressedResize){
            Avalonia.Point pos = args.GetPosition(resize_rect);
            double x_pos = (double)pos.X;
            double y_pos = (double)pos.Y;
            bool changed = false;

            if(x_pos > 10) {
                size[0]+=10;
                changed = true;
            }
            
            if(y_pos > 10) {
                size[1]+=10; 
                changed = true;
            }
            
            if(x_pos < -10) {
                size[0]-=10;
                changed = true;
            }
            
            if(y_pos < -10) {
                size[1]-=10; 
                changed = true;
            }
            
            if(changed){
                this.Width = size[0];
                main_grid.Width = size[0];
                main_grid.ColumnDefinitions.Clear();
                main_grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                this.Height = size[1];
                main_grid.Height = size[1];
                main_grid.RowDefinitions.Clear();
                main_grid.RowDefinitions.Add(new RowDefinition(new GridLength(10)));
                main_grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                main_grid.RowDefinitions.Add(new RowDefinition(new GridLength(10)));
            }

            
        }
        if (isPressedDrag){  
            Avalonia.Point pos = args.GetPosition(main_grid);
            double x_pos = Math.Round((double)pos.X-x_offset);
            double y_pos = Math.Round((double)pos.Y-y_offset);

            if(x_pos > 10)
                this.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty)+10);
            if(x_pos < -10)
                this.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty)-10);
            if(y_pos > 10)
                this.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty)+10);
            if(y_pos < -10)
                this.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty)-10);
            

            // noteContent = x_pos.ToString() + ", " + y_pos.ToString()
            //                 + "\n" + 
            //                 args.GetPosition((Visual)Parent).ToString()
            //                 + "\n" + 
            //                 x_offset.ToString() + ", " + y_offset.ToString()
            //                 + "\n" + 
            //                 this.GetValue(Canvas.LeftProperty) + ", " + this.GetValue(Canvas.TopProperty)
            //                 + "\n" + 
            //                  ((Canvas)Parent).Bounds.Width.ToString() + ", " + ((Canvas)Parent).Bounds.Height.ToString();
            // noteContent = this.GetValue(Canvas.LeftProperty).ToString() + ", " + this.GetValue(Canvas.TopProperty).ToString();


        }
    }
    public void DeleteThis(object sender, PointerReleasedEventArgs args){
        if (Parent is Canvas canvas)
            canvas.Children.Remove(this);
    }
}