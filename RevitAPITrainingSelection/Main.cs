using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
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
            double info = 0;
            foreach (var element in selectedRef)
            {
                var selectedElement = doc.GetElement(element);

                if (selectedElement is Pipe)
                {
                    Parameter lParameter = selectedElement.LookupParameter("Длина");
                    if (lParameter.StorageType == StorageType.Double)
                    {
                        double summa = lParameter.AsDouble();
                        info += summa;
                    }
                }
            }
            TaskDialog.Show("Длина", info.ToString());
            return Result.Succeeded;
        }
    }
}
