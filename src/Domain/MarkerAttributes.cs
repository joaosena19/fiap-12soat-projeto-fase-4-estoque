namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateRootAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class AggregateMemberAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class ValueObjectAttribute : Attribute { }
}
