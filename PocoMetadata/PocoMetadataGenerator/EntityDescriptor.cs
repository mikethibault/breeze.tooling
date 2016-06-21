﻿using System;
using System.Reflection;

namespace Breeze.PocoMetadata
{
    /// <summary>
    /// Describes the metadata for a set of entities.
    /// 
    /// The PocoMetadataBuilder calls methods on this class to determine how to generate
    /// metadata for the entities.
    /// 
    /// Extend this class to adapt to your data model.
    /// </summary>
    public class EntityDescriptor
    {
        /// <summary>
        /// Filter types from metadata generation.
        /// </summary>
        /// <returns>
        /// true if a Type should be included, false otherwise.
        /// </returns>
        /// <example>
        /// // exclude certain entities, and all Audit* entities
        /// var excluded = new string[] { "Comment", "LogRecord", "UserPermission" };
        /// bool Include(Type type)
        /// {
        ///   if (excluded.Contains(type.Name)) return false;
        ///   if (type.Name.StartsWith("Audit")) return false;
        ///   return true;
        /// };
        /// </example>
        public virtual bool Include(Type type)
        {
            return true;
        }

        /// <summary>
        /// Get the autoGeneratedKeyType value for the given type.  Should be defined even if the actual key property is on a base type.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <returns>One of:
        /// "Identity" - key is generated by db server, or is a Guid.
        /// "KeyGenerator" - key is generated by code on app server, e.g. using Breeze.ContextProvider.IKeyGenerator 
        /// "None" - key is not auto-generated, but is assigned manually.
        /// null - same as None.
        /// </returns>
        public virtual string GetAutoGeneratedKeyType(Type type)
        {
            return null;
        }

        /// <summary>
        /// Get the server resource name (endpoint) for the given type.  E.g. for entity type Product, it might be "Products".
        /// This value is used by Breeze client when composing a query URL for an entity.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <returns>Resource name</returns>
        public virtual string GetResourceName(Type type)
        {
            return Pluralize(type.Name);
        }

        /// <summary>
        /// Determine if the given type is a "Complex Type" instead of an "Entity".
        /// Complex Types are sometimes called component types or embedded types, because they are 
        /// part of the parent entity instead of being related by foreign keys.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <returns>true for a complex type, false for an entity</returns>
        public virtual bool IsComplexType(Type type)
        {
            return false;
        }

        /// <summary>
        /// Determine if the property is part of the entity key.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <param name="propertyInfo">Property being considered</param>
        /// <returns>True if property is part of the entity key, false otherwise</returns>
        public virtual bool IsKeyProperty(Type type, PropertyInfo propertyInfo)
        {
            var name = propertyInfo.Name;
            if (name == (type.Name + "ID")) return true;
            if (name == "ID") return true;
            return false;
        }

        /// <summary>
        /// Determine if the property is a version property used for optimistic concurrency control.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <param name="propertyInfo">Property being considered</param>
        /// <returns>True if property is the entity's version property, false otherwise</returns>
        public virtual bool IsVersionProperty(Type type, PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == "RowVersion") return true;
            return false;
        }

        /// <summary>
        /// Change the type of the given data property in the metadata.
        /// For example, a custom wrapper type on the server may be unwrapped on the client, and the metadata reflects this.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <param name="propertyInfo">Property being considered</param>
        /// <returns>Type of the property to put in the metadata, or null to exclude the property.</returns>
        public virtual Type GetDataPropertyType(Type type, PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType;
        }

        /// <summary>
        /// Get the foreign key for the given scalar navigation property.  This is another property on the 
        /// same entity that establishes the foreign key relationship.
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <param name="propertyInfo">Scalar navigation/association property</param>
        /// <returns>Name of the related property</returns>
        /// <example>if Order has a Customer navigation property, that is related via the CustomerID data property, 
        /// we would return "CustomerID"
        /// </example>
        public virtual string GetForeignKeyName(Type type, PropertyInfo propertyInfo)
        {
            return propertyInfo.Name + "ID";
        }

        /// <summary>
        /// Whether to throw an error if a foreign key data property cannot be found for the given navigation property
        /// </summary>
        /// <param name="type">Entity type for which metadata is being generated</param>
        /// <param name="propertyInfo">Scalar navigation/association property</param>
        /// <returns>true to throw an error, false to ignore and continue</returns>
        public virtual bool ThrowOnForeignKeyError(Type type, PropertyInfo propertyInfo)
        {
            return true;
        }

        /// <summary>
        /// Lame pluralizer.  Assumes we just need to add a suffix.  
        /// Consider using System.Data.Entity.Design.PluralizationServices.PluralizationService.
        /// </summary>
        /// <param name="s">String to pluralize</param>
        /// <returns>Pseudo-pluralized string</returns>
        public virtual string Pluralize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var last = s.Length - 1;
            var c = s[last];
            switch (c)
            {
                case 'y':
                    return s.Substring(0, last) + "ies";
                default:
                    return s + 's';
            }
        }

    }
}
