using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace Laboratorio_2_CSV_JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = "estudiantes.csv";
            string[] lineas = File.ReadAllLines(csvPath);
            List<Estudiante> estudiantes = new List<Estudiante>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string linea = lineas[i];
                string[] partes = linea.Split(',');
                if (partes.Length >= 3)
                {
                    Estudiante est = new Estudiante
                    {
                        Id = int.Parse(partes[0].Trim()),
                        Nombre = partes[1].Trim(),
                        Carrera = partes[2].Trim()
                    };
                    estudiantes.Add(est);
                }
            }
            Console.WriteLine("Lista de estudiantes:");
            foreach (var est in estudiantes)
            {
                Console.WriteLine($"ID: {est.Id}, Nombre: {est.Nombre}, Carrera: {est.Carrera}");
            }
            string json = JsonSerializer.Serialize(estudiantes, new JsonSerializerOptions { WriteIndented = true });
            string jsonPath = "estudiantes.json";
            File.WriteAllText(jsonPath, json);
            Console.WriteLine($"\nArchivo JSON guardado como: {jsonPath}");
        }
    }
}