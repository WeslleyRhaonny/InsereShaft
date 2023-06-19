using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary1
{
    public partial class MainWindow : Window
    {
        private List<CustomRevitLink> revitLinks;
        public Document doc;

        public MainWindow(Document doc, List<CustomRevitLink> revitLinks)
        {
            InitializeComponent();

            this.doc = doc;
            this.revitLinks = revitLinks;

            // Define a fonte de dados da ComboBox
            cmbArquivoEstrutural.ItemsSource = revitLinks.Select(link => link.Name);
            cmbArquivoTubulacao.ItemsSource = revitLinks.Select(link => link.Name);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string nomeEstrutural = cmbArquivoEstrutural.SelectedItem as string;
            string nomeTubulacao = cmbArquivoTubulacao.SelectedItem as string;

            //Aqui tem um problema de que o ElementId de cada documento muda de acordo com o pc.
            //Para o código ser funcional é necessário atulizar os ids dos documentos, eles podem ser acessados utilizando o "Revit Lookup"

            ElementId idDocLaje = new ElementId(4428668);
            ElementId idDocTubo = new ElementId(4428671);

            if (nomeEstrutural != null && nomeTubulacao != null)
            {
                View vistaTeste = FuncAux.GetVistaTeste(doc); // Substitua pelo código para obter a vista desejada

                if (vistaTeste != null && nomeEstrutural == "Arquivo do Projeto Estrutural.rvt" && nomeTubulacao == "Arquivo do Projeto de Tubos.rvt")
                {
                    // Obtém os elementos estruturais e de tubos da vista "vista teste"
                    List<Element> elementosEstruturais = new List<Element>();
                    List<Element> elementosTubos = new List<Element>();
                    elementosEstruturais.AddRange(FuncAux.ColetarElementosLajes(doc, idDocLaje));
                    elementosTubos.AddRange(FuncAux.ColetarElementosTubos(doc, idDocTubo));

                    // Imprime a lista de elementos estruturais
                    StringBuilder sbEstrutural = new StringBuilder();
                    sbEstrutural.AppendLine("Elementos Estruturais na vista 'vista teste':");
                    foreach (Element elemento in elementosEstruturais)
                    {
                        sbEstrutural.AppendLine($"- Nome: {elemento.Name}, Categoria: {elemento.Category.Name}");
                    }
                    MessageBox.Show(sbEstrutural.ToString());

                    // Imprime a lista de elementos de tubos
                    StringBuilder sbTubos = new StringBuilder();
                    sbTubos.AppendLine("Elementos de Tubos na vista 'vista teste':");
                    foreach (Element elemento in elementosTubos)
                    {
                        sbTubos.AppendLine($"- Nome: {elemento.Name}, Categoria: {elemento.Category.Name}");
                    }
                    MessageBox.Show(sbTubos.ToString());
                    }
                    else
                    {
                        MessageBox.Show("A vista 'vista teste' não foi encontrada ou os arquivos selecionados sao invalidos");
                    }
            }
            else
            {
                MessageBox.Show("Selecione um elemento em cada ComboBox!");
            }
        }

    }
}

