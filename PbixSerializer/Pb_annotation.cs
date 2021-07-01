using Microsoft.AnalysisServices.Tabular;
namespace PbixSerializer
{
    class Pb_annotation
    {
        public string name;
        public string value;

        public Pb_annotation(Annotation a)
        {
            this.name = a.Name;
            this.value = a.Value;
        }
    }
}
