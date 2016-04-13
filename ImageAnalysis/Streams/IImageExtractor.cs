namespace ImageAnalysis.Streams
{
    public interface IImageExtractor
    {
        event ImageExtractor.FramerateBroadcastEvent framerateBroadcast;
        event ImageExtractor.ImageCreatedEvent imageCreated;

        void Run();
        void Run(int minutes);
        void Run(bool singleImage);
        bool asyncrohous { get; set; }
    }
}