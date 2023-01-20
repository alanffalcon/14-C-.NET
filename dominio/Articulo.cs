using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class Articulo
    {
        // Propiedades

        public int Id { get; set; }
        [DisplayName("Codigo")]
        public string CodigoArticulo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string UrlImagen { get; set; }
        public decimal Precio { get; set; }

        // Elementos
        public Marca Marca { get; set; }
        public Categoria Categoria { get; set; }


    }
}
