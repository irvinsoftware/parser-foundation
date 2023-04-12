namespace Irvin.Parser
{
    public class SubgroupSettings
    {
        public string StartSymbol { get; set; }
        public string EscapeSymbol { get; set; }
        public string EndSymbol { get; set; }

        public override string ToString()
        {
            return $"{StartSymbol}...{EndSymbol}";
        }
    }
}