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

namespace RevitAPITrainingLibrary
{
    public class DuctsUtils
    {
        public static List<DuctType> GetDuctTypes(ExternalCommandData commandData)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<DuctType> ductTypes = new FilteredElementCollector(doc)
                .OfClass(typeof(DuctType))
                .Cast<DuctType>()
                .ToList();

            return ductTypes;
        }

    } 
    public class SystemType
    {
        public static MEPSystemType GetSystemType(ExternalCommandData commandData)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            MEPSystemType systemType = new FilteredElementCollector(doc)
    .OfClass(typeof(MEPSystemType))
    .Cast<MEPSystemType>()
    .FirstOrDefault(m => m.SystemClassification == MEPSystemClassification.SupplyAir);

            return systemType;
        }
    }
}
