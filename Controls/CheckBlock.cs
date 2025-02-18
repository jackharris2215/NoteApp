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
    double x_offset = 0;
    double y_offset = 0;

    public List<Rectangle> rects = new List<Rectangle>();

        public static readonly StyledProperty<string> NoteNameProperty =
        AvaloniaProperty.Register<CheckBlock, string>(nameof(id));

    public static readonly StyledProperty<string[]> NoteContentProperty =
        AvaloniaProperty.Register<CheckBlock, string[]>(nameof(noteContent));

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
    public string[] noteContent{
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
        // rects.Add(border_rect_l); rects.Add(border_rect_t); rects.Add(border_rect_r); rects.Add(border_rect_b);
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
    public void OnPointerPressed(object sender, PointerPressedEventArgs args){
        var object_name = (sender as Rectangle)?.Name;
        if (object_name == null)
            return;

        Avalonia.Point position = args.GetCurrentPoint(this).Position;
        x_offset = position.X;
        y_offset = position.Y;

        switch(object_name){
            case "drag_rect_top":
                isPressedDrag = true;
                break;
        }
    }
    public void OnPointerReleased(object sender, PointerReleasedEventArgs args){
        if (Parent == null)
            return;
        isPressedDrag = false;
    }
    protected override void OnPointerMoved(PointerEventArgs args){
        if (Parent == null)
            return;

        // if (isPressedResize)
        //     ObjectTools.HandleResize(this, main_grid, size, args, resize_rect);

        if(isPressedDrag)
            ObjectTools.HandleDrag(this, main_grid, x_offset, y_offset, args);
        
    }
    public void DeleteThis(object sender, PointerReleasedEventArgs args){
        if (Parent is Canvas canvas)
            canvas.Children.Remove(this);
    }
}