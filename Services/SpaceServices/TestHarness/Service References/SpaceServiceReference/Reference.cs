﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestHarness.SpaceServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SpaceServiceReference.SpaceServiceSoap")]
    public interface SpaceServiceSoap {
        
        // CODEGEN: Generating message contract since element name GetPlayerResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetPlayer", ReplyAction="*")]
        TestHarness.SpaceServiceReference.GetPlayerResponse GetPlayer(TestHarness.SpaceServiceReference.GetPlayerRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetPlayer", ReplyAction="*")]
        System.Threading.Tasks.Task<TestHarness.SpaceServiceReference.GetPlayerResponse> GetPlayerAsync(TestHarness.SpaceServiceReference.GetPlayerRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetPlayerRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetPlayer", Namespace="http://tempuri.org/", Order=0)]
        public TestHarness.SpaceServiceReference.GetPlayerRequestBody Body;
        
        public GetPlayerRequest() {
        }
        
        public GetPlayerRequest(TestHarness.SpaceServiceReference.GetPlayerRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetPlayerRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int PlayerId;
        
        public GetPlayerRequestBody() {
        }
        
        public GetPlayerRequestBody(int PlayerId) {
            this.PlayerId = PlayerId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetPlayerResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetPlayerResponse", Namespace="http://tempuri.org/", Order=0)]
        public TestHarness.SpaceServiceReference.GetPlayerResponseBody Body;
        
        public GetPlayerResponse() {
        }
        
        public GetPlayerResponse(TestHarness.SpaceServiceReference.GetPlayerResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetPlayerResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string GetPlayerResult;
        
        public GetPlayerResponseBody() {
        }
        
        public GetPlayerResponseBody(string GetPlayerResult) {
            this.GetPlayerResult = GetPlayerResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface SpaceServiceSoapChannel : TestHarness.SpaceServiceReference.SpaceServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SpaceServiceSoapClient : System.ServiceModel.ClientBase<TestHarness.SpaceServiceReference.SpaceServiceSoap>, TestHarness.SpaceServiceReference.SpaceServiceSoap {
        
        public SpaceServiceSoapClient() {
        }
        
        public SpaceServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SpaceServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SpaceServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SpaceServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TestHarness.SpaceServiceReference.GetPlayerResponse TestHarness.SpaceServiceReference.SpaceServiceSoap.GetPlayer(TestHarness.SpaceServiceReference.GetPlayerRequest request) {
            return base.Channel.GetPlayer(request);
        }
        
        public string GetPlayer(int PlayerId) {
            TestHarness.SpaceServiceReference.GetPlayerRequest inValue = new TestHarness.SpaceServiceReference.GetPlayerRequest();
            inValue.Body = new TestHarness.SpaceServiceReference.GetPlayerRequestBody();
            inValue.Body.PlayerId = PlayerId;
            TestHarness.SpaceServiceReference.GetPlayerResponse retVal = ((TestHarness.SpaceServiceReference.SpaceServiceSoap)(this)).GetPlayer(inValue);
            return retVal.Body.GetPlayerResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TestHarness.SpaceServiceReference.GetPlayerResponse> TestHarness.SpaceServiceReference.SpaceServiceSoap.GetPlayerAsync(TestHarness.SpaceServiceReference.GetPlayerRequest request) {
            return base.Channel.GetPlayerAsync(request);
        }
        
        public System.Threading.Tasks.Task<TestHarness.SpaceServiceReference.GetPlayerResponse> GetPlayerAsync(int PlayerId) {
            TestHarness.SpaceServiceReference.GetPlayerRequest inValue = new TestHarness.SpaceServiceReference.GetPlayerRequest();
            inValue.Body = new TestHarness.SpaceServiceReference.GetPlayerRequestBody();
            inValue.Body.PlayerId = PlayerId;
            return ((TestHarness.SpaceServiceReference.SpaceServiceSoap)(this)).GetPlayerAsync(inValue);
        }
    }
}
