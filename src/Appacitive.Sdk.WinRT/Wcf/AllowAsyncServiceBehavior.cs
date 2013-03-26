using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class AllowAsyncService : Attribute, IServiceBehavior, IOperationBehavior
    {
        private readonly object _syncRoot = new object();

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            /* IMPORTANT NOTE:
             * This service behavior assumes that the said operation and endpoint behaviors are stateless.
             * As a result it uses the same instance of the behaviors across all endpoints and operations.
             * All changes to these  behaviors need to ensure that the STATELESSNESS IS MAINTAINED.
             */

            lock (_syncRoot)
            {
                // Get endpoint and operation behaviors.
                // Install behaviors
                for (int i = 0; i < serviceDescription.Endpoints.Count; i++)
                {
                    var endpoint = serviceDescription.Endpoints[i];
                    // Add operation behavior

                    // Add the operation behavior to the endpoint
                    for (int j = 0; j < endpoint.Contract.Operations.Count; j++)
                    {
                        var operation = endpoint.Contract.Operations[j];
                        if (operation.Behaviors.Contains(typeof(AllowAsyncService)) == false)
                            operation.Behaviors.Add(this);
                    }

                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {   
        }

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {   
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {   
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new ContextPropogatingInvoker(dispatchOperation.Invoker);
        }

        public void Validate(OperationDescription operationDescription)
        {   
        }
    }
}
