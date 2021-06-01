namespace L2Data2Code.BaseGenerator.Entities
{
    public class Relation
    {
        public string Table { get; set; }
        public string Column { get; set; }
        public string RelatedColumn { get; set; }
        public bool CanBeNull { get; set; }
        public string DbTable { get; set; }
        public string DbColumn { get; set; }
        public string DbRelatedColumn { get; set; }
    }

}
