using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bbox_finder {
	public class Extent {


		public Extent(
			double llx
			, double lly
			, double ulx
			, double uly
			, double urx
			, double ury
			, double lrx
			, double lry
		) {

			LL = new PointD() { X = llx, Y = lly };
			UL = new PointD() { X = ulx, Y = uly };
			UR = new PointD() { X = urx, Y = ury };
			LR = new PointD() { X = lrx, Y = lry };

		}


		public PointD LL { get; set; }
		public PointD UL { get; set; }
		public PointD UR { get; set; }
		public PointD LR { get; set; }


		public override string ToString() {
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat(
				"{0}/{1}\t{2}/{3}{4}"
				, UL.X
				, UL.Y
				, UR.X
				, UR.Y
				, Environment.NewLine
			);
			sb.AppendFormat(
				"{0}/{1}\t{2}/{3}{4}"
				, LL.X
				, LL.Y
				, LR.X
				, LR.Y
				, Environment.NewLine
			);

			return sb.ToString();
		}


	}
}
