using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    internal class ContextPropogatingInvoker : IOperationInvoker
    {
        public ContextPropogatingInvoker(IOperationInvoker invoker)
        {
            _invoker = invoker;
        }

        private IOperationInvoker _invoker;

        public object[] AllocateInputs()
        {
            return _invoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            using (OperationContext.Current.Propagate())
            {
                return _invoker.Invoke(instance, inputs, out outputs);
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("InvokeBegin is not supported.");
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw new NotSupportedException("InvokeEnd is not supported.");
        }

        public bool IsSynchronous
        {
            get { return _invoker.IsSynchronous; }
        }
    }
}
