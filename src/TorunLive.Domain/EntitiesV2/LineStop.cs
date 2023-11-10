namespace TorunLive.Domain.EntitiesV2
{
    public class LineStop
    {
        public int Id { get; set; }
        public string LineId { get; set; }
        public int DirectionId { get; set; }
        public string StopId { get; set; }
        public int StopOrder {  get; set; }
        public bool IsOnDemand { get; set; }
        public int? TimeToNextStop { get; set; }

        public Line Line { get; set; }
        public Stop Stop { get; set; }
        public Direction Direction { get; set; }

        public ICollection<LineStopTime> LineStopTimes { get; set; }
    }
}
