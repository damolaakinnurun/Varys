using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Varys.Elastic.Entities
{
    [ElasticType(Name = "Address_Info", IdProperty = "Id")]
    public class EsAddress
    {
        [ElasticProperty(Name = "Id", Index = FieldIndexOption.NotAnalyzed)]
        public string Id { get; set;}
        [ElasticProperty(Name = "Application Environment", NumericType = NumberType.Integer)]
        public int Environment { get; set; }
        [ElasticProperty(Name = "Application Name", Index = FieldIndexOption.NotAnalyzed)]
        public string AppName { get; set; }
        [ElasticProperty(Name = "Web Address", Index = FieldIndexOption.NotAnalyzed)]
        public string Address { get; set; }
   
    }
}
