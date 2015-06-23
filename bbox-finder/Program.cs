using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProjApi;



namespace bbox_finder {


	class Program {


		private static double _Difference = Math.Abs( 20037508 * .000001 );

		static void Main( string[] args ) {

			Dictionary<string, string> projDefs = new Dictionary<string, string>();
			using (TextReader tw = new StreamReader( "epsg" )) {

				string line;
				while (!string.IsNullOrEmpty( line = tw.ReadLine() )) {

					line = line.Trim();
					if (line.StartsWith( "#" )) { continue; }

					int idx1 = line.IndexOf( "<" );
					int idx2 = line.IndexOf( ">" );

					string epsg = line.Substring( idx1 + 1, idx2 - (idx1 + 1) );
					string projDef = line.Substring( idx2 + 1, line.LastIndexOf( "<" ) - (idx2 + 1) ).Trim();

					projDefs.Add( epsg, projDef );
				}
			}


			string projDefWebMerc = projDefs["3857"];
			double llx = -20037508;
			double lly = -20037508;

			double[] x;
			double[] y;
			long prjIniErrorCnt = 0;
			long prjTransErrorCnt = 0;
			long prjBackErrorCnt = 0;

			using (Projection prjMercSrc = new Projection( projDefWebMerc )) {

				foreach (string epsg in projDefs.Keys) {

					try {
						using (Projection prjDest = new Projection( projDefs[epsg] )) {

							x = new double[] { llx };
							y = new double[] { lly };

							try {
								prjMercSrc.Transform( prjDest, x, y );
							}
							catch (Exception e2) {
								prjTransErrorCnt++;
								Console.WriteLine( "{0}: {1}", epsg, e2.ToString() );
								continue;
							}
							try {
								prjDest.Transform( prjMercSrc, x, y );
								if (!isEqual( x[0], llx ) || !isEqual( y[0], lly )) {
									prjBackErrorCnt++;
								}
							}
							catch (Exception e2) {
								prjBackErrorCnt++;
								Console.WriteLine( "{0}: {1}", epsg, e2.ToString() );
							}

						}
					}
					catch (Exception e) {
						prjIniErrorCnt++;
						Console.WriteLine( "{0}: {1}", epsg, e.ToString() );
					}
				}
			}

			Console.WriteLine(
				"{0} projs {1} ini errors {2} trans errors {3} back trans errors"
				, projDefs.Count
				, prjIniErrorCnt
				, prjTransErrorCnt
				, prjBackErrorCnt
			);
		}


		private static bool isEqual( double d1, double d2 ) {

			return (Math.Abs( d2 - d1 ) <= _Difference);
		}



	}
}
