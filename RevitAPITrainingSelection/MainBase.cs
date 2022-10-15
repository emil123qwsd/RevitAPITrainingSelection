using Autodesk.Revit.Attributes;

namespace RevitAPITrainingSelection
{
    [Transaction(TransactionMode.Manual)]
    public class MainBase
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберете элемент");
            var selectedElement = doc.GetElement(selectedRef);

            if (selectedElement is Pipe)
            {
                Parameter diameterPar1 = selectedElement.LookupParameter("Внутренний диаметр");
                if (diameterPar1.StorageType == StorageType.Double)
                {
                    double d1 = diameterPar1.AsDouble();
                    TaskDialog.Show("Внутренний диаметр", diameterPar1.AsDouble().ToString());
                }
                Parameter diameterPar2 = selectedElement.LookupParameter("Внешний диаметр");
                if (diameterPar2.StorageType == StorageType.Double)
                {
                    double d2 = diameterPar2.AsDouble();
                    TaskDialog.Show("Внешний диаметр", diameterPar2.AsDouble().ToString());
                }
            }
        }
    }
}