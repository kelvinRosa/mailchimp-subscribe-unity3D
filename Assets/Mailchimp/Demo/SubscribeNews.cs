/// <summary>
/// Created by KirbyRawr(Jairo Baldán) in Monkimun Inc.
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MailChimp;

namespace MailChimp.Demos {
	public class SubscribeNews : MonoBehaviour {

		public MailchimpPlugin mailchimpPlugin;
		public InputField input;
		public Text info;
		MailchimpPlugin.Callback callback;

		public void SubscribeOnClick() {
			mailchimpPlugin.SubscribeEmail(input.text, callback2);
		}

		void callback2(bool error) {
			if(error) {
				info.text = "Error";
			} else {
				info.text = "Done!";
			} 
		}
	}
}
