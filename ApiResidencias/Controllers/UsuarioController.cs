using ApiResidencias.Helpers;
using ApiResidencias.Models.DTO_s;
using ApiResidencias.Models.DTOs;
using ApiResidencias.Models.Entities;
using ApiResidencias.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ApiResidencias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        Repository<Usuario> usuarioRepository;
        Repository<Alumno> alumnoRepository;
        Repository<DivisionAcademica> divisionRepository;
        Repository<Coordinador> coordinadorRepository;
        Repository<AlumnoTarea> alumnoTareaRepository;
        Repository<Tarea> tareaRepository;

        Cifrado cf = new Cifrado();
        public UsuarioController(residenciasContext residenciasContext)
        {
            usuarioRepository = new(residenciasContext);
            alumnoRepository = new(residenciasContext);
            divisionRepository = new(residenciasContext);
            coordinadorRepository = new(residenciasContext);
            alumnoTareaRepository = new(residenciasContext);
            tareaRepository = new(residenciasContext);
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        #region POST
        [HttpPost]
        public IActionResult Post(UsuarioDTO usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario.Correo))
                {
                    return BadRequest("El correo no debe ir vacio");
                }
                if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    return BadRequest("La contraseña no debe ir vacia");

                }
                if (usuarioRepository.Get().FirstOrDefault(x => x.Correo.ToUpper() == usuario.Correo.ToUpper()) != null)
                    return BadRequest("Ya hay un alumno con ese correo vinculado");
                //HACEMOS UN USUARIO
                Usuario u = new()
                {
                    IdUsuario = 0,
                    Correo = usuario.Correo.ToUpper(),
                    Contrasena = usuario.Contrasena,
                    IdTipoUsuario = usuario.IdTipoUsuario
                };

                usuarioRepository.Insert(u);

                return Ok(u.IdUsuario);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [HttpPost("Alumno")]
        public IActionResult PostAlumno(UsuarioAlumnoDTO usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario.Correo))
                {
                    return BadRequest("El correo no debe ir vacio");
                }
                usuario.Contrasena = "ESCOLARES";
                if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    return BadRequest("La contraseña no debe ir vacia");

                }
                if (usuarioRepository.Get().FirstOrDefault(x => x.Correo.ToUpper() == usuario.Correo.ToUpper()) != null)
                    return BadRequest("Ya hay un alumno con ese correo vinculado");

                //VALIDACION
                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                {
                    return BadRequest("El nombre no debe ir vacio");
                }
                if (string.IsNullOrWhiteSpace(usuario.APaterno))
                {
                    return BadRequest("El apellido paterno no debe ir vacio");
                }

                if (string.IsNullOrWhiteSpace(usuario.AMaterno))
                {
                    return BadRequest("El apellido materno no debe ir vacio");
                }

                if (string.IsNullOrWhiteSpace(usuario.NumeroControl))
                {
                    return BadRequest("El numero de control no debe ir vacio");
                }
                if (alumnoRepository.Get().FirstOrDefault(x => x.NumeroControl.Trim().ToUpper() == usuario.NumeroControl) != null)
                    return BadRequest("Ya hay un alumno con ese numero de control vinculado");

                if (!divisionRepository.Get().Any(x => x.IdDivisionAcademica == usuario.DivisionAcademica))
                    return BadRequest("No se encontro la division academica");

                //HACEMOS UN USUARIO
                Usuario u = new()
                {
                    IdUsuario = 0,
                    Correo = usuario.Correo.ToUpper(),
                    Contrasena = cf.CifradoTexto(usuario.Contrasena),
                    IdTipoUsuario = 3
                };

                usuarioRepository.Insert(u);

                Alumno alumno = new()
                {
                    IdAlumno = 0,
                    Nombre = usuario.Nombre.ToUpper(),
                    APaterno = usuario.APaterno.ToUpper(),
                    AMaterno = usuario.AMaterno.ToUpper(),
                    NumeroControl = usuario.NumeroControl.ToUpper(),
                    Activo = true,
                    IdDivisionAcademica = usuario.DivisionAcademica,
                    IdUsuario = u.IdUsuario
                };

                alumnoRepository.Insert(alumno);

                AsignarTareas(alumno);

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        private void AsignarTareas(Alumno alumno)
        {
            var yearActual = new DateTime(DateTime.Now.ToMexicoTime().Year, 01, 01);
            //Tarea 1 - Seleccion de la empresa de residencia

            Asignar(yearActual.AddMonths(7).AddDays(27), alumno, "1. Selección de empresa de residencia",
    @"Estimados estudiantes,

En esta primera tarea, su objetivo principal es iniciar la búsqueda de una empresa o institución donde deseen realizar su Residencia Profesional en el campo de la Ingeniería en Sistemas Computacionales. Esta experiencia les brindará la oportunidad de aplicar sus conocimientos teóricos en un entorno práctico y adquirir una valiosa experiencia laboral.

Para comenzar con el proceso de selección, se les recomienda utilizar diferentes recursos disponibles, como bolsas de trabajo, contactos personales y el apoyo de profesores. Estas fuentes pueden proporcionarles oportunidades adecuadas y abrir puertas hacia empresas que estén alineadas con sus intereses y metas profesionales.

Es fundamental que nos informen lo antes posible una vez que hayan asegurado una empresa para realizar su residencia. Esta notificación es necesaria para que podamos asignarles un asesor apropiado y garantizar un seguimiento efectivo durante todo el proceso.

Recuerden que la búsqueda de una empresa para su Residencia Profesional requiere tiempo y esfuerzo, por lo tanto, se les sugiere comenzar lo antes posible y mantener una comunicación constante con nosotros. 

Entregable:

Un documento conteniendo los datos de la empresa y una descripción del proyecto o de las actividades que realizarán en la empresa.



¡Les deseamos mucho éxito en su búsqueda de empresa para la Residencia Profesional!");

            //Tarea 2 - Carta de presentacion
            Asignar(yearActual.AddMonths(7).AddDays(27), alumno, "2. Carta de Presentación",
              @"En esta segunda actividad, deben solicitar una carta de presentación en el Departamento de Residencia Profesional y Servicio Social de nuestra institución una vez que hayan identificado una empresa donde deseen realizar su Residencia. Esta carta de presentación es un requisito importante y también se solicita a aquellos alumnos que llevarán a cabo residencias en sus lugares de trabajo.

Asegúrense de presentar la documentación requerida y explicar los detalles de la empresa en la que desean hacer la Residencia. Los datos que se les solicitarán son: la Razón Social de la empresa, el nombre de la persona a la que va dirigida la carta y su puesto dentro de la empresa, el número de control, el nombre completo y la carrera del estudiante.

Pueden solicitar la carta de presentación por correo electrónico a residencia@rcarbonifera.tecnm.mx o de manera presencial en el departamento de Servicio Social y Residencias Profesionales. Les recomendamos solicitar tantas cartas como sean necesarias para cubrir sus requerimientos.

Una vez que hayan recibido la carta de presentación, llévenla a la empresa donde realizarán la Residencia Profesional y asegúrense de obtener el acuse correspondiente (firma y sello de la empresa). La carta con el acuse debe ser entregada en formato PDF por correo electrónico o entregado físicamente en el Departamento de Servicio Social y Residencia Profesional.

La carta de presentación estará disponible para su entrega al día hábil siguiente, pueden solicitar que se las envíen a su correo o recogerla personalmente en el departamento.

Ante cualquier duda o consulta adicional, no duden en comunicarse con nosotros en el canal General.



Entregable:

Solicitud de la carta de presentación con el acuse adjunto en formato PDF.

");
            //Tarea 3 - Carta de compromiso
            Asignar(yearActual.AddMonths(7).AddDays(27), alumno, "3. Carta compromiso",
    @"En esta tercera actividad, una vez que te hayan asignado un asesor interno y un asesor externo para tu Residencia Profesional en el campo de la Ingeniería en Sistemas Computacionales, te solicitamos que llenes la Carta Compromiso. Este documento representa tu responsabilidad de cumplir con el programa de Residencia Profesional de acuerdo con los reglamentos y políticas establecidos por el TecNM Campus Región Carbonífera. 

Tu asesor interno designado será el jefe de la División Académica de Ingeniería en Sistemas Computacionales, M.I. Oscar Raúl Sánchez Flores. El asesor interno designará a un docente de la academia de Ingeniería en Sistemas Computacionales como coasesor interno. La asignación del coasesor interno se realizará en función de los objetivos del proyecto. 

El coasesor interno se encargará de brindarte orientación y apoyo durante el desarrollo de tu Residencia Profesional. Además, será el encargado de evaluar tu desempeño en base a criterios específicos establecidos para el proyecto asignado. 

Por otro lado, el asesor externo será designado por la empresa y tendrá la función de brindarte orientación específica sobre el proyecto y las actividades que llevarás a cabo, así como evaluar tu desempeño y proporcionarte retroalimentación.  

Tanto el asesor externo como el coasesor serán los responsables de evaluar tu desempeño durante la Residencia Profesional. La función de asesor interno será validar las calificaciones otorgadas por los asesores externo y coasesor. 

El promedio de las evaluaciones realizadas por el asesor externo y el coasesor será considerado para asignar tu calificación. El asesor interno, a través de su función de validación, asegurará que las calificaciones sean justas y consistentes. 

Una vez tengas asignado tanto el asesor interno como el asesor externo, procede a llenar la Carta Compromiso. Puedes descargar el formulario PDF desde el sitio web del TecNM Campus Región Carbonífera (https://www.itesrc.edu.mx/descripcion.php?type=residencias-profesionales), específicamente en la sección de archivos relacionados bajo el título ""01. CARTA COMPROMISO DE RESIDENCIA PROFESIONAL"". Asegúrate de completar todos los campos requeridos de manera precisa en la Carta Compromiso. 

Duración de la Residencia: Planifica y asegúrate de cumplir con 500 horas en un plazo mínimo de 4 meses y un máximo de 6 meses. 

Una vez que hayas llenado la Carta Compromiso, puedes entregarla al Departamento de Residencia Profesional y Servicio Social. Puedes hacerlo enviando el formulario en formato PDF por correo electrónico a residencia@rcarbonifera.tecnm.mx o entregándolo de manera presencial en el departamento. Recuerda cumplir con los plazos establecidos por el departamento para la entrega de la Carta Compromiso. 

 

Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Carta compromiso en formato PDF ");
            //Tarea 4 - Plan de reuniones
            Asignar(yearActual.AddMonths(8).AddDays(28), alumno, "4. Plan de reuniones",
@"En esta etapa crucial de tu Residencia Profesional, es de suma importancia establecer una comunicación efectiva y una programación sólida de asesorías con tu coasesor interno. Esto asegurará un seguimiento constante y eficiente de tu proyecto.  

Deberás iniciar el diálogo con tu coasesor interno, designado por el M.I. Oscar Raúl Sánchez Flores, para discutir y acordar la forma y el calendario de las revisiones y seguimientos del proyecto. Podrás comunicarte mediante reuniones presenciales, intercambio de correos electrónicos, videoconferencias u otras modalidades que sean adecuadas para ambas partes. 

Trabajando en conjunto con tu coasesor interno, diseñarás un calendario de asesorías que contemple al menos cuatro momentos de comunicación.  

La primera reunión se enfocará en definir las características iniciales del proyecto (informe preliminar). Las siguientes dos reuniones se centrarán en la evaluación en dos etapas intermedias del desarrollo, y la última reunión será para evaluar el informe final de la Residencia Profesional. 

Una vez que hayas acordado el calendario de asesorías con tu coasesor interno, deberás documentar este plan. El calendario, que debe contener fechas, modalidades y objetivos de cada reunión. septiembre. 

En caso de que no recibas respuesta por parte de tu coasesor interno, puedes contactar a M.I. Oscar Raúl Sánchez Flores, Jefe de la División Académica de Ingeniería en Sistemas Computacionales, para garantizar una comunicación fluida y efectiva. 

Recuerda que estas reuniones planificadas y la comunicación constante con tu coasesor interno son esenciales para asegurar el éxito y la calidad de tu proyecto de Residencia Profesional. Aprovecha al máximo estas oportunidades para recibir orientación, resolver dudas y mantener a tus asesores al tanto de tu progreso. 

 

Fecha Límite para la Entrega del Calendario de Asesorías en Teams: 29 de septiembre. 

 

Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Calendario de asesorías en formato PDF ");
            //Tarea 5 -  Reporte preeliminar
            Asignar(yearActual.AddMonths(9).AddDays(5), alumno, "5. Reporte preliminar de residencias",
@"Una vez seleccionado el proyecto de Residencia Profesional, es necesario que el estudiante sostenga una reunión con su coasesor interno y su asesor externo. Esta reunión tiene como finalidad recibir la orientación necesaria para la confección del reporte preliminar del proyecto. 

Es importante comprender que el reporte preliminar del proyecto se asemeja a un bosquejo inicial que establece la dirección que tomarás durante tu Residencia Profesional. Este bosquejo se crea antes de iniciar la ejecución completa del proyecto y, por lo tanto, está diseñado para ser flexible. Es como el esbozo inicial de un gran proyecto, un mapa que puede ajustarse y evolucionar antes de llegar al informe final. Sin embargo, este informe preliminar proporciona una base sólida para tu trabajo y actúa como un punto de partida. Es un documento esencial para que tu coasesor interno entienda tus intenciones, te brinde orientación y te apoye en cada etapa del proceso. Su conocimiento temprano de tu enfoque y objetivos le permite asesorarte de manera efectiva, asegurando que estés encaminado desde el inicio. 

Estructura del Reporte Preliminar de Residencia Profesional: 

a) Nombre y Objetivo del Proyecto: 

Se debe presentar el nombre asignado al proyecto de Residencia Profesional y se procederá a establecer con claridad y concisión el objetivo global que se persigue con la ejecución de dicho proyecto. 

b) Delimitación: 

En esta sección, se precisarán los alcances y límites del proyecto, definiendo de manera específica qué aspectos serán abordados y cuáles quedarán fuera del ámbito de trabajo. 

c) Objetivos: 

Se detallarán los objetivos particulares que se plantean para la Residencia Profesional, describiendo las metas específicas que se tienen como meta alcanzar a lo largo de la ejecución del proyecto. 

d) Justificación: 

En este apartado, se expondrán los motivos y razones que fundamentan la relevancia y necesidad de llevar a cabo el proyecto de Residencia Profesional. Se explicará cómo el proyecto contribuirá al campo de la Ingeniería en Sistemas Computacionales. 

e) Cronograma Preliminar de Actividades: 

Se elaborará un cronograma tentativo que contendrá las fases y fechas aproximadas para el desarrollo de las diversas actividades previstas en la ejecución del proyecto. 

f) Descripción Detallada de las Actividades: 

En esta sección, se proporcionará una descripción minuciosa de las actividades que se llevarán a cabo durante el periodo de Residencia Profesional. Se incluirán detalles acerca de los métodos, herramientas y tecnologías que serán utilizadas en cada una de ellas. 

g) Lugar de Realización del Proyecto: 

