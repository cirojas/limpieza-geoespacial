using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LimpiezaGeoespacial
{
    public enum Granularidad { REGION, COMUNA, PROVINCIA };

    public partial class MainWindow : Window
    {
        // public string RegionFilePath;
        // public string ProvinciaFilePath;
        // public string ComunaFilePath;

        private Dictionary<int, string> Regiones;
        private Dictionary<int, string> Provincias;
        private Dictionary<int, string> Comunas;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AbrirCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Por ahora supongo que los archivos se llamaran siempre de la misma manera y estarán en
                // la misma carpera que el CSV que se abre ahora
                string directoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                // TODO  
                /*RegionFilePath = directoryPath + "/regiones.csv";
                ProvinciaFilePath = directoryPath + "/provincias.csv";
                ComunaFilePath = directoryPath + "/comunas.csv";
                */

                Regiones = new Dictionary<int, string>();
                Provincias = new Dictionary<int, string>();
                Comunas = new Dictionary<int, string>();

                bool isReadingHeader = true;
                foreach (string line in File.ReadLines(@"C:\Users\Carlos\Downloads\imfd\Comunas.csv", Encoding.UTF8))
                {
                    if (isReadingHeader)
                    {
                        isReadingHeader = false;
                        continue;
                    }
                    string[] row = line.Split('\t');

                    int codeRegion = Int32.Parse(row[0]);
                    int codeProvincia = Int32.Parse(row[2]);
                    int codeComuna = Int32.Parse(row[4]);

                    if (!Regiones.ContainsKey(codeRegion))
                    {
                        Regiones.Add(codeRegion, row[1]);
                    }
                    if (!Provincias.ContainsKey(codeProvincia))
                    {
                        Provincias.Add(codeProvincia, row[3]);
                    }
                    if (!Comunas.ContainsKey(codeComuna))
                    {
                        Comunas.Add(codeComuna, row[5]);
                    }
                }

                AbrirCSV(',', openFileDialog.FileName, true);
            }
        }

        private void AbrirCSV(char separator, string filename, bool hasHeader)
        {
            // TODO: preguntar que columna tiene la informacion
            int columnaCodigo = 0;

            // TODO: 
            Granularidad granularidad = Granularidad.COMUNA;

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Public\Downloads\WriteLines.csv"))
            {
                bool isReadingHeader = hasHeader;
                foreach (string line in File.ReadLines(filename, Encoding.UTF8))
                {
                    if (isReadingHeader)
                    {
                        isReadingHeader = false;
                        continue;
                    }

                    string[] row = line.Split(separator);

                    if (granularidad == Granularidad.COMUNA)
                    {
                        long code = long.Parse(row[columnaCodigo]);
                        // ej: 110_110_020_201 => 1|1|01|10020201
                        string region = GetRegion((int)(code / 100_000_000_000));
                        string provincia = GetProvincia((int)(code / 10_000_000_000));
                        string comuna = GetComuna((int)(code / 100_000_000));

                        file.Write(string.Format("{0},{1},{2},", region, provincia, comuna));
                        file.Write(string.Join(",", row));
                        file.WriteLine();
                    }

                }
            }
        }

        private string GetRegion(int idRegion)
        {
            return Regiones[idRegion];
        }

        private string GetProvincia(int idProvincia)
        {
            return Provincias[idProvincia];
        }

        private string GetComuna(int idComuna)
        {
            return Comunas[idComuna];
        }
    }
}
