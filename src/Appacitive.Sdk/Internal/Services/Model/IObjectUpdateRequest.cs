using System;
using System.Collections.Generic;

namespace Appacitive.Sdk.Services
{
    public interface IObjectUpdateRequest
    {
        List<string> AddedTags { get; }
        
        List<global::Appacitive.Sdk.Claim> AllowClaims { get; }
        
        IDictionary<string, string> AttributeUpdates { get; }
        
        List<global::Appacitive.Sdk.Claim> DenyClaims { get; }
        
        string Id { get; set; }
        
        IDictionary<string, object> PropertyUpdates { get; }
        
        List<string> RemovedTags { get; }
        
        List<global::Appacitive.Sdk.ResetRequest> ResetClaims { get; }
        
        bool ReturnAcls { get; set; }
        
        int Revision { get; set; }
        
        string Type { get; set; }
    }
}
