using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingSelection
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedRef = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите элементы");
            //var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберите элемент");
            double info = 0;
            foreach (var element in selectedRef)
            {
                var selectedElement = doc.GetElement(element);

                if (selectedElement is Wall)
                {
                    Parameter volParameter = selectedElement.LookupParameter("Объем");
                    if (volParameter.StorageType == StorageType.Double)
                    {
                        double summa = volParameter.AsDouble();
                        info += summa;
                    }
                }
            }
            TaskDialog.Show("Объем", info.ToString());
            return Result.Succeeded;
        }
    }
}
