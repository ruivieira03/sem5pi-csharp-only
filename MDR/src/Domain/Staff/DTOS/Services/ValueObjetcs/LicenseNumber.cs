using System;
using Hospital.Domain.Shared;
using Newtonsoft.Json;

namespace Hospital.Domain.Users.staffmanagement
{
    public class LicenseNumber : EntityId
    {
        [JsonConstructor]
        public LicenseNumber(Guid value) : base(value)
        {
        }

        public LicenseNumber(String value) : base(value)
        {
        }

        private LicenseNumber() : base(Guid.Empty)
        {
        }

        override
        protected  Object createFromString(String text){
            return new Guid(text);
        }

        override
        public String AsString(){
            Guid obj = (Guid) base.ObjValue;
            return obj.ToString();
        }

        public Guid AsGuid(){
            return (Guid) base.ObjValue;
        } 
    }
}