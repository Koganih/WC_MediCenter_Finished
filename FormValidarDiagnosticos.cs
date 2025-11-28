using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq; // Necesario para FirstOrDefault y Where
using System.Windows.Forms;

namespace MEDICENTER
{
    // Formulario para que el personal médico revise y valide diagnósticos preliminares pendientes
    // generados por el sistema automático, o para atender directamente al paciente asociado.
    public partial class FormValidarDiagnosticos : Form
    {
        private Sistema sistema; // Instancia de la clase Sistema para acceder a los datos de registros, pacientes y personal.
        private PersonalHospitalario medico; // El objeto PersonalHospitalario (médico) logueado.
        private ListBox listBoxPendientes; // Control ListBox para mostrar los diagnósticos pendientes de validación.

        // Constructor del formulario.
        public FormValidarDiagnosticos(Sistema sistemaParam, PersonalHospitalario medicoParam)
        {
            sistema = sistemaParam; // Asigna la instancia del sistema.
            medico = medicoParam; // Asigna el objeto médico.
            InitializeComponent(); // Inicializa los componentes de la interfaz de usuario.
        }

        // Método que inicializa programáticamente todos los componentes visuales del formulario.
        private void InitializeComponent()
        {
            // Configuración básica del formulario.
            this.ClientSize = new Size(900, 700); // Tamaño del formulario.
            this.Text = "Validar Diagnósticos Pendientes"; // Título de la ventana.
            this.StartPosition = FormStartPosition.CenterScreen; // Posiciona el formulario en el centro.
            this.BackColor = Color.FromArgb(230, 230, 250); // Color de fondo.

            // Título principal del formulario.
            Label lblTitulo = new Label();
            lblTitulo.Text = "Validar Diagnósticos Pendientes";
            lblTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitulo.Location = new Point(250, 20);
            lblTitulo.Size = new Size(400, 40);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTitulo);

            // ListBox para mostrar los diagnósticos pendientes.
            listBoxPendientes = new ListBox();
            listBoxPendientes.Location = new Point(50, 80);
            listBoxPendientes.Size = new Size(800, 550);
            listBoxPendientes.Font = new Font("Consolas", 10); // Fuente monoespaciada para mejor alineación.
            listBoxPendientes.MouseDoubleClick += ListBoxPendientes_MouseDoubleClick; // Asigna el evento de doble clic.
            this.Controls.Add(listBoxPendientes);

            CargarPendientes(); // Llama al método para cargar los diagnósticos pendientes.

            // Botón "Cerrar".
            Button btnCerrar = new Button();
            btnCerrar.Text = "Cerrar";
            btnCerrar.Font = new Font("Segoe UI", 12);
            btnCerrar.Location = new Point(390, 640);
            btnCerrar.Size = new Size(120, 45);
            btnCerrar.BackColor = Color.White;
            btnCerrar.Click += (s, e) => this.Close(); // Cierra el formulario al hacer clic.
            this.Controls.Add(btnCerrar);
        }

        // Carga y muestra los diagnósticos pendientes de validación para el hospital del médico.
        private void CargarPendientes()
        {
            listBoxPendientes.Items.Clear(); // Limpia la lista antes de recargar.

            // Verifica si hay registros asociados al hospital del médico.
            if (!sistema.RegistrosPorHospital.ContainsKey(medico.IdHospital))
            {
                listBoxPendientes.Items.Add("No hay registros en este hospital");
                return;
            }

            // Filtra los registros para obtener solo aquellos que no han sido confirmados.
            List<RegistroMedico> pendientes = sistema.RegistrosPorHospital[medico.IdHospital]
                .Where(r => !r.Confirmado).ToList();

            // Verifica si hay diagnósticos pendientes.
            if (!pendientes.Any()) 
            {
                listBoxPendientes.Items.Add("Todos los diagnósticos están confirmados");
                return;
            }

            // Muestra el número de diagnósticos pendientes y un separador.
            listBoxPendientes.Items.Add($"Diagnósticos pendientes: {pendientes.Count}");
            listBoxPendientes.Items.Add("═══════════════════════════════════════════════");
            listBoxPendientes.Items.Add("");

            // Itera sobre cada registro pendiente y lo muestra en el ListBox.
            foreach (var registro in pendientes)
            {
                // Busca el paciente asociado al registro para mostrar su nombre.
                Paciente paciente = sistema.Pacientes.FirstOrDefault(p => p.Id == registro.IdPaciente);
                string nombrePaciente = paciente?.Nombre ?? registro.IdPaciente; // Muestra el nombre o el ID.

                // Añade la información formateada del registro al ListBox.
                listBoxPendientes.Items.Add("───────────────────────────────────────────────");
                listBoxPendientes.Items.Add($"ID Registro: {registro.IdRegistro}");
                listBoxPendientes.Items.Add($"Paciente: {nombrePaciente} (ID: {registro.IdPaciente})");
                listBoxPendientes.Items.Add($"Fecha: {registro.Fecha:dd/MM/yyyy HH:mm}");
                listBoxPendientes.Items.Add($"Diagnóstico preliminar: {registro.Diagnostico}");
                listBoxPendientes.Items.Add(""); // Línea en blanco para separar entradas.
            }
        }

