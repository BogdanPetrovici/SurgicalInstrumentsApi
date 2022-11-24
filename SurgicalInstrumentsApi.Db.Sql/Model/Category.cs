using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgicalInstrumentsApi.Db.Sql.Model
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<Subcategory> Subcategories { get; set; }
    }
}
