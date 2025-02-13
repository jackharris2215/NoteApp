using System;
using Avalonia;
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
    
    public bool edited = true;
    public bool remove = false;
    public bool isPressedDrag = false;
    public bool isPressedResize = false;
    
    public static readonly StyledProperty<string> NoteNameProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(id));
    
    public static readonly StyledProperty<Window> WindowReference =
        AvaloniaProperty.Register<NoteBlock, Window>(nameof(window));

    public static readonly StyledProperty<string> NoteContentProperty =
        AvaloniaProperty.Register<NoteBlock, string>(nameof(noteContent));

    public static readonly StyledProperty<int[]> SizeProperty =
        AvaloniaProperty.Register<NoteBlock, int[]>(nameof(size));

    // id is also file name
    public string id{
        get => GetValue(NoteNameProperty);
        set => SetValue(NoteNameProperty, value);
    }
    public Window window{
        get => GetValue(WindowReference);
        set => SetValue(WindowReference, value);
    }
    public string noteContent{
        get => GetValue(NoteContentProperty);
        set => SetValue(NoteContentProperty, value);
    }
    public int[] size{
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    
    public NoteBlock(){
        InitializeComponent();
        DataContext=this;
    }

    public void OnFocusHandler(object sender, GotFocusEventArgs args){
        //drag_rect_top delete_rect drag_rect_bottom resize_rect
        drag_rect_top.Height = 10;
        delete_rect.Height = 10;
        drag_rect_bottom.Height = 10;
        resize_rect.Height = 10;
        this.ZIndex = 2;
    }
    public void OffFocusHandler(object sender, RoutedEventArgs args){
        drag_rect_top.Height = 0;
        delete_rect.Height = 0;
        drag_rect_bottom.Height = 0;
        resize_rect.Height = 0;
        this.ZIndex = 1;
    }

    public void KeyStrokeHandler(object sender, KeyEventArgs args){
        if(!edited) edited=true;
    }

    public void OnPointerPressed(object sender, PointerPressedEventArgs args){
        // noteContent = (sender as Rectangle).Name.ToString();
        if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        if (Parent == null)
            return;
        if (sender == null)
            return;

        var grid_center_object = window?.FindControl<Rectangle>("grid_center_object");
            if (grid_center_object != null){
                grid_center_x = (double)grid_center_object.GetValue(Canvas.LeftProperty);
                grid_center_y = (double)grid_center_object.GetValue(Canvas.TopProperty);
            }

        switch((sender as Rectangle)?.Name){
            case "drag_rect_top":
                isPressedDrag = true;
                Avalonia.Point position = args.GetCurrentPoint(sender as Control).Position;
                x_offset = position.X;
                y_offset = position.Y;
                edited = true;
                break;
            case "resize_rect":
                isPressedResize = true;
                edited = true;
                break;
        }
    }
    public void OnPointerReleased(object sender, PointerReleasedEventArgs args){
        if (Parent == null)
            return;
        
        if(isPressedResize){
            int w = (int)note_box.Width;
            int h = (int)note_box.Height;

            size[0] = w;
            size[1] = h+20;
            size[2] = h;
            size[3] = w-10;
        }
        
        edited = true;
        isPressedDrag = false;
        isPressedResize = false;
    }

    protected override void OnPointerMoved(PointerEventArgs args){
        if (Parent == null)
            return;
        if (isPressedResize){
            Avalonia.Point pos = args.GetPosition(this);
            double x_pos = (double)pos.X;
            double y_pos = (double)pos.Y;
            if(x_pos >= 50 && (x_pos) % 10 == 0){
                note_box.Width = x_pos;
                drag_rect_top.Width = x_pos-10;
                drag_rect_bottom.Width = x_pos-10;
            }
            if(y_pos >= 50 && (y_pos) % 10 == 0){
                note_box.Height = y_pos-20;
            }
            
        }
        if (isPressedDrag){  
            double x_pos = (double)args.GetPosition((Visual)Parent).X-x_offset;
            double y_pos = (double)args.GetPosition((Visual)Parent).Y-y_offset;
            if(x_pos < ((Canvas)Parent).Bounds.Width-x_offset && x_pos > 0-x_offset && (x_pos-grid_center_x) % 10 == 0)
                this.SetValue(Canvas.LeftProperty, x_pos);

            if(y_pos < ((Canvas)Parent).Bounds.Height-y_offset && y_pos > 0-y_offset && (y_pos-grid_center_y) % 10 == 0)
                this.SetValue(Canvas.TopProperty, y_pos);

            
        }
        
    }
    public void DeleteThis(object sender, PointerReleasedEventArgs args){
        if (Parent is Canvas canvas)
            canvas.Children.Remove(this);
    }
}