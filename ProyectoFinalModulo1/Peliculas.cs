using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ProyectoFinalModulo1
{
    class Peliculas
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;
        static SqlDataReader registros;
        public int ID { get; set; }
        public string Titulo { get; set; }
        public string Sinopsis { get; set; }
        public int EdadRecomendada { get; set; }
        public string Estado { get; set; }

        public Peliculas()
        {
            Titulo =Sinopsis= "";
            EdadRecomendada = 0;
            Estado = "";
        }
        public Peliculas(int id,string titulo,string sinopsis, int edadRecomendada,string estado)
        {
            this.ID =id;
            this.Titulo = titulo;
            this.Sinopsis = sinopsis;
            this.EdadRecomendada = edadRecomendada;
            this.Estado = estado;
        }
        public List<Peliculas> PeliculasALaLista()
        {
            List<Peliculas> ListPeliculas = new List<Peliculas>();
            conexion.Open();
            cadena = $"SELECT * from Peliculas";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            while (registros.Read())
            {
                Peliculas pelicula = new Peliculas();
                pelicula.ID = Convert.ToInt32(registros["ID"].ToString());
                pelicula.Titulo = registros["Titulo"].ToString();
                pelicula.EdadRecomendada = Convert.ToInt32(registros["EdadRecomendada"].ToString());
                pelicula.Estado = registros["Estado"].ToString();
                pelicula.Sinopsis = registros["Sinopsis"].ToString();
                ListPeliculas.Add(pelicula);
            } 
                conexion.Close();
            return ListPeliculas;
        }
        public void VerSinopsis(int edad)
        {
            Clientes cliente = new Clientes();
            try
            {
                List<Peliculas> ListPeliculas = PeliculasALaLista();
                List<Peliculas> ListPelicula = new List<Peliculas>();
                int i = 1;
                foreach (Peliculas p in ListPeliculas.Where(n => n.EdadRecomendada <= edad))
                {
                    Console.WriteLine(i + ")-" + p.Titulo);
                    i++;
                    ListPelicula.Add(p);
                }
                Console.WriteLine("Introduce el numero a la izquierda de la pelicula para ver mas informacion");
                int numPel = Convert.ToInt32(Console.ReadLine());
                if (numPel <= ListPelicula.Count && numPel > 0)
                {
                    Console.WriteLine($"Titulo: {ListPelicula.ElementAt(numPel - 1).Titulo}\tEdad Recomendada: {ListPelicula.ElementAt(numPel - 1).EdadRecomendada}\nSinopsis:" +
                        $"\n {ListPelicula.ElementAt(numPel - 1).Sinopsis}\n");
                }
                else
                {
                    Console.WriteLine("Has elegido una opcion incorrecta");
                }
            }
            catch(FormatException)
            {
                Console.WriteLine("Has introducido un caracter erroneo");
            }
            catch(ArgumentOutOfRangeException)
            {
                Console.WriteLine("Error");
            }
        }
    }
}
