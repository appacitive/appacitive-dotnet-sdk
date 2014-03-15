using Appacitive.Sdk.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class InvokeWithUserContext : IOperationInvoker
    {
        public InvokeWithUserContext(IOperationInvoker invoker)
        {
            this.Internal = invoker;
        }

        public IOperationInvoker Internal {get; private set;}

        public object[] AllocateInputs()
        {   
            return this.Internal.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            var sessionId = AppSession.Setup();
            try
            {
                return this.Internal.Invoke(instance, inputs, out outputs);
            }
            finally
            {
                AppSession.Persist(sessionId);
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            var sessionId = AppSession.Setup();
            OperationContext.Current.Extensions.Add(new SessionIdExtension(sessionId));
            return this.Internal.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            Object obj;
            try
            {   
                obj = this.Internal.InvokeEnd(instance, out outputs, result);
            }
            finally
            {
                var ext = OperationContext.Current.Extensions.Find<SessionIdExtension>();
                if( ext != null )
                    AppSession.Persist(ext.SessionId);
                OperationContext.Current.Extensions.Remove(ext);
            }
            return obj;
        }

        public bool IsSynchronous
        {
            get { return this.Internal.IsSynchronous; }
        }
    }
}
