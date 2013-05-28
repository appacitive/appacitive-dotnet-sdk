using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public static class PropagateOperationContext
    {
        /// <summary>
        ///     Propagate the operation context across thread boundaries (eg. for async / await).
        /// </summary>
        /// <param name="operationContext">
        ///     The operation context to propagate.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> implementation that restores the previous synchronisation context when disposed.
        /// </returns>
        /// <remarks>
        ///     Also sets the operation context, as a convenience, for the calling thread.
        ///     This is usually what you want, in async / await scenarios.
        /// </remarks>
        public static IDisposable Propagate(this OperationContext operationContext)
        {
            if (operationContext == null)
                throw new ArgumentNullException("operationContext");

            return
                new ContextScope(
                    new OperationContextPreservingSynchronizationContext(
                        operationContext
                    )
                );
        }

        /// <summary>
        ///     Use the operation context as the current operation context.
        /// </summary>
        /// <param name="operationContext">
        ///     The operation context to use.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> implementation that restores the operation context when disposed.
        /// </returns>
        /// <remarks>
        ///     Also sets the operation context, as a convenience, for the calling thread.
        ///     This is usually what you want, in async / await scenarios.
        /// </remarks>
        public static IDisposable Use(this OperationContext operationContext)
        {
            if (operationContext == null)
                throw new ArgumentNullException("operationContext");

            return new OperationContextScope(operationContext);
        }

        /// <summary>
        ///     Use the operation context as the current operation context, and propagate it across thread boundaries (eg. for async / await).
        /// </summary>
        /// <param name="operationContext">
        ///     The operation context to use / propagate.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> implementation that restores the previous synchronisation and operation contexts when disposed.
        /// </returns>
        public static IDisposable UseAndPropagate(this OperationContext operationContext)
        {
            if (operationContext == null)
                throw new ArgumentNullException("operationContext");

            return new ContextScope(new OperationContextPreservingSynchronizationContext(operationContext), operationContext);
        }
    }
}
