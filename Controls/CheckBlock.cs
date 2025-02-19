using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Shapes;
using CustomControl.Helpers;

namespace CustomControl.Controls;

public partial class CheckBlock : UserControl{

    bool isPressedDrag = false;
    bool isPressedResize = false;

    int[] thicknesses = {0, 0, 0, 0};

    double x_offset = 0;
    double y_offset = 0;

    public List<Rectangle> rects = new List<Rectangle>();
    public List<PathIcon> icons = new List<PathIcon>();

    public static readonly StyledProperty<string> NoteNameProperty =
        AvaloniaProperty.Register<CheckBlock, string>(nameof(id));

    public static readonly StyledProperty<string> NoteContentProperty =
        AvaloniaProperty.Register<CheckBlock, string>(nameof(blockContent));

    public static readonly StyledProperty<int[]> SizeProperty =
        AvaloniaProperty.Register<CheckBlock, int[]>(nameof(size));

    public static readonly StyledProperty<Window> WindowProperty =
        AvaloniaProperty.Register<CheckBlock, Window>(nameof(window));

    public static readonly StyledProperty<int> CustomFontSizeProperty =
        AvaloniaProperty.Register<CheckBlock, int>(nameof(fontSize));

    // id is also file name
    public string id{
        get => GetValue(NoteNameProperty);
        set => SetValue(NoteNameProperty, value);
    }
    public string blockContent{
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

    public CheckBlock()
    {
        InitializeComponent();
        DataContext=this;

        // Add all rectangles to list
        // font buttons
        // rects.Add(bold_rect);
        // rects.Add(font_up); rects.Add(font_down); 
        // border buttons
        icons.Add(border_rect_l); icons.Add(border_rect_t); icons.Add(border_rect_r); icons.Add(border_rect_b);
        // drag, resize, delete
        icons.Add(add_box_rect);
        rects.Add(drag_rect_top); icons.Add(delete_rect); rects.Add(drag_rect_bottom); icons.Add(resize_rect);
    }
    public void OnFocusHandler(object sender, GotFocusEventArgs args){
        OnFocus();
    }
    public void OnFocus(){
        foreach(Rectangle r in rects) r.Height = 10;
        foreach(PathIcon r in icons) r.Height = 8;
        this.ZIndex = 2;
    }
    public void OffFocusHandler(object sender, RoutedEventArgs args){
        foreach(Rectangle r in rects) r.Height = 0;
        foreach(PathIcon r in icons) r.Height = 0;
        this.ZIndex = 1;
    }
    public void OnPointerPressed(object sender, PointerPressedEventArgs args){
        var object_name = (sender as Rectangle)?.Name;
        if (object_name == null)
            object_name = (sender as PathIcon)?.Name;
        if (object_name == null)
            return;

        Avalonia.Point position = args.GetCurrentPoint(this).Position;
        x_offset = position.X;
        y_offset = position.Y;

        if(object_name.StartsWith("border_rect")){
            thicknesses = ObjectTools.HandleBorder(object_name, sender, thicknesses);
            border.BorderThickness = new Thickness(thicknesses[0], thicknesses[1], thicknesses[2], thicknesses[3]);
        }

        switch(object_name){
            case "drag_rect_top":
                isPressedDrag = true;
                break;
            case "resize_rect":
                isPressedResize = true;
                break;
            case "add_box_rect":
                check_container.Children.Add(new checkTile());
                break;
        }
    }
    public void OnPointerReleased(object sender, PointerReleasedEventArgs args){
        if (Parent == null)
            return;
        isPressedDrag = false;
        isPressedResize = false;
    }
    protected override void OnPointerMoved(PointerEventArgs args){
        if (Parent == null)
            return;

        if (isPressedResize)
            ObjectTools.HandleResize(this, main_grid, size, args, resize_rect);

        if(isPressedDrag)
            ObjectTools.HandleDrag(this, main_grid, x_offset, y_offset, args);
        
    }
    public void DeleteThis(object sender, PointerReleasedEventArgs args){
        if (Parent is Canvas canvas)
            canvas.Children.Remove(this);
    }
}