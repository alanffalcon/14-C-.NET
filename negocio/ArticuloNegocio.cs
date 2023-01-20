using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dominio;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using System.Windows;
using System.Net.Http;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();

            // Conexion SQL
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select A.Id, Codigo, Nombre, A.Descripcion, IdMarca, IdCategoria, ImagenUrl, Precio, C.Id, C.Descripcion as cDescripcion, M.Id, M.Descripcion as mDescripcion from ARTICULOS A, CATEGORIAS C, MARCAS M Where A.IdMarca = M.Id And A.IdCategoria = C.Id\r\n";
                comando.Connection= conexion;

                conexion.Open();
                // Guardamos lectura
                lector = comando.ExecuteReader();

                // Cargamos las filas mientras haya.
                while (lector.Read())
                {
                    Articulo aux = new Articulo();

                    aux.Id = (int)lector["Id"];
                    aux.CodigoArticulo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    if (!(lector["ImagenUrl"] is DBNull))
                    {
                        aux.UrlImagen = (string)lector["ImagenUrl"];
                    }
                    aux.Precio = (decimal)lector["Precio"];
                    
                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)lector["IdMarca"];
                    aux.Marca.Descripcion = (string)lector["mDescripcion"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)lector["cDescripcion"];
                
                    // Agregamos a la lista
                    lista.Add(aux);
                }

                conexion.Close();

                return lista;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
        }
        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from ARTICULOS Where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // AGREGE LOGICO NECESITA ACTIVO...
        public void eliminarLogico(int id)
        {

        }
        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "Select Codigo, Nombre, A.Descripcion as Adescripcion, ImagenUrl, Precio, M.Descripcion as mdescripcion, C.Descripcion as cdescripcion, M.Id as mid, C.Id as cid, A.Id as aid from ARTICULOS A, CATEGORIAS C, MARCAS M Where A.Id = C.Id And A.Id = M.Id And ";
               
                if (campo == "Precio")
                {
                    switch(criterio)
                    {
                        case "Mayor a":
                            consulta += "Precio > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Precio < " + filtro;
                            break;
                        default:
                            consulta += "Precio = " + filtro;
                            break;
                    }
                }
                else if (campo == "Codigo")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Codigo like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Codigo like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Codigo like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();

                    aux.Id = (int)datos.Lector["aid"];
                    aux.CodigoArticulo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Adescripcion"];
                    
                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                    {
                        aux.UrlImagen = (string)datos.Lector["ImagenUrl"];
                    }

                    aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["mid"];
                    aux.Marca.Descripcion = (string)datos.Lector["mdescripcion"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["cid"];
                    aux.Categoria.Descripcion = (string)datos.Lector["cdescripcion"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Insert into ARTICULOS (Codigo, Nombre, Descripcion, ImagenUrl, Precio, IdMarca, IdCategoria)" +
                    "values(@codigo, @nombre, @descripcion, @imagenUrl, @precio, @idMarca, @idCategoria)");

                datos.setearParametro("@codigo", nuevo.CodigoArticulo);
                datos.setearParametro("@nombre", nuevo.Nombre);
                datos.setearParametro("@descripcion", nuevo.Descripcion);
                datos.setearParametro("@imagenUrl", nuevo.UrlImagen);
                datos.setearParametro("@precio", nuevo.Precio);
                datos.setearParametro("@idMarca", nuevo.Marca.Id);
                datos.setearParametro("@idCategoria", nuevo.Categoria.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificar(Articulo artic)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @idmarca, IdCategoria = @idcategoria, ImagenUrl = @imagenurl, Precio = @precio Where Id = @id");

                datos.setearParametro("@id", artic.Id);

                datos.setearParametro("@codigo", artic.CodigoArticulo);
                datos.setearParametro("@nombre", artic.Nombre);
                datos.setearParametro("@descripcion", artic.Descripcion);
                datos.setearParametro("@idmarca", artic.Marca.Id);
                datos.setearParametro("@idcategoria", artic.Categoria.Id);
                datos.setearParametro("@imagenurl", artic.UrlImagen);
                datos.setearParametro("@precio", artic.Precio);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }


    }
}
