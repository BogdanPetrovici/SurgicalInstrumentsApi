using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgicalInstrumentsApi.Db.Sql.Model
{
    public class Subcategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubcategoryID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public Category Category { get; set; }
    }
}
