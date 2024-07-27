using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Presupuesto.Entidades;
using Presupuesto.Migrations;
using Presupuesto.Models;
using Presupuesto.Servicios;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Presupuesto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext applicationDbContext; 
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IServicioUsuarios servicioUsuarios;

        public HomeController(ILogger<HomeController> logger, 
                              ApplicationDbContext applicationDbContext,  
                              IWebHostEnvironment webHostEnvironment, IServicioUsuarios servicioUsuarios)
        {
            this.applicationDbContext = applicationDbContext; 
            this.webHostEnvironment = webHostEnvironment;
            this.servicioUsuarios = servicioUsuarios;
            _logger = logger;
        }

        [HttpGet] 
        public IActionResult Index(string buscarObjetivo = "")
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (usuarioId is null)
            {
                return Forbid();
            }
            List<ObjetivoViewModel> objetivos = new List<ObjetivoViewModel>();
            //Filtrando a la persona
            if (!string.IsNullOrEmpty(buscarObjetivo))
            {
                objetivos = applicationDbContext.Objetivos
                   .Where(objetivos => objetivos.UsuarioCreacionId == usuarioId
                                    && objetivos.NombreObjetivo == buscarObjetivo).Select(objetivo => new ObjetivoViewModel
                   {
                       Id = objetivo.Id,
                       Nombre = objetivo.NombreObjetivo,
                       Descripcion = objetivo.Descripcion,
                       FechaLimite = objetivo.FechaLimite.Date,
                       Cantidad = objetivo.ObjetivoMonetario,
                       FormaPago = objetivo.FormaPago,
                       CantidadAdicional = objetivo.CantidadAdicional,
                       Activo = objetivo.Activo
                   }).ToList();
            }

            else
            {
                objetivos = applicationDbContext.Objetivos
                    .Where(objetivos => objetivos.UsuarioCreacionId == usuarioId)
                    .Select(objetivos => new ObjetivoViewModel
                {
                    Id = objetivos.Id,
                    Nombre = objetivos.NombreObjetivo,
                    Descripcion = objetivos.Descripcion,
                    FechaLimite = objetivos.FechaLimite.Date,
                    Cantidad = objetivos.ObjetivoMonetario,
                    FormaPago = objetivos.FormaPago,
                    CantidadAdicional = objetivos.CantidadAdicional,
                    Activo = objetivos.Activo

                }).ToList();
            }


            if (objetivos is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var modelo = new ListadoObjetivos();

            modelo.listadoObjetivos = objetivos;
            return View(modelo);
 
        }

        [HttpGet]
        public IActionResult AgregarObjetivo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarObjetivo(ObjetivoViewModel objetivoViewModel)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (usuarioId is null)
            {
                return Forbid();
            }

            if (objetivoViewModel.Cantidad <= 0)
            {
                ModelState.AddModelError(string.Empty, "Necesitas agregar una total a recaudar");
                return View(objetivoViewModel);
            }

            if (!ModelState.IsValid)
            {
                return View(objetivoViewModel);
            }

            var objetivo = new Objetivos();
            objetivo.NombreObjetivo = objetivoViewModel.Nombre;
            objetivo.Descripcion = objetivoViewModel.Descripcion;
            objetivo.ObjetivoMonetario = objetivoViewModel.Cantidad;
            objetivo.FechaLimite = objetivoViewModel.FechaLimite;
            objetivo.CantidadAdicional = objetivoViewModel.CantidadAdicional;
            objetivo.FormaPago = objetivoViewModel.FormaPago;
            objetivo.Activo = true;
            objetivo.UsuarioCreacionId = usuarioId;
            applicationDbContext.Add(objetivo);
            await applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditarObjetivo(int Id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
   
            var objetivo = applicationDbContext.Objetivos.FirstOrDefault
                (obj => obj.Id == Id && obj.UsuarioCreacionId == usuarioId);

            if(objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var modelo = new ObjetivoViewModel();
            modelo.Nombre = objetivo.NombreObjetivo;
            modelo.FechaLimite = objetivo.FechaLimite;
            modelo.Cantidad = objetivo.ObjetivoMonetario;
            modelo.CantidadAdicional = objetivo.CantidadAdicional;
            modelo.Descripcion = objetivo.Descripcion;
            modelo.FormaPago = objetivo.FormaPago;
            
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarObjetivo(int id, ObjetivoViewModel objetivoViewModel)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if(usuarioId is null)
            {
                return Forbid();
            }  

            var objetivo_a_editar = await applicationDbContext.Objetivos
                .FirstOrDefaultAsync(obj => obj.Id == id && obj.UsuarioCreacionId == usuarioId);

            if (objetivo_a_editar is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            if (objetivo_a_editar.ObjetivoMonetario != objetivoViewModel.Cantidad
                || objetivo_a_editar.CantidadAdicional != objetivoViewModel.CantidadAdicional)
            {
                objetivo_a_editar.HaSidoEditado = true;
            }

            if(objetivoViewModel.Cantidad <= 0)
            {
                ModelState.AddModelError(string.Empty, "Debe agregar un total a recaudar");
                return EditarObjetivo(objetivo_a_editar.Id);
            }

            objetivo_a_editar.NombreObjetivo = objetivoViewModel.Nombre;
            objetivo_a_editar.FechaLimite = objetivoViewModel.FechaLimite;
            objetivo_a_editar.ObjetivoMonetario = objetivoViewModel.Cantidad; 
            objetivo_a_editar.CantidadAdicional = objetivoViewModel.CantidadAdicional;
            objetivo_a_editar.Descripcion = objetivoViewModel.Descripcion;
            objetivo_a_editar.FormaPago = objetivoViewModel.FormaPago; 
             
            await applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EliminarObjetivo(int Id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (usuarioId is null)
            {
                return Forbid();
            }
            var objetivo = applicationDbContext.Objetivos
                .FirstOrDefault(obj => obj.Id == Id && obj.UsuarioCreacionId == usuarioId);

            if (objetivo is null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            ViewBag.message = objetivo;
             
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EliminarObjetivo(int Id, string message = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var objetivo = await applicationDbContext.Objetivos
                .FirstOrDefaultAsync(p => p.Id == Id && p.UsuarioCreacionId == usuarioId);

            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            objetivo.Activo = false; 
            await applicationDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ListadoPersonas(int Id, string buscarPersona)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            if (usuarioId is null)
            {
                return Forbid();
            }

            var objetivo = await applicationDbContext.Objetivos
                .FirstOrDefaultAsync(obj => obj.Id == Id && obj.UsuarioCreacionId == usuarioId);

            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            List<Trabajo> aportador = new List<Trabajo>();
                    //Filtrando a la persona
                    if (!string.IsNullOrEmpty(buscarPersona))
                    {
                        aportador = await applicationDbContext.Trabajo.Include(p => p.Transacciones)
                           .Where(persona => persona.ObjetivosId == Id && persona.Nombre == buscarPersona).Select(persona => new Trabajo
                           {
                               Id = persona.Id,
                               Nombre = persona.Nombre,
                               Nota = persona.Nota,
                               Aporte = persona.Transacciones.Sum(a => a.CantidadDepositada),
                               TiempoDeposito = objetivo.FechaLimite,
                               Cantidad_personas = persona.Cantidad_personas,
                               CantidadMenores = persona.CantidadMenores,
                               Total_a_pagar = (persona.Cantidad_personas * objetivo.ObjetivoMonetario) // Se calcula el precio agregado al objetivo x la cantidad de personas
                             + (persona.CantidadMenores * objetivo.CantidadAdicional),
                               Pago_restante = persona.Pago_restante + persona.Transacciones.Sum(a => a.CantidadDepositada),
                               TotalPersonas = persona.TotalPersonas

                           }).ToListAsync();
                    }

                    else
                    {
                        aportador = await applicationDbContext.Trabajo
                            .Where(personas => personas.ObjetivosId == Id)
                            .Select(personas => new Trabajo
                            {
                                Id = personas.Id,
                                Nombre = personas.Nombre,
                                Nota = personas.Nota,
                                Aporte = personas.Transacciones.Sum(a => a.CantidadDepositada),
                                TiempoDeposito = personas.TiempoDeposito,
                                Cantidad_personas = personas.Cantidad_personas,
                                CantidadMenores = personas.CantidadMenores,
                                Total_a_pagar = (personas.Cantidad_personas * objetivo.ObjetivoMonetario) // Se calcula el precio agregado al objetivo x la cantidad de personas
                             + (personas.CantidadMenores * objetivo.CantidadAdicional),
                                Pago_restante = ((personas.Cantidad_personas * objetivo.ObjetivoMonetario) // Se calcula el precio agregado al objetivo x la cantidad de personas
                             + (personas.CantidadMenores * objetivo.CantidadAdicional)) - personas.Transacciones.Sum(a => a.CantidadDepositada), //(personas.Cantidad_personas * objetivo.ObjetivoMonetario) // Se calcula el precio agregado al objetivo x la cantidad de personas
                                TotalPersonas = personas.TotalPersonas

                            }).ToListAsync();
                    }
                    
            if (objetivo.HaSidoEditado is true)
            {
                aportador.ForEach(a =>
                {
                    var aportadorIndividual = applicationDbContext.Trabajo.Find(a.Id);

                    if (aportadorIndividual is not null)
                    {
                        aportadorIndividual.Total_a_pagar
                             = (aportadorIndividual.Cantidad_personas * objetivo.ObjetivoMonetario)
                             + (aportadorIndividual.CantidadMenores * objetivo.CantidadAdicional);

                        aportadorIndividual.Pago_restante = 
                        ((aportadorIndividual.Cantidad_personas * objetivo.ObjetivoMonetario)
                       + (aportadorIndividual.CantidadMenores * objetivo.CantidadAdicional)) 
                       - aportadorIndividual.Aporte;
                    }
                     
                });

                objetivo.HaSidoEditado = false;
            }
            if (aportador is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var modelo = new ListadoPersonas();
            ViewBag.Id = Id;
            ViewBag.message = objetivo;
            modelo.listadoPersonas = aportador;

            await applicationDbContext.SaveChangesAsync();
            return View(modelo);
        }  


        [HttpGet]
        public IActionResult Crear(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }

            var objetivo = applicationDbContext.Objetivos.FirstOrDefault(obj => obj.Id == id && obj.UsuarioCreacionId == usuarioId);

            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            ViewBag.message = objetivo;
            var trabajo = new Trabajo();
             
            trabajo.ObjetivoId = id;
            return View(trabajo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Trabajo persona)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var objetivo = await applicationDbContext.Objetivos
                .FirstOrDefaultAsync(obj => obj.Id == persona.ObjetivoId && obj.UsuarioCreacionId == usuarioId);

            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            if (persona.Cantidad_personas < 1)
            { 
                ModelState.AddModelError(string.Empty, "Debe tener al menos una persona");
                return Crear(objetivo.Id);
            }

            int total_a_pagar = (persona.Cantidad_personas * objetivo.ObjetivoMonetario)// Se calcula el precio agregado al objetivo x la cantidad de personas
                + (persona.CantidadMenores * objetivo.CantidadAdicional);//Lo mismo con los menores, si no hay, simplemente se sumara 0

            var modelo = new Personas
            {
                Nombre = persona.Nombre,
                Nota = persona.Nota,
                Aporte = 0,
                Cantidad_personas = persona.Cantidad_personas,
                CantidadMenores = persona.CantidadMenores,
                TiempoDeposito = objetivo.FechaLimite,
                Total_a_pagar = total_a_pagar,
                ObjetivosId = persona.ObjetivoId,
                TotalPersonas = persona.Cantidad_personas + persona.CantidadMenores,
                Pago_restante = total_a_pagar
            };
            applicationDbContext.Add(modelo);
            await applicationDbContext.SaveChangesAsync();

            return RedirectToAction("ListadoPersonas", new {id = persona.ObjetivoId});
        }  

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if(usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == id 
            && p.Objetivos.UsuarioCreacionId == usuarioId);

            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
            ViewBag.message = persona;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id, string message = null)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == id);

            if(persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            applicationDbContext.Remove(persona);
            await applicationDbContext.SaveChangesAsync();
            return RedirectToAction("ListadoPersonas", new { id = persona.ObjetivosId});
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            var persona = applicationDbContext.Trabajo.FirstOrDefault(p => p.Id == id 
            && p.Objetivos.UsuarioCreacionId == usuarioId);
            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var objetivo = applicationDbContext.Objetivos.FirstOrDefault(obj => obj.Id == persona.ObjetivosId);
            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }


            var modelo = new Trabajo();

            modelo.Nombre = persona.Nombre;
            modelo.Nota = persona.Nota;
            modelo.Cantidad_personas = persona.Cantidad_personas; 
            modelo.CantidadMenores = persona.CantidadMenores;
            modelo.TotalPersonas = persona.CantidadMenores + persona.Cantidad_personas;
            modelo.ObjetivoId = persona.ObjetivosId;
            ViewBag.message = objetivo;
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, Trabajo persona)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            } 
            var persona_a_editar = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == id 
            && p.Objetivos.UsuarioCreacionId == usuarioId);
             
            if (persona_a_editar is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
            var objetivo = await applicationDbContext.Objetivos.FirstOrDefaultAsync(obj => obj.Id == persona_a_editar.ObjetivosId);

            if (objetivo is null || !objetivo.Activo)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            if (persona.Cantidad_personas < 1)
            {
                ModelState.AddModelError(string.Empty, "Debe tener al menos un adulto.");
                return Editar(id);
            }

            var total_a_pagar = (persona.Cantidad_personas * objetivo.ObjetivoMonetario) // Se calcula el precio agregado al objetivo x la cantidad de personas
                     + (persona.CantidadMenores * objetivo.CantidadAdicional);

            persona_a_editar.Nombre = persona.Nombre;
            persona_a_editar.Nota = persona.Nota;
            persona_a_editar.Cantidad_personas = persona.Cantidad_personas; 
            persona_a_editar.Total_a_pagar = total_a_pagar; 
            persona_a_editar.Pago_restante = total_a_pagar;
            persona_a_editar.CantidadMenores = persona.CantidadMenores; 
            
            await applicationDbContext.SaveChangesAsync();
            
            return RedirectToAction("ListadoPersonas", new { id = persona_a_editar.ObjetivosId });
        }

        [HttpGet]
        public IActionResult Transaccion(int id) 
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            ViewData["IdPersona"] = id;

            var persona = applicationDbContext.Trabajo.FirstOrDefault(p => p.Id == id
            && p.Objetivos.UsuarioCreacionId == usuarioId);

            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
            
            ViewBag.message = persona;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transaccion(int personaId, TransaccionesDTO transacciones)
        {

            if (transacciones is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            
            if (usuarioId is null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(transacciones);
            }

            if (transacciones.File is null || transacciones.File.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Agregue una imagen");
                return Transaccion(personaId);
            }

            //Validando que sea una imagen o un pdf
            var fileExtension = Path.GetExtension(transacciones.File.FileName).ToLower();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".pdf")
            { 
                ModelState.AddModelError("File", "Solo se permiten imagenes o PDF.");
                return Transaccion(personaId);
            }

            // Validate file content type
            if (!transacciones.File.ContentType.StartsWith("image/") && transacciones.File.ContentType != "application/pdf")
            {
                ModelState.AddModelError("File", "El archivo ingresado no es valido.");
                return Transaccion(personaId);
            }

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == personaId);
            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var codigos_transacciones = await applicationDbContext
                                               .Transacciones.ToListAsync();
            
            if (codigos_transacciones is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            bool existeTransaccion = await applicationDbContext.Transacciones
                .AnyAsync(t => t.Codigo_transaccion == transacciones.Codigo_transaccion);

            if (existeTransaccion)
            {
                ModelState.AddModelError("Codigo_transaccion", "Este código de transacción ya existe");
                return Transaccion(personaId);
            }

            ViewBag.message = persona; 

            if (transacciones.CantidadDepositada > persona.Pago_restante || transacciones.CantidadDepositada <= 0)
            {  
                ModelState.AddModelError("CantidadDepositada", "Necesitas pagar RD$" + persona.Pago_restante + ", agregue una cantidad"); 
                return Transaccion(persona.Id); 
            }
            var uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "bouchers");
            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(transacciones.File.FileName)}";
            var rutaArchivo = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await transacciones.File.CopyToAsync(stream);
            }

            var pago_restante = persona.Pago_restante - transacciones.CantidadDepositada; 

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                { 
                    var transaccion = new Transacciones();
                    transaccion.CantidadDepositada = transacciones.CantidadDepositada;
                    transaccion.Codigo_transaccion = transacciones.Codigo_transaccion;
                    transaccion.FechaTransaccion = transacciones.FechaTransaccion;
                    transaccion.personas = persona;
                    transaccion.PersonaIds = persona.Id;
                    transaccion.TamañoImagen = transacciones.File.Length;
                    transaccion.ContentType = transacciones.File.FileName;
                    transaccion.URL = fileName;
                
                    var NuevoAporte = new Aportes();

                    persona.Aporte += transacciones.CantidadDepositada;
                    persona.Pago_restante = pago_restante;

                    applicationDbContext.Add(transaccion);
                    await applicationDbContext.SaveChangesAsync();

                    NuevoAporte.Cantidad = transaccion.CantidadDepositada;
                    NuevoAporte.PersonaId = persona.Id;
                    NuevoAporte.CodigoFactura = GenerateRandomCode();
                    NuevoAporte.TransaccionId = transaccion.Id;
                    NuevoAporte.FechaRealizacion = transacciones.FechaTransaccion;
                    NuevoAporte.CodigoTransaccion = transacciones.Codigo_transaccion;
                    NuevoAporte.Persona = persona;
                    NuevoAporte.Aprobado = true;
                    applicationDbContext.Add(NuevoAporte);
                    await applicationDbContext.SaveChangesAsync(); 
                     
                    scope.Complete();
                }
                catch (Exception ex)
                { 
                    throw ex;  
                }
            }
            
            return RedirectToAction("ListadoTransacciones", new { id = persona.Id});

        }
        //Que los usuarios puedan crear actividades, y que las los usuarios puedan ingresar el # de cuenta al que se realizaran las transacciones
        [HttpGet]
        public async Task<IActionResult> ListadoTransacciones(int id, string codigoTransaccion = "")
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            List<Transacciones> listadoTransacciones = new List<Transacciones>(); 

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == id 
            && p.Objetivos.UsuarioCreacionId == usuarioId);
 
            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            ViewBag.message = persona;

            if (!string.IsNullOrEmpty(codigoTransaccion))
            {
                //Listado de transacciones
                listadoTransacciones = await applicationDbContext.Transacciones.Where(t => t.personas == persona 
                && t.Codigo_transaccion == codigoTransaccion)
                    .Select(transaccion => new Transacciones
                    {
                        Id = transaccion.Id,
                        Codigo_transaccion = transaccion.Codigo_transaccion,
                        FechaTransaccion = transaccion.FechaTransaccion,
                        URL = transaccion.URL,
                        CantidadDepositada  = transaccion.CantidadDepositada,
                        
                    }).ToListAsync();
            }

            else
            {
                //Listado de transacciones
                listadoTransacciones = await applicationDbContext.Transacciones.Where(u => u.personas == persona)
                    .Select(transaccion => new Transacciones
                {
                    Id = transaccion.Id,
                    Codigo_transaccion = transaccion.Codigo_transaccion,
                    FechaTransaccion = transaccion.FechaTransaccion,
                    URL = transaccion.URL,
                    CantidadDepositada = transaccion.CantidadDepositada

                }).ToListAsync();
            }


            if (listadoTransacciones is null)
            {
                return BadRequest(ModelState);
            }

            var modelo = new ListadoTransacciones();

            modelo.transacciones = listadoTransacciones;
            return View(modelo);
        }

        [HttpPost]
        public IActionResult ListadoTransacciones()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            return View();
        }
         
        [HttpGet]
        public IActionResult EditarTransaccion(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
 
            var transaccion = applicationDbContext.Transacciones.FirstOrDefault(t => t.Id == id);
             
            if (transaccion is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var persona = applicationDbContext.Trabajo.FirstOrDefault(p => p.Id == transaccion.PersonaIds 
                            && p.Objetivos.UsuarioCreacionId == usuarioId);
            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
             

            var modelo = new TransaccionesDTO();

            modelo.Id = transaccion.Id;
            modelo.FechaTransaccion = transaccion.FechaTransaccion;
            modelo.CantidadDepositada = transaccion.CantidadDepositada;
            modelo.Codigo_transaccion = transaccion.Codigo_transaccion;
            modelo.IdPersona = transaccion.PersonaIds;
            ViewBag.persona = persona;
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarTransaccion(int id, TransaccionesDTO transaccionesDTO)
        {
            if(transaccionesDTO is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var transaccion = await applicationDbContext.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
             
            var aportes = await applicationDbContext.Aportes.FirstOrDefaultAsync(aporte => aporte.TransaccionId == id);
              
            if (transaccion is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            if (aportes is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(persona => persona.Id == transaccion.PersonaIds);
            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var objetivo = await applicationDbContext.Objetivos.FirstOrDefaultAsync(obj => obj.Id == persona.ObjetivosId);
            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            //Validando que sea una imagen o un pdf
            var fileExtension = Path.GetExtension(transaccionesDTO.File.FileName).ToLower();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
            {
                ModelState.AddModelError("File", "Solo se permiten imagenes.");
                return EditarTransaccion(transaccion.Id);
            }

            // Validate file content type
            if (!transaccionesDTO.File.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("File", "El archivo ingresado no es valido.");
                return EditarTransaccion(transaccion.Id);
            }

            if (transaccionesDTO.File is not null)
            {
                // Save the new file if provided
                var uploadsDirectory = Path.Combine(webHostEnvironment.WebRootPath, "bouchers");
                var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(transaccionesDTO.File.FileName)}";
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await transaccionesDTO.File.CopyToAsync(stream);
                } 
                 
                transaccion.FechaTransaccion = transaccionesDTO.FechaTransaccion;
                transaccion.Codigo_transaccion = transaccionesDTO.Codigo_transaccion;
                transaccion.TamañoImagen = transaccionesDTO.File.Length;

                if (transaccionesDTO.CantidadDepositada <= 0 || transaccionesDTO.CantidadDepositada > persona.Pago_restante)
                {
                    ModelState.AddModelError(string.Empty, "Debe pagar RD$" + persona.Pago_restante + " Agregue una cantidad.");
                    return EditarTransaccion(transaccion.Id);
                }

                //var total_a_pagar = ((persona.Cantidad_personas * objetivo.ObjetivoMonetario) // Se calcula el precio agregado al objetivo x la cantidad de personas
                //        + (persona.CantidadMenores * objetivo.CantidadAdicional)) - transaccion.CantidadDepositada;

                //var pago = persona.Total_a_pagar + transaccion.CantidadDepositada;


                //var pago_restante = pago - transaccionesDTO.CantidadDepositada;

                persona.Aporte -= transaccion.CantidadDepositada;

                persona.Aporte += transaccionesDTO.CantidadDepositada;

                var pago_restante = persona.Total_a_pagar - persona.Aporte;

                transaccion.CantidadDepositada = transaccionesDTO.CantidadDepositada;
                transaccion.ContentType = transaccionesDTO.File.FileName;
                transaccion.URL = fileName;
                await applicationDbContext.SaveChangesAsync();
                aportes.Cantidad = transaccionesDTO.CantidadDepositada;
                aportes.CodigoTransaccion = transaccionesDTO.Codigo_transaccion;
                aportes.FechaRealizacion = transaccionesDTO.FechaTransaccion;
                persona.Pago_restante = pago_restante;
                await applicationDbContext.SaveChangesAsync();     
            }

            return RedirectToAction("ListadoTransacciones", new { id = transaccion.PersonaIds });   
        }

        [HttpGet]
        public async Task<IActionResult> EliminarTransaccion(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            } 
            var transaccion = await applicationDbContext.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
            if (transaccion is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == transaccion.PersonaIds
            && p.Objetivos.UsuarioCreacionId == usuarioId);

            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            ViewBag.message = transaccion;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EliminarTransaccion(int id, string message = "Eliminado")
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid) { 
            
                return BadRequest();
            } 
            var transaccion = await applicationDbContext.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
            var aporte = await applicationDbContext.Aportes.FirstOrDefaultAsync(a => a.TransaccionId == id);
        
            if (transaccion is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            if (aporte is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(persona => persona.Id == transaccion.PersonaIds);
            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var objetivo = await applicationDbContext.Objetivos.FindAsync(persona.ObjetivosId);
            if (objetivo is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            persona.Pago_restante += transaccion.CantidadDepositada;
            persona.Aporte -= transaccion.CantidadDepositada;
            aporte.Aprobado = false;
             
            applicationDbContext.Remove(transaccion);  
             
            await applicationDbContext.SaveChangesAsync();

            return RedirectToAction("ListadoTransacciones", new { id = transaccion.PersonaIds });
        }

        [HttpGet]
        public async Task<IActionResult> ListadoAportes(int id, string facturaTransaccion = "")
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == id 
            && p.Objetivos.UsuarioCreacionId == usuarioId);

            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            List<Aportes> aportes = new List<Aportes>();

            if (!string.IsNullOrEmpty(facturaTransaccion))
            {
                aportes = await applicationDbContext.Aportes
                    .Where(aporte => aporte.PersonaId == id && aporte.CodigoFactura == facturaTransaccion)
                    .Select(aporte => new Aportes
                    {
                        Id = aporte.Id,
                        Cantidad = aporte.Cantidad,
                        PersonaId = aporte.PersonaId,
                        CodigoFactura = aporte.CodigoFactura,
                        TransaccionId = aporte.TransaccionId,
                        FechaRealizacion = aporte.FechaRealizacion,
                        CodigoTransaccion = aporte.CodigoTransaccion,
                        Aprobado = aporte.Aprobado
                    }).ToListAsync();
            }

            else
            {
                aportes = await applicationDbContext.Aportes.Where(persona => persona.PersonaId == id)
                    .Select(aporte => new Aportes
                    {
                        Id = aporte.Id,
                        Cantidad = aporte.Cantidad,
                        PersonaId = aporte.PersonaId,
                        CodigoFactura = aporte.CodigoFactura,
                        TransaccionId = aporte.TransaccionId,
                        FechaRealizacion = aporte.FechaRealizacion,
                        CodigoTransaccion = aporte.CodigoTransaccion,
                        Aprobado = aporte.Aprobado
                    }).ToListAsync();
            }


            if (aportes is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            ViewBag.persona = persona;
            var modelo = new ListadoAportes();

            modelo.aportes = aportes;
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> RestarAporte(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var aporte = await applicationDbContext.Aportes.FirstOrDefaultAsync(a => a.TransaccionId == id);
             
            if (aporte is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == aporte.PersonaId
            && p.Objetivos.UsuarioCreacionId == usuarioId);

            if (persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
            
            ViewBag.message = aporte;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestarAporte(int id, string mensaje = "Eliminado")
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var transaccion = await applicationDbContext.Transacciones.FirstOrDefaultAsync(t => t.Id == id);
            var aporte = await applicationDbContext.Aportes.FirstOrDefaultAsync(t => t.TransaccionId == id);

            if (transaccion is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }
            var persona = await applicationDbContext.Trabajo.FirstOrDefaultAsync(p => p.Id == transaccion.PersonaIds);

            if (aporte is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            if(persona is null)
            {
                return RedirectToAction("NotFounded", "Home");
            }

            persona.Pago_restante += transaccion.CantidadDepositada;

            persona.Aporte -= transaccion.CantidadDepositada;
            
            applicationDbContext.Remove(transaccion);
            applicationDbContext.Remove(aporte);
            await applicationDbContext.SaveChangesAsync();
            return RedirectToAction("ListadoAportes", new { id = aporte.PersonaId });
        }

        [HttpGet]
        public IActionResult BuscarPersona(string buscarPersona)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            if (usuarioId is null)
            {
                return Forbid();
            }
            var filtarData = applicationDbContext.Trabajo
                                  .Where(e => e.Nombre.Contains(buscarPersona))
                                  .ToList();

            return View(filtarData);
        }
         
        public IActionResult NotFounded()
        {
            return View();
        } 

        private static string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; 
            Random random = new Random();
            char[] codeChars = new char[10];

            for (int i = 0; i < 10; i++)
            {
                codeChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(codeChars);
        }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
