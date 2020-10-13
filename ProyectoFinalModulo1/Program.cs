using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace ProyectoFinalModulo1
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;
        static SqlDataReader registros;
        static void Main(string[] args)
        {
            string opcion="";
            string opcionMenu = "";
            Peliculas pelicula = new Peliculas();
            do
            {
                Clientes cliente = new Clientes();
                Alquiler alquiler = new Alquiler();
                Console.WriteLine("Hola! Bienvenido a nuestro videclub.\n1.-Log in\n2.-Registrarse\n3.-Salir");
                opcion = Console.ReadLine();
                if (opcion == "1")
                {

                    if (cliente.LogIn())
                    {
                        do
                        {
                            Console.WriteLine("Introduce una opcion\n1.-Ver informacion de las peliculas\n" +
                                "2.-Alquilar pelicula\n" +
                                "3.-Mis alquileres\n" +
                                "4.-Cambiar datos\n" +
                                "5.-Logout");
                            opcionMenu = Console.ReadLine();
                            switch (opcionMenu)
                            {
                                case "1":
                                    pelicula.VerSinopsis(cliente.ObtenerEdad());
                                    break;
                                case "2":
                                    alquiler.AlquilarPeliculas(cliente.ObtenerEdad(), cliente.Email);
                                    break;
                                case "3":
                                    alquiler.PeliculasAlquiladas(cliente.ObtenerEdad(), cliente.Email);
                                    break;
                                case "4":
                                    cliente.CambiarDatos();
                                    break;
                                case "5":
                                    Console.Clear();
                                    break;
                                default:
                                    Console.WriteLine("Opcion incorrecta\n");
                                    break;                                       
                            }
                        

                        } while (opcionMenu != "5");
                    }
                    else
                    {
                        Console.WriteLine("No te has logueado, has introducido un e-mail o contraseña incorrecta");
                    }

                }
                else if (opcion == "2")
                {
                    cliente.Registrar();
                }
                else if (opcion == "3")
                {
                    Console.WriteLine("Gracias");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Opcion incorrecta");
                }
                
                
            } while (opcion.ToLower()!="3");
        }
    }
}
