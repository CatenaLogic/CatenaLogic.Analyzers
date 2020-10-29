namespace CatenaLogic.Analyzers
{
    using Gu.Roslyn.AnalyzerExtensions;

    internal static class KnownSymbols
    {
        internal static readonly TaskType Task = new TaskType();
        internal static readonly TaskOfTType TaskOfT = new TaskOfTType();

        private static QualifiedType Create(string qualifiedName, string alias = "")
        {
            return new QualifiedType(qualifiedName, alias);
        }
    }

    internal class TaskType : QualifiedType
    {
        public TaskType()
            : base("System.Threading.Tasks.Task")
        {
        }
    }

    internal class TaskOfTType : QualifiedType
    {
        public TaskOfTType()
            : base("System.Threading.Tasks.Task<T>")
        {
        }
    }
}
