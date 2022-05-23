// using System;
// using System.Collections;
// using System.Reflection;
//
// namespace Meadow.Reflection.ObjectTree
// {
//     public class CollectionAccessNode : AccessNode
//     {
//         private readonly PropertyInfo _collectionPropertyInfo;
//         private readonly Type _elementType;
//
//         public CollectionAccessNode(string name, PropertyInfo collectionPropertyInfo, PropertyInfo info,
//             Type elementType, bool isReadonly) : base(name, info)
//         {
//             _collectionPropertyInfo = collectionPropertyInfo;
//
//             _elementType = elementType;
//         }
//         
//         private object GetSingleObjectFromCollection(object rootObject)
//         {
//             var collection = (ICollection) _collectionPropertyInfo.GetValue(rootObject);
//
//             var col = new CollectionCollection(collection);
//
//             if (col.Count == 0)
//             {
//                 col.Add(_elementType.GetConstructor(new Type[] { })?.Invoke(new object[] { }));
//             }
//
//             var e = collection.GetEnumerator();
//             e.Reset();
//             e.MoveNext();
//             return e.Current;
//         }
//
//         public override void SetValue(object rootObject, object value)
//         {
//             var singleElement = GetSingleObjectFromCollection(rootObject);
//             
//             PropertyInfo.SetValue(singleElement, value);
//         }
//
//         public override object GetValue(object rootObject)
//         {
//             var singleElement = GetSingleObjectFromCollection(rootObject);
//
//             return PropertyInfo.GetValue(singleElement);
//         }
//     }
// }