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
			double webMercMax = 20037508;
			double tolerance = 10;

			Extent extent = new Extent(
				-webMercMax
				, -webMercMax
				, -webMercMax
				, webMercMax
				, webMercMax
				, webMercMax
				, webMercMax
				, -webMercMax
			);

			List<ProjInfo> projInfos = new List<ProjInfo>();

			using (Projection prjMercSrc = new Projection( projDefWebMerc )) {
				foreach (string epsg in projDefs.Keys) {
					using (ProjCheck check = new ProjCheck( prjMercSrc, epsg, projDefs[epsg], extent, tolerance )) {
						projInfos.Add( check.DoTest() );
					}
				}
			}


			long projs = projInfos.Count();
			long allGreen = projInfos.Where( i => i.AllGreen ).Count();
			long iniOk = projInfos.Where( i => i.initOk ).Count();
			long iniFail = projInfos.Where( i => !i.initOk ).Count();
			string[] initFailEpsg = projInfos.Where( i => !i.initOk ).Select( i => i.EPSG ).ToArray();

			var transFailLL = projInfos.Where( i => !i.transLLOk );
			var transFailUL = projInfos.Where( i => !i.transULOk );
			var transFailUR = projInfos.Where( i => !i.transUROk );
			var transFailLR = projInfos.Where( i => !i.transLROk );

			long transOkLL = projInfos.Count( i => i.transLLOk == true );
			long transOkUL = projInfos.Count( i => i.transULOk == true );
			long transOkUR = projInfos.Count( i => i.transUROk == true );
			long transOkLR = projInfos.Count( i => i.transLROk == true );

			long backFailLLx = projInfos.Count( i => !i.backLLxOk );
			long backFailLLy = projInfos.Count( i => !i.backLLyOk );
			long backFailULx = projInfos.Count( i => !i.backULxOk );
			long backFailULy = projInfos.Count( i => !i.backULyOk );
			long backFailURx = projInfos.Count( i => !i.backULxOk );
			long backFailURy = projInfos.Count( i => !i.backULyOk );
			long backFailLRx = projInfos.Count( i => !i.backLRxOk );
			long backFailLRy = projInfos.Count( i => !i.backLRyOk );

			long backOkLLx = projInfos.Count( i => i.backLLxOk );
			long backOkLLy = projInfos.Count( i => i.backLLyOk );
			long backOkULx = projInfos.Count( i => i.backULxOk );
			long backOkULy = projInfos.Count( i => i.backULyOk );
			long backOkURx = projInfos.Count( i => i.backULxOk );
			long backOkURy = projInfos.Count( i => i.backULyOk );
			long backOkLRx = projInfos.Count( i => i.backLRxOk );
			long backOkLRy = projInfos.Count( i => i.backLRyOk );


			writeToFile( "trans-failed-LL.txt", transFailLL.Select( i => i.EPSG ).ToArray() );
			writeToFile( "trans-failed-UL.txt", transFailUL.Select( i => i.EPSG ).ToArray() );
			writeToFile( "trans-failed-UR.txt", transFailUR.Select( i => i.EPSG ).ToArray() );
			writeToFile( "trans-failed-LR.txt", transFailLR.Select( i => i.EPSG ).ToArray() );


			Console.WriteLine( "backward transformation tolerance: {0}m", tolerance );
			Console.WriteLine( "bbox:{0}{1}", Environment.NewLine, extent );

			Console.WriteLine( "# of projections tested: {0}", projs );
			Console.WriteLine( "[AOK] everything successful: {0}", allGreen );
			Console.WriteLine( "[IOK] pj_init successful: {0}", iniOk );
			Console.WriteLine( "# of pj_init failed: {0} (EPSG: {1})", iniFail, string.Join( ",", initFailEpsg ) );
			Console.WriteLine( "# of failed forward transformations:" );
			Console.WriteLine( "LL: {0}", transFailLL.Count() );
			Console.WriteLine( "UL: {0}", transFailUL.Count() );
			Console.WriteLine( "UR: {0}", transFailUR.Count() );
			Console.WriteLine( "LR: {0}", transFailLR.Count() );
			Console.WriteLine( "# of successful forward transformations:" );
			Console.WriteLine( "[LLF] LL: {0}", transOkLL );
			Console.WriteLine( "[ULF] UL: {0}", transOkUL );
			Console.WriteLine( "[URF] UR: {0}", transOkUR );
			Console.WriteLine( "[LRF] LR: {0}", transOkLR );
			Console.WriteLine( "# of failed back transformations within tolerance: {0}", tolerance );
			Console.WriteLine( "LLx: {0}", backFailLLx );
			Console.WriteLine( "LLy: {0}", backFailLLy );
			Console.WriteLine( "ULx: {0}", backFailULx );
			Console.WriteLine( "ULy: {0}", backFailULy );
			Console.WriteLine( "URx: {0}", backFailURx );
			Console.WriteLine( "URy: {0}", backFailURy );
			Console.WriteLine( "LRx: {0}", backFailLRx );
			Console.WriteLine( "LRy: {0}", backFailLRy );
			Console.WriteLine( "# of successful back transformations within tolerace {0}:", tolerance );
			Console.WriteLine( "[LLx]: {0}", backOkLLx );
			Console.WriteLine( "[LLy]: {0}", backOkLLy );
			Console.WriteLine( "[ULx]: {0}", backOkULx );
			Console.WriteLine( "[ULy]: {0}", backOkULy );
			Console.WriteLine( "[URx]: {0}", backOkURx );
			Console.WriteLine( "[URy]: {0}", backOkURy );
			Console.WriteLine( "[LRx]: {0}", backOkLRx );
			Console.WriteLine( "[LRy]: {0}", backOkLRy );

			Console.WriteLine( "=============================================================" );
			Console.WriteLine( projInfos[0].Headers() );
			foreach (var pi in projInfos) {
				Console.WriteLine( pi );
			}

		}

		private static void writeToFile( string fileName, string[] rows ) {
			using (TextWriter tw = new StreamWriter( fileName, false )) {
				tw.WriteLine( string.Join( Environment.NewLine, rows ) );
			}
		}



	}
}
