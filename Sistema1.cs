using System;
using System.Collections.Generic;
using System.Linq;

namespace MEDICENTER
{
    // Clase principal del sistema MediCenter que gestiona toda la lógica de negocio y los datos.
    // Actúa como un punto central para acceder y manipular información de pacientes, personal y hospitales.
    public class Sistema
    {
        // Propiedades públicas que almacenan colecciones de las entidades principales del sistema.
        public List<Paciente> Pacientes { get; set; } // Lista de todos los pacientes registrados.
        public List<PersonalHospitalario> Personal { get; set; } // Lista de todo el personal hospitalario (médicos, administradores).
        public List<Hospital> Hospitales { get; set; } // Lista de todos los hospitales disponibles en el sistema.
        public DecisionNode ArbolDiagnostico { get; set; } // Raíz del árbol de decisiones para diagnósticos.
        public Dictionary<string, Queue<string>> ColasPorHospital { get; set; } // Diccionario para gestionar colas de pacientes por hospital.
        public Dictionary<string, List<RegistroMedico>> RegistrosPorHospital { get; set; } // Diccionario para almacenar registros médicos por hospital.

        // Contadores internos para generar IDs únicos para pacientes, personal y registros.
        private int contadorPacientes;
        private int contadorPersonal;
        private int contadorRegistros;
        
        // Instancia del manejador de persistencia binaria para guardar y cargar datos.
        internal readonly PersistenciaBinaria _persistencia; // Hacemos internal para acceso directo para eliminación
        
        // Constructor de la clase Sistema.
        public Sistema()
        {
            _persistencia = new PersistenciaBinaria(); // Inicializa el objeto de persistencia.

            // Asegura que las carpetas de datos existan y carga todos los registros existentes.
            _persistencia.AsegurarCarpetasDeDatos();
            var usuariosCargados = _persistencia.CargarTodosLosRegistros();
            
            // Separa los usuarios cargados en listas de Pacientes y PersonalHospitalario.
            Pacientes = usuariosCargados.OfType<Paciente>().ToList();
            Personal = usuariosCargados.OfType<PersonalHospitalario>().ToList();

            // Inicialización de colecciones para datos no persistentes de forma global.
            Hospitales = new List<Hospital>();
            ColasPorHospital = new Dictionary<string, Queue<string>>();
            RegistrosPorHospital = new Dictionary<string, List<RegistroMedico>>();

            // Inicialización de contadores basado en los datos cargados para asegurar IDs únicos.
            if (Pacientes.Any())
            {
                // Si hay pacientes, el contador empieza desde el ID más alto existente + 1.
                contadorPacientes = Pacientes.Max(p => int.Parse(p.Id.Substring(1))) + 1;
            }
            else
            {
                // Si no hay pacientes, el contador empieza en 1.
                contadorPacientes = 1;
            }
            
            if (Personal.Any(p => p.Id.StartsWith("M")))
            {
                // Si hay personal con ID que comienza con 'M', el contador empieza desde el ID más alto existente + 1.
                contadorPersonal = Personal.Where(p => p.Id.StartsWith("M"))
                                           .Max(p => int.Parse(p.Id.Substring(1))) + 1;
            }
            else
            {
                // Si no hay personal con ID 'M', el contador empieza en 1.
                contadorPersonal = 1; 
            }
            
            contadorRegistros = 1; // Este se podría calcular si fuera necesario

            // Configuración de datos iniciales del sistema que no se persisten automáticamente.
            InicializarHospitales(); // Carga hospitales predefinidos.
            InicializarArbolDiagnostico(); // Configura el árbol de decisiones para diagnósticos.
            
            // Crea un administrador por defecto si no existe y lo guarda.
            InicializarAdministradorPorDefecto();
        }
        
        // Guarda un usuario (Paciente o PersonalHospitalario) usando el sistema de persistencia.
        public void GuardarUsuario(Usuario usuario)
        {
            _persistencia.GuardarRegistro(usuario);
        }
        
