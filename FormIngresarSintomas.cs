using System;
﻿using System.Drawing;
﻿using System.Windows.Forms;
﻿
﻿namespace MEDICENTER
﻿{
﻿    // Formulario para que el paciente ingrese sus síntomas seleccionándolos de una lista.
﻿    // Una vez seleccionados, estos síntomas inician el proceso de diagnóstico automático.
﻿    public partial class FormIngresarSintomas : Form
﻿    {
﻿        private Sistema sistema; // Instancia de la clase Sistema para interactuar con la lógica de negocio.
﻿        private Paciente paciente; // El paciente que está ingresando los síntomas.
﻿        private Hospital hospital; // El hospital donde se registrará la consulta.
﻿        private CheckedListBox checkedListSintomas; // Control para la lista de síntomas seleccionables.
﻿
﻿        // Constructor del formulario.
﻿        public FormIngresarSintomas(Sistema sistemaParam, Paciente pacienteParam, Hospital hospitalParam)
﻿        {
﻿            sistema = sistemaParam; // Asigna la instancia del sistema.
﻿            paciente = pacienteParam; // Asigna el objeto paciente.
﻿            hospital = hospitalParam; // Asigna el objeto hospital.
﻿            InitializeComponent(); // Inicializa los componentes de la interfaz de usuario.
﻿        }
﻿
﻿        // Método que inicializa programáticamente todos los componentes visuales del formulario.
﻿        private void InitializeComponent()
﻿        {
﻿            // Configuración básica del formulario.
﻿            this.ClientSize = new Size(800, 700); // Tamaño del formulario.
﻿            this.Text = "Ingresar Síntomas"; // Título de la ventana.
﻿            this.StartPosition = FormStartPosition.CenterScreen; // Posiciona el formulario en el centro.
﻿            this.BackColor = Color.FromArgb(230, 230, 250); // Color de fondo.
﻿
﻿            // Título principal del formulario.
﻿            Label lblTitulo = new Label();
﻿            lblTitulo.Text = "Ingresar Síntomas";
﻿            lblTitulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
﻿            lblTitulo.Location = new Point(280, 20);
﻿            lblTitulo.Size = new Size(250, 40);
﻿            this.Controls.Add(lblTitulo);
﻿
﻿            // Etiqueta para mostrar el hospital seleccionado.
﻿            Label lblHospital = new Label();
﻿            lblHospital.Text = $"Hospital: {hospital.Nombre}";
﻿            lblHospital.Font = new Font("Segoe UI", 12);
﻿            lblHospital.Location = new Point(50, 70);
﻿            lblHospital.Size = new Size(700, 30);
﻿            this.Controls.Add(lblHospital);
﻿
﻿            // Instrucción para el usuario.
﻿            Label lblInstruccion = new Label();
﻿            lblInstruccion.Text = "Seleccione los síntomas que presenta:";
﻿            lblInstruccion.Font = new Font("Segoe UI", 11, FontStyle.Bold);
﻿            lblInstruccion.Location = new Point(50, 110);
﻿            lblInstruccion.Size = new Size(400, 30);
﻿            this.Controls.Add(lblInstruccion);
﻿
﻿            // CheckedListBox para la selección de síntomas.
﻿            checkedListSintomas = new CheckedListBox();
﻿            checkedListSintomas.Location = new Point(50, 150);
﻿            checkedListSintomas.Size = new Size(700, 400);
﻿            checkedListSintomas.Font = new Font("Segoe UI", 11);
﻿            checkedListSintomas.CheckOnClick = true; // Permite marcar/desmarcar con un solo clic.
﻿
﻿            // Array de síntomas predefinidos.
﻿            string[] sintomas = {
﻿                "Fiebre", "Tos", "Dolor de cabeza", "Dolor de garganta",
﻿                "Fatiga", "Náuseas", "Dolor abdominal", "Dificultad para respirar",
﻿                "Mareos", "Dolor muscular"
﻿            };
﻿
﻿            checkedListSintomas.Items.AddRange(sintomas); // Añade los síntomas a la lista.
﻿            this.Controls.Add(checkedListSintomas);
﻿
﻿            // Botón "Continuar al Diagnóstico".
﻿            Button btnContinuar = new Button();
﻿            btnContinuar.Text = "Continuar al Diagnóstico";
﻿            btnContinuar.Font = new Font("Segoe UI", 12);
﻿            btnContinuar.Location = new Point(250, 580);
﻿            btnContinuar.Size = new Size(220, 50);
﻿            btnContinuar.BackColor = Color.White;
﻿            btnContinuar.Click += BtnContinuar_Click; // Asigna el evento para continuar.
﻿            this.Controls.Add(btnContinuar);
﻿
﻿            // Botón "Cancelar".
﻿            Button btnCancelar = new Button();
﻿            btnCancelar.Text = "Cancelar";
﻿            btnCancelar.Font = new Font("Segoe UI", 12);
﻿            btnCancelar.Location = new Point(490, 580);
﻿            btnCancelar.Size = new Size(120, 50);
﻿            btnCancelar.BackColor = Color.White;
﻿            btnCancelar.Click += (s, e) => this.Close(); // Cierra el formulario al cancelar.
﻿            this.Controls.Add(btnCancelar);
﻿        }
﻿
﻿        // Manejador de eventos para el botón "Continuar al Diagnóstico".
﻿        private void BtnContinuar_Click(object sender, EventArgs e)
﻿        {
﻿            // Valida que se haya seleccionado al menos un síntoma.
﻿            if (checkedListSintomas.CheckedItems.Count == 0)
﻿            {
﻿                MessageBox.Show("Debe seleccionar al menos un síntoma", "Error",
﻿                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
﻿                return;
﻿            }
﻿
﻿            // Crea un nuevo RegistroMedico con la información inicial.
﻿            RegistroMedico nuevoRegistro = new RegistroMedico();
﻿            nuevoRegistro.IdRegistro = sistema.GenerarIdRegistro(); // Genera un ID único para el registro.
﻿            nuevoRegistro.IdPaciente = paciente.Id; // Asigna el ID del paciente.
﻿            nuevoRegistro.IdHospital = hospital.Id; // Asigna el ID del hospital.
﻿
﻿            // Añade los síntomas seleccionados al nuevo registro.
﻿            foreach (object item in checkedListSintomas.CheckedItems)
﻿            {
﻿                nuevoRegistro.Sintomas.Add(item.ToString());
﻿            }
﻿
﻿            // Abre el formulario de diagnóstico, pasando el sistema, paciente, hospital y el nuevo registro.
﻿            FormDiagnostico formDiagnostico =
﻿                new FormDiagnostico(sistema, paciente, hospital, nuevoRegistro);
﻿            this.Hide(); // Oculta el formulario actual.
﻿            formDiagnostico.ShowDialog(); // Muestra el formulario de diagnóstico como diálogo modal.
﻿            this.Close(); // Cierra el formulario de síntomas una vez que el diagnóstico ha terminado.
﻿        }
﻿    }
﻿}
﻿