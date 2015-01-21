/// <summary>
/// Created by KirbyRawr(Jairo Baldán) in Monkimun Inc.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Linq;

namespace MailChimp {
	public class MailchimpPlugin : MonoBehaviour {

		public delegate void Callback(bool error);

		public string url = "https://dc.api.mailchimp.com/2.0/lists/subscribe.json";
		public List<RequiredParameter> requiredParameters = new List<RequiredParameter>();
		public List<OptionalParameter> optionalParameters = new List<OptionalParameter>();
		public bool debugMode;

		private string data;
		private Callback callback;

		[System.Serializable]
		public class RequiredParameter {
			public string parameterKey;
			public string parameterValue;
			public string parameterStruct;

			public RequiredParameter(string ParameterKey, string ParameterValue) {
				parameterKey = ParameterKey;
				parameterValue = ParameterValue;
			}

			public RequiredParameter(string ParameterKey, string ParameterValue, string ParameterStruct) {
				parameterKey = ParameterKey;
				parameterValue = ParameterValue;
				parameterStruct = ParameterStruct;
	        }
		}

		[System.Serializable]
		public class OptionalParameter {
			public string parameterKey;
			public string parameterValue;
			public string parameterStruct;

			public OptionalParameter(string ParameterKey, string ParameterValue) {
				parameterKey = ParameterKey;
				parameterValue = ParameterValue;
			}

			public OptionalParameter(string ParameterKey, string ParameterValue, string ParameterStruct) {
				parameterKey = ParameterKey;
				parameterValue = ParameterValue;
				parameterStruct = ParameterStruct;
			}
		}

		class OptionalParameterComparer : IEqualityComparer<OptionalParameter> {
			// Parameters are equal if their structs and parameter keys are equal.
			public bool Equals(OptionalParameter x, OptionalParameter y)
			{
				
				//Check whether the compared objects reference the same data.
				if (Object.ReferenceEquals(x, y)) return true;
				
				//Check whether any of the compared objects is null.
				if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
					return false;
				
				//Check whether the parameters properties are equal.
				return x.parameterStruct == y.parameterStruct && x.parameterKey == y.parameterKey;
			}
			
			// If Equals() returns true for a pair of objects 
			// then GetHashCode() must return the same value for these objects.
			
			public int GetHashCode(OptionalParameter optionalParameter)
			{
				//Check whether the object is null
				if (Object.ReferenceEquals(optionalParameter, null)) return 0;
				
				//Get hash code for the Name field if it is not null.
				int hashProductName = optionalParameter.parameterKey == null ? 0 : optionalParameter.parameterKey.GetHashCode();
				
				//Get hash code for the Code field.
				int hashProductCode = optionalParameter.parameterStruct.GetHashCode();
				
				//Calculate the hash code for the product.
				return hashProductName ^ hashProductCode;
			}
			
		}


		void Awake() {
			if(debugMode){
				//Debug List of Mailchimp.
				requiredParameters[1].parameterValue = "";

				if(string.IsNullOrEmpty(requiredParameters[1].parameterValue)){
					Debug.Log("Your Debug Key is empty add it the line upper this debug");
				}
			}
		}
		

		[ContextMenu("Required Variables")]
		void RequiredVariables() {
			url = "https://dc.api.mailchimp.com/2.0/lists/subscribe.json";
			requiredParameters = new List<RequiredParameter>(){new RequiredParameter("apikey", ""), new RequiredParameter("id", ""), new RequiredParameter("email", "", "email")};
		}

		[ContextMenu("Optional Variables")]
		void OptionalVariables() {
			optionalParameters = new List<OptionalParameter>(){new OptionalParameter("merge_vars", "", "")};
		}

		public void SubscribeEmail(string emailAddress, Callback tempCallback) {
			callback = tempCallback;
			if(!IsEmail(emailAddress)) { 
				Debug.Log(emailAddress);
				callback(true);
				return;
			}
			encodeData(emailAddress);
		}

		public void SetParameters(List<RequiredParameter> required) {
			required = requiredParameters.Union(required).ToList();
		}

		public void SetParameters(List<OptionalParameter> optional) {
			List<OptionalParameter> tempList = optional.Union(optionalParameters, new OptionalParameterComparer()).ToList();
			optionalParameters = tempList;
			Debug.Log(tempList.Count);
		}

		public void SetParameters(List<RequiredParameter> required, List<OptionalParameter> optional) {
			requiredParameters = requiredParameters.Union(required).ToList();
			optionalParameters = optionalParameters.Union(optional, new OptionalParameterComparer()).ToList();
		}

		void encodeData(string emailAddress){
			data = "";
			//Required Parameters (apikey, id, email)
			for (int r = 0; r < requiredParameters.Count; r++) {
				
				if(requiredParameters[r].parameterKey == "email") {
					if(!string.IsNullOrEmpty(emailAddress)){requiredParameters[r].parameterValue = emailAddress;}
					requiredParameters[r].parameterValue = requiredParameters[r].parameterValue.Replace("+", "%2b");
				}
				
				if(r == 0) {
					if(string.IsNullOrEmpty(requiredParameters[r].parameterStruct)) {
						data = data + requiredParameters[r].parameterKey + "=" + requiredParameters[r].parameterValue;
					} else {data = data + requiredParameters[r].parameterKey + "[" + requiredParameters[r].parameterStruct + "]"+"=" + requiredParameters[r].parameterValue;}
				}
				else {
					if(string.IsNullOrEmpty(requiredParameters[r].parameterStruct)) {
						data = data + "&" + requiredParameters[r].parameterKey + "=" + requiredParameters[r].parameterValue;
					} else {data = data + "&" + requiredParameters[r].parameterKey + "[" + requiredParameters[r].parameterStruct + "]"+"=" + requiredParameters[r].parameterValue;}
				}
			}
			
			//Optional Parameters (merged_vars, etc...)
			for (int o = 0; o < optionalParameters.Count; o++) {
				if(string.IsNullOrEmpty(optionalParameters[o].parameterStruct)) {
					data = data + "&" + optionalParameters[o].parameterKey + "=" + optionalParameters[o].parameterValue;
				} else {data = data + "&" + optionalParameters[o].parameterKey + "[" + optionalParameters[o].parameterStruct + "]"+"=" + optionalParameters[o].parameterValue;}
			} 
			
			UTF8Encoding utf8 = new UTF8Encoding(); 
			byte[] encodedData = utf8.GetBytes(data);
			
			StartCoroutine(postMail(url, encodedData));

		}

		bool IsEmail(string emailAddress) {

			if(!emailAddress.Contains("@") || string.IsNullOrEmpty(emailAddress)) {
				if(debugMode) {Debug.Log("The Email address: " + emailAddress + " is not valid.");}
				Debug.Log("Not Valid");
				return false;
			}

			try {
				MailAddress m = new MailAddress(emailAddress);
				m.ToString();
				if(debugMode) {Debug.Log("The Email address: " + emailAddress + " is valid.");}
				return true;
			}
			catch (UnityException){
				if(debugMode) {Debug.LogError("The Email address: " + emailAddress + " is not valid.");}
				return false;
			}
		}


		IEnumerator postMail(string postURL, byte[] postData) {

			WWW wwwPost = new WWW(postURL, postData);

			yield return wwwPost;

			if(!string.IsNullOrEmpty(wwwPost.error)) {
				callback(true);
				if(debugMode) {Debug.Log("Subscribe error: " + wwwPost.error);}
				yield break;
			} else {
				callback(false);
				if(debugMode) {Debug.Log("Subscribed to the list!");}
			}	
		}
	}
}