        // Elimina un usuario del sistema (paciente o personal) y de la persistencia.
        public void EliminarUsuario(string idUsuario)
        {
            // Intenta eliminar de la lista de Pacientes.
            Paciente pacienteAEliminar = Pacientes.FirstOrDefault(p => p.Id == idUsuario);
            if (pacienteAEliminar != null)
            {
                Pacientes.Remove(pacienteAEliminar);
            }
            else
            {
                // Si no es un paciente, intenta eliminar de la lista de Personal.
                PersonalHospitalario personalAEliminar = Personal.FirstOrDefault(p => p.Id == idUsuario);
                if (personalAEliminar != null)
                {
                    Personal.Remove(personalAEliminar);
                    // Si el personal estaba asignado a un hospital, también se elimina de la lista de IDs de personal de ese hospital.
                    Hospital hospitalAsignado = Hospitales.FirstOrDefault(h => h.Id == personalAEliminar.IdHospital);
                    if (hospitalAsignado != null && hospitalAsignado.PersonalIds.Contains(personalAEliminar.Id))
                    {
                        hospitalAsignado.PersonalIds.Remove(personalAEliminar.Id);
                    }
                }
            }
            // Elimina el registro del usuario de la persistencia.
            _persistencia.EliminarRegistro(idUsuario);
        }
        
        // Inicializa la lista de hospitales con datos predefinidos.
        private void InicializarHospitales()
        {
            Hospitales = new List<Hospital>
            {
                new Hospital { Id = "H001", Nombre = "Hospital Manolo Morales Peralta", EsPublico = true, CostoConsulta = 0, PrecisionDiagnostico = 85, TiempoPromedioMin = 45 },
                new Hospital { Id = "H002", Nombre = "Hospital Velez Paiz", EsPublico = true, CostoConsulta = 0, PrecisionDiagnostico = 82, TiempoPromedioMin = 50 },
                new Hospital { Id = "H003", Nombre = "Hospital Bautista", EsPublico = false, CostoConsulta = 200.00m, PrecisionDiagnostico = 95, TiempoPromedioMin = 25 },
                new Hospital { Id = "H004", Nombre = "Hospital Vivian Pellas", EsPublico = false, CostoConsulta = 220.00m, PrecisionDiagnostico = 97, TiempoPromedioMin = 20 }
            };

            // Para cada hospital, inicializa su cola de pacientes y su lista de registros médicos.
            foreach (var hospital in Hospitales)
            {
                ColasPorHospital[hospital.Id] = new Queue<string>(); // Cola de IDs de pacientes esperando.
                RegistrosPorHospital[hospital.Id] = new List<RegistroMedico>(); // Lista de registros médicos del hospital.
            }
        }

        // Inicializa un usuario administrador por defecto si no existe en el sistema.
        private void InicializarAdministradorPorDefecto()
        {
            // Verifica si ya existe un administrador con el ID "ADMIN001".
            if (Personal.Exists(p => p.Id == "ADMIN001"))
                return; // Si existe, no hace nada.

            // Crea un nuevo objeto PersonalHospitalario para el administrador por defecto.
            PersonalHospitalario adminDefault = new PersonalHospitalario
            {
                Id = "ADMIN001",
                Nombre = "Administrador General",
                Email = "admin@medicenter.com",
                Password = "admin123",
                IdHospital = "H001", // Asigna al primer hospital por defecto.
                NivelAcceso = NivelAcceso.Administrador, // Define su nivel de acceso.
                CambioPassword = true // Indica que debe cambiar su contraseña en el primer login.
            };

            Personal.Add(adminDefault); // Añade el administrador a la lista de personal.
            GuardarUsuario(adminDefault); // Guarda el administrador en la persistencia.
        }