Se indicará el sitio físico donde se llevará a cabo la Residencia Profesional, ya sea en las instalaciones de la empresa, organismo o dependencia asociada al proyecto, u otro entorno relevante. 

h) Información sobre la Empresa, Organismo o Dependencia: 

Se proporcionará contexto sobre la empresa, organismo o dependencia para la cual se está ejecutando el proyecto de Residencia Profesional. Esto incluirá detalles sobre su misión, visión, actividades primordiales y su relevancia en el sector. 

El estudiante se compromete a confeccionar el reporte preliminar siguiendo esta estructura y a someterlo a la revisión y aprobación de su coasesor interno y su asesor externo. La retroalimentación brindada en esta etapa será esencial para el desarrollo exitoso del proyecto de Residencia Profesional. 

Fecha Límite para la Entrega del Reporte Preliminar: 6 de octubre. 

El reporte preliminar aprobado deberá ser remitido por correo electrónico a la dirección residencia@rcarbonifera.tecnm.mx y también será cargado en la plataforma Teams, según las instrucciones brindadas por el Departamento de Residencia Profesional y Servicio Social. Recuerda cumplir con los plazos establecidos por el departamento para la entrega. 

 

Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Reporte preliminar en formato PDF");
            // Tarea 6 -Primera evaluacion 

            Asignar(yearActual.AddMonths(9).AddDays(5), alumno, "6. Primera Evaluación del Seguimiento de Residencia Profesional",
@"Para garantizar el éxito en la acreditación de tu Residencia Profesional, es imprescindible cumplir con dos evaluaciones parciales de seguimiento, las cuales desempeñan un papel fundamental en tu calificación final, contribuyendo con un 10%. 

Comienza descargando el formato ""EVALUACIÓN Y SEGUIMIENTO RESIDENCIA PROFESIONAL"" desde la página https://www.itesrc.edu.mx/descripcion.php?type=residencias-profesionales. En este formato, proporcionarás información sobre tu progreso y rendimiento en la Residencia Profesional. 

Después de completar el formato, preséntalo al asesor externo de tu proyecto. El asesor externo evaluará diversos aspectos, como tu puntualidad, habilidades de trabajo en equipo, progreso en el proyecto y comportamiento en el entorno laboral. 

Recibirás una calificación del asesor externo y obtendrás comentarios sobre tu desempeño en la Residencia Profesional. 

Una vez evaluado por el asesor externo, lleva el formato al coasesor interno. El coasesor interno no evalúa directamente el proyecto, sino más bien tu cumplimiento con las reuniones y actividades programadas. 

El coasesor interno evaluará si has asistido puntualmente a las reuniones de asesoría programadas y si cumples con las actividades a tiempo. También evaluará si subes los entregables y avances al equipo de trabajo (Teams). 

Esta evaluación se centra en tu compromiso y responsabilidad en la Residencia Profesional, así como en tu capacidad para mantener comunicación y colaboración efectivas. 

Una vez que el coasesor interno haya evaluado y calificado el formato, este debe ser acompañado por el visto bueno del asesor externo, en este caso, Oscar Raúl Sánchez Flores. Esto certificará la evaluación y asegurará que ambas perspectivas hayan sido consideradas. 

Recuerda que este proceso de evaluación y seguimiento es fundamental para demostrar tu avance y dedicación en la Residencia Profesional. Asegúrate de completar y entregar el formato evaluado antes de las fechas límite correspondientes. 

 

Las fechas límite para el envío de los Formatos de Evaluación y Seguimiento son las siguientes: 

Primer Formato: 6 de octubre. 

Segundo Formato: 6 de noviembre. 

 

Una vez que tengas firmado el Formato de Evaluación y Seguimiento debes escanearlo y entregar el documento impreso al Departamento de Residencia Profesional y Servicio Social. Puedes hacerlo enviando el documento en formato PDF por correo electrónico a residencia@rcarbonifera.tecnm.mx o entregándolo impreso de manera presencial en el departamento.  

 

Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Formatos de Evaluación y Seguimiento en formato PDF");
            //Tarea 7- Segunda evaluacion
            Asignar(yearActual.AddMonths(10).AddDays(5), alumno, "7. Segunda Evaluación del Seguimiento de Residencia Profesional",
@"Para garantizar el éxito en la acreditación de tu Residencia Profesional, es imprescindible cumplir con dos evaluaciones parciales de seguimiento, las cuales desempeñan un papel fundamental en tu calificación final, contribuyendo con un 10%. 

Comienza descargando el formato ""EVALUACIÓN Y SEGUIMIENTO RESIDENCIA PROFESIONAL"" desde la página https://www.itesrc.edu.mx/descripcion.php?type=residencias-profesionales. En este formato, proporcionarás información sobre tu progreso y rendimiento en la Residencia Profesional. 

Después de completar el formato, preséntalo al asesor externo de tu proyecto. El asesor externo evaluará diversos aspectos, como tu puntualidad, habilidades de trabajo en equipo, progreso en el proyecto y comportamiento en el entorno laboral. 

Recibirás una calificación del asesor externo y obtendrás comentarios sobre tu desempeño en la Residencia Profesional. 

Una vez evaluado por el asesor externo, lleva el formato al coasesor interno. El coasesor interno no evalúa directamente el proyecto, sino más bien tu cumplimiento con las reuniones y actividades programadas. 

El coasesor interno evaluará si has asistido puntualmente a las reuniones de asesoría programadas y si cumples con las actividades a tiempo. También evaluará si subes los entregables y avances al equipo de trabajo (Teams). 

Esta evaluación se centra en tu compromiso y responsabilidad en la Residencia Profesional, así como en tu capacidad para mantener comunicación y colaboración efectivas. 

Una vez que el coasesor interno haya evaluado y calificado el formato, este debe ser acompañado por el visto bueno del asesor externo, en este caso, Oscar Raúl Sánchez Flores. Esto certificará la evaluación y asegurará que ambas perspectivas hayan sido consideradas. 

Recuerda que este proceso de evaluación y seguimiento es fundamental para demostrar tu avance y dedicación en la Residencia Profesional. Asegúrate de completar y entregar el formato evaluado antes de las fechas límite correspondientes. 

 

Las fechas límite para el envío de los Formatos de Evaluación y Seguimiento son las siguientes: 

Primer Formato: 6 de octubre. 

Segundo Formato: 6 de noviembre. 

 

Una vez que tengas firmado el Formato de Evaluación y Seguimiento debes escanearlo y entregar el documento impreso al Departamento de Residencia Profesional y Servicio Social. Puedes hacerlo enviando el documento en formato PDF por correo electrónico a residencia@rcarbonifera.tecnm.mx o entregándolo impreso de manera presencial en el departamento.  

 

Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Formatos de Evaluación y Seguimiento en formato PDF");
            //Tarea 8 - Informe final
            Asignar(yearActual.AddMonths(11).AddDays(14), alumno, "8. Elaboración del informe final",
@"En esta fase de tu Residencia Profesional, llega el momento de abordar la creación de tu reporte final. Este documento, esencial para la acreditación de la residencia y puede ser posible usarlo para titulación. 

Un enfoque recomendado es abordar la elaboración del reporte de manera progresiva a lo largo de todo el tiempo de la residencia. Esta metodología permite distribuir la carga de trabajo y evitar el riesgo de acumular tareas hacia el final del período de residencias. Además, contribuye a la calidad del reporte, ya que brinda la oportunidad de recibir retroalimentación temprana y hacer mejoras en etapas previas a la entrega final. 

La estructura detallada del informe final se encuentra disponible para su descarga en el archivo denominado ""05. ESTRUCTURA DEL REPORTE PRELIMINAR DE RESIDENCIA PROFESIONAL"". Puedes encontrar este archivo en la sección de ""Residencias Profesionales"" en la página web: https://www.itesrc.edu.mx/descripcion.php?type=residencias-profesionales. 

Si en el proyecto participan dos o más estudiantes, se elaborará un solo reporte de residencia profesional que integrará las actividades desarrolladas por cada estudiante. 

Considera la plataforma Teams como una herramienta útil en este proceso. A medida que avances en la elaboración del reporte, puedes utilizar la actividad correspondiente en Teams para compartir tus avances con tu coasesor interno. Esto permitirá que tu coasesor interno evalúe tus progresos y te ofrezca observaciones constructivas para perfeccionar tu trabajo. 

Recuerda que la fecha límite para cargar tu reporte final en la plataforma Teams es el 15 de diciembre. Asegurarte de entregar antes de esa fecha, lo que te brindará el tiempo necesario para realizar ajustes en base a las recomendaciones de tu coasesor interno. 

 

 

Entregable en Teams: 

Reporte final editable para ser revisado por el coasesor");
            //Tarea 9 - Evaluacion del informe final
            Asignar(yearActual.AddMonths(11).AddDays(14), alumno, "9. Evaluación del informe final",
@"Llegando a la etapa final de tu Residencia Profesional, te será evaluado el Reporte Final. Esta evaluación, realizada por tus asesores y determinará la calificación que reflejará tu desempeño en la materia de Residencias Profesionales. 

Descarga el formato ""EVALUACIÓN DE REPORTE DE RESIDENCIA PROFESIONAL"" desde la página web: https://www.itesrc.edu.mx/descripcion.php?type=residencias-profesionales. Tu asesor externo y coasesor interno serán los encargados de evaluar tu reporte final utilizando este formato. El asesor externo evaluará aspectos relacionados con el rendimiento en la empresa, la calidad del trabajo y habilidades técnicas. Por otro lado, el coasesor interno se enfocará en la coherencia con los objetivos planteados y tu desempeño global durante la Residencia Profesional. 

Si en el proyecto participan dos o más estudiantes, se elaborará un solo reporte de residencia profesional que integrará las actividades desarrolladas por cada estudiante. 

Es fundamental subrayar que la evaluación del reporte final tiene un peso significativo en tu calificación final, representando un 80% de la misma. Por tanto, se espera que este informe refleje de manera precisa y detallada tus logros y aprendizajes a lo largo de la Residencia Profesional. 

Una vez que el coasesor interno haya evaluado y calificado el formato, este debe ser acompañado por el visto bueno del asesor externo, en este caso, Oscar Raúl Sánchez Flores. Esto certificará la evaluación y asegurará que ambas perspectivas hayan sido consideradas. 

 

Fecha límite: 15 de diciembre. 

 

Una vez que tengas firmado el documento debes escanearlo y entregarlo al Departamento de Servicio Social y Residencia Profesional. Puedes hacerlo enviando el documento en formato PDF por correo electrónico a residencia@rcarbonifera.tecnm.mx o entregándolo impreso de manera presencial en el departamento.  

 

Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Evaluación del reporte final en formato PDF");
            //Tarea 10 - Liberacion
            Asignar(yearActual.AddYears(1).AddDays(11), alumno, "10.Liberación de Residencia Profesional",
           @"La culminación exitosa de tu Residencia Profesional se define mediante la satisfacción de los siguientes requisitos: 

 

Entrega del Reporte de Residencia Profesional: Asegúrate de presentar tu reporte de Residencia Profesional en formato PDF al Departamento de Servicio Social y Residencia Profesional a través del correo residencia@rcarbonifera.tecnm.mx. 
Solicita a la empresa la Carta de Terminación de Residencias. Este documento oficial confirma la conclusión satisfactoria de tu Residencia Profesional y se debe entregar a la División Académica de Ingeniería en Sistemas Computacionales. 
El asesor interno Oscar Raúl Sánchez Flores calificará la materia de Residencia Profesional al término del semestre en el SIE. 
Si tienes alguna pregunta adicional o necesitas orientación, no dudes en comunicarte con nosotros mediante el canal General. 

Entregable en Teams: 

Carta de terminación de Residencias en formato PDF ");
        }

        private void Asignar(DateTime fechaVencimiento, Alumno alumno, string nombre, string descripcion)
        {
            //Ver si existe la tarea para agregar o solo asignar la referencia
            var tareaAAsignar= tareaRepository.Get().FirstOrDefault(x=>x.NombreTarea == nombre && x.FechaVencimiento ==  fechaVencimiento);

            if (tareaAAsignar == null)
            {
                tareaAAsignar = new()
                {
                    IdTarea = 0,
                    NombreTarea = nombre,
                    Descripcion = descripcion,
                    FechaVencimiento = fechaVencimiento
                };
                tareaRepository.Insert(tareaAAsignar);
            }

            AlumnoTarea alumnoTarea = new AlumnoTarea();

            alumnoTarea = new AlumnoTarea()
            {
                Id = 0,
                IdTarea = tareaAAsignar.IdTarea,
                IdAlumno = alumno.IdAlumno,
                Estado = 3
            };
            alumnoTareaRepository.Insert(alumnoTarea);

        }

        [HttpPost("Coordinador")]
        public IActionResult PostCoordinador(UsuarioCoordinadorDTO usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario.Correo))
                {
                    return BadRequest("El correo no debe ir vacio");
                }
                if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                {
                    return BadRequest("La contraseña no debe ir vacia");

                }
                if (usuarioRepository.Get().FirstOrDefault(x => x.Correo.ToUpper() == usuario.Correo.ToUpper()) != null)
                    return BadRequest("Ya hay un coordinador con ese correo vinculado");

                //VALIDACION
                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                {
                    return BadRequest("El nombre no debe ir vacio");
                }

                if (!divisionRepository.Get().Any(x => x.IdDivisionAcademica == usuario.IdDivision))
                    return BadRequest("No se encontro la division academica");

                //HACEMOS UN USUARIO
                Usuario u = new()
                {
                    IdUsuario = 0,
                    Correo = usuario.Correo.ToUpper(),
                    Contrasena = cf.CifradoTexto(usuario.Contrasena),
                    IdTipoUsuario = 2
                };

                usuarioRepository.Insert(u);

                Coordinador coordinador = new()
                {
                    IdCoordinador = 0,
                    Nombre = usuario.Nombre,
                    IdUsuario = u.IdUsuario,
                    IdDivision = usuario.IdDivision
                };

                coordinadorRepository.Insert(coordinador);


                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region PUT


        [HttpPut("cambiarContra/alumno")] //Aqui se recibe entre los datos el id de ALUMNO * NO USUARIO
        public IActionResult CambiarContrAlumno(CambiarContraDTO cambiarContraDTO)
        {
            if(string.IsNullOrWhiteSpace(cambiarContraDTO.Contra) || string.IsNullOrWhiteSpace(cambiarContraDTO.Confirmacion))
                return BadRequest("Los datos no deben ir vacios");

            var usuario = alumnoRepository.Get()
                .Include(x=>x.IdUsuarioNavigation)
                .FirstOrDefault(x=>x.IdAlumno == cambiarContraDTO.Id);
            if (usuario == null)
                return NotFound();

            if (cambiarContraDTO.Contra != cambiarContraDTO.Confirmacion)
                return BadRequest("Las contraseñas no coinciden");

            usuario.IdUsuarioNavigation.Contrasena = cf.CifradoTexto(cambiarContraDTO.Contra);

            alumnoRepository.Update(usuario);

            return Ok();
        }

        [HttpPut("cambiarContra/admin")]
        public IActionResult CambiarContrAdmin(CambiarContraDTO cambiarContraDTO)
        {
            if (string.IsNullOrWhiteSpace(cambiarContraDTO.Contra) || string.IsNullOrWhiteSpace(cambiarContraDTO.Confirmacion))
                return BadRequest("Los datos no deben ir vacios");

            var usuario = usuarioRepository.Get(cambiarContraDTO.Id);
            if (usuario == null)
                return NotFound();

            if (cambiarContraDTO.Contra != cambiarContraDTO.Confirmacion)
                return BadRequest("Las contraseñas no coinciden");

            usuario.Contrasena = cf.CifradoTexto(cambiarContraDTO.Contra);

            usuarioRepository.Update(usuario);

            return Ok();
        }
        #endregion

    }
}
