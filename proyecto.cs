using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace proyecto
{
    public class Program
    {
        static void Main(string[] args)
        {
            AdministradorTareas administrador = new AdministradorTareas();
            bool continuar = true; //mientras continuar sea verdad:

            administrador.TareaAgregada += (mensaje) => Console.WriteLine(mensaje);
            administrador.TareaCompletada += (mensaje) => Console.WriteLine(mensaje);
            administrador.TareaEliminada += (mensaje) => Console.WriteLine(mensaje);
            administrador.TareaActualizada += (mensaje) => Console.WriteLine(mensaje);

            while (continuar)
            {
                //menú para interactuar con el usuario
                Console.WriteLine("- - Administrador de Tareas - -");
                Console.WriteLine("1. Agregar Tarea");
                Console.WriteLine("2. Marcar Tarea como Completada");
                Console.WriteLine("3. Mostrar Todas las Tareas");
                Console.WriteLine("4. Mostrar Tareas por Prioridad");
                Console.WriteLine("5. Actualizar");
                Console.WriteLine("6. Eliminar");
                Console.WriteLine("7. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();
                string nombreTarea;
                switch (opcion)
                {
                    case "1": //crear una tarea nueva con todas sus caracteristicas
                        Console.Write("Nombre de la tarea: ");
                        nombreTarea = Console.ReadLine();
                        Console.Write("Prioridad (Baja/Media/Alta): ");
                        string prioridad = Console.ReadLine();
                        Console.Write("fecha Vencimiento DD/MM/YYYY: ");
                        DateTime fechaVencimiento = new DateTime();
                        try
                        {
                            fechaVencimiento = DateTime.Parse(Console.ReadLine());
                            string estado = "No completado"; // estado predeterminado (no completado)
                            Console.Write("Frecuencia: ");
                            string frecuencia = Console.ReadLine();
                            int frecuenciaNumero = 0;
                            if (!int.TryParse(frecuencia, out frecuenciaNumero))
                            {
                                Console.WriteLine("Por favor ingrese un numero valido");
                                return;
                            }
                            Tarea tarea;
                            if (frecuenciaNumero>=2) { 
                                 tarea = new TareaRecurrente(nombreTarea, fechaVencimiento, prioridad, estado, frecuenciaNumero); //guardar información de la tarea
                            } else
                            {
                                tarea = new Tarea(nombreTarea, fechaVencimiento, prioridad, estado);
                            }
                            administrador.AnadirTarea(tarea); //añadir la tarea
                        }
                        catch (Exception)
                        {
                            Console.Write("fecha Vencimiento no valida, debe ser DD/MM/YYYY"); //corrección de errores (ingresar una fecha que no cumple con el formato solicitado)
                        }
                        break;
                    case "2": //marcar una tarea como completada
                        Console.Write("Nombre de la tarea a marcar como completada: ");
                        string nombreTareaCompletada = Console.ReadLine();
                        administrador.MarcarComoCompletada(nombreTareaCompletada);
                        break;
                    case "3": //mostrar tareas creadas
                        administrador.MostrarTareas();
                        break;
                    case "4": //mostrar tareas segun la prioridad pedida por el usuario
                        Console.Write("Prioridad a mostrar (Baja/Media/Alta): ");
                        string prioridadMostrar = Console.ReadLine();
                        administrador.MostrarTareasPorPrioridad(prioridadMostrar);
                        break;
                    case "5": //Actualizar tarea
                        Console.Write("escriba el nombre de la tarea que desea actualizar ");
                        nombreTarea = Console.ReadLine();
                        administrador.ActualizarTarea(nombreTarea); //Actualizr la tarea
                        break;
                    case "6": //Eliminar tarea
                        Console.Write("escriba el nombre de la tarea que desea eliminar ");
                        nombreTarea = Console.ReadLine();
                        administrador.EliminarTarea(nombreTarea);
                        break;
                    case "7": //salir
                        continuar = false;
                        break;
                    default: //correccion de errores (ingresar una respuesta no marcada)
                        Console.WriteLine("Opción no válida.");
                        break;
                }

                Console.WriteLine();
            }
        }
    }

    public class AdministradorTareas // clase administrador de tareas
    {
        private List<Tarea> listaTareas = new List<Tarea>();
       
        // Delegados para los eventos
        public delegate void TareaEventHandler(string mensaje);
        public event TareaEventHandler TareaAgregada;
        public event TareaEventHandler TareaCompletada;
        public event TareaEventHandler TareaEliminada;
        public event TareaEventHandler TareaActualizada;

        public void AnadirTarea(Tarea x) // metodo para añadir tarea
        {
            listaTareas.Add(x);
            TareaAgregada?.Invoke($"Se agregó una nueva tarea: {x.NombreTarea}");
        }

        public void EliminarTarea(string nombre) // eliminar tareas 
        {
            Tarea tareaAEliminar = listaTareas.FirstOrDefault(t => t.NombreTarea == nombre); // eliminar la primera tarea que tenga el nombre ingresado por el usuario
            if (tareaAEliminar != null)
            {
                listaTareas.Remove(tareaAEliminar);
                TareaEliminada?.Invoke($"Se eliminó la tarea: {nombre}");
            }
        }

        public void MarcarComoCompletada(string nombre) // marcar tarea como completada
        {
            Tarea tarea = listaTareas.FirstOrDefault(t => t.NombreTarea == nombre); // marcar como completada la primera tarea con el nombre ingresado por el usuario
            if (tarea != null)
            {
                tarea.Estado = "Completada"; // pasar estado a completado
                TareaCompletada?.Invoke($"Se completó la tarea: {nombre}");
            }
        }

        public void MostrarTareas() // mostrar tareas
        {
            var newList = listaTareas.OrderBy(x => x.FechaVencimiento).ToList();
            foreach (var tarea in newList)
            {
                tarea.ImprimirTarea(); //imprimir tareas
            }
        }

        public void MostrarTareasPorPrioridad(string prioridad)
        {
            var tareasPorPrioridad = listaTareas.Where(t => t.Prioridad == prioridad);
            Console.WriteLine($"Tareas con prioridad {prioridad}:");
            foreach (var tarea in tareasPorPrioridad)
            {
                tarea.ImprimirTarea();
            }
        }

        public void ActualizarTarea(string nombre)
        {
            Tarea tarea = listaTareas.FirstOrDefault(t => t.NombreTarea == nombre);
            if (tarea != null)
            {
                listaTareas.Remove(tarea);
                Console.Write("Nombre de la tarea: ");
                nombre = Console.ReadLine();
                tarea.NombreTarea = nombre;
                Console.Write("Prioridad (Baja/Media/Alta): ");
                string prioridad = Console.ReadLine();
                tarea.Prioridad = prioridad;
                Console.Write("fecha Vencimiento DD/MM/YYYY: ");
                DateTime fechaVencimiento = new DateTime();
                try
                {
                    fechaVencimiento = DateTime.Parse(Console.ReadLine());
                    tarea.FechaVencimiento = fechaVencimiento;
                    listaTareas.Add(tarea);
                    TareaActualizada?.Invoke($"Se actualizó la tarea: {nombre}");
                }
                catch (Exception)
                {
                    Console.Write("fecha Vencimiento no valida, debe ser DD/MM/YYYY"); //corrección de errores (ingresar una fecha que no cumple con el formato solicitado)
                }
            }
        }

    }

    public abstract class Tarea //clase tarea
    {
        //propiedades para las diferentes partes de la tarea
        public string NombreTarea;
        public DateTime FechaVencimiento;
        public string Prioridad;
        public string Estado;

        //constructor de la clase tarea
        public Tarea(string nombreTarea, DateTime fechaVencimiento, string prioridad, string estado)
        {
            // asignar los atributos correspondientes
            NombreTarea = nombreTarea;
            FechaVencimiento = fechaVencimiento;
            Prioridad = prioridad;
            Estado = estado;
        }
        // Método para imprimir los detalles de la tarea
        public abstract void ImprimirTarea();
    }

    public class TareaRecurrente : Tarea
    {
        public int Frecuencia;

        public TareaRecurrente(string nombreTarea, DateTime fechaVencimiento, string prioridad, string estado, int frecuencia) : base(nombreTarea, fechaVencimiento, prioridad, estado)
        {
            Frecuencia = frecuencia;
        }

        public override void ImprimirTarea()
        {
            Console.WriteLine($"Nombre: {NombreTarea}, Fecha de Vencimiento: {FechaVencimiento}, Prioridad: {Prioridad}, Estado: {Estado}, Frecuencia: {Frecuencia}");
        }
    }
}