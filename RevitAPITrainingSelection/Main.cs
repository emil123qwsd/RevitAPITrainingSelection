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

            IList<Reference> selectedRef = uidoc.Selection.PickObjects(ObjectType.Element, "Выберите трубы");

            double info = 0;
           
            var categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

            using (Transaction ts=new Transaction(doc, "Add parameter"))
            {
                ts.Start();
                CreateSheredParameter(uiapp.Application, doc, "Длинна с запасом 10 процентов", categorySet, BuiltInParameterGroup.PG_TEXT, true);
                ts.Commit();
            }

            foreach (var element in selectedRef)
            {
                var selectedElement = doc.GetElement(element);

                if (selectedElement is Pipe)
                {
                    Parameter lParameter = selectedElement.LookupParameter("Длина");
                    if (lParameter.StorageType == StorageType.Double)
                    {
                        double lValue = lParameter.AsDouble();
                        using (Transaction ts=new Transaction(doc, "Set Parameters"))
                        {
                            ts.Start();
                            var familyInstance = (Pipe)selectedElement;
                            Parameter parameter = familyInstance.LookupParameter("Длинна с запасом 10 процентов");
                            parameter.Set(lValue * 1.1);
                            ts.Commit();
                        }
                        info += lValue;
                    }
                }
            }
            TaskDialog.Show("Длина", info.ToString());
            return Result.Succeeded;
        }

        private void CreateSheredParameter(Autodesk.Revit.ApplicationServices.Application application, Document doc, string v1, CategorySet categorySet, BuiltInParameterGroup pG_Text, bool isInstance)
        {
                DefinitionFile definitionFile = application.OpenSharedParameterFile();
                if (definitionFile == null)
                {
                    TaskDialog.Show("Ошибка", "Не найден файл общих параметров");
                    return;
                }

            Definition definition = definitionFile.Groups
                    .SelectMany(group => group.Definitions)
                    .FirstOrDefault(def => def.Name.Equals("Длинна с запасом 10 процентов"));
                if (definitionFile == null)
                {
                    TaskDialog.Show("Ошибка", "Не найден указанный параметров");
                    return;
                }
            Binding binding = application.Create.NewTypeBinding(categorySet);
            if (isInstance)
                binding = application.Create.NewInstanceBinding(categorySet);
            BindingMap map = doc.ParameterBindings;
            map.Insert(definition, binding, pG_Text);
        }
    }
}
