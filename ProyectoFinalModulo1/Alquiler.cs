using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ProyectoFinalModulo1
{
    class Alquiler
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;
        static SqlDataReader registros;
        public int ID { get; set; }
        public string Email { get; set; }
        public int IDPeliculas { get; set; }
        public string FechaAlquilado { get; set; }
        public string FechaLimite { get; set; }
        public string FechaDevolucion { get; set; }

        public Alquiler()
        {
            FechaAlquilado = FechaLimite = FechaDevolucion = Email="";
            IDPeliculas = 0;
        }
        public Alquiler(int idPeliculas, string email,string fechaAlquilado, string fechaLimite, string fechaDevolucion)
        {
            this.IDPeliculas = idPeliculas;
            this.Email=email;
            this.FechaAlquilado= fechaAlquilado;
            this.FechaLimite = fechaLimite;
            this.FechaDevolucion = fechaDevolucion;
        }

        public void AlquilarPeliculas(int edad,string email)
        {
            try
            {
                Peliculas pelicula = new Peliculas();
                List<Peliculas> ListPeliculas = pelicula.PeliculasALaLista();
                List<Peliculas> ListPelAlq = new List<Peliculas>();
                int i = 1;
                foreach (Peliculas p in ListPeliculas.Where(n => n.EdadRecomendada <= edad))
                {
                    if (p.Estado == "disponible")
                    {
                        Console.WriteLine(i +")- " + p.Titulo);
                        i++;
                        ListPelAlq.Add(p);
                    }

                }
                if (ListPelAlq.Count != 0)
                {
                    Console.WriteLine("Introduce el numero a la izquierda de la pelicula para alquilarlo");
                    int numPel = Convert.ToInt32(Console.ReadLine());
                    if (ListPelAlq.Count >= numPel && numPel > 0)
                    {

                        Console.WriteLine("Introduce el numero de dias que quieres alquilarlo");
                        double dias = Convert.ToUInt32(Console.ReadLine());
                        if (dias > 30)
                        {
                            dias = 30;
                        }
                        conexion.Open();
                        cadena = $"INSERT INTO Alquiler (IDPeliculas,Email,FechaAlquilado,FechaLimite) VALUES('{ListPelAlq.ElementAt(numPel - 1).ID}','{email}','{DateTime.Now}','{DateTime.Now.AddDays(dias)}')";
                        comando = new SqlCommand(cadena, conexion);
                        comando.ExecuteNonQuery();
                        conexion.Close();

                        conexion.Open();
                        cadena = $"UPDATE Peliculas SET Estado='Alquilado'where ID='{ListPelAlq.ElementAt(numPel - 1).ID}'";
                        comando = new SqlCommand(cadena, conexion);
                        comando.ExecuteNonQuery();
                        conexion.Close();
                    }
                    else
                    {
                        Console.WriteLine("Mal numero");
                    }
                }
                else
                {
                    Console.WriteLine("No hay peliculas aptas para que tu puedas alquilarlas o estan alquiladas");
                }
            }
            catch (OverflowException)
            {
                Console.WriteLine("Has introducido demasiados caracteres en algun campo");
            }
            catch (FormatException)
            {
                Console.WriteLine("Has introducido un formato equivocado");
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }
        }
        public List<Alquiler> AlquilerALaLista()
        {
            List<Alquiler> ListAlquiler = new List<Alquiler>();
            conexion.Open();
            cadena = $"SELECT * from Alquiler";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            while (registros.Read())
            {
                Alquiler alquiler = new Alquiler();
                alquiler.ID = Convert.ToInt32(registros["ID"].ToString());
                alquiler.IDPeliculas =Convert.ToInt32( registros["IDPeliculas"].ToString());
                alquiler.Email = (registros["Email"].ToString());
                alquiler.FechaAlquilado = registros["FechaAlquilado"].ToString();
                alquiler.FechaLimite = registros["FechaLimite"].ToString();
                alquiler.FechaDevolucion = registros["FechaDevolucion"].ToString();
                ListAlquiler.Add(alquiler);
            }
            conexion.Close();
            
            return ListAlquiler;
        }
        public void PeliculasAlquiladas(int edad, string email)
        {
            try
            {
                Peliculas pelicula = new Peliculas();
                List<Alquiler> ListAlquiler = AlquilerALaLista();
                List<Peliculas> ListPeliculas = pelicula.PeliculasALaLista();
                List<Alquiler> PeliculaAlquiladas = new List<Alquiler>();
                int i = 1;

                foreach (Alquiler a in ListAlquiler.Where(a => a.Email == email))
                {
                    DateTime.TryParse(a.FechaLimite, out DateTime rojo);
                    if (DateTime.Compare(rojo, DateTime.Now) < 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    if (a.FechaDevolucion.Length==0)
                    {
                        foreach (Peliculas p in ListPeliculas.Where(n => n.ID == a.IDPeliculas)) 
                        {
                            Console.Write(i + ")-" + p.Titulo + "\t");
                        }
                        Console.WriteLine(" " + a.FechaLimite);
                        
                        PeliculaAlquiladas.Add(a);
                        i++;
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (PeliculaAlquiladas.Count() > 0)
                {
                    Console.WriteLine("Introduce una de estas opciones:\n1.-Devolver pelicula\n2.-Extender alquiler");
                    string devo = Console.ReadLine();
                    if (devo == "1")
                    {
                        Console.WriteLine("Introduce el numero a la izquierda de la pelicula que quieres devolver");
                        int.TryParse(Console.ReadLine(), out int alq);
                        if (alq > 0&&alq-1<PeliculaAlquiladas.Count())
                        {
                            conexion.Open();
                            cadena = $"UPDATE Alquiler SET FechaDevolucion='{DateTime.Now}'where FechaDevolucion IS NULL and Email='{email}' and IDPeliculas='{PeliculaAlquiladas.ElementAt(alq - 1).IDPeliculas}'";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();
                            conexion.Open();
                            cadena = $"UPDATE Peliculas SET Estado='disponible'where ID='{PeliculaAlquiladas.ElementAt(alq - 1).IDPeliculas}'";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();
                        }
                    }
                    else if (devo == "2")
                    {
                        Console.WriteLine("Introduce el numero a la izquierda de la pelicula que quieres extender el alquiler");
                        int.TryParse(Console.ReadLine(), out int ext);
                        if (ext > 0 && ext - 1 < PeliculaAlquiladas.Count())
                            {
                            Console.WriteLine("Introduce el numero de dias que quieres extender el alquiler");
                            double dias = Convert.ToUInt32(Console.ReadLine());
                            if (dias > 30)
                            {
                                dias = 30;
                            }
                            DateTime.TryParse(PeliculaAlquiladas.ElementAt(ext - 1).FechaLimite, out DateTime date);
                            if (DateTime.Now.AddDays(31) > date.AddDays(dias))
                            {
                                conexion.Open();
                                cadena = $"UPDATE Alquiler SET FechaLimite='{date.AddDays(dias)}'where FechaDevolucion IS NULL and Email='{email}' and IDPeliculas='{PeliculaAlquiladas.ElementAt(ext - 1).IDPeliculas}'";
                                comando = new SqlCommand(cadena, conexion);
                                comando.ExecuteNonQuery();
                                conexion.Close();
                            }
                            else
                            {
                                Console.WriteLine("No puedes extender el alquiler por mas de 30 dias");
                            }
                            }
                        
                    }
                    else
                    {
                        Console.WriteLine("Opcion incorrecta");
                    }
                }
                else Console.WriteLine("No tienes peliculas alquiladas");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Has introducido demasiados caracteres en algun campo");
            }
            catch (FormatException)
            {
                Console.WriteLine("Has introducido un formato equivocado");
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
            }

        }
    }
}
