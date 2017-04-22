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

        public bool IsLessThan(Accuracy other)
        {
            if (ReferenceEquals(this, other)) return false;
            if (ReferenceEquals(other, null)) return false;

            return Vertical < other.Vertical && Horizontal < other.Horizontal;
        }
    }
}
