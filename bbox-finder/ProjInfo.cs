using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace bbox_finder {


	public class ProjInfo {


		//KEEP ORDER OF PROPERTIES TO KEEP ToString() working as expected
		[Description( "EPSG" )]
		public string EPSG { get; set; }


		[Description( "AOK" )]
		public bool AllGreen {
			get {
				return
					initOk
					&& transLLOk
					&& transLROk
					&& transULOk
					&& transUROk
					&& backLLxOk
					&& backLLyOk
					&& backLRxOk
					&& backLRyOk
					&& backULxOk
					&& backULyOk
					&& backURxOk
					&& backURyOk;
			}
		}

		[Description( "IOK" )]
		public bool initOk { get; set; }

		[Description( "LLF" )]
		public bool transLLOk { get; set; }

		[Description( "ULF" )]
		public bool transULOk { get; set; }

		[Description( "URF" )]
		public bool transUROk { get; set; }


		[Description( "LRF" )]
		public bool transLROk { get; set; }


		[Description( "LLx" )]
		public bool backLLxOk { get; set; }
		[Description( "LLx" )]
		public bool backLLyOk { get; set; }

		[Description( "ULx" )]
		public bool backULxOk { get; set; }
		[Description( "ULx" )]
		public bool backULyOk { get; set; }

		[Description( "URx" )]
		public bool backURxOk { get; set; }
		[Description( "URy" )]
		public bool backURyOk { get; set; }


		[Description( "LRx" )]
		public bool backLRxOk { get; set; }
		[Description( "LRy" )]
		public bool backLRyOk { get; set; }

		[Description( "projdef" )]
		public string ProjDef { get; set; }


		public string Headers() {

			List<string> values = new List<string>();

			var props = this.GetType().GetProperties();
			foreach (var prop in props) {
				var attribs = prop.GetCustomAttributes( typeof( DescriptionAttribute ), false );
				foreach (DescriptionAttribute desc in attribs) {
					values.Add( desc.Description );
				}
			}

			return string.Join( "|", values.ToArray() );
		}


		public override string ToString() {

			List<string> values = new List<string>();

			var props = this.GetType().GetProperties();
			foreach (var prop in props) {
				object objVal = prop.GetValue( this, null );
				if (objVal is bool) {
					bool bVal = (bool)objVal;
					values.Add( bVal ? " X " : " - " );
				} else {
					values.Add( objVal.ToString() );
				}
			}

			return string.Join( "|", values.ToArray() );
		}


	}
}
