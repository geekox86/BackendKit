// using System.Reflection;
// // using Abstraction;
// using HotChocolate.Types.Descriptors;

// namespace BackendKit;

// internal sealed class NamingConventions(string appName)
//   : DefaultNamingConventions
// {
//   internal string GetNamespacePrefix(Type type)
//   {
//     var namespaces = type.Namespace!.Split(".");

//     return namespaces[0] == appName && namespaces.Length == 3
//       ? $"{namespaces[1]}_{namespaces[2]}_"
//       : "";
//   }

//   public override string GetTypeName(Type type, TypeKind kind)
//   {
//     var namespacePrefix = GetNamespacePrefix(type);
//     var typeName = base.GetTypeName(type, kind);

//     return namespacePrefix != ""
//       ? namespacePrefix
//         + (
//           kind == TypeKind.InputObject
//             ? type.GetInterfaces().Contains(typeof(IRequestMessage))
//               ? typeName[0..^"MessageInput".Length]
//               : typeName[0..^"MessageInput".Length] + "Input"
//             : typeName[0..^"Message".Length]
//         )
//       : typeName;
//   }

//   public override string GetMemberName(MemberInfo member, MemberKind kind)
//   {
//     if (member is MethodInfo method && kind == MemberKind.ObjectField)
//     {
//       var extendedTypeName = method
//         .DeclaringType?.CustomAttributes.FirstOrDefault(attribute =>
//           attribute.AttributeType == typeof(ExtendObjectTypeAttribute)
//         )
//         ?.ConstructorArguments.First()
//         .Value!.ToString();

//       if (extendedTypeName == "Query" || extendedTypeName == "Mutation")
//       {
//         var requestTypeName = method.GetParameters()[0].ParameterType.Name;

//         return GetNamespacePrefix(method.DeclaringType!)
//           + char.ToLower(requestTypeName[0])
//           + requestTypeName[1..^"Message".Length];
//       }
//     }

//     return base.GetMemberName(member, kind);
//   }

//   public override string GetArgumentName(ParameterInfo parameter)
//   {
//     if (parameter.Name == "_" || parameter.Name == "causeMessage")
//     {
//       var extendedTypeName = parameter
//         .Member.DeclaringType?.CustomAttributes.FirstOrDefault(attribute =>
//           attribute.AttributeType == typeof(ExtendObjectTypeAttribute)
//         )
//         ?.ConstructorArguments.First()
//         .Value!.ToString();

//       if (extendedTypeName == "Query")
//         return "query";

//       if (extendedTypeName == "Mutation")
//         return "command";
//     }

//     return base.GetArgumentName(parameter)[0..^"Message".Length];
//   }
// }
