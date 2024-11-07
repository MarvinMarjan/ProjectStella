using Stella.UI.Elements.Shapes;


namespace Stella.UI.Elements;


public abstract class PopupElement : RectangleElement
{
    public TextElement Title { get; }
    
    
    public PopupElement(string title) : base(null, new(), new())
    {
        Size = new(300, 200);
        Color = new(50, 50, 50, 200);
        Alignment = AlignmentType.Center;

        Title = new(this, new(), 20, title)
        {
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Top,
            AlignmentMargin = new(0, 10)
        };
        
        // not visible by default
        Hide();
    }
}