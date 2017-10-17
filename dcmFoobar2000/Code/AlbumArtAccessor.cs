using System;
using System.Drawing;

namespace dcmFoobar2000.Code
{
    public class AlbumArtAccessor : IDisposable
    {
        private Bitmap _bitmap = null;
        private bool _stub = true;
        private object _locker = new object();

        public void Dispose()
        {
            lock (_locker)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
            }
        }

        /// <summary>
        /// Returns data with bitmap instance stored in accessor (or null). Do not dispose this instance.
        /// </summary>
        public AlbumArtBitmapData GetBitmapData()
        {
            lock (_locker)
            {
                return new AlbumArtBitmapData(_bitmap, _stub);
            }
        }

        /// <summary>
        /// Sets bitmap instance. Accessor is now responsible for this instance, do not dispose it.
        /// </summary>
        public void SetBitmap(Bitmap bmp, bool stub)
        {
            lock (_locker)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                }
                _bitmap = bmp;
                _stub = stub;
            }
        }
    }

    public class AlbumArtBitmapData
    {
        public Bitmap Bitmap { get; private set; }
        public bool IsStub { get; private set; }
        public AlbumArtBitmapData(Bitmap bmp, bool stub)
        {
            Bitmap = bmp;
            IsStub = stub;
        }
    }

}
