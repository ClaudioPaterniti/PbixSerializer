using Microsoft.AnalysisServices.Tabular;
using System.Collections.Generic;

namespace PbixSerializer
{
    class Pb_column
    {
        public string name;
        public string type;
        public string sourceColumn;
        public string dataType;
        public string[] expression;
        public string summarizeBy;
        public string formatString;
        public List<Pb_annotation> annotations;
        static List<string> ann_to_include = new List<string>{"Format"};
        public Pb_column(Column c)
        {
            this.name = c.Name;
            this.type = c.Type.ToString();
            this.dataType = c.DataType.ToString();
            if (this.type == "Calculated")
            {
                CalculatedColumn cColumn = (CalculatedColumn)c;
                this.expression = cColumn.Expression.Split('\n');
            }
            else if (this.type == "Data")
            {
                DataColumn dColumn = (DataColumn)c;
                this.sourceColumn = dColumn.SourceColumn;
            }
            this.summarizeBy = c.SummarizeBy.ToString();
            this.formatString = c.FormatString;
            this.annotations = new List<Pb_annotation>();
            foreach (Annotation a in c.Annotations)
            {
                if (ann_to_include.Contains(a.Name))
                    this.annotations.Add(new Pb_annotation(a));
            }
        }
    }
}
