using System;
using Hospital.Domain.Shared;
using Newtonsoft.Json;

namespace Hospital.Domain.Users.SystemUser{
    public class SystemUserId : EntityId{
        [JsonConstructor]
        public SystemUserId(Guid value) : base(value){
        }

        public SystemUserId(String value) : base(value){
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