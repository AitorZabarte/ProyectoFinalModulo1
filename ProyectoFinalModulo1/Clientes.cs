﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace ProyectoFinalModulo1
{

    class Clientes
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["VIDEOCLUB"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;
        static SqlDataReader registros;
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string FechaDeNacimiento { get; set; }
        public string Contraseña { get; set; }

        public Clientes()
        {
            Email = Nombre = Apellido = FechaDeNacimiento = Contraseña = null;
        }
        public Clientes(string email,string nombre,string apellido,string fechaDeNacimiento,string contraseña)
        {
            this.Email = email;
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.FechaDeNacimiento = fechaDeNacimiento;
            this.Contraseña = contraseña;
        }
        public bool LogIn()
        {
            Console.Clear();
            bool usuarioRegistrado = false;
            bool emailCorrecto = false;
            Console.WriteLine("Introduce tu E-mail");
            this.Email = Console.ReadLine();
            conexion.Open();
            cadena = $"SELECT Email from Clientes where Email='{this.Email}'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                emailCorrecto = true;
            }
            conexion.Close();
            if (emailCorrecto)
            {
                Console.WriteLine("E-mail correcto ahora introduce tu contraseña");
                this.Contraseña = Console.ReadLine();
                conexion.Open();
                cadena = $"SELECT * from Clientes where Contraseña='{this.Contraseña}'";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                if (registros.Read())
                {
                    this.Nombre = registros["Nombre"].ToString();
                    this.Apellido = registros["Apellido"].ToString();
                    this.FechaDeNacimiento = registros["FechaDeNacimiento"].ToString();

                    usuarioRegistrado = true;
                    Console.WriteLine("Enhorabuena te has logueado con exito");
                }
                conexion.Close();
            }
            return usuarioRegistrado;
        }
        public bool Registrar()
        {
            bool registOK=false;
            try
            {
                Console.Clear();
                bool emailRegistrado = false;
                Console.WriteLine("Introduce tu E-mail");
                this.Email = Console.ReadLine();

                if (this.Email.Contains("@") &&( (this.Email.Contains(".com") || this.Email.Contains(".es"))))
                {
                    conexion.Open();
                    cadena = $"SELECT Email from Clientes where Email='{this.Email}'";
                    comando = new SqlCommand(cadena, conexion);
                    registros = comando.ExecuteReader();
                    if (registros.Read())
                    {
                        emailRegistrado = true;
                        Console.WriteLine("Este usuario ya está registrado");
                    }
                    conexion.Close();
                    if (emailRegistrado != true)
                    {
                        Console.WriteLine("E-mail correcto ahora introduce tu contraseña, como maximo 20 caracteres");
                        this.Contraseña = Console.ReadLine();
                        if (this.Contraseña.Length < 21&&this.Contraseña.Length>0)
                        {
                            Console.WriteLine("Introduce tu fecha de nacimiento de esta manera AAAA-MM-DD");
                            DateTime.TryParse(Console.ReadLine(), out DateTime fecha);
                            if (fecha < DateTime.Now)
                            {
                                this.FechaDeNacimiento = fecha.ToString();
                            }
                            else
                            {
                                this.FechaDeNacimiento = DateTime.Now.ToString();
                            }
                            Console.WriteLine("Si quieres insertar tu nombre y apellidos escribe 'si'");
                            string aniadir = Console.ReadLine();
                            if (aniadir.ToLower() == "si")
                            {
                                Console.WriteLine("Introduce tu nombre");
                                this.Nombre = Console.ReadLine();
                                Console.WriteLine("Introduce tu apellido");
                                this.Apellido = Console.ReadLine();
                            }
                            conexion.Open();
                            cadena = $"INSERT INTO Clientes (Email,Nombre,Apellido,FechaDeNacimiento,Contraseña) VALUES ('{this.Email}','{this.Nombre}','{this.Apellido}','{this.FechaDeNacimiento}','{this.Contraseña}')";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();
                        }
                        else
                        {
                            Console.WriteLine("Has introducido una contraseña demasiado larga");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("El e-mail no tiene el formato aceptado");
                }
            }
            catch(OverflowException)
            {
                Console.WriteLine("Has introducido demasiados caracteres en algun campo");
                conexion.Close();
            }
            catch(FormatException)
            {
                Console.WriteLine("Has introducido un formato equivocado");
                conexion.Close();
            }
            catch(Exception)
            {
                Console.WriteLine("Error");
                conexion.Close();
            }
            return registOK;
        }
        public int ObtenerEdad()
        {
            try
            {
                if (DateTime.Today.Day > Convert.ToDateTime(this.FechaDeNacimiento).Day && DateTime.Today.Month > Convert.ToDateTime(this.FechaDeNacimiento).Month)
                {
                    return DateTime.Today.Year - Convert.ToDateTime(this.FechaDeNacimiento).Year - 1;
                }
                else
                {
                    return (DateTime.Today.Year - Convert.ToDateTime(this.FechaDeNacimiento).Year);
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Error");
                return 0;
            }
        }
        public void CambiarDatos()
        {
            try
            {
                string opcion = "";
                Console.WriteLine("1.Cambiar la contraseña\n2.Cambiar fecha de nacimiento\n3.Cambiar Nombre\n4.Cambiar Apellido");
                opcion=Console.ReadLine();
                {
                    if (opcion == "1")
                    {
                        Console.WriteLine("Introduce tu contraseña, como maximo 20 caracteres");
                        this.Contraseña = Console.ReadLine();
                        if (this.Contraseña.Length < 21 && this.Contraseña.Length > 0)
                        {
                            conexion.Open();
                            cadena = $"UPDATE Clientes SET Contraseña='{this.Contraseña}' where Email='{this.Email}'";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();
                        }
                    }
                    else if (opcion == "2")
                    {
                            Console.WriteLine("Introduce tu fecha de nacimiento de esta manera AAAA-MM-DD");
                            this.FechaDeNacimiento =Console.ReadLine();
                            conexion.Open();
                            cadena = $"UPDATE Clientes SET FechaDeNacimiento='{this.FechaDeNacimiento}'where Email='{this.Email}'";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();

                    }
                    else if (opcion == "3")
                    {
                            Console.WriteLine("Introduce tu nombre");
                            this.Nombre = Console.ReadLine();
                            conexion.Open();
                            cadena = $"UPDATE Clientes SET Nombre='{this.Nombre}'where Email='{this.Email}'";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();

                    }
                    else if (opcion == "4")
                    {
                            Console.WriteLine("Introduce tu apellido");
                            this.Apellido = Console.ReadLine();
                            conexion.Open();
                            cadena = $"UPDATE Clientes SET Apellido='{this.Apellido}'where Email='{this.Email}'";
                            comando = new SqlCommand(cadena, conexion);
                            comando.ExecuteNonQuery();
                            conexion.Close();
                    }

                    else
                    {
                        Console.WriteLine("Has introducido una opcion incorrecta");
                    }
                }
            }
            catch (OverflowException)
            {
                Console.WriteLine("Has introducido demasiados caracteres en algun campo");
                conexion.Close();
            }
            catch (FormatException)
            {
                Console.WriteLine("Has introducido un formato equivocado");
                conexion.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                conexion.Close();

            }
        }
    }
}