        // Inicializa el árbol de decisiones para el diagnóstico de síntomas.
        private void InicializarArbolDiagnostico()
        {
            // Nivel 1: Nodo raíz con la primera pregunta.
            ArbolDiagnostico = new DecisionNode("root", "¿Tiene fiebre?");

            // --- RAMA: Tiene fiebre (SI) ---
            // Nodo que sigue si la respuesta a "¿Tiene fiebre?" es "sí".
            DecisionNode nodoFiebreSi = new DecisionNode("fiebre_si", "¿Tiene tos persistente y dificultad para respirar?");
            nodoFiebreSi.RespuestaEsperada = "si";

            // Nodo que sigue si la respuesta es "sí" a la tos y dificultad.
            DecisionNode nodoTosDificultadSi = new DecisionNode("tos_dificultad_si", "¿Siente dolor en el pecho o fatiga extrema?");
            nodoTosDificultadSi.RespuestaEsperada = "si";

            // Diagnóstico final crítico si la respuesta es "sí".
            DecisionNode diagNeumoniaCovid = new DecisionNode("diag_neumonia_covid", "CRÍTICO: Posible neumonía, COVID-19 o bronquitis severa. Consulte médico inmediatamente.", true);
            diagNeumoniaCovid.RespuestaEsperada = "si";

            // Diagnóstico final moderado si la respuesta es "no".
            DecisionNode diagGripeFuerte = new DecisionNode("diag_gripe_fuerte", "ADVERTENCIA: Posible gripe fuerte o infección respiratoria. Reposo, hidratación y monitoreo. Consulte si empeora.", true);
            diagGripeFuerte.RespuestaEsperada = "no";

            nodoTosDificultadSi.AgregarHijo(diagNeumoniaCovid); // Agrega el diagnóstico crítico como hijo.
            nodoTosDificultadSi.AgregarHijo(diagGripeFuerte); // Agrega el diagnóstico moderado como hijo.

            // Nodo que sigue si la respuesta es "no" a la tos y dificultad.
            DecisionNode nodoTosDificultadNo = new DecisionNode("tos_dificultad_no", "¿Tiene dolor de cabeza intenso o dolor muscular generalizado?");
            nodoTosDificultadNo.RespuestaEsperada = "no";

            // Diagnóstico final moderado si la respuesta es "sí".
            DecisionNode diagMigranaInfeccionViral = new DecisionNode("diag_migrana_viral", "MODERADO: Posible migraña severa o infección viral. Analgésicos, reposo. Consulte si la fiebre persiste.", true);
            diagMigranaInfeccionViral.RespuestaEsperada = "si";

            // Diagnóstico final leve si la respuesta es "no".
            DecisionNode diagFiebreLeve = new DecisionNode("diag_fiebre_leve", "LEVE: Fiebre leve. Reposo, hidratación y antipiréticos. Monitoreo en casa.", true);
            diagFiebreLeve.RespuestaEsperada = "no";

            nodoTosDificultadNo.AgregarHijo(diagMigranaInfeccionViral); // Agrega el diagnóstico moderado como hijo.
            nodoTosDificultadNo.AgregarHijo(diagFiebreLeve); // Agrega el diagnóstico leve como hijo.

            nodoFiebreSi.AgregarHijo(nodoTosDificultadSi); // Conecta la rama de tos/dificultad a la rama de fiebre "sí".
            nodoFiebreSi.AgregarHijo(nodoTosDificultadNo); // Conecta la otra rama de tos/dificultad a la rama de fiebre "sí".


            // --- RAMA: No tiene fiebre (NO) ---
            // Nodo que sigue si la respuesta a "¿Tiene fiebre?" es "no".
            DecisionNode nodoFiebreNo = new DecisionNode("fiebre_no", "¿Tiene náuseas, vómitos o dolor abdominal?");
            nodoFiebreNo.RespuestaEsperada = "no";

            // Nodo que sigue si la respuesta es "sí" a náuseas/vómitos/dolor abdominal.
            DecisionNode nodoNauseasVomitosSi = new DecisionNode("nauseas_vomitos_si", "¿Presenta diarrea o deshidratación?");
            nodoNauseasVomitosSi.RespuestaEsperada = "si";

            // Diagnóstico final moderado si la respuesta es "sí".
            DecisionNode diagGastroenteritis = new DecisionNode("diag_gastroenteritis", "MODERADO: Posible gastroenteritis o intoxicación alimentaria. Hidratación, dieta blanda. Consulte si los síntomas persisten o empeoran.", true);
            diagGastroenteritis.RespuestaEsperada = "si";

            // Diagnóstico final leve si la respuesta es "no".
            DecisionNode diagIndigestionLeve = new DecisionNode("diag_indigestion_leve", "LEVE: Indigestión leve. Evite comidas pesadas, antiácidos. Monitoreo en casa.", true);
            diagIndigestionLeve.RespuestaEsperada = "no";

            nodoNauseasVomitosSi.AgregarHijo(diagGastroenteritis); // Agrega el diagnóstico de gastroenteritis como hijo.
            nodoNauseasVomitosSi.AgregarHijo(diagIndigestionLeve); // Agrega el diagnóstico de indigestión como hijo.

            // Nodo que sigue si la respuesta es "no" a náuseas/vómitos/dolor abdominal.
            DecisionNode nodoNauseasVomitosNo = new DecisionNode("nauseas_vomitos_no", "¿Experimenta dolor de garganta sin tos o congestión nasal?");
            nodoNauseasVomitosNo.RespuestaEsperada = "no";

            // Diagnóstico final leve si la respuesta es "sí".
            DecisionNode diagAmigdalitis = new DecisionNode("diag_amigdalitis", "LEVE: Posible irritación de garganta o amigdalitis leve. Beba líquidos, evite irritantes. Consulte si hay pus o fiebre alta.", true);
            diagAmigdalitis.RespuestaEsperada = "si";
            
            // Diagnóstico final leve si la respuesta es "no".
            DecisionNode diagSintomasLeves = new DecisionNode("diag_sintomas_leves", "LEVE: Síntomas leves o inespecíficos. Monitoreo general. Si aparecen nuevos síntomas, reevaluar.", true);
            diagSintomasLeves.RespuestaEsperada = "no";

            nodoNauseasVomitosNo.AgregarHijo(diagAmigdalitis); // Agrega el diagnóstico de amigdalitis como hijo.
            nodoNauseasVomitosNo.AgregarHijo(diagSintomasLeves); // Agrega el diagnóstico de síntomas leves como hijo.

            nodoFiebreNo.AgregarHijo(nodoNauseasVomitosSi); // Conecta la rama de náuseas/vómitos a la rama de fiebre "no".
            nodoFiebreNo.AgregarHijo(nodoNauseasVomitosNo); // Conecta la otra rama a la rama de fiebre "no".

            ArbolDiagnostico.AgregarHijo(nodoFiebreSi); // Añade la rama "sí tiene fiebre" a la raíz del árbol.
            ArbolDiagnostico.AgregarHijo(nodoFiebreNo); // Añade la rama "no tiene fiebre" a la raíz del árbol.
        }

