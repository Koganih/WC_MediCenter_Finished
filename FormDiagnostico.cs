using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MEDICENTER
{
    // Formulario de Diagnóstico Automático: Guía al paciente a través de un árbol de decisiones
    // para obtener un diagnóstico preliminar basado en sus síntomas.
    public partial class FormDiagnostico : Form
    {
        private Sistema sistema; // Instancia de la clase Sistema para interactuar con la lógica de negocio.
        private Paciente paciente; // El paciente que está realizando el diagnóstico.
        private Hospital hospital; // El hospital donde se registra la consulta.
        private RegistroMedico registro; // El registro médico que se está creando/actualizando.
        private DecisionNode nodoActual; // El nodo actual en el árbol de decisiones.
        
        // Controles de UI para mostrar preguntas y recibir respuestas.
        private Label lblPregunta;
        private Button btnSi;
        private Button btnNo;
        private Panel panelPregunta; // Panel que contiene la pregunta y los botones de respuesta.
        private Panel panelResultado; // Panel que muestra el resultado del diagnóstico.

        // Constructor del formulario de diagnóstico.
        public FormDiagnostico(Sistema sistemaParam, Paciente pacienteParam, Hospital hospitalParam, RegistroMedico registroParam)
        {
            sistema = sistemaParam; // Asigna la instancia del sistema.
            paciente = pacienteParam; // Asigna el objeto paciente.
            hospital = hospitalParam; // Asigna el objeto hospital.
            registro = registroParam; // Asigna el objeto registro médico.
            nodoActual = sistema.ArbolDiagnostico; // Inicia el diagnóstico en la raíz del árbol.

            InitializeComponent(); // Inicializa los componentes de la interfaz de usuario.
        }

        // Método que inicializa programáticamente todos los componentes visuales del formulario.
        private void InitializeComponent()
        {
            // Configuración básica del formulario.
            this.ClientSize = new Size(900, 650); // Tamaño del formulario.
            this.Text = "Diagnóstico Automático"; // Título de la ventana.
            this.StartPosition = FormStartPosition.CenterScreen; // Posiciona el formulario en el centro.
            this.BackColor = Color.FromArgb(230, 230, 250); // Color de fondo.

            // Título principal del formulario.
            Label lblTitulo = new Label();
            lblTitulo.Text = "Diagnóstico Automático";
            lblTitulo.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitulo.Location = new Point(250, 30);
            lblTitulo.Size = new Size(400, 50);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTitulo);

            // Instrucción para el usuario.
            Label lblInstruccion = new Label();
            lblInstruccion.Text = "Responda las siguientes preguntas:";
            lblInstruccion.Font = new Font("Segoe UI", 13);
            lblInstruccion.Location = new Point(250, 90);
            lblInstruccion.Size = new Size(400, 30);
            lblInstruccion.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblInstruccion);

            // Panel para mostrar la pregunta actual y las opciones de respuesta.
            panelPregunta = new Panel();
            panelPregunta.Location = new Point(100, 150);
            panelPregunta.Size = new Size(700, 350);
            panelPregunta.BackColor = Color.White;
            panelPregunta.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panelPregunta);

            // Etiqueta donde se muestra la pregunta del nodo actual.
            lblPregunta = new Label();
            lblPregunta.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblPregunta.Location = new Point(50, 80);
            lblPregunta.Size = new Size(600, 100);
            lblPregunta.TextAlign = ContentAlignment.MiddleCenter;
            panelPregunta.Controls.Add(lblPregunta);

            // Botón "SÍ" para responder a la pregunta.
            btnSi = new Button();
            btnSi.Text = "SÍ";
            btnSi.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnSi.Location = new Point(150, 220);
            btnSi.Size = new Size(150, 60);
            btnSi.BackColor = Color.LightGreen;
            btnSi.Click += (s, e) => ProcesarRespuesta("si"); // Asigna el evento de respuesta "sí".
            panelPregunta.Controls.Add(btnSi);

            // Botón "NO" para responder a la pregunta.
            btnNo = new Button();
            btnNo.Text = "NO";
            btnNo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            btnNo.Location = new Point(400, 220);
            btnNo.Size = new Size(150, 60);
            btnNo.BackColor = Color.LightCoral;
            btnNo.Click += (s, e) => ProcesarRespuesta("no"); // Asigna el evento de respuesta "no".
            panelPregunta.Controls.Add(btnNo);

            // Panel para mostrar el resultado final del diagnóstico.
            panelResultado = new Panel();
            panelResultado.Location = new Point(100, 150);
            panelResultado.Size = new Size(700, 350);
            panelResultado.BackColor = Color.White;
            panelResultado.BorderStyle = BorderStyle.FixedSingle;
            panelResultado.Visible = false; // Inicialmente oculto.
            this.Controls.Add(panelResultado);

            MostrarPregunta(); // Inicia el proceso mostrando la primera pregunta.
        }

        // Muestra la pregunta del nodo actual o el resultado si es un nodo hoja.
        private void MostrarPregunta()
        {
            if (nodoActual.EsHoja())
            {
                MostrarResultado(); // Si es una hoja, muestra el resultado final.
            }
            else
            {
                lblPregunta.Text = nodoActual.Pregunta; // Si no es hoja, muestra la pregunta.
            }
        }

        // Procesa la respuesta del usuario y avanza en el árbol de decisiones.
        private void ProcesarRespuesta(string respuesta)
        {
            // Busca un hijo cuyo RespuestaEsperada coincida con la respuesta del usuario.
            foreach (DecisionNode hijo in nodoActual.Hijos)
            {
                if (hijo.RespuestaEsperada == respuesta)
                {
                    nodoActual = hijo; // Avanza al siguiente nodo.
                    MostrarPregunta(); // Muestra la pregunta o resultado del nuevo nodo.
                    return;
                }
            }

            // En caso de que la respuesta no tenga un camino definido en el árbol (error en la configuración del árbol).
            MessageBox.Show("Error en el árbol de decisión", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Muestra el panel con el resultado del diagnóstico.
        private void MostrarResultado()
        {
            panelPregunta.Visible = false; // Oculta el panel de preguntas.
            panelResultado.Visible = true; // Muestra el panel de resultados.

            string diagnostico = nodoActual.Diagnostico; // Obtiene el diagnóstico del nodo hoja.
            registro.Diagnostico = diagnostico; // Asigna el diagnóstico al registro médico.
            registro.Tratamiento = "Pendiente de revisión médica"; // Establece un tratamiento inicial.

            // Título para el resultado del diagnóstico.
            Label lblResultadoTitulo = new Label();
            lblResultadoTitulo.Text = "Resultado del Diagnóstico";
            lblResultadoTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblResultadoTitulo.Location = new Point(150, 30);
            lblResultadoTitulo.Size = new Size(400, 40);
            lblResultadoTitulo.TextAlign = ContentAlignment.MiddleCenter;
            panelResultado.Controls.Add(lblResultadoTitulo);

            // Etiqueta para mostrar el diagnóstico.
            Label lblDiagnostico = new Label();
            lblDiagnostico.Text = diagnostico;
            lblDiagnostico.Font = new Font("Segoe UI", 14);
            lblDiagnostico.Location = new Point(50, 100);
            lblDiagnostico.Size = new Size(600, 120);
            lblDiagnostico.TextAlign = ContentAlignment.MiddleCenter;
            panelResultado.Controls.Add(lblDiagnostico);

            // Si el diagnóstico contiene "CRITICO", muestra una alerta visual.
            if (diagnostico.Contains("CRÍTICO"))
            {
                lblDiagnostico.ForeColor = Color.Red;
                Label lblAlerta = new Label();
                lblAlerta.Text = "¡ALERTA: Diagnóstico crítico detectado!";
                lblAlerta.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblAlerta.ForeColor = Color.Red;
                lblAlerta.Location = new Point(150, 230);
                lblAlerta.Size = new Size(400, 30);
                lblAlerta.TextAlign = ContentAlignment.MiddleCenter;
                panelResultado.Controls.Add(lblAlerta);
            }

            // Botón "Finalizar Consulta".
            Button btnFinalizar = new Button();
            btnFinalizar.Text = "Finalizar Consulta";
            btnFinalizar.Font = new Font("Segoe UI", 12);
            btnFinalizar.Location = new Point(250, 280);
            btnFinalizar.Size = new Size(200, 45);
            btnFinalizar.BackColor = Color.White;
            btnFinalizar.Click += BtnFinalizar_Click; // Asigna el evento de finalización.
            panelResultado.Controls.Add(btnFinalizar);
        }

        // Manejador de eventos para el botón "Finalizar Consulta".
        private void BtnFinalizar_Click(object sender, EventArgs e)
        {
            // Asegura que el diccionario de registros por hospital exista para el hospital actual.
            if (!sistema.RegistrosPorHospital.ContainsKey(hospital.Id))
            {
                sistema.RegistrosPorHospital[hospital.Id] = new List<RegistroMedico>();
            }
            // Añade el registro médico al historial del hospital.
            sistema.RegistrosPorHospital[hospital.Id].Add(registro);

            // Añade el registro médico al historial personal del paciente.
            paciente.Historial.Add(registro);

            // Asegura que la cola de pacientes del hospital exista.
            if (!sistema.ColasPorHospital.ContainsKey(hospital.Id))
            {
                sistema.ColasPorHospital[hospital.Id] = new Queue<string>();
            }
            // Encola al paciente para revisión médica. La clave combina ID de paciente y ID de registro.
            string clave = paciente.Id + "|" + registro.IdRegistro;
            sistema.ColasPorHospital[hospital.Id].Enqueue(clave);
            sistema.GuardarUsuario(paciente); // Guarda los datos actualizados del paciente.

            // Prepara un mensaje de confirmación para el usuario.
            string mensaje = $"---------------------------------------\n";
            mensaje += "Consulta registrada exitosamente\n";
            mensaje += "---------------------------------------\n\n";
            mensaje += $"ID de Registro: {registro.IdRegistro}\n";
            mensaje += "Hospital: " + hospital.Nombre + "\n";
            mensaje += "Diagnóstico Preliminar: " + registro.Diagnostico + "\n\n";
            mensaje += "Un médico revisará su caso pronto\n";
            mensaje += "Posición en cola: " + sistema.ColasPorHospital[hospital.Id].Count + "\n";

            MessageBox.Show(mensaje, "Consulta Completada",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close(); // Cierra el formulario.
        }
    }
}