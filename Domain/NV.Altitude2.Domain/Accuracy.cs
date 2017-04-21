namespace NV.Altitude2.Domain
{
    public sealed class Accuracy
    {
        public decimal Horizontal { get; }

        public decimal Vertical { get; }

        public Accuracy(decimal horizontal, decimal vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }
}
