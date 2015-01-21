/// <summary>
/// Created by KirbyRawr(Jairo Baldán) in Monkimun Inc.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MailChimp;

namespace MailChimp.Demos {
	public class SetParametersMailchimp : MonoBehaviour {
		public MailchimpPlugin mailchimpPlugin;
		public string store;
		public string language;

		public bool SetPlayerPrefs;
		public bool SetParameters;
		
		void Update () {
		
			if(SetPlayerPrefs) {
				SetPlayerPrefs = false;
				setPlayerPrefs();
			}

			if(SetParameters) {
				SetParameters = false;
				getPlayerPrefs();
			}
		}

		void setPlayerPrefs() {
			PlayerPrefs.SetString("store", "apple");
			PlayerPrefs.SetString("language","spanish");
		}

		void getPlayerPrefs() {
			store = PlayerPrefs.GetString("store");
			language = PlayerPrefs.GetString("language");
			buildData();
		}

		void buildData() {

			List<MailchimpPlugin.OptionalParameter> optionalParameters = new List<MailchimpPlugin.OptionalParameter>(){new MailchimpPlugin.OptionalParameter("merge_vars", store, "store"), new MailchimpPlugin.OptionalParameter("merge_vars", language, "language")};
			setParameters(optionalParameters);
		}

		void setParameters(List<MailchimpPlugin.OptionalParameter> optionalParameters) {
			mailchimpPlugin.SetParameters(optionalParameters);
		}
	}
}