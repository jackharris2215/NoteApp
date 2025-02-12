using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using MouseButton = Avalonia.Remote.Protocol.Input.MouseButton;

namespace CustomControl.Controls;

public partial class NoteBlock : UserControl
{
    public double x_offset = 0;
    public double y_offset = 0;
    // public int width = 400;
    // public int height = 200;
    // public int text_box_height = 280;
    // public int bottom_bar_width = 190;
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

    public void OnPointerPressed(object sender, PointerPressedEventArgs args){
        // noteContent = (sender as Rectangle).Name.ToString();
        if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;
        if (Parent == null)
            return;
        if (sender == null)
            return;

        switch((sender as Rectangle)?.Name){
            case "drag_rect_top":
                isPressedDrag = true;
                Avalonia.Point position = args.GetCurrentPoint(sender as Control).Position;
                x_offset = position.X;
                y_offset = position.Y;
                break;
            case "resize_rect":
                isPressedResize = true;
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
            if(x_pos > 50){
                note_box.Width = x_pos;
                drag_rect_top.Width = x_pos;
                drag_rect_bottom.Width = x_pos-10;
            }
            if(y_pos > 50){
                note_box.Height = y_pos-20;
            }
            
        }
        if (isPressedDrag){
            double x_pos = (double)args.GetPosition((Visual)Parent).X-x_offset;
            double y_pos = (double)args.GetPosition((Visual)Parent).Y-y_offset;
            if(x_pos < ((Canvas)Parent).Bounds.Width-x_offset && x_pos > 0-x_offset)
                this.SetValue(Canvas.LeftProperty, x_pos);

            if(y_pos < ((Canvas)Parent).Bounds.Height-y_offset && y_pos > 0-y_offset)
                this.SetValue(Canvas.TopProperty, y_pos);
        }
    }
    
}