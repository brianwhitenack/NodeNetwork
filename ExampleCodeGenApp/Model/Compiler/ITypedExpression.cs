namespace ExampleCodeGenApp.Model.Compiler
{
    public interface IExpression
    {

    }

    public interface ITypedExpression<T> : IExpression
    {
        public T Evaluate();
    }

    public interface ITypedVariable<T> : ITypedExpression<T>
    {
        public void SetValue(T value);
    }
}
