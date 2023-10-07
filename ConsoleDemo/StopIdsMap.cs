namespace ConsoleDemo
{
    public static class StopIdsMap
    {
        public static IDictionary<int, int> SIPtoRozkladzik { get; set; } = new Dictionary<int, int>
        {
            { 64301, 679 },
            { 31005, 688 }
        };
        public static IDictionary<int, int> RozkladzikToSIP { get; set; } = new Dictionary<int, int>
        {
            { 679, 64301 },
            { 688, 31005 }
        };
    }
}
