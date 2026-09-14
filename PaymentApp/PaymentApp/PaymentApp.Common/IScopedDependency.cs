using System;

namespace PaymentApp.Common
{
    //just to mark
    public interface IScopedDependency
    {
    }

    public interface ITransientDependency
    {
    }

    public interface ISingletonDependency
    {
    }

    public interface IIgnoreDependency
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IgnoreMigrationAttribute : Attribute
    {
    }
}