        // Manejador de eventos para el doble clic del ratón en un elemento del ListBox.
        // Permite al médico elegir entre atender al paciente o validar el registro.
        private void ListBoxPendientes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Ignora clics en las líneas de cabecera.
            if (listBoxPendientes.SelectedItem == null || listBoxPendientes.SelectedIndex < 3) 
            {
                return;
            }

            string lineaSeleccionada = listBoxPendientes.SelectedItem.ToString();
            string idRegistro = "";

            try
            {
                // Extrae el ID del registro de la línea seleccionada.
                int startIndex = lineaSeleccionada.IndexOf("ID Registro: ");
                if (startIndex != -1)
                {
                    string idPart = lineaSeleccionada.Substring(startIndex + "ID Registro: ".Length);
                    idRegistro = idPart.Split(new[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                }
                else
                {
                    return; // No es una línea de registro válida.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al extraer ID de registro: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(idRegistro))
            {
                return; // No se pudo extraer un ID válido.
            }

            // Busca el objeto RegistroMedico correspondiente al ID.
            RegistroMedico registroSeleccionado = sistema.RegistrosPorHospital[medico.IdHospital]
                                                        .FirstOrDefault(r => r.IdRegistro == idRegistro);

            if (registroSeleccionado == null)
            {
                MessageBox.Show("Registro no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Pregunta al médico qué acción desea realizar (Atender o Validar).
            DialogResult resultadoOpciones = MessageBox.Show(
                $"Registro: {registroSeleccionado.IdRegistro}\nPaciente: {registroSeleccionado.IdPaciente}\nDiagnóstico: {registroSeleccionado.Diagnostico}\n\n¿Desea Atender Paciente (Sí) o Validar Registro (No)?",
                "Opciones de Registro",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            // Procesa la elección del médico.
            if (resultadoOpciones == DialogResult.Yes) // Si elige "Sí", abre el formulario para atender al paciente.
            {
                AtenderPaciente(registroSeleccionado);
            }
            else if (resultadoOpciones == DialogResult.No) // Si elige "No", abre el formulario para validar el registro.
            {
                ValidarRegistro(registroSeleccionado);
            }
        }

        // Abre el formulario FormAtenderPaciente para que el médico atienda al paciente asociado al registro.
        private void AtenderPaciente(RegistroMedico registro)
        {
            // Busca el paciente asociado al registro.
            Paciente paciente = sistema.Pacientes.FirstOrDefault(p => p.Id == registro.IdPaciente);
            if (paciente == null)
            {
                MessageBox.Show("Paciente asociado al registro no encontrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Abre el formulario de atención al paciente, pasando el sistema, médico, paciente y registro.
            FormAtenderPaciente formAtender = new FormAtenderPaciente(sistema, medico, paciente, registro);
            formAtender.ShowDialog();
            CargarPendientes(); // Recarga la lista de pendientes después de atender al paciente.
        }

        // Abre un diálogo para que el médico valide el registro, agregando observaciones y tratamiento.
        private void ValidarRegistro(RegistroMedico registro)
        {
            // Crea un diálogo para obtener las observaciones y el tratamiento del médico.
            FormDialogoValidacion formDialogo = new FormDialogoValidacion(registro);
            if (formDialogo.ShowDialog() == DialogResult.OK) // Si el médico acepta en el diálogo.
            {
                registro.Confirmado = true; // Marca el registro como confirmado.
                registro.IdMedico = medico.Id; // Asigna el ID del médico validador.
                registro.ObservacionDoctor = formDialogo.Observaciones; // Asigna las observaciones.
                registro.Tratamiento = formDialogo.Tratamiento; // Asigna el tratamiento.

                // Persiste los cambios del registro en el historial del paciente.
                Paciente paciente = sistema.Pacientes.FirstOrDefault(p => p.Id == registro.IdPaciente);
                if (paciente != null)
                {
                    // Encuentra el registro específico en el historial del paciente y lo actualiza.
                    var regEnHistorial = paciente.Historial.FirstOrDefault(r => r.IdRegistro == registro.IdRegistro);
                    if (regEnHistorial != null)
                    {
                        regEnHistorial.Confirmado = registro.Confirmado;
                        regEnHistorial.IdMedico = registro.IdMedico;
                        regEnHistorial.ObservacionDoctor = registro.ObservacionDoctor;
                        regEnHistorial.Tratamiento = registro.Tratamiento;
                    }
                    sistema.GuardarUsuario(paciente); // Guarda el paciente actualizado.

                    // También se guarda el objeto médico (aunque en este flujo no se modifica, es buena práctica).
                    sistema.GuardarUsuario(medico);

                    MessageBox.Show("Diagnóstico validado y registro actualizado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPendientes(); // Recarga la lista de pendientes después de la validación.
                }
                else
                {
                    MessageBox.Show("Paciente asociado al registro no encontrado, no se pudo guardar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}