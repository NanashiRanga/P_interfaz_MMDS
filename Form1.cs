using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registro_FGO_MMDS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Deshabilitar botones de editar y eliminar al iniciar
            btnActualizar.Enabled = false;
            btnEliminar.Enabled = false;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            String codigo = txtCodigo.Text;
            String nombre = txtNombre.Text;
            String descripcion = txtDescripcion.Text;
            int existencia;
            double precio;

            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion) ||
                string.IsNullOrWhiteSpace(txtPreciopub.Text) || string.IsNullOrWhiteSpace(txtExistencias.Text))
            {
                MessageBox.Show("Error: Todos los campos son obligatorios.");
                return;
            }

            // Validar que el código sea solo números
            if (!int.TryParse(codigo, out _)) { MessageBox.Show("Error: El código debe ser un número."); txtCodigo.Focus(); return; }
            // Validar que el precio sea un número
            if (!double.TryParse(txtPreciopub.Text, out precio)) { MessageBox.Show("Error: El precio debe ser un número."); txtPreciopub.Focus(); return; }
            // Validar que la existencia sea un número
            if (!int.TryParse(txtExistencias.Text, out existencia)) { MessageBox.Show("Error: La existencia debe ser un número."); txtExistencias.Focus(); return; }
            // Verificar que el código no exista ya en la BD
            if (CodigoExiste(codigo)) { MessageBox.Show("Error: El código ya existe."); txtCodigo.Focus(); return; }
            string sql = "INSERT INTO Productos (codigo,nombre,descripcion,precio,existencias) VALUES ('" + codigo + "','" + nombre + "','" + descripcion + "','" + precio + "','" + existencia + "')";

            MySqlConnection con = Conexion.conexion();
            con.Open();

            try
            {
                MySqlCommand command = new MySqlCommand(sql, con);
                command.ExecuteNonQuery();
                MessageBox.Show("Registro almacenado");
                Clean();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al almacenar: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            String id = txtId.Text;
            String codigo = txtCodigo.Text;
            String nombre = txtNombre.Text;
            String descripcion = txtDescripcion.Text;
            int existencia;
            double precio;

            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion) ||string.IsNullOrWhiteSpace(txtPreciopub.Text) || string.IsNullOrWhiteSpace(txtExistencias.Text))
            {
                MessageBox.Show("Error: Todos los campos son obligatorios.");
                return;
            }

            // Validar que el código sea solo números
            if (!int.TryParse(codigo, out _)) { MessageBox.Show("Error: El código debe ser un número."); txtCodigo.Focus(); return; }
            // Validar que el precio sea un número
            if (!double.TryParse(txtPreciopub.Text, out precio)) { MessageBox.Show("Error: El precio debe ser un número."); txtPreciopub.Focus(); return; }
            // Validar que la existencia sea un número
            if (!int.TryParse(txtExistencias.Text, out existencia)) { MessageBox.Show("Error: La existencia debe ser un número."); txtExistencias.Focus(); return; }
            // Confirmacion
            var confirmacion = MessageBox.Show("¿Está seguro de que desea editar este registro?", "Confirmacion", MessageBoxButtons.YesNo);
            if (confirmacion == DialogResult.No)
            {
                return;
            }
            // Actualizar el producto en la base de datos
            string sql = "UPDATE Productos SET codigo='" + codigo + "', nombre='" + nombre + "', descripcion='" + descripcion + "', precio='" + precio + "', existencias='" + existencia + "' WHERE IdProducto='" + id + "'";

            MySqlConnection con = Conexion.conexion();
            con.Open();

            try
            {
                MySqlCommand command = new MySqlCommand(sql, con);
                command.ExecuteNonQuery();
                MessageBox.Show("Registro Actualizado");
                Clean();
                btnActualizar.Enabled = false;
                btnEliminar.Enabled = false;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al Actualizar: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Boton Eliminar
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Confirmacion
            String id = txtId.Text;
            var confirmacion = MessageBox.Show("¿Está seguro de que desea borrar este registro?", "Confirmacion", MessageBoxButtons.YesNo);
            if (confirmacion == DialogResult.No)
            {
                return;
            }
            // Eliminar el producto de la base de datos
            string sql = "DELETE FROM Productos WHERE IdProducto='" + id + "'";

            MySqlConnection con = Conexion.conexion();
            con.Open();

            try
            {
                MySqlCommand command = new MySqlCommand(sql, con);
                int rowsAffected = command.ExecuteNonQuery();
                // Bloqueo de botones y mensajes
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Registro borrado");
                    Clean();
                    btnActualizar.Enabled = false;
                    btnEliminar.Enabled = false;
      
                }
                else
                {
                    MessageBox.Show("Error al Borrar: Registro no existente o encontrado");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al Borrar: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        // Boton buscar
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            String codigo = txtCodigo.Text;
            MySqlDataReader reader = null;
            // Método para buscar un producto por su código
            string sql = "SELECT IdProducto, codigo, nombre, descripcion, precio, existencias FROM Productos WHERE codigo LIKE '" + codigo + "' LIMIT 1";
            MySqlConnection con = Conexion.conexion();
            con.Open();

            try
            {
                MySqlCommand comando = new MySqlCommand(sql, con);
                reader = comando.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        txtId.Text = reader.GetString(0);
                        txtCodigo.Text = reader.GetString(1);
                        txtNombre.Text = reader.GetString(2);
                        txtDescripcion.Text = reader.GetString(3);
                        txtPreciopub.Text = reader.GetString(4);
                        txtExistencias.Text = reader.GetString(5);
                    }
                    // Habilitar botones de editar y eliminar después de una búsqueda exitosa
                    btnActualizar.Enabled = true;
                    btnEliminar.Enabled = true;
                }
                else
                {
                    MessageBox.Show("No se encontró registro");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        // Boton Limpiar
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Método para limpiar los campos del formulario
            Clean();
        }
        // Método auxiliar para limpiar los campos del formulario
        private void Clean()
        {
            txtId.Text = "";
            txtCodigo.Text = "";
            txtDescripcion.Text = "";
            txtNombre.Text = "";
            txtPreciopub.Text = "";
            txtExistencias.Text = "";
        }

        // Método para verificar si un código de producto ya existe en la base de datos
        private bool CodigoExiste(string codigo)
        {
            string sql = "SELECT COUNT(*) FROM Productos WHERE codigo='" + codigo + "'";
            MySqlConnection con = Conexion.conexion();
            con.Open();
            try
            {
                MySqlCommand command = new MySqlCommand(sql, con);
                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al buscar en el código: " + ex.Message);
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        // Método que se ejecuta al cargar el formulario
        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
