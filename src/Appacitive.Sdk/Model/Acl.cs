using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Appacitive.Sdk
{
    public class Acl
    {
        public Acl()
        {
            this.CurrentClaims = new List<Claim>();
            this.LastKnownClaims = new List<Claim>();
            this.ResetRequests = new List<ResetRequest>();
        }


        internal List<Claim> CurrentClaims { get; private set; }
        internal List<Claim> LastKnownClaims { get; private set; }
        internal List<ResetRequest> ResetRequests { get; private set; }

        public IEnumerable<Claim> Claims
        {
            get { return CurrentClaims; }
        }

        internal IEnumerable<Claim> Allowed
        {
            get 
            {
                // Allowed claims are 
                // 1. Present in the current claims
                // 2. Not present in the last known claims.
                var lastKnownAllowed = this.LastKnownClaims.Where(x => x.Permission == Permission.Allow).ToList();
                var currentAllowed = this.CurrentClaims.Where(x => x.Permission == Permission.Allow).ToList();
                return currentAllowed.Except(lastKnownAllowed);
            }
        }

        internal IEnumerable<Claim> Denied
        {
            get
            {
                // Allowed claims are 
                // 1. Present in the current claims
                // 2. Not present in the last known claims.
                var lastKnownDenied = this.LastKnownClaims.Where(x => x.Permission == Permission.Deny).ToList();
                var currentDenied = this.CurrentClaims.Where(x => x.Permission == Permission.Deny).ToList();
                return currentDenied.Except(lastKnownDenied);
            }
        }

        internal IEnumerable<ResetRequest> Reset
        {
            get { return this.ResetRequests;  }
        }


        public void AllowUser(string userId, params Access[] accessToAllow)
        {
            AllowAccess(ClaimType.User, userId, accessToAllow);
        }

        public void AllowUserGroup(string groupId, params Access[] accessToAllow)
        {
            AllowAccess(ClaimType.UserGroup, groupId, accessToAllow);
        }

        private void AllowAccess(ClaimType type, string sid, params Access[] accessToAllow)
        {
            // Spec:
            // To allow a user we add the corresponding allow claim and remove any deny claims.
            // We also remove any pending reset requests.

            var allowClaims = accessToAllow.Select(x => new Claim(Permission.Allow, x, type, sid));
            var denyClaims = accessToAllow.Select(x => new Claim(Permission.Deny, x, type, sid));
            // Add the new added claims
            allowClaims.For(c =>
            {
                if (this.CurrentClaims.Contains(c) == false)
                    this.CurrentClaims.Add(c);
            });
            denyClaims.For(c => 
                {
                    while( this.CurrentClaims.Contains(c) == true )
                        this.CurrentClaims.Remove(c);
                });

            accessToAllow.For(a =>
            {
                var request = new ResetRequest(type, sid, a);
                while (this.ResetRequests.Contains(request) == true)
                    this.ResetRequests.Remove(request);
            });
        }

        public void DenyUser(string userId, params Access[] accessToDeny)
        {
            DenyAccess(ClaimType.User, userId, accessToDeny);
        }

        public void DenyUserGroup(string groupId, params Access[] accessToDeny)
        {
            DenyAccess(ClaimType.UserGroup, groupId, accessToDeny);
        }

        private void DenyAccess(ClaimType type, string sid, params Access[] accessToDeny)
        {
            // Spec:
            // To allow a user we add the corresponding deny claim and remove any allow claims.
            // We also remove any pending reset requests.

            var allowClaims = accessToDeny.Select(x => new Claim(Permission.Allow, x, type, sid));
            var denyClaims = accessToDeny.Select(x => new Claim(Permission.Deny, x, type, sid));
            denyClaims.For(c =>
            {
                if (this.CurrentClaims.Contains(c) == false)
                    this.CurrentClaims.Add(c);
            });
            allowClaims.For(c => 
                {
                    while(this.CurrentClaims.Contains(c) == true )
                        this.CurrentClaims.Remove(c);
                });

            accessToDeny.For(a =>
                {
                    var request = new ResetRequest(type, sid, a);
                    while (this.ResetRequests.Contains(request) == true)
                        this.ResetRequests.Remove(request);
                });
            
        }

        public void ResetUser(string userId, params Access[] accessToReset)
        {
            // Resetting will remove any allow and deny claims
            // We also keep track of reset requests incase the changes are made on an empty proxy object.
            ResetAccess(ClaimType.User, userId, accessToReset);
        }

        public void ResetUserGroup(string groupId, params Access[] accessToReset)
        {
            ResetAccess(ClaimType.UserGroup, groupId, accessToReset);
        }

        private void ResetAccess(ClaimType type, string sid, params Access[] accessToReset)
        {
            // Spec:
            // To allow a user we add the corresponding deny claim and remove any allow claims.
            // We also remove any pending reset requests.

            var allowClaims = accessToReset.Select(x => new Claim(Permission.Allow, x, type, sid));
            var denyClaims = accessToReset.Select(x => new Claim(Permission.Deny, x, type, sid));

            denyClaims.For(c => 
                {
                    while(this.CurrentClaims.Contains(c) == true )
                        this.CurrentClaims.Remove(c);
                });
            allowClaims.For(c =>
            {
                while (this.CurrentClaims.Contains(c) == true)
                    this.CurrentClaims.Remove(c);
            });
            accessToReset.For(a =>
            {
                var request = new ResetRequest(type, sid, a);
                if (this.ResetRequests.Contains(request) == false)
                    this.ResetRequests.Add(request);
            });
        }

        internal void SetInternal(IEnumerable<Claim> claims)
        {
            this.LastKnownClaims.Clear();
            this.CurrentClaims.Clear();
            this.ResetRequests.Clear();
            this.LastKnownClaims.AddRange(claims);
            this.CurrentClaims.AddRange(claims);
        }
    }

    public class ResetRequest : IEquatable<ResetRequest>
    {
        public ResetRequest(ClaimType type, string sid, Access access)
        {
            this.Type = type;
            this.Sid = sid;
            this.Access = access;
        }

        public ClaimType Type {get; private set;}

        public string Sid {get; private set;}

        public Access Access { get; private set; }

        public bool Equals(ResetRequest other)
        {
            if (other == null)
                return false;
            return 
                this.Type == other.Type &&
                this.Access == other.Access && 
                this.Sid.Equals(other.Sid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ResetRequest);
        }

        public override int GetHashCode()
        {
            var sid = (this.Sid ?? string.Empty).ToLower();
            return this.Type.GetHashCode() ^ sid.GetHashCode() ^ this.Access.GetHashCode() ;
        }
    }

    public class Claim : IEquatable<Claim>, IEqualityComparer<Claim>
    {
        public Claim(Permission permission, Access accessType, ClaimType claimType, string sid)
        {
            this.Permission = permission;
            this.AccessType = accessType;
            this.ClaimType = claimType;
            this.Sid = sid;
        }

        public Permission Permission { get; set; }

        public string Sid { get; set; }

        public Access AccessType { get; set; }

        public ClaimType ClaimType { get; set; }

        public bool Equals(Claim other)
        {
            if (other == null)
                return false;
            return
                this.Sid.Equals(other.Sid, StringComparison.OrdinalIgnoreCase) &&
                this.AccessType == other.AccessType &&
                this.Permission == other.Permission &&
                this.ClaimType == other.ClaimType;
        }


        public override bool Equals(object obj)
        {
            return this.Equals(obj as Claim);
        }

        public override int GetHashCode()
        {
            var sid = this.Sid ?? string.Empty;
            return sid.GetHashCode() ^ this.ClaimType.GetHashCode() ^ this.AccessType.GetHashCode() ^ this.Permission.GetHashCode();
        }

        public bool Equals(Claim x, Claim y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Claim obj)
        {
            return obj.GetHashCode();
        }
    }

    
}
