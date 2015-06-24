using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjApi;

namespace bbox_finder {

	public class ProjCheck : IDisposable {


		public ProjCheck(
			Projection webMercator
			, string epsg
			, string projDefDest
			, Extent extent
			, double tolerance
		) {

			_ProjWM = webMercator;
			_Extent = extent;
			_Tolerance = tolerance;

			_ProjInfo.EPSG = epsg;
			_ProjInfo.ProjDef = projDefDest;

			try {
				_ProjDest = new Projection( projDefDest );
				_ProjInfo.initOk = true;
			}
			catch {
				_InitFailed = true;
			}
		}

		~ProjCheck() { Dispose( false ); }

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool dispose ) {
			if (!_Disposed) {
				if (dispose) {
					if (null != _ProjDest) {
						_ProjDest.Dispose();
						_ProjDest = null;
					}
				}
				_Disposed = true;
			}
		}


		private Projection _ProjWM;
		private Projection _ProjDest;
		private Extent _Extent;
		private double _Tolerance;
		private bool _InitFailed;
		private ProjInfo _ProjInfo = new ProjInfo();
		private bool _Disposed;


		public ProjInfo DoTest() {

			if (_InitFailed) { return _ProjInfo; }

			TestVertexResult res = testVertex( _Extent.LL );
			_ProjInfo.transLLOk = res.Transform;
			_ProjInfo.backLLxOk = res.BackX;
			_ProjInfo.backLLyOk = res.BackY;

			res = testVertex( _Extent.UL );
			_ProjInfo.transULOk = res.Transform;
			_ProjInfo.backULxOk = res.BackX;
			_ProjInfo.backULyOk = res.BackY;

			res = testVertex( _Extent.UR );
			_ProjInfo.transUROk = res.Transform;
			_ProjInfo.backURxOk = res.BackX;
			_ProjInfo.backURyOk = res.BackY;

			res = testVertex( _Extent.LR );
			_ProjInfo.transLROk = res.Transform;
			_ProjInfo.backLRxOk = res.BackX;
			_ProjInfo.backLRyOk = res.BackY;


			return _ProjInfo;
		}


		private TestVertexResult testVertex( PointD pntWM ) {

			TestVertexResult retVal = new TestVertexResult();

			double[] x = new double[] { pntWM.X };
			double[] y = new double[] { pntWM.Y };

			//project from WebMerc to local
			try {
				_ProjWM.Transform( _ProjDest, x, y );
				retVal.Transform = true;
			}
			catch { return retVal; }

			//project from local back to WebMerc
			//and check if webm is within reasonable tolerance
			try {
				_ProjDest.Transform( _ProjWM, x, y );
				retVal.BackX = isEqual( pntWM.X, x[0] );
				retVal.BackY = isEqual( pntWM.Y, y[0] );
			}
			catch { return retVal; }

			return retVal;
		}


		private bool isEqual( double d1, double d2 ) {
			return (Math.Abs( d2 - d1 ) <= _Tolerance);
		}

	}
}
