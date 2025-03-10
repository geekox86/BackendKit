using HotChocolate.Configuration;
using HotChocolate.Internal;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using OneOf;

namespace BackendKit;

internal sealed class UnionTypeInterceptor : TypeInterceptor
{
  public override void OnBeforeRegisterDependencies(
    ITypeDiscoveryContext discoveryContext,
    DefinitionBase definition
  )
  {
    var namingConventions = (NamingConventions)
      discoveryContext.DescriptorContext.Naming;

    // TODO Encapsulate into NamingConventions
    string GetNameForOneOf(Type oneOf, ObjectFieldDefinition field) =>
      namingConventions.GetNamespacePrefix(oneOf)
      + field.Name
      + "Of"
      + definition.Name[0..^"Message".Length];

    string GetNameForOneOfBase(Type oneOfBase) =>
      namingConventions.GetTypeName(oneOfBase, TypeKind.Union);

    // TODO Encapsulate into NamingConventions
    string GetNameForQueryOrMutationField(ObjectFieldDefinition field) =>
      namingConventions.GetTypeName(
        field.Arguments[0].Parameter!.ParameterType,
        TypeKind.InputObject
      ) + "Result";

    string GetNameForAbstract(Type @abstract) =>
      namingConventions.GetTypeName(@abstract, TypeKind.Union);

    if (definition is ObjectTypeDefinition objectTypeDefinition)
    {
      foreach (var field in objectTypeDefinition.Fields)
      {
        if (
          !field.IsIntrospectionField
          && field.Type is ExtendedTypeReference typeReference
          && UnwrapType(typeReference.Type) is Type unwrappedType
        )
        {
          if (IsOneOf(unwrappedType))
          {
            var name =
              IsOneOfBase(unwrappedType) ? GetNameForOneOfBase(unwrappedType)
              : objectTypeDefinition.Name == "Query"
              || objectTypeDefinition.Name == "Mutation"
                ? GetNameForQueryOrMutationField(field)
              : GetNameForOneOf(unwrappedType, field);
            var members = IsOneOfBase(unwrappedType)
              ? ExtractMembersFromOneOf(unwrappedType.BaseType!)
              : ExtractMembersFromOneOf(unwrappedType);
            var unionType = CreateUnionType(name, members);
            var wrappedType = WrapType(typeReference.Type, unionType);

            field.Type = TypeReference.Create(wrappedType);

            field.MiddlewareDefinitions.Add(_oneOfFieldTransformer);
          }
          else if (IsAbstract(unwrappedType))
          {
            var name = GetNameForAbstract(unwrappedType);
            var members = ExtractMembersFromAbstract(unwrappedType);
            var unionType = CreateUnionType(name, members);
            var wrappedType = WrapType(typeReference.Type, unionType);

            field.Type = TypeReference.Create(wrappedType);
          }
        }
      }
    }
  }

  private bool IsOneOf(Type type) =>
    type.GetInterfaces().Any(@interface => @interface == typeof(IOneOf));

  private bool IsOneOfBase(Type type) =>
    type.BaseType?.GetInterfaces()
      .Any(@interface => @interface == typeof(IOneOf)) ?? false;

  private bool IsAbstract(Type type) => type.IsAbstract;

  private IEnumerable<Type> ExtractMembersFromOneOf(Type oneOf) =>
    oneOf
      .GetGenericArguments()
      .SelectMany(type =>
        IsOneOfBase(type) ? ExtractMembersFromOneOf(type.BaseType!)
        : IsOneOf(type) ? ExtractMembersFromOneOf(type)
        : IsAbstract(type) ? ExtractMembersFromAbstract(type)
        : [type]
      )
      .ToList();

  private IEnumerable<Type> ExtractMembersFromAbstract(Type @abstract) =>
    @abstract
      .Assembly.GetTypes()
      .Where(type => type.IsSubclassOf(@abstract) && !type.IsAbstract)
      .ToList();

  private UnionType CreateUnionType(string name, IEnumerable<Type> members) =>
    new(descriptor =>
    {
      descriptor.Name(name);

      foreach (var member in members)
      {
        if (!_memberObjectTypes.ContainsKey(member))
        {
          var memberObjectType = (ObjectType)
            Activator.CreateInstance(
              typeof(ObjectType<>).MakeGenericType(member)
            )!;

          _memberObjectTypes.Add(member, memberObjectType);
        }

        descriptor.Type(_memberObjectTypes[member]);
      }
    });

  private IType WrapType(
    IExtendedType type,
    IType wrappedType,
    bool checkNullability = true
  ) =>
    checkNullability && !type.IsNullable
      ? WrapType(type, new NonNullType(wrappedType), false)
    : type.IsArrayOrList
      ? WrapType(type.ElementType!, new ListType(wrappedType))
    : wrappedType;

  private Type? UnwrapType(IExtendedType type) =>
    type.IsArrayOrList ? UnwrapType(type.ElementType!) : type.Type;

  private readonly Dictionary<Type, ObjectType> _memberObjectTypes = new();

  private readonly FieldMiddlewareDefinition _oneOfFieldTransformer = new(
    next =>
      async context =>
      {
        await next(context);

        while (context.Result is IOneOf oneOf)
        {
          context.Result = oneOf.Value;
        }
      }
  );
}
