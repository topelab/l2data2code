namespace L2Data2Code.SharedContext.Base
{
    public class DelegateCommandFactory : IDelegateCommandFactory
    {
        public IDelegateCommand Create()
        {
            return new DelegateCommandWrapper(Execute, CanExecute);
        }

        protected virtual void Execute() { }

        protected virtual bool CanExecute() => true;
    }

    public class DelegateCommandFactory<T> : IDelegateCommandFactory<T>
    {
        public IDelegateCommand Create()
        {
            return new DelegateCommandWrapper<T>(Execute, CanExecute);
        }

        protected virtual void Execute(T parameter) { }

        protected virtual bool CanExecute(T parameter) => true;
    }

}
