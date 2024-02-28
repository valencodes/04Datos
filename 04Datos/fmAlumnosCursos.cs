using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _04Datos
{
    public partial class fmAlumnosCursos : Form
    {
        public fmAlumnosCursos()
        {
            InitializeComponent();
            cursosDataGridView.SelectionChanged += cursosDataGridView_SelectionChanged;
        }



        private void cursosBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.cursosBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cursosformDataSet);

        }

        private void fmAlumnosCursos_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'cursosformDataSet.alumnos' Puede moverla o quitarla según sea necesario.
            this.alumnosTableAdapter.Fill(this.cursosformDataSet.alumnos);
            // TODO: esta línea de código carga datos en la tabla 'cursosformDataSet.cursos' Puede moverla o quitarla según sea necesario.
            this.cursosTableAdapter.Fill(this.cursosformDataSet.cursos);

            // Conexión a la base de datos para obtener niveles distintos
            using (OleDbConnection connection = new OleDbConnection(Properties.Settings.Default.cursosformConnectionString))
            {
                try
                {
                    connection.Open();
                    using (OleDbCommand command = new OleDbCommand("SELECT DISTINCT nivel FROM cursos", connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            cbFiltrarNivel.Items.Clear();
                            cbFiltrarNivel.Items.Add("Todos"); // Añade el ítem "Todos" al principio
                            while (reader.Read())
                            {
                                cbFiltrarNivel.Items.Add(reader.GetString(0)); // Asume que la columna Nivel es un string
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar niveles de cursos: {ex.Message}");
                }
            }

            // Configura el ComboBox para mostrar todos los cursos al inicio
            cbFiltrarNivel.SelectedIndex = 0; // Selecciona el ítem "Todos" por defecto

        }

        private void cbFiltrarNivel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFiltrarNivel.SelectedItem.ToString() == "Todos")
            {
                cursosBindingSource.RemoveFilter(); // Quita el filtro si se selecciona "Todos"
            }
            else
            {
                // Aplica un filtro basado en la selección del nivel
                cursosBindingSource.Filter = $"Nivel = '{cbFiltrarNivel.SelectedItem.ToString()}'";
            }
        }
        private void alumnosDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            int totalMujeres = 0;
            int totalVarones = 0;
            int sumaEdades = 0;
            int totalAlumnos = alumnosDataGridView.Rows.Count - 1; // Asumiendo que AllowUserToAddRows está activado.

            foreach (DataGridViewRow row in alumnosDataGridView.Rows)
            {
                if (row.IsNewRow) continue; // Ignora la fila de nuevo registro.

                string sexo = row.Cells[5].Value?.ToString() ?? "";
                int edad = Convert.ToInt32(row.Cells[4].Value ?? 0);

                if (sexo == "MUJER") totalMujeres++;
                else if (sexo == "VARON") totalVarones++;

                sumaEdades += edad;
            }

            int mediaEdad = totalAlumnos > 0 ? sumaEdades / totalAlumnos : 0;

            // se asegura de que CurrentRow no es null
            int vacantes = 0;
            if (cursosDataGridView.CurrentRow != null)
            {
                int numAlumnosCurso = Convert.ToInt32(cursosDataGridView.CurrentRow.Cells[2].Value ?? 0);
                vacantes = numAlumnosCurso - totalAlumnos;
            }

            // actualiza los Labels
            if (alumnosDataGridView.RowCount > 0) { 
             lbTotalMujeres.Text = $" {totalMujeres}";
            lbTotalVarones.Text = $" {totalVarones}";
            lbMediaEdad.Text = $" {mediaEdad}";
            lbTotalVacantes.Text = $"{vacantes}";
            lbCurso.Text+= $" {totalAlumnos}";
            }
        }

        private void alumnosBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void alumnosDataGridView_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }

        private void cursosDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            int totalAlumnos = alumnosDataGridView.Rows.Count - 1;
            if (cursosDataGridView.CurrentRow != null)
            {

                // Obtén el ID del curso seleccionado
                var cursoId = Convert.ToInt32(cursosDataGridView.CurrentRow.Cells[0].Value);

                // Actualiza el filtro del BindingSource de alumnos
                alumnosBindingSource.Filter = $"IdCurso = {cursoId}";

                string nombreCurso = cursosDataGridView.CurrentRow != null ? cursosDataGridView.CurrentRow.Cells[1].Value.ToString() : "Curso desconocido";
                // Opcionalmente, actualiza el nombre del curso seleccionado en un Label
                lbCurso.Text = $"Curso: {nombreCurso}  {totalAlumnos}";
            }
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Verifica si el CheckBox está marcado
            if (cbMostrarMujeres.Checked)
            {
                // Aplica un filtro para mostrar solo mujeres
                alumnosBindingSource.Filter = "sexo = 'MUJER'";
            }
            else
            {
                // Remueve el filtro para mostrar todos los alumnos
                alumnosBindingSource.RemoveFilter();
            }
        }

        private void rbOrdenarNombre_CheckedChanged(object sender, EventArgs e) //ver practica 01
        {
            AplicarOrdenamiento();
        }

        private void AplicarOrdenamiento()
        {
            string campoOrdenamiento = rbOrdenarNombre.Checked ? "nombrealu" : "idalumno";
            string direccionOrdenamiento = cbAscendente.Checked ? "ASC" : "DESC";

            // Asume que alumnosBindingSource es el BindingSource para tu DataGridView
            alumnosBindingSource.Sort = $"{campoOrdenamiento} {direccionOrdenamiento}";
        }

        private void cbFiltrarSexo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-aplica el filtro solo si el CheckBox está marcado.
            if (ckFiltrarSexo.Checked)
            {
                AplicarFiltroSexo();
            }
        }
        private void AplicarFiltroSexo()
        {
             // Inicialmente, establece el filtro para incluir solo alumnos del curso seleccionado.
    string filtroCurso = "";
    if (cursosDataGridView.CurrentRow != null)
    {
        var cursoId = Convert.ToInt32(cursosDataGridView.CurrentRow.Cells[0].Value);
        filtroCurso = $"IdCurso = {cursoId}";
    }

    // Si el CheckBox para filtrar por sexo está marcado, añade el filtro de sexo.
    if (ckFiltrarSexo.Checked && cbFiltrarSexo.SelectedItem != null)
    {
        string filtroSexo = cbFiltrarSexo.SelectedItem.ToString();
        // Si ya hay un filtro de curso aplicado, añade "AND" para combinar los filtros.
        if (!string.IsNullOrEmpty(filtroCurso))
        {
            filtroCurso += $" AND sexo = '{filtroSexo}'";
        }
        else
        {
            filtroCurso = $"sexo = '{filtroSexo}'";
        }
    }

    // Aplica el filtro combinado al BindingSource de alumnos.
    alumnosBindingSource.Filter = filtroCurso;
           /* if (ckFiltrarSexo.Checked && cbFiltrarSexo.SelectedItem != null)
            {
                string filtroSexo = cbFiltrarSexo.SelectedItem.ToString();
                alumnosBindingSource.Filter = $"sexo = '{filtroSexo}'";
            }
            else
            {
                alumnosBindingSource.RemoveFilter();
            }*/
        }

        private void ckFiltrarSexo_CheckedChanged(object sender, EventArgs e)
        {
            AplicarFiltroSexo();
        }
    }
}
