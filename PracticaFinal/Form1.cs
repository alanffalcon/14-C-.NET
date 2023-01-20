using dominio;
using negocio;
using System.ComponentModel;
using System.Data;
using System.Text;


namespace PracticaFinal
{
    public partial class frmArticulo : Form
    {
        private List<Articulo> listaArticulo;

        public frmArticulo()
        {
            InitializeComponent();
        }


        // LOAD
        private void frmArticulo_Load(object sender, EventArgs e)
        {
            cargar();
            cbCampo.Items.Add("Codigo");
            cbCampo.Items.Add("Nombre");
            cbCampo.Items.Add("Precio");
        }

        // BOTONES
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaArticulo alta = new frmAltaArticulo();
            alta.ShowDialog();
            cargar();
        }
        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;

            frmAltaArticulo modificar = new frmAltaArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();

        }
        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }


        // FUNCIONES
        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                listaArticulo = negocio.listar();
                dgvArticulo.DataSource = listaArticulo;
                ocultarColumnas();
                cargarImagen(listaArticulo[0].UrlImagen);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);    
            }
        }
        private void ocultarColumnas()
        {
            dgvArticulo.Columns["UrlImagen"].Visible = false;
            dgvArticulo.Columns["Id"].Visible = false;
            dgvArticulo.Columns["Descripcion"].Visible = false;
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbArticulo.Load(imagen);
            }
            catch (Exception)
            {
                pbArticulo.Load("https://thumb1.shutterstock.com/image-photo/stock-vector-vector-illustration-of-cd-or-dvd-s-in-gray-icon-250nw-574210795.jpg");
            }
        }     
        private void eliminar(bool logico = false)
        {
            ArticuloNegocio negocio= new ArticuloNegocio();
            Articulo seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿Estas seguro que lo queres eliminar?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;

                    if (logico)
                    {
                        negocio.eliminarLogico(seleccionado.Id);
                    }
                    else
                    {
                        negocio.eliminar(seleccionado.Id);
                    }

                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private bool validarFiltro()
        {
            if(cbCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el campo a filtrar.");
                return true;
            }
            if (cbCriterio.SelectedItem.ToString() == "Precio")
            {
                if (string.IsNullOrEmpty(tbFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar un precio.");
                    return true;
                }
                if (!(soloNumeros(tbFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Debes cargar solo numeros.");
                    return true;
                }
            }

            return false;
        }
        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }
        private void btnFiltro_Click(object sender, EventArgs e)
        {
            ArticuloNegocio negocio = new ArticuloNegocio();

            try
            {
                if (validarFiltro())
                {
                    return;
                }

                string campo = cbCampo.SelectedItem.ToString();
                string criterio = cbCriterio.SelectedItem.ToString();
                string filtro = tbFiltroAvanzado.Text;
                dgvArticulo.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        // CAMBIOS
        private void cbCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cbCampo.SelectedItem.ToString();

            if (opcion == "Precio")
            {
                cbCriterio.Items.Clear();
                cbCriterio.Items.Add("Mayor a");
                cbCriterio.Items.Add("Menor a");
                cbCriterio.Items.Add("Igual a");
            }
            else
            {
                cbCriterio.Items.Clear();
                cbCriterio.Items.Add("Comienza con");
                cbCriterio.Items.Add("Termina con");
                cbCriterio.Items.Add("Contiene");
            }
        }
        private void dgvArticulo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulo.CurrentRow!= null)
            {
                Articulo seleccionado = (Articulo)dgvArticulo.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }       
        }
        private void tbFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Articulo> listaFiltrada;
            string filtro = tbFiltro.Text;

            if (filtro.Length >= 3)
            {
                // Agregar mas filtros al filtros rapidos aca...
                listaFiltrada = listaArticulo.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.CodigoArticulo.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaArticulo;
            }

            dgvArticulo.DataSource = null;
            dgvArticulo.DataSource = listaFiltrada;
            ocultarColumnas();

        }
    }
}