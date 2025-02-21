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

public partial class NoteBlock : UserControl
{
    public double x_offset = 0;
    public double y_offset = 0;

    public List<Rectangle> rects = new List<Rectangle>();
    public List<PathIcon> icons = new List<PathIcon>();

    // public int[] thicknesses = [0, 0, 0, 0];
    
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

    public static readonly StyledProperty<int[]> CustomBordersProperty =
        AvaloniaProperty.Register<NoteBlock, int[]>(nameof(borders));

    public static readonly StyledProperty<bool> IsBoldProperty =
        AvaloniaProperty.Register<NoteBlock, bool>(nameof(bold));

    public static readonly StyledProperty<NotePreview> PreviewProperty =
        AvaloniaProperty.Register<NoteBlock, NotePreview>(nameof(fontSize));

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
    public int[] borders{
        get => GetValue(CustomBordersProperty);
        set => SetValue(CustomBordersProperty, value);
    }
    public bool bold{
        get => GetValue(IsBoldProperty);
        set => SetValue(IsBoldProperty, value);
    }
    public NotePreview preview{
        get => GetValue(PreviewProperty);
        set => SetValue(PreviewProperty, value);
    }
    
    public NoteBlock(){
        InitializeComponent();
        DataContext=this;

        // Add all rectangles to list
        // font buttons
        icons.Add(bold_rect);
        icons.Add(font_up); icons.Add(font_down); 
        // border buttons
        icons.Add(border_rect_l); icons.Add(border_rect_t); icons.Add(border_rect_r); icons.Add(border_rect_b);
        // drag, resize, delete
        rects.Add(drag_rect_top); icons.Add(delete_rect); rects.Add(drag_rect_bottom); icons.Add(resize_rect);

    }
    
    public void refresh(){
        // alternative to binding initial value
        note_box.BorderThickness = new Thickness(borders[0], borders[1], borders[2], borders[3]);
        var fontWeight = bold == true ? FontWeight.Bold : FontWeight.Normal;
        note_box.FontWeight = fontWeight;
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
        OffFocus();
    }
    public void OffFocus(){
        foreach(Rectangle r in rects) r.Height = 0;
        foreach(PathIcon r in icons) r.Height = 0;
        preview.sub_heading.Text = noteContent.Length>10?noteContent.Substring(0, 10)+"...":noteContent;
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
            object_name = (sender as PathIcon)?.Name;
        if (object_name == null)
            return;

        // if border rect -> do border things
        if(object_name.StartsWith("border_rect")){
            borders = ObjectTools.HandleBorder(object_name, sender, borders);
            note_box.BorderThickness = new Thickness(borders[0], borders[1], borders[2], borders[3]);
        }

        Avalonia.Point position = args.GetCurrentPoint(this).Position;
        x_offset = position.X;
        y_offset = position.Y;

        // drag, resize
        switch(object_name){
            case "drag_rect_top":
                isPressedDrag = true;
                break;
            case "resize_rect":
                isPressedResize = true;
                break;
            case "font_up":
                fontSize = Math.Min(75, fontSize+5);
                break;
            case "font_down":
                fontSize = Math.Max(5, fontSize-5);
                break;
            case "bold_rect":
                var fontWeight = note_box.FontWeight == FontWeight.Bold ? FontWeight.Normal : FontWeight.Bold;
                bold = bold == true ? false : true;
                note_box.FontWeight = fontWeight;
                break;
        }
        edited = true;
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

        if (isPressedResize)
            ObjectTools.HandleResize(this, main_grid, size, args, resize_rect);

        if(isPressedDrag)
            ObjectTools.HandleDrag(this, main_grid, x_offset, y_offset, args);
        
    }
    public void DeleteThis(object sender, PointerReleasedEventArgs args){
        if (Parent is Canvas canvas){
            canvas.Children.Remove(this);
        }
        if (preview.Parent is StackPanel s){
            s.Children.Remove(preview);
        }
    }
}