using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    }
}
