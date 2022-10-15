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
using static System.Net.Mime.MediaTypeNames;

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

            Reference selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберите трубу");


            var categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

            using (Transaction ts = new Transaction(doc, "Add parameter"))
            {
                ts.Start();
                CreateSheredParameter(uiapp.Application, doc, "Диаметр внешний/внутренний", categorySet, BuiltInParameterGroup.PG_TEXT, true);
                ts.Commit();
            }

            var selectedElement = doc.GetElement(selectedRef); 
            if (selectedElement is Pipe)
            {
                Parameter diameterPar1 = selectedElement.LookupParameter("Внутренний диаметр");
                if (diameterPar1.StorageType == StorageType.Double)
                {
                    double d1 = diameterPar1.AsDouble();
                    Parameter diameterPar2 = selectedElement.LookupParameter("Внешний диаметр");
                    if (diameterPar2.StorageType == StorageType.Double)
                    {
                        double d2 = diameterPar2.AsDouble();
                        string c1 = Convert.ToString(d1);
                        string c2 = Convert.ToString(d2);
                        using (Transaction ts = new Transaction(doc, "Set parameters"))
                        {
                            ts.Start();
                            var familyInstance = selectedElement as FamilyInstance;
                            Parameter commentParameter = familyInstance.LookupParameter("Диаметр внешний/внутренний");
                            commentParameter.Set("c1/c2");

                            Parameter typeCommentsParameter = familyInstance.Symbol.LookupParameter("Диаметр внешний/внутренний");
                            typeCommentsParameter.Set("c1 / c2");
                            ts.Commit();
                        }
                    }
                }

            }
            return Result.Succeeded;

        }
        private void CreateSheredParameter(Autodesk.Revit.ApplicationServices.Application application, Document doc, string v1, CategorySet selectedElement, BuiltInParameterGroup PG_Text, bool isInstance)
        {
            DefinitionFile definitionFile = application.OpenSharedParameterFile();
            if (definitionFile == null)
            {
                TaskDialog.Show("Ошибка", "Не найден файл общих параметров");
                return;
            }

            Definition definition = definitionFile.Groups
                    .SelectMany(group => group.Definitions)
                    .FirstOrDefault(def => def.Name.Equals("Диаметр внешний/внутренний"));
            if (definitionFile == null)
            {
                TaskDialog.Show("Ошибка", "Не найден указанный параметров");
                return;
            }
            Binding binding = application.Create.NewTypeBinding(selectedElement);
            if (isInstance)
                binding = application.Create.NewInstanceBinding(selectedElement);
            BindingMap map = doc.ParameterBindings;
            map.Insert(definition, binding, PG_Text);
        }
    }
}
