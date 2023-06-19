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
using System.Windows.Interop;
using System.IO;
using CustomRevitLink = ClassLibrary1.CustomRevitLink;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    public class MyCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Obtém a lista de vínculos Revit como objetos CustomRevitLink
            List<CustomRevitLink> revitLinks = FuncAux.GetRevitLinks(doc);

            // Cria uma instância da janela WPF
            MainWindow wpfWindow = new MainWindow(doc, revitLinks);

            // Obtém o identificador da janela do Revit
            IntPtr revitWindowHandle = uiapp.MainWindowHandle;

            // Define o identificador da janela do Revit como o proprietário da janela WPF
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(wpfWindow);
            windowInteropHelper.Owner = revitWindowHandle;

            // Exibe a janela WPF como um diálogo modal
            wpfWindow.ShowDialog();

            return Result.Succeeded;
        }
    }
}