using System;
using System.Drawing;
using System.Windows.Forms;

namespace MEDICENTER
{
    // Formulario para que el paciente pueda actualizar sus datos personales (teléfono, email, contacto de emergencia)
    // y también cambiar su contraseña.
    public partial class FormActualizarDatos : Form
    {
        private Sistema sistema; // Instancia de la clase Sistema para interactuar con la lógica de negocio y guardar cambios.
        private Paciente paciente; // Objeto Paciente cuyos datos se van a actualizar.
        
        // Campos de texto para la entrada de datos.
        private TextBox txtTelefono;
        private TextBox txtEmail;
        private TextBox txtContacto;
        private TextBox txtPasswordActual; // Campo para la contraseña actual del paciente.
        private TextBox txtPasswordNueva; // Campo para la nueva contraseña del paciente.

        // Constructor del formulario.
        public FormActualizarDatos(Sistema sistemaParam, Paciente pacienteParam)
        {
            sistema = sistemaParam; // Asigna la instancia del sistema.
            paciente = pacienteParam; // Asigna el objeto paciente.
            InitializeComponent(); // Inicializa los componentes de la interfaz de usuario.
        }

        // Método que inicializa programáticamente todos los componentes visuales del formulario.
        private void InitializeComponent()
        {
            // Configuración básica del formulario.
            this.ClientSize = new Size(700, 600); // Tamaño del formulario.
            this.Text = "Actualizar Datos Personales"; // Título de la ventana.
            this.StartPosition = FormStartPosition.CenterScreen; // Posiciona el formulario en el centro.
            this.BackColor = Color.FromArgb(230, 230, 250); // Color de fondo.

            // Título principal del formulario.
            Label lblTitulo = new Label();
            lblTitulo.Text = "Actualizar Datos Personales";
            lblTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitulo.Location = new Point(180, 20);
            lblTitulo.Size = new Size(340, 40);
            this.Controls.Add(lblTitulo);

            int yPos = 90; // Posición vertical inicial para los controles.

            // Campo para "Nuevo Teléfono".
            Label lblTelefono = new Label();
            lblTelefono.Text = "Nuevo Teléfono:";
            lblTelefono.Font = new Font("Segoe UI", 11);
            lblTelefono.Location = new Point(80, yPos);
            lblTelefono.Size = new Size(150, 30);
            lblTelefono.TextAlign = ContentAlignment.MiddleRight;
            this.Controls.Add(lblTelefono);

            txtTelefono = new TextBox();
            txtTelefono.Font = new Font("Segoe UI", 11);
            txtTelefono.Location = new Point(250, yPos);
            txtTelefono.Size = new Size(300, 30);
            txtTelefono.Text = paciente.Telefono; // Muestra el teléfono actual del paciente.
            txtTelefono.KeyPress += TxtNumeric_KeyPress; // Añade validación numérica para el campo.
            this.Controls.Add(txtTelefono);

            yPos += 50;

            // Campo para "Nuevo Email".
            Label lblEmail = new Label();
            lblEmail.Text = "Nuevo Email:";
            lblEmail.Font = new Font("Segoe UI", 11);
            lblEmail.Location = new Point(80, yPos);
            lblEmail.Size = new Size(150, 30);
            lblEmail.TextAlign = ContentAlignment.MiddleRight;
            this.Controls.Add(lblEmail);

            txtEmail = new TextBox();
            txtEmail.Font = new Font("Segoe UI", 11);
            txtEmail.Location = new Point(250, yPos);
            txtEmail.Size = new Size(300, 30);
            txtEmail.Text = paciente.Email; // Muestra el email actual del paciente.
            this.Controls.Add(txtEmail);

            yPos += 50;

            // Campo para "Nuevo Contacto de Emergencia".
            Label lblContacto = new Label();
            lblContacto.Text = "Nuevo Contacto Emergencia:";
            lblContacto.Font = new Font("Segoe UI", 11);
            lblContacto.Location = new Point(50, yPos);
            lblContacto.Size = new Size(180, 30);
            lblContacto.TextAlign = ContentAlignment.MiddleRight;
            this.Controls.Add(lblContacto);

            txtContacto = new TextBox();
            txtContacto.Font = new Font("Segoe UI", 11);
            txtContacto.Location = new Point(250, yPos);
            txtContacto.Size = new Size(300, 30);
            txtContacto.Text = paciente.ContactoEmergencia; // Muestra el contacto de emergencia actual.
            txtContacto.KeyPress += TxtNumeric_KeyPress; // Añade validación numérica.
            this.Controls.Add(txtContacto);

            yPos += 80;

            // Separador y título para la sección de "Cambiar Contraseña".
            Label lblSeparador = new Label();
            lblSeparador.Text = "Cambiar Contraseña";
            lblSeparador.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblSeparador.Location = new Point(240, yPos);
            lblSeparador.Size = new Size(220, 30);
            this.Controls.Add(lblSeparador);

            yPos += 40;

            // Campo para la "Contraseña Actual".
            Label lblPassActual = new Label();
            lblPassActual.Text = "Contraseña Actual:";
            lblPassActual.Font = new Font("Segoe UI", 11);
            lblPassActual.Location = new Point(80, yPos);
            lblPassActual.Size = new Size(150, 30);
            lblPassActual.TextAlign = ContentAlignment.MiddleRight;
            this.Controls.Add(lblPassActual);

            txtPasswordActual = new TextBox();
            txtPasswordActual.Font = new Font("Segoe UI", 11);
            txtPasswordActual.Location = new Point(250, yPos);
            txtPasswordActual.Size = new Size(300, 30);
            txtPasswordActual.UseSystemPasswordChar = true; // Oculta la entrada.
            this.Controls.Add(txtPasswordActual);

            yPos += 50;

            // Campo para la "Nueva Contraseña".
            Label lblPassNueva = new Label();
            lblPassNueva.Text = "Nueva Contraseña:";
            lblPassNueva.Font = new Font("Segoe UI", 11);
            lblPassNueva.Location = new Point(80, yPos);
            lblPassNueva.Size = new Size(150, 30);
            lblPassNueva.TextAlign = ContentAlignment.MiddleRight;
            this.Controls.Add(lblPassNueva);

            txtPasswordNueva = new TextBox();
            txtPasswordNueva.Font = new Font("Segoe UI", 11);
            txtPasswordNueva.Location = new Point(250, yPos);
            txtPasswordNueva.Size = new Size(300, 30);
            txtPasswordNueva.UseSystemPasswordChar = true; // Oculta la entrada.
            this.Controls.Add(txtPasswordNueva);

            yPos += 70;

            // Botón "Guardar Cambios".
            Button btnGuardar = new Button();
            btnGuardar.Text = "Guardar Cambios";
            btnGuardar.Font = new Font("Segoe UI", 12);
            btnGuardar.Location = new Point(200, yPos);
            btnGuardar.Size = new Size(160, 45);
            btnGuardar.BackColor = Color.White;
            btnGuardar.Click += BtnGuardar_Click; // Asigna el evento para guardar.
            this.Controls.Add(btnGuardar);

            // Botón "Cancelar".
            Button btnCancelar = new Button();
            btnCancelar.Text = "Cancelar";
            btnCancelar.Font = new Font("Segoe UI", 12);
            btnCancelar.Location = new Point(380, yPos);
            btnCancelar.Size = new Size(120, 45);
            btnCancelar.BackColor = Color.White;
            btnCancelar.Click += (s, e) => this.Close(); // Cierra el formulario al cancelar.
            this.Controls.Add(btnCancelar);
        }

