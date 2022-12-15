using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace V2Ray.Api.Extensions
{
    public class Mapper
    {
        /// <summary>
        /// maps Tfrom to Tto
        /// </summary>
        public IEnumerable<Tto> Create<Tfrom, Tto>(
            IEnumerable<Tfrom> model,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            if (model == null) return null;
            return model.Select(o => Create<Tfrom, Tto>(o, typeMappings, exclude));
        }

        /// <summary>
        /// maps Tfrom to Tto
        /// </summary>
        public IEnumerable<Tto> Create<Tfrom, Tto>(
            IEnumerable<Tfrom> model,
            params string[] exclude
        )
        {
            return Create<Tfrom, Tto>(model, null, exclude);
        }

        /// <summary>
        /// maps Tfrom to Tto
        /// </summary>
        public Tto Create<Tfrom, Tto>(
            Tfrom model,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            if (model == null) return default;

            var TtoType = typeof(Tto);

            // create an insTfromnce from Tto class
            var to = (Tto)Activator.CreateInstance(TtoType);

            Update(model, to, typeMappings, exclude);

            return to;
        }

        /// <summary>
        /// maps Tfrom to Tto
        /// </summary>
        public Tto Create<Tfrom, Tto>(
            Tfrom model,
            params string[] exclude
        )
        {
            return Create<Tfrom, Tto>(model, null, exclude);
        }

        /// <summary>
        /// maps Tfrom to Tto
        /// </summary>
        public void Update<Tfrom, Tto>(
            IEnumerable<Tfrom> from,
            IEnumerable<Tto> to,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            var newEnumerable = to = from.Select(o => Create<Tfrom, Tto>(o, typeMappings, exclude));
            to = newEnumerable;
        }

        /// <summary>
        /// Updates propeties of 'to' by 'from' properties
        /// </summary>
        public void Update<Tfrom, Tto>(
            Tfrom from,
            Tto to,
            params string[] exclude
        )
        {
            Update(from, to, null, exclude);
        }

        /// <summary>
        /// Updates propeties of 'to' by 'from' properties
        /// </summary>
        public void Update<Tfrom, Tto>(
            Tfrom from,
            Tto to,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            if (from == null || to == null)
                return;

            typeMappings ??= new Dictionary<Type, Type>();

            exclude ??= new string[0];

            var TfromType = typeof(Tfrom);
            var TtoType = typeof(Tto);

            // get Tfrom class properties list
            var fromProps = TfromType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            // iterate over Tfrom class properties and map them one by one
            foreach (var fromProp in fromProps)
            {
                // check if property name is excluded
                if (exclude.Contains(fromProp.Name))
                    continue;

                // get equivalent property in Tto class
                var toProp = TtoType.GetProperty(fromProp.Name, BindingFlags.Public | BindingFlags.Instance);

                if (toProp == null || !toProp.CanWrite)
                    continue;

                var fromPropType = fromProp.PropertyType;
                var toPropType = toProp.PropertyType;

                var fromPropValue = fromProp.GetValue(from, null);

                if (fromPropType == toPropType)
                {
                    toProp.SetValue(to, fromPropValue);
                }

                //else if (IsValueType(fromPropType))
                //{
                //    toProp.SetValue(to, fromPropValue);
                //}

                // check if t1 property is found in mappings
                else if (typeMappings.ContainsKey(fromPropType))
                {
                    var mappedType = typeMappings[fromPropType];
                    if (mappedType != toPropType)
                        continue;

                    var childExclude = exclude.Where(o => o.StartsWith(fromProp.Name)).Select(o => o.Substring(fromProp.Name.Length + 1)).ToArray();

                    var val = GetComplexValue(from, to, fromProp, toProp, typeMappings, childExclude);
                    toProp.SetValue(to, val);
                }

                // check if IEnumerable<mappedType> property is found in mappings
                else if (typeMappings.Keys.Any(tm => fromPropType == GetIEnumerableType(tm)))
                {
                    var mappedType = typeMappings[fromPropType.GetGenericArguments()[0]];

                    if (IsIEnumerableOf(toPropType, mappedType))
                    {
                        var childExclude = exclude.Where(o => o.StartsWith(fromProp.Name)).Select(o => o.Substring(fromProp.Name.Length + 1)).ToArray();

                        var val = GetComplexValue(from, to, fromProp, toProp, typeMappings, childExclude);
                        toProp.SetValue(to, val);
                    }
                }

                // check if IEnumerable<mappedType> property is found in mappings
                else if (typeMappings.Keys.Any(tm => fromPropType == GetICollectionType(tm)))
                {
                    var mappedType = typeMappings[fromPropType.GetGenericArguments()[0]];

                    if (IsICollectionOf(toPropType, mappedType))
                    {
                        var childExclude = exclude.Where(o => o.StartsWith(fromProp.Name)).Select(o => o.Substring(fromProp.Name.Length + 1)).ToArray();

                        var val = GetComplexValue(from, to, fromProp, toProp, typeMappings, childExclude);
                        toProp.SetValue(to, val);
                    }
                }

                //else
                //{
                //    var val = GetComplexValue(from, to, fromProp, toProp, typeMappings, exclude);
                //    toProp.SetValue(to, val);
                //}
            }
        }

        private object GetComplexValue(
            object from,
            object to,
            PropertyInfo fromProp,
            PropertyInfo toProp,
            Dictionary<Type, Type> typeMappings,
            string[] exclude
        )
        {
            var fromPropType = fromProp.PropertyType;
            var toPropType = toProp.PropertyType;
            var fromPropValue = fromProp.GetValue(from, null);

            if (fromPropValue == null)
                return null;

            if (IsIEnumerable(toPropType))
            {
                var fromItemType = fromPropType.GetGenericArguments()[0];
                var toItemType = toPropType.GetGenericArguments()[0];

                if (fromPropType == toPropType)
                    if (IsValueType(fromItemType) || toItemType.IsAbstract)
                        return fromProp.GetValue(from, null);

                var fromPropValueType = fromProp.GetValue(from, null).GetType();

                MethodInfo methodInfo;

                if (fromItemType.MakeArrayType() == fromPropValueType)
                {
                    methodInfo = GetType().GetMethod(nameof(Mapper.CreateArray), BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else if (typeof(HashSet<>).MakeGenericType(fromItemType) == fromPropValueType)
                {
                    methodInfo = GetType().GetMethod(nameof(Mapper.CreateHashSet), BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else if (typeof(List<>).MakeGenericType(fromItemType) == fromPropValueType)
                {
                    methodInfo = GetType().GetMethod(nameof(Mapper.CreateList), BindingFlags.NonPublic | BindingFlags.Instance);
                }
                else
                    throw new NotImplementedException($"{nameof(toPropType.Name)} is not supported");

                var genericCreateMethodInfo = methodInfo.MakeGenericMethod(fromItemType, toItemType);
                var fromValue = fromProp.GetValue(from, null);

                return genericCreateMethodInfo.Invoke(this, new[] { fromValue, typeMappings, exclude });
            }
            else
            {
                var toPropValue = toProp.GetValue(to, null);
                if (toPropValue == null)
                    toPropValue = Activator.CreateInstance(toProp.PropertyType);

                var methodInfo = GetType().GetMethod(nameof(Mapper.UpdateObject), BindingFlags.NonPublic | BindingFlags.Instance);
                var genericCreateMethodInfo = methodInfo.MakeGenericMethod(fromPropValue.GetType(), toPropValue.GetType());
                genericCreateMethodInfo.Invoke(this, new[] { fromPropValue, toPropValue, typeMappings, exclude });

                return toPropValue;
            }
        }

        public object GetDefault(Type t)
        {
            return GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
        }

        public T GetDefaultGeneric<T>()
        {
            return default;
        }

        private void UpdateObject<Tfrom, Tto>(
            Tfrom from,
            Tto to,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            Update(from, to, typeMappings, exclude);
        }

        private HashSet<Tto> CreateHashSet<Tfrom, Tto>(
            HashSet<Tfrom> model,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            if (model == null) return null;

            return model.Select(o => Create<Tfrom, Tto>(o, typeMappings, exclude)).ToHashSet();
        }

        private List<Tto> CreateList<Tfrom, Tto>(
            List<Tfrom> model,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            if (model == null) return null;

            return model.Select(o => Create<Tfrom, Tto>(o, typeMappings, exclude)).ToList();
        }

        private Tto[] CreateArray<Tfrom, Tto>(
            Tfrom[] model,
            Dictionary<Type, Type> typeMappings,
            params string[] exclude
        )
        {
            if (model == null) return null;

            return model.Select(o => Create<Tfrom, Tto>(o, typeMappings, exclude)).ToArray();
        }

        private Type GetHashSetType(Type t)
        {
            return typeof(HashSet<>).MakeGenericType(t);
        }

        private Type GetIEnumerableType(Type t)
        {
            return typeof(IEnumerable<>).MakeGenericType(t);
        }

        private bool IsIEnumerable(Type tIEnumerable)
        {
            return tIEnumerable.IsGenericType && tIEnumerable.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private bool IsIEnumerableOf(Type tIEnumerable, Type t)
        {
            return IsIEnumerable(tIEnumerable) && tIEnumerable.GetGenericArguments()[0] == t;
        }

        private Type GetICollectionType(Type t)
        {
            return typeof(ICollection<>).MakeGenericType(t);
        }

        private bool IsICollectionOf(Type tCollection, Type t)
        {
            return tCollection.IsGenericType && tCollection.GetGenericTypeDefinition() == typeof(ICollection<>) && tCollection.GetGenericArguments()[0] == t;
        }

        private bool IsValueType(Type t)
        {
            return t.IsValueType || t == typeof(string);
        }
    }
}