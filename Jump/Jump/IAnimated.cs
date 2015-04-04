namespace Jump
{
    public interface IAnimated
    {
        float TotalElapsed { get; set; }
        float TimePerFrame { get; set; }
        int Frame { get; set; }
        int FrameCount { get; set; }
    }
}