using Microsoft.AnalysisServices.Tabular;
using System.Collections.Generic;

namespace PbixSerializer
{
    class Pb_measure
    {
        public string name;
        public string dataType;
        public string[] expression;
        public string displayFolder;
        public string formatString;
        public List<Pb_annotation> annotations;
        static List<string> ann_to_include = new List<string> {"Format"};
        public Pb_measure(Measure m)
        {
            this.name = m.Name;
            this.dataType = m.DataType.ToString();
            this.expression = m.Expression.Split('\n');
            this.formatString = m.FormatString;
            this.displayFolder = m.DisplayFolder;
            this.annotations = new List<Pb_annotation>();
            foreach (Annotation a in m.Annotations)
            {
                if (ann_to_include.Contains(a.Name))
                    this.annotations.Add(new Pb_annotation(a));
            }
        }
    }
}
