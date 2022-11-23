using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingSelection2
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите стены");

            double SumVol = 0;


            foreach (var element in selectedElementRefList)
            {
                var selectedElement = doc.GetElement(element);
                if (selectedElement is Wall)
                {
                    Parameter VolumePar = selectedElement.LookupParameter("Объем");
                    if(VolumePar.StorageType == StorageType.Double)
                    {
                        double VolumeValue = VolumePar.AsDouble();
                        SumVol += VolumeValue;
                    } 
                        
                }

            }
            return Result.Succeeded;
        }
    }
}
