using Microsoft.AnalysisServices.Tabular;
using System.Collections.Generic;

namespace PbixSerializer
{
    class Pb_partition
    {
        public string name;
        public string type;
        public string[] expression;

        public Pb_partition(Partition p)
        {
            this.name = p.Name;
            this.type = p.SourceType.ToString();
            switch (this.type)
            {
                case "M":
                    MPartitionSource mPartition = (MPartitionSource)p.Source;
                    this.expression = mPartition.Expression.Split('\n');
                    break;
                case "Calculated":
                    CalculatedPartitionSource cPartition = (CalculatedPartitionSource)p.Source;
                    this.expression = cPartition.Expression.Split('\n');
                    break;
                case "Entity":
                    EntityPartitionSource ePartition = (EntityPartitionSource)p.Source;
                    this.expression = ePartition.ExpressionSource.Expression.Split('\n');
                    break;
                case "Query":
                    QueryPartitionSource qPartition = (QueryPartitionSource)p.Source;
                    this.expression = qPartition.Query.Split('\n');
                    break;
                default:
                    break;
            }
        }
    }
    class Pb_table
    {
        public string name;
        public List<Pb_partition> partitions;
        public Pb_table(Table t)
        {
            this.name = t.Name;
            this.partitions = new List<Pb_partition>();
            foreach (Partition p in t.Partitions)
                this.partitions.Add(new Pb_partition(p));
        }
    }
}