        // Manejador de eventos para restringir la entrada a solo números en campos de texto específicos.
        private void TxtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite solo dígitos y la tecla de retroceso (borrar).
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Ignora cualquier otro carácter presionado.
            }
        }

        // Manejador de eventos para el botón "Guardar Cambios".
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            bool cambios = false; // Flag para indicar si se realizaron cambios.

            // Actualiza el teléfono si el nuevo valor es diferente y no está vacío.
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text) && txtTelefono.Text != paciente.Telefono)
            {
                paciente.Telefono = txtTelefono.Text.Trim();
                cambios = true;
            }

            // Actualiza el email si el nuevo valor es diferente y no está vacío.
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && txtEmail.Text != paciente.Email)
            {
                paciente.Email = txtEmail.Text.Trim();
                cambios = true;
            }

            // Actualiza el contacto de emergencia si el nuevo valor es diferente y no está vacío.
            if (!string.IsNullOrWhiteSpace(txtContacto.Text) && txtContacto.Text != paciente.ContactoEmergencia)
            {
                paciente.ContactoEmergencia = txtContacto.Text.Trim();
                cambios = true;
            }

            // Lógica para cambiar la contraseña.
            if (!string.IsNullOrWhiteSpace(txtPasswordActual.Text) &&
                !string.IsNullOrWhiteSpace(txtPasswordNueva.Text))
            {
                // Verifica que la contraseña actual ingresada coincida con la contraseña del paciente.
                if (txtPasswordActual.Text == paciente.Password)
                {
                    // Valida que la nueva contraseña tenga al menos 6 caracteres.
                    if (txtPasswordNueva.Text.Length >= 6)
                    {
                        paciente.Password = txtPasswordNueva.Text; // Actualiza la contraseña.
                        cambios = true;
                    }
                    else
                    {
                        MessageBox.Show("La nueva contraseña debe tener al menos 6 caracteres.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Sale del método si la nueva contraseña es muy corta.
                    }
                }
                else
                {
                    MessageBox.Show("La contraseña actual es incorrecta.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Sale del método si la contraseña actual no coincide.
                }
            }

            // Si se realizaron cambios, guarda el usuario en el sistema.
            if (cambios)
            {
                sistema.GuardarUsuario(paciente); // Persiste los cambios del paciente.
                MessageBox.Show("Datos actualizados exitosamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Cierra el formulario.
            }
            else
            {
                // Si no se detectaron cambios, informa al usuario.
                MessageBox.Show("No se realizaron cambios.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}