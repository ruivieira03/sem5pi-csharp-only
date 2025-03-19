using System;
using Hospital.Domain.Shared;
using Newtonsoft.Json;

namespace Hospital.Domain.OperationRequest
{
    public class OperationRequestId : EntityId{
        [JsonConstructor]
        public OperationRequestId(Guid value) : base(value)
        {
        }

        public OperationRequestId(String value) : base(value)
        {
        }

        public OperationRequestId() : base(Guid.Empty)
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