        // Busca un paciente en la lista de pacientes por ID y contraseña.
        public Paciente BuscarPaciente(string id, string password)
        {
            return Pacientes.Find(p => p.Id == id && p.Password == password);
        }

        // Busca personal hospitalario en la lista de personal por ID y contraseña.
        public PersonalHospitalario BuscarPersonal(string id, string password)
        {
            return Personal.Find(p => p.Id == id && p.Password == password);
        }

        // Busca un hospital en la lista de hospitales por ID.
        public Hospital BuscarHospital(string id)
        {
            return Hospitales.Find(h => h.Id == id);
        }

        // Obtiene una lista de hospitales disponibles según el tipo de seguro del paciente.
        public List<Hospital> ObtenerHospitalesDisponibles(TipoSeguro seguro)
        {
            List<Hospital> disponibles = new List<Hospital>();

            switch (seguro)
            {
                case TipoSeguro.SeguroCompleto:
                    // Con seguro completo, todos los hospitales están disponibles.
                    return new List<Hospital>(Hospitales);
                case TipoSeguro.SeguroBasico:
                case TipoSeguro.SinSeguro:
                    // Con seguro básico o sin seguro, solo los hospitales públicos están disponibles.
                    disponibles = Hospitales.FindAll(h => h.EsPublico);
                    break;
            }

            return disponibles;
        }

        // Obtiene una lista de hospitales privados.
        public List<Hospital> ObtenerHospitalesPrivados()
        {
            return Hospitales.FindAll(h => !h.EsPublico);
        }

        // Genera un nuevo ID único para un paciente.
        public string GenerarIdPaciente()
        {
            string id = $"P{contadorPacientes:D4}"; // Formato P0001, P0002, etc.
            contadorPacientes++; // Incrementa el contador para el próximo ID.
            return id;
        }

        // Genera un nuevo ID único para personal hospitalario.
        public string GenerarIdPersonal()
        {
            string id = $"M{contadorPersonal:D4}"; // Formato M0001, M0002, etc.
            contadorPersonal++; // Incrementa el contador para el próximo ID.
            return id;
        }

        // Genera un nuevo ID único para un registro médico.
        public string GenerarIdRegistro()
        {
            string id = $"R{contadorRegistros:D5}"; // Formato R00001, R00002, etc.
            contadorRegistros++; // Incrementa el contador para el próximo ID.
            return id;
        }
    }
}
﻿