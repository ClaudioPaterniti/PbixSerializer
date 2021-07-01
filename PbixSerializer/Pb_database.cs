using Microsoft.AnalysisServices.Tabular;
using System.Collections.Generic;

namespace PbixSerializer
{
    class Pb_db_expression
    {
        public string name;
        public string kind;
        public string expression;

        public Pb_db_expression(NamedExpression ex)
        {
            this.name = ex.Name;
            this.kind = ex.Kind.ToString();
            this.expression = ex.Expression;
        }
    }
    class Pb_database
    {
        public List<Pb_db_expression> expressions;
        public List<Pb_annotation> annotations;
        static List<string> ann_to_include = new List<string>
            {"PBI_QueryOrder", "__PBI_TimeIntelligenceEnabled"};

        public Pb_database(Database db)
        {
            this.expressions = new List<Pb_db_expression>();
            foreach (NamedExpression ex in db.Model.Expressions)
                this.expressions.Add(new Pb_db_expression(ex));
            this.annotations = new List<Pb_annotation>();
            foreach (Annotation a in db.Annotations)
            {
                if (ann_to_include.Contains(a.Name))
                    this.annotations.Add(new Pb_annotation(a));
            }
        }

    }
}
