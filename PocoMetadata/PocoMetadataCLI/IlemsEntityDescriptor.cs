﻿using System;
using System.Reflection;

namespace Breeze.PocoMetadata
{
    public class IlemsEntityDescriptor : EntityDescriptor
    {
        public override bool Include(Type type)
        {
            var name = type.Name;
            //if (type.Name == "EvidenceItem" || type.Name == "SubjectLinkedAssociator") return true;
            //return false;

            if (name == "EntityKey") return false;
            if (name == "IAssociation") return true;
            if (name == "IIntoxicationTest") return true;
            if (name == "ILocation") return true;
            if (name == "IModelEntity") return true;
            if (name == "IReportSection") return true;
            if (name == "ISubject") return true;
            if (name == "IVehicle") return true;
            if (name == "IReportSection") return true;

            var btype = type;
            while (btype != null)
            {
                // for entities, we encounter EntityBase before ElementBase and ModelBase in the hierarchy
                if (btype.Name == "EntityBase") return true;
                if (btype.Name == "ElementBase") return true;
                if (btype.Name == "ModelBase") return true;
                btype = btype.BaseType;
            }
            return false;
        }

        public override string GetAutoGeneratedKeyType(Type type)
        {
            var name = type.Name;
            //if (name == "Comment" || name == "OrderDetail" || name == "PreviousEmployee"
            //    || name == "Region" || name == "Territory" || name == "InternationalOrder")
            //{
            //    return "None";
            //}
            return "KeyGenerator";
        }

        public override bool IsComplexType(Type type)
        {
            if (type.Namespace.StartsWith("System")) return false;
            if (type.Name == "IAssociation") return true;
            // All entities extend from EntityBase
            var btype = type;
            while (btype != null)
            {
                // for entities, we encounter EntityBase before ElementBase and ModelBase in the hierarchy
                if (btype.Name == "EntityBase") return false;
                if (btype.Name == "IModelElement") return true;
                if (btype.Name == "ElementBase") return true;
                if (btype.Name == "ModelBase") return true;
                btype = btype.BaseType;
            }
            return false;
            //if (type.Name == "EntityKey") return true;
            //if (type.Name == "DistanceFromLocation") return true;
            //if (type.Name == "GeoCoordinateExtended") return true;
            ////if (type.Name == "Location") return true;
            //return false;
        }

        public override bool IsKeyProperty(Type type, PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            if (name == "EntityKey") return true;
            return false;
        }

        public override string GetForeignKeyName(Type type, PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            //if (name == "InternationalOrder") return "OrderID";
            //if (name == "Manager") return "ReportsToEmployeeID";
            //if (name == "Category2") return "CategoryID2";
            return name + "FKey";
        }

        public override Type GetDataPropertyType(Type type, PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            if (name == "JmodelVersion" || name == "CmodelVersion" || 
                name == "TypeName" || name == "EntityType" || name == "PersistenceKeys")
                return null;
            if (propertyInfo.PropertyType.Name == "EntityKey")
                return typeof(System.Guid);
            return propertyInfo.PropertyType;
        }

        public override MissingFKHandling GetMissingFKHandling(Type type, PropertyInfo propertyInfo)
        {
            return MissingFKHandling.Add;
        }

    }
}