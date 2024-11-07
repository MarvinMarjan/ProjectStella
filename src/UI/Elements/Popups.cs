namespace Stella.UI.Elements;


public class ProgressBarPopup : PopupElement
{
    public ProgressBarElement ProgressBar { get; }
    
    public bool CloseOnComplete { get; set; }
    
    
    public ProgressBarPopup(string title) : base(title)
    {
        CloseOnComplete = true;
        
        ProgressBar = new(this, new(), new(Size.X - 30, 20))
        {
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Bottom,
            AlignmentMargin = new(0, -10)
        };
    }


    public override void Update()
    {
        if (!Visible)
            return;
        
        // TODO: Add a way to fully delete an element
        
        if (CloseOnComplete && ProgressBar.IsAtMax)
            Hide();
        
        base.Update();
    }
}