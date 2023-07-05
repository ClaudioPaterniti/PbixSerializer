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

    class Pb_relation
    {
        public string fromTable;
        public string fromColumn;
        public string toTable;
        public string toColumn;
        public string fromCardinality;
        public string toCardinality;
        public bool isActive;
        public string CrossFilteringBehavior;

        public Pb_relation(Relationship r)
        {
            if (r.Type == RelationshipType.SingleColumn)
            {
                SingleColumnRelationship sr = (SingleColumnRelationship)r;
                this.fromTable = sr.FromTable.Name;
                this.fromColumn = sr.FromColumn.Name;
                this.toTable = sr.ToTable.Name;
                this.toColumn = sr.ToColumn.Name;
                this.fromCardinality = sr.FromCardinality.ToString();
                this.toCardinality = sr.ToCardinality.ToString();
                this.isActive = sr.IsActive;
                this.CrossFilteringBehavior = sr.CrossFilteringBehavior.ToString();
            }
        }
    }
    class Pb_table
    {
        public string name;
        public string description;
        public bool isHidden;
        public bool isPrivate;
        public List<Pb_partition> partitions;
        public List<Pb_relation> relationships;
        public Pb_table(Table t, List<Relationship> relations)
        {
            this.name = t.Name;
            this.description = t.Description;
            this.isHidden = t.IsHidden;
            this.isPrivate = t.IsPrivate;
            this.partitions = new List<Pb_partition>();
            foreach (Partition p in t.Partitions)
                this.partitions.Add(new Pb_partition(p));
            this.relationships = new List<Pb_relation>();
            if (relations!= null)
            {
                foreach (Relationship r in relations)
                    this.relationships.Add(new Pb_relation(r));
            }
            
        }
    }
}
